# Sprint S1,5 - Debug And Maintenance Checklist

## Debug Readiness

- Unknown routes and missing drug details return 404 without stack trace text.
- Login validation errors are shown on the form instead of throwing.
- Anti-forgery failures return 400 and are reproducible from the test runner.
- Acceptance runner records a JSON report after every execution.

## Maintenance Checks

- Test project is added to `CMPM.sln`.
- Web project excludes `tests/**/*.cs` from its compile glob.
- Test runner does not require external NuGet packages.
- Test cases are grouped by clear non-functional direction.
- README includes restore, build and acceptance runner commands.

## PR Checklist

- Branch name starts with a Jira key.
- Commit message includes Jira keys.
- PR body lists Jira keys and verification commands.
- Reviewer reruns build and acceptance tests before approval.
- Jira issue is moved to Done only after merge.

## Known Limits

- Data is in memory, so the runner treats each app start as a clean environment.
- Performance thresholds are local smoke checks.
- Memory threshold is a guardrail, not a profiling substitute.
- Maintainability checks are currently documented and build-based; static analyzers can be added later.
