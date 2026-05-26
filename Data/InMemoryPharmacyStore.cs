using Nhom4WebThuocThayThe.Models;

namespace Nhom4WebThuocThayThe.Data;

public sealed class InMemoryPharmacyStore
{
    private int _nextDrugId = 5;
    private int _nextBatchId = 5;

    public List<DrugCategory> Categories { get; } =
    [
        new() { Id = 1, Name = "Giam dau ha sot", Description = "Thuoc dieu tri dau, sot thong thuong." },
        new() { Id = 2, Name = "Khang sinh", Description = "Thuoc can ke don va can xac nhan chuyen mon." },
        new() { Id = 3, Name = "Tieu hoa", Description = "Thuoc ho tro he tieu hoa." }
    ];

    public List<DosageForm> DosageForms { get; } =
    [
        new() { Id = 1, Name = "Vien nen" },
        new() { Id = 2, Name = "Vien nang" },
        new() { Id = 3, Name = "Siro" }
    ];

    public List<MeasurementUnit> Units { get; } =
    [
        new() { Id = 1, Name = "Vien" },
        new() { Id = 2, Name = "Hop" },
        new() { Id = 3, Name = "Chai" }
    ];

    public List<Manufacturer> Manufacturers { get; } =
    [
        new() { Id = 1, Name = "DHG Pharma", Country = "Viet Nam" },
        new() { Id = 2, Name = "Sanofi", Country = "France" },
        new() { Id = 3, Name = "Traphaco", Country = "Viet Nam" }
    ];

    public List<ActiveIngredient> ActiveIngredients { get; } =
    [
        new() { Id = 1, Name = "Paracetamol", Warning = "Than trong voi benh gan." },
        new() { Id = 2, Name = "Ibuprofen", Warning = "Than trong voi dau da day." },
        new() { Id = 3, Name = "Amoxicillin", Warning = "Khang sinh can ke don." }
    ];

    public List<Drug> Drugs { get; } =
    [
        new()
        {
            Id = 1,
            Name = "Panadol 500mg",
            Strength = "500mg",
            Price = 2500,
            CategoryId = 1,
            DosageFormId = 1,
            UnitId = 1,
            ManufacturerId = 2,
            Description = "Thuoc giam dau ha sot chua paracetamol.",
            Usage = "Dung theo huong dan cua duoc si.",
            Contraindications = "Qua man voi paracetamol."
        },
        new()
        {
            Id = 2,
            Name = "Paracetamol DHG 500mg",
            Strength = "500mg",
            Price = 1800,
            CategoryId = 1,
            DosageFormId = 1,
            UnitId = 1,
            ManufacturerId = 1,
            Description = "Thuoc giam dau ha sot thay the cung hoat chat.",
            Usage = "Dung theo huong dan cua duoc si.",
            Contraindications = "Qua man voi paracetamol."
        },
        new()
        {
            Id = 3,
            Name = "Ibuprofen 400mg",
            Strength = "400mg",
            Price = 3200,
            CategoryId = 1,
            DosageFormId = 2,
            UnitId = 1,
            ManufacturerId = 3,
            Description = "Thuoc giam dau khang viem non-steroid.",
            Usage = "Dung sau an.",
            Contraindications = "Loet da day tien trien."
        },
        new()
        {
            Id = 4,
            Name = "Amoxicillin 500mg",
            Strength = "500mg",
            Price = 4200,
            CategoryId = 2,
            DosageFormId = 2,
            UnitId = 1,
            ManufacturerId = 1,
            PrescriptionRequired = true,
            Description = "Khang sinh beta-lactam.",
            Usage = "Dung theo don bac si.",
            Contraindications = "Di ung penicillin."
        }
    ];

    public List<DrugActiveIngredient> DrugActiveIngredients { get; } =
    [
        new() { DrugId = 1, ActiveIngredientId = 1, Strength = "500mg" },
        new() { DrugId = 2, ActiveIngredientId = 1, Strength = "500mg" },
        new() { DrugId = 3, ActiveIngredientId = 2, Strength = "400mg" },
        new() { DrugId = 4, ActiveIngredientId = 3, Strength = "500mg" }
    ];

    public List<Warehouse> Warehouses { get; } =
    [
        new() { Id = 1, Name = "Kho trung tam", Address = "Quan 1" },
        new() { Id = 2, Name = "Quay ban le", Address = "Nha thuoc so 1" }
    ];

    public List<DrugBatch> Batches { get; } =
    [
        new() { Id = 1, DrugId = 1, WarehouseId = 1, BatchNumber = "PA-001", Quantity = 0, ImportedDate = new DateOnly(2026, 1, 10), ExpiryDate = new DateOnly(2027, 1, 10) },
        new() { Id = 2, DrugId = 2, WarehouseId = 1, BatchNumber = "DHG-101", Quantity = 120, ImportedDate = new DateOnly(2026, 2, 1), ExpiryDate = new DateOnly(2027, 2, 1) },
        new() { Id = 3, DrugId = 3, WarehouseId = 2, BatchNumber = "IBU-020", Quantity = 75, ImportedDate = new DateOnly(2026, 3, 15), ExpiryDate = new DateOnly(2027, 3, 15) },
        new() { Id = 4, DrugId = 4, WarehouseId = 1, BatchNumber = "AMX-009", Quantity = 40, ImportedDate = new DateOnly(2026, 4, 20), ExpiryDate = new DateOnly(2027, 4, 20) }
    ];

    public int GetNextDrugId()
    {
        return _nextDrugId++;
    }

    public int GetNextBatchId()
    {
        return _nextBatchId++;
    }
}
