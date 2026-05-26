# Sprint S1,6 - Security Remediation Checklist

## Completed In This Sprint

- Replace plaintext seed account password storage with PBKDF2 hash and salt.
- Add automated security runner with JSON report output.
- Verify invalid login does not issue an auth cookie.
- Verify role access matrix for inventory access.
- Verify CSRF rejection on protected mutation.
- Verify open redirect protection.
- Verify HTML encoding for search payloads.
- Verify error pages do not leak internals.

## Deferred To Later Sprints

- Replace in-memory accounts with database-backed Identity.
- Add account lockout after repeated failed attempts.
- Add password reset/change flow.
- Add persisted audit logs for catalog and inventory mutations.
- Add HTTPS deployment security header review.
- Add dependency vulnerability scanning in CI.

## Review Rule

Every future authentication, authorization or mutation PR must run:

```powershell
dotnet run --project .\tests\Nhom4WebThuocThayThe.SecurityTests\Nhom4WebThuocThayThe.SecurityTests.csproj --no-build
dotnet run --project .\tests\Nhom4WebThuocThayThe.AcceptanceTests\Nhom4WebThuocThayThe.AcceptanceTests.csproj --no-build
```
