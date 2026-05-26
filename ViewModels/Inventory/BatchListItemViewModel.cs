namespace Nhom4WebThuocThayThe.ViewModels.Inventory;

public sealed class BatchListItemViewModel
{
    public int Id { get; set; }

    public required string DrugName { get; set; }

    public required string Strength { get; set; }

    public required string WarehouseName { get; set; }

    public required string BatchNumber { get; set; }

    public int Quantity { get; set; }

    public DateOnly ImportedDate { get; set; }

    public DateOnly ExpiryDate { get; set; }

    public bool IsExpired { get; set; }

    public bool IsNearExpiry { get; set; }

    public bool IsUsable { get; set; }

    public string StatusLabel
    {
        get
        {
            if (IsExpired)
            {
                return "Qua han";
            }

            if (Quantity <= 0)
            {
                return "Het lo";
            }

            if (IsNearExpiry)
            {
                return "Can han";
            }

            return "Kha dung";
        }
    }

    public string StatusCssClass
    {
        get
        {
            if (IsExpired || Quantity <= 0)
            {
                return "is-danger";
            }

            return IsNearExpiry ? "is-warning" : "is-success";
        }
    }
}
