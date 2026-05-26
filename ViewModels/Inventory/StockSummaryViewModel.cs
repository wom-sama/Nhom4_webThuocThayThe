namespace Nhom4WebThuocThayThe.ViewModels.Inventory;

public sealed class StockSummaryViewModel
{
    public int DrugId { get; set; }

    public required string DrugName { get; set; }

    public required string Strength { get; set; }

    public int Quantity { get; set; }

    public bool IsOutOfStock => Quantity <= 0;
}
