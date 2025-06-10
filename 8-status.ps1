# 8-status.ps1
# Quick status check
Write-Host "CamBridge Status Check" -ForegroundColor Cyan
Write-Host ""

# Service status
$service = Get-Service "CamBridgeService" -ErrorAction SilentlyContinue
if ($service) {
    $color = switch ($service.Status) {
        "Running" { "Green" }
        "Stopped" { "Red" }
        default { "Yellow" }
    }
    Write-Host "Service: $($service.Status)" -ForegroundColor $color
} else {
    Write-Host "Service: NOT INSTALLED" -ForegroundColor DarkGray
}

# API status
try {
    $health = Invoke-RestMethod "http://localhost:5050/health" -TimeoutSec 2
    Write-Host "API: $($health.status)" -ForegroundColor Green
} catch {
    Write-Host "API: OFFLINE" -ForegroundColor Red
}

# Latest deployment
$latestDeploy = Get-ChildItem -Path ".\Deploy" -Filter "CamBridge-Deploy-v*" -Directory | 
                Sort-Object Name -Descending | 
                Select-Object -First 1
if ($latestDeploy) {
    Write-Host "Latest: $($latestDeploy.Name)" -ForegroundColor Gray
} else {
    Write-Host "Latest: NO DEPLOYMENT" -ForegroundColor DarkGray
}

Write-Host ""
