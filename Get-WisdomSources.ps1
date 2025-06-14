# Get-WisdomSources.ps1
# Collects all source files for project knowledge
# Part of the Sources Revolution from Session 61!
# Version: 1.0
# (c) 2025 Claude's Improbably Reliable Software Solutions

param(
    [string]$OutputPath = ".\docs\sources",
    [switch]$IncludeTests = $false
)

# Ensure output directory exists
if (-not (Test-Path $OutputPath)) {
    New-Item -ItemType Directory -Path $OutputPath -Force | Out-Null
}

Write-Host "=== CamBridge Sources Collector v1.0 ===" -ForegroundColor Cyan
Write-Host "Collecting all source files for project knowledge..." -ForegroundColor Yellow
Write-Host ""

# Define source categories
$categories = @{
    "CORE" = @{
        Path = "src\CamBridge.Core"
        Extensions = @("*.cs")
        Output = "SOURCES_CORE.txt"
    }
    "INFRASTRUCTURE" = @{
        Path = "src\CamBridge.Infrastructure" 
        Extensions = @("*.cs")
        Output = "SOURCES_INFRASTRUCTURE.txt"
    }
    "SERVICE" = @{
        Path = "src\CamBridge.Service"
        Extensions = @("*.cs", "*.json")
        Output = "SOURCES_SERVICE.txt"
    }
    "CONFIG" = @{
        Path = "src\CamBridge.Config"
        Extensions = @("*.cs", "*.xaml", "*.xaml.cs")
        Output = "SOURCES_CONFIG.txt"
    }
}

if ($IncludeTests) {
    $categories["TESTS"] = @{
        Path = "tests\CamBridge.Tests"
        Extensions = @("*.cs")
        Output = "SOURCES_TESTS.txt"
    }
}

# Function to add file header
function Add-FileHeader {
    param(
        [string]$FilePath,
        [string]$RelativePath
    )
    
    $separator = "=" * 80
    @"

$separator
FILE: $RelativePath
$separator

"@
}

# Process each category
foreach ($category in $categories.GetEnumerator()) {
    $categoryName = $category.Key
    $config = $category.Value
    
    Write-Host "Processing $categoryName..." -ForegroundColor Green
    
    $outputFile = Join-Path $OutputPath $config.Output
    $content = @()
    
    # Add category header
    $content += @"
# CamBridge Sources - $categoryName
# Generated: $(Get-Date -Format "yyyy-MM-dd HH:mm:ss")
# Version: 0.7.11
# Purpose: Complete source code for project knowledge
# Token-efficient access to all implementations

"@
    
    $fileCount = 0
    $totalLines = 0
    
    # Get all files for this category
    foreach ($extension in $config.Extensions) {
        $files = Get-ChildItem -Path $config.Path -Filter $extension -Recurse -File -ErrorAction SilentlyContinue
        
        foreach ($file in $files) {
            $relativePath = $file.FullName.Substring((Get-Location).Path.Length + 1)
            
            # Skip designer files and generated files
            if ($file.Name -match "\.Designer\.cs$" -or 
                $file.Name -match "\.g\.cs$" -or
                $file.Name -match "\.g\.i\.cs$") {
                continue
            }
            
            # Add file header
            $content += Add-FileHeader -FilePath $file.FullName -RelativePath $relativePath
            
            # Add file content
            $fileContent = Get-Content $file.FullName -Raw
            $content += $fileContent
            
            $fileCount++
            $totalLines += ($fileContent -split "`n").Count
        }
    }
    
    # Write to output file
    $content | Out-File -FilePath $outputFile -Encoding UTF8
    
    Write-Host "  - Files: $fileCount" -ForegroundColor White
    Write-Host "  - Lines: $totalLines" -ForegroundColor White
    Write-Host "  - Output: $($config.Output)" -ForegroundColor White
    Write-Host ""
}

# Create index file
$indexPath = Join-Path $OutputPath "SOURCES_INDEX.md"
@"
# CamBridge Sources Index
Generated: $(Get-Date -Format "yyyy-MM-dd HH:mm:ss")
Version: 0.7.11

## Purpose
These files contain the complete source code of CamBridge for efficient project knowledge access.
This enables token-efficient pattern matching and prevents code duplication.

## Files Generated
"@ | Out-File -FilePath $indexPath -Encoding UTF8

foreach ($category in $categories.GetEnumerator()) {
    $config = $category.Value
    $fileInfo = Get-Item (Join-Path $OutputPath $config.Output) -ErrorAction SilentlyContinue
    if ($fileInfo) {
        $sizeMB = [math]::Round($fileInfo.Length / 1MB, 2)
        "- **$($config.Output)** - $($category.Key) implementation ($sizeMB MB)" | 
            Out-File -FilePath $indexPath -Encoding UTF8 -Append
    }
}

@"

## Usage
Upload these files to project knowledge for:
1. Instant code access without token-heavy requests
2. Pattern matching across entire codebase
3. Preventing accidental code duplication
4. Understanding existing implementations

## Note
These files are preprocessed for optimal project knowledge usage.
Claude can search and reference them efficiently.
"@ | Out-File -FilePath $indexPath -Encoding UTF8 -Append

Write-Host "=== Collection Complete! ===" -ForegroundColor Green
Write-Host "Files saved to: $OutputPath" -ForegroundColor Yellow
Write-Host "Upload these to project knowledge for token-efficient development!" -ForegroundColor Cyan
Write-Host ""
Write-Host "Remember: SOURCES FIRST - The code is already written!" -ForegroundColor Magenta