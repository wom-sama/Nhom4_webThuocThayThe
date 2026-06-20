using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Nhom4WebThuocThayThe.Data;
using Nhom4WebThuocThayThe.Models;
using Nhom4WebThuocThayThe.ViewModels.Catalog;

namespace Nhom4WebThuocThayThe.Services;

public sealed class DrugCatalogService(
    PharmacyDbContext dbContext,
    IInventoryService inventoryService) : IDrugCatalogService
{
    public IReadOnlyCollection<DrugListItemViewModel> GetDrugs()
    {
        return dbContext.Drugs
            .AsNoTracking()
            .OrderBy(drug => drug.Name)
            .Select(ToListItem)
            .ToList();
    }

    public DrugFormViewModel CreateForm()
    {
        return Populate(new DrugFormViewModel());
    }

    public DrugFormViewModel? CreateForm(int id)
    {
        var drug = GetDrug(id);
        if (drug is null)
        {
            return null;
        }

        var ingredient = dbContext.DrugActiveIngredients
            .AsNoTracking()
            .FirstOrDefault(item => item.DrugId == drug.Id);
        return Populate(new DrugFormViewModel
        {
            Id = drug.Id,
            Name = drug.Name,
            Strength = drug.Strength,
            Price = drug.Price,
            CategoryId = drug.CategoryId,
            DosageFormId = drug.DosageFormId,
            UnitId = drug.UnitId,
            ManufacturerId = drug.ManufacturerId,
            ActiveIngredientId = ingredient?.ActiveIngredientId ?? dbContext.ActiveIngredients.AsNoTracking().First().Id,
            ActiveIngredientStrength = ingredient?.Strength ?? drug.Strength,
            PrescriptionRequired = drug.PrescriptionRequired,
            IsActive = drug.IsActive,
            Description = drug.Description,
            Usage = drug.Usage,
            Contraindications = drug.Contraindications
        });
    }

    public Drug? GetDrug(int id)
    {
        return dbContext.Drugs.FirstOrDefault(drug => drug.Id == id);
    }

    public void CreateDrug(DrugFormViewModel model)
    {
        var id = dbContext.Drugs.Any() ? dbContext.Drugs.Max(item => item.Id) + 1 : 1;
        dbContext.Drugs.Add(new Drug
        {
            Id = id,
            Name = model.Name.Trim(),
            Strength = model.Strength.Trim(),
            Price = model.Price,
            CategoryId = model.CategoryId,
            DosageFormId = model.DosageFormId,
            UnitId = model.UnitId,
            ManufacturerId = model.ManufacturerId,
            PrescriptionRequired = model.PrescriptionRequired,
            IsActive = model.IsActive,
            Description = model.Description,
            Usage = model.Usage,
            Contraindications = model.Contraindications
        });

        dbContext.DrugActiveIngredients.Add(new DrugActiveIngredient
        {
            DrugId = id,
            ActiveIngredientId = model.ActiveIngredientId,
            Strength = model.ActiveIngredientStrength.Trim()
        });
        dbContext.SaveChanges();
    }

    public bool UpdateDrug(DrugFormViewModel model)
    {
        if (model.Id is null)
        {
            return false;
        }

        var drug = GetDrug(model.Id.Value);
        if (drug is null)
        {
            return false;
        }

        drug.Name = model.Name.Trim();
        drug.Strength = model.Strength.Trim();
        drug.Price = model.Price;
        drug.CategoryId = model.CategoryId;
        drug.DosageFormId = model.DosageFormId;
        drug.UnitId = model.UnitId;
        drug.ManufacturerId = model.ManufacturerId;
        drug.PrescriptionRequired = model.PrescriptionRequired;
        drug.IsActive = model.IsActive;
        drug.Description = model.Description;
        drug.Usage = model.Usage;
        drug.Contraindications = model.Contraindications;

        var ingredient = dbContext.DrugActiveIngredients.FirstOrDefault(item => item.DrugId == drug.Id);
        if (ingredient is null)
        {
            dbContext.DrugActiveIngredients.Add(new DrugActiveIngredient
            {
                DrugId = drug.Id,
                ActiveIngredientId = model.ActiveIngredientId,
                Strength = model.ActiveIngredientStrength.Trim()
            });
        }
        else
        {
            ingredient.ActiveIngredientId = model.ActiveIngredientId;
            ingredient.Strength = model.ActiveIngredientStrength.Trim();
        }

        dbContext.SaveChanges();
        return true;
    }

    public IReadOnlyCollection<DrugCategory> GetCategories()
    {
        return dbContext.Categories.AsNoTracking().OrderBy(category => category.Name).ToList();
    }

    private DrugListItemViewModel ToListItem(Drug drug)
    {
        return new DrugListItemViewModel
        {
            Id = drug.Id,
            Name = drug.Name,
            Strength = drug.Strength,
            Price = drug.Price,
            Category = dbContext.Categories.AsNoTracking().First(category => category.Id == drug.CategoryId).Name,
            DosageForm = dbContext.DosageForms.AsNoTracking().First(form => form.Id == drug.DosageFormId).Name,
            Manufacturer = dbContext.Manufacturers.AsNoTracking().First(manufacturer => manufacturer.Id == drug.ManufacturerId).Name,
            PrescriptionRequired = drug.PrescriptionRequired,
            IsActive = drug.IsActive,
            StockQuantity = inventoryService.GetAvailableQuantity(drug.Id)
        };
    }

    private DrugFormViewModel Populate(DrugFormViewModel model)
    {
        model.Categories = dbContext.Categories.AsNoTracking().Select(category => new SelectListItem(category.Name, category.Id.ToString(), category.Id == model.CategoryId)).ToList();
        model.DosageForms = dbContext.DosageForms.AsNoTracking().Select(form => new SelectListItem(form.Name, form.Id.ToString(), form.Id == model.DosageFormId)).ToList();
        model.Units = dbContext.Units.AsNoTracking().Select(unit => new SelectListItem(unit.Name, unit.Id.ToString(), unit.Id == model.UnitId)).ToList();
        model.Manufacturers = dbContext.Manufacturers.AsNoTracking().Select(manufacturer => new SelectListItem(manufacturer.Name, manufacturer.Id.ToString(), manufacturer.Id == model.ManufacturerId)).ToList();
        model.ActiveIngredients = dbContext.ActiveIngredients.AsNoTracking().Select(ingredient => new SelectListItem(ingredient.Name, ingredient.Id.ToString(), ingredient.Id == model.ActiveIngredientId)).ToList();

        return model;
    }
}
