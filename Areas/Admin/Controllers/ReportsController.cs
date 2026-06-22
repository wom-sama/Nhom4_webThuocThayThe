using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nhom4WebThuocThayThe.Data;
using Nhom4WebThuocThayThe.Models;
using Nhom4WebThuocThayThe.Services;
using Nhom4WebThuocThayThe.ViewModels.Reports;

namespace Nhom4WebThuocThayThe.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = AppRoles.Admin)]
public sealed class ReportsController(IReportingService reportingService, IAuditLogService auditLogService, PharmacyDbContext dbContext) : Controller
{
    public IActionResult Index()
    {
        ViewData["Layout"] = "~/Areas/Admin/Views/Shared/_Layout.cshtml";
        return View("~/Views/Reports/Index.cshtml", reportingService.GetDashboard());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult DownloadBackup()
    {
        auditLogService.Add(User.Identity?.Name ?? "Unknown", "Backup", "SQL Server database", $"Generated JSON backup at {DateTimeOffset.Now:O}.");
        var backup = new DatabaseBackupViewModel
        {
            Version = "1.0",
            GeneratedAt = DateTimeOffset.Now,
            Categories = dbContext.Categories.AsNoTracking().ToList(),
            DosageForms = dbContext.DosageForms.AsNoTracking().ToList(),
            Units = dbContext.Units.AsNoTracking().ToList(),
            Manufacturers = dbContext.Manufacturers.AsNoTracking().ToList(),
            ActiveIngredients = dbContext.ActiveIngredients.AsNoTracking().ToList(),
            Drugs = dbContext.Drugs.AsNoTracking().ToList(),
            DrugActiveIngredients = dbContext.DrugActiveIngredients.AsNoTracking().ToList(),
            Warehouses = dbContext.Warehouses.AsNoTracking().ToList(),
            Batches = dbContext.Batches.AsNoTracking().ToList(),
            PatientSafetyProfiles = dbContext.PatientSafetyProfiles.AsNoTracking().ToList(),
            ExternalDataSources = dbContext.ExternalDataSources.AsNoTracking().ToList(),
            AuditLogs = dbContext.AuditLogs.AsNoTracking().ToList(),
            ExpertReviews = dbContext.ExpertReviews.AsNoTracking().ToList()
        };
        var bytes = JsonSerializer.SerializeToUtf8Bytes(backup, new JsonSerializerOptions { WriteIndented = true });
        return File(bytes, "application/json", $"n4wtt-backup-{DateTime.Now:yyyyMMdd-HHmmss}.json");
    }
}
