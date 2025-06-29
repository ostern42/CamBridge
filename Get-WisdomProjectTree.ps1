# Get-WisdomFileTree.ps1
# Enhanced version with automatic insights - Everything WISDOM Claude needs!
# (c) 2025 Claude's Improbably Reliable Software Solutions
#
# ============================================================================
# WISDOM CLAUDE - This is YOUR DAILY DASHBOARD!
# ============================================================================
# Just run: .\Get-WisdomFileTree.ps1
# No parameters needed - it shows EVERYTHING important!
#
# What you get:
# - Recently modified files (know what changed!)
# - Monster files that need refactoring
# - Classes/Interfaces in each file
# - New files added recently
# - Quick stats and warnings
# ============================================================================

param(
    [string]$OutputFile = "FILETREE_WISDOM.md",
    [int]$RecentDays = 7,
    [int]$MonsterFileThreshold = 500
)

# Get version
$version = "?.?.?"
if (Test-Path "Version.props") {
    $content = Get-Content "Version.props" -Raw
    if ($content -match '<VersionPrefix>([^<]+)</VersionPrefix>') {
        $version = $matches[1]
    }
}

# Define what to include
$includePatterns = @(
    "*.cs", "*.csproj", "*.xaml", "*.xaml.cs",
    "*.json", "*.xml", "*.config", "*.props", "*.targets",
    "*.md", "*.txt", "*.sln",
    "*.ps1", "*.bat", "*.cmd"
)

# Folders to exclude
$excludeFolders = @("bin", "obj", ".vs", "packages", ".git", "Deploy", "TestResults")

Write-Host "Scanning CamBridge v$version..." -ForegroundColor Cyan

