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
    public DrugSearchPageViewModel Search(string? keyword, int? categoryId)
    {
        var normalizedKeyword = keyword?.Trim();
        var categories = dbContext.Categories
            .AsNoTracking()
            .OrderBy(category => category.Name)
            .ToList();
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

        var drugs = query.OrderBy(drug => drug.Name).ToList();
        var categoryNames = categories.ToDictionary(item => item.Id, item => item.Name);
        var dosageForms = dbContext.DosageForms.AsNoTracking().ToDictionary(item => item.Id, item => item.Name);
        var manufacturers = dbContext.Manufacturers.AsNoTracking().ToDictionary(item => item.Id, item => item.Name);
        var today = DateOnly.FromDateTime(DateTime.Today);
        var stockByDrug = dbContext.Batches
            .AsNoTracking()
            .Where(batch => batch.Quantity > 0 && batch.ExpiryDate >= today)
            .GroupBy(batch => batch.DrugId)
            .Select(group => new { DrugId = group.Key, Quantity = group.Sum(batch => batch.Quantity) })
            .ToDictionary(item => item.DrugId, item => item.Quantity);
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

    public DrugDetailViewModel? GetDetail(int id, string? userEmail)
    {
        var drug = dbContext.Drugs.AsNoTracking().FirstOrDefault(item => item.Id == id);
        if (drug is null)
        {
            return null;
        }

        var ingredientLink = dbContext.DrugActiveIngredients
            .AsNoTracking()
            .FirstOrDefault(item => item.DrugId == drug.Id);
        var ingredient = ingredientLink is null
            ? null
            : dbContext.ActiveIngredients.AsNoTracking().FirstOrDefault(item => item.Id == ingredientLink.ActiveIngredientId);
        var profile = string.IsNullOrWhiteSpace(userEmail)
            ? null
            : dbContext.PatientSafetyProfiles.AsNoTracking().FirstOrDefault(item => item.Email == userEmail);

        return new DrugDetailViewModel
        {
            Id = drug.Id,
            Name = drug.Name,
            Strength = drug.Strength,
            Price = drug.Price,
            Category = dbContext.Categories.AsNoTracking().First(category => category.Id == drug.CategoryId).Name,
            DosageForm = dbContext.DosageForms.AsNoTracking().First(form => form.Id == drug.DosageFormId).Name,
            Unit = dbContext.Units.AsNoTracking().First(unit => unit.Id == drug.UnitId).Name,
            Manufacturer = dbContext.Manufacturers.AsNoTracking().First(manufacturer => manufacturer.Id == drug.ManufacturerId).Name,
            ActiveIngredient = ingredient?.Name ?? "Chua khai bao",
            ActiveIngredientWarning = ingredient?.Warning,
            StockQuantity = inventoryService.GetAvailableQuantity(drug.Id),
            PrescriptionRequired = drug.PrescriptionRequired,
            Description = drug.Description,
            Usage = drug.Usage,
            Contraindications = drug.Contraindications,
            SafetyProfileName = profile?.DisplayName,
            SafetyProfileNote = profile?.ClinicalNote,
            Alternatives = recommendationService.GetRecommendations(drug.Id, userEmail)
        };
    }

}
