# service-manager.ps1
# Service Control Menu for CamBridge Testing
# Works with deployed Release version
# Version: 0.7.5 (FIXED SYNTAX)
# © 2025 Claude's Improbably Reliable Software Solutions

param(
    [string]$DeployPath = ".\Deploy"
)

# Find latest deployment
$latestDeploy = Get-ChildItem -Path $DeployPath -Filter "CamBridge-Deploy-v*" -Directory | 
                Sort-Object Name -Descending | 
                Select-Object -First 1

if (-not $latestDeploy) {
    Write-Host "ERROR: No deployment found in $DeployPath" -ForegroundColor Red
    Write-Host "Please run Create-DeploymentPackage.ps1 first!" -ForegroundColor Yellow
    Read-Host "Press Enter to exit"
    exit 1
}

$serviceDir = Join-Path $latestDeploy.FullName "Service"
$serviceExe = Join-Path $serviceDir "CamBridge.Service.exe"
$version = $latestDeploy.Name -replace "CamBridge-Deploy-v", ""

# Check if running as admin
$isAdmin = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator")

function Show-Menu {
    Clear-Host
    Write-Host "================================================" -ForegroundColor Cyan
    Write-Host "        CamBridge Service Manager v$version" -ForegroundColor Cyan
    Write-Host "================================================" -ForegroundColor Cyan
    Write-Host ""
    
    if (-not $isAdmin) {
        Write-Host "WARNING: NOT RUNNING AS ADMINISTRATOR" -ForegroundColor Yellow
        Write-Host "   Some functions require admin rights" -ForegroundColor Gray
        Write-Host ""
    }
    
    # Get service status
    $service = Get-Service "CamBridgeService" -ErrorAction SilentlyContinue
    if ($service) {
        $status = $service.Status
        $startType = $service.StartType
        
        switch ($status) {
            "Running" { 
                Write-Host "Service Status: " -NoNewline
                Write-Host "RUNNING" -ForegroundColor Green 
            }
            "Stopped" { 
                Write-Host "Service Status: " -NoNewline
                Write-Host "STOPPED" -ForegroundColor Red 
            }
            default { 
                Write-Host "Service Status: " -NoNewline
                Write-Host "$status" -ForegroundColor Yellow 
            }
        }
        Write-Host "Startup Type:   $startType" -ForegroundColor Gray
    } else {
        Write-Host "Service Status: " -NoNewline
        Write-Host "NOT INSTALLED" -ForegroundColor DarkGray
    }
    
    Write-Host ""
    Write-Host "Using: $($latestDeploy.FullName)" -ForegroundColor DarkGray
    Write-Host ""
    Write-Host "------------------------------------------------" -ForegroundColor Gray
    Write-Host ""
    
    # Menu options
    if ($service) {
        if ($service.Status -eq "Running") {
            Write-Host "  [1] Stop Service" -ForegroundColor White
            Write-Host "  [2] Restart Service" -ForegroundColor White
        } else {
            Write-Host "  [1] Start Service" -ForegroundColor White
            Write-Host "  [2] Start in Console Mode (Debug)" -ForegroundColor White
        }
        if (-not $isAdmin) {
            Write-Host "  [3] Uninstall Service (Admin required)" -ForegroundColor Gray
        } else {
            Write-Host "  [3] Uninstall Service" -ForegroundColor White
        }
    } else {
        if (-not $isAdmin) {
            Write-Host "  [1] Install Service (Admin required)" -ForegroundColor Gray
        } else {
            Write-Host "  [1] Install Service" -ForegroundColor White
        }
        Write-Host "  [2] Run in Console Mode (Debug)" -ForegroundColor White
    }
    
    Write-Host ""
    Write-Host "  [4] View Service Logs" -ForegroundColor White
    Write-Host "  [5] Open Service Folder" -ForegroundColor White
    Write-Host "  [6] Check ExifTool" -ForegroundColor White
    Write-Host ""
    Write-Host "  [R] Refresh Status" -ForegroundColor Gray
    Write-Host "  [A] Restart as Admin" -ForegroundColor Gray
    Write-Host "  [X] Exit" -ForegroundColor Gray
    Write-Host ""
}

function Install-CamBridgeService {
    if (-not $isAdmin) {
        Write-Host "ERROR: Administrator rights required!" -ForegroundColor Red
        Read-Host "Press Enter to continue"
        return
    }
    
    Write-Host "Installing CamBridge Service..." -ForegroundColor Yellow
    $result = & sc.exe create "CamBridgeService" `
        binPath= "$serviceExe" `
        DisplayName= "CamBridge DICOM Converter" `
        start= "auto" `
        obj= "LocalSystem"
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "Service installed successfully!" -ForegroundColor Green
        
        # Set description
        & sc.exe description "CamBridgeService" "Converts JPEG images from Ricoh cameras to DICOM format with multi-pipeline support"
        
        # Set recovery options
        & sc.exe failure "CamBridgeService" reset= 86400 actions= restart/60000/restart/60000/restart/60000
    } else {
        Write-Host "Failed to install service: $result" -ForegroundColor Red
    }
    
    Read-Host "Press Enter to continue"
}

