# Simple Ricoh JPEG Test using MetadataExtractor
# © 2025 Claude's Improbably Reliable Software Solutions

param(
    [Parameter(Mandatory=$true)]
    [string]$JpegPath
)

# Create temp directory for DLLs
$tempDir = Join-Path $env:TEMP "CamBridgeSimpleTest"
New-Item -ItemType Directory -Force -Path $tempDir | Out-Null

# NuGet packages we need
$packages = @(
    @{Name="MetadataExtractor"; Version="2.8.1"}
)

Write-Host "CamBridge Simple Ricoh Test" -ForegroundColor Cyan
Write-Host "===========================" -ForegroundColor Cyan
Write-Host ""

# Check file
if (-not (Test-Path $JpegPath)) {
    Write-Host "ERROR: File not found: $JpegPath" -ForegroundColor Red
    exit 1
}

# Download MetadataExtractor if needed
$metadataExtractorPath = Join-Path $tempDir "MetadataExtractor.dll"
if (-not (Test-Path $metadataExtractorPath)) {
    Write-Host "Downloading MetadataExtractor..." -ForegroundColor Yellow
    
    $url = "https://www.nuget.org/api/v2/package/MetadataExtractor/2.8.1"
    $zipPath = Join-Path $tempDir "metadataextractor.zip"
    
    try {
        Invoke-WebRequest -Uri $url -OutFile $zipPath
        Expand-Archive -Path $zipPath -DestinationPath $tempDir -Force
        
        # Find the DLL
        $dll = Get-ChildItem -Path $tempDir -Filter "MetadataExtractor.dll" -Recurse | Select-Object -First 1
        if ($dll) {
            Copy-Item $dll.FullName $metadataExtractorPath
        }
    } catch {
        Write-Host "Failed to download MetadataExtractor. Using fallback method." -ForegroundColor Red
    }
}

# If we have the DLL, use it
if (Test-Path $metadataExtractorPath) {
    Add-Type -Path $metadataExtractorPath
    
    Write-Host "Using MetadataExtractor library" -ForegroundColor Green
    
    # Read metadata
    $directories = [MetadataExtractor.ImageMetadataReader]::ReadMetadata($JpegPath)
    
    Write-Host "Found $($directories.Count) metadata directories" -ForegroundColor Yellow
    Write-Host ""
    
    $foundQRBridge = $false
    
    foreach ($directory in $directories) {
        Write-Host "Directory: $($directory.Name)" -ForegroundColor Cyan
        
        foreach ($tag in $directory.Tags) {
            $tagName = $tag.Name
            $tagValue = $tag.Description
            
            # Special handling for User Comment
            if ($tag.Type -eq 0x9286 -or $tagName -like "*User*Comment*") {
                Write-Host "  User Comment: $tagValue" -ForegroundColor Yellow
                
                # Check for pipe-delimited format
                if ($tagValue -match '\|') {
                    $foundQRBridge = $true
                    Write-Host "    -> FOUND QRBridge data!" -ForegroundColor Green
                    
                    $parts = $tagValue -split '\|'
                    Write-Host "    Exam ID: $($parts[0])" -ForegroundColor Green
                    if ($parts.Length -gt 1) { Write-Host "    Patient: $($parts[1])" -ForegroundColor Green }
                    if ($parts.Length -gt 2) { Write-Host "    Birth Date: $($parts[2])" -ForegroundColor Green }
                    if ($parts.Length -gt 3) { Write-Host "    Gender: $($parts[3])" -ForegroundColor Green }
                    if ($parts.Length -gt 4) { Write-Host "    Comment: $($parts[4])" -ForegroundColor Green }
                }
            }
            elseif ($tagName -like "*Make*" -or $tagName -like "*Model*" -or $tagName -like "*Software*") {
                Write-Host "  $tagName : $tagValue"
            }
            elseif ($tagValue -match '\|' -and $tagValue -match '^[A-Z]{2}\d+\|') {
                # Found QRBridge data in another field
                Write-Host "  $tagName : $tagValue" -ForegroundColor Yellow
                Write-Host "    -> FOUND QRBridge data in custom field!" -ForegroundColor Green
                $foundQRBridge = $true
            }
        }
        Write-Host ""
    }
    
    if (-not $foundQRBridge) {
        Write-Host "WARNING: No QRBridge data found!" -ForegroundColor Red
        Write-Host "Make sure a QR code was scanned before taking the photo." -ForegroundColor Yellow
    }
    
} else {
    # Fallback: Use .NET System.Drawing
    Write-Host "Using System.Drawing (limited EXIF support)" -ForegroundColor Yellow
    
    Add-Type -AssemblyName System.Drawing
    $image = [System.Drawing.Image]::FromFile($JpegPath)
    
    Write-Host "File: $(Split-Path $JpegPath -Leaf)"
    Write-Host "Size: $([Math]::Round((Get-Item $JpegPath).Length / 1MB, 2)) MB"
    Write-Host "Dimensions: $($image.Width) x $($image.Height)"
    Write-Host ""
    Write-Host "EXIF Properties:"
    
    foreach ($prop in $image.PropertyItems) {
        $id = "0x{0:X4}" -f $prop.Id
        
        # Try to decode value
        $value = switch ($prop.Type) {
            2 { [System.Text.Encoding]::ASCII.GetString($prop.Value).TrimEnd("`0") }
            7 { [System.Text.Encoding]::UTF8.GetString($prop.Value).TrimEnd("`0") }
            default { "Binary data" }
        }
        
        Write-Host "  $id : $value"
        
        if ($value -match '\|') {
            Write-Host "    -> Possible QRBridge data!" -ForegroundColor Green
        }
    }
    
    $image.Dispose()
}

Write-Host ""
Write-Host "Test complete." -ForegroundColor Cyan