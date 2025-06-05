# Install-CamBridge.ps1
# Version: 0.5.29
# © 2025 Claude's Improbably Reliable Software Solutions
# Professional Installation Script for CamBridge Service

param(
    [string]$InstallPath = "C:\Program Files\CamBridge",
    [string]$DataPath = "C:\ProgramData\CamBridge",
    [string]$ServiceName = "CamBridgeService",
    [string]$ServiceDisplayName = "CamBridge Image Processing Service",
    [string]$ServiceDescription = "Converts JPEG images from Ricoh cameras to DICOM format with QR code metadata extraction"
)

# Require Administrator
if (-NOT ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator")) {
    Write-Error "This script requires Administrator privileges. Please run as Administrator."
    exit 1
}

Write-Host "================================================" -ForegroundColor Cyan
Write-Host "  CamBridge Service Installation v0.5.29" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""

# Check if service already exists
$existingService = Get-Service -Name $ServiceName -ErrorAction SilentlyContinue
if ($existingService) {
    Write-Host "Service '$ServiceName' already exists." -ForegroundColor Yellow
    $response = Read-Host "Do you want to reinstall? (Y/N)"
    if ($response -ne 'Y') {
        Write-Host "Installation cancelled." -ForegroundColor Yellow
        exit 0
    }
    
    # Stop and remove existing service
    Write-Host "Stopping existing service..." -ForegroundColor Yellow
    Stop-Service -Name $ServiceName -Force -ErrorAction SilentlyContinue
    Start-Sleep -Seconds 2
    
    Write-Host "Removing existing service..." -ForegroundColor Yellow
    sc.exe delete $ServiceName | Out-Null
    Start-Sleep -Seconds 2
}

# Create installation directory
Write-Host "Creating installation directory..." -ForegroundColor Green
New-Item -Path $InstallPath -ItemType Directory -Force | Out-Null

# Create data directories
Write-Host "Creating data directories..." -ForegroundColor Green
$directories = @(
    $DataPath,
    "$DataPath\Logs",
    "$DataPath\Config",
    "$DataPath\DeadLetters"
)
foreach ($dir in $directories) {
    New-Item -Path $dir -ItemType Directory -Force | Out-Null
}

# Copy files
Write-Host "Copying application files..." -ForegroundColor Green
$sourceDir = Split-Path -Parent $PSScriptRoot

# Core files
$filesToCopy = @(
    "CamBridge.Service.exe",
    "CamBridge.Service.dll",
    "CamBridge.Service.deps.json",
    "CamBridge.Service.runtimeconfig.json",
    "CamBridge.Core.dll",
    "CamBridge.Infrastructure.dll",
    "appsettings.json",
    "mappings.json"
)

foreach ($file in $filesToCopy) {
    $sourcePath = Join-Path $sourceDir $file
    if (Test-Path $sourcePath) {
        Copy-Item $sourcePath -Destination $InstallPath -Force
        Write-Host "  - Copied: $file" -ForegroundColor Gray
    }
}

# Copy all DLL dependencies
Write-Host "Copying dependencies..." -ForegroundColor Green
Get-ChildItem -Path $sourceDir -Filter "*.dll" | ForEach-Object {
    Copy-Item $_.FullName -Destination $InstallPath -Force
}

# Copy Tools directory (ExifTool)
Write-Host "Copying Tools..." -ForegroundColor Green
$toolsSource = Join-Path $sourceDir "Tools"
$toolsTarget = Join-Path $InstallPath "Tools"
if (Test-Path $toolsSource) {
    Copy-Item $toolsSource -Destination $InstallPath -Recurse -Force
    Write-Host "  - Copied: ExifTool and dependencies" -ForegroundColor Gray
}

# Update configuration paths
Write-Host "Updating configuration..." -ForegroundColor Green
$configPath = Join-Path $InstallPath "appsettings.json"
if (Test-Path $configPath) {
    $config = Get-Content $configPath -Raw | ConvertFrom-Json
    
    # Update default paths
    if ($config.CamBridge.DefaultOutputFolder -eq "C:\CamBridge\Test\Output") {
        $config.CamBridge.DefaultOutputFolder = "C:\CamBridge\Output"
    }
    
    # Update log path
    if ($config.Logging.File.Path -like "*logs/cambridge-*") {
        $config.Logging.File.Path = "$DataPath\Logs\service-.log"
    }
    
    $config | ConvertTo-Json -Depth 10 | Set-Content $configPath -Encoding UTF8
}

