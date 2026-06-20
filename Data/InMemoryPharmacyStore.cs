using Nhom4WebThuocThayThe.Models;

namespace Nhom4WebThuocThayThe.Data;

public sealed class InMemoryPharmacyStore
{
    private int _nextDrugId = 8;
    private int _nextBatchId = 8;
    private int _nextAuditLogId = 4;

    public List<DrugCategory> Categories { get; } =
    [
        new() { Id = 1, Name = "Giam dau ha sot", Description = "Thuoc dieu tri dau, sot thong thuong." },
        new() { Id = 2, Name = "Khang sinh", Description = "Thuoc can ke don va can xac nhan chuyen mon." },
        new() { Id = 3, Name = "Tieu hoa", Description = "Thuoc ho tro he tieu hoa." },
        new() { Id = 4, Name = "Di ung", Description = "Thuoc ho tro trieu chung di ung." }
    ];

    public List<DosageForm> DosageForms { get; } =
    [
        new() { Id = 1, Name = "Vien nen" },
        new() { Id = 2, Name = "Vien nang" },
        new() { Id = 3, Name = "Siro" },
        new() { Id = 4, Name = "Vien sui" }
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
        new() { Id = 3, Name = "Traphaco", Country = "Viet Nam" },
        new() { Id = 4, Name = "Imexpharm", Country = "Viet Nam" }
    ];

