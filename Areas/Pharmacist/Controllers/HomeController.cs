using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nhom4WebThuocThayThe.Models;
using Nhom4WebThuocThayThe.Services;
using Nhom4WebThuocThayThe.ViewModels.Pharmacist;

namespace Nhom4WebThuocThayThe.Areas.Pharmacist.Controllers;

[Area("Pharmacist")]
[Authorize(Roles = AppRoles.Pharmacist)]
public sealed class HomeController(
    IInventoryService inventoryService,
    IRoleDecisionSupportService roleDecisionSupportService) : Controller
{
    public IActionResult Index() => View(new PharmacistHomeViewModel
    {
        StockSummaries = inventoryService.GetStockSummaries(),
        Insights = roleDecisionSupportService.GetInsights(AppRoles.Pharmacist, User.FindFirstValue(ClaimTypes.Email))
    });
}
