# Create-DeploymentPackage.ps1
# Creates a deployment package for CamBridge Service
# Version: 0.5.29

param(
    [string]$Version = "0.5.29",
    [string]$OutputPath = ".\Deploy"
)

Write-Host "Creating CamBridge Deployment Package v$Version..." -ForegroundColor Cyan

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
Write-Host "Copying Service files..." -ForegroundColor Green
$servicePath = ".\src\CamBridge.Service\bin\Release\net8.0-windows\win-x64\publish"
if (Test-Path $servicePath) {
    Copy-Item "$servicePath\*" -Destination $serviceDir -Recurse -Force
    Write-Host "  ✓ Service files copied" -ForegroundColor Green
} else {
    Write-Error "Service publish folder not found. Run: dotnet publish first!"
    exit 1
}

# Copy Config UI
Write-Host "Copying Configuration UI..." -ForegroundColor Green
$configPath = ".\src\CamBridge.Config\bin\x64\Release\net8.0-windows"
if (Test-Path $configPath) {
    Copy-Item "$configPath\*" -Destination $configDir -Recurse -Force
    Write-Host "  ✓ Config UI copied" -ForegroundColor Green
} else {
    Write-Warning "Config UI not found at expected location"
}

# Copy deployment scripts
Write-Host "Adding deployment scripts..." -ForegroundColor Green
Copy-Item ".\Install-CamBridge.ps1" -Destination $deployDir -Force
Copy-Item ".\Uninstall-CamBridge.ps1" -Destination $deployDir -Force
Copy-Item ".\README-Deployment.md" -Destination "$deployDir\README.md" -Force

# Create version file
@"
CamBridge Deployment Package
Version: $Version
Build Date: $(Get-Date -Format "yyyy-MM-dd HH:mm:ss")
© 2025 Claude's Improbably Reliable Software Solutions
"@ | Set-Content "$deployDir\version.txt"

# Create ZIP archive
Write-Host "Creating ZIP archive..." -ForegroundColor Green
$zipPath = Join-Path $OutputPath "CamBridge-v$Version-Deploy.zip"
Compress-Archive -Path $deployDir -DestinationPath $zipPath -Force

# Calculate sizes
$serviceSize = (Get-ChildItem $serviceDir -Recurse | Measure-Object -Property Length -Sum).Sum / 1MB
$configSize = (Get-ChildItem $configDir -Recurse | Measure-Object -Property Length -Sum).Sum / 1MB
$zipSize = (Get-Item $zipPath).Length / 1MB

Write-Host ""
Write-Host "================================================" -ForegroundColor Green
Write-Host "  Deployment Package Created Successfully!" -ForegroundColor Green
Write-Host "================================================" -ForegroundColor Green
Write-Host ""
Write-Host "Package: $zipPath" -ForegroundColor Cyan
Write-Host "Service Size: $([math]::Round($serviceSize, 2)) MB" -ForegroundColor Gray
Write-Host "Config Size: $([math]::Round($configSize, 2)) MB" -ForegroundColor Gray
Write-Host "Total ZIP: $([math]::Round($zipSize, 2)) MB" -ForegroundColor Gray
Write-Host ""
Write-Host "Deployment folder: $deployDir" -ForegroundColor Gray
Write-Host ""

# Test checklist
Write-Host "Pre-deployment Checklist:" -ForegroundColor Yellow
Write-Host "  □ Test installation on clean machine" -ForegroundColor Gray
Write-Host "  □ Verify service starts correctly" -ForegroundColor Gray
Write-Host "  □ Test Config UI connection" -ForegroundColor Gray
Write-Host "  □ Process test JPEG file" -ForegroundColor Gray
Write-Host "  □ Test uninstallation" -ForegroundColor Gray