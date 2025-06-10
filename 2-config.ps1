# 2-config.ps1
# Start Configuration UI from latest deployment
$latestDeploy = Get-ChildItem -Path ".\Deploy" -Filter "CamBridge-Deploy-v*" -Directory | 
                Sort-Object Name -Descending | 
                Select-Object -First 1

if (-not $latestDeploy) {
    Write-Host "ERROR: No deployment found!" -ForegroundColor Red
    exit 1
}

$configExe = Join-Path $latestDeploy.FullName "Config\CamBridge.Config.exe"
if (Test-Path $configExe) {
    Write-Host "Starting Config UI from: $($latestDeploy.Name)" -ForegroundColor Cyan
    Start-Process $configExe
} else {
    Write-Host "ERROR: Config UI not found!" -ForegroundColor Red
}
