using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Nhom4WebThuocThayThe.Data;
using Nhom4WebThuocThayThe.Models;
using Nhom4WebThuocThayThe.Services;
using Nhom4WebThuocThayThe.ViewModels.Home;

namespace Nhom4WebThuocThayThe.Controllers;

public class HomeController(PharmacyDbContext dbContext) : Controller
{
    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var model = new HomeDashboardViewModel
        {
            DrugCount = await dbContext.Drugs.AsNoTracking().CountAsync(),
            CategoryCount = await dbContext.Categories.AsNoTracking().CountAsync(),
            BatchCount = await dbContext.Batches.AsNoTracking().CountAsync(),
            StockoutCount = await dbContext.Drugs
                .AsNoTracking()
                .CountAsync(drug => !dbContext.Batches.Any(batch =>
                    batch.DrugId == drug.Id &&
                    batch.Quantity > 0 &&
                    batch.ExpiryDate >= today)),
            Categories = await dbContext.Categories
                .AsNoTracking()
                .OrderBy(category => category.Name)
                .Select(category => new SelectListItem(category.Name, category.Id.ToString()))
                .ToListAsync()
        };
        return View(model);
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
