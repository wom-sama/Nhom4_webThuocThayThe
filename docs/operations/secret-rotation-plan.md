# Kế hoạch quản lý và luân phiên secret

Tài liệu này quản lý vòng đời credential cho Jira, GitHub, Gemini, Somee và SQL. Không ghi giá trị secret, một phần token, ảnh chụp control panel hoặc connection string production vào GitHub/Jira.

## Danh mục và ownership

| Secret | Mục đích | Owner vận hành | Quyền tối thiểu | Chu kỳ tối đa |
|---|---|---|---|---:|
| Jira API token từng thành viên | Tạo/cập nhật issue bằng đúng danh tính | Từng thành viên; Nam audit | Chỉ site/project cần thiết | 90 ngày |
| GitHub PAT từng thành viên | Push branch, tạo/review PR | Từng thành viên; Nam quản trị repo | Contents/PR theo vai trò; workflow chỉ khi cần | 90 ngày |
| Gemini API key | Gọi AI explanation từ server | Đạt; Nam dự phòng | Chỉ API/project demo, có quota | 60 ngày |
| Somee control panel/FTP | Deploy và xem log | Nam; Tân deploy | Website hiện tại, không chia sẻ tài khoản | 90 ngày hoặc theo host |
| SQL production credential | Migration và runtime database | Đạt | Database ứng dụng; tách runtime/admin khi host cho phép | 90 ngày |

Luân phiên ngay, không chờ chu kỳ, khi credential xuất hiện trong log/ảnh/chat/commit, thiết bị hoặc thành viên thay đổi, có truy cập bất thường, hoặc quyền hiện tại lớn hơn nhu cầu.

## Nơi lưu và truyền secret

- Chỉ lưu trong credential manager/secret store của máy hoặc file cục bộ ngoài repository với ACL giới hạn cho tài khoản Windows hiện tại.
- Dùng environment variable hoặc file cấu hình không track để đưa secret vào process. Không truyền giá trị trực tiếp trong command line, PR, Jira comment hoặc automation prompt.
- `.gitignore` chặn `*TK.txt`, `*GitHubPAT.txt`, `someeDeploy.txt` và `geminiKey.txt`; đây là lớp phòng thủ bổ sung, không thay thế secret scan.
- `artifacts/somee-deploy-staging` và backup `web.config` có thể chứa secret runtime. Chỉ giữ trong cửa sổ rollback, không upload Jira/GitHub và xóa sau release review.
- Credential SQL trong GitHub Actions là dữ liệu test cô lập cho container CI, không dùng lại ở production.

## Quy trình rotate chuẩn

1. Tạo issue Jira với owner, secret type, lý do, thời hạn và người xác nhận; không ghi giá trị cũ/mới.
2. Kiểm tra nơi đang sử dụng: máy thành viên, automation, GitHub Actions, Somee runtime, tài liệu và backup cục bộ.
3. Tạo credential mới với quyền tối thiểu và ngày hết hạn. Không revoke credential cũ trước khi có đường rollback, trừ incident P0.
4. Cập nhật secret store/file cục bộ và runtime production. Không commit file đã cập nhật.
5. Chạy kiểm tra chức năng nhỏ nhất: Jira `myself`, GitHub đọc repo/push branch thử, Gemini request không PII, Somee dry-run/health, SQL `SELECT 1` và migration status.
6. Revoke credential cũ ngay sau khi kiểm tra mới pass; xác nhận credential cũ không còn hoạt động.
7. Kiểm tra audit log, secret scan và production health. Ghi thời điểm, người thực hiện, kết quả pass/fail và ticket liên quan, không ghi secret.
8. Nếu thất bại, rollback cấu hình dùng credential cũ còn hiệu lực, xử lý nguyên nhân rồi lặp lại; không để song song hai credential lâu hơn cửa sổ đã duyệt.

## Playbook theo hệ thống

### Jira

- Mỗi người dùng token riêng; không dùng token của Nam cho công việc của thành viên khác.
- Kiểm tra `GET /rest/api/3/myself`, quyền browse/create/transition/comment trên `N4WTT`.
- Revoke token cũ trong Atlassian account và kiểm tra request cũ trả unauthorized.

### GitHub

- Fine-grained PAT chỉ giới hạn repo `wom-sama/Nhom4_webThuocThayThe` khi tài khoản có thể chọn repo đó.
- Tân/Tú/Đạt cần Contents và Pull requests; chỉ owner dùng Administration. Quyền Workflows chỉ cấp cho tác vụ thực sự sửa `.github/workflows`.
- Kiểm tra branch push/PR bằng branch tạm có Jira key, sau đó xóa branch qua PR workflow.

### Gemini

- Giới hạn key theo Google project/API/quota khi nền tảng hỗ trợ; chỉ server đọc key.
- Sau rotate, chạy live smoke không chứa hồ sơ bệnh nhân và xác nhận browser response không lộ key/header xác thực.
- Khi key lỗi hoặc hết quota, deterministic scoring và fallback phải tiếp tục hoạt động.

### Somee và SQL

- Đổi credential trong control panel/SQL trước, sau đó cập nhật file deploy cục bộ và runtime `web.config` qua script.
- Chạy dry-run, deploy có maintenance, upload `web.config` cuối và health gate HTTPS.
- SQL credential mới phải kết nối đúng database, chạy migration status và truy cập theo quyền ứng dụng; credential cũ bị từ chối sau revoke.

## Quy trình khẩn cấp P0

1. Đạt thông báo Nam, ghi thời điểm phát hiện và phạm vi nghi ngờ.
2. Revoke/disable credential bị lộ ngay; dừng AI hoặc website nếu cần để ngăn truy cập tiếp tục.
3. Tạo credential mới từ thiết bị tin cậy, cập nhật runtime và chạy security/RBAC/health smoke.
4. Tìm và xóa secret khỏi nơi công khai. Nếu đã vào Git history, rotate vẫn là bắt buộc; rewrite history chỉ thực hiện sau đánh giá ảnh hưởng clone/fork.
5. Kiểm tra audit log từ thời điểm lộ đến lúc revoke, tạo defect/incident cho mọi hành vi bất thường.
6. Post-incident review trong hai ngày làm việc với nguyên nhân gốc và hành động phòng ngừa.

## Bằng chứng hoàn tất

- Ticket Jira có owner, loại secret, thời điểm rotate/revoke và kết quả kiểm tra.
- Credential mới đạt least privilege; credential cũ bị từ chối.
- Git status sạch; secret scan không tìm thấy token, key, password production hoặc connection string production.
- `/health`, RBAC, Gemini/fallback và SQL connectivity đạt sau rotate.
- Không có secret trong Jira attachment, PR body, CI log hoặc artifact đã commit.
