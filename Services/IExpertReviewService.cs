using Nhom4WebThuocThayThe.ViewModels.ExpertReviews;

namespace Nhom4WebThuocThayThe.Services;

public interface IExpertReviewService
{
    IReadOnlyCollection<ExpertReviewListItemViewModel> GetReviews();

    IReadOnlyCollection<ExpertReviewListItemViewModel> GetPendingReviews();

    IReadOnlyCollection<ExpertReviewListItemViewModel> GetEvidenceReviews();

    IReadOnlyCollection<ExpertReviewListItemViewModel> GetDecisionHistory();

    bool UpdateReview(int id, string status, string reviewer, string note);
}
