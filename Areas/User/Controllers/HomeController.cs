using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nhom4WebThuocThayThe.Models;
using Nhom4WebThuocThayThe.Services;
using Nhom4WebThuocThayThe.ViewModels.User;

namespace Nhom4WebThuocThayThe.Areas.User.Controllers;

[Area("User")]
[Authorize(Roles = AppRoles.User)]
public sealed class HomeController(
    IRoleDecisionSupportService roleDecisionSupportService,
    IUserLibraryService userLibraryService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var userEmail = User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
        return View(new UserHomeViewModel
        {
            DisplayName = User.Identity?.Name ?? "Người dùng",
            LibrarySummary = await userLibraryService.GetSummaryAsync(userEmail),
            Insights = roleDecisionSupportService.GetInsights(AppRoles.User, userEmail)
        });
    }

    public async Task<IActionResult> History()
    {
        var userEmail = User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
        return View(await userLibraryService.GetHistoryAsync(userEmail));
    }

    public async Task<IActionResult> Saved()
    {
        var userEmail = User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
        return View(await userLibraryService.GetSavedDrugsAsync(userEmail));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveSaved(int drugId)
    {
        var userEmail = User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
        await userLibraryService.RemoveSavedDrugAsync(userEmail, drugId);
        TempData["StatusMessage"] = "Đã bỏ thuốc khỏi danh sách đã lưu.";
        return RedirectToAction(nameof(Saved));
    }
}
