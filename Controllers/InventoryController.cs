using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nhom4WebThuocThayThe.Services;
using Nhom4WebThuocThayThe.ViewModels.Inventory;

namespace Nhom4WebThuocThayThe.Controllers;

[Authorize(Policy = "InventoryManager")]
public sealed class InventoryController(
    IInventoryService inventoryService,
    IAuditLogService auditLogService) : Controller
{
    public IActionResult Index()
    {
        ViewBag.Batches = inventoryService.GetBatchList();
        return View(inventoryService.GetStockSummaries());
    }

    [HttpGet]
    public IActionResult CreateBatch()
    {
        return View(inventoryService.CreateBatchForm());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CreateBatch(BatchFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(inventoryService.CreateBatchForm(model));
        }

        inventoryService.AddBatch(model);
        auditLogService.Add(
            User.Identity?.Name ?? "Unknown",
            "Create",
            "Drug batch",
            $"Added batch {model.BatchNumber} with quantity {model.Quantity}.");
        return RedirectToAction(nameof(Index));
    }
}
