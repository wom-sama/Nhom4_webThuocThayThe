[CmdletBinding()]
param(
    [string]$BaseUrl = "https://nnhom4web.somee.com",
    [string]$JiraBaseUrl = "https://nguyennamnhn125.atlassian.net",
    [string]$JiraEmail = "nvu15673@gmail.com",
    [string]$JiraTokenFile = "D:\OneDrive\Desktop\vuTK.txt",
    [string]$MonitoringIssueKey = "N4WTT-203",
    [string]$DefectIssueKey = "N4WTT-204",
    [string]$ArtifactDirectory = ".\artifacts\monitoring",
    [ValidateRange(1, 20)]
    [int]$Rounds = 5,
    [ValidateRange(0, 10000)]
    [int]$PacingMilliseconds = 300,
    [switch]$Force,
    [switch]$SkipJira
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12

$repoRoot = Split-Path -Parent $PSScriptRoot
$artifactsRoot = [IO.Path]::GetFullPath((Join-Path $repoRoot "artifacts"))
$artifactsPrefix = $artifactsRoot + [IO.Path]::DirectorySeparatorChar
$monitoringRoot = if ([IO.Path]::IsPathRooted($ArtifactDirectory)) {
    [IO.Path]::GetFullPath($ArtifactDirectory)
} else {
    [IO.Path]::GetFullPath((Join-Path $repoRoot $ArtifactDirectory))
}
if (-not $monitoringRoot.StartsWith($artifactsPrefix, [StringComparison]::OrdinalIgnoreCase)) {
    throw "Monitoring artifacts must stay under $artifactsRoot"
}

$siteUri = [Uri]$BaseUrl
if (-not $siteUri.IsAbsoluteUri -or $siteUri.Scheme -ne "https") {
    throw "BaseUrl must be an absolute HTTPS URL."
}
$jiraUri = [Uri]$JiraBaseUrl
if (-not $jiraUri.IsAbsoluteUri -or $jiraUri.Scheme -ne "https") {
    throw "JiraBaseUrl must be an absolute HTTPS URL."
}

function ConvertTo-AsciiJson {
    param(
        [Parameter(Mandatory)]
        [object]$Value,
        [int]$Depth = 20
    )

    $json = $Value | ConvertTo-Json -Depth $Depth
    return [regex]::Replace($json, "[^\x00-\x7F]", {
        param($match)
        return "\u{0:x4}" -f [int][char]$match.Value
    })
}

function Get-Percentile {
    param(
        [Parameter(Mandatory)]
        [long[]]$Value,
        [Parameter(Mandatory)]
        [ValidateRange(0, 1)]
        [double]$Percentile
    )

    $ordered = @($Value | Sort-Object)
    if ($ordered.Count -eq 0) {
        return 0
    }
    $index = [Math]::Ceiling(($ordered.Count - 1) * $Percentile)
    return $ordered[$index]
}

function Get-RouteSample {
    param(
        [Parameter(Mandatory)]
        [string]$PublicBaseUrl,
        [Parameter(Mandatory)]
        [string]$Path,
        [Parameter(Mandatory)]
        [int]$Round
    )

    $stopwatch = [Diagnostics.Stopwatch]::StartNew()
    try {
        $response = Invoke-WebRequest -UseBasicParsing -Uri ($PublicBaseUrl.TrimEnd("/") + $Path) -TimeoutSec 30
        $stopwatch.Stop()
        $databaseConnected = $null
        if ($Path -eq "/health") {
            try {
                $health = $response.Content | ConvertFrom-Json
                $databaseConnected = $health.status -eq "healthy" -and $health.database -eq "connected"
            } catch {
                $databaseConnected = $false
            }
        }
        return [pscustomobject]@{
            Round = $Round
            Path = $Path
            Status = [int]$response.StatusCode
            Milliseconds = $stopwatch.ElapsedMilliseconds
            Bytes = $response.RawContentLength
            Hsts = [bool]$response.Headers["Strict-Transport-Security"]
            Csp = [bool]$response.Headers["Content-Security-Policy"]
            Frame = $response.Headers["X-Frame-Options"]
            DatabaseConnected = $databaseConnected
            Error = $null
        }
    } catch {
        $stopwatch.Stop()
        return [pscustomobject]@{
            Round = $Round
            Path = $Path
            Status = 0
            Milliseconds = $stopwatch.ElapsedMilliseconds
            Bytes = 0
            Hsts = $false
            Csp = $false
            Frame = $null
            DatabaseConnected = if ($Path -eq "/health") { $false } else { $null }
            Error = $_.Exception.Message
        }
    }
}

function Get-JiraHeader {
    param(
        [Parameter(Mandatory)]
        [string]$Email,
        [Parameter(Mandatory)]
        [string]$TokenPath
    )

    if (-not (Test-Path -LiteralPath $TokenPath -PathType Leaf)) {
        throw "Jira token file was not found."
    }
    $token = (Get-Content -LiteralPath $TokenPath -Raw).Trim()
    if ([string]::IsNullOrWhiteSpace($token)) {
        throw "Jira token file is empty."
    }
    $basic = [Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes("${Email}:${token}"))
    return @{
        Authorization = "Basic $basic"
        Accept = "application/json"
        "Content-Type" = "application/json"
    }
}

function Send-JiraComment {
    param(
        [Parameter(Mandatory)]
        [string]$JiraUrl,
        [Parameter(Mandatory)]
        [hashtable]$Headers,
        [Parameter(Mandatory)]
        [string]$IssueKey,
        [Parameter(Mandatory)]
        [string]$Text
    )

    $document = @{
        type = "doc"
        version = 1
        content = @(@{
            type = "paragraph"
            content = @(@{ type = "text"; text = $Text })
        })
    }
    $body = ConvertTo-AsciiJson @{ body = $document }
    Invoke-RestMethod -Method Post -Headers $Headers `
        -Uri ($JiraUrl.TrimEnd("/") + "/rest/api/3/issue/$IssueKey/comment") `
        -Body ([Text.Encoding]::UTF8.GetBytes($body)) | Out-Null
}

function Send-JiraTransition {
    param(
        [Parameter(Mandatory)]
        [string]$JiraUrl,
        [Parameter(Mandatory)]
        [hashtable]$Headers,
        [Parameter(Mandatory)]
        [string]$IssueKey,
        [Parameter(Mandatory)]
        [string]$TransitionId
    )

    $body = ConvertTo-AsciiJson @{ transition = @{ id = $TransitionId } }
    Invoke-RestMethod -Method Post -Headers $Headers `
        -Uri ($JiraUrl.TrimEnd("/") + "/rest/api/3/issue/$IssueKey/transitions") `
        -Body ([Text.Encoding]::UTF8.GetBytes($body)) | Out-Null
}

New-Item -ItemType Directory -Path $monitoringRoot -Force | Out-Null
$timeZone = [TimeZoneInfo]::FindSystemTimeZoneById("SE Asia Standard Time")
$localNow = [TimeZoneInfo]::ConvertTimeFromUtc([DateTime]::UtcNow, $timeZone)
$date = $localNow.ToString("yyyy-MM-dd")
$reportPath = Join-Path $monitoringRoot "$date.json"

if ((Test-Path -LiteralPath $reportPath) -and -not $Force) {
    return [pscustomobject]@{
        Mode = "Skipped"
        Reason = "A monitoring report already exists for $date."
        Report = $reportPath
    }
}

$routes = @("/health", "/", "/Drugs?keyword=para", "/Drugs/Details/1")
$samples = @()
for ($round = 1; $round -le $Rounds; $round++) {
    foreach ($path in $routes) {
        $samples += Get-RouteSample -PublicBaseUrl $BaseUrl -Path $path -Round $round
        if ($PacingMilliseconds -gt 0) {
            Start-Sleep -Milliseconds $PacingMilliseconds
        }
    }
}

$summary = @()
foreach ($path in $routes) {
    $routeSamples = @($samples | Where-Object Path -eq $path)
    $latencies = [long[]]@($routeSamples | Select-Object -ExpandProperty Milliseconds)
    $summary += [pscustomobject]@{
        Path = $path
        Samples = $routeSamples.Count
        Success = @($routeSamples | Where-Object Status -eq 200).Count
        P50Ms = Get-Percentile -Value $latencies -Percentile 0.50
        P95Ms = Get-Percentile -Value $latencies -Percentile 0.95
        MaxMs = ($latencies | Measure-Object -Maximum).Maximum
        SecurityHeaders = @($routeSamples | Where-Object {
            $_.Hsts -and $_.Csp -and $_.Frame -eq "DENY"
        }).Count
    }
}

$failedStatus = @($samples | Where-Object Status -ne 200).Count -gt 0
$failedDatabase = @($samples | Where-Object {
    $_.Path -eq "/health" -and $_.DatabaseConnected -ne $true
}).Count -gt 0
$failedHeaders = @($samples | Where-Object {
    -not $_.Hsts -or -not $_.Csp -or $_.Frame -ne "DENY"
}).Count -gt 0
$failedLatency = @($summary | Where-Object P95Ms -gt 1500).Count -gt 0
$passed = -not ($failedStatus -or $failedDatabase -or $failedHeaders -or $failedLatency)

$report = [ordered]@{
    Date = $date
    TimeZone = "Asia/Bangkok"
    GeneratedAt = $localNow.ToString("o")
    BaseUrl = $BaseUrl
    Passed = $passed
    FailureFlags = [ordered]@{
        Status = $failedStatus
        Database = $failedDatabase
        SecurityHeaders = $failedHeaders
        P95Latency = $failedLatency
    }
    Summary = $summary
    Samples = $samples
    SecretsRecorded = $false
}
$report | ConvertTo-Json -Depth 10 | Set-Content -LiteralPath $reportPath -Encoding UTF8

$dailyReports = @(Get-ChildItem -LiteralPath $monitoringRoot -File -Filter "????-??-??.json" |
    Sort-Object Name)
$dayNumber = $dailyReports.Count

if (-not $SkipJira) {
    $headers = Get-JiraHeader -Email $JiraEmail -TokenPath $JiraTokenFile
    $metrics = @($summary | ForEach-Object {
        "$($_.Path): $($_.Success)/$($_.Samples), p50=$($_.P50Ms)ms, p95=$($_.P95Ms)ms, max=$($_.MaxMs)ms"
    }) -join "; "
    $resultLabel = if ($passed) { "PASS" } else { "FAIL" }
    Send-JiraComment -JiraUrl $JiraBaseUrl -Headers $headers -IssueKey $MonitoringIssueKey `
        -Text "Monitoring day $dayNumber/7 ($date): $resultLabel. $metrics. Evidence: artifacts/monitoring/$date.json; no secrets recorded."

    if (-not $passed) {
        $dailyLabel = "monitoring-$date"
        $jql = [Uri]::EscapeDataString("project=N4WTT AND labels=$dailyLabel")
        $existing = Invoke-RestMethod -Method Get -Headers $headers `
            -Uri ($JiraBaseUrl.TrimEnd("/") + "/rest/api/3/search/jql?jql=$jql&maxResults=1&fields=key")
        if (@($existing.issues).Count -eq 0) {
            $severity = if ($failedStatus -or $failedDatabase) { "p1" } else { "p2" }
            $description = @{
                type = "doc"
                version = 1
                content = @(@{
                    type = "paragraph"
                    content = @(@{
                        type = "text"
                        text = "Production monitoring failed on $date. Status=$failedStatus; Database=$failedDatabase; Headers=$failedHeaders; P95=$failedLatency. See sanitized evidence at artifacts/monitoring/$date.json."
                    })
                })
            }
            $issueBody = @{
                fields = @{
                    project = @{ key = "N4WTT" }
                    issuetype = @{ id = "10044" }
                    parent = @{ key = "N4WTT-189" }
                    summary = "Production monitoring failure $date"
                    description = $description
                    assignee = @{ accountId = "712020:2f804fd8-1552-4e07-a028-578aafcb5c7c" }
                    labels = @("s7", "monitoring", $dailyLabel, $severity)
                    customfield_10020 = 48
                }
            }
            $issueJson = ConvertTo-AsciiJson $issueBody
            Invoke-RestMethod -Method Post -Headers $headers `
                -Uri ($JiraBaseUrl.TrimEnd("/") + "/rest/api/3/issue") `
                -Body ([Text.Encoding]::UTF8.GetBytes($issueJson)) | Out-Null
        }
    }

    if ($dayNumber -ge 7) {
        $historicalFailures = @($dailyReports | Where-Object {
            try {
                -not ((Get-Content -LiteralPath $_.FullName -Raw | ConvertFrom-Json).Passed)
            } catch {
                $true
            }
        }).Count
        $openJql = [Uri]::EscapeDataString(
            "project=N4WTT AND parent=N4WTT-189 AND labels in (p0,p1) AND statusCategory != Done")
        $openCritical = Invoke-RestMethod -Method Get -Headers $headers `
            -Uri ($JiraBaseUrl.TrimEnd("/") + "/rest/api/3/search/jql?jql=$openJql&maxResults=1&fields=key")
        if ($historicalFailures -eq 0 -and @($openCritical.issues).Count -eq 0) {
            Send-JiraComment -JiraUrl $JiraBaseUrl -Headers $headers -IssueKey $MonitoringIssueKey `
                -Text "Seven distinct daily reports are present with no failed day and no open P0/P1. Monitoring acceptance criteria are complete."
            Send-JiraTransition -JiraUrl $JiraBaseUrl -Headers $headers -IssueKey $MonitoringIssueKey -TransitionId "31"
            Send-JiraTransition -JiraUrl $JiraBaseUrl -Headers $headers -IssueKey $DefectIssueKey -TransitionId "21"
        }
    }
}

$result = [pscustomobject]@{
    Mode = "Monitored"
    Date = $date
    Passed = $passed
    DailyReports = $dayNumber
    Report = $reportPath
    JiraUpdated = -not $SkipJira
}
if (-not $passed) {
    Write-Error "Production monitoring failed. Review $reportPath"
}
return $result
