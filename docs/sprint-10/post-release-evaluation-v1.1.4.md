# Post-release evaluation v1.1.4

Jira sprint: `S10 Hau kiem production v1.1.4`

Issues: `N4WTT-237`, `N4WTT-238`, `N4WTT-239`, `N4WTT-240`

Evaluation date: 22/06/2026

## Release under evaluation

- Repository: `wom-sama/Nhom4_webThuocThayThe`
- Production tag: `v1.1.4`
- Production merge commit: `2c39d10`
- Production URL: <https://nnhom4web.somee.com>
- S8 implementation: PR #37; release PR #38; production AI hotfix line PR #39 to PR #46
- S8 evidence update: PR #47 and PR #48

## Product and production smoke

| Check | Result | Evidence |
| --- | --- | --- |
| HTTP `/health` | Pass, HTTP 200 | `http://nnhom4web.somee.com/health` |
| HTTPS `/health` | Pass, HTTP 200 | `https://nnhom4web.somee.com/health` |
| Database connectivity | Pass | Health response reports `database=connected` |
| Public home | Pass, HTTP 200 | `/` |
| Public medicine search | Pass, HTTP 200 | `/Drugs` |
| Login page | Pass, HTTP 200 | `/Auth/Login` |
| Pharmacist login and Area redirect | Pass | Login redirects to `/Pharmacist` |
| Pharmacist medicine detail | Pass | `/Pharmacist/Workspace/Details/1` |
| Gemini live provider on Somee | Not accepted | Safe deterministic fallback; defect `N4WTT-236` |

The release remains available while provider failure is isolated. The deterministic recommendation,
stock, safety warnings and professional review workflow do not depend on Gemini.

## Automated test evidence

| Level | Result | Scope |
| --- | --- | --- |
| Unit | 26/26 pass | account, scoring, controller AI boundary, Gemini request and response parsing |
| Integration | 3/3 pass | SQL Server persistence and restart behavior |
| Acceptance/system | 40/40 pass | public flows, RBAC, UI localization, role Areas, static assets and fallback |
| Security | 17/17 pass | PBKDF2, cookie, CSRF, RBAC, XSS, open redirect, headers, AI privacy and rate limits |
| Performance | 10/10 pass | latency, concurrency, memory, 100 users and paced Somee Free profile |

Relevant generated reports are under `TestResults/` during validation. The latest paced Somee profile
recorded p95 20.2 ms locally, 37.7 requests/second, zero errors and an estimated 1.95 million responses
per 5 GB transfer under the measured payload profile. This is a laboratory estimate, not a hosting SLA.

## Figma, Vietnamese UI and role separation

- Figma source of truth: `N4WTT - S8 UI Design`, file key `fu43wNGxTFRMghHrlH9lhI`.
- Nam approved Manual V1 in `N4WTT-212` before implementation.
- Repository handoff: `docs/ux/figma-s8/`.
- Visual QA passed for public, login, User, Pharmacist, Expert and Admin shells.
- Desktop and mobile checks found no horizontal overflow and no visible demo credentials.
- Default culture is `vi-VN`; HTML language is `vi`; responses emit `Content-Language: vi-VN`.
- Authenticated roles use separate ASP.NET Core Areas and layouts rather than one shared dashboard.
- Sample credentials exist only in internal test documentation.

## Security and operational assessment

Accepted for the classroom/demo production scope:

- server-side role authorization and Area isolation;
- PBKDF2 password hashes for seeded test identities;
- HttpOnly and SameSite authentication cookie;
- anti-forgery validation on protected writes;
- CSP and browser hardening headers;
- login and AI rate limiting;
- no Gemini key in browser-delivered content;
- deployment secrets remain outside Git and Jira;
- HTTPS health gate, FTP backup and deployment manifest.

Known constraints:

- seeded identities are not a production identity platform;
- Somee Free is not treated as a high-availability or regulated medical host;
- live Gemini calls from Somee currently return non-success and use the safe fallback;
- the application does not yet expose a versioned public REST API.

## API and MCP decision

Public integration work is intentionally deferred to `S9 API & MCP Integration`:

- `N4WTT-230`: governance epic;
- `N4WTT-231`: REST API v1 contract;
- `N4WTT-232`: OAuth/JWT or scoped API-key security, CORS allowlist, rate limit and audit;
- `N4WTT-233`: OpenAPI and contract tests;
- `N4WTT-234`: MCP architecture decision record.

MCP is not part of the public web contract. If adopted, it must remain an internal, allowlisted,
audited tool bridge with least privilege and no direct access to production secrets or unrestricted
database writes.

## Retrospective

### Keep

- Figma approval gate before implementation.
- Role-specific Areas and server-side authorization.
- Cross-profile PR review and CI before merge.
- Automated acceptance, security and performance runners.
- Safe AI fallback and secret isolation.

### Problems found

- Jira S8 remained in `future` after implementation and had to be reconciled from evidence.
- Figma brief still said approval was pending after the owner approved Manual V1.
- Gemini provider contracts varied by model and auth transport.
- Somee outbound Gemini behavior is less reliable than local/container execution.

### Actions

- Close S8 only after every issue has release evidence and Done status.
- Keep `N4WTT-236` open until the Somee provider limitation is accepted or resolved.
- Execute API/MCP work only in S9 with an explicit security contract.
- Add a release-version endpoint or build metadata in a later sprint so production commit identity is
  directly observable without relying only on deployment records.

## Release decision

**Accepted with a documented external-provider limitation.**

`v1.1.4` remains the approved Somee classroom/demo release. Core recommendation, safety, inventory,
RBAC, Vietnamese role UI and deterministic fallback are accepted. Live Gemini generation on Somee is
not claimed as passing; it remains tracked in `N4WTT-236`.
