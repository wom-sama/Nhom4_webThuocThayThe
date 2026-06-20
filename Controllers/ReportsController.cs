using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nhom4WebThuocThayThe.Services;

namespace Nhom4WebThuocThayThe.Controllers;

[Authorize(Policy = "ExpertReviewer")]
public sealed class ReportsController(
    IReportingService reportingService,
    IAuditLogService auditLogService) : Controller
{
    public IActionResult Index()
    {
        return View(reportingService.GetDashboard());
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    [ValidateAntiForgeryToken]
    public IActionResult DownloadBackup()
    {
        var snapshot = auditLogService.CreateSnapshot();
        auditLogService.Add(
            User.Identity?.Name ?? "Unknown",
            "Backup",
            "System snapshot",
            $"Generated backup metadata at {snapshot.GeneratedAt:O}.");

        var bytes = JsonSerializer.SerializeToUtf8Bytes(snapshot, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        return File(bytes, "application/json", $"n4wtt-backup-{DateTime.Now:yyyyMMdd-HHmmss}.json");
    }
}
