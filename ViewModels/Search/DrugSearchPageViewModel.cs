using Microsoft.AspNetCore.Mvc.Rendering;
using Nhom4WebThuocThayThe.ViewModels.Catalog;

namespace Nhom4WebThuocThayThe.ViewModels.Search;

public sealed class DrugSearchPageViewModel
{
    public string? Keyword { get; set; }

    public int? CategoryId { get; set; }

    public IEnumerable<SelectListItem> Categories { get; set; } = [];

    public IReadOnlyCollection<DrugListItemViewModel> Results { get; set; } = [];
}
