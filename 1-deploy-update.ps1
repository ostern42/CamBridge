# 1-deploy-update.ps1
# Complete service update cycle: Stop -> Uninstall -> Deploy -> Install -> Start
param(
    [switch]$StartConfig,
    [switch]$SkipBuild
)

Write-Host "CamBridge Service Update Cycle" -ForegroundColor Cyan
Write-Host ""

# Step 0: Build if not skipped
if (-not $SkipBuild) {
    Write-Host "Building deployment package..." -ForegroundColor Yellow
    & .\0-build.ps1 -Quick
    Write-Host ""
}

# Check if running as admin
$isAdmin = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator")
if (-not $isAdmin) {
    Write-Host "ERROR: This script requires Administrator privileges!" -ForegroundColor Red
    Write-Host "Restarting as Administrator..." -ForegroundColor Yellow
    Start-Process powershell -ArgumentList "-ExecutionPolicy Bypass -File `"$PSCommandPath`" $(if($StartConfig){'-StartConfig'}) $(if($SkipBuild){'-SkipBuild'})" -Verb RunAs
    exit
}

# Step 1: Stop service if running
$service = Get-Service "CamBridgeService" -ErrorAction SilentlyContinue
if ($service) {
    if ($service.Status -eq "Running") {
        Write-Host "Stopping service..." -ForegroundColor Yellow
        Stop-Service "CamBridgeService" -Force
        Start-Sleep -Seconds 2
    }
    
    # Step 2: Uninstall old service
    Write-Host "Uninstalling old service..." -ForegroundColor Yellow
    & sc.exe delete "CamBridgeService" | Out-Null
    Start-Sleep -Seconds 2
}

# Step 3: Find latest deployment
$latestDeploy = Get-ChildItem -Path ".\Deploy" -Filter "CamBridge-Deploy-v*" -Directory | 
                Sort-Object Name -Descending | 
                Select-Object -First 1

if (-not $latestDeploy) {
    Write-Host "ERROR: No deployment found!" -ForegroundColor Red
    exit 1
}

$serviceExe = Join-Path $latestDeploy.FullName "Service\CamBridge.Service.exe"

# Step 4: Install new service
Write-Host "Installing service from: $($latestDeploy.Name)" -ForegroundColor Yellow
& sc.exe create "CamBridgeService" `
    binPath= "$serviceExe" `
    DisplayName= "CamBridge DICOM Converter" `
    start= "auto" `
    obj= "LocalSystem" | Out-Null

& sc.exe description "CamBridgeService" "Converts JPEG images from Ricoh cameras to DICOM format" | Out-Null
& sc.exe failure "CamBridgeService" reset= 86400 actions= restart/60000/restart/60000/restart/60000 | Out-Null

# Step 5: Start service
Write-Host "Starting new service..." -ForegroundColor Yellow
Start-Service "CamBridgeService"
Start-Sleep -Seconds 2

# Verify
$newService = Get-Service "CamBridgeService" -ErrorAction SilentlyContinue
if ($newService -and $newService.Status -eq "Running") {
    Write-Host "Service updated and running!" -ForegroundColor Green
} else {
    Write-Host "WARNING: Service installed but not running!" -ForegroundColor Yellow
}

# Step 6: Start config if requested
if ($StartConfig) {
    Write-Host ""
    Write-Host "Starting Configuration UI..." -ForegroundColor Cyan
    & .\2-config.ps1
}

Write-Host ""
Write-Host "Update complete!" -ForegroundColor Green
