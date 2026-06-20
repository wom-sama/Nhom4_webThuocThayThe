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

        modelBuilder.Entity<Drug>().Property(item => item.Price).HasPrecision(18, 2);
        modelBuilder.Entity<Drug>().HasIndex(item => item.Name);
        modelBuilder.Entity<DrugBatch>().HasIndex(item => new { item.DrugId, item.ExpiryDate });
        modelBuilder.Entity<AuditLogEntry>().HasIndex(item => item.CreatedAt);

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
    }
}
