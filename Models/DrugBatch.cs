namespace Nhom4WebThuocThayThe.Models;

public sealed class DrugBatch
{
    public int Id { get; set; }

    public int DrugId { get; set; }

    public int WarehouseId { get; set; }

    public required string BatchNumber { get; set; }

    public int Quantity { get; set; }

    public DateOnly ExpiryDate { get; set; }

    public DateOnly ImportedDate { get; set; }

    public bool IsUsable(DateOnly today)
    {
        return Quantity > 0 && ExpiryDate >= today;
    }
}
