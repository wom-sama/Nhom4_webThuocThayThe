using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Nhom4WebThuocThayThe.Services;

namespace Nhom4WebThuocThayThe.Controllers;

public sealed class DrugsController(IDrugSearchService drugSearchService) : Controller
{
    [AllowAnonymous]
    public async Task<IActionResult> Index(string? keyword, int? categoryId)
    {
        return View(await drugSearchService.SearchAsync(keyword, categoryId));
    }

    [AllowAnonymous]
    public async Task<IActionResult> Details(int id)
    {
        var userEmail = User.FindFirstValue(ClaimTypes.Email);
        var model = await drugSearchService.GetDetailAsync(id, userEmail);
        if (model is null)
        {
            return NotFound();
        }

        return View(model);
    }
}
