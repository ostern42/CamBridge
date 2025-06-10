# 7-clean.ps1
# Clean everything for fresh start
param([switch]$Force)

if (-not $Force) {
    Write-Host "WARNING: This will clean all build outputs and deployments!" -ForegroundColor Yellow
    $confirm = Read-Host "Are you sure? (Y/N)"
    if ($confirm -ne "Y") {
        Write-Host "Cancelled." -ForegroundColor Gray
        exit
    }
}

Write-Host "Cleaning build directories..." -ForegroundColor Yellow
Get-ChildItem -Path "src","tests" -Include "bin","obj" -Recurse -Directory | ForEach-Object {
    Write-Host "  Removing: $($_.FullName)" -ForegroundColor Gray
    Remove-Item $_.FullName -Recurse -Force -ErrorAction SilentlyContinue
}

Write-Host "Cleaning deployment directory..." -ForegroundColor Yellow
if (Test-Path "Deploy") {
    Remove-Item "Deploy" -Recurse -Force
}

Write-Host "Clean complete!" -ForegroundColor Green
