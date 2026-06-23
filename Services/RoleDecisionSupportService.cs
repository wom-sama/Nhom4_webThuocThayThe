using Microsoft.EntityFrameworkCore;
using Nhom4WebThuocThayThe.Data;
using Nhom4WebThuocThayThe.Models;
using Nhom4WebThuocThayThe.ViewModels.RoleSupport;

namespace Nhom4WebThuocThayThe.Services;

public sealed class RoleDecisionSupportService(PharmacyDbContext dbContext) : IRoleDecisionSupportService
{
    public IReadOnlyCollection<RoleInsightViewModel> GetInsights(string role, string? userEmail)
    {
        return role switch
        {
            AppRoles.Admin => GetAdminInsights(),
            AppRoles.Pharmacist => GetPharmacistInsights(),
            AppRoles.Expert => GetExpertInsights(),
            AppRoles.User => GetUserInsights(userEmail),
            _ => []
        };
    }

    private IReadOnlyCollection<RoleInsightViewModel> GetAdminInsights()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var stock = GetStockByDrug(today);
        var activeDrugIds = dbContext.Drugs
            .AsNoTracking()
            .Where(drug => drug.IsActive)
            .Select(drug => drug.Id)
            .ToList();
        var stockouts = activeDrugIds.Count(drugId => !stock.ContainsKey(drugId));
        var nearExpiry = dbContext.Batches
            .AsNoTracking()
            .Count(batch => batch.Quantity > 0 && batch.ExpiryDate >= today && batch.ExpiryDate <= today.AddDays(90));
        var staleSources = dbContext.ExternalDataSources
            .AsNoTracking()
            .Count(source => source.LastSyncDate == null || source.LastSyncDate < today.AddDays(-45));

