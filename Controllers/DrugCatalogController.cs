using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nhom4WebThuocThayThe.Services;
using Nhom4WebThuocThayThe.ViewModels.Catalog;

namespace Nhom4WebThuocThayThe.Controllers;

[Authorize(Policy = "CatalogManager")]
public sealed class DrugCatalogController(IDrugCatalogService drugCatalogService) : Controller
{
    public IActionResult Index()
    {
        return View(drugCatalogService.GetDrugs());
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(drugCatalogService.CreateForm());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(DrugFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(drugCatalogService.CreateForm());
        }

        drugCatalogService.CreateDrug(model);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var model = drugCatalogService.CreateForm(id);
        if (model is null)
        {
            return NotFound();
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(DrugFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var hydrated = model.Id is null ? drugCatalogService.CreateForm() : drugCatalogService.CreateForm(model.Id.Value);
            if (hydrated is null)
            {
                return NotFound();
            }

            hydrated.Name = model.Name;
            hydrated.Strength = model.Strength;
            hydrated.Price = model.Price;
            hydrated.CategoryId = model.CategoryId;
            hydrated.DosageFormId = model.DosageFormId;
            hydrated.UnitId = model.UnitId;
            hydrated.ManufacturerId = model.ManufacturerId;
            hydrated.ActiveIngredientId = model.ActiveIngredientId;
            hydrated.ActiveIngredientStrength = model.ActiveIngredientStrength;
            hydrated.PrescriptionRequired = model.PrescriptionRequired;
            hydrated.IsActive = model.IsActive;
            hydrated.Description = model.Description;
            hydrated.Usage = model.Usage;
            hydrated.Contraindications = model.Contraindications;
            return View(hydrated);
        }

        if (!drugCatalogService.UpdateDrug(model))
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }
}
