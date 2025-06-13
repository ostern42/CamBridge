# 99-testit-full.ps1
# Full test with build: Build + Update + Config
# FIXED: Only builds if needed

Write-Host "Full Test: Build + Update + Config" -ForegroundColor Cyan
Write-Host ""

# First ensure we have a build (smart check)
& .\0-build.ps1

# Then deploy and start config (skip redundant build)
& .\1-deploy-update.ps1 -StartConfig -SkipBuild