    public List<ActiveIngredient> ActiveIngredients { get; } =
    [
        new() { Id = 1, Name = "Paracetamol", Warning = "Than trong voi benh gan." },
        new() { Id = 2, Name = "Ibuprofen", Warning = "Than trong voi dau da day." },
        new() { Id = 3, Name = "Amoxicillin", Warning = "Khang sinh can ke don." },
        new() { Id = 4, Name = "Cetirizine", Warning = "Co the gay buon ngu." }
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
        },
        new()
        {
            Id = 5,
            Name = "Efferalgan 500mg",
            Strength = "500mg",
            Price = 3000,
            CategoryId = 1,
            DosageFormId = 4,
            UnitId = 1,
            ManufacturerId = 2,
            Description = "Thuoc giam dau ha sot dang vien sui.",
            Usage = "Hoa tan trong nuoc truoc khi dung.",
            Contraindications = "Qua man voi paracetamol."
        },
        new()
        {
            Id = 6,
            Name = "Hapacol 650mg",
            Strength = "650mg",
            Price = 2200,
            CategoryId = 1,
            DosageFormId = 1,
            UnitId = 1,
            ManufacturerId = 1,
            Description = "Thuoc giam dau ha sot ham luong cao hon.",
            Usage = "Dung theo huong dan cua duoc si.",
            Contraindications = "Than trong voi nguoi benh gan."
        },
        new()
        {
            Id = 7,
            Name = "Cetirizine 10mg",
            Strength = "10mg",
            Price = 1500,
            CategoryId = 4,
            DosageFormId = 1,
            UnitId = 1,
            ManufacturerId = 4,
            Description = "Thuoc khang histamine ho tro trieu chung di ung.",
            Usage = "Dung mot lan moi ngay theo huong dan.",
            Contraindications = "Qua man voi cetirizine."
        }
    ];

    public List<DrugActiveIngredient> DrugActiveIngredients { get; } =
    [
        new() { DrugId = 1, ActiveIngredientId = 1, Strength = "500mg" },
        new() { DrugId = 2, ActiveIngredientId = 1, Strength = "500mg" },
        new() { DrugId = 3, ActiveIngredientId = 2, Strength = "400mg" },
        new() { DrugId = 4, ActiveIngredientId = 3, Strength = "500mg" },
        new() { DrugId = 5, ActiveIngredientId = 1, Strength = "500mg" },
        new() { DrugId = 6, ActiveIngredientId = 1, Strength = "650mg" },
        new() { DrugId = 7, ActiveIngredientId = 4, Strength = "10mg" }
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
        new() { Id = 4, DrugId = 4, WarehouseId = 1, BatchNumber = "AMX-009", Quantity = 40, ImportedDate = new DateOnly(2026, 4, 20), ExpiryDate = new DateOnly(2027, 4, 20) },
        new() { Id = 5, DrugId = 5, WarehouseId = 2, BatchNumber = "EFF-050", Quantity = 55, ImportedDate = new DateOnly(2026, 5, 5), ExpiryDate = new DateOnly(2027, 5, 5) },
        new() { Id = 6, DrugId = 6, WarehouseId = 1, BatchNumber = "HAP-650", Quantity = 25, ImportedDate = new DateOnly(2026, 5, 15), ExpiryDate = new DateOnly(2027, 5, 15) },
        new() { Id = 7, DrugId = 7, WarehouseId = 2, BatchNumber = "CET-010", Quantity = 90, ImportedDate = new DateOnly(2026, 4, 28), ExpiryDate = new DateOnly(2027, 4, 28) }
    ];

    public List<PatientSafetyProfile> PatientSafetyProfiles { get; } =
    [
        new()
        {
            Email = "user@nhom4.local",
            DisplayName = "Nguoi dung mac dinh",
            AllergyActiveIngredientIds = [2],
            ClinicalNote = "Di ung voi nhom NSAID, uu tien canh bao ibuprofen."
        },
        new()
        {
            Email = "duocsi@nhom4.local",
            DisplayName = "Ho so kiem thu duoc si",
            AllergyActiveIngredientIds = [1],
            ClinicalNote = "Ho so mau dung de kiem tra canh bao paracetamol."
        }
    ];

    public List<ExternalDataSource> ExternalDataSources { get; } =
    [
        new()
        {
            Id = 1,
            Name = "DrugBank",
            SourceUrl = "https://go.drugbank.com/",
            MappingStatus = "San sang mapping",
            LastSyncDate = new DateOnly(2026, 5, 20),
            Purpose = "Thong tin thuoc, hoat chat, chi dinh va tuong tac."
        },
        new()
        {
            Id = 2,
            Name = "PubChem",
            SourceUrl = "https://pubchem.ncbi.nlm.nih.gov/",
            MappingStatus = "Dang danh gia",
            Purpose = "Doi chieu compound, CID va cau truc hoa hoc."
        },
        new()
        {
            Id = 3,
            Name = "ATC Index",
            SourceUrl = "https://www.whocc.no/atc_ddd_index/",
            MappingStatus = "Da mapping mau",
            LastSyncDate = new DateOnly(2026, 5, 25),
            Purpose = "Phan loai nhom dieu tri va ho tro loc ung vien thay the."
        }
    ];

    public List<AuditLogEntry> AuditLogs { get; } =
    [
        new()
        {
            Id = 1,
            CreatedAt = new DateTimeOffset(2026, 5, 26, 9, 10, 0, TimeSpan.FromHours(7)),
            Actor = "System",
            Action = "Seed data",
            Entity = "Catalog",
            Detail = "Khoi tao du lieu thuoc, lo thuoc va hoat chat."
        },
        new()
        {
            Id = 2,
            CreatedAt = new DateTimeOffset(2026, 5, 26, 11, 30, 0, TimeSpan.FromHours(7)),
            Actor = "QC",
            Action = "Regression",
            Entity = "Search",
            Detail = "Bo acceptance test nen tang dat 20/20."
        },
        new()
        {
            Id = 3,
            CreatedAt = new DateTimeOffset(2026, 5, 26, 14, 5, 0, TimeSpan.FromHours(7)),
            Actor = "System",
            Action = "Performance test",
            Entity = "Application",
            Detail = "Bo performance realtime dat 8/8."
        }
    ];

    public List<ExpertReviewItem> ExpertReviews { get; } =
    [
        new()
        {
            Id = 1,
            SourceDrugId = 1,
            RecommendedDrugId = 2,
            Score = 100,
            Status = "Cho danh gia",
            Reviewer = "Chua gan",
            Note = "Cung hoat chat, cung ham luong, con hang.",
            UpdatedAt = new DateTimeOffset(2026, 6, 20, 10, 0, 0, TimeSpan.FromHours(7))
        },
        new()
        {
            Id = 2,
            SourceDrugId = 1,
            RecommendedDrugId = 5,
            Score = 80,
            Status = "Can xem xet",
            Reviewer = "Chuyen gia mau",
            Note = "Cung hoat chat va ham luong nhung khac dang bao che.",
            UpdatedAt = new DateTimeOffset(2026, 6, 20, 10, 10, 0, TimeSpan.FromHours(7))
        }
    ];

    public int GetNextDrugId()
    {
        return _nextDrugId++;
    }

    public int GetNextBatchId()
    {
        return _nextBatchId++;
    }

    public int GetNextAuditLogId()
    {
        return _nextAuditLogId++;
    }
}