# Get ALL relevant files with metadata
$allFiles = Get-ChildItem -Path . -Recurse -Include $includePatterns -File -ErrorAction SilentlyContinue |
    Where-Object { 
        $path = $_.FullName
        $excluded = $false
        foreach ($folder in $excludeFolders) {
            if ($path -match "\\$folder\\") {
                $excluded = $true
                break
            }
        }
        -not $excluded
    } |
    ForEach-Object {
        $relativePath = $_.FullName.Replace((Get-Location).Path + "\", "")
        
        # Determine project - FIXED REGEX
        $project = "ROOT"
        $projectType = "Other"
        if ($relativePath -match '^src\\([^\\]+)\\') {
            $project = $matches[1]
            $projectType = "Source"
        } elseif ($relativePath -match '^tests\\([^\\]+)\\') {
            $project = $matches[1]
            $projectType = "Test"
        } elseif ($relativePath -match "^Tools\\") {
            $project = "TOOLS"
            $projectType = "Tools"
        }
        
        # Get line count for code files
        $lineCount = 0
        $classes = @()
        $interfaces = @()
        
        if ($_.Extension -in @(".cs", ".xaml.cs")) {
            $content = Get-Content $_.FullName -Raw -ErrorAction SilentlyContinue
            if ($content) {
                $lineCount = ($content -split "`n").Count
                
                # Find classes and interfaces (simple regex)
                $classes = [regex]::Matches($content, 'public\s+(?:partial\s+)?(?:sealed\s+)?(?:abstract\s+)?class\s+(\w+)') | 
                    ForEach-Object { $_.Groups[1].Value } | 
                    Select-Object -Unique
                    
                $interfaces = [regex]::Matches($content, 'public\s+interface\s+(\w+)') | 
                    ForEach-Object { $_.Groups[1].Value } | 
                    Select-Object -Unique
            }
        }
        
        # Calculate age
        $age = (Get-Date) - $_.LastWriteTime
        $ageCategory = if ($age.TotalHours -lt 24) { "Today" }
                       elseif ($age.TotalDays -lt 2) { "Yesterday" }
                       elseif ($age.TotalDays -lt 7) { "This Week" }
                       elseif ($age.TotalDays -lt 30) { "This Month" }
                       else { "Older" }
        
        [PSCustomObject]@{
            FullPath = $relativePath
            Project = $project
            ProjectType = $projectType
            Directory = Split-Path $relativePath -Parent
            FileName = $_.Name
            Extension = $_.Extension
            Size = $_.Length
            LineCount = $lineCount
            LastModified = $_.LastWriteTime
            Age = $age
            AgeCategory = $ageCategory
            Classes = $classes
            Interfaces = $interfaces
            IsMonster = $lineCount -gt $MonsterFileThreshold
        }
    }

# Build the output
$output = @()
$output += "# WISDOM FILE TREE v$version"
$output += "**Generated**: $(Get-Date -Format 'yyyy-MM-dd HH:mm')  "
$output += "**Total Files**: $($allFiles.Count)  "
$output += "**Your Daily Code Dashboard** - Everything important at a glance!"
$output += ""

# Recent changes section
$output += "## RECENTLY MODIFIED (Last $RecentDays days)"
$output += ""
$recentFiles = $allFiles | Where-Object { $_.Age.TotalDays -le $RecentDays } | Sort-Object LastModified -Descending
if ($recentFiles) {
    $todayFiles = $recentFiles | Where-Object { $_.AgeCategory -eq "Today" }
    $yesterdayFiles = $recentFiles | Where-Object { $_.AgeCategory -eq "Yesterday" }
    $weekFiles = $recentFiles | Where-Object { $_.AgeCategory -eq "This Week" }
    
    if ($todayFiles) {
        $output += "### Today [HOT]"
        $todayFiles | ForEach-Object {
            $markers = @()
            if ($_.IsMonster) { $markers += "MONSTER" }
            if ($_.Classes) { $markers += "Classes: $($_.Classes -join ', ')" }
            $markerStr = if ($markers) { " [$($markers -join '] [')]" } else { "" }
            $output += "- **$($_.FullPath)**$markerStr"
        }
        $output += ""
    }
    
    if ($yesterdayFiles) {
        $output += "### Yesterday"
        $yesterdayFiles | ForEach-Object {
            $output += "- $($_.FullPath)"
        }
        $output += ""
    }
    
    if ($weekFiles) {
        $output += "### This Week"
        $weekFiles | ForEach-Object {
            $output += "- $($_.FullPath) *($('{0:N0}' -f $_.Age.TotalDays) days ago)*"
        }
        $output += ""
    }
} else {
    $output += "*No files modified in the last $RecentDays days*"
    $output += ""
}

# Monster files section
$output += "## MONSTER FILES (Need Refactoring!)"
$output += ""
$monsterFiles = $allFiles | Where-Object { $_.IsMonster } | Sort-Object LineCount -Descending
if ($monsterFiles) {
    $output += "File | Lines | Classes | Status"
    $output += "-----|-------|---------|--------"
    $monsterFiles | ForEach-Object {
        $status = if ($_.LineCount -gt 1000) { "CRITICAL" }
                  elseif ($_.LineCount -gt 700) { "HIGH" }
                  else { "MEDIUM" }
        $classStr = if ($_.Classes) { $_.Classes -join ", " } else { "-" }
        $output += "$($_.FullPath) | **$($_.LineCount)** | $classStr | $status"
    }
    $output += ""
    $output += "**Total Monster Lines**: $(($monsterFiles | Measure-Object -Property LineCount -Sum).Sum) lines to refactor!"
} else {
    $output += "*Great! No files exceed $MonsterFileThreshold lines*"
}
$output += ""

# Quick class/interface finder
$output += "## QUICK CLASS FINDER"
$output += ""
$allClasses = $allFiles | Where-Object { $_.Classes -or $_.Interfaces } | Sort-Object FullPath
$output += "<details>"
$output += "<summary>Click to expand class/interface list</summary>"
$output += ""
$allClasses | ForEach-Object {
    if ($_.Classes -or $_.Interfaces) {
        $output += "**$($_.FullPath)**"
        if ($_.Classes) {
            $output += "  - Classes: $($_.Classes -join ', ')"
        }
        if ($_.Interfaces) {
            $output += "  - Interfaces: $($_.Interfaces -join ', ')"
        }
        $output += ""
    }
}
$output += "</details>"
$output += ""

# Project structure (compact)
$output += "## PROJECT STRUCTURE"
$output += ""
$projectGroups = $allFiles | Group-Object Project | Sort-Object Name
foreach ($group in $projectGroups) {
    $testMarker = if ($group.Group[0].ProjectType -eq "Test") { " [TEST]" } else { "" }
    $output += "### [$($group.Name)]$testMarker"
    
    # Stats for this project
    $projectStats = @{
        Files = $group.Count
        TotalLines = ($group.Group | Measure-Object -Property LineCount -Sum).Sum
        Classes = ($group.Group | ForEach-Object { $_.Classes } | Where-Object { $_ }).Count
        MonsterCount = ($group.Group | Where-Object { $_.IsMonster }).Count
    }
    
    $output += "*$($projectStats.Files) files, $($projectStats.TotalLines) lines, $($projectStats.Classes) classes*"
    if ($projectStats.MonsterCount -gt 0) {
        $output += " **WARNING: $($projectStats.MonsterCount) monster files**"
    }
    $output += ""
    
    # List files with indicators
    $group.Group | Sort-Object FullPath | ForEach-Object {
        $indicators = @()
        if ($_.AgeCategory -eq "Today") { $indicators += "[TODAY]" }
        if ($_.IsMonster) { $indicators += "[MONSTER]" }
        if ($_.Classes) { $indicators += "[HAS-CLASSES]" }
        
        $indicatorStr = if ($indicators) { " $($indicators -join ' ')" } else { "" }
        $output += "- $($_.FullPath)$indicatorStr"
    }
    $output += ""
}

# Summary statistics
$output += "## SUMMARY STATISTICS"
$output += ""
$output += "Metric | Value"
$output += "-------|-------"
$output += "Total Files | **$($allFiles.Count)**"
$output += "Total Lines of Code | **$(($allFiles | Measure-Object -Property LineCount -Sum).Sum)**"
$output += "Files Modified Today | **$(($allFiles | Where-Object { $_.AgeCategory -eq 'Today' }).Count)**"
$output += "Files Modified This Week | **$(($allFiles | Where-Object { $_.Age.TotalDays -le 7 }).Count)**"
$output += "Monster Files (>$MonsterFileThreshold lines) | **$($monsterFiles.Count)**"
$output += "Test Files | **$(($allFiles | Where-Object { $_.ProjectType -eq 'Test' }).Count)**"
$output += "Total Classes Found | **$(($allFiles | ForEach-Object { $_.Classes } | Where-Object { $_ }).Count)**"
$output += "Total Interfaces Found | **$(($allFiles | ForEach-Object { $_.Interfaces } | Where-Object { $_ }).Count)**"
$output += ""

# Quick tips
$output += "## WISDOM TIPS"
$output += ""
$output += "- [TODAY] = Modified today"
$output += "- [MONSTER] = Monster file (>$MonsterFileThreshold lines)"
$output += "- [HAS-CLASSES] = Contains classes/interfaces"
$output += "- [TEST] = Test project"
$output += ""
$output += "**Remember**: Before creating ANY file, search this document first!"
$output += ""
$output += "---"
$output += "*Generated by Get-WisdomFileTree.ps1 - Your daily code dashboard*"

# Save to file
$output | Out-File -FilePath $OutputFile -Encoding UTF8

# Console output with colors
Write-Host "`nSUCCESS: WISDOM File Tree saved to: $OutputFile" -ForegroundColor Green
Write-Host ""
Write-Host "Quick Stats:" -ForegroundColor Cyan
Write-Host "   Files modified today: $(($allFiles | Where-Object { $_.AgeCategory -eq 'Today' }).Count)" -ForegroundColor Yellow
Write-Host "   Monster files: $($monsterFiles.Count)" -ForegroundColor Red
Write-Host "   Total files: $($allFiles.Count)" -ForegroundColor White
Write-Host ""
Write-Host "Tip: Open $OutputFile in VS Code for best viewing experience!" -ForegroundColor DarkGray