namespace Nhom4WebThuocThayThe.Models;

public sealed class Drug
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public required string Strength { get; set; }

    public decimal Price { get; set; }

    public int CategoryId { get; set; }

    public int DosageFormId { get; set; }

    public int UnitId { get; set; }

    public int ManufacturerId { get; set; }

    public bool PrescriptionRequired { get; set; }

    public string? Description { get; set; }

    public string? Usage { get; set; }

    public string? Contraindications { get; set; }

    public bool IsActive { get; set; } = true;
}
