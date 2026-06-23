# S17 Motion Security Audit Report

Date: 2026-06-23
Production URL: https://nnhom4web.somee.com
Sprint: S17 Motion Security Audit (Jira sprint id 57)

## Jira Scope

- N4WTT-266: Figma-first motion polish evidence.
- N4WTT-267: UI animation polish.
- N4WTT-268: Comprehensive production test expansion.
- N4WTT-269: Safe web attack validation on production.
- N4WTT-270: Report readiness audit against the course requirement document.
- N4WTT-271: Release evidence and Jira closure.

Backlog check before the S17 release: `project = N4WTT AND statusCategory != Done` returned 0 open issues.

## Implementation Summary

- Added Figma-ready S17 motion boards in `docs/ux/figma-s17`.
- Added motion tokens, entry animation, hover lift, nav focus polish, live indicator animation and `prefers-reduced-motion` fallback in `wwwroot/css/site.css`.
- Expanded the production validation runner from 16 to 30 cases.
- Added safe production attack checks for reflected XSS, SQL injection probes, traversal/config probes, open redirect, auth cookie flags, missing CSRF/method tampering and login brute-force rate limiting.
- Confirmed Gemini explanation route is live or safely reported without exposing the key to browser-delivered content.

## Local Verification

- Build Release: passed, 0 warnings.
- Unit tests: 31/31 passed.
- Integration tests: 4/4 passed.
- Security tests: 17/17 passed.
- Performance tests: 10/10 passed.
- Acceptance/system tests: 49/49 passed.

## Browser UI QA

Local Release browser checks:

- Home, search and privacy pages had 0 console errors.
- Desktop width had no horizontal overflow.
- Mobile 390px viewport had no horizontal overflow.
- Motion transitions were detected on public cards and result rows.
- `prefers-reduced-motion` is present in CSS for accessibility.

Production browser checks:

- `/`, `/Drugs?keyword=Omeprazole` and `/Home/Privacy` had 0 console errors.
- Desktop and mobile production checks had no horizontal overflow.
- Motion CSS was loaded on production with `surface-enter` and transform/box-shadow transitions.
- Evidence screenshots were written to `C:\Users\ADMIN\Documents\Codex\2026-06-23\b\outputs`.

## Production Verification

Production deploy completed via `scripts/Deploy-Somee.ps1`.

- Health: healthy/database connected.
- Production validation: 30/30 passed.
- Role coverage: Admin, Pharmacist, Expert and User login/workspace flows.
- Public coverage: home, login, search, category filter, drug detail, privacy policy and invalid detail 404.
- Security coverage: headers, cache contract, invalid login, RBAC isolation, CSRF rejection, XSS encoding, SQLi probes, traversal/config probes, open redirect rejection, cookie flags, method tampering and brute-force rate limit.
- AI coverage: Gemini endpoint live/safe status and protected Pharmacist AI endpoint.
- Report artifact: `TestResults/S17-production-validation-report.json`.

## Course Requirement Readiness

Source requirement document reviewed: `D:\New folder\BCCNPM\YEU CAU DO AN MON CONG NGHE PHAN MEM.docx` (original file name uses Vietnamese accents).

Current report draft reviewed:

- Draft file: `D:\New folder\BCCNPM\BaoCao_CongNghePhanMem_WebThuocThayThe_DU_THAO.docx`.
- Extracted table of contents indicates chapters 1 through 6 and about 29 pages, which fits the 25-40 page requirement.
- Existing draft already references Jira, Product Vision, Product Backlog, Sprint Review, Retrospective, Figma, UML, database design, GitHub and test levels.

Remaining report update before final submission:

- Add final production URL, Somee deployment evidence and S17 release evidence.
- Add the 30-case production validation matrix and the safe attack-test summary.
- Add final GitHub/Jira/Figma/demo-video links.
- Add the latest screenshots for production desktop/mobile UI.
- Re-render the DOCX/PDF after inserting evidence and verify page count still stays inside 25-40 pages.

## Release Notes

- GitHub commits:
  - `da31c11 docs(N4WTT-266): add S17 motion design board`
  - `26a57c0 feat(N4WTT-267,N4WTT-268,N4WTT-269): add motion polish and production attack tests`
- Release candidate tag after final evidence commit: `v1.5.0`.
