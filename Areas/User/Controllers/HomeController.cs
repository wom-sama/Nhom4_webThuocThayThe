using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nhom4WebThuocThayThe.Models;
using Nhom4WebThuocThayThe.Services;
using Nhom4WebThuocThayThe.ViewModels.User;

namespace Nhom4WebThuocThayThe.Areas.User.Controllers;

[Area("User")]
[Authorize(Roles = AppRoles.User)]
public sealed class HomeController(IRoleDecisionSupportService roleDecisionSupportService) : Controller
{
    public IActionResult Index() => View(new UserHomeViewModel
    {
        DisplayName = User.Identity?.Name ?? "Người dùng",
        Insights = roleDecisionSupportService.GetInsights(AppRoles.User, User.FindFirstValue(ClaimTypes.Email))
    });

    public IActionResult History() => View();

    public IActionResult Saved() => View();
}
