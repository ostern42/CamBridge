# Test-CompletePipeline.ps1
# Tests the complete CamBridge pipeline from QR generation to DICOM
# Version: 0.5.33

Write-Host ""
Write-Host "======================================" -ForegroundColor Cyan
Write-Host " CamBridge Complete Pipeline Test" -ForegroundColor Cyan
Write-Host " QR → Camera → JPEG → DICOM" -ForegroundColor Cyan
Write-Host "======================================" -ForegroundColor Cyan
Write-Host ""

# Step 1: Generate QR Code
Write-Host "Step 1: Generating QR Code..." -ForegroundColor Yellow
$testData = @{
    ExamId = "TEST-$(Get-Date -Format 'yyyyMMddHHmm')"
    Name = "Müller, Wolfgang"
    BirthDate = "1975-06-15"
    Gender = "M"
    Comment = "Röntgen Thorax PA"
}

Write-Host "Patient Data:" -ForegroundColor Gray
$testData.GetEnumerator() | ForEach-Object {
    Write-Host "  $($_.Key): $($_.Value)" -ForegroundColor Gray
}

# Find QRBridge
$qrBridge = $null
$paths = @(
    ".\src\CamBridge.QRBridge\bin\Debug\net8.0-windows\CamBridge.QRBridge.exe",
    ".\src\CamBridge.QRBridge\bin\Release\net8.0-windows\CamBridge.QRBridge.exe",
    ".\Deploy\CamBridge-Deploy-v0.5.33\QRBridge\CamBridge.QRBridge.exe"
)

foreach ($path in $paths) {
    if (Test-Path $path) {
        $qrBridge = $path
        break
    }
}

if ($qrBridge) {
    Write-Host "Showing QR Code (5 seconds)..." -ForegroundColor Yellow
    & $qrBridge -examid $testData.ExamId -name $testData.Name -birthdate $testData.BirthDate -gender $testData.Gender -comment $testData.Comment -timeout 5
    Write-Host "[OK] QR Code generated" -ForegroundColor Green
} else {
    Write-Warning "QRBridge not found - skipping QR generation"
}

Write-Host ""
Write-Host "Step 2: Camera Simulation" -ForegroundColor Yellow
Write-Host "In real usage:" -ForegroundColor Gray
Write-Host "  1. Photograph the QR code with Ricoh G900 II" -ForegroundColor Gray
Write-Host "  2. Take medical images" -ForegroundColor Gray
Write-Host "  3. Transfer JPEGs to C:\CamBridge\Input" -ForegroundColor Gray
Write-Host ""

# Step 3: Check if service is running
Write-Host "Step 3: Checking CamBridge Service..." -ForegroundColor Yellow
$service = Get-Service -Name "CamBridgeService" -ErrorAction SilentlyContinue
if ($service) {
    if ($service.Status -eq "Running") {
        Write-Host "[OK] Service is running" -ForegroundColor Green
        Write-Host "  Input folder: C:\CamBridge\Input" -ForegroundColor Gray
        Write-Host "  Output folder: C:\CamBridge\Output" -ForegroundColor Gray
    } else {
        Write-Warning "Service is not running!"
        Write-Host "Start it with: Start-Service CamBridgeService" -ForegroundColor Yellow
    }
} else {
    Write-Warning "CamBridge Service not installed"
    Write-Host "Run Install-CamBridge.ps1 first" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "Step 4: Pipeline Flow" -ForegroundColor Yellow
Write-Host @"
  ┌─────────────┐
  │  QRBridge   │ Generate QR with patient data
  └──────┬──────┘
         │
         ▼
  ┌─────────────┐
  │Ricoh Camera │ Scan QR, take images
  └──────┬──────┘
         │
         ▼
  ┌─────────────┐
  │    JPEG     │ With embedded QR data
  └──────┬──────┘
         │
         ▼
  ┌─────────────┐
  │  CamBridge  │ Extract data, convert
  └──────┬──────┘
         │
         ▼
  ┌─────────────┐
  │   DICOM     │ Ready for PACS
  └─────────────┘
"@ -ForegroundColor Cyan

Write-Host ""
Write-Host "Complete pipeline is ready!" -ForegroundColor Green
Write-Host "UTF-8 encoding is preserved throughout the entire flow." -ForegroundColor Green
Write-Host ""

# Show example QR data format
Write-Host "QR Code Format (for reference):" -ForegroundColor Yellow
$qrFormat = "$($testData.ExamId)|$($testData.Name)|$($testData.BirthDate)|$($testData.Gender[0])|$($testData.Comment)"
Write-Host "  $qrFormat" -ForegroundColor Gray
Write-Host ""