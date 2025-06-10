# 99-testit-full.ps1
# The FULL test cycle: Build + Update service + start config
Write-Host "Full Test Cycle: Build + Update + Config" -ForegroundColor Cyan
Write-Host ""
& .\1-deploy-update.ps1 -StartConfig
