using Nhom4WebThuocThayThe.Models;

namespace Nhom4WebThuocThayThe.Data;

public sealed class InMemoryPharmacyStore
{
    private int _nextDrugId = 116;
    private int _nextBatchId = 116;
    private int _nextAuditLogId = 108;

    public List<DrugCategory> Categories { get; } =
    [
        new() { Id = 1, Name = "Giảm đau hạ sốt", Description = "Thuốc điều trị đau, sốt thông thường." },
        new() { Id = 2, Name = "Kháng sinh", Description = "Thuốc cần kê đơn và cần xác nhận chuyên môn." },
        new() { Id = 3, Name = "Tiêu hóa", Description = "Thuốc hỗ trợ hệ tiêu hóa." },
        new() { Id = 4, Name = "Dị ứng", Description = "Thuốc hỗ trợ triệu chứng dị ứng." },
        new() { Id = 5, Name = "Tim mạch", Description = "Thuốc điều trị huyết áp và nguy cơ tim mạch thường gặp." },
        new() { Id = 6, Name = "Hô hấp", Description = "Thuốc hỗ trợ triệu chứng đường hô hấp và hen phế quản." },
        new() { Id = 7, Name = "Vitamin - khoáng chất", Description = "Sản phẩm bổ sung vitamin, điện giải và khoáng chất." },
        new() { Id = 8, Name = "Sát khuẩn và da liễu", Description = "Thuốc dùng ngoài, sát khuẩn và chăm sóc da." },
        new() { Id = 9, Name = "Nội tiết - chuyển hóa", Description = "Thuốc hỗ trợ bệnh lý chuyển hóa cần theo dõi dài hạn." }
    ];

    public List<DosageForm> DosageForms { get; } =
    [
        new() { Id = 1, Name = "Viên nén" },
        new() { Id = 2, Name = "Viên nang" },
        new() { Id = 3, Name = "Siro" },
        new() { Id = 4, Name = "Viên sủi" },
        new() { Id = 5, Name = "Viên bao phim" },
        new() { Id = 6, Name = "Dung dịch uống" },
        new() { Id = 7, Name = "Gói bột" },
        new() { Id = 8, Name = "Dung dịch dùng ngoài" },
        new() { Id = 9, Name = "Bình xịt/khí dung" }
    ];

    public List<MeasurementUnit> Units { get; } =
    [
        new() { Id = 1, Name = "Viên" },
        new() { Id = 2, Name = "Hộp" },
        new() { Id = 3, Name = "Chai" },
        new() { Id = 4, Name = "Gói" },
        new() { Id = 5, Name = "Lọ" },
        new() { Id = 6, Name = "Tuýp" },
        new() { Id = 7, Name = "Bình" }
    ];

    public List<Manufacturer> Manufacturers { get; } =
    [
        new() { Id = 1, Name = "DHG Pharma", Country = "Việt Nam" },
        new() { Id = 2, Name = "Sanofi", Country = "France" },
        new() { Id = 3, Name = "Traphaco", Country = "Việt Nam" },
        new() { Id = 4, Name = "Imexpharm", Country = "Việt Nam" },
        new() { Id = 5, Name = "Stella Pharm", Country = "Việt Nam" },
        new() { Id = 6, Name = "Mekophar", Country = "Việt Nam" },
        new() { Id = 7, Name = "Pfizer", Country = "United States" },
        new() { Id = 8, Name = "OPC", Country = "Việt Nam" },
        new() { Id = 9, Name = "Boston Pharma", Country = "Việt Nam" }
    ];

