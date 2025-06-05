# Test-QRBridge.ps1
# Version: 0.5.33
# Tests the new QRBridge 2.0 implementation

$ErrorActionPreference = "Stop"

# Try to find the correct path
$possiblePaths = @(
    "C:\Users\aiadmin\source\repos\CamBridge\src\CamBridge.QRBridge\bin\Debug\net8.0-windows\win-x64\CamBridge.QRBridge.exe",
    "C:\Users\aiadmin\source\repos\CamBridge\src\CamBridge.QRBridge\bin\Debug\net8.0-windows\CamBridge.QRBridge.exe",
    "C:\Users\aiadmin\source\repos\CamBridge\src\CamBridge.QRBridge\bin\Debug\net8.0\CamBridge.QRBridge.exe"
)

$qrBridgeExe = $null
foreach ($path in $possiblePaths) {
    if (Test-Path $path) {
        $qrBridgeExe = $path
        Write-Host "Found QRBridge at: $path" -ForegroundColor Green
        break
    }
}

if (-not $qrBridgeExe) {
    Write-Host "ERROR: CamBridge.QRBridge.exe not found!" -ForegroundColor Red
    Write-Host "Searching for it..." -ForegroundColor Yellow
    $found = Get-ChildItem "C:\Users\aiadmin\source\repos\CamBridge\src\CamBridge.QRBridge\bin" -Recurse -Filter "CamBridge.QRBridge.exe" -ErrorAction SilentlyContinue
    if ($found) {
        Write-Host "Found at: $($found.FullName)" -ForegroundColor Yellow
    }
    exit 1
}

Write-Host "Testing CamBridge QRBridge 2.0..." -ForegroundColor Cyan
Write-Host "=================================" -ForegroundColor Cyan

# Test 1: Help display
Write-Host "`nTest 1: Help Display" -ForegroundColor Yellow
& $qrBridgeExe -help
Write-Host "Press any key to continue..." -ForegroundColor Gray
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")

# Test 2: Basic QR Code (no special characters)
Write-Host "`nTest 2: Basic QR Code Generation" -ForegroundColor Yellow
Write-Host "Command: -examid 'EX001' -name 'Schmidt, Hans' -birthdate '1985-03-15' -gender 'M' -timeout 5"
& $qrBridgeExe -examid "EX001" -name "Schmidt, Hans" -birthdate "1985-03-15" -gender "M" -timeout 5

# Test 3: Special Characters (UTF-8 test)
Write-Host "`nTest 3: UTF-8 Special Characters" -ForegroundColor Yellow
Write-Host "Command: -examid 'EX002' -name 'Müller, Björn' -birthdate '1990-12-24' -gender 'M' -comment 'Röntgen Thorax' -timeout 5"
& $qrBridgeExe -examid "EX002" -name "Müller, Björn" -birthdate "1990-12-24" -gender "M" -comment "Röntgen Thorax" -timeout 5

# Test 4: Female patient with comment
Write-Host "`nTest 4: Female Patient with Comment" -ForegroundColor Yellow
Write-Host "Command: -examid 'EX003' -name 'Schäfer, Anna' -birthdate '1975-07-30' -gender 'F' -comment 'CT Abdomen mit KM' -timeout 5"
& $qrBridgeExe -examid "EX003" -name "Schäfer, Anna" -birthdate "1975-07-30" -gender "F" -comment "CT Abdomen mit KM" -timeout 5

# Test 5: Minimal required fields only
Write-Host "`nTest 5: Minimal Fields (only required)" -ForegroundColor Yellow
Write-Host "Command: -examid 'EX004' -name 'Test, Patient'"
& $qrBridgeExe -examid "EX004" -name "Test, Patient"

# Test 6: Error case - missing required field
Write-Host "`nTest 6: Error Handling (missing -name)" -ForegroundColor Yellow
Write-Host "Command: -examid 'EX005'"
& $qrBridgeExe -examid "EX005"

Write-Host "`n✅ All tests completed!" -ForegroundColor Green
Write-Host "`nNotes:" -ForegroundColor Cyan
Write-Host "- Check if UTF-8 characters (ä, ö, ü, ß) display correctly"
Write-Host "- Verify countdown timer works"
Write-Host "- Test QR code with Ricoh camera if available"
Write-Host "- ESC key should close the window immediately"