# Create default folders if needed
Write-Host "Creating default folders..." -ForegroundColor Green
$defaultFolders = @(
    "C:\CamBridge\Input",
    "C:\CamBridge\Output",
    "C:\CamBridge\Archive"
)
foreach ($folder in $defaultFolders) {
    if (-not (Test-Path $folder)) {
        New-Item -Path $folder -ItemType Directory -Force | Out-Null
        Write-Host "  - Created: $folder" -ForegroundColor Gray
    }
}

# Install service
Write-Host "Installing Windows Service..." -ForegroundColor Green
$servicePath = Join-Path $InstallPath "CamBridge.Service.exe"
$result = sc.exe create $ServiceName binPath= "$servicePath" start= auto DisplayName= "$ServiceDisplayName"

if ($LASTEXITCODE -eq 0) {
    Write-Host "  - Service installed successfully" -ForegroundColor Green
    
    # Set service description
    sc.exe description $ServiceName "$ServiceDescription" | Out-Null
    
    # Configure service recovery options
    sc.exe failure $ServiceName reset= 86400 actions= restart/60000/restart/60000/restart/60000 | Out-Null
    Write-Host "  - Recovery options configured" -ForegroundColor Gray
} else {
    Write-Error "Failed to install service: $result"
    exit 1
}

# Create shortcuts
Write-Host "Creating shortcuts..." -ForegroundColor Green
$WshShell = New-Object -ComObject WScript.Shell

# Desktop shortcut for Config UI
$configExe = Get-ChildItem -Path $sourceDir -Filter "CamBridge.Config.exe" -Recurse | Select-Object -First 1
if ($configExe) {
    $desktopPath = [Environment]::GetFolderPath("Desktop")
    $shortcut = $WshShell.CreateShortcut("$desktopPath\CamBridge Configuration.lnk")
    $shortcut.TargetPath = $configExe.FullName
    $shortcut.WorkingDirectory = $configExe.DirectoryName
    $shortcut.IconLocation = $configExe.FullName
    $shortcut.Description = "CamBridge Configuration UI"
    $shortcut.Save()
    Write-Host "  - Desktop shortcut created" -ForegroundColor Gray
}

# Start menu shortcut
$startMenuPath = "$env:ProgramData\Microsoft\Windows\Start Menu\Programs\CamBridge"
New-Item -Path $startMenuPath -ItemType Directory -Force | Out-Null
if ($configExe) {
    $shortcut = $WshShell.CreateShortcut("$startMenuPath\CamBridge Configuration.lnk")
    $shortcut.TargetPath = $configExe.FullName
    $shortcut.WorkingDirectory = $configExe.DirectoryName
    $shortcut.IconLocation = $configExe.FullName
    $shortcut.Description = "CamBridge Configuration UI"
    $shortcut.Save()
}

# Log folder shortcut
$shortcut = $WshShell.CreateShortcut("$startMenuPath\CamBridge Logs.lnk")
$shortcut.TargetPath = "$DataPath\Logs"
$shortcut.Save()

Write-Host ""
Write-Host "================================================" -ForegroundColor Green
Write-Host "  Installation completed successfully!" -ForegroundColor Green
Write-Host "================================================" -ForegroundColor Green
Write-Host ""
Write-Host "Service Name: $ServiceName" -ForegroundColor Cyan
Write-Host "Install Path: $InstallPath" -ForegroundColor Cyan
Write-Host "Data Path: $DataPath" -ForegroundColor Cyan
Write-Host ""

# Ask to start service
$response = Read-Host "Do you want to start the service now? (Y/N)"
if ($response -eq 'Y') {
    Write-Host "Starting service..." -ForegroundColor Green
    Start-Service -Name $ServiceName
    Start-Sleep -Seconds 2
    
    $service = Get-Service -Name $ServiceName
    if ($service.Status -eq 'Running') {
        Write-Host "Service started successfully!" -ForegroundColor Green
        
        # Launch Config UI
        if ($configExe) {
            Write-Host "Launching Configuration UI..." -ForegroundColor Green
            Start-Process $configExe.FullName
        }
    } else {
        Write-Warning "Service failed to start. Check Event Log for details."
    }
}

Write-Host ""
Write-Host "Installation log saved to: $DataPath\Logs\installation.log" -ForegroundColor Gray
Write-Host "Press any key to exit..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")