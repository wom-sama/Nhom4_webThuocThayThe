# Product Completion Release Plan

## Sprint S2 - Goi y an toan

Sprint goal: Hoan thien luong de xuat thuoc thay the co score, ly do, ton kho va canh bao y khoa.

Stories:

- N4WTT-15: Goi y thuoc thay the khi thuoc chinh het hang.
- N4WTT-16: Xem chi tiet thuoc de xuat.
- N4WTT-17: Tinh AI Score cho thuoc thay the.
- N4WTT-18: Canh bao y khoa.
- N4WTT-53: Nghien cuu dataset.
- N4WTT-121: Regression cac chuc nang cu.

Increment:

- Recommendation service va score 0-100.
- Ly do cung hoat chat, ham luong, dang bao che, ton kho va gia.
- Canh bao ke don, hoat chat, di ung va chong chi dinh.
- Acceptance suite 28 case.

## Sprint S3 - Admin va bao cao

Sprint goal: Hoan thien expert review va dashboard van hanh.

Stories:

- N4WTT-19: Chuyen gia danh gia ket qua AI.
- N4WTT-20: Dashboard bao cao.

Increment:

- Expert review workflow.
- Stock risk dashboard.
- Audit log va backup metadata.

## Sprint S4 - Nghiem thu va van hanh

Sprint goal: Hoan thien external data registry, audit/backup, regression va release hardening.

Stories:

- N4WTT-21: Quan ly nguon du lieu ngoai.
- N4WTT-22: Logging va backup.
- N4WTT-121: Full regression/retest.

Release gate:

- Build: 0 warning, 0 error.
- Acceptance: 28/28.
- Security: 10/10.
- Performance: 8/8.
- Khong con bug blocker/critical.
