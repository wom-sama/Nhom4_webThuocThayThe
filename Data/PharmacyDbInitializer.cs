using Microsoft.EntityFrameworkCore;

namespace Nhom4WebThuocThayThe.Data;

public static class PharmacyDbInitializer
{
    public static void Initialize(PharmacyDbContext dbContext)
    {
        dbContext.Database.Migrate();

        if (dbContext.Drugs.Any())
        {
            return;
        }

        var seed = new InMemoryPharmacyStore();
        dbContext.Categories.AddRange(seed.Categories);
        dbContext.DosageForms.AddRange(seed.DosageForms);
        dbContext.Units.AddRange(seed.Units);
        dbContext.Manufacturers.AddRange(seed.Manufacturers);
        dbContext.ActiveIngredients.AddRange(seed.ActiveIngredients);
        dbContext.Warehouses.AddRange(seed.Warehouses);
        dbContext.Drugs.AddRange(seed.Drugs);
        dbContext.DrugActiveIngredients.AddRange(seed.DrugActiveIngredients);
        dbContext.Batches.AddRange(seed.Batches);
        dbContext.PatientSafetyProfiles.AddRange(seed.PatientSafetyProfiles);
        dbContext.ExternalDataSources.AddRange(seed.ExternalDataSources);
        dbContext.AuditLogs.AddRange(seed.AuditLogs);
        dbContext.ExpertReviews.AddRange(seed.ExpertReviews);
        dbContext.SaveChanges();
    }
}
