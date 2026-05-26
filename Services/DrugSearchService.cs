using Microsoft.AspNetCore.Mvc.Rendering;
using Nhom4WebThuocThayThe.Data;
using Nhom4WebThuocThayThe.ViewModels.Catalog;
using Nhom4WebThuocThayThe.ViewModels.Search;

namespace Nhom4WebThuocThayThe.Services;

public sealed class DrugSearchService(
    InMemoryPharmacyStore store,
    IDrugCatalogService catalogService,
    IInventoryService inventoryService) : IDrugSearchService
{
    public DrugSearchPageViewModel Search(string? keyword, int? categoryId)
    {
        var normalizedKeyword = keyword?.Trim();
        var query = catalogService.GetDrugs().AsEnumerable();

        if (categoryId is not null)
        {
            var category = store.Categories.FirstOrDefault(item => item.Id == categoryId.Value);
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
            Categories = store.Categories
                .OrderBy(category => category.Name)
                .Select(category => new SelectListItem(category.Name, category.Id.ToString(), category.Id == categoryId))
                .ToList(),
            Results = query.ToList()
        };
    }

    public DrugDetailViewModel? GetDetail(int id)
    {
        var drug = store.Drugs.FirstOrDefault(item => item.Id == id);
        if (drug is null)
        {
            return null;
        }

        var ingredientLink = store.DrugActiveIngredients.FirstOrDefault(item => item.DrugId == drug.Id);
        var ingredient = ingredientLink is null
            ? null
            : store.ActiveIngredients.FirstOrDefault(item => item.Id == ingredientLink.ActiveIngredientId);
        var alternatives = ingredientLink is null
            ? []
            : catalogService.GetDrugs()
                .Where(item => IsSameActiveIngredient(item.Id, ingredientLink.ActiveIngredientId) && item.Id != drug.Id)
                .OrderByDescending(item => item.StockQuantity)
                .ThenBy(item => item.Name)
                .ToList();

        return new DrugDetailViewModel
        {
            Id = drug.Id,
            Name = drug.Name,
            Strength = drug.Strength,
            Price = drug.Price,
            Category = store.Categories.First(category => category.Id == drug.CategoryId).Name,
            DosageForm = store.DosageForms.First(form => form.Id == drug.DosageFormId).Name,
            Unit = store.Units.First(unit => unit.Id == drug.UnitId).Name,
            Manufacturer = store.Manufacturers.First(manufacturer => manufacturer.Id == drug.ManufacturerId).Name,
            ActiveIngredient = ingredient?.Name ?? "Chua khai bao",
            ActiveIngredientWarning = ingredient?.Warning,
            StockQuantity = inventoryService.GetAvailableQuantity(drug.Id),
            PrescriptionRequired = drug.PrescriptionRequired,
            Description = drug.Description,
            Usage = drug.Usage,
            Contraindications = drug.Contraindications,
            Alternatives = alternatives
        };
    }

    private bool Matches(DrugListItemViewModel item, string keyword)
    {
        var drug = store.Drugs.First(current => current.Id == item.Id);
        var ingredientIds = store.DrugActiveIngredients
            .Where(link => link.DrugId == drug.Id)
            .Select(link => link.ActiveIngredientId)
            .ToHashSet();
        var activeIngredientNames = store.ActiveIngredients
            .Where(ingredient => ingredientIds.Contains(ingredient.Id))
            .Select(ingredient => ingredient.Name);

        return Contains(item.Name, keyword) ||
               Contains(item.Category, keyword) ||
               Contains(item.Manufacturer, keyword) ||
               activeIngredientNames.Any(name => Contains(name, keyword));
    }

    private static bool Contains(string value, string keyword)
    {
        return value.Contains(keyword, StringComparison.OrdinalIgnoreCase);
    }

    private bool IsSameActiveIngredient(int drugId, int activeIngredientId)
    {
        return store.DrugActiveIngredients.Any(item =>
            item.DrugId == drugId && item.ActiveIngredientId == activeIngredientId);
    }
}
