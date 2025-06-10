# Create-NumberedTools-Fixed.ps1
# Creates all numbered testing tools at once (FIXED VERSION)
# Version: 0.7.4

Write-Host "Creating CamBridge Numbered Testing Tools..." -ForegroundColor Cyan
Write-Host ""

# 0-build.ps1
@'
# 0-build.ps1
# Build and create deployment package (NO ZIP - faster!)
param([switch]$Quick)

Write-Host "Building CamBridge Release (No ZIP)..." -ForegroundColor Cyan
if ($Quick) {
    & .\Create-DeploymentPackage.ps1 -SkipClean -SkipZip
} else {
    & .\Create-DeploymentPackage.ps1 -SkipZip
}
'@ | Set-Content "0-build.ps1"
Write-Host "Created 0-build.ps1" -ForegroundColor Green

# 00-build-zip.ps1
@'
# 00-build-zip.ps1
# Build and create deployment package WITH ZIP
param([switch]$Quick)

Write-Host "Building CamBridge Release (With ZIP)..." -ForegroundColor Cyan
if ($Quick) {
    & .\Create-DeploymentPackage.ps1 -SkipClean
} else {
    & .\Create-DeploymentPackage.ps1
}
'@ | Set-Content "00-build-zip.ps1"
Write-Host "Created 00-build-zip.ps1" -ForegroundColor Green

# 1-deploy-update.ps1
@'
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
'@ | Set-Content "1-deploy-update.ps1"
Write-Host "Created 1-deploy-update.ps1" -ForegroundColor Green

# 2-config.ps1
@'
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
'@ | Set-Content "2-config.ps1"
Write-Host "Created 2-config.ps1" -ForegroundColor Green

# 3-service.ps1
@'
# 3-service.ps1
# Service manager (existing service-manager.ps1)
Write-Host "Starting Service Manager..." -ForegroundColor Cyan
& .\service-manager.ps1
'@ | Set-Content "3-service.ps1"
Write-Host "Created 3-service.ps1" -ForegroundColor Green

# 4-console.ps1
@'
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
'@ | Set-Content "4-console.ps1"
Write-Host "Created 4-console.ps1" -ForegroundColor Green

# 5-test-api.ps1
@'
# 5-test-api.ps1
# API testing (redirect to existing)
& .\test-api.ps1 $args
'@ | Set-Content "5-test-api.ps1"
Write-Host "Created 5-test-api.ps1" -ForegroundColor Green

# 6-logs.ps1
@'
# 6-logs.ps1
# Quick log viewer
$logPaths = @(
    "C:\ProgramData\CamBridge\Logs",
    ".\Deploy\CamBridge-Deploy-v*\Service\Logs"
)

foreach ($path in $logPaths) {
    $resolved = Resolve-Path $path -ErrorAction SilentlyContinue
    if ($resolved) {
        Write-Host "Opening logs: $resolved" -ForegroundColor Cyan
        Start-Process explorer.exe $resolved
        exit
    }
}

Write-Host "ERROR: No log folder found!" -ForegroundColor Red
'@ | Set-Content "6-logs.ps1"
Write-Host "Created 6-logs.ps1" -ForegroundColor Green

# 7-clean.ps1
@'
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
'@ | Set-Content "7-clean.ps1"
Write-Host "Created 7-clean.ps1" -ForegroundColor Green

# 8-status.ps1
@'
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
'@ | Set-Content "8-status.ps1"
Write-Host "Created 8-status.ps1" -ForegroundColor Green

# 9-testit.ps1
@'
# 9-testit.ps1
# The ultimate test shortcut: Update service + start config
Write-Host "Quick Test: Update + Config" -ForegroundColor Cyan
& .\1-deploy-update.ps1 -StartConfig -SkipBuild
'@ | Set-Content "9-testit.ps1"
Write-Host "Created 9-testit.ps1" -ForegroundColor Green

# 99-testit-full.ps1
@'
# 99-testit-full.ps1
# The FULL test cycle: Build + Update service + start config
Write-Host "Full Test Cycle: Build + Update + Config" -ForegroundColor Cyan
Write-Host ""
& .\1-deploy-update.ps1 -StartConfig
'@ | Set-Content "99-testit-full.ps1"
Write-Host "Created 99-testit-full.ps1" -ForegroundColor Green

# h-help.ps1
@'
# h-help.ps1
# Quick help for numbered tools

Clear-Host
Write-Host "================================================" -ForegroundColor Cyan
Write-Host "    CamBridge Numbered Tools - Quick Help" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Just type number + TAB to use!" -ForegroundColor Yellow
Write-Host ""
Write-Host "  0  - Build (no ZIP)                 " -NoNewline; Write-Host "0-build [-Quick]" -ForegroundColor DarkGray
Write-Host " 00  - Build WITH ZIP                " -NoNewline; Write-Host "00-build-zip [-Quick]" -ForegroundColor DarkGray
Write-Host "  1  - Update service cycle           " -NoNewline; Write-Host "1-deploy-update [-StartConfig]" -ForegroundColor DarkGray
Write-Host "  2  - Start Config UI                " -NoNewline; Write-Host "2-config" -ForegroundColor DarkGray
Write-Host "  3  - Service Manager menu           " -NoNewline; Write-Host "3-service" -ForegroundColor DarkGray
Write-Host "  4  - Console mode (debug)           " -NoNewline; Write-Host "4-console" -ForegroundColor DarkGray
Write-Host "  5  - API testing                    " -NoNewline; Write-Host "5-test-api [-Continuous]" -ForegroundColor DarkGray
Write-Host "  6  - Open log folder                " -NoNewline; Write-Host "6-logs" -ForegroundColor DarkGray
Write-Host "  7  - Clean everything               " -NoNewline; Write-Host "7-clean [-Force]" -ForegroundColor DarkGray
Write-Host "  8  - Quick status check             " -NoNewline; Write-Host "8-status" -ForegroundColor DarkGray
Write-Host "  9  - Test (no build)                " -NoNewline; Write-Host "9-testit" -ForegroundColor DarkGray
Write-Host " 99  - FULL test (with build)         " -NoNewline; Write-Host "99-testit-full" -ForegroundColor DarkGray
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
Write-Host "Location: C:\Users\oliver.stern\source\repos\CamBridge" -ForegroundColor DarkGray
Write-Host ""
'@ | Set-Content "h-help.ps1"
Write-Host "Created h-help.ps1" -ForegroundColor Green

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  All numbered tools created!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Try it now:" -ForegroundColor Yellow
Write-Host "  0[TAB]    = Build (no ZIP - fast!)" -ForegroundColor Gray
Write-Host "  00[TAB]   = Build with ZIP" -ForegroundColor Gray
Write-Host "  9[TAB]    = Quick Test (no build)" -ForegroundColor Gray
Write-Host "  99[TAB]   = Full Test (with build)" -ForegroundColor Gray
Write-Host "  h[TAB]    = Help" -ForegroundColor Gray
Write-Host ""