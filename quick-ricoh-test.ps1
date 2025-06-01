# Quick Ricoh JPEG EXIF Test
# © 2025 Claude's Improbably Reliable Software Solutions

param(
    [Parameter(Mandatory=$true)]
    [string]$JpegPath
)

Write-Host "Quick Ricoh JPEG Analysis" -ForegroundColor Cyan
Write-Host "=========================" -ForegroundColor Cyan
Write-Host ""

if (-not (Test-Path $JpegPath)) {
    Write-Host "ERROR: File not found: $JpegPath" -ForegroundColor Red
    exit 1
}

# Load the image
Add-Type -AssemblyName System.Drawing
$image = [System.Drawing.Image]::FromFile($JpegPath)

Write-Host "File: $($JpegPath | Split-Path -Leaf)"
Write-Host "Size: $((Get-Item $JpegPath).Length / 1MB) MB"
Write-Host "Dimensions: $($image.Width) x $($image.Height)"
Write-Host ""

# Extract EXIF properties
Write-Host "EXIF Properties:" -ForegroundColor Yellow
$foundUserComment = $false
$foundBarcode = $false

foreach ($prop in $image.PropertyItems) {
    $id = "0x{0:X4}" -f $prop.Id
    $value = $null
    
    # Decode based on type
    switch ($prop.Type) {
        2 { # ASCII
            $value = [System.Text.Encoding]::ASCII.GetString($prop.Value).TrimEnd("`0")
        }
        7 { # Undefined (often used for User Comment)
            # Try different encodings
            $value = [System.Text.Encoding]::UTF8.GetString($prop.Value).TrimEnd("`0")
            if ($value -match '^(ASCII|UNICODE|JIS)') {
                # Skip encoding prefix
                $value = $value.Substring(8)
            }
        }
        default {
            $value = "Binary data (Type $($prop.Type))"
        }
    }
    
    # Known EXIF tags
    $tagName = switch ($prop.Id) {
        0x010F { "Make" }
        0x0110 { "Model" }
        0x0131 { "Software" }
        0x0132 { "DateTime" }
        0x9003 { "DateTimeOriginal" }
        0x9286 { "UserComment" }
        0x9999 { "Barcode/Custom" }
        default { "Unknown" }
    }
    
    Write-Host "  $id ($tagName): $value"
    
    # Check for QRBridge data
    if ($value -and $value -match '\|') {
        if ($prop.Id -eq 0x9286) {
            $foundUserComment = $true
            Write-Host "    -> Found QRBridge data in UserComment!" -ForegroundColor Green
        } elseif ($prop.Id -eq 0x9999 -or $tagName -eq "Unknown") {
            $foundBarcode = $true
            Write-Host "    -> Found QRBridge data in custom field!" -ForegroundColor Green
        }
        
        # Parse QRBridge data
        $parts = $value -split '\|'
        if ($parts.Count -ge 4) {
            Write-Host ""
            Write-Host "QRBridge Data Parsed:" -ForegroundColor Green
            Write-Host "  Exam ID: $($parts[0])"
            Write-Host "  Patient: $($parts[1])"
            Write-Host "  Birth Date: $($parts[2])"
            Write-Host "  Gender: $($parts[3])"
            if ($parts.Count -gt 4) {
                Write-Host "  Comment: $($parts[4])"
            }
        }
    }
}

$image.Dispose()

if (-not $foundUserComment -and -not $foundBarcode) {
    Write-Host ""
    Write-Host "WARNING: No QRBridge data found!" -ForegroundColor Red
    Write-Host "Make sure to scan a QR code before taking the photo." -ForegroundColor Yellow
}

Write-Host ""
Write-Host "Analysis complete." -ForegroundColor Cyan