# Create-DeploymentPackage.ps1
# Complete build and deployment package creator for CamBridge
# Version: 0.5.30

param(
    [string]$Version = "0.5.30",
    [string]$OutputPath = ".\Deploy",
    [switch]$SkipClean = $false
)

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host " CamBridge Deployment Package Builder" -ForegroundColor Cyan
Write-Host " Version: $Version" -ForegroundColor Cyan
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
dotnet restore CamBridge.sln
if ($LASTEXITCODE -ne 0) {
    Write-Error "Restore failed!"
    exit 1
}
Write-Host "  [OK] Packages restored" -ForegroundColor Green
Write-Host ""

# Step 3: Build Config UI
Write-Host "Step 3: Building Configuration UI..." -ForegroundColor Yellow
dotnet build src\CamBridge.Config\CamBridge.Config.csproj -c Release
if ($LASTEXITCODE -ne 0) {
    Write-Error "Config UI build failed!"
    exit 1
}
Write-Host "  [OK] Config UI built" -ForegroundColor Green
Write-Host ""

# Step 4: Publish Service
Write-Host "Step 4: Publishing Windows Service..." -ForegroundColor Yellow
dotnet publish src\CamBridge.Service\CamBridge.Service.csproj -c Release -r win-x64 --self-contained
if ($LASTEXITCODE -ne 0) {
    Write-Error "Service publish failed!"
    exit 1
}
Write-Host "  [OK] Service published" -ForegroundColor Green
Write-Host ""

# Step 5: Create deployment package
Write-Host "Step 5: Creating deployment package..." -ForegroundColor Yellow

# Create deployment structure
$deployDir = Join-Path $OutputPath "CamBridge-Deploy-v$Version"
$serviceDir = Join-Path $deployDir "Service"
$configDir = Join-Path $deployDir "Config"

# Clean and create directories
if (Test-Path $deployDir) {
    Remove-Item $deployDir -Recurse -Force
}
New-Item -Path $serviceDir -ItemType Directory -Force | Out-Null
New-Item -Path $configDir -ItemType Directory -Force | Out-Null

# Copy Service files
Write-Host "  Copying Service files..." -ForegroundColor Gray
$servicePath = ".\src\CamBridge.Service\bin\Release\net8.0-windows\win-x64\publish"
if (Test-Path $servicePath) {
    Copy-Item "$servicePath\*" -Destination $serviceDir -Recurse -Force
} else {
    Write-Error "Service publish folder not found!"
    exit 1
}

# Copy Config UI
Write-Host "  Copying Configuration UI..." -ForegroundColor Gray
$configPath = ".\src\CamBridge.Config\bin\x64\Release\net8.0-windows"
if (Test-Path $configPath) {
    Copy-Item "$configPath\*" -Destination $configDir -Recurse -Force
} else {
    # Try alternate path
    $configPath = ".\src\CamBridge.Config\bin\Release\net8.0-windows"
    if (Test-Path $configPath) {
        Copy-Item "$configPath\*" -Destination $configDir -Recurse -Force
    } else {
        Write-Warning "Config UI not found!"
    }
}

# Copy deployment scripts
Write-Host "  Adding deployment scripts..." -ForegroundColor Gray
@("Install-CamBridge.ps1", "Uninstall-CamBridge.ps1") | ForEach-Object {
    if (Test-Path $_) {
        Copy-Item $_ -Destination $deployDir -Force
    }
}
if (Test-Path "README-Deployment.md") {
    Copy-Item "README-Deployment.md" -Destination "$deployDir\README.md" -Force
}

# Create version file
$versionContent = @"
CamBridge Deployment Package
Version: $Version
Build Date: $(Get-Date -Format "yyyy-MM-dd HH:mm:ss")
(c) 2025 Claude's Improbably Reliable Software Solutions
"@
$versionContent | Set-Content "$deployDir\version.txt"

Write-Host "  [OK] Package created" -ForegroundColor Green
Write-Host ""

# Step 6: Create ZIP archive
Write-Host "Step 6: Creating ZIP archive..." -ForegroundColor Yellow
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
Write-Host "  Version:      $Version" -ForegroundColor White
Write-Host "  Service Size: $([math]::Round($serviceSize, 2)) MB" -ForegroundColor Gray
Write-Host "  Config Size:  $([math]::Round($configSize, 2)) MB" -ForegroundColor Gray
Write-Host "  Total ZIP:    $([math]::Round($zipSize, 2)) MB" -ForegroundColor Gray
Write-Host ""
Write-Host "Output Files:" -ForegroundColor Cyan
Write-Host "  Folder: $deployDir" -ForegroundColor White
Write-Host "  ZIP:    $zipPath" -ForegroundColor White
Write-Host ""
Write-Host "Next Steps:" -ForegroundColor Yellow
Write-Host "  1. Extract ZIP on target machine" -ForegroundColor Gray
Write-Host "  2. Run Install-CamBridge.ps1 as Administrator" -ForegroundColor Gray
Write-Host "  3. Start Config UI from: C:\Program Files\CamBridge\Config" -ForegroundColor Gray
Write-Host ""