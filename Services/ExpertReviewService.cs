using Nhom4WebThuocThayThe.Data;
using Nhom4WebThuocThayThe.ViewModels.ExpertReviews;

namespace Nhom4WebThuocThayThe.Services;

public sealed class ExpertReviewService(
    InMemoryPharmacyStore store,
    IAuditLogService auditLogService) : IExpertReviewService
{
    public IReadOnlyCollection<ExpertReviewListItemViewModel> GetReviews()
    {
        return store.ExpertReviews
            .OrderByDescending(item => item.UpdatedAt)
            .Select(item =>
            {
                var source = store.Drugs.First(drug => drug.Id == item.SourceDrugId);
                var recommendation = store.Drugs.First(drug => drug.Id == item.RecommendedDrugId);
                return new ExpertReviewListItemViewModel
                {
                    Id = item.Id,
                    SourceDrug = $"{source.Name} - {source.Strength}",
                    RecommendedDrug = $"{recommendation.Name} - {recommendation.Strength}",
                    Score = item.Score,
                    Status = item.Status,
                    Reviewer = item.Reviewer,
                    Note = item.Note,
                    UpdatedAt = item.UpdatedAt
                };
            })
            .ToList();
    }

    public bool UpdateReview(int id, string status, string reviewer, string note)
    {
        var item = store.ExpertReviews.FirstOrDefault(review => review.Id == id);
        if (item is null)
        {
            return false;
        }

        item.Status = string.IsNullOrWhiteSpace(status) ? item.Status : status.Trim();
        item.Reviewer = string.IsNullOrWhiteSpace(reviewer) ? "Chuyen gia" : reviewer.Trim();
        item.Note = string.IsNullOrWhiteSpace(note) ? item.Note : note.Trim();
        item.UpdatedAt = DateTimeOffset.Now;
        auditLogService.Add(item.Reviewer, "Expert review", "AI recommendation", $"Review #{id} moved to {item.Status}.");
        return true;
    }
}
