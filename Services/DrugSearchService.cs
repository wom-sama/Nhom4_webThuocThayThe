using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Nhom4WebThuocThayThe.Data;
using Nhom4WebThuocThayThe.ViewModels.Catalog;
using Nhom4WebThuocThayThe.ViewModels.Search;

namespace Nhom4WebThuocThayThe.Services;

public sealed class DrugSearchService(
    PharmacyDbContext dbContext,
    IInventoryService inventoryService,
    IRecommendationService recommendationService) : IDrugSearchService
{
    public async Task<DrugSearchPageViewModel> SearchAsync(string? keyword, int? categoryId)
    {
        var normalizedKeyword = keyword?.Trim();
        var categories = await dbContext.Categories
            .AsNoTracking()
            .OrderBy(category => category.Name)
            .ToListAsync();
        var query = dbContext.Drugs.AsNoTracking().AsQueryable();

        if (categoryId is not null && categories.Any(item => item.Id == categoryId.Value))
        {
            query = query.Where(drug => drug.CategoryId == categoryId.Value);
        }

        if (!string.IsNullOrWhiteSpace(normalizedKeyword))
        {
            query = query.Where(drug =>
                drug.Name.Contains(normalizedKeyword) ||
                dbContext.Categories.Any(category => category.Id == drug.CategoryId && category.Name.Contains(normalizedKeyword)) ||
                dbContext.Manufacturers.Any(manufacturer => manufacturer.Id == drug.ManufacturerId && manufacturer.Name.Contains(normalizedKeyword)) ||
                dbContext.DrugActiveIngredients.Any(link =>
                    link.DrugId == drug.Id &&
                    dbContext.ActiveIngredients.Any(ingredient => ingredient.Id == link.ActiveIngredientId && ingredient.Name.Contains(normalizedKeyword))));
        }

        var drugs = await query.OrderBy(drug => drug.Name).ToListAsync();
        var categoryNames = categories.ToDictionary(item => item.Id, item => item.Name);
        var dosageForms = await dbContext.DosageForms.AsNoTracking().ToDictionaryAsync(item => item.Id, item => item.Name);
        var manufacturers = await dbContext.Manufacturers.AsNoTracking().ToDictionaryAsync(item => item.Id, item => item.Name);
        var today = DateOnly.FromDateTime(DateTime.Today);
        var stockByDrug = await dbContext.Batches
            .AsNoTracking()
            .Where(batch => batch.Quantity > 0 && batch.ExpiryDate >= today)
            .GroupBy(batch => batch.DrugId)
            .Select(group => new { DrugId = group.Key, Quantity = group.Sum(batch => batch.Quantity) })
            .ToDictionaryAsync(item => item.DrugId, item => item.Quantity);
        var results = drugs.Select(drug => new DrugListItemViewModel
        {
            Id = drug.Id,
            Name = drug.Name,
            Strength = drug.Strength,
            Price = drug.Price,
            Category = categoryNames[drug.CategoryId],
            DosageForm = dosageForms[drug.DosageFormId],
            Manufacturer = manufacturers[drug.ManufacturerId],
            PrescriptionRequired = drug.PrescriptionRequired,
            IsActive = drug.IsActive,
            StockQuantity = stockByDrug.GetValueOrDefault(drug.Id)
        }).ToList();

        return new DrugSearchPageViewModel
        {
            Keyword = normalizedKeyword,
            CategoryId = categoryId,
            Categories = categories
                .Select(category => new SelectListItem(category.Name, category.Id.ToString(), category.Id == categoryId))
                .ToList(),
            Results = results
        };
    }

    public async Task<DrugDetailViewModel?> GetDetailAsync(int id, string? userEmail)
    {
        var drug = await dbContext.Drugs.AsNoTracking().FirstOrDefaultAsync(item => item.Id == id);
        if (drug is null)
        {
            return null;
        }

        var ingredientLink = await dbContext.DrugActiveIngredients
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.DrugId == drug.Id);
        var ingredient = ingredientLink is null
            ? null
            : await dbContext.ActiveIngredients.AsNoTracking().FirstOrDefaultAsync(item => item.Id == ingredientLink.ActiveIngredientId);
        var profile = string.IsNullOrWhiteSpace(userEmail)
            ? null
            : await dbContext.PatientSafetyProfiles.AsNoTracking().FirstOrDefaultAsync(item => item.Email == userEmail);
        var category = await dbContext.Categories.AsNoTracking().FirstAsync(item => item.Id == drug.CategoryId);
        var dosageForm = await dbContext.DosageForms.AsNoTracking().FirstAsync(item => item.Id == drug.DosageFormId);
        var unit = await dbContext.Units.AsNoTracking().FirstAsync(item => item.Id == drug.UnitId);
        var manufacturer = await dbContext.Manufacturers.AsNoTracking().FirstAsync(item => item.Id == drug.ManufacturerId);
        var stockQuantity = await inventoryService.GetAvailableQuantityAsync(drug.Id);
        var alternatives = await recommendationService.GetRecommendationsAsync(drug.Id, userEmail);

        return new DrugDetailViewModel
        {
            Id = drug.Id,
            Name = drug.Name,
            Strength = drug.Strength,
            Price = drug.Price,
            Category = category.Name,
            DosageForm = dosageForm.Name,
            Unit = unit.Name,
            Manufacturer = manufacturer.Name,
            ActiveIngredient = ingredient?.Name ?? "Chưa khai báo",
            ActiveIngredientWarning = ingredient?.Warning,
            StockQuantity = stockQuantity,
            PrescriptionRequired = drug.PrescriptionRequired,
            Description = drug.Description,
            Usage = drug.Usage,
            Contraindications = drug.Contraindications,
            SafetyProfileName = profile?.DisplayName,
            SafetyProfileNote = profile?.ClinicalNote,
            Alternatives = alternatives
        };
    }

}
