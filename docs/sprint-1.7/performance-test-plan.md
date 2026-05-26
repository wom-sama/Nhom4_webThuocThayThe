# Sprint S1,7 - Realtime Performance Test Plan

## Objective

Measure local realtime performance characteristics for the current MVC app before adding database-backed recommendation logic. The runner is intended for regression detection during coursework, not production capacity certification.

## Scope

- Public route latency for dashboard, search and drug detail.
- Authenticated inventory flow with PBKDF2 login.
- Burst concurrency on search/detail routes.
- Sustained realtime sampling window.
- Static CSS/JS cache metadata and response time.
- Memory footprint after repeated requests.

## Thresholds

| Area | Threshold |
| --- | --- |
| Public route p95 | 450-550 ms depending on route |
| Authenticated inventory p95 | 900 ms |
| Concurrency burst p95 | 1500 ms and at least 20 requests/second |
| Sustained sampling p95 | 800 ms and at least 12 requests/second |
| Static asset p95 | 250 ms plus cache revalidation metadata |
| Memory sustain | Working set below 420 MB |

## Entry Criteria

- Build passes.
- Acceptance and security runners pass.
- `S1,7 Perf realtime` issues are linked in PR body.

## Exit Criteria

- Performance runner passes all checks.
- JSON report is generated at `TestResults/performance-report.json`.
- Thresholds and limitations are documented.
