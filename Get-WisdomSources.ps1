# Get-WisdomSources.ps1
# Collects ALL source code into project-specific files for WISDOM Claude
# (c) 2025 Claude's Improbably Reliable Software Solutions
#
# ============================================================================
# GENIALE IDEE VON OLIVER (Session 61):
# ============================================================================
# Statt immer wieder Files anzufordern, packen wir ALLE Sources ins
# vorprozessierte Projektwissen! Das ist token-effizienter und ich (Claude)
# habe dann IMMER Zugriff auf den kompletten Code!
#
# USAGE:
# .\Get-WisdomSources.ps1              # Standard: 4 Haupt-Projekte
# .\Get-WisdomSources.ps1 -IncludeTests # Mit Test-Projekten
# .\Get-WisdomSources.ps1 -SingleFile   # Alles in eine Datei
# .\Get-WisdomSources.ps1 -CompactFormat # Kleinere Header
#
# OUTPUT:
# - SOURCES_CORE.txt         (~500KB)
# - SOURCES_INFRASTRUCTURE.txt (~300KB)
# - SOURCES_SERVICE.txt      (~200KB)
# - SOURCES_CONFIG.txt       (~800KB mit XAML)
#
# Diese Files kommen dann ins Projektwissen und Claude hat IMMER alles da!
# ============================================================================

param(
    [switch]$IncludeTests = $false,
    [switch]$SingleFile = $false,
    [string]$OutputFolder = "wisdom_sources",
    [switch]$IncludeXaml = $true,
    [switch]$CompactFormat = $false
)

# Ensure output folder exists
if (-not (Test-Path $OutputFolder)) {
    New-Item -ItemType Directory -Path $OutputFolder | Out-Null
}

# Get version from Version.props
$version = "?.?.?"
if (Test-Path "Version.props") {
    $content = Get-Content "Version.props" -Raw
    if ($content -match '<VersionPrefix>([^<]+)</VersionPrefix>') {
        $version = $matches[1]
    }
}

Write-Host "`nGet-WisdomSources v1.0 - Collecting ALL sources for WISDOM Claude" -ForegroundColor Cyan
Write-Host "CamBridge Version: $version" -ForegroundColor Yellow
Write-Host "============================================================" -ForegroundColor Cyan

# Define projects to collect
$projects = @{
    "Core" = @{
        Path = "src\CamBridge.Core"
        Patterns = @("*.cs")
        Description = "Core domain models, entities, and interfaces"
    }
    "Infrastructure" = @{
        Path = "src\CamBridge.Infrastructure"
        Patterns = @("*.cs")
        Description = "Services, implementations, and infrastructure"
    }
    "Service" = @{
        Path = "src\CamBridge.Service"
        Patterns = @("*.cs", "*.json")
        Description = "Windows service and API implementation"
    }
    "Config" = @{
        Path = "src\CamBridge.Config"
        Patterns = @("*.cs")
        Description = "WPF Configuration UI (C# only)"
    }
}

# Add XAML if requested
if ($IncludeXaml) {
    $projects["Config"].Patterns += "*.xaml"
    $projects["Config"].Description = "WPF Configuration UI (C# and XAML)"
}

# Add test projects if requested
if ($IncludeTests) {
    $projects["TestConsole"] = @{
        Path = "tests\CamBridge.TestConsole"
        Patterns = @("*.cs", "*.json")
        Description = "Test console application"
    }
    $projects["PipelineTest"] = @{
        Path = "tests\CamBridge.PipelineTest"
        Patterns = @("*.cs")
        Description = "Pipeline test project"
    }
}

