using Nhom4WebThuocThayThe.ViewModels.Reports;
using Nhom4WebThuocThayThe.ViewModels.RoleSupport;

namespace Nhom4WebThuocThayThe.ViewModels.Admin;

public sealed class AdminHomeViewModel
{
    public required DashboardViewModel Dashboard { get; init; }

    public IReadOnlyCollection<RoleInsightViewModel> Insights { get; init; } = [];
}
