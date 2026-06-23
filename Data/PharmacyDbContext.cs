using Microsoft.EntityFrameworkCore;
using Nhom4WebThuocThayThe.Models;

namespace Nhom4WebThuocThayThe.Data;

public sealed class PharmacyDbContext(DbContextOptions<PharmacyDbContext> options) : DbContext(options)
{
    public DbSet<DrugCategory> Categories => Set<DrugCategory>();

    public DbSet<DosageForm> DosageForms => Set<DosageForm>();

    public DbSet<MeasurementUnit> Units => Set<MeasurementUnit>();

    public DbSet<Manufacturer> Manufacturers => Set<Manufacturer>();

    public DbSet<ActiveIngredient> ActiveIngredients => Set<ActiveIngredient>();

    public DbSet<Drug> Drugs => Set<Drug>();

    public DbSet<DrugActiveIngredient> DrugActiveIngredients => Set<DrugActiveIngredient>();

    public DbSet<Warehouse> Warehouses => Set<Warehouse>();

    public DbSet<DrugBatch> Batches => Set<DrugBatch>();

    public DbSet<PatientSafetyProfile> PatientSafetyProfiles => Set<PatientSafetyProfile>();

    public DbSet<ExternalDataSource> ExternalDataSources => Set<ExternalDataSource>();

    public DbSet<AuditLogEntry> AuditLogs => Set<AuditLogEntry>();

    public DbSet<ExpertReviewItem> ExpertReviews => Set<ExpertReviewItem>();

    public DbSet<RegisteredUserAccount> RegisteredUserAccounts => Set<RegisteredUserAccount>();

    public DbSet<UserSearchHistory> UserSearchHistories => Set<UserSearchHistory>();

    public DbSet<SavedDrug> SavedDrugs => Set<SavedDrug>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DrugCategory>().Property(item => item.Id).ValueGeneratedNever();
        modelBuilder.Entity<DosageForm>().Property(item => item.Id).ValueGeneratedNever();
        modelBuilder.Entity<MeasurementUnit>().Property(item => item.Id).ValueGeneratedNever();
        modelBuilder.Entity<Manufacturer>().Property(item => item.Id).ValueGeneratedNever();
        modelBuilder.Entity<ActiveIngredient>().Property(item => item.Id).ValueGeneratedNever();
        modelBuilder.Entity<Drug>().Property(item => item.Id).ValueGeneratedNever();
        modelBuilder.Entity<Warehouse>().Property(item => item.Id).ValueGeneratedNever();
        modelBuilder.Entity<DrugBatch>().Property(item => item.Id).ValueGeneratedNever();
        modelBuilder.Entity<ExternalDataSource>().Property(item => item.Id).ValueGeneratedNever();
        modelBuilder.Entity<AuditLogEntry>().Property(item => item.Id).ValueGeneratedNever();
        modelBuilder.Entity<ExpertReviewItem>().Property(item => item.Id).ValueGeneratedNever();

        modelBuilder.Entity<DrugActiveIngredient>()
            .HasKey(item => new { item.DrugId, item.ActiveIngredientId });
        modelBuilder.Entity<PatientSafetyProfile>().HasKey(item => item.Email);
        modelBuilder.Entity<RegisteredUserAccount>().HasKey(item => item.Email);

        modelBuilder.Entity<Drug>().Property(item => item.Price).HasPrecision(18, 2);
        modelBuilder.Entity<Drug>().HasIndex(item => item.Name);
        modelBuilder.Entity<DrugBatch>().HasIndex(item => new { item.DrugId, item.ExpiryDate });
        modelBuilder.Entity<AuditLogEntry>().HasIndex(item => item.CreatedAt);
        modelBuilder.Entity<RegisteredUserAccount>().HasIndex(item => item.Role);
        modelBuilder.Entity<UserSearchHistory>().HasIndex(item => new { item.UserEmail, item.SearchedAt });
        modelBuilder.Entity<SavedDrug>().HasIndex(item => new { item.UserEmail, item.DrugId }).IsUnique();

        modelBuilder.Entity<Drug>()
            .HasOne<DrugCategory>()
            .WithMany()
            .HasForeignKey(item => item.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Drug>()
            .HasOne<DosageForm>()
            .WithMany()
            .HasForeignKey(item => item.DosageFormId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Drug>()
            .HasOne<MeasurementUnit>()
            .WithMany()
            .HasForeignKey(item => item.UnitId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Drug>()
            .HasOne<Manufacturer>()
            .WithMany()
            .HasForeignKey(item => item.ManufacturerId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<DrugActiveIngredient>()
            .HasOne<Drug>()
            .WithMany()
            .HasForeignKey(item => item.DrugId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<DrugActiveIngredient>()
            .HasOne<ActiveIngredient>()
            .WithMany()
            .HasForeignKey(item => item.ActiveIngredientId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<DrugBatch>()
            .HasOne<Drug>()
            .WithMany()
            .HasForeignKey(item => item.DrugId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<DrugBatch>()
            .HasOne<Warehouse>()
            .WithMany()
            .HasForeignKey(item => item.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<ExpertReviewItem>()
            .HasOne<Drug>()
            .WithMany()
            .HasForeignKey(item => item.SourceDrugId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<ExpertReviewItem>()
            .HasOne<Drug>()
            .WithMany()
            .HasForeignKey(item => item.RecommendedDrugId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<SavedDrug>()
            .HasOne<Drug>()
            .WithMany()
            .HasForeignKey(item => item.DrugId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<UserSearchHistory>()
            .HasOne<DrugCategory>()
            .WithMany()
            .HasForeignKey(item => item.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
