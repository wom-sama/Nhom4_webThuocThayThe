using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nhom4WebThuocThayThe.Data;
using Nhom4WebThuocThayThe.Models;
using Nhom4WebThuocThayThe.Services;

namespace Nhom4WebThuocThayThe.Controllers;

public class HomeController(
    PharmacyDbContext dbContext,
    IInventoryService inventoryService) : Controller
{
    [AllowAnonymous]
    public IActionResult Index()
    {
        var drugs = dbContext.Drugs.AsNoTracking().ToList();
        ViewBag.DrugCount = drugs.Count;
        ViewBag.CategoryCount = dbContext.Categories.Count();
        ViewBag.BatchCount = dbContext.Batches.Count();
        ViewBag.StockoutCount = drugs.Count(drug => inventoryService.GetAvailableQuantity(drug.Id) == 0);
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
