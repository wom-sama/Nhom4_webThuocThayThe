param(
    [string]$OutputDirectory = ".\artifacts\somee-publish",
    [string]$ArchivePath = ".\artifacts\N4WTT-somee.zip"
)

$ErrorActionPreference = "Stop"
$repoRoot = Split-Path -Parent $PSScriptRoot
$publishPath = [IO.Path]::GetFullPath((Join-Path $repoRoot $OutputDirectory))
$archiveFullPath = [IO.Path]::GetFullPath((Join-Path $repoRoot $ArchivePath))
$artifactsRoot = [IO.Path]::GetFullPath((Join-Path $repoRoot "artifacts")) + [IO.Path]::DirectorySeparatorChar

if (-not $publishPath.StartsWith($artifactsRoot, [StringComparison]::OrdinalIgnoreCase) -or
    -not $archiveFullPath.StartsWith($artifactsRoot, [StringComparison]::OrdinalIgnoreCase)) {
    throw "Publish outputs must stay under $artifactsRoot"
}

if (Test-Path -LiteralPath $publishPath) {
    Remove-Item -LiteralPath $publishPath -Recurse -Force
}

dotnet publish (Join-Path $repoRoot "Nhom4WebThuocThayThe.csproj") `
    -c Release `
    -o $publishPath `
    --nologo
if ($LASTEXITCODE -ne 0) {
    throw "dotnet publish failed"
}

$publishFiles = Get-ChildItem -LiteralPath $publishPath -Recurse -File
$publishBytes = ($publishFiles | Measure-Object Length -Sum).Sum
$maximumBytes = 145MB
if ($publishBytes -gt $maximumBytes) {
    throw "Publish package is $([math]::Round($publishBytes / 1MB, 2)) MB; limit is 145 MB."
}

if (Test-Path -LiteralPath $archiveFullPath) {
    Remove-Item -LiteralPath $archiveFullPath -Force
}

Compress-Archive -Path (Join-Path $publishPath "*") -DestinationPath $archiveFullPath -CompressionLevel Optimal
$archive = Get-Item -LiteralPath $archiveFullPath

[pscustomobject]@{
    PublishDirectory = $publishPath
    PublishMB = [math]::Round($publishBytes / 1MB, 2)
    Archive = $archive.FullName
    ArchiveMB = [math]::Round($archive.Length / 1MB, 2)
}
