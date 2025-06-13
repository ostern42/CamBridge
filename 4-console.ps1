# 4-console.ps1
# Quick console mode for debugging
# FIXED: Proper version sorting (v0.7.10 > v0.7.9)

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