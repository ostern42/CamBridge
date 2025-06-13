# h-help.ps1
# Quick help for numbered tools

Clear-Host
Write-Host "================================================" -ForegroundColor Cyan
Write-Host "    CamBridge Numbered Tools - Quick Help" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Just type number + TAB to use!" -ForegroundColor Yellow
Write-Host ""
Write-Host "  0  - Build (no ZIP)                 " -NoNewline; Write-Host "0-build [-Quick] [-Force]" -ForegroundColor DarkGray
Write-Host "       Smart: Skips if already built" -ForegroundColor DarkGray
Write-Host " 00  - Build WITH ZIP                " -NoNewline; Write-Host "00-build-zip [-Quick] [-Force]" -ForegroundColor DarkGray
Write-Host "  1  - Update service cycle           " -NoNewline; Write-Host "1-deploy-update [-StartConfig] [-ForceBuild]" -ForegroundColor DarkGray
Write-Host "       Fixed: v0.7.10 > v0.7.9 now!" -ForegroundColor DarkGray
Write-Host "  2  - Start Config UI                " -NoNewline; Write-Host "2-config" -ForegroundColor DarkGray
Write-Host "  3  - Service Manager menu           " -NoNewline; Write-Host "3-service" -ForegroundColor DarkGray
Write-Host "  4  - Console mode (debug)           " -NoNewline; Write-Host "4-console" -ForegroundColor DarkGray
Write-Host "  5  - API testing                    " -NoNewline; Write-Host "5-test-api [-Continuous]" -ForegroundColor DarkGray
Write-Host "  6  - Open log folder                " -NoNewline; Write-Host "6-logs" -ForegroundColor DarkGray
Write-Host "  7  - Clean everything               " -NoNewline; Write-Host "7-clean [-Force]" -ForegroundColor DarkGray
Write-Host "  8  - Quick status check             " -NoNewline; Write-Host "8-status" -ForegroundColor DarkGray
Write-Host "  9  - Test (no build)                " -NoNewline; Write-Host "9-testit" -ForegroundColor DarkGray
Write-Host "       Assumes 0[TAB] already run" -ForegroundColor DarkGray
Write-Host " 99  - FULL test (with build)         " -NoNewline; Write-Host "99-testit-full" -ForegroundColor DarkGray
Write-Host "       Smart: Only builds if needed" -ForegroundColor DarkGray
Write-Host "  h  - This help                      " -NoNewline; Write-Host "h-help" -ForegroundColor DarkGray
Write-Host ""
Write-Host "================================================" -ForegroundColor Gray
Write-Host ""
Write-Host "Common workflows:" -ForegroundColor Yellow
Write-Host ""
Write-Host "  Dev testing (fast):       " -NoNewline; Write-Host "0[TAB] then 9[TAB]" -ForegroundColor Green
Write-Host "  Distribution build:       " -NoNewline; Write-Host "00[TAB]" -ForegroundColor Green
Write-Host "  Full test with build:     " -NoNewline; Write-Host "99[TAB]" -ForegroundColor Green
Write-Host "  Fresh start:              " -NoNewline; Write-Host "7[TAB] then 0[TAB] then 1[TAB]" -ForegroundColor Green
Write-Host ""
Write-Host "Build behavior (NEW):" -ForegroundColor Yellow
Write-Host "  - 0[TAB] checks if build exists, skips if yes" -ForegroundColor Gray
Write-Host "  - 1[TAB] uses existing build (no rebuild)" -ForegroundColor Gray
Write-Host "  - 9[TAB] never builds (assumes you did 0[TAB])" -ForegroundColor Gray
Write-Host "  - Version sorting fixed (10 > 9 correctly)" -ForegroundColor Gray
Write-Host ""
Write-Host "Location: C:\Users\oliver.stern\source\repos\CamBridge" -ForegroundColor DarkGray
Write-Host ""