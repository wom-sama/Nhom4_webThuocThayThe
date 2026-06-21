using Nhom4WebThuocThayThe.Data;
using Microsoft.EntityFrameworkCore;
using Nhom4WebThuocThayThe.ViewModels.ExpertReviews;

namespace Nhom4WebThuocThayThe.Services;

public sealed class ExpertReviewService(
    PharmacyDbContext dbContext,
    IAuditLogService auditLogService) : IExpertReviewService
{
    public IReadOnlyCollection<ExpertReviewListItemViewModel> GetReviews()
    {
        var drugs = dbContext.Drugs.AsNoTracking().ToDictionary(item => item.Id);

        return dbContext.ExpertReviews
            .AsNoTracking()
            .OrderByDescending(item => item.UpdatedAt)
            .AsEnumerable()
            .Select(item =>
            {
                var source = drugs[item.SourceDrugId];
                var recommendation = drugs[item.RecommendedDrugId];
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
        var item = dbContext.ExpertReviews.FirstOrDefault(review => review.Id == id);
        if (item is null)
        {
            return false;
        }

        item.Status = string.IsNullOrWhiteSpace(status) ? item.Status : status.Trim();
        item.Reviewer = string.IsNullOrWhiteSpace(reviewer) ? "Chuyen gia" : reviewer.Trim();
        item.Note = string.IsNullOrWhiteSpace(note) ? item.Note : note.Trim();
        item.UpdatedAt = DateTimeOffset.Now;
        dbContext.SaveChanges();
        auditLogService.Add(item.Reviewer, "Expert review", "Rule-based recommendation", $"Review #{id} moved to {item.Status}.");
        return true;
    }
}
