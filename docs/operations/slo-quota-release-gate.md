# SLO, quota và release gate production

Tài liệu này là tiêu chuẩn nội bộ cho bản phát hành trên Somee Free. Somee Free không công bố SLA, CPU, RAM hoặc số kết nối đồng thời được bảo đảm; vì vậy nhóm chỉ cam kết các chỉ số do chính nhóm đo và không quảng bá một con số người dùng đồng thời tuyệt đối.

## Baseline môi trường

| Hạng mục | Giá trị đã xác nhận |
|---|---:|
| Web storage | 150 MB |
| Guardrail gói publish | Tối đa 145 MB |
| Gói hiện tại | 85 file, 19,86 MB |
| Băng thông tháng | 5 GB |
| SQL database | 30 MB data và 30 MB log |
| HTTPS | Hoạt động; HSTS được trả về |
| Runtime | ASP.NET Core trên IIS 10 / Windows Server 2022 |

Giới hạn trên phải được kiểm tra lại trong control panel trước mỗi release vì gói miễn phí có thể thay đổi. Không suy ra concurrency từ storage hoặc bandwidth.

## SLO quan sát nội bộ

| Chỉ số | Mục tiêu | Cửa sổ và cách đo |
|---|---:|---|
| Health availability | >= 99% mẫu thành công | Probe `/health` mỗi 15 phút trong 7 ngày; planned maintenance được ghi riêng |
| Health latency | p95 <= 1.500 ms | Cùng cửa sổ probe, HTTPS từ máy giám sát |
| Public route error rate | <= 1% | Trang chủ, tìm kiếm và chi tiết thuốc; không tính 4xx do request sai |
| PERF10 production | 0% error, p95 <= 1.500 ms, >= 5 req/s | 10 virtual users, pacing 250 ms, 120 request |
| AI explanation | Có kết quả hoặc fallback <= 10 giây | Không gửi hồ sơ bệnh nhân; deterministic score vẫn là nguồn quyết định |
| Recovery time objective | <= 10 phút cho rollback ứng dụng | Đo từ maintenance đến health/database connected |
| Recovery point objective | <= 24 giờ cho dữ liệu demo | Backup trước release và sau thay đổi dữ liệu quan trọng |

Baseline S7 ngày 22/06/2026: rollback `v1.0.0` 468,6 giây; restore `develop` 472,8 giây; cả hai 0 retry. PERF10 production gần nhất: p50 352 ms, p95 1.100 ms, max 2.325 ms, 0% error, 12,69 req/s.

## Quota guardrail

- Chặn deploy nếu publish lớn hơn 145 MB; mục tiêu vận hành là dưới 100 MB để còn headroom cho host.
- Cảnh báo khi web storage hoặc SQL data/log đạt 70%; no-go ở 85% nếu chưa có kế hoạch dọn hoặc nâng gói.
- Cảnh báo băng thông ở 70% quota tháng; tắt tải không thiết yếu và giảm cache miss trước khi đạt 90%.
- Static asset phải có compression, ETag/revalidation và cache public bảy ngày khi phù hợp.
- Artifact có `web.config` production, connection string hoặc secret chỉ lưu cục bộ trong cửa sổ rollback, sau đó xóa an toàn.
- AI phải có timeout, rate limit, cache và fallback; lỗi Gemini không được làm hỏng luồng đề xuất thuốc xác định.

## Release gate bắt buộc

| Gate | Điều kiện go |
|---|---|
| Scope | Jira issue có acceptance criteria; UI có Figma URL/version và owner `APPROVED` trước khi code |
| Source control | Worktree sạch; branch có Jira key; PR vào `develop`; reviewer độc lập approve |
| Build/lint | Release build 0 warning/0 error; PSScriptAnalyzer 0 finding cho script vận hành |
| Unit | 24/24 hoặc cao hơn; không giảm test hiện có |
| Integration | 3/3 hoặc cao hơn với SQL Server thật/container |
| System/Acceptance | 34/34 hoặc cao hơn; có RBAC, restart persistence và static asset |
| Security | 17/17 hoặc cao hơn; không còn P0/P1, secret scan sạch |
| Performance | 10/10 local và PERF10 production đạt SLO |
| UAT/UI | Chrome, Edge, Firefox; desktop/mobile; keyboard; role matrix; không có lỗi blocker/critical |
| Database | Migration idempotent; backup trước release; restore drill có RPO/RTO và người xác nhận |
| Deploy | Dry-run pass; package <= 145 MB; manifest SHA-256; backup; `web.config` upload cuối |
| Post-deploy | HTTPS health/database, root/search/detail, RBAC, security headers và AI/fallback pass |
| Rollback | Tag trước còn dùng được; lệnh, owner, ngưỡng rollback và bằng chứng được ghi trong Jira |

Bất kỳ gate nào chưa có bằng chứng đều là **no-go**, không phải “pass có điều kiện”. Nam là người quyết định release; reviewer/QC có quyền chặn khi có P0/P1 hoặc bằng chứng không đầy đủ.

## Quy trình go/no-go

1. Chủ issue gắn commit/PR/test evidence lên Jira.
2. Vũ tổng hợp defect mở theo severity; Đạt xác nhận security/database; Tân xác nhận package/deploy; Tú xác nhận UAT/UI.
3. Nam ghi comment go/no-go, release commit, tag và rollback tag.
4. Chỉ merge `develop` vào `main` sau go; tag/release trước khi deploy.
5. Sau deploy, chạy smoke và PERF10. Nếu health thất bại, error rate > 1%, sai role, mất dữ liệu hoặc P0/P1 xuất hiện, rollback ngay.
6. Mở sprint hậu kiểm để theo dõi SLO bảy ngày, regression production, quota và post-release review.

## Bằng chứng lưu giữ

- GitHub: PR, approval, Actions run, tag và release notes.
- Jira: trạng thái, assignee, quyết định, test summary, defect và timeline sự cố.
- `artifacts/` cục bộ: manifest, report JSON và backup ngắn hạn; không commit secret.
- Báo cáo kết thúc sprint phải phân biệt rõ test local, test container và test production.
