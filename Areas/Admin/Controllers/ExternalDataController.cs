using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nhom4WebThuocThayThe.Data;
using Nhom4WebThuocThayThe.Models;
using Nhom4WebThuocThayThe.Services;

namespace Nhom4WebThuocThayThe.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = AppRoles.Admin)]
public sealed class ExternalDataController(PharmacyDbContext dbContext, IAuditLogService auditLogService) : Controller
{
    public IActionResult Index()
    {
        ViewData["Layout"] = "~/Areas/Admin/Views/Shared/_Layout.cshtml";
        return View("~/Views/ExternalData/Index.cshtml", dbContext.ExternalDataSources.AsNoTracking().OrderBy(item => item.Name).ToList());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult MarkSynced(int id)
    {
        var source = dbContext.ExternalDataSources.FirstOrDefault(item => item.Id == id);
        if (source is null)
        {
            return NotFound();
        }

        source.LastSyncDate = DateOnly.FromDateTime(DateTime.Today);
        source.MappingStatus = "Đã đồng bộ metadata";
        dbContext.SaveChanges();
        auditLogService.Add(User.Identity?.Name ?? "Unknown", "Sync", "External data source", $"Marked {source.Name} as synchronized.");
        TempData["SuccessMessage"] = $"Đã cập nhật trạng thái {source.Name}.";
        return RedirectToAction(nameof(Index));
    }
}
