using Microsoft.EntityFrameworkCore;

namespace Nhom4WebThuocThayThe.Data;

public static class PharmacyDbInitializer
{
    public static void Initialize(PharmacyDbContext dbContext)
    {
        dbContext.Database.Migrate();

        var seed = new InMemoryPharmacyStore();
        AddMissing(dbContext.Categories, seed.Categories, item => item.Id);
        AddMissing(dbContext.DosageForms, seed.DosageForms, item => item.Id);
        AddMissing(dbContext.Units, seed.Units, item => item.Id);
        AddMissing(dbContext.Manufacturers, seed.Manufacturers, item => item.Id);
        AddMissing(dbContext.ActiveIngredients, seed.ActiveIngredients, item => item.Id);
        AddMissing(dbContext.Warehouses, seed.Warehouses, item => item.Id);
        AddMissing(dbContext.Drugs, seed.Drugs, item => item.Id);
        AddMissing(dbContext.DrugActiveIngredients, seed.DrugActiveIngredients, item => (item.DrugId, item.ActiveIngredientId));
        AddMissing(dbContext.Batches, seed.Batches, item => item.Id);
        AddMissing(dbContext.PatientSafetyProfiles, seed.PatientSafetyProfiles, item => item.Email);
        AddMissing(dbContext.ExternalDataSources, seed.ExternalDataSources, item => item.Id);
        AddMissing(dbContext.AuditLogs, seed.AuditLogs, item => item.Id);
        AddMissing(dbContext.ExpertReviews, seed.ExpertReviews, item => item.Id);
        dbContext.SaveChanges();
    }

    private static void AddMissing<TEntity, TKey>(
        DbSet<TEntity> dbSet,
        IEnumerable<TEntity> seedItems,
        Func<TEntity, TKey> keySelector)
        where TEntity : class
        where TKey : notnull
    {
        var existingKeys = dbSet
            .AsNoTracking()
            .AsEnumerable()
            .Select(keySelector)
            .ToHashSet();

        foreach (var item in seedItems)
        {
            if (existingKeys.Add(keySelector(item)))
            {
                dbSet.Add(item);
            }
        }
    }
}
