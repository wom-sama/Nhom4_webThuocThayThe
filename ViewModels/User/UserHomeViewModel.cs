using Nhom4WebThuocThayThe.ViewModels.RoleSupport;

namespace Nhom4WebThuocThayThe.ViewModels.User;

public sealed class UserHomeViewModel
{
    public required string DisplayName { get; init; }

    public UserLibrarySummaryViewModel LibrarySummary { get; init; } = new(0, 0, null);

    public IReadOnlyCollection<RoleInsightViewModel> Insights { get; init; } = [];
}
