# Nhom4 Web Thuoc Thay The

ASP.NET Core MVC framework for the "Web de xuat thuoc thay the khi thuoc chinh khong co san" software engineering course project.

## Links

- Jira project: [Nhom4_WebThuocThayThe](https://nguyennamnhn125.atlassian.net/jira/software/c/projects/N4WTT/boards/36)
- GitHub repository: [wom-sama/Nhom4_webThuocThayThe](https://github.com/wom-sama/Nhom4_webThuocThayThe)

## Tech Stack

- .NET 10
- ASP.NET Core MVC
- Razor Views
- Bootstrap
- EF Core 10.0.9
- SQL Server / LocalDB

## Run Locally

```powershell
dotnet restore
dotnet tool restore
dotnet ef database update
dotnet build
dotnet run
```

Open the local URL printed by `dotnet run`.

## Test Locally

```powershell
dotnet restore .\CMPM.sln
dotnet build .\CMPM.sln --no-restore
dotnet test .\CMPM.sln --no-build
dotnet run --project .\tests\Nhom4WebThuocThayThe.AcceptanceTests\Nhom4WebThuocThayThe.AcceptanceTests.csproj --no-build
dotnet run --project .\tests\Nhom4WebThuocThayThe.SecurityTests\Nhom4WebThuocThayThe.SecurityTests.csproj --no-build
dotnet run --project .\tests\Nhom4WebThuocThayThe.PerformanceTests\Nhom4WebThuocThayThe.PerformanceTests.csproj --no-build
```

Each runner starts the MVC app on a free local port and writes JSON result files under `TestResults/`.

## Run With Docker

Copy `.env.example` to `.env`, replace the sample SQL password, then run:

```powershell
docker compose up --build -d
docker compose ps
Invoke-WebRequest http://localhost:8080/health
```

The compose stack runs the web image as a non-root user and persists SQL Server data in a named volume.

## Jira Workflow

Use Jira issue keys in branch names, commit messages, and pull request titles.

Examples:

- Branch: `N4WTT-1-setup-repository`
- Commit: `N4WTT-1 Initialize repository`
- Pull request: `N4WTT-1 Initialize repository`

## Product Features

- Role-based login for Admin, Pharmacist, Expert and User.
- Drug search by name, active ingredient and category.
- Drug catalog and inventory/batch administration.
- Stock-aware substitute recommendation with score and explanation.
- Prescription, active ingredient, allergy and contraindication warnings.
- Expert review workflow for recommendation results.
- Dashboard for stock risk, external sources, audit logs and backup metadata.
- SQL Server persistence with EF Core migration and JSON database backup.
- Automated acceptance, security and realtime performance test runners.

Current automated baseline:

- Acceptance: 31 cases.
- Security: 12 cases, including login throttling and browser hardening headers.
- Performance: 9 scenarios, including 100 virtual users.
- Unit and SQL Server integration tests: xUnit projects in `tests/`.

The `S5 Container Validation` GitHub Actions workflow runs the suites against SQL Server 2022, builds the Docker image and smoke-tests the Compose stack across a web-container restart.

Scrum continuity documents are under `docs/scrum/`.

Database documentation and the idempotent schema script are under `database/`.
