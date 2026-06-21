# S5 Test, Security, AI and Deployment Assessment

Date: 2026-06-21

## Evidence baseline

| Layer | Automated evidence | Current result | Scope limit |
| --- | --- | --- | --- |
| Unit | xUnit `Nhom4WebThuocThayThe.UnitTests` | 15/15 passed | Authentication validation and recommendation scoring rules |
| Integration | xUnit `Nhom4WebThuocThayThe.IntegrationTests` | 3/3 passed | EF migration, seed, SQL persistence and search on SQL Server LocalDB |
| System | Chrome/Playwright against the running build | 6/6 route/viewport combinations passed | Home, search and login on desktop/mobile; no console/network error or viewport overflow |
| Acceptance | Process-level HTTP runner | 31/31 passed | Functional, RBAC, persistence, restart and browser-like static asset checks |
| Security | Process-level HTTP/source runner | 12/12 passed | Password hashing, RBAC, CSRF, redirect, XSS encoding, error leakage, rate limit and headers |
| Performance | Process-level load runner | 9/9 passed locally and on CI | CI: 100 virtual users, 500 requests, p95 794 ms, max 1,124 ms, 297.5 rps and 0.00% error |

The local 100-user result is not a capacity guarantee for shared hosting. It proves only that the current build and LocalDB setup handled the defined workload on the test workstation.

## Security and authorization

Implemented controls:

- Cookie authentication with roles `Admin`, `Pharmacist`, `Expert` and `User`.
- Authorization policies restrict catalog, inventory, reports, external source management and expert review.
- Anti-forgery validation on state-changing MVC actions.
- PBKDF2-SHA256 password hashes and fixed-time hash comparison.
- Login throttling per IP and user-agent partition.
- HttpOnly/SameSite cookies, HSTS outside Development and HTTPS redirection.
- CSP, frame denial, MIME sniffing prevention and referrer restriction.
- Audit records for catalog, inventory, external source and expert review changes.

Residual risks that block a production/medical safety claim:

- Accounts and password hashes are demo data held in memory. There is no ASP.NET Core Identity lifecycle, per-account lockout, password reset, email verification, MFA or administrative user management.
- Demo credentials are known and must never protect real data.
- Data Protection keys are not configured for shared persistent storage, so multi-instance/restart cookie behavior is not production ready.
- CSP still permits inline script/style for compatibility with the current Razor template.
- `AllowedHosts` is broad and no deployment-specific proxy/network allowlist has been configured.
- There is no independent SAST, DAST, dependency vulnerability scan or penetration test.
- The drug rules and seed data have not been clinically validated. No real patient or prescription data may be used.

Decision: acceptable for a classroom demonstration using synthetic data after the S5 controls; not approved for public medical production or real personal health data.

## AI status and governance

No AI model is integrated. There is no LLM, ML.NET, ONNX model, inference endpoint, API key or multi-agent flow in the source.

The current recommendation engine is deterministic. It scores candidates from:

- same active ingredient or therapeutic category;
- strength and dosage form compatibility;
- available stock and price;
- prescription requirement;
- active ingredient warning, contraindication and patient allergy profile.

The engine reads SQL Server data and returns a score, reasons and alerts. Public users can view suggestions. A signed-in profile can trigger allergy warnings. `Admin`, `Expert` and `Pharmacist` roles can review results. No recommender component can edit users, roles, catalog or inventory.

Current data is seed/demo data: 7 drugs, 4 active ingredients and registry records naming DrugBank, PubChem and the ATC Index. The `MarkSynced` action changes metadata only; it does not import or verify data from those sources.

If AI is added later, the safe design is: validated data ingestion -> deterministic candidate filter -> optional ranking model -> deterministic contraindication/allergy guardrail -> pharmacist/expert approval -> audited explanation. An LLM may explain evidence but must not prescribe, bypass safety rules, change protected data or approve its own output. Multiple models should communicate through typed, validated contracts and a single orchestrator, not free-form model-to-model chat.

## Docker status

- Docker Desktop was installed in per-user mode from the official Windows installer.
- `compose.yaml` defines the ASP.NET Core app and SQL Server 2022 with health-gated startup and persistent DB volume.
- The web image runs as the .NET image non-root `APP_UID`.
- GitHub Actions run `27892809788` built `n4wtt-web`, started SQL Server 2022 and the web container, received `database=connected` from `/health`, restarted the web container and passed health again.
- This workstation still requires Windows Subsystem for Linux and Virtual Machine Platform activation plus a restart before the Linux engine can build the stack.

Reference: <https://docs.docker.com/desktop/setup/install/windows-install/>

## Somee Free assessment

The current Somee Free page lists 150 MB web storage, 5 GB monthly transfer, one 30 MB SQL database, forced advertising, manual free SSL renewal and deletion after 30 inactive days. Its package table mentions ASP.NET Core 10 while the FAQ text mentions up to .NET 9, so runtime availability must be verified in the actual control panel before upload.

Reference: <https://somee.com/FreeAspNetHosting.aspx>

This is suitable for a temporary course demo, not a 100-concurrent-user production target. The plan publishes no CPU, memory, connection-pool, request queue or availability guarantee. A 100-user local test cannot validate those shared-host limits. Remote deployment also requires a Somee account, FTP/publish credentials and a provisioned SQL database, none of which are stored in this repository.

## Release gate

The application can proceed as a demo release after regression remains green. Production release remains `NO-GO` until persistent Identity, secret rotation, real data governance, independent security testing, target-host load testing and clinical review are complete.
