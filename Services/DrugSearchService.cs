using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Nhom4WebThuocThayThe.Data;
using Nhom4WebThuocThayThe.ViewModels.Catalog;
using Nhom4WebThuocThayThe.ViewModels.Search;

namespace Nhom4WebThuocThayThe.Services;

public sealed class DrugSearchService(
    PharmacyDbContext dbContext,
    IDrugCatalogService catalogService,
    IInventoryService inventoryService,
    IRecommendationService recommendationService) : IDrugSearchService
{
    public DrugSearchPageViewModel Search(string? keyword, int? categoryId)
    {
        var normalizedKeyword = keyword?.Trim();
        var query = catalogService.GetDrugs().AsEnumerable();

        if (categoryId is not null)
        {
            var category = dbContext.Categories
                .AsNoTracking()
                .FirstOrDefault(item => item.Id == categoryId.Value);
            if (category is not null)
            {
                query = query.Where(item => item.Category == category.Name);
            }
        }

        if (!string.IsNullOrWhiteSpace(normalizedKeyword))
        {
            query = query.Where(item => Matches(item, normalizedKeyword));
        }

        return new DrugSearchPageViewModel
        {
            Keyword = normalizedKeyword,
            CategoryId = categoryId,
            Categories = dbContext.Categories
                .AsNoTracking()
                .OrderBy(category => category.Name)
                .Select(category => new SelectListItem(category.Name, category.Id.ToString(), category.Id == categoryId))
                .ToList(),
            Results = query.ToList()
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

    private bool Matches(DrugListItemViewModel item, string keyword)
    {
        var drug = dbContext.Drugs.AsNoTracking().First(current => current.Id == item.Id);
        var ingredientIds = dbContext.DrugActiveIngredients
            .AsNoTracking()
            .Where(link => link.DrugId == drug.Id)
            .Select(link => link.ActiveIngredientId)
            .ToHashSet();
        var activeIngredientNames = dbContext.ActiveIngredients
            .AsNoTracking()
            .Where(ingredient => ingredientIds.Contains(ingredient.Id))
            .Select(ingredient => ingredient.Name)
            .ToList();

        return Contains(item.Name, keyword) ||
               Contains(item.Category, keyword) ||
               Contains(item.Manufacturer, keyword) ||
               activeIngredientNames.Any(name => Contains(name, keyword));
    }

    private static bool Contains(string value, string keyword)
    {
        return value.Contains(keyword, StringComparison.OrdinalIgnoreCase);
    }

}