function Uninstall-CamBridgeService {
    if (-not $isAdmin) {
        Write-Host "ERROR: Administrator rights required!" -ForegroundColor Red
        Read-Host "Press Enter to continue"
        return
    }
    
    # Stop service first if running
    $service = Get-Service "CamBridgeService" -ErrorAction SilentlyContinue
    if ($service -and $service.Status -eq "Running") {
        Write-Host "Stopping service..." -ForegroundColor Yellow
        Stop-Service "CamBridgeService" -Force
        Start-Sleep -Seconds 2
    }
    
    Write-Host "Uninstalling CamBridge Service..." -ForegroundColor Yellow
    $result = & sc.exe delete "CamBridgeService"
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "Service uninstalled successfully!" -ForegroundColor Green
    } else {
        Write-Host "Failed to uninstall service: $result" -ForegroundColor Red
    }
    
    Read-Host "Press Enter to continue"
}

function Start-CamBridgeService {
    Write-Host "Starting CamBridge Service..." -ForegroundColor Yellow
    try {
        Start-Service "CamBridgeService" -ErrorAction Stop
        Write-Host "Service started successfully!" -ForegroundColor Green
    } catch {
        Write-Host "Failed to start service: $_" -ForegroundColor Red
    }
    Read-Host "Press Enter to continue"
}

function Stop-CamBridgeService {
    Write-Host "Stopping CamBridge Service..." -ForegroundColor Yellow
    try {
        Stop-Service "CamBridgeService" -ErrorAction Stop
        Write-Host "Service stopped successfully!" -ForegroundColor Green
    } catch {
        Write-Host "Failed to stop service: $_" -ForegroundColor Red
    }
    Read-Host "Press Enter to continue"
}

function Restart-CamBridgeService {
    Write-Host "Restarting CamBridge Service..." -ForegroundColor Yellow
    try {
        Restart-Service "CamBridgeService" -ErrorAction Stop
        Write-Host "Service restarted successfully!" -ForegroundColor Green
    } catch {
        Write-Host "Failed to restart service: $_" -ForegroundColor Red
    }
    Read-Host "Press Enter to continue"
}

function Start-ConsoleMode {
    Write-Host "Starting CamBridge in Console Mode..." -ForegroundColor Yellow
    Write-Host "Press Ctrl+C to stop" -ForegroundColor Gray
    Write-Host ""
    
    Push-Location $serviceDir
    try {
        & $serviceExe
    } finally {
        Pop-Location
    }
    
    Write-Host ""
    Read-Host "Press Enter to continue"
}

function View-Logs {
    $logPath = "C:\ProgramData\CamBridge\Logs"
    if (Test-Path $logPath) {
        Write-Host "Opening log folder..." -ForegroundColor Green
        Start-Process explorer.exe $logPath
    } else {
        Write-Host "Log folder not found at: $logPath" -ForegroundColor Yellow
        
        # Try to find logs in service directory
        $altLogPath = Join-Path $serviceDir "Logs"
        if (Test-Path $altLogPath) {
            Write-Host "Opening service log folder..." -ForegroundColor Green
            Start-Process explorer.exe $altLogPath
        }
    }
}

function Check-ExifTool {
    Write-Host "Checking ExifTool installation..." -ForegroundColor Yellow
    Write-Host ""
    
    $exifToolPath = Join-Path $serviceDir "Tools\exiftool.exe"
    if (Test-Path $exifToolPath) {
        Write-Host "ExifTool found at:" -ForegroundColor Green
        Write-Host "  $exifToolPath" -ForegroundColor Gray
        
        # Get version
        try {
            $version = & $exifToolPath -ver 2>$null
            Write-Host "  Version: $version" -ForegroundColor Gray
        } catch {
            Write-Host "  Could not determine version" -ForegroundColor Yellow
        }
    } else {
        Write-Host "ExifTool NOT FOUND!" -ForegroundColor Red
        Write-Host "  Expected at: $exifToolPath" -ForegroundColor Gray
        Write-Host ""
        Write-Host "  The service will NOT work without ExifTool!" -ForegroundColor Yellow
    }
    
    Write-Host ""
    Read-Host "Press Enter to continue"
}

# Main loop
do {
    Show-Menu
    $choice = Read-Host "Select option"
    
    $service = Get-Service "CamBridgeService" -ErrorAction SilentlyContinue
    
    switch ($choice.ToUpper()) {
        "1" {
            if ($service) {
                if ($service.Status -eq "Running") {
                    Stop-CamBridgeService
                } else {
                    Start-CamBridgeService
                }
            } else {
                Install-CamBridgeService
            }
        }
        "2" {
            if ($service) {
                if ($service.Status -eq "Running") {
                    Restart-CamBridgeService
                } else {
                    Start-ConsoleMode
                }
            } else {
                Start-ConsoleMode
            }
        }
        "3" {
            if ($service) {
                Uninstall-CamBridgeService
            }
        }
        "4" { View-Logs }
        "5" { Start-Process explorer.exe $serviceDir }
        "6" { Check-ExifTool }
        "R" { Continue }
        "A" {
            if (-not $isAdmin) {
                Write-Host "Restarting as Administrator..." -ForegroundColor Yellow
                Start-Process powershell -ArgumentList "-ExecutionPolicy Bypass -File `"$PSCommandPath`" -DeployPath `"$DeployPath`"" -Verb RunAs
                exit
            } else {
                Write-Host "Already running as Administrator!" -ForegroundColor Yellow
                Read-Host "Press Enter to continue"
            }
        }
        "X" { 
            Write-Host "Goodbye!" -ForegroundColor Green
            break 
        }
        default {
            Write-Host "Invalid option!" -ForegroundColor Red
            Read-Host "Press Enter to continue"
        }
    }
} while ($choice.ToUpper() -ne "X")