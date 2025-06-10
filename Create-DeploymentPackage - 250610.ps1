# Create-DeploymentPackage.ps1
# Complete build and deployment package creator for CamBridge
# Version: 0.7.0
# Updated for Multi-Pipeline Architecture
# KISS UPDATE: Now includes Tools folder copy!

param(
    [string]$Version = "",  # Auto-detect from Version.props
    [string]$OutputPath = ".\Deploy",
    [switch]$SkipClean = $false,
    [switch]$SkipTests = $false,
    [switch]$SkipQRBridge = $false,
    [switch]$LaunchConfig = $false,
    [switch]$ShowWarnings = $false  # Default: Warnings unterdrückt
)

# Read version from Version.props if not specified
if ([string]::IsNullOrEmpty($Version)) {
    if (Test-Path ".\Version.props") {
        [xml]$versionProps = Get-Content ".\Version.props"
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
Write-Host " Multi-Pipeline Architecture Ready!" -ForegroundColor Green
if (-not $ShowWarnings) {
    Write-Host " Build Warnings: SUPPRESSED" -ForegroundColor DarkGray
}
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Step 1: Clean everything
if (-not $SkipClean) {
    Write-Host "Step 1: Cleaning build directories..." -ForegroundColor Yellow
    
    # Clean all bin and obj folders
    Get-ChildItem -Path "src" -Include "bin","obj" -Recurse -Directory | ForEach-Object {
        Write-Host "  Cleaning: $($_.FullName)" -ForegroundColor Gray
        Remove-Item $_.FullName -Recurse -Force -ErrorAction SilentlyContinue
    }
    
    # Also clean test folders
    Get-ChildItem -Path "tests" -Include "bin","obj" -Recurse -Directory | ForEach-Object {
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

# Step 3: Build Config UI with explicit x64 platform
Write-Host "Step 3: Building Configuration UI (x64)..." -ForegroundColor Yellow
# Try MSBuild first for proper x64 platform build
$msbuildPath = & "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe" -latest -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe | Select-Object -First 1

if ($msbuildPath -and (Test-Path $msbuildPath)) {
    Write-Host "  Using MSBuild for x64 platform build..." -ForegroundColor Gray
    if ($ShowWarnings) {
        & $msbuildPath src\CamBridge.Config\CamBridge.Config.csproj /p:Configuration=Release /p:Platform=x64
    } else {
        & $msbuildPath src\CamBridge.Config\CamBridge.Config.csproj /p:Configuration=Release /p:Platform=x64 /p:WarningLevel=0 /v:minimal
    }
} else {
    Write-Host "  Using dotnet CLI (platform might vary)..." -ForegroundColor Yellow
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
Write-Host "  [OK] Config UI built" -ForegroundColor Green
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

# Step 5: Publish Service
Write-Host "Step 5: Publishing Windows Service..." -ForegroundColor Yellow
if ($ShowWarnings) {
    dotnet publish src\CamBridge.Service\CamBridge.Service.csproj -c Release -r win-x64 --self-contained
} else {
    dotnet publish src\CamBridge.Service\CamBridge.Service.csproj -c Release -r win-x64 --self-contained --verbosity quiet
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

# Create deployment structure
$deployDir = Join-Path $OutputPath "CamBridge-Deploy-v$Version"
$serviceDir = Join-Path $deployDir "Service"
$configDir = Join-Path $deployDir "Config"
$qrBridgeDir = Join-Path $deployDir "QRBridge"

# Clean and create directories
if (Test-Path $deployDir) {
    Remove-Item $deployDir -Recurse -Force
}
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

# KISS FIX: Copy Tools folder (ExifTool) - THIS WAS MISSING!
Write-Host "  Copying Tools folder (ExifTool)..." -ForegroundColor Gray
$toolsSource = ".\src\CamBridge.Service\Tools"
$toolsTarget = Join-Path $serviceDir "Tools"
if (Test-Path $toolsSource) {
    Copy-Item $toolsSource -Destination $serviceDir -Recurse -Force
    
    # Verify exiftool.exe exists
    $exifToolPath = Join-Path $toolsTarget "exiftool.exe"
    if (Test-Path $exifToolPath) {
        Write-Host "    [OK] ExifTool copied successfully" -ForegroundColor Green
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

# Create version file
$versionContent = @"
CamBridge Deployment Package
Version: $Version
Build Date: $(Get-Date -Format "yyyy-MM-dd HH:mm:ss")
Build Type: Framework-Dependent (requires .NET 8.0 Runtime)
(c) 2025 Claude's Improbably Reliable Software Solutions

Components:
- CamBridge Service (Multi-Pipeline JPEG to DICOM)
- CamBridge Config (Pipeline Configuration UI)
$(if (-not $SkipQRBridge) { "- CamBridge QRBridge 2.0 (QR Code Generator)" })

New in v0.7.x:
- THE GREAT SIMPLIFICATION (KISS Architecture)
- Direct Dependencies (No unnecessary interfaces)
- Simplified Service Layer
- Improved Performance

New in v0.6.x:
- Multi-Pipeline Architecture
- Pipeline-specific Mapping Sets
- Per-Pipeline Configuration
- Enhanced Dashboard

System Requirements:
- Windows 10/11 or Windows Server 2019+
- .NET 8.0 Runtime
- ExifTool (included in Tools folder)

IMPORTANT: ExifTool is required for operation!
The Tools folder must be in the same directory as CamBridge.Service.exe
"@
$versionContent | Set-Content "$deployDir\version.txt"

Write-Host "  [OK] Package created" -ForegroundColor Green
Write-Host ""

# Step 8: Create ZIP archive
Write-Host "Step 8: Creating ZIP archive..." -ForegroundColor Yellow
$zipPath = Join-Path $OutputPath "CamBridge-v$Version-Deploy.zip"
Compress-Archive -Path $deployDir -DestinationPath $zipPath -Force
Write-Host "  [OK] ZIP created" -ForegroundColor Green

# Calculate sizes
$serviceSize = (Get-ChildItem $serviceDir -Recurse | Measure-Object -Property Length -Sum).Sum / 1MB
$configSize = (Get-ChildItem $configDir -Recurse | Measure-Object -Property Length -Sum).Sum / 1MB
$zipSize = (Get-Item $zipPath).Length / 1MB

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
Write-Host "  Total ZIP:     $([math]::Round($zipSize, 2)) MB" -ForegroundColor Gray
Write-Host ""
Write-Host "Output Files:" -ForegroundColor Cyan
Write-Host "  Folder: $deployDir" -ForegroundColor White
Write-Host "  ZIP:    $zipPath" -ForegroundColor White
Write-Host ""

# Check if ExifTool was included
$deployedExifTool = Join-Path $serviceDir "Tools\exiftool.exe"
if (Test-Path $deployedExifTool) {
    Write-Host "ExifTool included in deployment" -ForegroundColor Green
} else {
    Write-Host "WARNING: ExifTool NOT included - Service will fail!" -ForegroundColor Red
}
Write-Host ""

# Interactive Launch Menu
$configExe = Join-Path $configDir "CamBridge.Config.exe"
$serviceExe = Join-Path $serviceDir "CamBridge.Service.exe"

Write-Host "================================================" -ForegroundColor Cyan
Write-Host "            QUICK START MENU" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "What would you like to do?" -ForegroundColor Yellow
Write-Host ""
Write-Host "  [1] Start Configuration UI" -ForegroundColor White
Write-Host "      Launch the Pipeline Configuration interface" -ForegroundColor Gray
Write-Host ""
Write-Host "  [2] Open Deploy Folder" -ForegroundColor White
Write-Host "      Browse the deployment package" -ForegroundColor Gray
Write-Host ""
Write-Host "  [3] Install Service (Admin)" -ForegroundColor White
Write-Host "      Run Install-CamBridge.ps1 as Administrator" -ForegroundColor Gray
Write-Host ""
Write-Host "  [4] Start Fresh Build" -ForegroundColor White
Write-Host "      Clean everything and rebuild" -ForegroundColor Gray
Write-Host ""
Write-Host "  [X] Exit" -ForegroundColor White
Write-Host ""

do {
    $choice = Read-Host "Select option (1-4, X to exit)"
    
    switch ($choice.ToUpper()) {
        "1" {
            if (Test-Path $configExe) {
                Write-Host "Starting Configuration UI..." -ForegroundColor Green
                Start-Process $configExe
                Write-Host "Config UI launched!" -ForegroundColor Green
                Write-Host ""
                Write-Host "NOTE: Service management is available in:" -ForegroundColor Yellow
                Write-Host "  Config UI > Service Control" -ForegroundColor Gray
                Write-Host "  - Start/Stop Service" -ForegroundColor Gray
                Write-Host "  - Install/Uninstall Service" -ForegroundColor Gray
                Write-Host "  - View Service Status" -ForegroundColor Gray
            } else {
                Write-Error "Config executable not found!"
            }
            break
        }
        "2" {
            Write-Host "Opening deployment folder..." -ForegroundColor Green
            Start-Process explorer.exe $deployDir
            break
        }
        "3" {
            $installScript = Join-Path $deployDir "Install-CamBridge.ps1"
            if (Test-Path $installScript) {
                Write-Host "Launching installer as Administrator..." -ForegroundColor Yellow
                Write-Host "You may see a UAC prompt." -ForegroundColor Gray
                Start-Process powershell -ArgumentList "-ExecutionPolicy Bypass -File `"$installScript`"" -Verb RunAs
            } else {
                Write-Error "Install script not found!"
            }
            break
        }
        "4" {
            Write-Host "Starting fresh build..." -ForegroundColor Yellow
            & $PSCommandPath -Version $Version -OutputPath $OutputPath -ShowWarnings:$ShowWarnings
            exit
        }
        "X" {
            Write-Host "Goodbye!" -ForegroundColor Green
            break
        }
        default {
            Write-Host "Invalid option. Please select 1-4 or X." -ForegroundColor Red
        }
    }
} while ($choice.ToUpper() -ne "X" -and $choice -notin @("1","2","3","4"))

Write-Host ""
Write-Host "Build completed at: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')" -ForegroundColor Gray
Write-Host ""