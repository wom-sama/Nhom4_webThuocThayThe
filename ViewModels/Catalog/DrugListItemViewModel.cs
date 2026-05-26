namespace Nhom4WebThuocThayThe.ViewModels.Catalog;

public sealed class DrugListItemViewModel
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public required string Strength { get; set; }

    public required string Category { get; set; }

    public required string DosageForm { get; set; }

    public required string Manufacturer { get; set; }

    public decimal Price { get; set; }

    public int StockQuantity { get; set; }

    public bool PrescriptionRequired { get; set; }

    public bool IsActive { get; set; }
}
