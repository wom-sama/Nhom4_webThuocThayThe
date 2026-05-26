# Sprint S1,6 - Security Execution Report

## Execution

| Field | Value |
| --- | --- |
| Date | 2026-05-26 |
| PR | #9 security runner and password hashing |
| Reviewer | `wom-sama` reviewed work by `ttan73132` |
| Command | `dotnet run --project .\tests\Nhom4WebThuocThayThe.SecurityTests\Nhom4WebThuocThayThe.SecurityTests.csproj --no-build` |
| Result | 10/10 security checks passed |

## Regression

| Runner | Result |
| --- | --- |
| Acceptance | 20/20 passed |
| Security | 10/10 passed |

## Findings

- Seed account storage now uses PBKDF2 hash and salt instead of a plaintext password field.
- Cookie authentication exposes HttpOnly and SameSite=Lax on successful login.
- Protected POST requests without anti-forgery tokens are rejected.
- External `ReturnUrl` values are ignored after login.
- Search payloads are encoded and missing resources return clean 404 responses.

## Residual Risks

- This is still an in-memory account service; production work should move to ASP.NET Core Identity or equivalent.
- Account lockout and password rotation are not implemented yet.
- Audit logging for admin mutations is identified in the source material but should be implemented when persistence is introduced.
- HTTPS Secure cookie behavior should be verified in a deployed HTTPS environment.
