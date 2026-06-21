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
            MultipleActiveResultSets = true
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
