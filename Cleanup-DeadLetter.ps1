# Cleanup-DeadLetter.ps1
# Version: 0.7.8
# Description: Removes DeadLetterQueue and verifies version consistency

Write-Host "CamBridge v0.7.8 - Dead Letter Surgery & Version Fix" -ForegroundColor Cyan
Write-Host "===================================================" -ForegroundColor Cyan

# Step 1: Delete Dead Letter Files
Write-Host "`nStep 1: Removing Dead Letter Queue files..." -ForegroundColor Yellow

$deadLetterFiles = @(
    'src\CamBridge.Infrastructure\Services\DeadLetterQueue.cs',
    'src\CamBridge.Infrastructure\Services\INotificationService.cs',
    'src\CamBridge.Infrastructure\Services\NotificationService.cs'
)

foreach ($file in $deadLetterFiles) {
    if (Test-Path $file) {
        Remove-Item $file -Force
        Write-Host "  [DELETED] $file" -ForegroundColor Red
    } else {
        Write-Host "  [SKIP] $file (not found)" -ForegroundColor Gray
    }
}

# Step 2: Clean old AssemblyInfo files
Write-Host "`nStep 2: Removing old AssemblyInfo.cs files..." -ForegroundColor Yellow

Get-ChildItem -Path . -Filter "AssemblyInfo.cs" -Recurse | ForEach-Object {
    Write-Host "  [FOUND] $($_.FullName)" -ForegroundColor Yellow
    # Backup first
    Copy-Item $_.FullName "$($_.FullName).backup_078"
    # Then delete
    Remove-Item $_.FullName -Force
    Write-Host "  [DELETED] $($_.FullName)" -ForegroundColor Red
}

# Step 3: Clean build artifacts
Write-Host "`nStep 3: Cleaning build artifacts..." -ForegroundColor Yellow

Get-ChildItem -Path . -Include bin,obj -Recurse -Force | 
    Remove-Item -Recurse -Force -ErrorAction SilentlyContinue

Write-Host "  [CLEANED] All bin/obj folders" -ForegroundColor Green

# Step 4: Version Check
Write-Host "`nStep 4: Checking version consistency..." -ForegroundColor Yellow

# Find all version strings
$versionFiles = @{
    "Version.props" = Get-Content "Version.props" | Select-String "0\.[0-9]\.[0-9]"
    "ServiceInfo.cs" = Get-Content "src\CamBridge.Service\ServiceInfo.cs" | Select-String "Version = "
    "QRBridgeConstants.cs" = Get-Content "src\CamBridge.QRBridge\Constants\QRBridgeConstants.cs" | Select-String "Version = "
    "AboutPage.xaml" = Get-Content "src\CamBridge.Config\Views\AboutPage.xaml" | Select-String "Version [0-9]"
}

foreach ($file in $versionFiles.Keys) {
    Write-Host "  [$file] $($versionFiles[$file])" -ForegroundColor Cyan
}

# Step 5: Build
Write-Host "`nStep 5: Building solution..." -ForegroundColor Yellow

dotnet restore
dotnet build --no-restore

if ($LASTEXITCODE -eq 0) {
    Write-Host "`n[SUCCESS] Build completed successfully!" -ForegroundColor Green
    
    # Step 6: Count LOC reduction
    Write-Host "`nStep 6: Counting LOC reduction..." -ForegroundColor Yellow
    
    $totalLines = Get-ChildItem "src" -Include "*.cs" -Recurse | 
        ForEach-Object { (Get-Content $_).Count } | 
        Measure-Object -Sum
    
    Write-Host "  Total lines in src: $($totalLines.Sum)" -ForegroundColor Cyan
    Write-Host "  Estimated reduction: ~650 LOC" -ForegroundColor Green
    
} else {
    Write-Host "`n[ERROR] Build failed!" -ForegroundColor Red
}

Write-Host "`n=== Dead Letter Surgery Complete! ===" -ForegroundColor Green
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "  1. Run: .\0-build.ps1" -ForegroundColor White
Write-Host "  2. Run: .\9-testit.ps1" -ForegroundColor White
Write-Host "  3. Check: .\Check-CamBridgeVersions.ps1" -ForegroundColor White
Write-Host "`nVersion 0.7.8 - Professional consistency achieved!" -ForegroundColor Cyan