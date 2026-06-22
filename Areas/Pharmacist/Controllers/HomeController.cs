using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nhom4WebThuocThayThe.Models;
using Nhom4WebThuocThayThe.Services;

namespace Nhom4WebThuocThayThe.Areas.Pharmacist.Controllers;

[Area("Pharmacist")]
[Authorize(Roles = AppRoles.Pharmacist)]
public sealed class HomeController(IInventoryService inventoryService) : Controller
{
    public IActionResult Index() => View(inventoryService.GetStockSummaries());
}
