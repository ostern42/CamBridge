# CamBridge Project File Tree Generator
# Highlights important project files and shows sync status

param(
    [string]$ProjectPath = "C:\Users\aladmin\source\repos\CamBridge",
    [string]$OutputFile = "cambridge-structure.md"
)

$importantFiles = @{
    "*.sln" = "📘"  # Solution files
    "*.csproj" = "📦"  # Project files
    "*.cs" = "📄"  # C# source files
    "*.md" = "📝"  # Markdown docs
    "*.json" = "⚙️"  # Config files
    "*.xml" = "📋"  # XML files
    "*.bat" = "🔧"  # Batch scripts
    "*.ps1" = "🔧"  # PowerShell scripts
    "PROJECT_CONTEXT_*.md" = "📚"  # Context docs
    "CHANGELOG.md" = "📅"  # Changelog
    "README.md" = "📖"  # Readme
}

function Get-FileIcon {
    param([string]$FileName)
    
    foreach ($pattern in $importantFiles.Keys) {
        if ($FileName -like $pattern) {
            return $importantFiles[$pattern]
        }
    }
    return "📄"
}

function Get-ProjectTree {
    param(
        [string]$Path,
        [string]$Indent = "",
        [int]$Level = 0
    )
    
    $items = Get-ChildItem -Path $Path -Force | 
             Where-Object { $_.Name -notmatch '^(\.git|bin|obj|\.vs|packages)$' }
    
    $folders = $items | Where-Object { $_.PSIsContainer } | Sort-Object Name
    $files = $items | Where-Object { -not $_.PSIsContainer } | Sort-Object Name
    
    # Show folders first
    foreach ($folder in $folders) {
        $output = "$Indent📁 **$($folder.Name)**/"
        Write-Output $output
        
        if ($Level -lt 3) {  # Limit depth for readability
            Get-ProjectTree -Path $folder.FullName -Indent "$Indent  " -Level ($Level + 1)
        }
    }
    
    # Then show files
    foreach ($file in $files) {
        $icon = Get-FileIcon -FileName $file.Name
        $size = "{0:N2} KB" -f ($file.Length / 1KB)
        $output = "$Indent$icon $($file.Name) ($size)"
        Write-Output $output
    }
}

# Generate the tree
$output = @()
$output += "# CamBridge Project Structure"
$output += "Generated: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')"
$output += ""
$output += "## File Tree"
$output += "```"
$output += Get-ProjectTree -Path $ProjectPath
$output += "```"
$output += ""

# Add statistics
$stats = @{
    CSharpFiles = (Get-ChildItem -Path $ProjectPath -Recurse -Filter "*.cs" -File).Count
    ProjectFiles = (Get-ChildItem -Path $ProjectPath -Recurse -Filter "*.csproj" -File).Count
    MarkdownFiles = (Get-ChildItem -Path $ProjectPath -Recurse -Filter "*.md" -File).Count
    ContextFiles = (Get-ChildItem -Path $ProjectPath -Recurse -Filter "PROJECT_CONTEXT_*.md" -File).Count
    TotalSize = "{0:N2} MB" -f ((Get-ChildItem -Path $ProjectPath -Recurse -File | 
                                 Measure-Object -Property Length -Sum).Sum / 1MB)
}

$output += "## Project Statistics"
$output += "- C# Source Files: $($stats.CSharpFiles)"
$output += "- Project Files: $($stats.ProjectFiles)"
$output += "- Documentation Files: $($stats.MarkdownFiles)"
$output += "- Context History Files: $($stats.ContextFiles)"
$output += "- Total Project Size: $($stats.TotalSize)"

# Check for important files
$output += ""
$output += "## Key Files Check"
$requiredFiles = @(
    "CamBridge.sln",
    "README.md",
    "CHANGELOG.md",
    "src\CamBridge\CamBridge.csproj"
)

foreach ($file in $requiredFiles) {
    $fullPath = Join-Path $ProjectPath $file
    if (Test-Path $fullPath) {
        $output += "✅ $file - Vorhanden"
    } else {
        $output += "❌ $file - FEHLT!"
    }
}

# Save output
$output | Out-File -FilePath $OutputFile -Encoding UTF8

Write-Host "Projektstruktur wurde in '$OutputFile' gespeichert." -ForegroundColor Green
Write-Host ""
Write-Host "Zusammenfassung:" -ForegroundColor Cyan
Write-Host "- C# Dateien: $($stats.CSharpFiles)"
Write-Host "- Projektgröße: $($stats.TotalSize)"
Write-Host "- Context-Versionen: $($stats.ContextFiles)"