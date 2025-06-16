# 0-build-no-qr.ps1
# Build OHNE QRBridge - spart ~1 Minute Build-Zeit!
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

# Check if build already exists
if ((Test-Path $deployPath) -and -not $Force) {
    Write-Host "Build already exists: CamBridge-Deploy-v$version" -ForegroundColor Green
    Write-Host "Last built: $(Get-Item $deployPath | Select-Object -ExpandProperty LastWriteTime)" -ForegroundColor Gray
    Write-Host ""
    Write-Host "Note: This build might include QRBridge if built with 0-build.ps1" -ForegroundColor DarkGray
    Write-Host "Use -Force to rebuild without QRBridge" -ForegroundColor Yellow
    return
}

Write-Host "Building CamBridge Release (No QRBridge, No ZIP)..." -ForegroundColor Cyan
Write-Host "This saves ~1 minute build time!" -ForegroundColor Green
Write-Host ""

if ($Quick) {
    & .\Create-DeploymentPackage.ps1 -SkipClean -SkipZip -SkipQRBridge
} else {
    & .\Create-DeploymentPackage.ps1 -SkipZip -SkipQRBridge
}