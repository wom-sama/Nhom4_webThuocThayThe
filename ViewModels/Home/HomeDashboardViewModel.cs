using Microsoft.AspNetCore.Mvc.Rendering;

namespace Nhom4WebThuocThayThe.ViewModels.Home;

public sealed class HomeDashboardViewModel
{
    public int DrugCount { get; init; }

    public int CategoryCount { get; init; }

    public int BatchCount { get; init; }

    public int StockoutCount { get; init; }

    public IReadOnlyCollection<SelectListItem> Categories { get; init; } = [];
}
