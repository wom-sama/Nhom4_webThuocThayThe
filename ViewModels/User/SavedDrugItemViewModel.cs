namespace Nhom4WebThuocThayThe.ViewModels.User;

public sealed class SavedDrugItemViewModel
{
    public int SavedDrugId { get; set; }

    public int DrugId { get; set; }

    public required string Name { get; set; }

    public required string Strength { get; set; }

    public required string Category { get; set; }

    public decimal Price { get; set; }

    public int StockQuantity { get; set; }

    public bool PrescriptionRequired { get; set; }

    public DateTimeOffset SavedAt { get; set; }
}
