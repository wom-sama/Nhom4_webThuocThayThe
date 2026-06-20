using Nhom4WebThuocThayThe.ViewModels.ExpertReviews;

namespace Nhom4WebThuocThayThe.Services;

public interface IExpertReviewService
{
    IReadOnlyCollection<ExpertReviewListItemViewModel> GetReviews();

    bool UpdateReview(int id, string status, string reviewer, string note);
}
