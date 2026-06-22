using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nhom4WebThuocThayThe.Models;
using Nhom4WebThuocThayThe.Services;
using Nhom4WebThuocThayThe.ViewModels.Catalog;

namespace Nhom4WebThuocThayThe.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = AppRoles.Admin)]
public sealed class DrugCatalogController(IDrugCatalogService drugCatalogService, IAuditLogService auditLogService) : Controller
{
    public IActionResult Index() => AreaView("~/Views/DrugCatalog/Index.cshtml", drugCatalogService.GetDrugs());

    [HttpGet]
    public IActionResult Create() => AreaView("~/Views/DrugCatalog/Create.cshtml", drugCatalogService.CreateForm());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(DrugFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return AreaView("~/Views/DrugCatalog/Create.cshtml", drugCatalogService.CreateForm());
        }

        drugCatalogService.CreateDrug(model);
        auditLogService.Add(User.Identity?.Name ?? "Unknown", "Create", "Drug", $"Created drug {model.Name} - {model.Strength}.");
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var model = drugCatalogService.CreateForm(id);
        return model is null ? NotFound() : AreaView("~/Views/DrugCatalog/Edit.cshtml", model);
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
            return AreaView("~/Views/DrugCatalog/Edit.cshtml", hydrated);
        }

        if (!drugCatalogService.UpdateDrug(model))
        {
            return NotFound();
        }

        auditLogService.Add(User.Identity?.Name ?? "Unknown", "Update", "Drug", $"Updated drug #{model.Id}: {model.Name} - {model.Strength}.");
        return RedirectToAction(nameof(Index));
    }

    private ViewResult AreaView(string path, object model)
    {
        ViewData["Layout"] = "~/Areas/Admin/Views/Shared/_Layout.cshtml";
        return View(path, model);
    }
}
