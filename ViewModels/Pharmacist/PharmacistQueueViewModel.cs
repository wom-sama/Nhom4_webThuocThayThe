using Nhom4WebThuocThayThe.ViewModels.Search;

namespace Nhom4WebThuocThayThe.ViewModels.Pharmacist;

public sealed class PharmacistQueueViewModel
{
    public IReadOnlyCollection<DrugDetailViewModel> OutOfStockDrugs { get; init; } = [];

    public IReadOnlyCollection<DrugDetailViewModel> LowStockDrugs { get; init; } = [];
}
