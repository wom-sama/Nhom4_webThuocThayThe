# Báo cáo keyboard, responsive, accessibility và RBAC S7

- Người thực hiện: Tú
- Người ghi defect: Vũ
- Môi trường: `https://nnhom4web.somee.com`
- Ngày thực hiện: 22/06/2026, UTC+7
- Dữ liệu axe-core: [`assets/accessibility-uat.json`](assets/accessibility-uat.json)
- Dữ liệu browser/RBAC: [`assets/cross-browser-uat.json`](assets/cross-browser-uat.json)

## Accessibility scan

axe-core được chạy với tag WCAG 2.0 A/AA, WCAG 2.1 A/AA và WCAG 2.2 AA trên:

- Chrome 149, Edge 149 và Firefox 151.
- Desktop `1440x900` và mobile `390x844`.
- Trang chủ, tìm kiếm, chi tiết thuốc và đăng nhập.

Tổng cộng `24/24` scan hoàn tất, `0` violation và `0` node vi phạm. Cả 24 route đều trả `200` và không horizontal overflow.

Kết quả axe không thay thế đánh giá thủ công. Cụ thể, `lang="en"` là mã hợp lệ nên axe không báo lỗi, nhưng không khớp nội dung tiếng Việt; finding này vẫn được quản lý bằng Bug `N4WTT-226`.

## Keyboard focus

| Engine | Desktop | Mobile | Focus đầu tiên | Focus indicator |
|---|---:|---:|---|---|
| Chrome | Pass | Pass | `#main-content` | `outline: auto 1px` |
| Edge | Pass | Pass | `#main-content` | `outline: auto 1px` |
| Firefox | Pass | Pass | `#main-content` | `outline: auto 1px` |

Skip-link hiển thị khi nhận focus và có kích thước khác 0. Tám tab stop đầu tiên đều là control/link tương tác; không phát hiện focus trap trong header và form tìm kiếm.

## Responsive

- Không có horizontal overflow ở trang chủ, tìm kiếm hoặc chi tiết thuốc trên cả desktop/mobile.
- Menu mobile mở sau thao tác và cập nhật `aria-expanded=true`.
- Form tìm kiếm chuyển thành một cột trên mobile, control giữ đủ chiều rộng và chữ không bị cắt.
- Ảnh Chrome/Edge/Firefox đã được review trực quan trong báo cáo [`cross-browser-uat-report.md`](cross-browser-uat-report.md); không phát hiện overlap hoặc khác biệt engine gây chặn thao tác.

## Role matrix

| Role | `/DrugCatalog` | Kết quả trên 3 engine |
|---|---|---:|
| Anonymous | Redirect tới `/Auth/Login` | 3/3 Pass |
| User | Redirect tới `/Auth/AccessDenied` | 3/3 Pass |
| Admin | Mở trang quản trị | 3/3 Pass |

RBAC đạt `9/9`. Kết quả được đối chiếu với automated Acceptance và Security, không dựa duy nhất vào URL hiển thị.

## Kết luận

`N4WTT-200` đạt cho increment S7: accessibility automation `24/24`, keyboard focus `6/6`, responsive route `18/18` và RBAC `9/9`. Không phát hiện P0/P1/P2 mới. Bug P3 `N4WTT-226` là release gate của S8 và chỉ được đóng sau khi triển khai `vi-VN`, cập nhật `html lang` và chạy lại cùng matrix.
