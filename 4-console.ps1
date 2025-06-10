# 4-console.ps1
# Quick console mode for debugging
$latestDeploy = Get-ChildItem -Path ".\Deploy" -Filter "CamBridge-Deploy-v*" -Directory | 
                Sort-Object Name -Descending | 
                Select-Object -First 1

if (-not $latestDeploy) {
    Write-Host "ERROR: No deployment found!" -ForegroundColor Red
    exit 1
}

$serviceDir = Join-Path $latestDeploy.FullName "Service"
$serviceExe = Join-Path $serviceDir "CamBridge.Service.exe"

Write-Host "Starting Console Mode from: $($latestDeploy.Name)" -ForegroundColor Cyan
Write-Host "Press Ctrl+C to stop" -ForegroundColor Gray
Write-Host ""

Push-Location $serviceDir
try {
    & $serviceExe
} finally {
    Pop-Location
}
