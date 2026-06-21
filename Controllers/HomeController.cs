using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nhom4WebThuocThayThe.Data;
using Nhom4WebThuocThayThe.Models;
using Nhom4WebThuocThayThe.Services;

namespace Nhom4WebThuocThayThe.Controllers;

public class HomeController(PharmacyDbContext dbContext) : Controller
{
    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        ViewBag.DrugCount = await dbContext.Drugs.AsNoTracking().CountAsync();
        ViewBag.CategoryCount = await dbContext.Categories.AsNoTracking().CountAsync();
        ViewBag.BatchCount = await dbContext.Batches.AsNoTracking().CountAsync();
        ViewBag.StockoutCount = await dbContext.Drugs
            .AsNoTracking()
            .CountAsync(drug => !dbContext.Batches.Any(batch =>
                batch.DrugId == drug.Id &&
                batch.Quantity > 0 &&
                batch.ExpiryDate >= today));
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
