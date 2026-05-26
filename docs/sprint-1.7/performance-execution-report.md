# Sprint S1,7 - Performance Execution Report

## Execution

| Field | Value |
| --- | --- |
| Date | 2026-05-26 |
| Runner | `Nhom4WebThuocThayThe.PerformanceTests` |
| Command | `dotnet run --project .\tests\Nhom4WebThuocThayThe.PerformanceTests\Nhom4WebThuocThayThe.PerformanceTests.csproj --no-build` |
| Report | `TestResults/performance-report.json` |

## Interpretation

- `p50` is the median observed latency.
- `p95` is the main guardrail for route regressions.
- `rps` is calculated inside the local runner and is only comparable on the same machine.
- Memory is measured as the web process working set after the repeated request loop.

## Limitations

- The current app uses in-memory data and therefore does not represent database performance.
- Local antivirus, IDE background build and other apps can add noise.
- Thresholds are intentionally smoke-level so they catch obvious regressions without pretending to be production SLOs.

## Follow-up

- Re-baseline after EF Core/SQL Server is introduced.
- Add browser performance timing once pages include heavier client-side behavior.
- Add CI artifacts for JSON reports when GitHub Actions is configured.
