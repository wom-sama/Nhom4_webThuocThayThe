using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nhom4WebThuocThayThe.Models;
using Nhom4WebThuocThayThe.Services;
using Nhom4WebThuocThayThe.ViewModels.Inventory;

namespace Nhom4WebThuocThayThe.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = AppRoles.Admin)]
public sealed class InventoryController(IInventoryService inventoryService, IAuditLogService auditLogService) : Controller
{
    public IActionResult Index()
    {
        ViewBag.Batches = inventoryService.GetBatchList();
        return AreaView("~/Views/Inventory/Index.cshtml", inventoryService.GetStockSummaries());
    }

    [HttpGet]
    public IActionResult CreateBatch() => AreaView("~/Views/Inventory/CreateBatch.cshtml", inventoryService.CreateBatchForm());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CreateBatch(BatchFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return AreaView("~/Views/Inventory/CreateBatch.cshtml", inventoryService.CreateBatchForm(model));
        }

        inventoryService.AddBatch(model);
        auditLogService.Add(User.Identity?.Name ?? "Unknown", "Create", "Drug batch", $"Added batch {model.BatchNumber} with quantity {model.Quantity}.");
        return RedirectToAction(nameof(Index));
    }

    private ViewResult AreaView(string path, object model)
    {
        ViewData["Layout"] = "~/Areas/Admin/Views/Shared/_Layout.cshtml";
        return View(path, model);
    }
}
