using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nhom4WebThuocThayThe.Models;
using Nhom4WebThuocThayThe.Services;

namespace Nhom4WebThuocThayThe.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = AppRoles.Admin)]
public sealed class HomeController(IReportingService reportingService) : Controller
{
    public IActionResult Index() => View(reportingService.GetDashboard());
}
