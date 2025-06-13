# 00-build-zip.ps1
# Build and create deployment package WITH ZIP
param([switch]$Quick, [switch]$Force)

# Get current version from Version.props
$version = "?.?.?"
if (Test-Path "Version.props") {
    $content = Get-Content "Version.props" -Raw
    if ($content -match '<VersionPrefix>([^<]+)</VersionPrefix>') {
        $version = $matches[1]
    }
}

$deployPath = ".\Deploy\CamBridge-Deploy-v$version"
$zipPath = ".\Deploy\CamBridge-v$version.zip"

# Check if build AND zip already exist
if ((Test-Path $deployPath) -and (Test-Path $zipPath) -and -not $Force) {
    Write-Host "Build already exists: CamBridge-Deploy-v$version" -ForegroundColor Green
    Write-Host "ZIP already exists: CamBridge-v$version.zip" -ForegroundColor Green
    Write-Host "Last built: $(Get-Item $deployPath | Select-Object -ExpandProperty LastWriteTime)" -ForegroundColor Gray
    Write-Host ""
    Write-Host "Use -Force to rebuild anyway" -ForegroundColor Yellow
    return
}

Write-Host "Building CamBridge Release (With ZIP)..." -ForegroundColor Cyan
if ($Quick) {
    & .\Create-DeploymentPackage.ps1 -SkipClean
} else {
    & .\Create-DeploymentPackage.ps1
}