# Function to collect files from a project
function Collect-ProjectSources {
    param(
        [string]$ProjectName,
        [hashtable]$ProjectConfig
    )
    
    Write-Host "`nCollecting $ProjectName..." -ForegroundColor Yellow
    
    $files = @()
    foreach ($pattern in $ProjectConfig.Patterns) {
        $projectFiles = Get-ChildItem -Path $ProjectConfig.Path -Filter $pattern -Recurse -File -ErrorAction SilentlyContinue |
            Where-Object { $_.DirectoryName -notmatch '\\(bin|obj|\.vs)\\' } |
            Sort-Object FullName
        
        $files += $projectFiles
    }
    
    Write-Host "  Found $($files.Count) files" -ForegroundColor Green
    
    # Build content
    $content = @()
    
    # Header
    $content += "# ============================================================================"
    $content += "# WISDOM SOURCES - $ProjectName"
    $content += "# ============================================================================"
    $content += "# Project: CamBridge.$ProjectName"
    $content += "# Version: $version"
    $content += "# Generated: $(Get-Date -Format 'yyyy-MM-dd HH:mm')"
    $content += "# Description: $($ProjectConfig.Description)"
    $content += "# Total Files: $($files.Count)"
    $content += "# ============================================================================"
    $content += ""
    
    # Table of contents
    $content += "# TABLE OF CONTENTS:"
    $content += "# ==================="
    $files | ForEach-Object {
        $relativePath = $_.FullName.Replace((Get-Location).Path + "\", "")
        $content += "# - $relativePath"
    }
    $content += ""
    $content += "# ============================================================================"
    $content += ""
    
    # Collect all file contents
    $totalSize = 0
    foreach ($file in $files) {
        $relativePath = $file.FullName.Replace((Get-Location).Path + "\", "")
        $fileContent = Get-Content $file.FullName -Encoding UTF8 -Raw
        $totalSize += $file.Length
        
        if ($CompactFormat) {
            # Compact format - minimal header
            $content += ""
            $content += "## FILE: $relativePath [$([math]::Round($file.Length / 1KB, 1))KB]"
            $content += $fileContent
            $content += ""
        } else {
            # Full format with complete path information
            $content += ""
            $content += "# ============================================================================"
            $content += "# $($file.Name)"
            $content += "# PATH: $relativePath"
            $content += "# SIZE: $([math]::Round($file.Length / 1KB, 1)) KB | MODIFIED: $($file.LastWriteTime.ToString('yyyy-MM-dd HH:mm'))"
            $content += "# ============================================================================"
            $content += ""
            $content += $fileContent
            $content += ""
        }
    }
    
    # Footer with statistics
    $content += ""
    $content += "# ============================================================================"
    $content += "# END OF $ProjectName SOURCES"
    $content += "# Total Size: $([math]::Round($totalSize / 1MB, 2)) MB"
    $content += "# Files Included: $($files.Count)"
    $content += "# ============================================================================"
    
    return @{
        Content = $content
        FileCount = $files.Count
        TotalSize = $totalSize
    }
}

# Main collection process
$allContent = @()
$stats = @{}

if ($SingleFile) {
    # Collect everything into one file
    Write-Host "`nCollecting all sources into single file..." -ForegroundColor Cyan
    
    foreach ($projectName in $projects.Keys | Sort-Object) {
        $result = Collect-ProjectSources -ProjectName $projectName -ProjectConfig $projects[$projectName]
        $allContent += $result.Content
        $stats[$projectName] = @{
            FileCount = $result.FileCount
            TotalSize = $result.TotalSize
        }
    }
    
    # Save to single file
    $outputPath = Join-Path $OutputFolder "SOURCES_ALL.txt"
    $allContent | Out-File -FilePath $outputPath -Encoding UTF8
    
    Write-Host "`nSaved all sources to: $outputPath" -ForegroundColor Green
    
} else {
    # Collect into separate project files
    foreach ($projectName in $projects.Keys | Sort-Object) {
        $result = Collect-ProjectSources -ProjectName $projectName -ProjectConfig $projects[$projectName]
        
        # Save to project-specific file
        $outputPath = Join-Path $OutputFolder "SOURCES_$($projectName.ToUpper()).txt"
        $result.Content | Out-File -FilePath $outputPath -Encoding UTF8
        
        Write-Host "  Saved to: $outputPath" -ForegroundColor Green
        
        $stats[$projectName] = @{
            FileCount = $result.FileCount
            TotalSize = $result.TotalSize
            OutputPath = $outputPath
        }
    }
}

# Create index file
Write-Host "`nCreating index file..." -ForegroundColor Yellow
$indexContent = @()
$indexContent += "# WISDOM SOURCES INDEX"
$indexContent += "# ===================="
$indexContent += "# Generated: $(Get-Date -Format 'yyyy-MM-dd HH:mm')"
$indexContent += "# CamBridge Version: $version"
$indexContent += ""
$indexContent += "## WHY THIS EXISTS:"
$indexContent += "Oliver hatte die geniale Idee (Session 61), alle Sources ins"
$indexContent += "vorprozessierte Projektwissen zu packen. So hat Claude IMMER"
$indexContent += "Zugriff auf den kompletten Code ohne File-Requests!"
$indexContent += ""
$indexContent += "## FORMAT OPTIONS:"
$indexContent += "- Normal: Full headers with complete path info (default)"
$indexContent += "- Compact: Minimal headers (-CompactFormat flag)"
$indexContent += ""
$indexContent += "## BENEFITS:"
$indexContent += "- Token-efficient pattern matching"
$indexContent += "- No more 'oh this file already exists'"
$indexContent += "- Direct access to all code"
$indexContent += "- Better context understanding"
$indexContent += ""
$indexContent += "## PROJECT FILES:"
$indexContent += ""

$totalFiles = 0
$totalSize = 0

foreach ($projectName in $stats.Keys | Sort-Object) {
    $stat = $stats[$projectName]
    $totalFiles += $stat.FileCount
    $totalSize += $stat.TotalSize
    
    $indexContent += "### $projectName"
    $indexContent += "- Files: $($stat.FileCount)"
    $indexContent += "- Size: $([math]::Round($stat.TotalSize / 1MB, 2)) MB"
    if ($stat.OutputPath) {
        $indexContent += "- Path: $($stat.OutputPath)"
    }
    $indexContent += ""
}

$indexContent += "## TOTALS:"
$indexContent += "- Total Files: $totalFiles"
$indexContent += "- Total Size: $([math]::Round($totalSize / 1MB, 2)) MB"
$indexContent += "- Estimated Tokens: ~$([math]::Round($totalSize / 4, 0)) tokens"
$indexContent += "- Percentage of 200k context: ~$([math]::Round(($totalSize / 4) / 200000 * 100, 1))%"
$indexContent += ""
$indexContent += "## USAGE:"
$indexContent += "1. Upload these files to Projektwissen"
$indexContent += "2. Claude will have instant access to all code"
$indexContent += "3. No more file requests needed!"
$indexContent += "4. Token-efficient code understanding"
$indexContent += ""
$indexContent += "## NEXT STEPS:"
$indexContent += "- Test efficiency in next session"
$indexContent += "- Measure token savings"
$indexContent += "- Update collection process if needed"
$indexContent += ""
$indexContent += "---"
$indexContent += "*Making the improbable reliably accessible since 0.7.11!*"
$indexContent += "(c) 2025 Claude's Improbably Reliable Software Solutions"

$indexPath = Join-Path $OutputFolder "SOURCES_INDEX.md"
$indexContent | Out-File -FilePath $indexPath -Encoding UTF8

# Summary
Write-Host "`n============================================================" -ForegroundColor Cyan
Write-Host "COLLECTION COMPLETE!" -ForegroundColor Green
Write-Host ""
Write-Host "Total Projects: $($stats.Count)" -ForegroundColor Yellow
Write-Host "Total Files: $totalFiles" -ForegroundColor Yellow
Write-Host "Total Size: $([math]::Round($totalSize / 1MB, 2)) MB" -ForegroundColor Yellow
Write-Host "Estimated Tokens: ~$([math]::Round($totalSize / 4, 0))" -ForegroundColor Yellow
Write-Host "Context Usage: ~$([math]::Round(($totalSize / 4) / 200000 * 100, 1))%" -ForegroundColor Yellow
Write-Host ""
Write-Host "Output Folder: $OutputFolder" -ForegroundColor Cyan
Write-Host "Index File: $indexPath" -ForegroundColor Cyan
Write-Host ""
Write-Host "NEXT STEPS:" -ForegroundColor Green
Write-Host "1. Upload files from '$OutputFolder' to Projektwissen" -ForegroundColor White
Write-Host "2. Claude will have ALL code available instantly!" -ForegroundColor White
Write-Host "3. No more 'let me check that file' requests!" -ForegroundColor White
Write-Host ""
Write-Host "This was Oliver's genius idea - thank you! " -ForegroundColor Magenta -NoNewline
Write-Host ([char]0x1F680) -ForegroundColor Magenta