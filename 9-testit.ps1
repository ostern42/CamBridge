# 9-testit.ps1
# The ultimate test shortcut: Update service + start config
Write-Host "Quick Test: Update + Config" -ForegroundColor Cyan
& .\1-deploy-update.ps1 -StartConfig -SkipBuild