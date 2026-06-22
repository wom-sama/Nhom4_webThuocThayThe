using Nhom4WebThuocThayThe.Models;

namespace Nhom4WebThuocThayThe.Data;

public sealed class InMemoryPharmacyStore
{
    private int _nextDrugId = 8;
    private int _nextBatchId = 8;
    private int _nextAuditLogId = 4;

    public List<DrugCategory> Categories { get; } =
    [
        new() { Id = 1, Name = "Giảm đau hạ sốt", Description = "Thuốc điều trị đau, sốt thông thường." },
        new() { Id = 2, Name = "Kháng sinh", Description = "Thuốc cần kê đơn và cần xác nhận chuyên môn." },
        new() { Id = 3, Name = "Tiêu hóa", Description = "Thuốc hỗ trợ hệ tiêu hóa." },
        new() { Id = 4, Name = "Dị ứng", Description = "Thuốc hỗ trợ triệu chứng dị ứng." }
    ];

    public List<DosageForm> DosageForms { get; } =
    [
        new() { Id = 1, Name = "Viên nén" },
        new() { Id = 2, Name = "Viên nang" },
        new() { Id = 3, Name = "Siro" },
        new() { Id = 4, Name = "Viên sủi" }
    ];

    public List<MeasurementUnit> Units { get; } =
    [
        new() { Id = 1, Name = "Viên" },
        new() { Id = 2, Name = "Hộp" },
        new() { Id = 3, Name = "Chai" }
    ];

    public List<Manufacturer> Manufacturers { get; } =
    [
        new() { Id = 1, Name = "DHG Pharma", Country = "Việt Nam" },
        new() { Id = 2, Name = "Sanofi", Country = "France" },
        new() { Id = 3, Name = "Traphaco", Country = "Việt Nam" },
        new() { Id = 4, Name = "Imexpharm", Country = "Việt Nam" }
    ];

    public List<ActiveIngredient> ActiveIngredients { get; } =
    [
        new() { Id = 1, Name = "Paracetamol", Warning = "Thận trọng với bệnh gan." },
        new() { Id = 2, Name = "Ibuprofen", Warning = "Thận trọng với đau dạ dày." },
        new() { Id = 3, Name = "Amoxicillin", Warning = "Kháng sinh cần kê đơn." },
        new() { Id = 4, Name = "Cetirizine", Warning = "Có thể gây buồn ngủ." }
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
            Description = "Thuốc giảm đau hạ sốt chứa paracetamol.",
            Usage = "Dùng theo hướng dẫn của dược sĩ.",
            Contraindications = "Quá mẫn với paracetamol."
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
            Description = "Thuốc giảm đau hạ sốt thay thế cùng hoạt chất.",
            Usage = "Dùng theo hướng dẫn của dược sĩ.",
            Contraindications = "Quá mẫn với paracetamol."
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
            Description = "Thuốc giảm đau kháng viêm không steroid.",
            Usage = "Dùng sau ăn.",
            Contraindications = "Loét dạ dày tiến triển."
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
            Description = "Kháng sinh beta-lactam.",
            Usage = "Dùng theo đơn bác sĩ.",
            Contraindications = "Dị ứng penicillin."
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
            Description = "Thuốc giảm đau hạ sốt dạng viên sủi.",
            Usage = "Hòa tan trong nước trước khi dùng.",
            Contraindications = "Quá mẫn với paracetamol."
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
            Description = "Thuốc giảm đau hạ sốt hàm lượng cao hơn.",
            Usage = "Dùng theo hướng dẫn của dược sĩ.",
            Contraindications = "Thận trọng với người bệnh gan."
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
            Description = "Thuốc kháng histamine hỗ trợ triệu chứng dị ứng.",
            Usage = "Dùng một lần mỗi ngày theo hướng dẫn.",
            Contraindications = "Quá mẫn với cetirizine."
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
        new() { Id = 1, Name = "Kho trung tâm", Address = "Quận 1" },
        new() { Id = 2, Name = "Quầy bán lẻ", Address = "Nhà thuốc số 1" }
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
            DisplayName = "Người dùng mặc định",
            AllergyActiveIngredientIdsCsv = "2",
            ClinicalNote = "Dị ứng với nhóm NSAID, ưu tiên cảnh báo ibuprofen."
        },
        new()
        {
            Email = "duocsi@nhom4.local",
            DisplayName = "Hồ sơ kiểm thử dược sĩ",
            AllergyActiveIngredientIdsCsv = "1",
            ClinicalNote = "Hồ sơ mẫu dùng để kiểm tra cảnh báo paracetamol."
        }
    ];

    public List<ExternalDataSource> ExternalDataSources { get; } =
    [
        new()
        {
            Id = 1,
            Name = "DrugBank",
            SourceUrl = "https://go.drugbank.com/",
            MappingStatus = "Sẵn sàng ánh xạ",
            LastSyncDate = new DateOnly(2026, 5, 20),
            Purpose = "Thông tin thuốc, hoạt chất, chỉ định và tương tác."
        },
        new()
        {
            Id = 2,
            Name = "PubChem",
            SourceUrl = "https://pubchem.ncbi.nlm.nih.gov/",
            MappingStatus = "Đang đánh giá",
            Purpose = "Đối chiếu hợp chất, CID và cấu trúc hóa học."
        },
        new()
        {
            Id = 3,
            Name = "ATC Index",
            SourceUrl = "https://www.whocc.no/atc_ddd_index/",
            MappingStatus = "Đã ánh xạ mẫu",
            LastSyncDate = new DateOnly(2026, 5, 25),
            Purpose = "Phân loại nhóm điều trị và hỗ trợ lọc ứng viên thay thế."
        }
    ];

    public List<AuditLogEntry> AuditLogs { get; } =
    [
        new()
        {
            Id = 1,
            CreatedAt = new DateTimeOffset(2026, 5, 26, 9, 10, 0, TimeSpan.FromHours(7)),
            Actor = "System",
            Action = "Khởi tạo dữ liệu",
            Entity = "Danh mục",
            Detail = "Khởi tạo dữ liệu thuốc, lô thuốc và hoạt chất."
        },
        new()
        {
            Id = 2,
            CreatedAt = new DateTimeOffset(2026, 5, 26, 11, 30, 0, TimeSpan.FromHours(7)),
            Actor = "QC",
            Action = "Kiểm thử hồi quy",
            Entity = "Tra cứu",
            Detail = "Bộ kiểm thử chấp nhận nền tảng đạt 20/20."
        },
        new()
        {
            Id = 3,
            CreatedAt = new DateTimeOffset(2026, 5, 26, 14, 5, 0, TimeSpan.FromHours(7)),
            Actor = "System",
            Action = "Kiểm thử hiệu năng",
            Entity = "Ứng dụng",
            Detail = "Bộ kiểm thử hiệu năng thời gian thực đạt 8/8."
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
            Reviewer = "Chưa gán",
            Note = "Cùng hoạt chất, cùng hàm lượng, còn hàng.",
            UpdatedAt = new DateTimeOffset(2026, 6, 20, 10, 0, 0, TimeSpan.FromHours(7))
        },
        new()
        {
            Id = 2,
            SourceDrugId = 1,
            RecommendedDrugId = 5,
            Score = 80,
            Status = "Can xem xet",
            Reviewer = "Chuyên gia mẫu",
            Note = "Cùng hoạt chất và hàm lượng nhưng khác dạng bào chế.",
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
