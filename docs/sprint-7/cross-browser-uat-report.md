# Báo cáo UAT cross-browser production S7

- Người thực hiện: Tú (Frontend/UI), Vũ (QC ghi defect)
- Thời điểm: 22/06/2026, UTC+7
- Môi trường: `https://nnhom4web.somee.com`
- Viewport: desktop `1440x900`, mobile `390x844`
- Dữ liệu thô: [`assets/cross-browser-uat.json`](assets/cross-browser-uat.json)

## Phạm vi

Runner Playwright cô lập kiểm tra route lõi, horizontal overflow, menu mobile, console error, keyboard focus, accessible name của ô tìm kiếm và role matrix trên ba engine. Ảnh được kiểm tra trực quan để phát hiện khác biệt font, kích thước, tràn nội dung và thành phần chồng lấp.

| Trình duyệt | Phiên bản | Desktop | Mobile | Console error | Kết quả |
|---|---:|---:|---:|---:|---|
| Google Chrome | 149.0.7827.115 | Pass | Pass | 0 | Pass |
| Microsoft Edge | 149.0.4022.80 | Pass | Pass | 0 | Pass |
| Mozilla Firefox | 151.0 | Pass | Pass | 0 | Pass |

Mỗi tổ hợp đều trả `200` cho trang chủ, `/Drugs?keyword=para` và `/Drugs/Details/1`. Không có horizontal overflow ở ba route. Menu mobile chuyển `aria-expanded=true` và vùng điều hướng có class `show` sau thao tác.

## Keyboard và accessibility

- Tab đầu tiên là skip-link `#main-content` trên cả sáu tổ hợp.
- Ô tìm kiếm có accessible name tường minh.
- Nội dung chính và nút menu mobile xuất hiện trong DOM đúng viewport.
- Không thấy chữ bị cắt, control chồng lấp hoặc layout dịch chuyển khác nhau giữa ba engine.
- Finding P3: `<html lang="en">` chưa phản ánh nội dung tiếng Việt. Vũ đã tạo Bug `N4WTT-226` trong S8; đây là release gate của bản địa hóa, không bị coi là đã sửa trong S7.

## Role-based UAT

| Role | Route | Hành vi mong đợi | Chrome | Edge | Firefox |
|---|---|---|---:|---:|---:|
| Anonymous | `/DrugCatalog` | Chuyển tới `/Auth/Login` | Pass | Pass | Pass |
| User | `/DrugCatalog` | Chuyển tới `/Auth/AccessDenied` | Pass | Pass | Pass |
| Admin | `/DrugCatalog` | Truy cập thành công | Pass | Pass | Pass |

Role matrix đạt `9/9`. Kết quả khớp Acceptance TC07/TC10/TC24 và Security SEC05; UAT không thay thế các test tự động này mà xác nhận hành vi trong browser thật trên production.

## Bằng chứng hình ảnh

| Engine | Desktop | Mobile |
|---|---|---|
| Chrome | [`chrome-desktop-home.png`](assets/chrome-desktop-home.png) | [`chrome-mobile-home.png`](assets/chrome-mobile-home.png) |
| Edge | [`edge-desktop-home.png`](assets/edge-desktop-home.png) | [`edge-mobile-home.png`](assets/edge-mobile-home.png) |
| Firefox | [`firefox-desktop-home.png`](assets/firefox-desktop-home.png) | [`firefox-mobile-home.png`](assets/firefox-mobile-home.png) |

## Kết luận

Cross-browser UAT S7 đạt cho hành vi hiện tại: `6/6` tổ hợp layout/route và `9/9` role checks pass, không có P0/P1/P2 mới. `N4WTT-226` P3 được chuyển sang S8 vì thay đổi thuộc thiết kế/bản địa hóa và phải theo Figma design gate trước khi triển khai.
