# Sprint S1,5 - Test Execution Report

## Execution

| Field | Value |
| --- | --- |
| Date | 2026-05-26 |
| Branch reviewed | PR #7 acceptance test runner |
| Runner | `Nhom4WebThuocThayThe.AcceptanceTests` |
| Command | `dotnet run --project .\tests\Nhom4WebThuocThayThe.AcceptanceTests\Nhom4WebThuocThayThe.AcceptanceTests.csproj --no-build` |
| Result | 20/20 automated checks passed |

## Result Summary

| Area | Passed | Failed |
| --- | ---: | ---: |
| Smoke and search | 4 | 0 |
| Drug detail and inventory | 2 | 0 |
| Auth, session and RBAC | 5 | 0 |
| Catalog validation and security | 2 | 0 |
| Cache, responsive and accessibility | 3 | 0 |
| Performance, memory, debug and concurrency | 4 | 0 |

## Observed Runtime Notes

- Local search smoke average was well below the 650 ms threshold.
- Working set stayed below the 350 MB smoke budget during repeated requests.
- Static stylesheet exposed cache revalidation metadata.
- Anti-forgery validation rejected direct inventory mutation without a token.

## Evidence

The runner writes machine-readable results to `TestResults/acceptance-report.json`. The directory is ignored by Git because it is generated output.

## Follow-up

- Add browser-level screenshots after the UI stabilizes further.
- Add database integration tests after EF Core or SQL Server is introduced.
- Add recommendation quality tests after the substitution scoring rules are implemented.
