# S12 UI copy cleanup report

Jira sprint: `S12 UI Copy Cleanup` (`future`, not started because `S11 UI & Prod Validation` is active)

Issues: `N4WTT-252`, `N4WTT-253`

Date: 23/06/2026

## Scope

- Reduce verbose explanatory copy on the public home page.
- Keep search as the primary first-screen task.
- Remove the login benefit bullet list while keeping the security note.
- Preserve the rule that demo credentials are documented only in internal docs, never in UI.
- Keep the functional home category filter from S11.

## Changes

- Public home title shortened to `Tìm thuốc và phương án thay thế`.
- Removed the three verbose workflow cards from the home page.
- Converted home metrics to a compact `metric-strip`.
- Removed login page description and benefit bullets.
- Added mobile footer stacking to avoid cramped footer text.
- Updated acceptance/security tests to assert stable UI contracts instead of fragile copy.

## Verification

- Build Release: passed.
- Unit tests: `30/30`.
- Acceptance tests: `44/44`.
- Security tests: `17/17`.
- Performance tests: `10/10`.
- Integration runner: exit `0`; the current runner does not emit a numbered report file.

## Browser QA

Local Release URL: `http://127.0.0.1:5077`

Checked:

- `/` at `1440x900` and `390x844`.
- `/Auth/Login` at `1440x900` and `390x844`.

Observed:

- No horizontal overflow.
- No disabled fake controls.
- No demo credentials visible.
- Home workflow cards: `0`.
- Login benefit bullets: `0`.
- One paragraph on each page.
- Mobile footer stacks as a column.

Local evidence files:

- `artifacts/s12-ui-cleanup/browser-qa-summary.json`
- `artifacts/s12-ui-cleanup/home-1440x900.png`
- `artifacts/s12-ui-cleanup/login-1440x900.png`
- `artifacts/s12-ui-cleanup/home-390x844.png`
- `artifacts/s12-ui-cleanup/login-390x844.png`
- `artifacts/s12-ui-cleanup/home-390x844-after-footer.png`

These artifacts are local QA evidence and should not be committed unless explicitly needed.
