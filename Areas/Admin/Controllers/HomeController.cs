using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nhom4WebThuocThayThe.Models;
using Nhom4WebThuocThayThe.Services;
using Nhom4WebThuocThayThe.ViewModels.Admin;

namespace Nhom4WebThuocThayThe.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = AppRoles.Admin)]
public sealed class HomeController(
    IReportingService reportingService,
    IRoleDecisionSupportService roleDecisionSupportService) : Controller
{
    public IActionResult Index() => View(new AdminHomeViewModel
    {
        Dashboard = reportingService.GetDashboard(),
        Insights = roleDecisionSupportService.GetInsights(AppRoles.Admin, User.FindFirstValue(ClaimTypes.Email))
    });
}
