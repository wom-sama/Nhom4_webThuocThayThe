using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Nhom4WebThuocThayThe.Data;
using Nhom4WebThuocThayThe.Models;
using Nhom4WebThuocThayThe.Services;

namespace Nhom4WebThuocThayThe.IntegrationTests;

public sealed class SqlServerPersistenceTests : IClassFixture<SqlServerFixture>
{
    private readonly SqlServerFixture _fixture;

    public SqlServerPersistenceTests(SqlServerFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task MigrationAndSeed_CreateUsableDatabase()
    {
        await using var db = _fixture.CreateContext();

        Assert.True(await db.Database.CanConnectAsync());
        Assert.NotEmpty(await db.Database.GetAppliedMigrationsAsync());
        Assert.True(await db.Drugs.CountAsync() >= 7);
        Assert.NotEmpty(await db.Batches.ToListAsync());
    }

    [Fact]
    public async Task SavedAuditLog_SurvivesNewContext()
    {
        var marker = $"integration-{Guid.NewGuid():N}";

        await using (var writeDb = _fixture.CreateContext())
        {
            var nextId = await writeDb.AuditLogs.MaxAsync(item => (int?)item.Id) + 1 ?? 1;
            writeDb.AuditLogs.Add(new AuditLogEntry
            {
                Id = nextId,
                Actor = "integration-test",
                Action = "PersistenceProbe",
                Entity = "Database",
                Detail = marker,
                CreatedAt = DateTimeOffset.UtcNow
            });
            await writeDb.SaveChangesAsync();
        }

        await using var readDb = _fixture.CreateContext();
        Assert.True(await readDb.AuditLogs.AnyAsync(item => item.Detail == marker));
    }

    [Fact]
    public async Task SearchService_ExecutesAgainstSqlServer()
    {
        using var db = _fixture.CreateContext();
        var inventory = new InventoryService(db);
        var recommendation = new RecommendationService(db);
        var search = new DrugSearchService(db, inventory, recommendation);

        var result = await search.SearchAsync("paracetamol", categoryId: null);

        Assert.NotEmpty(result.Results);
        Assert.Contains(result.Results, item => item.Name.Contains("Para", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ReportingDashboard_WorksWithoutMultipleActiveResultSets()
    {
        using var db = _fixture.CreateContext();
        var inventory = new InventoryService(db);
        var recommendation = new RecommendationService(db);
        var audit = new AuditLogService(db);
        var reporting = new ReportingService(db, inventory, recommendation, audit);

        var dashboard = reporting.GetDashboard();

        Assert.Equal(4, dashboard.Metrics.Count);
        Assert.NotEmpty(dashboard.StockRisks);
    }

    [Fact]
    public void ManagedAccount_CanRegisterLockUnlockAndResetPassword()
    {
        using var db = _fixture.CreateContext();
        var service = new UserAccountService(
            db,
            new SeedUserAccountStore(InMemoryUserAccountService.CreateDemoUsers()),
            new AuditLogService(db));
        var email = $"integration-{Guid.NewGuid():N}@example.test";

        var created = service.RegisterUser("Integration User", email, "Initial@123");
        Assert.True(created.IsSuccess, created.Message);
        Assert.Equal(AppRoles.User, service.ValidateCredentials(email, "Initial@123")?.Role);

        Assert.True(service.SetLocked(email, true, "integration").IsSuccess);
        Assert.Null(service.ValidateCredentials(email, "Initial@123"));

        Assert.True(service.SetLocked(email, false, "integration").IsSuccess);
        Assert.True(service.ResetPassword(email, "Changed@123", "integration").IsSuccess);
        Assert.Null(service.ValidateCredentials(email, "Initial@123"));
        Assert.Equal(AppRoles.User, service.ValidateCredentials(email, "Changed@123")?.Role);
    }

    [Fact]
    public async Task UserLibrary_PersistsSearchHistoryAndSavedDrug()
    {
        await using var db = _fixture.CreateContext();
        var service = new UserLibraryService(db);
        var email = $"library-{Guid.NewGuid():N}@example.test";

        await service.RecordSearchAsync(email, "Panadol", categoryId: null, resultCount: 2);
        await service.SaveDrugAsync(email, drugId: 1);

        var summary = await service.GetSummaryAsync(email);
        var history = await service.GetHistoryAsync(email);
        var saved = await service.GetSavedDrugsAsync(email);

        Assert.Equal(1, summary.SearchCount);
        Assert.Equal(1, summary.SavedDrugCount);
        Assert.Contains(history, item => item.Keyword == "Panadol" && item.ResultCount == 2);
        Assert.Contains(saved, item => item.DrugId == 1);

        await service.RemoveSavedDrugAsync(email, drugId: 1);
        Assert.False(await service.IsSavedAsync(email, drugId: 1));
    }
}

public sealed class SqlServerFixture : IAsyncLifetime
{
    private readonly string _databaseName = $"N4WTT_Integration_{Guid.NewGuid():N}";
    private readonly string _serverConnectionString =
        Environment.GetEnvironmentVariable("N4WTT_TEST_SQLSERVER") ??
        "Server=(localdb)\\MSSQLLocalDB;Database=master;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";

    public PharmacyDbContext CreateContext()
    {
        var connectionString = new SqlConnectionStringBuilder(_serverConnectionString)
        {
            InitialCatalog = _databaseName,
            TrustServerCertificate = true,
            MultipleActiveResultSets = false
        }.ConnectionString;
        var options = new DbContextOptionsBuilder<PharmacyDbContext>()
            .UseSqlServer(connectionString)
            .Options;
        return new PharmacyDbContext(options);
    }

    public Task InitializeAsync()
    {
        using var db = CreateContext();
        PharmacyDbInitializer.Initialize(db);
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await using var db = CreateContext();
        await db.Database.EnsureDeletedAsync();
    }
}
