using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nhom4WebThuocThayThe.Services;
using Nhom4WebThuocThayThe.ViewModels.Inventory;

namespace Nhom4WebThuocThayThe.Controllers;

[Authorize(Policy = "InventoryManager")]
public sealed class InventoryController(IInventoryService inventoryService) : Controller
{
    public IActionResult Index()
    {
        ViewBag.Batches = inventoryService.GetBatches();
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
            return View(inventoryService.CreateBatchForm());
        }

        inventoryService.AddBatch(model);
        return RedirectToAction(nameof(Index));
    }
}