        return
        [
            new()
            {
                Title = "AI ưu tiên vận hành",
                Summary = stockouts == 0
                    ? "Không có thuốc hết hàng trong dữ liệu khả dụng."
                    : $"{stockouts} thuốc đang hết hàng, cần kiểm tra đề xuất thay thế trước demo.",
                Severity = stockouts == 0 ? "success" : "danger",
                ActionLabel = "Mở kho",
                ActionUrl = "/Admin/Inventory",
                Signals = [$"Cận hạn: {nearExpiry} lô", $"Nguồn cần đồng bộ: {staleSources}"]
            },
            new()
            {
                Title = "AI kiểm tra release",
                Summary = "Trước khi chấm, cần có health xanh, backup/audit và dữ liệu demo đủ rộng.",
                Severity = staleSources > 0 ? "warning" : "info",
                ActionLabel = "Mở báo cáo",
                ActionUrl = "/Admin/Reports",
                Signals = [$"Audit logs: {dbContext.AuditLogs.Count():N0}", $"Nguồn dữ liệu: {dbContext.ExternalDataSources.Count():N0}"]
            }
        ];
    }

    private IReadOnlyCollection<RoleInsightViewModel> GetPharmacistInsights()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var stock = GetStockByDrug(today);
        var activeDrugs = dbContext.Drugs.AsNoTracking().Where(drug => drug.IsActive).ToList();
        var stockouts = activeDrugs.Count(drug => !stock.ContainsKey(drug.Id));
        var lowStock = activeDrugs.Count(drug => stock.TryGetValue(drug.Id, out var quantity) && quantity is > 0 and <= 30);
        var prescriptionCandidates = activeDrugs.Count(drug => drug.PrescriptionRequired);

        return
        [
            new()
            {
                Title = "AI xếp hàng xử lý",
                Summary = stockouts > 0
                    ? $"{stockouts} thuốc hết hàng cần mở hàng đợi để chọn phương án thay thế."
                    : "Không có thuốc hết hàng, ưu tiên rà lô sắp hết.",
                Severity = stockouts > 0 ? "danger" : "success",
                ActionLabel = "Hàng đợi",
                ActionUrl = "/Pharmacist/Workspace",
                Signals = [$"Sắp hết: {lowStock}", $"Cần kê đơn: {prescriptionCandidates}"]
            },
            new()
            {
                Title = "AI nhắc an toàn",
                Summary = "Khi tư vấn, ưu tiên ứng viên còn hàng, cùng hoạt chất và không có cảnh báo cao.",
                Severity = "info",
                ActionLabel = "So sánh",
                ActionUrl = "/Pharmacist/Workspace/Compare",
                Signals = ["Không thay thế chỉ định bác sĩ", "Gemini chỉ diễn giải khi được yêu cầu"]
            }
        ];
    }

    private IReadOnlyCollection<RoleInsightViewModel> GetExpertInsights()
    {
        var reviews = dbContext.ExpertReviews.AsNoTracking().ToList();
        var waiting = reviews.Count(item => item.Status == "Cho danh gia");
        var needsReview = reviews.Count(item => item.Status == "Can xem xet");
        var lowEvidence = reviews.Count(item => item.Score < 70);

        return
        [
            new()
            {
                Title = "AI lọc hồ sơ",
                Summary = waiting + needsReview > 0
                    ? $"{waiting + needsReview} hồ sơ cần quyết định hoặc nhận xét chuyên môn."
                    : "Không còn hồ sơ cần xử lý ngay.",
                Severity = waiting + needsReview > 0 ? "warning" : "success",
                ActionLabel = "Mở hàng chờ",
                ActionUrl = "/Expert/Reviews",
                Signals = [$"Chờ đánh giá: {waiting}", $"Cần xem xét: {needsReview}"]
            },
            new()
            {
                Title = "AI kiểm tra bằng chứng",
                Summary = lowEvidence > 0
                    ? $"{lowEvidence} đề xuất có điểm thấp, nên đọc bằng chứng trước khi chấp nhận."
                    : "Các đề xuất hiện có đều đạt ngưỡng bằng chứng trung bình trở lên.",
                Severity = lowEvidence > 0 ? "warning" : "info",
                ActionLabel = "Xem bằng chứng",
                ActionUrl = "/Expert/Reviews/Evidence",
                Signals = [$"Tổng hồ sơ: {reviews.Count}", "Lịch sử quyết định tách riêng"]
            }
        ];
    }

    private IReadOnlyCollection<RoleInsightViewModel> GetUserInsights(string? userEmail)
    {
        var profile = string.IsNullOrWhiteSpace(userEmail)
            ? null
            : dbContext.PatientSafetyProfiles.AsNoTracking().FirstOrDefault(item => item.Email == userEmail);
        var drugCount = dbContext.Drugs.AsNoTracking().Count(item => item.IsActive);
        var categoryCount = dbContext.Categories.AsNoTracking().Count();

        return
        [
            new()
            {
                Title = "AI gợi ý tra cứu",
                Summary = "Bắt đầu bằng tên thuốc hoặc hoạt chất; hệ thống sẽ ưu tiên thuốc còn hàng và cảnh báo rõ ràng.",
                Severity = "info",
                ActionLabel = "Tra cứu",
                ActionUrl = "/Drugs",
                Signals = [$"{drugCount} thuốc", $"{categoryCount} nhóm điều trị"]
            },
            new()
            {
                Title = "AI nhắc hồ sơ an toàn",
                Summary = profile is null
                    ? "Chưa có hồ sơ an toàn riêng cho tài khoản này."
                    : profile.ClinicalNote ?? "Hồ sơ an toàn chưa có ghi chú lâm sàng.",
                Severity = profile is null ? "warning" : "success",
                ActionLabel = "Mở tra cứu",
                ActionUrl = "/Drugs",
                Signals = profile is null ? ["Không lưu đơn thuốc cá nhân"] : [$"Hồ sơ: {profile.DisplayName}"]
            }
        ];
    }

    private Dictionary<int, int> GetStockByDrug(DateOnly today)
    {
        return dbContext.Batches
            .AsNoTracking()
            .Where(batch => batch.Quantity > 0 && batch.ExpiryDate >= today)
            .GroupBy(batch => batch.DrugId)
            .Select(group => new { DrugId = group.Key, Quantity = group.Sum(batch => batch.Quantity) })
            .ToDictionary(item => item.DrugId, item => item.Quantity);
    }
}
