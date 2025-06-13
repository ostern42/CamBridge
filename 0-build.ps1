# 0-build.ps1
# Build and create deployment package (NO ZIP - faster!)
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
    Write-Host "Use -Force to rebuild anyway" -ForegroundColor Yellow
    return
}

Write-Host "Building CamBridge Release (No ZIP)..." -ForegroundColor Cyan
if ($Quick) {
    & .\Create-DeploymentPackage.ps1 -SkipClean -SkipZip
} else {
    & .\Create-DeploymentPackage.ps1 -SkipZip
}