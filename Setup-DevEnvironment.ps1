# Setup-DevEnvironment.ps1
# Creates folder structure for CamBridge development environment

Write-Host "Setting up CamBridge Development Environment..." -ForegroundColor Cyan

# Base paths
$testBase = "C:\CamBridge\Test"
$prodBase = "C:\CamBridge"

# Create test folders
$testFolders = @(
    "$testBase\Input",
    "$testBase\Output",
    "$testBase\Archive",
    "$testBase\DeadLetter",
    "$testBase\Debug",
    "$testBase\Debug\ExifDumps",
    "$testBase\Debug\ParsedData",
    "$testBase\Debug\DicomTags"
)

# Create production folders (for later)
$prodFolders = @(
    "$prodBase\Input",
    "$prodBase\Output",
    "$prodBase\Archive",
    "$prodBase\DeadLetter",
    "$prodBase\Logs"
)

Write-Host "`nCreating Test Folders:" -ForegroundColor Yellow
foreach ($folder in $testFolders) {
    if (!(Test-Path $folder)) {
        New-Item -ItemType Directory -Path $folder -Force | Out-Null
        Write-Host "  ✓ Created: $folder" -ForegroundColor Green
    } else {
        Write-Host "  • Exists: $folder" -ForegroundColor Gray
    }
}

Write-Host "`nCreating Production Folders:" -ForegroundColor Yellow
foreach ($folder in $prodFolders) {
    if (!(Test-Path $folder)) {
        New-Item -ItemType Directory -Path $folder -Force | Out-Null
        Write-Host "  ✓ Created: $folder" -ForegroundColor Green
    } else {
        Write-Host "  • Exists: $folder" -ForegroundColor Gray
    }
}

# Create sample README files
$readmeContent = @"
# CamBridge Test Folders

## Folder Structure:
- **Input**: Place JPEG files here for processing
- **Output**: Converted DICOM files will appear here
- **Archive**: Successfully processed files (if archiving enabled)
- **DeadLetter**: Files that failed processing
- **Debug**: Diagnostic output (Development mode only)

## Testing:
1. Copy a Ricoh G900 II JPEG to the Input folder
2. Watch the console output
3. Check Output folder for DICOM file
4. Check Debug folder for diagnostic data
"@

$readmePath = "$testBase\README.md"
$readmeContent | Out-File -FilePath $readmePath -Encoding UTF8
Write-Host "`n✓ Created README at: $readmePath" -ForegroundColor Green

# Create a test monitoring script
$monitorScript = @'
# Monitor-TestFolders.ps1
# Watches test folders for activity

param(
    [int]$RefreshSeconds = 2
)

Write-Host "Monitoring CamBridge Test Folders..." -ForegroundColor Cyan
Write-Host "Press Ctrl+C to stop`n" -ForegroundColor Gray

$folders = @{
    "Input"      = "C:\CamBridge\Test\Input"
    "Output"     = "C:\CamBridge\Test\Output"
    "DeadLetter" = "C:\CamBridge\Test\DeadLetter"
    "Debug"      = "C:\CamBridge\Test\Debug"
}

while ($true) {
    Clear-Host
    Write-Host "CamBridge Folder Monitor - $(Get-Date -Format 'HH:mm:ss')" -ForegroundColor Cyan
    Write-Host ("=" * 50) -ForegroundColor DarkGray
    
    foreach ($name in $folders.Keys) {
        $path = $folders[$name]
        if (Test-Path $path) {
            $files = Get-ChildItem -Path $path -File
            $count = $files.Count
            
            if ($count -gt 0) {
                Write-Host "`n$name ($count files):" -ForegroundColor Yellow
                $files | Select-Object -First 5 | ForEach-Object {
                    $size = "{0:N2} KB" -f ($_.Length / 1KB)
                    Write-Host "  • $($_.Name) ($size)" -ForegroundColor White
                }
                if ($count -gt 5) {
                    Write-Host "  ... and $($count - 5) more" -ForegroundColor Gray
                }
            } else {
                Write-Host "`n$name`: Empty" -ForegroundColor DarkGray
            }
        }
    }
    
    Start-Sleep -Seconds $RefreshSeconds
}
'@

$monitorPath = "$testBase\Monitor-TestFolders.ps1"
$monitorScript | Out-File -FilePath $monitorPath -Encoding UTF8
Write-Host "✓ Created monitoring script at: $monitorPath" -ForegroundColor Green

# Check for ExifTool
Write-Host "`nChecking for ExifTool..." -ForegroundColor Yellow
$exifToolPaths = @(
    "$prodBase\Tools\exiftool.exe",
    (Join-Path $PSScriptRoot "..\..\Tools\exiftool.exe"),
    "C:\Tools\exiftool.exe"
)

$exifToolFound = $false
foreach ($path in $exifToolPaths) {
    if (Test-Path $path) {
        Write-Host "  ✓ ExifTool found at: $path" -ForegroundColor Green
        $exifToolFound = $true
        break
    }
}

if (!$exifToolFound) {
    Write-Host "  ⚠ ExifTool not found!" -ForegroundColor Red
    Write-Host "  Download from: https://exiftool.org/exiftool-12.96.zip" -ForegroundColor Yellow
    Write-Host "  Extract to: $prodBase\Tools\exiftool.exe" -ForegroundColor Yellow
}

# Summary
Write-Host "`n" ("=" * 50) -ForegroundColor DarkGray
Write-Host "Setup Complete!" -ForegroundColor Green
Write-Host "`nNext steps:" -ForegroundColor Cyan
Write-Host "1. Run CamBridge.Service in console mode with Development settings"
Write-Host "2. Copy test JPEG files to: $testBase\Input"
Write-Host "3. Monitor folders with: $monitorPath"
Write-Host "4. Check debug output in: $testBase\Debug"

Write-Host "`nPress any key to exit..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")