# 00-build-zip.ps1
# Build and create deployment package WITH ZIP
param([switch]$Quick)

Write-Host "Building CamBridge Release (With ZIP)..." -ForegroundColor Cyan
if ($Quick) {
    & .\Create-DeploymentPackage.ps1 -SkipClean
} else {
    & .\Create-DeploymentPackage.ps1
}
