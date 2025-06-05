# Get-WisdomFileTree.ps1
# Complete file tree for WISDOM Claude - EVERY file matters!
# © 2025 Claude's Improbably Reliable Software Solutions
#
# ============================================================================
# AN DEN NÄCHSTEN WISDOM CLAUDE:
# ============================================================================
# PROBLEM: Wir erstellen oft Dateien die schon existieren (z.B. Program.cs)!
# LÖSUNG: Dieses Skript zeigt ALLE Dateien im Projekt!
#
# VERWENDUNG:
# - VOR dem Erstellen einer neuen Datei: Führe dieses Skript aus!
# - Prüfe ob die Datei schon existiert (besonders Program.cs, App.xaml, etc.)
# - Nutze IMMER den vollen Pfad (src\CamBridge.Service\Program.cs)
#
# DIE 3 MODI:
# - Minimal: Nur Pfade (schnelle Referenz)
# - Compact: Nach Projekt gruppiert (Standard)
# - Detailed: Mit Größen + Duplikat-Check!
#
# TIPP: ".\check-dupes.ps1" zeigt alle doppelten Dateinamen!
# ============================================================================

param(
    [ValidateSet("Minimal", "Compact", "Detailed")]
    [string]$Mode = "Compact",
    [string]$OutputFile = $null,
    [switch]$IncludeTestData = $false
)

# Set output file based on mode if not specified
if (-not $OutputFile) {
    $OutputFile = switch ($Mode) {
        "Minimal" { "FILETREE_MINIMAL.txt" }
        "Compact" { "FILETREE_COMPACT.txt" }
        "Detailed" { "FILETREE_DETAILED.txt" }
    }
}

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
    "*.ps1", "*.bat", "*.cmd",
    "*.exe", "*.dll"
)

# Folders to exclude
$excludeFolders = @("bin", "obj", ".vs", "packages", ".git", "Deploy")

# Get ALL relevant files
Write-Host "Scanning CamBridge v$version file structure..." -ForegroundColor Cyan

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
        
        # Determine project
        $project = "ROOT"
        if ($relativePath -match "^src\\([^\\]+)\\") {
            $project = $matches[1]
        } elseif ($relativePath -match "^tests\\([^\\]+)\\") {
            $project = $matches[1] + " [TEST]"
        } elseif ($relativePath -match "^Tools\\") {
            $project = "TOOLS"
        } elseif ($relativePath -match "^TestImages\\" -and $IncludeTestData) {
            $project = "TESTDATA"
        }
        
        [PSCustomObject]@{
            FullPath = $relativePath
            Project = $project
            Directory = Split-Path $relativePath -Parent
            FileName = $_.Name
            Extension = $_.Extension
            Size = $_.Length
            LastModified = $_.LastWriteTime
        }
    } |
    Sort-Object Project, FullPath

# Build output
$output = @()

