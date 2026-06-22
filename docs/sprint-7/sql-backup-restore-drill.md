# Báo cáo backup/restore SQL và RPO/RTO S7

- Người thực hiện: Đạt
- Ngày thực hiện: 22/06/2026, UTC+7
- Engine: SQL Server 2022 Linux container
- Bằng chứng máy đọc: [`assets/sql-restore-drill.json`](assets/sql-restore-drill.json)

## Mục tiêu và an toàn

Drill xác nhận backup SQL Server của ứng dụng có thể verify và restore sang database mới mà không ghi đè production. Credential dùng trong container là dữ liệu test cô lập; report không chứa password hoặc connection string.

## Quy trình

1. Khởi động SQL Server 2022 container tách biệt trên localhost.
2. Build ứng dụng Release `0 warning / 0 error`.
3. Cho ứng dụng chạy migration và seed database nguồn `N4WTT_Restore_Source`.
4. Tạo backup với `CHECKSUM` và `COMPRESSION`.
5. Chạy `RESTORE VERIFYONLY ... WITH CHECKSUM`.
6. Restore sang database mới `N4WTT_Restore_Validation` bằng logical data/log name lấy từ source.
7. Chạy `DBCC CHECKDB` trên database đích.
8. So sánh số row nguồn/đích và thử kết nối/query tới database đích.
9. Sao chép backup vào `artifacts/` để tính SHA-256, sau đó xóa container trong `finally`.

## Kết quả

| Kiểm tra | Kết quả |
|---|---:|
| Release build | Pass, 0 warning / 0 error |
| Backup checksum | Pass |
| `RESTORE VERIFYONLY` | Pass |
| Restore sang DB mới | Pass |
| `DBCC CHECKDB` | Pass |
| Row count nguồn | Drugs 7, Batches 7, Categories 4, Migrations 1 |
| Row count đích | Drugs 7, Batches 7, Categories 4, Migrations 1 |
| Đối chiếu row count | Match |
| Dung lượng backup | 528.384 byte |
| RTO restore đo được | 0,815 giây |
| Thời gian kết nối sau restore | 0,127 giây |
| Secret trong evidence | Không |

SHA-256 được lưu trong JSON để kiểm tra tính toàn vẹn. File `.bak` nằm trong `artifacts/`, bị Git ignore và không được đính kèm Jira/GitHub.

## RPO/RTO

- **RPO đo trong drill:** 0 đối với transaction đã commit trước thời điểm backup; row counts nguồn/đích khớp.
- **RPO vận hành mục tiêu:** tối đa 24 giờ cho dữ liệu demo. Cần backup trước release và sau thay đổi dữ liệu quan trọng.
- **RTO drill local:** 0,815 giây cho restore + 0,127 giây để query.
- **RTO production mục tiêu:** 10 phút cho rollback ứng dụng. RTO database trên Somee chưa thể suy ra từ container vì phụ thuộc control panel, quota và quyền restore của host.

## Giới hạn và bước production

- Không restore đè database Somee; đây là yêu cầu an toàn bắt buộc.
- Trước production release, Đạt phải xác nhận phương thức export/backup của Somee, dung lượng database/log dưới guardrail và quyền tạo database đích hoặc yêu cầu host hỗ trợ.
- Một bản backup chưa được xem là hợp lệ nếu chưa `VERIFYONLY`, chưa có hash hoặc chưa từng restore thử sang database tách biệt.
- Mọi restore production phải có Nam phê duyệt, backup hiện tại, maintenance window và QC regression sau khôi phục.
