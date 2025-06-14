# 2-config.ps1
# Start Configuration UI from latest deployment
# FIXED: Proper version sorting + correct ConfigUI path

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

if (-not $latestDeploy) {
    Write-Host "ERROR: No deployment found!" -ForegroundColor Red
    exit 1
}

# FIX: ConfigUI not Config!
$configExe = Join-Path $latestDeploy.FullName "Config\CamBridge.Config.exe"
if (Test-Path $configExe) {
    Write-Host "Starting Config UI from: $($latestDeploy.Name)" -ForegroundColor Cyan
    Start-Process $configExe
} else {
    Write-Host "ERROR: Config UI not found!" -ForegroundColor Red
    Write-Host "Expected at: $configExe" -ForegroundColor Gray
}