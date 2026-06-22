# S11 production-safe validation plan

Jira: `N4WTT-245`, `N4WTT-246`, `N4WTT-250`

Target: <https://nnhom4web.somee.com>

## Test-level boundary

- Unit tests run against code and isolated dependencies before release. They do not run on production.
- Integration tests run against an isolated SQL Server database, including a connection with MARS disabled.
- Acceptance, system, security and performance runners execute locally and in CI before release.
- Production validation is read-only except authentication sessions and deliberately invalid POST requests that are rejected by anti-forgery validation.
- Production load is paced to protect the Somee Free quota. It is a baseline, not a stress test.

## Production cases

| ID | Area | Production assertion |
| --- | --- | --- |
| `PROD01` | Health | HTTPS health endpoint and SQL connectivity |
| `PROD02` | Public UI | Vietnamese document contract; no demo credential leakage |
| `PROD03` | Search | Keyword and category filtering |
| `PROD04` | Recommendation | Out-of-stock state and deterministic alternatives |
| `PROD05` | Browser security | CSP, anti-framing, nosniff, referrer policy and HSTS |
| `PROD06` | Cache | Static CSS public cache and seven-day max age |
| `PROD07` | Authentication | Invalid login stays anonymous and receives a generic error |
| `PROD08` | RBAC | Anonymous users cannot open any role Area |
| `PROD09` | RBAC | Four roles are isolated across all 16 role/Area combinations |
| `PROD10` | CSRF | Protected Admin write without anti-forgery token is rejected |
| `PROD11` | XSS/error handling | Reflected input encoded; unknown route hides internals |
| `PROD12` | AI security | Unsafe methods rejected; no API key in browser assets |
| `PROD13` | Performance | Thirty paced reads, zero errors, p95 at most 2.5 seconds |

## Configuration

The runner reads the target URL and role credentials from environment variables. Credentials remain in
`docs/testing/demo-accounts.md` for internal testing only and are never written to reports, Jira, browser
assets or command output.

The optional direct database diagnostic is read-only, does not run migrations or seed data, and is used
only to reproduce a production data-access failure with the production connection characteristics.

## Release gate

Release requires all local/CI suites to pass, a reviewed PR, successful deployment, `13/13` production
checks and a separate browser usability pass at desktop and mobile widths. A failed production case is
recorded as a Jira bug; the threshold is not relaxed to hide a product defect.
