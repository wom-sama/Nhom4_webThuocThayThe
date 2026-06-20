using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nhom4WebThuocThayThe.Data;
using Nhom4WebThuocThayThe.Services;

namespace Nhom4WebThuocThayThe.Controllers;

[Authorize(Policy = "AdminOnly")]
public sealed class ExternalDataController(
    PharmacyDbContext dbContext,
    IAuditLogService auditLogService) : Controller
{
    public IActionResult Index()
    {
        return View(dbContext.ExternalDataSources.AsNoTracking().OrderBy(item => item.Name).ToList());
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
        source.MappingStatus = "Da dong bo metadata";
        dbContext.SaveChanges();
        auditLogService.Add(
            User.Identity?.Name ?? "Unknown",
            "Sync",
            "External data source",
            $"Marked {source.Name} as synchronized.");

        TempData["SuccessMessage"] = $"Da cap nhat trang thai {source.Name}.";
        return RedirectToAction(nameof(Index));
    }
}
