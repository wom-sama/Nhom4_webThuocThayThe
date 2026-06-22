[CmdletBinding()]
param(
    [string]$PublishDirectory = ".\artifacts\somee-publish",
    [string]$ConfigurationFile = $env:SOMEE_DEPLOY_FILE,
    [string]$GeminiKeyFile = $env:GEMINI_KEY_FILE,
    [string]$UserAccountsFile = $env:N4WTT_ACCOUNTS_FILE,
    [ValidateRange(0, 10000)]
    [int]$PacingMilliseconds = 850,
    [ValidateRange(1, 10)]
    [int]$MaxAttempts = 7,
    [ValidateRange(1, 60)]
    [int]$HealthAttempts = 18,
    [switch]$DisableAi,
    [switch]$DryRun,
    [switch]$KeepStaging
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"
$retryLimit = $MaxAttempts
$retainStaging = $KeepStaging.IsPresent

$repoRoot = Split-Path -Parent $PSScriptRoot
$artifactsRoot = [IO.Path]::GetFullPath((Join-Path $repoRoot "artifacts"))
$artifactsPrefix = $artifactsRoot + [IO.Path]::DirectorySeparatorChar
$publishRoot = if ([IO.Path]::IsPathRooted($PublishDirectory)) {
    [IO.Path]::GetFullPath($PublishDirectory)
} else {
    [IO.Path]::GetFullPath((Join-Path $repoRoot $PublishDirectory))
}

if (-not $publishRoot.StartsWith($artifactsPrefix, [StringComparison]::OrdinalIgnoreCase)) {
    throw "Publish directory must stay under $artifactsRoot"
}
if (-not (Test-Path -LiteralPath $publishRoot -PathType Container)) {
    throw "Publish directory does not exist. Run scripts/Publish-Somee.ps1 first."
}
if ([string]::IsNullOrWhiteSpace($ConfigurationFile)) {
    throw "Set SOMEE_DEPLOY_FILE or pass -ConfigurationFile with an untracked key=value file."
}

$configurationPath = if ([IO.Path]::IsPathRooted($ConfigurationFile)) {
    [IO.Path]::GetFullPath($ConfigurationFile)
} else {
    [IO.Path]::GetFullPath((Join-Path (Get-Location) $ConfigurationFile))
}
if (-not (Test-Path -LiteralPath $configurationPath -PathType Leaf)) {
    throw "Somee configuration file was not found."
}
$publishPrefix = $publishRoot.TrimEnd("\") + "\"
if ($configurationPath.StartsWith($publishPrefix, [StringComparison]::OrdinalIgnoreCase)) {
    throw "The Somee configuration file must stay outside the publish directory."
}

function Read-KeyValueFile([string]$Path) {
    $values = @{}
    foreach ($rawLine in Get-Content -LiteralPath $Path) {
        $line = $rawLine.Trim()
        if (-not $line -or $line.StartsWith("#")) {
            continue
        }
        if (-not $line.Contains("=")) {
            throw "Invalid configuration line. Expected KEY=VALUE."
        }
        $parts = $line.Split("=", 2)
        $values[$parts[0].Trim()] = $parts[1].Trim()
    }
    return $values
}

function Read-SecretFile([string]$Path) {
    $lines = @(Get-Content -LiteralPath $Path | ForEach-Object { $_.Trim() } |
        Where-Object { $_ -and -not $_.StartsWith("#") })
    if ($lines.Count -eq 0) {
        throw "Secret file is empty."
    }
    $value = $lines[0]
    if ($value -match "^[A-Za-z0-9_]+\s*=") {
        $value = $value.Split("=", 2)[1].Trim()
    }
    if ([string]::IsNullOrWhiteSpace($value)) {
        throw "Secret value is empty."
    }
    return $value
}

$settings = Read-KeyValueFile $configurationPath
$requiredSettings = @(
    "SITE_URL",
    "FTP_HOST",
    "FTP_PORT",
    "FTP_USERNAME",
    "FTP_PASSWORD",
    "FTP_REMOTE_PATH",
    "SQL_CONNECTION_STRING"
)
foreach ($name in $requiredSettings) {
    if ([string]::IsNullOrWhiteSpace($settings[$name])) {
        throw "Missing required setting: $name"
    }
}
if ($settings["FTP_REMOTE_PATH"].Contains("..")) {
    throw "FTP_REMOTE_PATH must not contain parent traversal."
}

$geminiKey = ""
if (-not $DisableAi) {
    if (-not [string]::IsNullOrWhiteSpace($env:GEMINI_API_KEY)) {
        $geminiKey = $env:GEMINI_API_KEY.Trim()
    } elseif (-not [string]::IsNullOrWhiteSpace($GeminiKeyFile)) {
        $geminiPath = if ([IO.Path]::IsPathRooted($GeminiKeyFile)) {
            [IO.Path]::GetFullPath($GeminiKeyFile)
        } else {
            [IO.Path]::GetFullPath((Join-Path (Get-Location) $GeminiKeyFile))
        }
        if (-not (Test-Path -LiteralPath $geminiPath -PathType Leaf)) {
            throw "Gemini key file was not found."
        }
        if ($geminiPath.StartsWith($publishPrefix, [StringComparison]::OrdinalIgnoreCase)) {
            throw "The Gemini key file must stay outside the publish directory."
        }
        $geminiKey = Read-SecretFile $geminiPath
    } else {
        throw "Set GEMINI_API_KEY, GEMINI_KEY_FILE, or pass -DisableAi."
    }
}

if ([string]::IsNullOrWhiteSpace($UserAccountsFile)) {
    throw "Set N4WTT_ACCOUNTS_FILE or pass -UserAccountsFile with an untracked production account file."
}
$accountsPath = if ([IO.Path]::IsPathRooted($UserAccountsFile)) {
    [IO.Path]::GetFullPath($UserAccountsFile)
} else {
    [IO.Path]::GetFullPath((Join-Path (Get-Location) $UserAccountsFile))
}
if (-not (Test-Path -LiteralPath $accountsPath -PathType Leaf)) {
    throw "Production account file was not found."
}
if ($accountsPath.StartsWith($publishPrefix, [StringComparison]::OrdinalIgnoreCase)) {
    throw "The production account file must stay outside the publish directory."
}

$accountSettings = Read-KeyValueFile $accountsPath
$accountDefinitions = @(
    @{ Prefix = "ADMIN"; Role = "Admin" },
    @{ Prefix = "PHARMACIST"; Role = "Pharmacist" },
    @{ Prefix = "EXPERT"; Role = "Expert" },
    @{ Prefix = "USER"; Role = "User" }
)
$productionAccounts = @()
foreach ($definition in $accountDefinitions) {
    $emailKey = "$($definition.Prefix)_EMAIL"
    $displayNameKey = "$($definition.Prefix)_DISPLAY_NAME"
    $passwordKey = "$($definition.Prefix)_PASSWORD"
    if ([string]::IsNullOrWhiteSpace($accountSettings[$emailKey]) -or
        [string]::IsNullOrWhiteSpace($accountSettings[$displayNameKey]) -or
        [string]::IsNullOrWhiteSpace($accountSettings[$passwordKey])) {
        throw "Missing production account setting for $($definition.Prefix)."
    }

    $salt = New-Object byte[] 16
    $random = [Security.Cryptography.RandomNumberGenerator]::Create()
    try {
        $random.GetBytes($salt)
    } finally {
        $random.Dispose()
    }
    $derive = [Security.Cryptography.Rfc2898DeriveBytes]::new(
        $accountSettings[$passwordKey],
        $salt,
        100000,
        [Security.Cryptography.HashAlgorithmName]::SHA256)
    try {
        $hash = $derive.GetBytes(32)
    } finally {
        $derive.Dispose()
    }
    $productionAccounts += [ordered]@{
        Email = $accountSettings[$emailKey]
        DisplayName = $accountSettings[$displayNameKey]
        Role = $definition.Role
        PasswordSalt = [Convert]::ToBase64String($salt)
        PasswordHash = [Convert]::ToBase64String($hash)
        IsLocked = $false
    }
}
$accountsJson = ConvertTo-Json -InputObject $productionAccounts -Depth 5 -Compress
$encodedAccounts = [Convert]::ToBase64String([Text.Encoding]::UTF8.GetBytes($accountsJson))

$timestamp = [DateTime]::UtcNow.ToString("yyyyMMdd-HHmmss")
$stagingRoot = [IO.Path]::GetFullPath((Join-Path $artifactsRoot "somee-deploy-staging"))
$backupRoot = [IO.Path]::GetFullPath((Join-Path $artifactsRoot "somee-predeploy-backup\$timestamp"))
$manifestPath = [IO.Path]::GetFullPath((Join-Path $artifactsRoot "somee-deploy-manifest-$timestamp.json"))
foreach ($path in @($stagingRoot, $backupRoot, $manifestPath)) {
    if (-not $path.StartsWith($artifactsPrefix, [StringComparison]::OrdinalIgnoreCase)) {
        throw "Artifact path escaped $artifactsRoot"
    }
}

if (Test-Path -LiteralPath $stagingRoot) {
    Remove-Item -LiteralPath $stagingRoot -Recurse -Force
}
function Remove-StagingDirectory {
    [CmdletBinding(SupportsShouldProcess, ConfirmImpact = "Low")]
    param()

    if (-not $retainStaging -and
        (Test-Path -LiteralPath $stagingRoot) -and
        $PSCmdlet.ShouldProcess($stagingRoot, "Remove deployment staging directory")) {
        Remove-Item -LiteralPath $stagingRoot -Recurse -Force
    }
}

New-Item -ItemType Directory -Path $stagingRoot -Force | Out-Null
Get-ChildItem -LiteralPath $publishRoot -Force | Copy-Item -Destination $stagingRoot -Recurse -Force

$webConfigPath = Join-Path $stagingRoot "web.config"
if (-not (Test-Path -LiteralPath $webConfigPath -PathType Leaf)) {
    throw "Published web.config is missing."
}

[xml]$webConfig = Get-Content -LiteralPath $webConfigPath -Raw
$aspNetCoreNode = $webConfig.SelectSingleNode("/configuration/location/system.webServer/aspNetCore")
if ($null -eq $aspNetCoreNode) {
    throw "Published web.config does not contain the aspNetCore node."
}
$environmentNode = $aspNetCoreNode.SelectSingleNode("environmentVariables")
if ($null -eq $environmentNode) {
    $environmentNode = $webConfig.CreateElement("environmentVariables")
    [void]$aspNetCoreNode.AppendChild($environmentNode)
}

$runtimeValues = [ordered]@{
    "ASPNETCORE_ENVIRONMENT" = "Production"
    "ConnectionStrings__PharmacyDatabase" = $settings["SQL_CONNECTION_STRING"]
    "AI__Gemini__Enabled" = if ($DisableAi) { "false" } else { "true" }
    "AI__Gemini__ApiKey" = $geminiKey
    "AI__Gemini__Model" = "gemini-2.5-flash"
    "Authentication__EncodedAccounts" = $encodedAccounts
}
foreach ($name in $runtimeValues.Keys) {
    $node = $environmentNode.SelectSingleNode("environmentVariable[@name='$name']")
    if ($null -eq $node) {
        $node = $webConfig.CreateElement("environmentVariable")
        [void]$environmentNode.AppendChild($node)
    }
    $node.SetAttribute("name", $name)
    $node.SetAttribute("value", $runtimeValues[$name])
}

$writerSettings = New-Object System.Xml.XmlWriterSettings
$writerSettings.Indent = $true
$writerSettings.Encoding = New-Object System.Text.UTF8Encoding($false)
$writer = [System.Xml.XmlWriter]::Create($webConfigPath, $writerSettings)
try {
    $webConfig.Save($writer)
} finally {
    $writer.Dispose()
}

$secretFileNames = @("someeDeploy.txt", "geminiKey.txt", "jiraTK.txt", "n4wttProductionAccounts.txt")
$secretFiles = @(Get-ChildItem -LiteralPath $stagingRoot -Recurse -File |
    Where-Object { $secretFileNames -contains $_.Name })
if ($secretFiles.Count -gt 0) {
    throw "A known secret file was copied into staging."
}

$stagingUri = New-Object Uri(($stagingRoot.TrimEnd("\") + "\"))
function Get-StagingRelativePath([string]$FullPath) {
    return [Uri]::UnescapeDataString($stagingUri.MakeRelativeUri((New-Object Uri($FullPath))).ToString())
}

$stagedFiles = @(Get-ChildItem -LiteralPath $stagingRoot -Recurse -File)
$publishBytes = ($stagedFiles | Measure-Object Length -Sum).Sum
if ($publishBytes -gt 145MB) {
    throw "Staged package exceeds the 145 MB deployment guardrail."
}
$manifest = @($stagedFiles | ForEach-Object {
    [pscustomobject]@{
        Path = Get-StagingRelativePath $_.FullName
        Bytes = $_.Length
        Sha256 = (Get-FileHash -LiteralPath $_.FullName -Algorithm SHA256).Hash.ToLowerInvariant()
    }
})
$manifest | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath $manifestPath -Encoding UTF8

if ($DryRun) {
    $result = [pscustomobject]@{
        Mode = "DryRun"
        Files = $stagedFiles.Count
        PublishMB = [math]::Round($publishBytes / 1MB, 2)
        RuntimeVariables = $runtimeValues.Keys -join ", "
        Manifest = $manifestPath
        SecretsCopied = $false
    }
    Remove-StagingDirectory
    return $result
}

$credential = New-Object System.Net.NetworkCredential(
    $settings["FTP_USERNAME"],
    $settings["FTP_PASSWORD"])
$ftpPort = [int]$settings["FTP_PORT"]
$ftpAuthority = if ($ftpPort -eq 21) {
    $settings["FTP_HOST"]
} else {
    "$($settings["FTP_HOST"]):$ftpPort"
}
$remoteRoot = $settings["FTP_REMOTE_PATH"].Trim("/")

function Get-FtpUri([string]$RelativePath = "") {
    $segments = @($remoteRoot) + @($RelativePath.Trim("/").Split("/") | Where-Object { $_ })
    $encodedPath = ($segments | ForEach-Object { [Uri]::EscapeDataString($_) }) -join "/"
    return "ftp://$ftpAuthority/$encodedPath"
}

function Get-FtpRequest([string]$RelativePath, [string]$Method) {
    $request = [System.Net.FtpWebRequest]::Create((Get-FtpUri $RelativePath))
    $request.Method = $Method
    $request.Credentials = $credential
    $request.UsePassive = $true
    $request.UseBinary = $true
    $request.KeepAlive = $false
    $request.Proxy = $null
    $request.Timeout = 120000
    $request.ReadWriteTimeout = 120000
    return $request
}

function Invoke-WithRetry([scriptblock]$Operation, [string]$Label) {
    for ($attempt = 1; $attempt -le $retryLimit; $attempt++) {
        try {
            & $Operation
            return $attempt - 1
        } catch {
            if ($attempt -eq $retryLimit) {
                throw
            }
            $delaySeconds = [Math]::Min(30, 4 * $attempt)
            Write-Warning "Retry $attempt for $Label after ${delaySeconds}s"
            Start-Sleep -Seconds $delaySeconds
        }
    }
}

function Get-FtpName {
    $request = Get-FtpRequest "" ([System.Net.WebRequestMethods+Ftp]::ListDirectory)
    $response = $request.GetResponse()
    try {
        $reader = New-Object IO.StreamReader($response.GetResponseStream())
        try {
            return @($reader.ReadToEnd() -split "`r?`n" | Where-Object { $_ })
        } finally {
            $reader.Dispose()
        }
    } finally {
        $response.Dispose()
    }
}

function Receive-FtpFile([string]$RelativePath, [string]$LocalPath) {
    $request = Get-FtpRequest $RelativePath ([System.Net.WebRequestMethods+Ftp]::DownloadFile)
    $response = $request.GetResponse()
    try {
        $responseStream = $response.GetResponseStream()
        $output = [IO.File]::Create($LocalPath)
        try {
            $responseStream.CopyTo($output)
        } finally {
            $output.Dispose()
            $responseStream.Dispose()
        }
    } finally {
        $response.Dispose()
    }
}

function Send-FtpContent([string]$RelativePath, [byte[]]$Bytes) {
    $request = Get-FtpRequest $RelativePath ([System.Net.WebRequestMethods+Ftp]::UploadFile)
    $request.ContentLength = $Bytes.Length
    $stream = $null
    $response = $null
    try {
        $stream = $request.GetRequestStream()
        $stream.Write($Bytes, 0, $Bytes.Length)
        $stream.Close()
        $stream = $null
        $response = $request.GetResponse()
        $response.Close()
        $response = $null
    } finally {
        if ($null -ne $stream) {
            try { $stream.Dispose() } catch { Write-Warning "FTP request stream disposal failed: $($_.Exception.Message)" }
        }
        if ($null -ne $response) {
            try { $response.Dispose() } catch { Write-Warning "FTP response disposal failed: $($_.Exception.Message)" }
        }
    }
}

function Send-FtpFile([string]$RelativePath, [string]$LocalPath) {
    Send-FtpContent $RelativePath ([IO.File]::ReadAllBytes($LocalPath))
}

function Initialize-FtpDirectory([string]$RelativePath) {
    $request = Get-FtpRequest $RelativePath ([System.Net.WebRequestMethods+Ftp]::MakeDirectory)
    try {
        $response = $request.GetResponse()
        $response.Dispose()
    } catch [System.Net.WebException] {
        $response = $_.Exception.Response
        if ($null -ne $response) {
            $statusCode = [int]$response.StatusCode
            $response.Dispose()
            if ($statusCode -eq 550) {
                return
            }
        }
        throw
    }
}

function Remove-FtpFile {
    [CmdletBinding(SupportsShouldProcess, ConfirmImpact = "Low")]
    param(
        [string]$RelativePath,
        [switch]$IgnoreMissing
    )

    if (-not $PSCmdlet.ShouldProcess($RelativePath, "Delete FTP file")) {
        return
    }

    $request = Get-FtpRequest $RelativePath ([System.Net.WebRequestMethods+Ftp]::DeleteFile)
    try {
        $response = $request.GetResponse()
        $response.Dispose()
    } catch [System.Net.WebException] {
        $response = $_.Exception.Response
        if ($null -ne $response) {
            $statusCode = [int]$response.StatusCode
            $response.Dispose()
            if ($IgnoreMissing -and $statusCode -eq 550) {
                return
            }
        }
        throw
    }
}

$remoteNames = @()
$preflightRetries = Invoke-WithRetry { $script:remoteNames = @(Get-FtpName) } "list remote root"
New-Item -ItemType Directory -Path $backupRoot -Force | Out-Null
$remoteNames | Set-Content -LiteralPath (Join-Path $backupRoot "remote-root.txt") -Encoding UTF8
$backupFiles = @()
foreach ($name in @("web.config", "default.asp")) {
    if ($remoteNames -contains $name) {
        $localBackup = Join-Path $backupRoot $name
        $preflightRetries += Invoke-WithRetry { Receive-FtpFile $name $localBackup } "backup $name"
        $backupFiles += $name
    }
}

$offlineBytes = [Text.Encoding]::UTF8.GetBytes(
    "<!doctype html><html><body><h1>Deployment in progress</h1></body></html>")
[void](Invoke-WithRetry { Send-FtpContent "app_offline.htm" $offlineBytes } "enable maintenance")

$retryCount = $preflightRetries
$uploadedCount = 0
$deploymentError = $null
try {
    $directories = @(Get-ChildItem -LiteralPath $stagingRoot -Recurse -Directory |
        Sort-Object { $_.FullName.Length })
    foreach ($directory in $directories) {
        $relativePath = (Get-StagingRelativePath $directory.FullName).TrimEnd("/")
        $retryCount += Invoke-WithRetry { Initialize-FtpDirectory $relativePath } "mkdir $relativePath"
        Start-Sleep -Milliseconds ([Math]::Min($PacingMilliseconds, 350))
    }

    $payloadFiles = @($stagedFiles | Where-Object { $_.Name -ne "web.config" })
    foreach ($file in $payloadFiles) {
        $relativePath = Get-StagingRelativePath $file.FullName
        $retryCount += Invoke-WithRetry { Send-FtpFile $relativePath $file.FullName } "upload $relativePath"
        $uploadedCount++
        Start-Sleep -Milliseconds $PacingMilliseconds
    }

    $retryCount += Invoke-WithRetry { Send-FtpFile "web.config" $webConfigPath } "upload web.config"
    $uploadedCount++
    $retryCount += Invoke-WithRetry { Remove-FtpFile "default.asp" -IgnoreMissing } "remove default.asp"
} catch {
    $deploymentError = $_
    $webConfigBackup = Join-Path $backupRoot "web.config"
    if (Test-Path -LiteralPath $webConfigBackup) {
        try {
            [void](Invoke-WithRetry { Send-FtpFile "web.config" $webConfigBackup } "restore web.config")
        } catch {
            Write-Warning "The previous web.config could not be restored."
        }
    }
} finally {
    try {
        [void](Invoke-WithRetry { Remove-FtpFile "app_offline.htm" -IgnoreMissing } "disable maintenance")
    } catch {
        if ($null -eq $deploymentError) {
            throw
        }
        Write-Warning "Maintenance file cleanup failed after the deployment error."
    }
}
if ($null -ne $deploymentError) {
    Remove-StagingDirectory
    throw $deploymentError
}

$siteUri = [Uri]$settings["SITE_URL"]
if (-not $siteUri.IsAbsoluteUri -or [string]::IsNullOrWhiteSpace($siteUri.Host)) {
    Remove-StagingDirectory
    throw "SITE_URL must be an absolute URL."
}
$publicBaseUrl = "https://$($siteUri.Host)"
$healthBody = ""
$healthPassed = $false
for ($attempt = 1; $attempt -le $HealthAttempts; $attempt++) {
    try {
        $response = Invoke-WebRequest -Uri "$publicBaseUrl/health" -UseBasicParsing -TimeoutSec 30
        $healthBody = $response.Content
        if ($response.StatusCode -eq 200 -and $healthBody -match '"database"\s*:\s*"connected"') {
            $healthPassed = $true
            break
        }
    } catch {
        $healthBody = "unavailable"
    }
    Start-Sleep -Seconds 5
}
if (-not $healthPassed) {
    Remove-StagingDirectory
    throw "Deployment upload completed, but the HTTPS health gate did not pass."
}

$result = [pscustomobject]@{
    Mode = "Deploy"
    Site = $publicBaseUrl
    Files = $uploadedCount
    PublishMB = [math]::Round($publishBytes / 1MB, 2)
    Retries = $retryCount
    BackupDirectory = $backupRoot
    BackedUpFiles = $backupFiles -join ", "
    Manifest = $manifestPath
    Health = "healthy/database connected"
}
Remove-StagingDirectory
return $result
