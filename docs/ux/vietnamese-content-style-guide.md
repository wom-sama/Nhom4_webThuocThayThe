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

## Canonical product glossary

The Vietnamese column is the UI contract. English names remain valid in source code, database
identifiers, API payloads and audit fields when changing them would break an existing contract.

### Navigation and roles

| Concept | Vietnamese UI | Do not use |
| --- | --- | --- |
| Overview | `Tổng quan` | `Tong quan`, `Dashboard` on general navigation |
| Medicine search | `Tra cứu thuốc` | `Tra cuu`, `Tìm thuốc` when the action also checks stock |
| Drug catalogue | `Danh mục thuốc` | `Danh muc`, `Catalog` |
| Inventory | `Kho thuốc` | `Kho`, `Inventory` |
| Expert review | `Đánh giá chuyên môn` | `Review`, `Đánh giá AI` when a person makes the decision |
| Reports | `Báo cáo` | `Report` |
| External sources | `Nguồn dữ liệu` | `Nguon ngoai`, `External data` |
| Administrator | `Quản trị viên` | `Admin` |
| Pharmacist | `Dược sĩ` | `Duoc si` |
| Medical expert | `Chuyên gia y tế` | `Chuyen gia`, `Reviewer` |
| Standard user | `Người dùng tiêu chuẩn` | `User`, `Nguoi dung` |

### Medicine, inventory and safety

| Concept | Vietnamese UI | Notes |
| --- | --- | --- |
| Active ingredient | `Hoạt chất` | Brand and INN names are not translated. |
| Strength | `Hàm lượng` | Preserve the source unit, for example `500 mg`. |
| Dosage form | `Dạng bào chế` | Do not shorten to `Dạng` in comparison tables. |
| Prescription required | `Thuốc kê đơn` | Status badge must include text, not color alone. |
| Non-prescription | `Thuốc không kê đơn` | Avoid the ambiguous `Không kê đơn` by itself. |
| Available | `Còn hàng` | Include quantity when known. |
| Out of stock | `Hết hàng` | Use danger semantics. |
| Low stock | `Sắp hết hàng` | Use caution semantics. |
| Batch | `Lô thuốc` | Keep the immutable batch identifier unchanged. |
| Expiry date | `Hạn sử dụng` | Format `dd/MM/yyyy`. |
| Expired | `Đã hết hạn` | Never use only a red background to convey this state. |
| Near expiry | `Sắp hết hạn` | Always show the actual expiry date. |
| Contraindication | `Chống chỉ định` | Do not abbreviate to `CCĐ` in end-user copy. |
| Interaction | `Tương tác thuốc` | State the severity in adjacent text. |
| Allergy | `Nguy cơ dị ứng` | Avoid diagnostic wording. |

### Recommendation and review

| Concept | Vietnamese UI | Notes |
| --- | --- | --- |
| Alternative candidate | `Thuốc thay thế được đề xuất` | A candidate is not an automatic substitution. |
| Deterministic score | `Điểm phù hợp` | Do not label it as probability or safety guarantee. |
| Score reasons | `Lý do xếp hạng` | Derived from the rule engine. |
| AI explanation | `Giải thích từ AI` | Optional explanatory content only. |
| Pending review | `Chờ đánh giá` | Persisted legacy values may remain unchanged. |
| Approved | `Chấp nhận` | The reviewer accepts the recorded recommendation. |
| Needs review | `Cần xem xét thêm` | Requires a review note. |
| Rejected | `Từ chối` | Requires a reason. |
| Review note | `Nhận xét chuyên môn` | Never call this an AI decision. |
| Audit trail | `Lịch sử thao tác` | `Nhật ký kiểm toán` is reserved for technical reports. |

### Common actions and system states

| Context | Vietnamese UI |
| --- | --- |
| Primary search action | `Tra cứu` |
| Open details | `Xem chi tiết` |
| Create medicine | `Thêm thuốc` |
| Save new record | `Lưu thuốc` / `Lưu lô thuốc` |
| Save edit | `Lưu thay đổi` |
| Cancel | `Hủy` |
| Return | `Quay lại` |
| Sign in / sign out | `Đăng nhập` / `Đăng xuất` |
| Loading | `Đang tải dữ liệu…` |
| Saving | `Đang lưu…` |
| Empty search | `Không tìm thấy thuốc phù hợp với tiêu chí đã nhập.` |
| Offline | `Không có kết nối mạng. Hãy kiểm tra kết nối rồi thử lại.` |
| Service error | `Chưa thể tải dữ liệu. Hãy thử lại sau.` |
| Access denied | `Bạn không có quyền truy cập chức năng này.` |
| Session expired | `Phiên đăng nhập đã hết hạn. Hãy đăng nhập lại.` |
| Rate limited | `Bạn đã gửi quá nhiều yêu cầu. Hãy thử lại sau ít phút.` |

## Validation message contract

| Validation | Vietnamese message |
| --- | --- |
| Required | `{0} là thông tin bắt buộc.` |
| Email | `Địa chỉ email không đúng định dạng.` |
| String length | `{0} không được vượt quá {1} ký tự.` |
| Numeric range | `{0} phải nằm trong khoảng từ {1} đến {2}.` |
| Negative quantity | `Số lượng không được nhỏ hơn 0.` |
| Invalid price | `Giá bán phải lớn hơn hoặc bằng 0.` |
| Expired batch | `Hạn sử dụng phải sau ngày nhập.` |
| Invalid credentials | `Email hoặc mật khẩu không đúng.` |

Validation text identifies the field and correction. It must not reveal whether an account exists or
echo sensitive input.

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

## Documentation gate

- [x] Canonical glossary covers roles, navigation, medicine, inventory, safety and review.
- [x] Empty, loading, error, permission, session and rate-limit microcopy is defined.
- [x] Validation messages are defined without leaking account or clinical information.
- [x] AI wording preserves the deterministic scoring and professional-review boundary.
- [x] Translation boundaries for brands, ingredients, identifiers and source data are explicit.

The implementation checklist remains open until the Razor build is tested against the approved Figma
frames. Documentation completion does not authorize `N4WTT-210`.
