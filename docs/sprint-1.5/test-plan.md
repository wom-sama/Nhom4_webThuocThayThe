# Sprint S1,5 - Test Plan

## Objective

Validate the current MVC framework after S0, S1 and S0,5 before moving into database-backed recommendation work. The plan covers business flows, access control, UI behavior, static assets, performance, memory, debug behavior and maintainability.

## Scope

- Public dashboard and drug search.
- Drug detail page and same-active-ingredient alternatives.
- Login, logout and role-based access control.
- Catalog administration form validation.
- Inventory stock and batch dashboard.
- Static stylesheet, responsive CSS and accessibility markers.
- Local performance, cache metadata, memory footprint and parallel read stability.

## Out Of Scope

- Real database transaction tests.
- Browser visual regression screenshots.
- Production load testing.
- External medical decision validation.

## Environment

| Item | Value |
| --- | --- |
| OS | Windows local development machine |
| Runtime | .NET 10 |
| App | ASP.NET Core MVC |
| Data | In-memory seed data |
| Test runner | `tests/Nhom4WebThuocThayThe.AcceptanceTests` |
| Report path | `TestResults/acceptance-report.json` |

## Roles And Accounts

| Role | Account | Covered By |
| --- | --- | --- |
| Admin | `admin@nhom4.local / Admin@123` | Catalog, security, logout |
| Pharmacist | `duocsi@nhom4.local / Duocsi@123` | Inventory access |
| Expert | `chuyengia@nhom4.local / Chuyengia@123` | Reserved for later review workflow |
| User | `user@nhom4.local / User@123` | RBAC access denied |

## Entry Criteria

- `main` builds without warnings or errors.
- S0, S1 and S0,5 issues are Done.
- Acceptance runner exists in the solution.
- Jira tickets are linked in PR title/body.

## Exit Criteria

- At least 16 test directions are represented.
- All automated acceptance cases pass locally.
- Test matrix documents functional and non-functional coverage.
- Sprint review can trace each test direction back to a Jira ticket.

## Risk Notes

- In-memory data resets each app start, so inventory mutation tests must not assume persistence between runs.
- Local performance thresholds are smoke thresholds, not production SLOs.
- Static cache behavior is checked only for basic revalidation metadata.
- Browser rendering and screenshot comparison should be added after the UI is more stable.
