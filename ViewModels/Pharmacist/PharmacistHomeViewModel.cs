using Nhom4WebThuocThayThe.ViewModels.Inventory;
using Nhom4WebThuocThayThe.ViewModels.RoleSupport;

namespace Nhom4WebThuocThayThe.ViewModels.Pharmacist;

public sealed class PharmacistHomeViewModel
{
    public IReadOnlyCollection<StockSummaryViewModel> StockSummaries { get; init; } = [];

    public IReadOnlyCollection<RoleInsightViewModel> Insights { get; init; } = [];
}
