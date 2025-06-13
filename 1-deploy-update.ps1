# 1-deploy-update.ps1
# Complete service update cycle: Stop -> Uninstall -> Deploy -> Install -> Start
# FIXED: Proper version sorting + intelligent build check
param(
    [switch]$StartConfig,
    [switch]$SkipBuild,
    [switch]$ForceBuild
)

Write-Host "CamBridge Service Update Cycle" -ForegroundColor Cyan
Write-Host ""

# Get current version
$version = "?.?.?"
if (Test-Path "Version.props") {
    $content = Get-Content "Version.props" -Raw
    if ($content -match '<VersionPrefix>([^<]+)</VersionPrefix>') {
        $version = $matches[1]
    }
}

$expectedDeploy = ".\Deploy\CamBridge-Deploy-v$version"

# Step 0: Build if needed (smart check!)
if (-not $SkipBuild) {
    if (-not (Test-Path $expectedDeploy) -or $ForceBuild) {
        Write-Host "Building deployment package v$version..." -ForegroundColor Yellow
        & .\0-build.ps1 -Quick
        Write-Host ""
    } else {
        Write-Host "Using existing build: CamBridge-Deploy-v$version" -ForegroundColor Green
        Write-Host "Built: $(Get-Item $expectedDeploy | Select-Object -ExpandProperty LastWriteTime)" -ForegroundColor Gray
        Write-Host "(Use -ForceBuild to rebuild)" -ForegroundColor DarkGray
        Write-Host ""
    }
}

# Check if running as admin
$isAdmin = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator")
if (-not $isAdmin) {
    Write-Host "ERROR: This script requires Administrator privileges!" -ForegroundColor Red
    Write-Host "Restarting as Administrator..." -ForegroundColor Yellow
    $args = @()
    if ($StartConfig) { $args += '-StartConfig' }
    if ($SkipBuild) { $args += '-SkipBuild' }
    if ($ForceBuild) { $args += '-ForceBuild' }
    Start-Process powershell -ArgumentList "-ExecutionPolicy Bypass -File `"$PSCommandPath`" $($args -join ' ')" -Verb RunAs
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

# Step 3: Find deployment - prefer expected version
Write-Host "Finding deployment package..." -ForegroundColor Yellow

$latestDeploy = $null

# First, try to use the expected version
if (Test-Path $expectedDeploy) {
    $latestDeploy = Get-Item $expectedDeploy
    Write-Host "Using current version deployment: v$version" -ForegroundColor Green
} else {
    Write-Host "Current version not found, searching for latest..." -ForegroundColor Yellow
    
    # Parse version numbers properly for sorting - FIXED!
    $latestDeploy = Get-ChildItem -Path ".\Deploy" -Filter "CamBridge-Deploy-v*" -Directory | 
                    ForEach-Object {
                        if ($_.Name -match 'v(\d+)\.(\d+)\.(\d+)') {
                            [PSCustomObject]@{
                                Directory = $_
                                Major = [int]$matches[1]
                                Minor = [int]$matches[2]
                                Patch = [int]$matches[3]
                                Version = [Version]::new([int]$matches[1], [int]$matches[2], [int]$matches[3])
                            }
                        }
                    } |
                    Sort-Object Version -Descending |
                    Select-Object -First 1 -ExpandProperty Directory
}

if (-not $latestDeploy) {
    Write-Host "ERROR: No deployment found!" -ForegroundColor Red
    Write-Host "Run '0[TAB]' to build first." -ForegroundColor Yellow
    exit 1
}

Write-Host "Selected deployment: $($latestDeploy.Name)" -ForegroundColor Green

$serviceExe = Join-Path $latestDeploy.FullName "Service\CamBridge.Service.exe"

# Verify the exe exists
if (-not (Test-Path $serviceExe)) {
    Write-Host "ERROR: Service executable not found at: $serviceExe" -ForegroundColor Red
    exit 1
}

# Step 4: Install new service
Write-Host "Installing service from: $($latestDeploy.Name)" -ForegroundColor Yellow
Write-Host "Service path: $serviceExe" -ForegroundColor Gray

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
Start-Sleep -Seconds 3

# Verify
$newService = Get-Service "CamBridgeService" -ErrorAction SilentlyContinue
if ($newService -and $newService.Status -eq "Running") {
    Write-Host "Service updated and running!" -ForegroundColor Green
    
    # Show version info
    try {
        $versionInfo = [System.Diagnostics.FileVersionInfo]::GetVersionInfo($serviceExe)
        Write-Host "Service version: $($versionInfo.FileVersion)" -ForegroundColor Cyan
    } catch {
        Write-Host "Service version: $version (from deployment)" -ForegroundColor Cyan
    }
} else {
    Write-Host "WARNING: Service installed but not running!" -ForegroundColor Yellow
    Write-Host "Check Event Viewer for details." -ForegroundColor Yellow
}

# Step 6: Start config if requested
if ($StartConfig) {
    Write-Host ""
    Write-Host "Starting Configuration UI..." -ForegroundColor Cyan
    & .\2-config.ps1
}

Write-Host ""
Write-Host "Update complete!" -ForegroundColor Green