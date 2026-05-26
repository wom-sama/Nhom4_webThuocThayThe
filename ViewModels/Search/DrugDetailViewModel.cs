namespace Nhom4WebThuocThayThe.ViewModels.Search;

public sealed class DrugDetailViewModel
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public required string Strength { get; set; }

    public required string Category { get; set; }

    public required string DosageForm { get; set; }

    public required string Unit { get; set; }

    public required string Manufacturer { get; set; }

    public required string ActiveIngredient { get; set; }

    public decimal Price { get; set; }

    public int StockQuantity { get; set; }

    public bool PrescriptionRequired { get; set; }

    public string? Description { get; set; }

    public string? Usage { get; set; }

    public string? Contraindications { get; set; }
}