switch ($Mode) {
    "Minimal" {
        # Header for next WISDOM Claude
        $output += "# HEY WISDOM CLAUDE - LIES DAS HIER!"
        $output += "# ==================================="
        $output += "# Du bist dabei eine Datei zu erstellen? STOPP!"
        $output += "# "
        $output += "# Diese Liste zeigt ALLE $($allFiles.Count) Dateien im Projekt."
        $output += "# Program.cs existiert z.B. 5x in verschiedenen Projekten!"
        $output += "# "
        $output += "# BEVOR du IRGENDEINE Datei erstellst:"
        $output += "# 1. Suche hier ob sie schon existiert (Ctrl+F)"
        $output += "# 2. Wenn ja: UPDATE die existierende Datei"
        $output += "# 3. Wenn nein: Nutze den VOLLEN Pfad beim Erstellen"
        $output += "# "
        $output += "# WICHTIG: Unsere Dateien haben IMMER diesen Header:"
        $output += "# // src\CamBridge.Config\Views\AboutPage.xaml.cs"
        $output += "# // Version: 0.5.33"
        $output += "# // Fixed: Simplified easter egg - shows Vogon poetry on 5 clicks"
        $output += "# "
        $output += "# Der Pfad im Header muss EXAKT mit dem Pfad hier übereinstimmen!"
        $output += "# "
        $output += "# Häufige Fallen:"
        $output += "# - Program.cs (in Service, QRBridge, Tests...)"
        $output += "# - App.xaml.cs (Config Projekt)"
        $output += "# - Constants.cs (Core und QRBridge)"
        $output += "# "
        $output += "# POWERSHELL ONE-LINER FÜR DATEIEN:"
        $output += "# Wenn du mehrere Files brauchst, gib dem User IMMER einen One-Liner!"
        $output += "# Beispiel:"
        $output += "# @('src\CamBridge.Core\Entities\PatientInfo.cs','src\CamBridge.Core\Entities\StudyInfo.cs') | ForEach-Object { echo `"===== FILE: `$_ =====`"; Get-Content `$_ -Encoding UTF8; echo `"`" } | Out-File -FilePath analysis.txt -Encoding UTF8"
        $output += "# "
        $output += "# Generated: $(Get-Date -Format 'yyyy-MM-dd HH:mm')"
        $output += "# ===================================`n"
        
        # Just paths
        $allFiles | ForEach-Object {
            $output += $_.FullPath
        }
    }
    
    "Compact" {
        # Header for next WISDOM Claude
        $output += "# AN MEIN ZUKÜNFTIGES ICH: WISDOM CLAUDE"
        $output += "# ======================================="
        $output += "# "
        $output += "# Dies ist deine Projekt-Struktur-Referenz!"
        $output += "# "
        $output += "# WARUM DU DAS BRAUCHST:"
        $output += "# Wir haben schon mehrfach Dateien doppelt erstellt."
        $output += "# Besonders Program.cs - die gibt's in fast jedem Projekt!"
        $output += "# "
        $output += "# SO NUTZT DU DAS:"
        $output += "# 1. Vor JEDER Datei-Erstellung hier nachschauen"
        $output += "# 2. Die Dateien sind nach [PROJEKT] gruppiert"
        $output += "# 3. Nutze IMMER den vollen Pfad aus dieser Liste"
        $output += "# "
        $output += "# UNSER HEADER-STANDARD (seit v0.5.27):"
        $output += "# Jede Datei beginnt mit:"
        $output += "# // src\CamBridge.Service\Program.cs"
        $output += "# // Version: 0.5.33"
        $output += "# // Description: Windows service entry point"
        $output += "# "
        $output += "# Der Pfad MUSS mit dieser Liste übereinstimmen!"
        $output += "# "
        $output += "# MERKE DIR:"
        $output += "# - [CamBridge.Service] und [CamBridge.QRBridge] haben BEIDE Program.cs!"
        $output += "# - Config hat die ganzen XAML Views"
        $output += "# - Core hat die Entities und Interfaces"
        $output += "# - Infrastructure hat die Services"
        $output += "# "
        $output += "# POWERSHELL ONE-LINER TRICK:"
        $output += "# User mag One-Liner! Wenn du Files brauchst:"
        $output += "# @('file1.cs','file2.cs','file3.cs') | %{ echo `"=== `$_ ===`"; cat `$_ } > output.txt"
        $output += "# "
        $output += "# Projekte in dieser Liste:"
        $allFiles | Select-Object -ExpandProperty Project -Unique | ForEach-Object {
            $output += "#   - $_"
        }
        $output += "#"
        $output += "# Total files: $($allFiles.Count)"
        $output += ""
        
        # Group by project
        $allFiles | Group-Object Project | ForEach-Object {
            $output += "## [$($_.Name)]"
            $_.Group | ForEach-Object {
                $output += "$($_.FullPath)"
            }
            $output += ""
        }
    }
    
    "Detailed" {
        # Header for next WISDOM Claude
        $output += "# WISDOM CLAUDE - DEIN MASTER FILE INVENTORY!"
        $output += "# ==========================================="
        $output += "# "
        $output += "# Hallo zukünftiger Ich! Das hier ist WICHTIG!"
        $output += "# "
        $output += "# DAS PROBLEM:"
        $output += "# Du erstellst manchmal Dateien die schon existieren."
        $output += "# Letzte Session: Program.cs in QRBridge erstellt - gab's schon!"
        $output += "# "
        $output += "# DIE LÖSUNG:"
        $output += "# Diese Liste zeigt ALLE Dateien mit Größe und Datum."
        $output += "# Ganz unten: DUPLICATE CHECK - zeigt alle mehrfachen Dateinamen!"
        $output += "# "
        $output += "# HEADER-KONVENTION (WICHTIG!):"
        $output += "# Alle unsere Dateien starten mit diesem Header:"
        $output += "# "
        $output += "# // src\CamBridge.Infrastructure\Services\ExifToolReader.cs"
        $output += "# // Version: 0.5.32"
        $output += "# // Fixed: Windows-1252 encoding for Ricoh camera"
        $output += "# "
        $output += "# -> Der Pfad im Header = Der Pfad in dieser Liste!"
        $output += "# -> Version = Aktuelle Version beim Erstellen/Aendern"
        $output += "# -> Kommentar = Was wurde gemacht"
        $output += "# "
        $output += "# WORKFLOW:"
        $output += "# 1. Neue Datei geplant? → Suche erst hier (Ctrl+F)"
        $output += "# 2. Existiert schon? → UPDATE statt neu erstellen"
        $output += "# 3. Wirklich neu? → Nutze exakt den Pfad-Style von hier"
        $output += "# 4. Files sammeln? → PowerShell One-Liner für User!"
        $output += "# "
        $output += "# POWERSHELL ONE-LINER PATTERN:"
        $output += "# Der User liebt diese One-Liner zum File-Sammeln:"
        $output += '#   @("src\path\file1.cs","src\path\file2.cs") | ForEach-Object {'
        $output += '#     echo "===== FILE: $_ ====="; Get-Content $_ -Encoding UTF8; echo ""'
        $output += '#   } | Out-File -FilePath collected_files.txt -Encoding UTF8'
        $output += "# "
        $output += "# Die Pfade nimmst du direkt aus dieser Liste!"
        $output += "# "
        $output += "# TIPP: Spring direkt zum 'DUPLICATE FILENAME CHECK' am Ende!"
        $output += "# Da siehst du sofort welche Namen mehrfach vorkommen."
        $output += "# "
        $output += "# Generated: $(Get-Date -Format 'yyyy-MM-dd HH:mm') für v$version"
        $output += "# "
        
        # Some statistics
        $stats = @{
            TotalSize = ($allFiles | Measure-Object -Property Size -Sum).Sum
            ByExtension = $allFiles | Group-Object Extension | Sort-Object Count -Descending | Select-Object -First 5
            LargestFiles = $allFiles | Sort-Object Size -Descending | Select-Object -First 5
        }
        
        $output += "#   Total size: $([math]::Round($stats.TotalSize / 1MB, 2)) MB"
        $output += "#   Most common types: $($stats.ByExtension.Name -join ', ')"
        $output += "#"
        $output += "# Total files: $($allFiles.Count)"
        $output += ""
        
        # Detailed listing
        $allFiles | Group-Object Project | ForEach-Object {
            $output += "## [$($_.Name)] - $($_.Count) files"
            $output += ""
            
            $_.Group | ForEach-Object {
                $size = if ($_.Size -lt 1KB) { 
                    "{0,6} B" -f $_.Size 
                } elseif ($_.Size -lt 1MB) { 
                    "{0,6:N1}KB" -f ($_.Size / 1KB)
                } else { 
                    "{0,6:N1}MB" -f ($_.Size / 1MB)
                }
                
                $output += "{0,-60} | {1} | {2:yyyy-MM-dd HH:mm}" -f $_.FullPath, $size, $_.LastModified
            }
            $output += ""
        }
        
        # Duplicate name check
        $output += "## DUPLICATE FILENAME CHECK"
        $dupes = $allFiles | Group-Object FileName | Where-Object { $_.Count -gt 1 } | Sort-Object Count -Descending
        if ($dupes) {
            $output += "# WARNING: These filenames exist in multiple locations!"
            $dupes | ForEach-Object {
                $output += ""
                $output += "### $($_.Name) - Found $($_.Count) times:"
                $_.Group | ForEach-Object {
                    $output += "  - $($_.FullPath)"
                }
            }
        } else {
            $output += "# Good news: No duplicate filenames found!"
        }
    }
}

# Save to file
$output | Out-File -FilePath $OutputFile -Encoding UTF8

# Console summary
Write-Host "`nFile tree saved to: $OutputFile" -ForegroundColor Green
Write-Host "Mode: $Mode" -ForegroundColor Yellow
Write-Host "Total files indexed: $($allFiles.Count)" -ForegroundColor Cyan

# Show duplicate warning if any
$dupes = $allFiles | Group-Object FileName | Where-Object { $_.Count -gt 1 }
if ($dupes) {
    Write-Host "`nWARNING: $($dupes.Count) duplicate filenames found!" -ForegroundColor Red
    Write-Host "Check detailed mode for full list." -ForegroundColor Yellow
}

# Quick usage hint
Write-Host "`nUsage:" -ForegroundColor DarkGray
Write-Host "  .\Get-WisdomFileTree.ps1 -Mode Minimal   # Just paths" -ForegroundColor DarkGray
Write-Host "  .\Get-WisdomFileTree.ps1 -Mode Compact   # Organized by project" -ForegroundColor DarkGray
Write-Host "  .\Get-WisdomFileTree.ps1 -Mode Detailed  # Full metadata + dupe check" -ForegroundColor DarkGray