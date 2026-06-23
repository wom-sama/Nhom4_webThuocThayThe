using Nhom4WebThuocThayThe.ViewModels.RoleSupport;

namespace Nhom4WebThuocThayThe.ViewModels.ExpertReviews;

public sealed class ExpertHomeViewModel
{
    public IReadOnlyCollection<ExpertReviewListItemViewModel> Reviews { get; init; } = [];

    public IReadOnlyCollection<RoleInsightViewModel> Insights { get; init; } = [];
}
