# 0-build.ps1
# Build and create deployment package (NO ZIP - faster!)
param([switch]$Quick)

Write-Host "Building CamBridge Release (No ZIP)..." -ForegroundColor Cyan
if ($Quick) {
    & .\Create-DeploymentPackage.ps1 -SkipClean -SkipZip
} else {
    & .\Create-DeploymentPackage.ps1 -SkipZip
}
