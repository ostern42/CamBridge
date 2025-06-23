# Get-WisdomSources-Dynamic.ps1
# Dynamic recursive source collector with metadata
# Version: 2.0 - The flexible version!
# (c) 2025 Claude's Improbably Reliable Software Solutions

param(
    [string]$OutputPath = ".\docs\sources",
    [switch]$IncludeTests = $false,
    [switch]$IncludeGenerated = $false,
    [switch]$Verbose = $false
)

# Ensure output directory exists
if (-not (Test-Path $OutputPath)) {
    New-Item -ItemType Directory -Path $OutputPath -Force | Out-Null
}

Write-Host "=== CamBridge Dynamic Sources Collector v2.0 ===" -ForegroundColor Cyan
Write-Host "Recursively collecting all source files with metadata..." -ForegroundColor Yellow
Write-Host ""

# Define what to collect (extensible!)
$relevantExtensions = @(
    "*.cs",
    "*.csproj",
    "*.xaml",
    "*.json",
    "*.xml",
    "*.config",
    "*.props",
    "*.targets"
)

# Define what to exclude
$excludePatterns = @(
    "*.Designer.cs",
    "*.g.cs",
    "*.g.i.cs",
    "AssemblyInfo.cs",
    "GlobalUsings.cs"
)

if (-not $IncludeGenerated) {
    $excludePatterns += @("*.Generated.cs", "*.g.xaml")
}

# Define exclude folders
$excludeFolders = @(
    "bin",
    "obj",
    ".vs",
    "packages",
    "TestResults",
    ".git"
)

# Main projects mapping
$projects = @{
    "CORE" = @{
        RootPath = "src\CamBridge.Core"
        Output = "SOURCES_CORE.txt"
        Description = "Domain models, entities, value objects, interfaces"
    }
    "INFRASTRUCTURE" = @{
        RootPath = "src\CamBridge.Infrastructure"
        Output = "SOURCES_INFRASTRUCTURE.txt"
        Description = "Services, implementations, business logic"
    }
    "SERVICE" = @{
        RootPath = "src\CamBridge.Service"
        Output = "SOURCES_SERVICE.txt"
        Description = "Windows service, API endpoints, worker"
    }
    "CONFIG" = @{
        RootPath = "src\CamBridge.Config"
        Output = "SOURCES_CONFIG.txt"
        Description = "WPF configuration tool, UI, ViewModels"
    }
}

# Add QRBridge if it exists
if (Test-Path "src\CamBridge.QRBridge") {
    $projects["QRBRIDGE"] = @{
        RootPath = "src\CamBridge.QRBridge"
        Output = "SOURCES_QRBRIDGE.txt"
        Description = "QR code generation tool"
    }
}

if ($IncludeTests -and (Test-Path "tests")) {
    $projects["TESTS"] = @{
        RootPath = "tests"
        Output = "SOURCES_TESTS.txt"
        Description = "Unit tests, integration tests"
    }
}

# Function to check if file should be excluded
function Should-ExcludeFile {
    param([System.IO.FileInfo]$File)
    
    foreach ($pattern in $excludePatterns) {
        if ($File.Name -like $pattern) {
            return $true
        }
    }
    return $false
}

# Function to check if folder should be excluded
function Should-ExcludeFolder {
    param([string]$FolderName)
    
    foreach ($exclude in $excludeFolders) {
        if ($FolderName -eq $exclude) {
            return $true
        }
    }
    return $false
}

