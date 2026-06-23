# S22 User Account, Library Persistence and Production Evidence

Date: 2026-06-23

Jira: N4WTT-274

Production URL: https://nnhom4web.somee.com

## Scope

- Public users can register their own account.
- Admin can view all seed and database-backed accounts.
- Admin can create managed accounts, lock/unlock database accounts, and reset database account passwords.
- User search history is persisted when a signed-in user searches by keyword or category.
- User saved drugs are persisted, deduplicated by user and drug, visible in the user area, and removable.
- Drug details expose a visible save/unsave action for normal users.

## Database Change

Migration: `20260623133411_UserAccountsAndLibraryPersistence`

Tables:

- `RegisteredUserAccounts`
- `UserSearchHistories`
- `SavedDrugs`

Important constraints:

- `RegisteredUserAccounts.Email` primary key.
- `SavedDrugs(UserEmail, DrugId)` unique index.
- `SavedDrugs.DrugId` foreign key to `Drugs.Id`.

## Local Verification

- `dotnet test CMPM.sln --no-restore`: 31 unit tests and 6 integration tests passed.
- Acceptance console: 50/50 passed.
- Security console: 17/17 passed.

Reports:

- `TestResults/acceptance-report.json`
- `TestResults/security-report.json`

## Production Deployment

Deploy script: `scripts/Deploy-Somee.ps1`

Result:

- Site: `https://nnhom4web.somee.com`
- Files uploaded: 93
- Publish size: 21.04 MB
- Health gate: `healthy/database connected`
- Backup directory: `artifacts/somee-predeploy-backup/20260623-133841`
- Manifest: `artifacts/somee-deploy-manifest-20260623-133841.json`

## Production Verification

Production test suite:

- `dotnet run --project tests/Nhom4WebThuocThayThe.ProductionTests/Nhom4WebThuocThayThe.ProductionTests.csproj -c Release --no-build`
- Result: 31/31 passed.
- Report: `TestResults/production-validation-report.json`

New production case:

- `PROD30 User accounts`: registered a new production user, searched Panadol, verified history, saved Panadol, and verified Admin `/Admin/Accounts` lists the database-backed account.

Browser smoke:

- Registered `browser-s22-*.example.test` through the visible `/Auth/Register` form.
- Searched `Panadol` through the visible search form.
- Saved Panadol from `/Drugs/Details/1`.
- Verified `/User/Home/History` and `/User/Home/Saved` show the browser-created data.
- Logged in as Admin and verified `/Admin/Accounts` shows seed accounts, database accounts, lock action, and reset action.
- Verified mobile viewport `390x844` can sign in and render `/User` without a 400 response.

Screenshot:

- `TestResults/browser-s22-admin-accounts-1782222733995.png`
