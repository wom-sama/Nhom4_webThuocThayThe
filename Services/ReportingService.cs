using Nhom4WebThuocThayThe.Data;
using Microsoft.EntityFrameworkCore;
using Nhom4WebThuocThayThe.ViewModels.Reports;

namespace Nhom4WebThuocThayThe.Services;

public sealed class ReportingService(
    PharmacyDbContext dbContext,
    IInventoryService inventoryService,
    IRecommendationService recommendationService,
    IAuditLogService auditLogService) : IReportingService
{
    public DashboardViewModel GetDashboard()
    {
        var stockRisks = dbContext.Drugs
            .AsNoTracking()
            .AsEnumerable()
            .Select(drug =>
            {
                var quantity = inventoryService.GetAvailableQuantity(drug.Id);
                return new StockRiskViewModel
                {
                    DrugName = drug.Name,
                    Strength = drug.Strength,
                    Quantity = quantity,
                    RiskLevel = quantity == 0 ? "Het hang" : quantity <= 30 ? "Can nhap them" : "On dinh"
                };
            })
            .OrderBy(item => item.Quantity)
            .ThenBy(item => item.DrugName)
            .ToList();

        var stockoutCount = stockRisks.Count(item => item.Quantity == 0);
        var highRiskRecommendations = dbContext.Drugs
            .AsNoTracking()
            .AsEnumerable()
            .Where(drug => inventoryService.GetAvailableQuantity(drug.Id) == 0)
            .SelectMany(drug => recommendationService.GetRecommendations(drug.Id, null))
            .Count(item => item.HasHighRiskAlert);

        return new DashboardViewModel
        {
            Metrics =
            [
                new() { Label = "Thuoc dang quan ly", Value = dbContext.Drugs.Count().ToString("N0"), Hint = "Danh muc SQL Server" },
                new() { Label = "Thuoc het hang", Value = stockoutCount.ToString("N0"), Hint = "Can kich hoat de xuat thay the" },
                new() { Label = "Nguon du lieu ngoai", Value = dbContext.ExternalDataSources.Count().ToString("N0"), Hint = "DrugBank, PubChem, ATC..." },
                new() { Label = "De xuat can canh bao", Value = highRiskRecommendations.ToString("N0"), Hint = "Can QC/duoc si xac nhan" }
            ],
            StockRisks = stockRisks,
            ExternalSources = dbContext.ExternalDataSources.AsNoTracking().OrderBy(item => item.Name).ToList(),
            AuditLogs = auditLogService.GetRecent(8),
            BackupSnapshot = auditLogService.CreateSnapshot()
        };
    }
}
