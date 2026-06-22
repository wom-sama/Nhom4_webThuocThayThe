# Somee deployment runbook

Incident severity, ownership and escalation are defined in
[`support-escalation-runbook.md`](support-escalation-runbook.md).
Production SLOs, hosting guardrails and the go/no-go checklist are defined in
[`slo-quota-release-gate.md`](slo-quota-release-gate.md).
Daily production evidence and Jira defect rules are defined in
[`production-monitoring.md`](production-monitoring.md).

This runbook deploys the published ASP.NET Core package without storing credentials in Git, Jira,
shell history or the generated archive.

## Secret inputs

Keep both files outside the repository. `someeDeploy.txt` uses `KEY=VALUE` lines and must contain:

```text
SITE_URL=
FTP_HOST=
FTP_PORT=21
FTP_USERNAME=
FTP_PASSWORD=
FTP_REMOTE_PATH=
SQL_CONNECTION_STRING=
```

The Gemini file contains only the authorization key or `GEMINI_API_KEY=<value>`.

The production account file is also outside the repository and contains four pairs. These are
production credentials, not the documented demo passwords:

```text
ADMIN_EMAIL=
ADMIN_DISPLAY_NAME=
ADMIN_PASSWORD=
PHARMACIST_EMAIL=
PHARMACIST_DISPLAY_NAME=
PHARMACIST_PASSWORD=
EXPERT_EMAIL=
EXPERT_DISPLAY_NAME=
EXPERT_PASSWORD=
USER_EMAIL=
USER_DISPLAY_NAME=
USER_PASSWORD=
```

Set all three paths in the current process:

```powershell
$env:SOMEE_DEPLOY_FILE = "D:\path\someeDeploy.txt"
$env:GEMINI_KEY_FILE = "D:\path\geminiKey.txt"
$env:N4WTT_ACCOUNTS_FILE = "D:\path\n4wttProductionAccounts.txt"
```

Do not pass secret values as command-line arguments. Never attach these files to Jira or GitHub.

## Release sequence

1. Confirm the release commit and a clean worktree.
2. Run all automated suites and require a passing pull-request workflow.
3. Publish and validate the staging package.
4. Deploy with one paced FTP stream.
5. Verify HTTPS health, RBAC, one no-PII AI call and the PERF10 remote baseline.
6. Record the commit, release tag, result and metrics in Jira without credentials.

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\Publish-Somee.ps1

powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\Deploy-Somee.ps1 `
  -ConfigurationFile $env:SOMEE_DEPLOY_FILE `
  -GeminiKeyFile $env:GEMINI_KEY_FILE `
  -UserAccountsFile $env:N4WTT_ACCOUNTS_FILE `
  -DryRun

powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\Deploy-Somee.ps1 `
  -ConfigurationFile $env:SOMEE_DEPLOY_FILE `
  -GeminiKeyFile $env:GEMINI_KEY_FILE `
  -UserAccountsFile $env:N4WTT_ACCOUNTS_FILE
```

Use `-DisableAi` when the host must run without Gemini. Use `-KeepStaging` only for local diagnosis;
the staging `web.config` contains runtime secrets and remains under the ignored `artifacts/` folder.
Purge old pre-deployment backups after the agreed rollback window because their `web.config` can
contain the previous runtime secrets.

## Deployment safeguards

- Publish and staging paths must remain under `artifacts/`.
- The package must be at most 145 MB, preserving headroom on the 150 MB plan.
- Known secret files are rejected from staging.
- Production account plaintext never enters the publish folder; deployment writes only PBKDF2 salts
  and hashes into the runtime configuration.
- A SHA-256 manifest is written before upload.
- The current remote root listing and existing `web.config`/`default.asp` are backed up locally.
- FTP operations use passive binary mode, pacing, exponential retry and one sequential stream.
- `app_offline.htm` stops the IIS application during replacement.
- `web.config` is uploaded last; `.well-known` and unrelated remote files are not deleted.
- The script removes maintenance mode in `finally` and requires an HTTPS database-connected health gate.

## Rollback

For an upload failure, the script attempts to restore the previous `web.config` and always attempts to
remove `app_offline.htm`. The local backup and manifest are stored under
`artifacts/somee-predeploy-backup/<UTC timestamp>`.

For a full rollback, check out the previous Git release tag, publish it again and run the same deploy
script. Verify `/health`, root, search, login/RBAC and static assets before closing the incident.

## Evidence checklist

- Release commit and tag.
- GitHub Actions run URL and reviewer approval.
- Deploy summary: file count, package size, retry count and backup path.
- HTTPS `/health` response and public route status.
- SQL migration/seed counts.
- Security and RBAC smoke results.
- PERF10 p50, p95, max, request rate and error rate.
