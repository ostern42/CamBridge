# Uninstall-CamBridge.ps1
# Version: 0.5.29
# © 2025 Claude's Improbably Reliable Software Solutions
# Clean Uninstallation Script for CamBridge Service

param(
    [string]$InstallPath = "C:\Program Files\CamBridge",
    [string]$DataPath = "C:\ProgramData\CamBridge",
    [string]$ServiceName = "CamBridgeService",
    [switch]$KeepData,
    [switch]$Force
)

# Require Administrator
if (-NOT ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator")) {
    Write-Error "This script requires Administrator privileges. Please run as Administrator."
    exit 1
}

Write-Host "================================================" -ForegroundColor Red
Write-Host "  CamBridge Service Uninstallation" -ForegroundColor Red
Write-Host "================================================" -ForegroundColor Red
Write-Host ""

# Confirmation
if (-not $Force) {
    Write-Host "This will uninstall CamBridge Service from your system." -ForegroundColor Yellow
    if ($KeepData) {
        Write-Host "Data and logs will be preserved." -ForegroundColor Yellow
    } else {
        Write-Host "WARNING: All data and logs will be deleted!" -ForegroundColor Red
    }
    Write-Host ""
    $response = Read-Host "Are you sure you want to continue? (YES/NO)"
    if ($response -ne 'YES') {
        Write-Host "Uninstallation cancelled." -ForegroundColor Yellow
        exit 0
    }
}

# Stop service
$service = Get-Service -Name $ServiceName -ErrorAction SilentlyContinue
if ($service) {
    if ($service.Status -eq 'Running') {
        Write-Host "Stopping service..." -ForegroundColor Yellow
        Stop-Service -Name $ServiceName -Force
        Start-Sleep -Seconds 3
    }
    
    # Remove service
    Write-Host "Removing Windows Service..." -ForegroundColor Yellow
    sc.exe delete $ServiceName | Out-Null
    Start-Sleep -Seconds 2
    Write-Host "  - Service removed" -ForegroundColor Green
} else {
    Write-Host "Service not found." -ForegroundColor Gray
}

# Kill any remaining Config UI processes
Write-Host "Closing Configuration UI..." -ForegroundColor Yellow
Get-Process -Name "CamBridge.Config" -ErrorAction SilentlyContinue | Stop-Process -Force

# Remove installation files
if (Test-Path $InstallPath) {
    Write-Host "Removing application files..." -ForegroundColor Yellow
    try {
        Remove-Item -Path $InstallPath -Recurse -Force
        Write-Host "  - Application files removed" -ForegroundColor Green
    } catch {
        Write-Warning "Could not remove all files from $InstallPath. Some files may be in use."
    }
}

# Remove data (if not keeping)
if (-not $KeepData) {
    if (Test-Path $DataPath) {
        Write-Host "Removing data and logs..." -ForegroundColor Yellow
        try {
            Remove-Item -Path $DataPath -Recurse -Force
            Write-Host "  - Data removed" -ForegroundColor Green
        } catch {
            Write-Warning "Could not remove all data from $DataPath"
        }
    }
    
    # Remove default folders if empty
    $defaultFolders = @(
        "C:\CamBridge\Output",
        "C:\CamBridge\Input",
        "C:\CamBridge\Archive",
        "C:\CamBridge"
    )
    foreach ($folder in $defaultFolders) {
        if (Test-Path $folder) {
            $items = Get-ChildItem -Path $folder -Force
            if ($items.Count -eq 0) {
                Remove-Item -Path $folder -Force
                Write-Host "  - Removed empty folder: $folder" -ForegroundColor Gray
            }
        }
    }
} else {
    Write-Host "Data and logs preserved at: $DataPath" -ForegroundColor Cyan
}

# Remove shortcuts
Write-Host "Removing shortcuts..." -ForegroundColor Yellow

# Desktop shortcut
$desktopShortcut = Join-Path ([Environment]::GetFolderPath("Desktop")) "CamBridge Configuration.lnk"
if (Test-Path $desktopShortcut) {
    Remove-Item $desktopShortcut -Force
    Write-Host "  - Desktop shortcut removed" -ForegroundColor Gray
}

# Start menu shortcuts
$startMenuPath = "$env:ProgramData\Microsoft\Windows\Start Menu\Programs\CamBridge"
if (Test-Path $startMenuPath) {
    Remove-Item $startMenuPath -Recurse -Force
    Write-Host "  - Start menu shortcuts removed" -ForegroundColor Gray
}

# Clean registry (if any)
Write-Host "Cleaning registry..." -ForegroundColor Yellow
Remove-ItemProperty -Path "HKLM:\SYSTEM\CurrentControlSet\Services\EventLog\Application" -Name $ServiceName -ErrorAction SilentlyContinue

Write-Host ""
Write-Host "================================================" -ForegroundColor Green
Write-Host "  Uninstallation completed!" -ForegroundColor Green
Write-Host "================================================" -ForegroundColor Green

if ($KeepData) {
    Write-Host ""
    Write-Host "Your data has been preserved at:" -ForegroundColor Cyan
    Write-Host "  $DataPath" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "To completely remove all data, run:" -ForegroundColor Yellow
    Write-Host "  Remove-Item '$DataPath' -Recurse -Force" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "Press any key to exit..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")