# Function to get relative path with proper formatting
function Get-FormattedRelativePath {
    param(
        [string]$FullPath,
        [string]$BasePath
    )
    
    $relative = $FullPath.Substring($BasePath.Length).TrimStart('\')
    return $relative.Replace('\', '/')
}

# Function to format file size
function Format-FileSize {
    param([long]$Size)
    
    if ($Size -gt 1MB) {
        return "{0:N2} MB" -f ($Size / 1MB)
    } elseif ($Size -gt 1KB) {
        return "{0:N2} KB" -f ($Size / 1KB)
    } else {
        return "$Size bytes"
    }
}

# Function to create directory tree structure
function Get-DirectoryTree {
    param(
        [string]$Path,
        [string]$Prefix = "",
        [hashtable]$FileStats
    )
    
    $items = Get-ChildItem -Path $Path -Directory | Where-Object { -not (Should-ExcludeFolder $_.Name) } | Sort-Object Name
    $tree = @()
    
    foreach ($item in $items) {
        $fileCount = ($FileStats.Keys | Where-Object { $_ -like "$($item.FullName)\*" }).Count
        if ($fileCount -gt 0) {
            $tree += "$Prefix+-- $($item.Name)/ ($fileCount files)"
            $tree += Get-DirectoryTree -Path $item.FullName -Prefix "$Prefix|   " -FileStats $FileStats
        }
    }
    
    return $tree
}

# Process each project
$globalStats = @{
    TotalFiles = 0
    TotalLines = 0
    TotalSize = 0
}

foreach ($project in $projects.GetEnumerator()) {
    $projectName = $project.Key
    $config = $project.Value
    
    if (-not (Test-Path $config.RootPath)) {
        Write-Host "Skipping $projectName - path not found: $($config.RootPath)" -ForegroundColor Yellow
        continue
    }
    
    Write-Host "Processing $projectName..." -ForegroundColor Green
    Write-Host "  Root: $($config.RootPath)" -ForegroundColor Gray
    Write-Host "  Desc: $($config.Description)" -ForegroundColor Gray
    
    $outputFile = Join-Path $OutputPath $config.Output
    $content = @()
    $fileStats = @{}
    
    # Add header
    $content += @"
################################################################################
# CamBridge Sources - $projectName
# Generated: $(Get-Date -Format "yyyy-MM-dd HH:mm:ss")
# Root Path: $($config.RootPath)
# Description: $($config.Description)
################################################################################

"@
    
    # Collect all relevant files recursively
    $files = @()
    foreach ($extension in $relevantExtensions) {
        $found = Get-ChildItem -Path $config.RootPath -Filter $extension -Recurse -File -ErrorAction SilentlyContinue |
            Where-Object { 
                $folder = Split-Path $_.Directory -Leaf
                -not (Should-ExcludeFolder $folder) -and 
                -not (Should-ExcludeFile $_) 
            }
        $files += $found
    }
    
    # Sort files by relative path
    $basePath = (Get-Item $config.RootPath).FullName
    $files = $files | Sort-Object { Get-FormattedRelativePath $_.FullName $basePath }
    
    # Collect file statistics
    $projectStats = @{
        FileCount = 0
        TotalLines = 0
        TotalSize = 0
        FilesByExtension = @{}
    }
    
    foreach ($file in $files) {
        $relativePath = Get-FormattedRelativePath $file.FullName $basePath
        $fileContent = Get-Content $file.FullName -Raw -ErrorAction SilentlyContinue
        
        if ($fileContent) {
            $lineCount = ($fileContent -split "`n").Count
            $projectStats.TotalLines += $lineCount
            $projectStats.TotalSize += $file.Length
            $projectStats.FileCount++
            
            # Track by extension
            $ext = $file.Extension.ToLower()
            if (-not $projectStats.FilesByExtension.ContainsKey($ext)) {
                $projectStats.FilesByExtension[$ext] = 0
            }
            $projectStats.FilesByExtension[$ext]++
            
            # Store for directory tree
            $fileStats[$file.FullName] = @{
                Lines = $lineCount
                Size = $file.Length
            }
        }
    }
    
    # Add project summary
    $content += @"
## PROJECT SUMMARY
Total Files: $($projectStats.FileCount)
Total Lines: $($projectStats.TotalLines)
Total Size: $(Format-FileSize $projectStats.TotalSize)

Files by Type:
"@
    foreach ($ext in $projectStats.FilesByExtension.GetEnumerator() | Sort-Object Name) {
        $content += "  $($ext.Key): $($ext.Value) files"
    }
    
    # Add directory structure
    $content += @"

## DIRECTORY STRUCTURE
$($config.RootPath)/
"@
    $tree = Get-DirectoryTree -Path $config.RootPath -Prefix "" -FileStats $fileStats
    $content += $tree
    
    $content += @"

## FILE CONTENTS
"@
    
    # Add each file with metadata
    foreach ($file in $files) {
        $relativePath = Get-FormattedRelativePath $file.FullName $basePath
        $fileContent = Get-Content $file.FullName -Raw -ErrorAction SilentlyContinue
        
        if ($fileContent) {
            $lineCount = ($fileContent -split "`n").Count
            
            # File header with metadata
            $content += @"

================================================================================
FILE: $relativePath
--------------------------------------------------------------------------------
Size: $(Format-FileSize $file.Length) | Lines: $lineCount | Modified: $($file.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss"))
================================================================================

"@
            $content += $fileContent
        }
    }
    
    # Write to output file
    $content | Out-File -FilePath $outputFile -Encoding UTF8
    
    # Update global stats
    $globalStats.TotalFiles += $projectStats.FileCount
    $globalStats.TotalLines += $projectStats.TotalLines
    $globalStats.TotalSize += $projectStats.TotalSize
    
    # Display project stats
    Write-Host "  Files: $($projectStats.FileCount)" -ForegroundColor White
    Write-Host "  Lines: $($projectStats.TotalLines)" -ForegroundColor White
    Write-Host "  Size: $(Format-FileSize $projectStats.TotalSize)" -ForegroundColor White
    Write-Host "  Output: $($config.Output)" -ForegroundColor White
    Write-Host ""
}

# Create enhanced index file
$indexPath = Join-Path $OutputPath "SOURCES_INDEX.md"
@"
# CamBridge Sources Index
Generated: $(Get-Date -Format "yyyy-MM-dd HH:mm:ss")
Version: Dynamic Collection v2.0

## Summary
- **Total Files**: $($globalStats.TotalFiles)
- **Total Lines**: $($globalStats.TotalLines)
- **Total Size**: $(Format-FileSize $globalStats.TotalSize)

## Purpose
These files contain the complete source code of CamBridge for efficient project knowledge access.
All files are collected dynamically with full metadata for better navigation and understanding.

## Files Generated
"@ | Out-File -FilePath $indexPath -Encoding UTF8

foreach ($project in $projects.GetEnumerator()) {
    $config = $project.Value
    $fileInfo = Get-Item (Join-Path $OutputPath $config.Output) -ErrorAction SilentlyContinue
    if ($fileInfo) {
        $sizeMB = [math]::Round($fileInfo.Length / 1MB, 2)
        "- **$($config.Output)** - $($project.Key): $($config.Description) ($sizeMB MB)" | 
            Out-File -FilePath $indexPath -Encoding UTF8 -Append
    }
}

@"

## Features of Dynamic Collection
- Recursive file discovery (no hardcoded paths)
- Complete metadata (size, lines, modification date)
- Directory structure visualization
- File type statistics
- Exclusion of generated files
- Automatic detection of new projects

## Usage
Upload these files to project knowledge for:
1. Complete codebase access without token overhead
2. Understanding project structure at a glance
3. Finding files by metadata (recent changes, size)
4. Navigating complex directory hierarchies
5. Preventing phantom property syndrome!

## Collection Settings
- Extensions: $($relevantExtensions -join ", ")
- Excluded: $($excludePatterns -join ", ")
- Include Generated: $($IncludeGenerated)
- Include Tests: $($IncludeTests)
"@ | Out-File -FilePath $indexPath -Encoding UTF8 -Append

Write-Host "=== Collection Complete! ===" -ForegroundColor Green
Write-Host "Total Files: $($globalStats.TotalFiles)" -ForegroundColor Yellow
Write-Host "Total Lines: $($globalStats.TotalLines)" -ForegroundColor Yellow
Write-Host "Total Size: $(Format-FileSize $globalStats.TotalSize)" -ForegroundColor Yellow
Write-Host ""
Write-Host "Files saved to: $OutputPath" -ForegroundColor Cyan
Write-Host ""
Write-Host "Upload these to project knowledge for:" -ForegroundColor Magenta
Write-Host "  - Token-efficient development" -ForegroundColor White
Write-Host "  - No more phantom properties!" -ForegroundColor White
Write-Host "  - Complete codebase understanding" -ForegroundColor White

if ($Verbose) {
    Write-Host ""
    Write-Host "Verbose: Extensions searched: $($relevantExtensions -join ', ')" -ForegroundColor DarkGray
    Write-Host "Verbose: Patterns excluded: $($excludePatterns -join ', ')" -ForegroundColor DarkGray
}