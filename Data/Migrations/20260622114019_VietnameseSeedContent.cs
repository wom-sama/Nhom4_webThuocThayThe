using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nhom4WebThuocThayThe.Data.Migrations
{
    /// <inheritdoc />
    public partial class VietnameseSeedContent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                UPDATE [Categories] SET [Name] = N'Giảm đau hạ sốt', [Description] = N'Thuốc điều trị đau, sốt thông thường.' WHERE [Id] = 1;
                UPDATE [Categories] SET [Name] = N'Kháng sinh', [Description] = N'Thuốc cần kê đơn và cần xác nhận chuyên môn.' WHERE [Id] = 2;
                UPDATE [Categories] SET [Name] = N'Tiêu hóa', [Description] = N'Thuốc hỗ trợ hệ tiêu hóa.' WHERE [Id] = 3;
                UPDATE [Categories] SET [Name] = N'Dị ứng', [Description] = N'Thuốc hỗ trợ triệu chứng dị ứng.' WHERE [Id] = 4;

                UPDATE [DosageForms] SET [Name] = N'Viên nén' WHERE [Id] = 1;
                UPDATE [DosageForms] SET [Name] = N'Viên nang' WHERE [Id] = 2;
                UPDATE [DosageForms] SET [Name] = N'Viên sủi' WHERE [Id] = 4;
                UPDATE [Units] SET [Name] = N'Viên' WHERE [Id] = 1;
                UPDATE [Units] SET [Name] = N'Hộp' WHERE [Id] = 2;
                UPDATE [Manufacturers] SET [Country] = N'Việt Nam' WHERE [Id] IN (1, 3, 4);

                UPDATE [ActiveIngredients] SET [Warning] = N'Thận trọng với bệnh gan.' WHERE [Id] = 1;
                UPDATE [ActiveIngredients] SET [Warning] = N'Thận trọng với đau dạ dày.' WHERE [Id] = 2;
                UPDATE [ActiveIngredients] SET [Warning] = N'Kháng sinh cần kê đơn.' WHERE [Id] = 3;
                UPDATE [ActiveIngredients] SET [Warning] = N'Có thể gây buồn ngủ.' WHERE [Id] = 4;

                UPDATE [Drugs] SET [Description] = N'Thuốc giảm đau hạ sốt chứa paracetamol.', [Usage] = N'Dùng theo hướng dẫn của dược sĩ.', [Contraindications] = N'Quá mẫn với paracetamol.' WHERE [Id] = 1;
                UPDATE [Drugs] SET [Description] = N'Thuốc giảm đau hạ sốt thay thế cùng hoạt chất.', [Usage] = N'Dùng theo hướng dẫn của dược sĩ.', [Contraindications] = N'Quá mẫn với paracetamol.' WHERE [Id] = 2;
                UPDATE [Drugs] SET [Description] = N'Thuốc giảm đau kháng viêm không steroid.', [Usage] = N'Dùng sau ăn.', [Contraindications] = N'Loét dạ dày tiến triển.' WHERE [Id] = 3;
                UPDATE [Drugs] SET [Description] = N'Kháng sinh beta-lactam.', [Usage] = N'Dùng theo đơn bác sĩ.', [Contraindications] = N'Dị ứng penicillin.' WHERE [Id] = 4;
                UPDATE [Drugs] SET [Description] = N'Thuốc giảm đau hạ sốt dạng viên sủi.', [Usage] = N'Hòa tan trong nước trước khi dùng.', [Contraindications] = N'Quá mẫn với paracetamol.' WHERE [Id] = 5;
                UPDATE [Drugs] SET [Description] = N'Thuốc giảm đau hạ sốt hàm lượng cao hơn.', [Usage] = N'Dùng theo hướng dẫn của dược sĩ.', [Contraindications] = N'Thận trọng với người bệnh gan.' WHERE [Id] = 6;
                UPDATE [Drugs] SET [Description] = N'Thuốc kháng histamine hỗ trợ triệu chứng dị ứng.', [Usage] = N'Dùng một lần mỗi ngày theo hướng dẫn.', [Contraindications] = N'Quá mẫn với cetirizine.' WHERE [Id] = 7;

                UPDATE [Warehouses] SET [Name] = N'Kho trung tâm', [Address] = N'Quận 1' WHERE [Id] = 1;
                UPDATE [Warehouses] SET [Name] = N'Quầy bán lẻ', [Address] = N'Nhà thuốc số 1' WHERE [Id] = 2;
                UPDATE [PatientSafetyProfiles] SET [DisplayName] = N'Người dùng mặc định', [ClinicalNote] = N'Dị ứng với nhóm NSAID, ưu tiên cảnh báo ibuprofen.' WHERE [Email] = N'user@nhom4.local';
                UPDATE [PatientSafetyProfiles] SET [DisplayName] = N'Hồ sơ kiểm thử dược sĩ', [ClinicalNote] = N'Hồ sơ mẫu dùng để kiểm tra cảnh báo paracetamol.' WHERE [Email] = N'duocsi@nhom4.local';

                UPDATE [ExternalDataSources] SET [MappingStatus] = N'Sẵn sàng ánh xạ', [Purpose] = N'Thông tin thuốc, hoạt chất, chỉ định và tương tác.' WHERE [Id] = 1;
                UPDATE [ExternalDataSources] SET [MappingStatus] = N'Đang đánh giá', [Purpose] = N'Đối chiếu hợp chất, CID và cấu trúc hóa học.' WHERE [Id] = 2;
                UPDATE [ExternalDataSources] SET [MappingStatus] = N'Đã ánh xạ mẫu', [Purpose] = N'Phân loại nhóm điều trị và hỗ trợ lọc ứng viên thay thế.' WHERE [Id] = 3;

                UPDATE [ExpertReviews] SET [Reviewer] = N'Chưa gán', [Note] = N'Cùng hoạt chất, cùng hàm lượng, còn hàng.' WHERE [Id] = 1;
                UPDATE [ExpertReviews] SET [Reviewer] = N'Chuyên gia mẫu', [Note] = N'Cùng hoạt chất và hàm lượng nhưng khác dạng bào chế.' WHERE [Id] = 2;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                UPDATE [Categories] SET [Name] = N'Giam dau ha sot', [Description] = N'Thuoc dieu tri dau, sot thong thuong.' WHERE [Id] = 1;
                UPDATE [Categories] SET [Name] = N'Khang sinh', [Description] = N'Thuoc can ke don va can xac nhan chuyen mon.' WHERE [Id] = 2;
                UPDATE [Categories] SET [Name] = N'Tieu hoa', [Description] = N'Thuoc ho tro he tieu hoa.' WHERE [Id] = 3;
                UPDATE [Categories] SET [Name] = N'Di ung', [Description] = N'Thuoc ho tro trieu chung di ung.' WHERE [Id] = 4;
                UPDATE [DosageForms] SET [Name] = N'Vien nen' WHERE [Id] = 1;
                UPDATE [DosageForms] SET [Name] = N'Vien nang' WHERE [Id] = 2;
                UPDATE [DosageForms] SET [Name] = N'Vien sui' WHERE [Id] = 4;
                UPDATE [Units] SET [Name] = N'Vien' WHERE [Id] = 1;
                UPDATE [Units] SET [Name] = N'Hop' WHERE [Id] = 2;
                """);
        }
    }
}
