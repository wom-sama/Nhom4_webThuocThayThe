using Nhom4WebThuocThayThe.Data;
using Microsoft.EntityFrameworkCore;
using Nhom4WebThuocThayThe.Models;
using Nhom4WebThuocThayThe.ViewModels.ExpertReviews;

namespace Nhom4WebThuocThayThe.Services;

public sealed class ExpertReviewService(
    PharmacyDbContext dbContext,
    IAuditLogService auditLogService) : IExpertReviewService
{
    public IReadOnlyCollection<ExpertReviewListItemViewModel> GetReviews()
    {
        return BuildReviews()
            .OrderByDescending(item => item.UpdatedAt)
            .ToList();
    }

    public IReadOnlyCollection<ExpertReviewListItemViewModel> GetPendingReviews()
    {
        return BuildReviews()
            .Where(item => item.Status is "Cho danh gia" or "Can xem xet")
            .OrderBy(item => item.Status == "Can xem xet" ? 0 : 1)
            .ThenByDescending(item => item.Score)
            .ThenBy(item => item.UpdatedAt)
            .ToList();
    }

    public IReadOnlyCollection<ExpertReviewListItemViewModel> GetEvidenceReviews()
    {
        return BuildReviews()
            .OrderByDescending(item => item.Score)
            .ThenBy(item => item.StockSignal.Contains("Hết hàng", StringComparison.OrdinalIgnoreCase))
            .ThenBy(item => item.SourceDrug)
            .ToList();
    }

    public IReadOnlyCollection<ExpertReviewListItemViewModel> GetDecisionHistory()
    {
        return BuildReviews()
            .Where(item => item.Status != "Cho danh gia" || !item.Reviewer.Equals("Chưa gán", StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(item => item.UpdatedAt)
            .ToList();
    }

    private IReadOnlyCollection<ExpertReviewListItemViewModel> BuildReviews()
    {
        var drugs = dbContext.Drugs.AsNoTracking().ToDictionary(item => item.Id);
        var today = DateOnly.FromDateTime(DateTime.Today);
        var stockByDrug = dbContext.Batches
            .AsNoTracking()
            .Where(batch => batch.Quantity > 0 && batch.ExpiryDate >= today)
            .GroupBy(batch => batch.DrugId)
            .Select(group => new { DrugId = group.Key, Quantity = group.Sum(batch => batch.Quantity) })
            .ToDictionary(item => item.DrugId, item => item.Quantity);

        return dbContext.ExpertReviews
            .AsNoTracking()
            .AsEnumerable()
            .Select(item =>
            {
                var source = drugs[item.SourceDrugId];
                var recommendation = drugs[item.RecommendedDrugId];
                var ruleSignals = BuildRuleSignals(source, recommendation, item.Score, stockByDrug.GetValueOrDefault(recommendation.Id));
                return new ExpertReviewListItemViewModel
                {
                    Id = item.Id,
                    SourceDrugId = source.Id,
                    SourceDrug = $"{source.Name} - {source.Strength}",
                    RecommendedDrugId = recommendation.Id,
                    RecommendedDrug = $"{recommendation.Name} - {recommendation.Strength}",
                    Score = item.Score,
                    ScoreLabel = RecommendationScoring.GetScoreLabel(item.Score),
                    Status = item.Status,
                    StatusLabel = GetStatusLabel(item.Status),
                    Reviewer = item.Reviewer,
                    Note = item.Note,
                    UpdatedAt = item.UpdatedAt,
                    StockSignal = BuildStockSignal(stockByDrug.GetValueOrDefault(recommendation.Id)),
                    SafetySignal = recommendation.PrescriptionRequired
                        ? "Cần kê đơn hoặc xác nhận chuyên môn trước khi tư vấn."
                        : "Không yêu cầu kê đơn trong dữ liệu hiện tại.",
                    EvidenceLevel = item.Score switch
                    {
                        >= 85 => "Bằng chứng mạnh",
                        >= 70 => "Bằng chứng khá",
                        >= 55 => "Cần đọc kỹ",
                        _ => "Chỉ tham khảo"
                    },
                    RuleSignals = ruleSignals
                };
            })
            .ToList();
    }

    public bool UpdateReview(int id, string status, string reviewer, string note)
    {
        var item = dbContext.ExpertReviews.FirstOrDefault(review => review.Id == id);
        if (item is null)
        {
            return false;
        }

        item.Status = string.IsNullOrWhiteSpace(status) ? item.Status : status.Trim();
        item.Reviewer = string.IsNullOrWhiteSpace(reviewer) ? "Chuyên gia" : reviewer.Trim();
        item.Note = string.IsNullOrWhiteSpace(note) ? item.Note : note.Trim();
        item.UpdatedAt = DateTimeOffset.Now;
        dbContext.SaveChanges();
        auditLogService.Add(item.Reviewer, "Expert review", "Rule-based recommendation", $"Review #{id} moved to {item.Status}.");
        return true;
    }

    private static IReadOnlyCollection<string> BuildRuleSignals(Drug source, Drug recommendation, int score, int stock)
    {
        var signals = new List<string>
        {
            score >= 85 ? "Điểm quy tắc đạt ngưỡng khuyến nghị mạnh." : "Điểm quy tắc cần chuyên gia xác nhận.",
            stock > 0 ? $"Ứng viên còn {stock} đơn vị khả dụng." : "Ứng viên hiện chưa có tồn kho khả dụng."
        };

        if (source.CategoryId == recommendation.CategoryId)
        {
            signals.Add("Cùng nhóm điều trị với thuốc gốc.");
        }

        if (string.Equals(source.Strength, recommendation.Strength, StringComparison.OrdinalIgnoreCase))
        {
            signals.Add("Hàm lượng trùng khớp.");
        }

        if (source.DosageFormId == recommendation.DosageFormId)
        {
            signals.Add("Dạng bào chế tương thích.");
        }

        return signals;
    }

    private static string BuildStockSignal(int stock)
    {
        return stock switch
        {
            <= 0 => "Hết hàng",
            <= 30 => $"Sắp hết: {stock}",
            _ => $"Còn hàng: {stock}"
        };
    }

    private static string GetStatusLabel(string status)
    {
        return status switch
        {
            "Cho danh gia" => "Chờ đánh giá",
            "Chap nhan" => "Chấp nhận",
            "Can xem xet" => "Cần xem xét",
            "Tu choi" => "Từ chối",
            _ => status
        };
    }
}
