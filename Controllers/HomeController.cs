using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nhom4WebThuocThayThe.Data;
using Nhom4WebThuocThayThe.Models;
using Nhom4WebThuocThayThe.Services;

namespace Nhom4WebThuocThayThe.Controllers;

public class HomeController(
    InMemoryPharmacyStore store,
    IInventoryService inventoryService) : Controller
{
    [AllowAnonymous]
    public IActionResult Index()
    {
        ViewBag.DrugCount = store.Drugs.Count;
        ViewBag.CategoryCount = store.Categories.Count;
        ViewBag.BatchCount = store.Batches.Count;
        ViewBag.StockoutCount = store.Drugs.Count(drug => inventoryService.GetAvailableQuantity(drug.Id) == 0);
        return View();
    }

    [Authorize]
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
