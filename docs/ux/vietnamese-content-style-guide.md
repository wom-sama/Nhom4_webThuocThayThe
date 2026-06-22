# Vietnamese content and localization guide

Jira story: `N4WTT-209`

## Locale contract

- Default UI locale: `vi-VN`.
- Root document language: `<html lang="vi">`.
- File encoding: UTF-8.
- Date: `dd/MM/yyyy`; date/time uses the 24-hour clock.
- Currency: choose one reviewed form (`2.500 ₫` preferred) and use it everywhere.
- Numbers use the `vi-VN` culture, including grouping and decimal separators.
- Medicine brand names, active ingredients, identifiers and external-source names are not translated.

## Voice

- Direct, calm and operational.
- Prefer verbs that describe the next action: `Tra cứu`, `Lưu thay đổi`, `Xem tồn kho`.
- Avoid vague labels such as `Mở`, `Xử lý` or `OK` when a specific action is available.
- Explain clinical limits without alarming or promising treatment outcomes.
- Do not describe AI as deciding, prescribing, approving or guaranteeing safety.

## Preferred terminology

| Current text | Preferred text |
| --- | --- |
| `Tong quan` | `Tổng quan` |
| `Tra cuu` | `Tra cứu` |
| `Thuoc thay the de xuat` | `Thuốc thay thế được đề xuất` |
| `Danh gia de xuat` | `Đánh giá đề xuất` |
| `score` | `Điểm phù hợp` |
| `rule-based` | `theo bộ quy tắc` |
| `Giai thich co ho tro AI` | `Xem giải thích từ AI` |
| `AI generated` | `Nội dung do AI hỗ trợ` |
| `backup metadata` | `Thông tin bản sao lưu` |
| `admin` | `Quản trị viên` |
| `Khong ke don` | `Không kê đơn` |
| `Het hang` | `Hết hàng` |
| `Can nhap them` | `Cần nhập thêm` |
| `Quay lai` | `Quay lại` |
| `Luu` | `Lưu` or a specific save action |
| `Huy` | `Hủy` |

## Safety and AI microcopy

Preferred disclosure:

> AI chỉ giải thích kết quả do hệ thống xếp hạng. Nội dung này không thay thế tư vấn của dược sĩ
> hoặc bác sĩ và không làm thay đổi điểm phù hợp, tồn kho hay cảnh báo an toàn.

Preferred fallback:

> Hiện chưa thể kết nối dịch vụ AI. Hệ thống đang hiển thị giải thích theo bộ quy tắc.

Preferred missing-data warning:

> Chưa đủ thông tin để kết luận cho từng người dùng. Hãy để dược sĩ hoặc bác sĩ xác nhận trước khi
> thay thế thuốc.

## Capitalization and punctuation

- Use sentence case for headings, buttons and table headers.
- Do not capitalize every word.
- Use Vietnamese punctuation and complete sentences for descriptions/errors.
- Do not use `<` as a back icon; use an approved icon with the label `Quay lại`.
- Avoid English abbreviations unless they are established domain names; explain them at first use.

## Localization architecture

Implementation planning must cover:

- `RequestLocalization` with `vi-VN` as default culture and UI culture.
- Shared and feature `.resx` resources for Razor/navigation/actions.
- `DataAnnotations` resources for validation messages and field labels.
- Domain-message resources for recommendation, safety, audit and reporting services.
- AI prompt/fallback resources separated from visible UI resources.
- Seed/reference data review: distinguish translatable descriptions from immutable drug data.
- Tests that fail when a new user-facing hard-coded string is introduced outside approved resources.

## Review checklist

- [ ] All visible copy has Vietnamese diacritics.
- [ ] `lang`, accessible names, placeholders and tooltips are localized.
- [ ] Date, number and currency follow `vi-VN`.
- [ ] Validation, empty, loading, error and permission states are covered.
- [ ] Terminology is consistent across UI, Jira evidence and test assertions.
- [ ] AI wording preserves deterministic and clinical boundaries.
- [ ] Long Vietnamese text fits at 390 px and 200% zoom.
