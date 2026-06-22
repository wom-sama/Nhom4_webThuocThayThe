# Giám sát production S7

Script [`scripts/Monitor-Somee.ps1`](../../scripts/Monitor-Somee.ps1) lấy mẫu production, lưu bằng chứng cục bộ và cập nhật Jira bằng danh tính Vũ. Script không đọc Somee control panel nên không được dùng để tuyên bố quota/log mà nó không quan sát.

## Dữ liệu thu thập

- Năm vòng cho `/health`, `/`, `/Drugs?keyword=para` và `/Drugs/Details/1`.
- HTTP status, latency, response bytes, HSTS, CSP, `X-Frame-Options`.
- Trạng thái `healthy/database connected` riêng cho `/health`.
- JSON hằng ngày tại `artifacts/monitoring/YYYY-MM-DD.json`; không lưu response body, token hoặc connection string.

Một ngày pass khi toàn bộ route trả `200`, health kết nối database, header bảo mật đủ và p95 từng route không vượt 1.500 ms. Cold-start đơn lẻ không tự tạo incident nếu p95 vẫn đạt.

## Chạy thủ công

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\Monitor-Somee.ps1
```

Các chế độ kiểm tra:

```powershell
# Ghi đè report cùng ngày để retest có chủ đích.
.\scripts\Monitor-Somee.ps1 -Force

# Kiểm tra local, không comment/transition/create Bug Jira.
.\scripts\Monitor-Somee.ps1 -Rounds 1 -PacingMilliseconds 0 -SkipJira -Force
```

Nếu report cùng ngày đã tồn tại và không có `-Force`, script trả `Skipped` và không gọi Jira.

## Windows Scheduled Task

Task chạy bằng tài khoản Windows hiện tại khi người dùng đăng nhập. Action phải trỏ tới clone chính `D:\CMPM` sau khi PR chứa script đã merge.

```powershell
$action = New-ScheduledTaskAction `
  -Execute "powershell.exe" `
  -Argument '-NoProfile -ExecutionPolicy Bypass -File "D:\CMPM\scripts\Monitor-Somee.ps1"'
$trigger = New-ScheduledTaskTrigger -Daily -At 1:00PM
Register-ScheduledTask `
  -TaskName "N4WTT S7 Production Monitoring" `
  -Description "Vu QC daily production evidence for N4WTT-203" `
  -Action $action `
  -Trigger $trigger `
  -Force
```

Kiểm tra hoặc gỡ task:

```powershell
Get-ScheduledTask -TaskName "N4WTT S7 Production Monitoring" |
  Select-Object TaskName, State

Unregister-ScheduledTask -TaskName "N4WTT S7 Production Monitoring" -Confirm:$false
```

## Jira và defect

- Script comment day `N/7` lên `N4WTT-203` bằng token riêng của Vũ.
- Khi status/database/header/SLO fail, script tìm nhãn `monitoring-YYYY-MM-DD` trước khi tạo Bug để tránh trùng.
- Health/database failure là P1; header hoặc latency breach là P2; Nam nhận ticket để triage lại owner.
- `N4WTT-203` chỉ được chuyển Done khi có bảy file ngày khác nhau, không ngày nào fail và không còn P0/P1 mở trong epic S7.
- Sau đó script chuyển `N4WTT-204` sang In Progress để Vũ hoàn tất defect template/triage.

## Xử lý lỗi

- Task Scheduler có last result khác `0`: chạy script thủ công và xem JSON/log Task Scheduler; không xóa report lỗi.
- Jira lỗi nhưng report đã ghi: sửa quyền/token rồi dùng `-Force` để gửi lại có kiểm soát.
- Production fail: làm theo [`support-escalation-runbook.md`](support-escalation-runbook.md), không tự deploy/restore từ monitor.
- Rotate token Vũ theo [`secret-rotation-plan.md`](secret-rotation-plan.md); không sửa token vào script/task action.
