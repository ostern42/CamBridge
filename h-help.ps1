# h-help.ps1
# Quick help for numbered tools

Clear-Host
Write-Host "================================================" -ForegroundColor Cyan
Write-Host "    CamBridge Numbered Tools - v0.7.17" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Just type number + TAB to use!" -ForegroundColor Yellow
Write-Host ""
Write-Host "  0  - Smart Build (no ZIP/QR)        " -NoNewline; Write-Host "0-build [-Quick] [-Force]" -ForegroundColor DarkGray
Write-Host "       NEW: Auto-detects changes!" -ForegroundColor Green
Write-Host " 00  - Full Build WITH ZIP           " -NoNewline; Write-Host "00-build-zip [-Quick] [-Force]" -ForegroundColor DarkGray
Write-Host "  1  - Deploy & Start Service         " -NoNewline; Write-Host "1-deploy-update [-StartConfig]" -ForegroundColor DarkGray
Write-Host "  2  - Start Config UI                " -NoNewline; Write-Host "2-config" -ForegroundColor DarkGray
Write-Host "  3  - Service Manager menu           " -NoNewline; Write-Host "3-service" -ForegroundColor DarkGray
Write-Host "  4  - Console mode (debug)           " -NoNewline; Write-Host "4-console" -ForegroundColor DarkGray
Write-Host "  5  - API testing                    " -NoNewline; Write-Host "5-test-api [-Continuous]" -ForegroundColor DarkGray
Write-Host "       v0.7.17: All endpoints work!" -ForegroundColor Green
Write-Host "  6  - Open log folder                " -NoNewline; Write-Host "6-logs" -ForegroundColor DarkGray
Write-Host "  7  - Clean everything               " -NoNewline; Write-Host "7-clean [-Force]" -ForegroundColor DarkGray
Write-Host "  8  - Quick status check             " -NoNewline; Write-Host "8-status" -ForegroundColor DarkGray
Write-Host "  9  - Quick test (no build)          " -NoNewline; Write-Host "9-testit" -ForegroundColor DarkGray
Write-Host " 99  - Full test (with build)         " -NoNewline; Write-Host "99-testit-full" -ForegroundColor DarkGray
Write-Host "  h  - This help                      " -NoNewline; Write-Host "h-help" -ForegroundColor DarkGray
Write-Host ""
Write-Host "================================================" -ForegroundColor Gray
Write-Host ""
Write-Host "Common workflows:" -ForegroundColor Yellow
Write-Host ""
Write-Host "  Development cycle:        " -NoNewline; Write-Host "0[TAB] → 1[TAB] → 4[TAB]" -ForegroundColor Green
Write-Host "  Quick config change:      " -NoNewline; Write-Host "2[TAB] → Save → 9[TAB]" -ForegroundColor Green
Write-Host "  API testing:              " -NoNewline; Write-Host "5[TAB] -Continuous" -ForegroundColor Green
Write-Host "  Distribution build:       " -NoNewline; Write-Host "00[TAB]" -ForegroundColor Green
Write-Host "  Fresh start:              " -NoNewline; Write-Host "7[TAB] → 0[TAB] → 1[TAB]" -ForegroundColor Green
Write-Host ""
Write-Host "NEW Smart Build Features:" -ForegroundColor Yellow
Write-Host "  ✓ Auto-detects source changes" -ForegroundColor Gray
Write-Host "  ✓ No need for -Force during development" -ForegroundColor Gray
Write-Host "  ✓ Shows which files changed" -ForegroundColor Gray
Write-Host "  ✓ Skips QRBridge (saves 1 minute)" -ForegroundColor Gray
Write-Host ""
Write-Host "Build Times:" -ForegroundColor Yellow
Write-Host "  0[TAB]  = ~20 seconds (Service + Config)" -ForegroundColor Gray
Write-Host "  00[TAB] = ~90 seconds (+ QRBridge + ZIP)" -ForegroundColor Gray
Write-Host ""
Write-Host "API Endpoints (v0.7.17):" -ForegroundColor Yellow
Write-Host "  /health                  - Basic health" -ForegroundColor Gray
Write-Host "  /api/status              - Full status" -ForegroundColor Gray
Write-Host "  /api/status/version      - Version only " -ForegroundColor Green -NoNewline; Write-Host " NEW!" -ForegroundColor Yellow
Write-Host "  /api/status/health       - Health details " -ForegroundColor Green -NoNewline; Write-Host " NEW!" -ForegroundColor Yellow
Write-Host "  /api/pipelines           - All pipelines" -ForegroundColor Gray
Write-Host ""
Write-Host "Tips:" -ForegroundColor Yellow
Write-Host "  • Config validation warns but doesn't block" -ForegroundColor Gray
Write-Host "  • Invalid pipelines are skipped (not crashed)" -ForegroundColor Gray
Write-Host "  • Logs always go to file (+ console in mode 4)" -ForegroundColor Gray
Write-Host ""
Write-Host "Location: $(Get-Location)" -ForegroundColor DarkGray
Write-Host ""