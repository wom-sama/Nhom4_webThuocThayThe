# Sprint S1,6 - Security Test Plan

## Objective

Validate the current security controls before the project moves toward database and recommendation workflows. The plan incorporates the updated project notes: hashed passwords, role-based authorization, anti-forgery protection, safe error handling and practical security regression tests.

## Scope

- Seed account password storage.
- Login failure/success behavior.
- Authentication cookie flags.
- Role-based access matrix for public, user and pharmacist/admin workflows.
- CSRF protection on mutation forms.
- Open redirect protection through `ReturnUrl`.
- Search input HTML encoding.
- Error response leakage checks.

## Test Environment

| Item | Value |
| --- | --- |
| App | ASP.NET Core MVC |
| Runtime | .NET 10 |
| Data | In-memory seed users and pharmacy data |
| Runner | `tests/Nhom4WebThuocThayThe.SecurityTests` |
| Report | `TestResults/security-report.json` |

## Entry Criteria

- Security test runner builds in `CMPM.sln`.
- Password storage no longer uses the plaintext `Password` property on `AppUser`.
- Existing acceptance tests still pass after the password hashing change.

## Exit Criteria

- Security runner passes 10/10 checks.
- Acceptance runner still passes 20/20 checks.
- Security test matrix is traceable to Jira issues in S1,6.
- Remaining risks are documented for the database/Identity sprint.
