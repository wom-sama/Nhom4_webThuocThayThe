# S11 initial production execution

Execution date: 22/06/2026

Target: <https://nnhom4web.somee.com>

## Result

- Initial run: `11/13` passed.
- Production RBAC matrix: `15/16` combinations behaved correctly.
- Paced performance: `30/30` successful reads, p50 `232.20 ms`, p95 `334.23 ms`, max `374.45 ms`.
- Security headers, HSTS, cache metadata, CSRF, XSS encoding, safe error handling and AI key isolation passed.

## Findings

`PROD07` was a test-contract defect: the application returned a generic invalid-login validation summary
and did not issue an authentication cookie. The runner was changed to assert the stable validation
contract rather than a particular localized sentence.

`PROD09` found product bug `N4WTT-250`: an Admin session authenticated correctly, but `/Admin` and
`/Admin/Reports` returned HTTP 500. Other Admin pages remained available and no cross-role access was
granted.

Direct read-only diagnostics reproduced the exception: `ReportingService` streamed `Drugs` while
issuing nested inventory queries on the same SQL connection. Somee does not enable Multiple Active
Result Sets, so the second query failed while the first data reader was open.

## Remediation evidence

- Materialize the drug list before nested inventory and recommendation queries.
- Run SQL Server integration tests with `MultipleActiveResultSets=false`.
- Added `ReportingDashboard_WorksWithoutMultipleActiveResultSets` regression coverage.
- Direct read-only diagnostic against production data passes after the code fix: four metrics and seven
  stock-risk rows were built successfully.

The production site remains on the previous runtime until CI, cross-review and deployment finish. The
final `13/13` result is recorded separately after release.

## Security review addendum

PR review found `N4WTT-251`: the old production runtime still accepted the documented development
passwords. Release was blocked and the deployment contract was changed before rollout:

- demo accounts now load only in `Development` or `Testing`;
- `Production` fails closed when `Authentication__EncodedAccounts` is missing or invalid;
- the deployment script reads rotated credentials from an untracked, ACL-restricted operator file;
- only 16-byte salts and 32-byte PBKDF2-SHA256 hashes enter staging runtime configuration;
- staging scan found zero production plaintext and zero demo-password occurrences;
- production-mode local smoke accepted the rotated Admin credential and opened `/Admin`;
- PSScriptAnalyzer returned no warning/error for the deployment script.

Post-remediation pre-release suites: Unit `30/30`, Integration `4/4`, Acceptance `40/40`, Security
`17/17`, Performance `10/10`.
