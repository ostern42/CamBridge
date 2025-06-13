# 8-status.ps1
# Quick status check
# FIXED: Proper version sorting + correct API port

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

# API status - FIX: Port 5111 not 5050!
try {
    $health = Invoke-RestMethod "http://localhost:5111/health" -TimeoutSec 2
    Write-Host "API: $($health.status)" -ForegroundColor Green
} catch {
    Write-Host "API: OFFLINE" -ForegroundColor Red
}

# Latest deployment - FIXED version sorting
$latestDeploy = Get-ChildItem -Path ".\Deploy" -Filter "CamBridge-Deploy-v*" -Directory | 
                ForEach-Object {
                    if ($_.Name -match 'v(\d+)\.(\d+)\.(\d+)') {
                        [PSCustomObject]@{
                            Directory = $_
                            Version = [Version]::new([int]$matches[1], [int]$matches[2], [int]$matches[3])
                        }
                    }
                } |
                Sort-Object Version -Descending |
                Select-Object -First 1 -ExpandProperty Directory

if ($latestDeploy) {
    Write-Host "Latest: $($latestDeploy.Name)" -ForegroundColor Gray
} else {
    Write-Host "Latest: NO DEPLOYMENT" -ForegroundColor DarkGray
}

Write-Host ""