    public List<ActiveIngredient> ActiveIngredients { get; } =
    [
        new() { Id = 1, Name = "Paracetamol", Warning = "Thận trọng với bệnh gan." },
        new() { Id = 2, Name = "Ibuprofen", Warning = "Thận trọng với đau dạ dày." },
        new() { Id = 3, Name = "Amoxicillin", Warning = "Kháng sinh cần kê đơn." },
        new() { Id = 4, Name = "Cetirizine", Warning = "Có thể gây buồn ngủ." },
        new() { Id = 5, Name = "Omeprazole", Warning = "Dùng đúng thời điểm, thận trọng khi dùng kéo dài." },
        new() { Id = 6, Name = "Esomeprazole", Warning = "Theo dõi tương tác thuốc khi dùng dài ngày." },
        new() { Id = 7, Name = "Loratadine", Warning = "Thận trọng với người suy gan nặng." },
        new() { Id = 8, Name = "Azithromycin", Warning = "Kháng sinh cần kê đơn, thận trọng kéo dài QT." },
        new() { Id = 9, Name = "Metformin", Warning = "Theo dõi chức năng thận và nguy cơ hạ đường huyết." },
        new() { Id = 10, Name = "Amlodipine", Warning = "Theo dõi huyết áp, phù ngoại biên." },
        new() { Id = 11, Name = "Salbutamol", Warning = "Có thể gây run tay, hồi hộp; dùng theo chỉ định." },
        new() { Id = 12, Name = "Ascorbic acid", Warning = "Thận trọng khi dùng liều cao kéo dài." },
        new() { Id = 13, Name = "Povidone iodine", Warning = "Không dùng khi dị ứng iod hoặc rối loạn tuyến giáp nếu chưa hỏi chuyên môn." },
        new() { Id = 14, Name = "Oral rehydration salts", Warning = "Pha đúng lượng nước, không uống dung dịch quá đặc." },
        new() { Id = 15, Name = "Amoxicillin + Clavulanate", Warning = "Kháng sinh phối hợp cần kê đơn và theo dõi dị ứng beta-lactam." },
        new() { Id = 16, Name = "Berberine", Warning = "Thận trọng ở trẻ nhỏ, phụ nữ có thai và người đang dùng thuốc điều trị khác." }
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
        },
        new()
        {
            Id = 101,
            Name = "Omeprazole STADA 20mg",
            Strength = "20mg",
            Price = 2100,
            CategoryId = 3,
            DosageFormId = 2,
            UnitId = 1,
            ManufacturerId = 5,
            Description = "Thuốc ức chế bơm proton hỗ trợ triệu chứng trào ngược dạ dày.",
            Usage = "Uống trước bữa ăn theo hướng dẫn của dược sĩ hoặc bác sĩ.",
            Contraindications = "Quá mẫn với omeprazole."
        },
        new()
        {
            Id = 102,
            Name = "Esomeprazole Boston 20mg",
            Strength = "20mg",
            Price = 2600,
            CategoryId = 3,
            DosageFormId = 5,
            UnitId = 1,
            ManufacturerId = 9,
            Description = "Thuốc hỗ trợ giảm tiết acid dạ dày, dùng khi cần thay thế có kiểm soát.",
            Usage = "Uống trước ăn, không tự kéo dài thời gian dùng.",
            Contraindications = "Quá mẫn với esomeprazole hoặc benzimidazole."
        },
        new()
        {
            Id = 103,
            Name = "Loratadine 10mg",
            Strength = "10mg",
            Price = 1700,
            CategoryId = 4,
            DosageFormId = 1,
            UnitId = 1,
            ManufacturerId = 1,
            Description = "Thuốc kháng histamine thế hệ mới, ít gây buồn ngủ hơn một số lựa chọn cũ.",
            Usage = "Dùng một lần mỗi ngày theo khuyến cáo.",
            Contraindications = "Quá mẫn với loratadine."
        },
        new()
        {
            Id = 104,
            Name = "Claritin 10mg",
            Strength = "10mg",
            Price = 5200,
            CategoryId = 4,
            DosageFormId = 1,
            UnitId = 1,
            ManufacturerId = 2,
            Description = "Thuốc kháng histamine chứa loratadine, thường dùng cho viêm mũi dị ứng.",
            Usage = "Dùng theo hướng dẫn chuyên môn, không vượt liều khuyến cáo.",
            Contraindications = "Quá mẫn với loratadine."
        },
        new()
        {
            Id = 105,
            Name = "Azithromycin 500mg",
            Strength = "500mg",
            Price = 6400,
            CategoryId = 2,
            DosageFormId = 1,
            UnitId = 1,
            ManufacturerId = 4,
            PrescriptionRequired = true,
            Description = "Kháng sinh nhóm macrolide cần kê đơn.",
            Usage = "Dùng đúng liệu trình bác sĩ chỉ định.",
            Contraindications = "Quá mẫn với macrolide."
        },
        new()
        {
            Id = 106,
            Name = "Metformin 500mg",
            Strength = "500mg",
            Price = 1900,
            CategoryId = 9,
            DosageFormId = 5,
            UnitId = 1,
            ManufacturerId = 6,
            PrescriptionRequired = true,
            Description = "Thuốc điều trị đái tháo đường type 2, cần theo dõi chức năng thận.",
            Usage = "Dùng theo đơn và theo dõi đường huyết.",
            Contraindications = "Suy thận nặng hoặc nhiễm toan chuyển hóa."
        },
        new()
        {
            Id = 107,
            Name = "Amlodipine 5mg",
            Strength = "5mg",
            Price = 1600,
            CategoryId = 5,
            DosageFormId = 1,
            UnitId = 1,
            ManufacturerId = 6,
            PrescriptionRequired = true,
            Description = "Thuốc chẹn kênh canxi hỗ trợ điều trị tăng huyết áp.",
            Usage = "Dùng theo đơn, theo dõi huyết áp định kỳ.",
            Contraindications = "Hạ huyết áp nặng hoặc quá mẫn với dihydropyridine."
        },
        new()
        {
            Id = 108,
            Name = "Ventolin Inhaler 100mcg",
            Strength = "100mcg/liều",
            Price = 82000,
            CategoryId = 6,
            DosageFormId = 9,
            UnitId = 7,
            ManufacturerId = 7,
            PrescriptionRequired = true,
            Description = "Thuốc giãn phế quản dạng hít, dùng trong kiểm soát cơn co thắt phế quản.",
            Usage = "Dùng đúng kỹ thuật hít và theo chỉ định.",
            Contraindications = "Quá mẫn với salbutamol."
        },
        new()
        {
            Id = 109,
            Name = "Vitamin C DHG 500mg",
            Strength = "500mg",
            Price = 1400,
            CategoryId = 7,
            DosageFormId = 1,
            UnitId = 1,
            ManufacturerId = 1,
            Description = "Viên bổ sung vitamin C.",
            Usage = "Dùng sau ăn, tránh tự dùng liều cao kéo dài.",
            Contraindications = "Thận trọng với tiền sử sỏi thận."
        },
        new()
        {
            Id = 110,
            Name = "Povidine 10% 90ml",
            Strength = "10%",
            Price = 18500,
            CategoryId = 8,
            DosageFormId = 8,
            UnitId = 5,
            ManufacturerId = 8,
            Description = "Dung dịch sát khuẩn ngoài da chứa povidone iodine.",
            Usage = "Dùng ngoài da, tránh tiếp xúc mắt và niêm mạc rộng.",
            Contraindications = "Dị ứng iod hoặc bệnh tuyến giáp chưa được tư vấn."
        },
        new()
        {
            Id = 111,
            Name = "Oresol gói",
            Strength = "Pha 200ml",
            Price = 1200,
            CategoryId = 7,
            DosageFormId = 7,
            UnitId = 4,
            ManufacturerId = 3,
            Description = "Bột pha dung dịch bù nước và điện giải.",
            Usage = "Pha đúng lượng nước ghi trên nhãn, uống từng ngụm nhỏ.",
            Contraindications = "Không pha đặc hoặc dùng khi có chống chỉ định hạn chế natri."
        },
        new()
        {
            Id = 112,
            Name = "Augmentin 625mg",
            Strength = "500mg/125mg",
            Price = 12500,
            CategoryId = 2,
            DosageFormId = 5,
            UnitId = 1,
            ManufacturerId = 2,
            PrescriptionRequired = true,
            Description = "Kháng sinh phối hợp amoxicillin và clavulanate.",
            Usage = "Dùng theo đơn bác sĩ và hoàn tất liệu trình.",
            Contraindications = "Dị ứng beta-lactam hoặc tiền sử vàng da do thuốc."
        },
        new()
        {
            Id = 113,
            Name = "Berberin OPC 100mg",
            Strength = "100mg",
            Price = 900,
            CategoryId = 3,
            DosageFormId = 1,
            UnitId = 1,
            ManufacturerId = 8,
            Description = "Thuốc hỗ trợ triệu chứng tiêu hóa thông thường.",
            Usage = "Dùng theo hướng dẫn trên nhãn hoặc tư vấn dược sĩ.",
            Contraindications = "Không dùng cho trẻ nhỏ nếu chưa có tư vấn chuyên môn."
        },
        new()
        {
            Id = 114,
            Name = "Stadovas 5mg",
            Strength = "5mg",
            Price = 2200,
            CategoryId = 5,
            DosageFormId = 1,
            UnitId = 1,
            ManufacturerId = 5,
            PrescriptionRequired = true,
            Description = "Thuốc chứa amlodipine dùng theo đơn.",
            Usage = "Dùng hằng ngày theo chỉ định, không tự ngưng đột ngột.",
            Contraindications = "Hạ huyết áp nặng."
        },
        new()
        {
            Id = 115,
            Name = "Salbutamol Mekophar 2mg",
            Strength = "2mg",
            Price = 1300,
            CategoryId = 6,
            DosageFormId = 1,
            UnitId = 1,
            ManufacturerId = 6,
            PrescriptionRequired = true,
            Description = "Thuốc giãn phế quản đường uống cần kiểm soát liều.",
            Usage = "Dùng theo đơn, theo dõi hồi hộp hoặc run tay.",
            Contraindications = "Quá mẫn với salbutamol."
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
        new() { DrugId = 7, ActiveIngredientId = 4, Strength = "10mg" },
        new() { DrugId = 101, ActiveIngredientId = 5, Strength = "20mg" },
        new() { DrugId = 102, ActiveIngredientId = 6, Strength = "20mg" },
        new() { DrugId = 103, ActiveIngredientId = 7, Strength = "10mg" },
        new() { DrugId = 104, ActiveIngredientId = 7, Strength = "10mg" },
        new() { DrugId = 105, ActiveIngredientId = 8, Strength = "500mg" },
        new() { DrugId = 106, ActiveIngredientId = 9, Strength = "500mg" },
        new() { DrugId = 107, ActiveIngredientId = 10, Strength = "5mg" },
        new() { DrugId = 108, ActiveIngredientId = 11, Strength = "100mcg/liều" },
        new() { DrugId = 109, ActiveIngredientId = 12, Strength = "500mg" },
        new() { DrugId = 110, ActiveIngredientId = 13, Strength = "10%" },
        new() { DrugId = 111, ActiveIngredientId = 14, Strength = "Pha 200ml" },
        new() { DrugId = 112, ActiveIngredientId = 15, Strength = "500mg/125mg" },
        new() { DrugId = 113, ActiveIngredientId = 16, Strength = "100mg" },
        new() { DrugId = 114, ActiveIngredientId = 10, Strength = "5mg" },
        new() { DrugId = 115, ActiveIngredientId = 11, Strength = "2mg" }
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
        new() { Id = 7, DrugId = 7, WarehouseId = 2, BatchNumber = "CET-010", Quantity = 90, ImportedDate = new DateOnly(2026, 4, 28), ExpiryDate = new DateOnly(2027, 4, 28) },
        new() { Id = 101, DrugId = 101, WarehouseId = 1, BatchNumber = "OME-026", Quantity = 15, ImportedDate = new DateOnly(2026, 5, 28), ExpiryDate = new DateOnly(2026, 8, 15) },
        new() { Id = 102, DrugId = 102, WarehouseId = 2, BatchNumber = "ESO-202", Quantity = 120, ImportedDate = new DateOnly(2026, 5, 28), ExpiryDate = new DateOnly(2027, 9, 1) },
        new() { Id = 103, DrugId = 103, WarehouseId = 1, BatchNumber = "LOR-010", Quantity = 0, ImportedDate = new DateOnly(2026, 4, 12), ExpiryDate = new DateOnly(2027, 4, 12) },
        new() { Id = 104, DrugId = 104, WarehouseId = 2, BatchNumber = "CLA-110", Quantity = 60, ImportedDate = new DateOnly(2026, 4, 20), ExpiryDate = new DateOnly(2027, 4, 20) },
        new() { Id = 105, DrugId = 105, WarehouseId = 1, BatchNumber = "AZI-500", Quantity = 20, ImportedDate = new DateOnly(2026, 6, 1), ExpiryDate = new DateOnly(2027, 6, 1) },
        new() { Id = 106, DrugId = 106, WarehouseId = 1, BatchNumber = "MET-500", Quantity = 200, ImportedDate = new DateOnly(2026, 3, 16), ExpiryDate = new DateOnly(2027, 3, 16) },
        new() { Id = 107, DrugId = 107, WarehouseId = 2, BatchNumber = "AML-005", Quantity = 85, ImportedDate = new DateOnly(2026, 5, 8), ExpiryDate = new DateOnly(2027, 5, 8) },
        new() { Id = 108, DrugId = 108, WarehouseId = 1, BatchNumber = "VEN-100", Quantity = 12, ImportedDate = new DateOnly(2026, 6, 2), ExpiryDate = new DateOnly(2027, 2, 2) },
        new() { Id = 109, DrugId = 109, WarehouseId = 2, BatchNumber = "VTC-500", Quantity = 250, ImportedDate = new DateOnly(2026, 2, 18), ExpiryDate = new DateOnly(2027, 2, 18) },
        new() { Id = 110, DrugId = 110, WarehouseId = 2, BatchNumber = "POV-090", Quantity = 40, ImportedDate = new DateOnly(2026, 1, 28), ExpiryDate = new DateOnly(2026, 12, 28) },
        new() { Id = 111, DrugId = 111, WarehouseId = 1, BatchNumber = "ORS-200", Quantity = 0, ImportedDate = new DateOnly(2026, 6, 5), ExpiryDate = new DateOnly(2027, 6, 5) },
        new() { Id = 112, DrugId = 112, WarehouseId = 1, BatchNumber = "AUG-625", Quantity = 35, ImportedDate = new DateOnly(2026, 5, 25), ExpiryDate = new DateOnly(2027, 5, 25) },
        new() { Id = 113, DrugId = 113, WarehouseId = 2, BatchNumber = "BER-100", Quantity = 90, ImportedDate = new DateOnly(2026, 4, 3), ExpiryDate = new DateOnly(2027, 4, 3) },
        new() { Id = 114, DrugId = 114, WarehouseId = 1, BatchNumber = "STA-005", Quantity = 55, ImportedDate = new DateOnly(2026, 4, 22), ExpiryDate = new DateOnly(2027, 4, 22) },
        new() { Id = 115, DrugId = 115, WarehouseId = 2, BatchNumber = "SAL-002", Quantity = 5, ImportedDate = new DateOnly(2026, 6, 10), ExpiryDate = new DateOnly(2026, 9, 10) }
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
        },
        new()
        {
            Email = "chuyengia@nhom4.local",
            DisplayName = "Chuyên gia y tế",
            AllergyActiveIngredientIdsCsv = "8,15",
            ClinicalNote = "Ưu tiên rà kháng sinh cần kê đơn, dị ứng beta-lactam và đề xuất điểm thấp."
        },
        new()
        {
            Email = "admin@nhom4.local",
            DisplayName = "Quản trị hệ thống",
            AllergyActiveIngredientIdsCsv = "",
            ClinicalNote = "Hồ sơ vận hành dùng để kiểm tra số liệu release, audit và dữ liệu demo."
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
        },
        new()
        {
            Id = 101,
            Name = "Dược thư Quốc gia Việt Nam",
            SourceUrl = "https://dav.gov.vn/",
            MappingStatus = "Theo dõi thủ công",
            LastSyncDate = new DateOnly(2026, 6, 12),
            Purpose = "Đối chiếu thông tin kê đơn, cảnh báo an toàn và phân loại thuốc."
        },
        new()
        {
            Id = 102,
            Name = "Martindale Reference",
            SourceUrl = "https://www.medicinescomplete.com/",
            MappingStatus = "Cần giấy phép truy cập",
            LastSyncDate = null,
            Purpose = "Nguồn đối chiếu chuyên sâu cho hoạt chất, tương đương điều trị và dạng bào chế."
        },
        new()
        {
            Id = 103,
            Name = "Kho dữ liệu tồn nhà thuốc",
            SourceUrl = "internal://inventory-batches",
            MappingStatus = "Đồng bộ nội bộ",
            LastSyncDate = new DateOnly(2026, 6, 23),
            Purpose = "Tồn kho, hạn dùng và trạng thái sẵn có của từng lô thuốc."
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
        },
        new()
        {
            Id = 101,
            CreatedAt = new DateTimeOffset(2026, 6, 20, 9, 0, 0, TimeSpan.FromHours(7)),
            Actor = "Scrum Master",
            Action = "Sprint planning",
            Entity = "Jira",
            Detail = "Chốt backlog kiểm thử production, UI cleanup và release evidence."
        },
        new()
        {
            Id = 102,
            CreatedAt = new DateTimeOffset(2026, 6, 21, 10, 30, 0, TimeSpan.FromHours(7)),
            Actor = "Admin",
            Action = "Đồng bộ nguồn",
            Entity = "ExternalDataSource",
            Detail = "Đánh dấu nguồn DrugBank và ATC sẵn sàng cho quy trình so sánh."
        },
        new()
        {
            Id = 103,
            CreatedAt = new DateTimeOffset(2026, 6, 22, 8, 45, 0, TimeSpan.FromHours(7)),
            Actor = "Dược sĩ",
            Action = "Rà tồn kho",
            Entity = "Batches",
            Detail = "Phát hiện nhóm hết hàng và sắp hết cần đề xuất thay thế."
        },
        new()
        {
            Id = 104,
            CreatedAt = new DateTimeOffset(2026, 6, 22, 15, 20, 0, TimeSpan.FromHours(7)),
            Actor = "Chuyên gia",
            Action = "Duyệt đề xuất",
            Entity = "ExpertReviews",
            Detail = "Chấp nhận đề xuất Loratadine và yêu cầu xem xét lại kháng sinh."
        },
        new()
        {
            Id = 105,
            CreatedAt = new DateTimeOffset(2026, 6, 23, 9, 10, 0, TimeSpan.FromHours(7)),
            Actor = "QA",
            Action = "Production bug bash",
            Entity = "Role UI",
            Detail = "Ghi nhận lỗi menu chuyên gia mở cùng một màn hình và backlog S16."
        },
        new()
        {
            Id = 106,
            CreatedAt = new DateTimeOffset(2026, 6, 23, 10, 40, 0, TimeSpan.FromHours(7)),
            Actor = "UX",
            Action = "Figma-first update",
            Entity = "Design",
            Detail = "Bổ sung board Expert flow, Data/AI/Policy states trước khi code."
        },
        new()
        {
            Id = 107,
            CreatedAt = new DateTimeOffset(2026, 6, 23, 11, 20, 0, TimeSpan.FromHours(7)),
            Actor = "Release",
            Action = "Chuẩn bị triển khai",
            Entity = "Somee",
            Detail = "Chuẩn bị kiểm thử seed account, Gemini key và deployment gate."
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
        },
        new()
        {
            Id = 101,
            SourceDrugId = 4,
            RecommendedDrugId = 112,
            Score = 72,
            Status = "Can xem xet",
            Reviewer = "Chuyên gia y tế",
            Note = "Cùng nền amoxicillin nhưng có clavulanate; cần xác nhận chỉ định và tiền sử dị ứng.",
            UpdatedAt = new DateTimeOffset(2026, 6, 22, 9, 30, 0, TimeSpan.FromHours(7))
        },
        new()
        {
            Id = 102,
            SourceDrugId = 7,
            RecommendedDrugId = 103,
            Score = 68,
            Status = "Cho danh gia",
            Reviewer = "Chưa gán",
            Note = "Cùng nhóm kháng histamine, khác hoạt chất; cần đánh giá mức buồn ngủ và hồ sơ gan.",
            UpdatedAt = new DateTimeOffset(2026, 6, 22, 10, 5, 0, TimeSpan.FromHours(7))
        },
        new()
        {
            Id = 103,
            SourceDrugId = 103,
            RecommendedDrugId = 104,
            Score = 95,
            Status = "Chap nhan",
            Reviewer = "Chuyên gia y tế",
            Note = "Cùng hoạt chất loratadine 10mg, còn tồn kho và cùng dạng viên nén.",
            UpdatedAt = new DateTimeOffset(2026, 6, 22, 15, 20, 0, TimeSpan.FromHours(7))
        },
        new()
        {
            Id = 104,
            SourceDrugId = 101,
            RecommendedDrugId = 102,
            Score = 70,
            Status = "Can xem xet",
            Reviewer = "Dược sĩ trưởng",
            Note = "Cùng nhóm PPI nhưng khác hoạt chất; chỉ đề xuất khi triệu chứng và liều dùng phù hợp.",
            UpdatedAt = new DateTimeOffset(2026, 6, 22, 16, 10, 0, TimeSpan.FromHours(7))
        },
        new()
        {
            Id = 105,
            SourceDrugId = 105,
            RecommendedDrugId = 4,
            Score = 45,
            Status = "Tu choi",
            Reviewer = "Chuyên gia y tế",
            Note = "Khác nhóm kháng sinh, không thay thế nếu chưa có chỉ định bác sĩ.",
            UpdatedAt = new DateTimeOffset(2026, 6, 21, 11, 25, 0, TimeSpan.FromHours(7))
        },
        new()
        {
            Id = 106,
            SourceDrugId = 107,
            RecommendedDrugId = 114,
            Score = 100,
            Status = "Chap nhan",
            Reviewer = "Chuyên gia y tế",
            Note = "Cùng hoạt chất amlodipine 5mg, cùng dạng bào chế và còn tồn kho.",
            UpdatedAt = new DateTimeOffset(2026, 6, 22, 17, 5, 0, TimeSpan.FromHours(7))
        },
        new()
        {
            Id = 107,
            SourceDrugId = 108,
            RecommendedDrugId = 115,
            Score = 58,
            Status = "Can xem xet",
            Reviewer = "Chưa gán",
            Note = "Cùng hoạt chất salbutamol nhưng khác đường dùng; cần chuyên gia xác nhận trước tư vấn.",
            UpdatedAt = new DateTimeOffset(2026, 6, 23, 8, 45, 0, TimeSpan.FromHours(7))
        },
        new()
        {
            Id = 108,
            SourceDrugId = 111,
            RecommendedDrugId = 109,
            Score = 35,
            Status = "Tu choi",
            Reviewer = "Chuyên gia y tế",
            Note = "Khác mục tiêu điều trị, không phải lựa chọn thay thế cho bù nước điện giải.",
            UpdatedAt = new DateTimeOffset(2026, 6, 21, 14, 40, 0, TimeSpan.FromHours(7))
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
