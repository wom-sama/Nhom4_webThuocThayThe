# Runbook hỗ trợ và ma trận escalation

Tài liệu này áp dụng cho môi trường production `https://nnhom4web.somee.com`. Mục tiêu là xử lý sự cố có bằng chứng, hạn chế thay đổi thiếu kiểm soát và luôn có người chịu trách nhiệm quyết định.

## Vai trò trực vận hành

| Thành viên | Vai trò chính | Quyền và trách nhiệm khi có sự cố |
|---|---|---|
| Nam | Incident Lead, Product Owner | Xác định mức độ, điều phối, quyết định rollback/release và đóng sự cố |
| Tân | Backend/Deployment | Chẩn đoán ứng dụng, thực hiện deploy hoặc rollback theo runbook |
| Tú | Frontend/UI | Xác minh luồng người dùng, responsive, accessibility và thông báo lỗi trên giao diện |
| Đạt | Database/Security | Kiểm tra SQL, secret, audit log, backup/restore và dấu hiệu bảo mật |
| Vũ | QC | Tái hiện, ghi defect Jira, regression và xác nhận kết quả sau khắc phục |

Không chia sẻ token, mật khẩu, connection string, `web.config` production hoặc dữ liệu người dùng trong Jira/GitHub.

## Phân loại severity

| Mức | Tiêu chí | Phản hồi ban đầu | Mục tiêu khôi phục | Quyền quyết định |
|---|---|---:|---:|---|
| P0 | Mất toàn bộ dịch vụ, rò rỉ secret/dữ liệu, sai phân quyền diện rộng | 15 phút | 60 phút | Nam; Đạt có quyền yêu cầu dừng dịch vụ ngay |
| P1 | Chức năng lõi tìm kiếm/đề xuất/đăng nhập lỗi, dữ liệu không nhất quán | 30 phút | 4 giờ | Nam sau ý kiến Tân/Đạt |
| P2 | Một luồng phụ lỗi, có phương án tạm, không mất dữ liệu | 4 giờ làm việc | 2 ngày làm việc | Chủ story và Nam |
| P3 | Lỗi giao diện, nội dung hoặc cải tiến không chặn người dùng | 1 ngày làm việc | Đưa vào sprint kế tiếp | Product Owner |

Các mục tiêu trên là mục tiêu nội bộ. Somee Free không cung cấp SLA, vì vậy không được trình bày chúng như cam kết của nhà cung cấp.

## Luồng xử lý sự cố

1. **Phát hiện:** Vũ hoặc người phát hiện tạo Bug trên Jira, ghi thời điểm UTC+7, URL, tài khoản/role thử nghiệm, bước tái hiện, expected/actual và bằng chứng đã loại bỏ dữ liệu nhạy cảm.
2. **Triage:** Nam gán severity, incident lead, người xử lý và người xác nhận độc lập. P0/P1 phải có comment quyết định trong Jira.
3. **Ổn định:** Tân dừng release mới. Nếu dữ liệu hoặc secret có nguy cơ, Đạt vô hiệu hóa luồng liên quan trước khi điều tra sâu.
4. **Chẩn đoán:** Ghi commit/tag production, `/health`, route lỗi, log IIS đã làm sạch, trạng thái SQL, thay đổi gần nhất và phạm vi người dùng bị ảnh hưởng.
5. **Quyết định:** Ưu tiên rollback về tag đã xác nhận nếu không có bản sửa nhỏ, có test và có người review trong mục tiêu khôi phục.
6. **Khắc phục:** Mọi sửa đổi đi qua branch, commit có Jira key, PR, CI và review chéo. Không sửa trực tiếp file trên Somee ngoài runbook khẩn cấp đã được Nam phê duyệt.
7. **Xác nhận:** Vũ chạy regression theo phạm vi; Đạt xác nhận SQL/security; Tân xác nhận health; Nam chấp nhận increment.
8. **Đóng:** Jira phải có nguyên nhân gốc, thời gian phát hiện/phản hồi/khôi phục, bằng chứng test, commit/PR/release và hành động phòng ngừa.

## Escalation theo tín hiệu

| Tín hiệu | Người nhận đầu tiên | Escalation tiếp theo |
|---|---|---|
| `/health` lỗi hoặc database disconnected | Tân | Đạt sau 10 phút; Nam ngay nếu kéo dài 15 phút |
| HTTP 5xx tăng hoặc chức năng lõi lỗi | Tân và Vũ | Nam sau khi tái hiện hoặc ngay khi là P0 |
| Sai role, CSRF, lộ secret, truy cập trái phép | Đạt và Nam ngay | Dừng release; rotate/revoke theo kế hoạch secret |
| SQL đầy, lỗi migration, dữ liệu sai | Đạt | Nam và Tân trước mọi restore/migration |
| Layout vỡ, keyboard/accessibility regression | Tú và Vũ | Nam nếu chặn UAT hoặc luồng lõi |
| Hết quota/băng thông Somee | Nam và Tân | Giảm traffic/tính năng phụ; đánh giá nâng hosting |

## Kiểm tra tối thiểu sau khôi phục

- HTTPS `/health` trả `200` và `database=connected`.
- Trang chủ, `/Drugs?keyword=para` và `/Drugs/Details/1` trả `200`.
- Anonymous bị chuyển hướng khỏi trang quản trị; Admin truy cập được; User bị từ chối đúng role.
- Không mất HSTS, CSP, `X-Frame-Options=DENY` và `X-Content-Type-Options=nosniff`.
- Một lời giải thích AI không chứa email, secret hoặc HTML thực thi; fallback vẫn hoạt động khi Gemini lỗi.
- PERF10 không có error và p95 không vượt release gate hiện hành.

## Bằng chứng và diễn tập

- Jira là nguồn theo dõi công việc; GitHub là nguồn code/review/CI; thư mục `artifacts/` chỉ giữ bằng chứng cục bộ có thể chứa secret và không được commit.
- Diễn tập rollback ít nhất trước mỗi major release. Lần S7 ngày 22/06/2026: rollback `v1.0.0` đạt RTO 468,6 giây; restore `develop` đạt 472,8 giây; 0 retry; health/database kết nối.
- Sau P0/P1, tạo post-incident review trong vòng hai ngày làm việc và biến hành động phòng ngừa thành issue có owner, deadline và acceptance criteria.
