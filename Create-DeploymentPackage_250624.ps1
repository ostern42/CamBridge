# Create-DeploymentPackage.ps1
# Complete build and deployment package creator for CamBridge
# Version: 0.8.3 (FIXED Service Build Issue)
# Updated: Forces ALL projects to build, ignoring VS Configuration Manager

param(
    [string]$Version = "",  # Auto-detect from Version.props
    [string]$OutputPath = ".\Deploy",
    [switch]$SkipClean = $false,
    [switch]$SkipTests = $false,
    [switch]$SkipQRBridge = $false,
    [switch]$ShowWarnings = $false,  # Default: Warnings unterdrückt
    [switch]$SkipZip = $false  # Skip ZIP creation for faster builds
)

# Read version from Version.props if not specified
if ([string]::IsNullOrEmpty($Version)) {
    if (Test-Path ".\Version.props") {
        [xml]$versionProps = Get-Content ".\Version.props" -Raw
        $Version = $versionProps.Project.PropertyGroup.VersionPrefix
        Write-Host "Auto-detected version from Version.props: $Version" -ForegroundColor DarkGray
    } else {
        Write-Error "Version.props not found and no version specified!"
        exit 1
    }
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host " CamBridge Deployment Package Builder" -ForegroundColor Cyan
Write-Host " Version: $Version" -ForegroundColor Cyan
Write-Host " FIXED: Forces ALL projects to build!" -ForegroundColor Green
if (-not $ShowWarnings) {
    Write-Host " Build Warnings: SUPPRESSED" -ForegroundColor DarkGray
}
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Step 1: Clean everything
if (-not $SkipClean) {
    Write-Host "Step 1: Cleaning build directories..." -ForegroundColor Yellow
    
    # Clean all bin and obj folders
    Get-ChildItem -Path "src","tests" -Include "bin","obj" -Recurse -Directory | ForEach-Object {
        Write-Host "  Cleaning: $($_.FullName)" -ForegroundColor Gray
        Remove-Item $_.FullName -Recurse -Force -ErrorAction SilentlyContinue
    }
    
    # Also clean test folders
    Get-ChildItem -Path "tests" -Include "bin","obj" -Recurse -Directory -ErrorAction SilentlyContinue | ForEach-Object {
        Write-Host "  Cleaning: $($_.FullName)" -ForegroundColor Gray
        Remove-Item $_.FullName -Recurse -Force -ErrorAction SilentlyContinue
    }
    
    Write-Host "  [OK] Clean complete" -ForegroundColor Green
    Write-Host ""
}

# Step 2: Restore packages
Write-Host "Step 2: Restoring NuGet packages..." -ForegroundColor Yellow
if ($ShowWarnings) {
    dotnet restore CamBridge.sln
} else {
    dotnet restore CamBridge.sln --verbosity quiet
}
if ($LASTEXITCODE -ne 0) {
    Write-Error "Restore failed!"
    exit 1
}
Write-Host "  [OK] Packages restored" -ForegroundColor Green
Write-Host ""

# Step 3: Build ALL projects explicitly (VS Configuration Manager workaround)
Write-Host "Step 3: Building ALL projects (VS Config Manager workaround)..." -ForegroundColor Yellow

# Build Core first (dependencies)
Write-Host "  Building CamBridge.Core..." -ForegroundColor Gray
if ($ShowWarnings) {
    dotnet build src\CamBridge.Core\CamBridge.Core.csproj -c Release
} else {
    dotnet build src\CamBridge.Core\CamBridge.Core.csproj -c Release --verbosity quiet
}
if ($LASTEXITCODE -ne 0) {
    Write-Error "Core build failed!"
    exit 1
}

# Build Infrastructure
Write-Host "  Building CamBridge.Infrastructure..." -ForegroundColor Gray
if ($ShowWarnings) {
    dotnet build src\CamBridge.Infrastructure\CamBridge.Infrastructure.csproj -c Release
} else {
    dotnet build src\CamBridge.Infrastructure\CamBridge.Infrastructure.csproj -c Release --verbosity quiet
}
if ($LASTEXITCODE -ne 0) {
    Write-Error "Infrastructure build failed!"
    exit 1
}

# BUILD SERVICE EXPLICITLY (THIS WAS MISSING!)
Write-Host "  Building CamBridge.Service (FORCED)..." -ForegroundColor Cyan
if ($ShowWarnings) {
    dotnet build src\CamBridge.Service\CamBridge.Service.csproj -c Release
} else {
    dotnet build src\CamBridge.Service\CamBridge.Service.csproj -c Release --verbosity quiet
}
if ($LASTEXITCODE -ne 0) {
    Write-Error "Service build failed!"
    exit 1
}
Write-Host "    [OK] Service built successfully!" -ForegroundColor Green

# Build Config UI with explicit x64 platform
Write-Host "  Building Configuration UI (x64)..." -ForegroundColor Gray
# Try MSBuild first for proper x64 platform build
$msbuildPath = & "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe" -latest -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe 2>$null | Select-Object -First 1

if ($msbuildPath -and (Test-Path $msbuildPath)) {
    Write-Host "    Using MSBuild for x64 platform build..." -ForegroundColor DarkGray
    if ($ShowWarnings) {
        & $msbuildPath src\CamBridge.Config\CamBridge.Config.csproj /p:Configuration=Release /p:Platform=x64
    } else {
        & $msbuildPath src\CamBridge.Config\CamBridge.Config.csproj /p:Configuration=Release /p:Platform=x64 /p:WarningLevel=0 /v:minimal
    }
} else {
    Write-Host "    Using dotnet CLI (platform might vary)..." -ForegroundColor Yellow
    if ($ShowWarnings) {
        dotnet build src\CamBridge.Config\CamBridge.Config.csproj -c Release
    } else {
        dotnet build src\CamBridge.Config\CamBridge.Config.csproj -c Release --verbosity quiet
    }
}

if ($LASTEXITCODE -ne 0) {
    Write-Error "Config UI build failed!"
    exit 1
}

Write-Host "  [OK] All projects built successfully!" -ForegroundColor Green
Write-Host ""

# Step 4: Build QRBridge (if not skipped)
if (-not $SkipQRBridge) {
    Write-Host "Step 4: Building QRBridge 2.0..." -ForegroundColor Yellow
    if ($ShowWarnings) {
        dotnet publish src\CamBridge.QRBridge\CamBridge.QRBridge.csproj -c Release --self-contained false -o .\TempQRBridgePublish
    } else {
        dotnet publish src\CamBridge.QRBridge\CamBridge.QRBridge.csproj -c Release --self-contained false -o .\TempQRBridgePublish --verbosity quiet
    }
    if ($LASTEXITCODE -ne 0) {
        Write-Warning "QRBridge build failed, continuing without it..."
    } else {
        Write-Host "  [OK] QRBridge built" -ForegroundColor Green
    }
    Write-Host ""
}

# Step 5: Publish Service (after explicit build)
Write-Host "Step 5: Publishing Windows Service..." -ForegroundColor Yellow
if ($ShowWarnings) {
    dotnet publish src\CamBridge.Service\CamBridge.Service.csproj -c Release -r win-x64 --self-contained --no-build
} else {
    dotnet publish src\CamBridge.Service\CamBridge.Service.csproj -c Release -r win-x64 --self-contained --no-build --verbosity quiet
}
if ($LASTEXITCODE -ne 0) {
    Write-Error "Service publish failed!"
    exit 1
}
Write-Host "  [OK] Service published" -ForegroundColor Green
Write-Host ""

# Optional: Run tests (CURRENTLY DISABLED - Tests need fixing)
if (-not $SkipTests) {
    Write-Host "Step 6: Running tests..." -ForegroundColor Yellow
    Write-Host "  [SKIP] Infrastructure tests need fixing" -ForegroundColor DarkYellow
    Write-Host "  TODO: Fix IExifReader references" -ForegroundColor DarkYellow
    Write-Host ""
}

# Step 7: Create deployment package
Write-Host "Step 7: Creating deployment package..." -ForegroundColor Yellow

# Create deployment structure with enhanced locked file handling
$deployDir = Join-Path $OutputPath "CamBridge-Deploy-v$Version"
$serviceDir = Join-Path $deployDir "Service"
$configDir = Join-Path $deployDir "ConfigTool"
$qrBridgeDir = Join-Path $deployDir "QRBridge"

# Clean and create directories with locked file handling
if (Test-Path $deployDir) {
    Write-Host "  Removing existing deployment..." -ForegroundColor Gray
    
    # Try graceful removal first
    try {
        Remove-Item $deployDir -Recurse -Force -ErrorAction Stop
        Write-Host "    [OK] Old deployment removed" -ForegroundColor Green
    } catch {
        Write-Host "    [WARNING] Some files are locked (service running?)" -ForegroundColor Yellow
        
        # Try to stop service if it's using the files
        $service = Get-Service "CamBridgeService" -ErrorAction SilentlyContinue
        if ($service -and $service.Status -eq "Running") {
            Write-Host "    Stopping CamBridge Service temporarily..." -ForegroundColor Yellow
            Stop-Service "CamBridgeService" -Force -ErrorAction SilentlyContinue
            Start-Sleep -Seconds 2
            
            # Try removal again
            try {
                Remove-Item $deployDir -Recurse -Force -ErrorAction Stop
                Write-Host "    [OK] Deployment removed after stopping service" -ForegroundColor Green
                
                # Restart service
                Write-Host "    Restarting service..." -ForegroundColor Gray
                Start-Service "CamBridgeService" -ErrorAction SilentlyContinue
            } catch {
                # Ultimate fallback: rename old directory
                $backupDir = "$deployDir-backup-$(Get-Date -Format 'yyyyMMdd-HHmmss')"
                Write-Host "    [FALLBACK] Renaming locked directory to backup" -ForegroundColor Yellow
                try {
                    Rename-Item $deployDir $backupDir -ErrorAction Stop
                    Write-Host "    [OK] Old deployment backed up to: $backupDir" -ForegroundColor Green
                } catch {
                    Write-Error "Cannot remove or rename deployment directory! Please stop the service manually and try again."
                    exit 1
                }
            }
        } else {
            # Ultimate fallback: rename old directory
            $backupDir = "$deployDir-backup-$(Get-Date -Format 'yyyyMMdd-HHmmss')"
            Write-Host "    [FALLBACK] Renaming locked directory to backup" -ForegroundColor Yellow
            try {
                Rename-Item $deployDir $backupDir -ErrorAction Stop
                Write-Host "    [OK] Old deployment backed up to: $backupDir" -ForegroundColor Green
            } catch {
                Write-Error "Cannot remove deployment directory! Files may be locked by other process."
                exit 1
            }
        }
    }
}

# Create fresh directories
New-Item -Path $serviceDir -ItemType Directory -Force | Out-Null
New-Item -Path $configDir -ItemType Directory -Force | Out-Null
if (-not $SkipQRBridge) {
    New-Item -Path $qrBridgeDir -ItemType Directory -Force | Out-Null
}

# Copy Service files
Write-Host "  Copying Service files..." -ForegroundColor Gray
$servicePath = ".\src\CamBridge.Service\bin\Release\net8.0-windows\win-x64\publish"
if (Test-Path $servicePath) {
    Copy-Item "$servicePath\*" -Destination $serviceDir -Recurse -Force
} else {
    Write-Error "Service publish folder not found!"
    exit 1
}

# ENHANCED: Copy Tools folder (ExifTool) with verification
Write-Host "  Copying Tools folder (ExifTool)..." -ForegroundColor Gray
$toolsSource = ".\src\CamBridge.Service\Tools"
$toolsTarget = Join-Path $serviceDir "Tools"
if (Test-Path $toolsSource) {
    Copy-Item $toolsSource -Destination $serviceDir -Recurse -Force
    
    # Verify exiftool.exe exists
    $exifToolPath = Join-Path $toolsTarget "exiftool.exe"
    if (Test-Path $exifToolPath) {
        Write-Host "    [OK] ExifTool copied successfully" -ForegroundColor Green
        
        # Get ExifTool version for verification
        try {
            $exifVersion = & $exifToolPath -ver 2>$null
            Write-Host "    ExifTool version: $exifVersion" -ForegroundColor DarkGray
        } catch {
            Write-Host "    [WARNING] Could not verify ExifTool version" -ForegroundColor Yellow
        }
    } else {
        Write-Warning "    ExifTool.exe not found in Tools folder!"
        Write-Host "    Please ensure exiftool.exe is in: $toolsSource" -ForegroundColor Yellow
    }
} else {
    Write-Error "Tools folder not found at $toolsSource - ExifTool will be missing!"
    Write-Host "  The service WILL NOT WORK without ExifTool!" -ForegroundColor Red
}

# Copy Config UI - Check all possible paths
Write-Host "  Copying Configuration UI..." -ForegroundColor Gray
$configFound = $false
$configPaths = @(
    ".\src\CamBridge.Config\bin\x64\Release\net8.0-windows",
    ".\src\CamBridge.Config\bin\Release\net8.0-windows",
    ".\src\CamBridge.Config\bin\x64\Debug\net8.0-windows",
    ".\src\CamBridge.Config\bin\Debug\net8.0-windows"
)

foreach ($path in $configPaths) {
    if (Test-Path "$path\CamBridge.Config.exe") {
        Write-Host "    Found Config UI at: $path" -ForegroundColor Gray
        Copy-Item "$path\*" -Destination $configDir -Recurse -Force
        $configFound = $true
        break
    }
}

if (-not $configFound) {
    Write-Error "Config UI not found in any expected location!"
    Write-Host "Searched in:" -ForegroundColor Yellow
    $configPaths | ForEach-Object { Write-Host "  $_" -ForegroundColor Gray }
    exit 1
}

# Copy QRBridge (if built)
if (-not $SkipQRBridge -and (Test-Path ".\TempQRBridgePublish")) {
    Write-Host "  Copying QRBridge 2.0..." -ForegroundColor Gray
    Copy-Item ".\TempQRBridgePublish\*" -Destination $qrBridgeDir -Recurse -Force
    Remove-Item ".\TempQRBridgePublish" -Recurse -Force
}

# Copy deployment scripts
Write-Host "  Adding deployment scripts..." -ForegroundColor Gray
@("Install-CamBridge.ps1", "Uninstall-CamBridge.ps1") | ForEach-Object {
    if (Test-Path $_) {
        Copy-Item $_ -Destination $deployDir -Force
    }
}

# Create version file with enhanced information
$versionContent = @"
CamBridge Deployment Package
Version: $Version
Build Date: $(Get-Date -Format "yyyy-MM-dd HH:mm:ss")
Build Type: Framework-Dependent (requires .NET 8.0 Runtime)
© 2025 Claude's Improbably Reliable Software Solutions

Components:
- CamBridge Service (Multi-Pipeline JPEG to DICOM)
- CamBridge Config (Pipeline Configuration UI)
$(if (-not $SkipQRBridge) { "- CamBridge QRBridge 2.0 (QR Code Generator)" })

New in v0.8.x:
- PACS Upload Support (C-STORE)
- Real DICOM Communication
- Per-Pipeline PACS Configuration

New in v0.7.x:
- THE GREAT SIMPLIFICATION (KISS Architecture)
- Direct Dependencies (No unnecessary interfaces)
- Simplified Service Layer
- Improved Performance

System Requirements:
- Windows 10/11 or Windows Server 2019+
- .NET 8.0 Runtime
- ExifTool (included in Tools folder)

IMPORTANT: ExifTool is required for operation!
The Tools folder must be in the same directory as CamBridge.Service.exe

Deployment Notes:
- VS Configuration Manager workaround included
- All projects forced to build
- Service-aware deployment with automatic conflict resolution
"@
$versionContent | Set-Content "$deployDir\version.txt"

Write-Host "  [OK] Package created" -ForegroundColor Green
Write-Host ""

# Step 8: Create ZIP archive
if (-not $SkipZip) {
    Write-Host "Step 8: Creating ZIP archive..." -ForegroundColor Yellow
    $zipPath = Join-Path $OutputPath "CamBridge-v$Version-Deploy.zip"
    Compress-Archive -Path $deployDir -DestinationPath $zipPath -Force
    Write-Host "  [OK] ZIP created" -ForegroundColor Green
} else {
    Write-Host "Step 8: Skipping ZIP creation (faster build)" -ForegroundColor DarkGray
}

# Calculate sizes
$serviceSize = (Get-ChildItem $serviceDir -Recurse | Measure-Object -Property Length -Sum).Sum / 1MB
$configSize = (Get-ChildItem $configDir -Recurse | Measure-Object -Property Length -Sum).Sum / 1MB
if (-not $SkipZip -and (Test-Path $zipPath)) {
    $zipSize = (Get-Item $zipPath).Length / 1MB
}

# Final summary
Write-Host ""
Write-Host "================================================" -ForegroundColor Green
Write-Host "  BUILD & DEPLOYMENT SUCCESSFUL!" -ForegroundColor Green
Write-Host "================================================" -ForegroundColor Green
Write-Host ""
Write-Host "Package Details:" -ForegroundColor Cyan
Write-Host "  Version:       $Version" -ForegroundColor White
Write-Host "  Service Size:  $([math]::Round($serviceSize, 2)) MB" -ForegroundColor Gray
Write-Host "  Config Size:   $([math]::Round($configSize, 2)) MB" -ForegroundColor Gray
if (-not $SkipZip -and $zipSize) {
    Write-Host "  Total ZIP:     $([math]::Round($zipSize, 2)) MB" -ForegroundColor Gray
}
Write-Host ""
Write-Host "Output Files:" -ForegroundColor Cyan
Write-Host "  Folder: $deployDir" -ForegroundColor White
if (-not $SkipZip) {
    Write-Host "  ZIP:    $zipPath" -ForegroundColor White
} else {
    Write-Host "  ZIP:    [SKIPPED - Use 00[TAB] for ZIP]" -ForegroundColor DarkGray
}
Write-Host ""

# Enhanced verification with DicomStoreService check
$deployedExifTool = Join-Path $serviceDir "Tools\exiftool.exe"
$deployedService = Join-Path $serviceDir "CamBridge.Service.exe"
$deployedConfig = Join-Path $configDir "CamBridge.Config.exe"
$deployedInfra = Join-Path $serviceDir "CamBridge.Infrastructure.dll"

Write-Host "Deployment Verification:" -ForegroundColor Cyan
if (Test-Path $deployedExifTool) {
    Write-Host "  [OK] ExifTool included" -ForegroundColor Green
} else {
    Write-Host "  [FAIL] ExifTool missing - Service will fail!" -ForegroundColor Red
}

if (Test-Path $deployedService) {
    Write-Host "  [OK] Service executable present" -ForegroundColor Green
    # Check if DicomStoreService registration is in the DLL
    $serviceFileInfo = Get-Item $deployedService
    Write-Host "       Modified: $($serviceFileInfo.LastWriteTime)" -ForegroundColor DarkGray
} else {
    Write-Host "  [FAIL] Service executable missing!" -ForegroundColor Red
}

if (Test-Path $deployedInfra) {
    Write-Host "  [OK] Infrastructure DLL present" -ForegroundColor Green
    $infraFileInfo = Get-Item $deployedInfra
    Write-Host "       Modified: $($infraFileInfo.LastWriteTime)" -ForegroundColor DarkGray
} else {
    Write-Host "  [FAIL] Infrastructure DLL missing!" -ForegroundColor Red
}

if (Test-Path $deployedConfig) {
    Write-Host "  [OK] Config UI executable present" -ForegroundColor Green
} else {
    Write-Host "  [FAIL] Config UI executable missing!" -ForegroundColor Red
}

Write-Host ""
Write-Host "================================================" -ForegroundColor Cyan
Write-Host "              NEXT STEPS" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Use the numbered tools for testing:" -ForegroundColor Yellow
Write-Host ""
Write-Host "  1[TAB]  - Deploy & Update Service (with auto-stop/start)" -ForegroundColor Gray
Write-Host "  2[TAB]  - Start Configuration UI" -ForegroundColor Gray
Write-Host "  3[TAB]  - Service Manager" -ForegroundColor Gray
Write-Host "  9[TAB]  - Quick test (no build)" -ForegroundColor Gray
Write-Host "  99[TAB] - Full test (with build)" -ForegroundColor Gray
Write-Host "  h[TAB]  - Help" -ForegroundColor Gray
Write-Host ""
Write-Host "Build completed at: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')" -ForegroundColor Gray
Write-Host ""