# Sprint S1,7 - Performance Test Case Matrix

| ID | Jira | Area | Scenario | Expected Result |
| --- | --- | --- | --- | --- |
| PERF01 | N4WTT-66 | Latency | Repeated dashboard requests | p95 <= 450 ms |
| PERF02 | N4WTT-66 | Latency | Repeated drug search requests | p95 <= 550 ms |
| PERF03 | N4WTT-66 | Latency | Repeated drug detail requests | p95 <= 500 ms |
| PERF04 | N4WTT-68 | Cache/static | Repeated CSS/JS requests | p95 <= 250 ms and ETag or Last-Modified present |
| PERF05 | N4WTT-70 | Auth performance | Login plus inventory access loop | p95 <= 900 ms |
| PERF06 | N4WTT-67 | Burst concurrency | 60 concurrent search/detail requests | p95 <= 1500 ms and >= 20 rps |
| PERF07 | N4WTT-66 | Realtime sampling | Four-second mixed public route sampling | p95 <= 800 ms and >= 12 rps |
| PERF08 | N4WTT-69 | Memory sustain | 160 repeated mixed requests | p95 <= 600 ms, >= 15 rps, working set < 420 MB |

## Review Coverage

- `N4WTT-65`: plan and thresholds.
- `N4WTT-66`: route latency and realtime sampling.
- `N4WTT-67`: concurrency burst.
- `N4WTT-68`: static cache.
- `N4WTT-69`: memory sustain.
- `N4WTT-70`: authenticated flow timing.
- `N4WTT-71`: execution report.
- `N4WTT-72`: final regression review.
