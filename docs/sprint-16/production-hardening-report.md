# S16 Production Hardening Report

Date: 2026-06-23
Production URL: https://nnhom4web.somee.com
Sprint: S16 Prod Hardening (Jira sprint id 56)

## Jira Scope

- N4WTT-260: Expert navigation renders distinct Queue, Evidence and Decision History screens.
- N4WTT-261: Demo seed data expanded across therapeutic groups, stock states and expert review states.
- N4WTT-262: Privacy policy replaced template copy with concrete data, RBAC, AI and incident sections.
- N4WTT-263: Deterministic role decision-support insights added for Admin, Pharmacist, Expert and User dashboards.
- N4WTT-264: Figma-ready boards added before implementation in `docs/ux/figma-s16`.
- N4WTT-265: QA, release and production evidence collected for this increment.

## Implementation Summary

- Split Expert sidebar actions into separate `Index`, `Evidence` and `History` pages.
- Split Pharmacist workspace into stockout queue, search desk and replacement comparison pages.
- Added reusable role insight panel backed by deterministic database signals.
- Changed seed initialization to idempotently add missing reference/demo data without overwriting production records.
- Moved S16 drug and batch seed records to high IDs to avoid collisions with user-created records.
- Made privacy policy publicly accessible and replaced placeholder copy.
- Added acceptance and production regression tests for role routes, seed data, privacy content and Gemini endpoint state.

## Local Verification

- Build Release: passed, 0 warnings.
- Unit tests: 31/31 passed.
- Integration tests: 4/4 passed.
- Acceptance/system tests: 49/49 passed.
- Security tests: 17/17 passed.
- Performance tests: 10/10 passed.

## Browser UI QA

Local browser checks were run against the Release build:

- Expert `/Expert/Reviews`: `Hồ sơ cần quyết định`, 9 review forms, no horizontal overflow.
- Expert `/Expert/Reviews/Evidence`: `Cơ sở xếp hạng đề xuất`, 18 evidence cards, no horizontal overflow.
- Expert `/Expert/Reviews/History`: `Lịch sử quyết định`, 16 history items, no horizontal overflow.
- Pharmacist `/Pharmacist/Workspace`: `Thuốc cần phương án thay thế`, 14 queue cards, no horizontal overflow.
- Pharmacist `/Pharmacist/Workspace/Search?keyword=Loratadine`: 4 result cards, no horizontal overflow.
- Pharmacist `/Pharmacist/Workspace/Compare`: 14 comparison cards, no horizontal overflow.
- Privacy `/Home/Privacy`: 6 policy cards, AI boundary section present, no console errors.

## Production Verification

Production deploy completed via `scripts/Deploy-Somee.ps1`.

- Health: healthy/database connected.
- Production validation: 16/16 passed.
- Covered gates: health, public UI, search/filter, recommendation, security headers, static cache, invalid login, RBAC, CSRF, XSS encoding, AI key non-disclosure, paced production performance, role UI routes, expanded content, Gemini live response.
- Report artifact: `TestResults/production-validation-report.json`.

## Release Notes

- GitHub commits:
  - `756ff88 docs(N4WTT-264): add S16 Figma-ready hardening boards`
  - `b0f92ab feat(N4WTT-260,N4WTT-261,N4WTT-262,N4WTT-263): harden role flows and data`
- Release tag: `v1.4.0` after production validation.
