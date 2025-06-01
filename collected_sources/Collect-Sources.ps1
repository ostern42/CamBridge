# CamBridge Source Collector
# © 2025 Claude's Improbably Reliable Software Solutions
# Sammelt alle relevanten Dateien in einen flachen Ordner mit Pfadnamen

param(
    [string]$TargetDir = "collected_sources",
    [switch]$IncludeTests = $false,
    [switch]$OpenExplorer = $true
)

# Funktion zum sicheren Dateikopieren
function Copy-SourceFile {
    param(
        [string]$SourcePath,
        [string]$Prefix = ""
    )
    
    if (Test-Path $SourcePath) {
        $fileName = Split-Path $SourcePath -Leaf
        
        if ($Prefix) {
            $targetName = "${Prefix}_${fileName}"
        } else {
            # Ersetze Pfadtrenner durch Underscores
            $targetName = $SourcePath.Replace("\", "_").Replace("/", "_")
        }
        
        $targetPath = Join-Path $TargetDir $targetName
        Copy-Item $SourcePath $targetPath -Force
        Write-Host "[OK] $SourcePath" -ForegroundColor Green
        return $true
    } else {
        Write-Host "[--] $SourcePath (nicht gefunden)" -ForegroundColor Yellow
        return $false
    }
}

# Zielordner vorbereiten
if (Test-Path $TargetDir) {
    Remove-Item $TargetDir -Recurse -Force
}
New-Item -ItemType Directory -Path $TargetDir | Out-Null

Write-Host "Sammle CamBridge Quelldateien..." -ForegroundColor Cyan
Write-Host ""

$copiedCount = 0
$missingCount = 0

# Dateiliste definieren
$files = @(
    # Solution-Dateien
    @{Path = "CamBridge.sln"; Prefix = ""},
    @{Path = "Version.props"; Prefix = ""},
    @{Path = "README.md"; Prefix = ""},
    @{Path = "CHANGELOG.md"; Prefix = ""},
    @{Path = "LICENSE"; Prefix = ""},
    @{Path = ".gitignore"; Prefix = ""},
    @{Path = ".gitattributes"; Prefix = ""},
    @{Path = ".editorconfig"; Prefix = ""},
    
    # Core Projekt
    @{Path = "src\CamBridge.Core\CamBridge.Core.csproj"; Prefix = "src_CamBridge.Core"},
    @{Path = "src\CamBridge.Core\GlobalUsings.cs"; Prefix = "src_CamBridge.Core"},
    @{Path = "src\CamBridge.Core\AssemblyInfo.cs"; Prefix = "src_CamBridge.Core"},
    @{Path = "src\CamBridge.Core\CamBridgeSettings.cs"; Prefix = "src_CamBridge.Core"},
    @{Path = "src\CamBridge.Core\ProcessingOptions.cs"; Prefix = "src_CamBridge.Core"},
    @{Path = "src\CamBridge.Core\MappingRule.cs"; Prefix = "src_CamBridge.Core"},
    
    # Core Entities
    @{Path = "src\CamBridge.Core\Entities\ImageMetadata.cs"; Prefix = "src_CamBridge.Core_Entities"},
    @{Path = "src\CamBridge.Core\Entities\PatientInfo.cs"; Prefix = "src_CamBridge.Core_Entities"},
    @{Path = "src\CamBridge.Core\Entities\StudyInfo.cs"; Prefix = "src_CamBridge.Core_Entities"},
    
    # Core Interfaces
    @{Path = "src\CamBridge.Core\Interfaces\IDicomConverter.cs"; Prefix = "src_CamBridge.Core_Interfaces"},
    @{Path = "src\CamBridge.Core\Interfaces\IExifReader.cs"; Prefix = "src_CamBridge.Core_Interfaces"},
    @{Path = "src\CamBridge.Core\Interfaces\IFileProcessor.cs"; Prefix = "src_CamBridge.Core_Interfaces"},
    @{Path = "src\CamBridge.Core\Interfaces\IMappingConfiguration.cs"; Prefix = "src_CamBridge.Core_Interfaces"},
    
    # Core ValueObjects
    @{Path = "src\CamBridge.Core\ValueObjects\DicomTag.cs"; Prefix = "src_CamBridge.Core_ValueObjects"},
    @{Path = "src\CamBridge.Core\ValueObjects\ExifTag.cs"; Prefix = "src_CamBridge.Core_ValueObjects"},
    @{Path = "src\CamBridge.Core\ValueObjects\PatientId.cs"; Prefix = "src_CamBridge.Core_ValueObjects"},
    @{Path = "src\CamBridge.Core\ValueObjects\StudyId.cs"; Prefix = "src_CamBridge.Core_ValueObjects"},
    
    # Infrastructure Projekt
    @{Path = "src\CamBridge.Infrastructure\CamBridge.Infrastructure.csproj"; Prefix = "src_CamBridge.Infrastructure"},
    @{Path = "src\CamBridge.Infrastructure\GlobalUsings.cs"; Prefix = "src_CamBridge.Infrastructure"},
    @{Path = "src\CamBridge.Infrastructure\AssemblyInfo.cs"; Prefix = "src_CamBridge.Infrastructure"},
    @{Path = "src\CamBridge.Infrastructure\ServiceCollectionExtensions.cs"; Prefix = "src_CamBridge.Infrastructure"},
    
    # Infrastructure Services
    @{Path = "src\CamBridge.Infrastructure\Services\DicomConverter.cs"; Prefix = "src_CamBridge.Infrastructure_Services"},
    @{Path = "src\CamBridge.Infrastructure\Services\DicomTagMapper.cs"; Prefix = "src_CamBridge.Infrastructure_Services"},
    @{Path = "src\CamBridge.Infrastructure\Services\ExifReader.cs"; Prefix = "src_CamBridge.Infrastructure_Services"},
    @{Path = "src\CamBridge.Infrastructure\Services\RicohExifReader.cs"; Prefix = "src_CamBridge.Infrastructure_Services"},
    @{Path = "src\CamBridge.Infrastructure\Services\FileProcessor.cs"; Prefix = "src_CamBridge.Infrastructure_Services"},
    @{Path = "src\CamBridge.Infrastructure\Services\FolderWatcherService.cs"; Prefix = "src_CamBridge.Infrastructure_Services"},
    @{Path = "src\CamBridge.Infrastructure\Services\ProcessingQueue.cs"; Prefix = "src_CamBridge.Infrastructure_Services"},
    @{Path = "src\CamBridge.Infrastructure\Services\MappingConfigurationLoader.cs"; Prefix = "src_CamBridge.Infrastructure_Services"},
    
    # Service Projekt
    @{Path = "src\CamBridge.Service\CamBridge.Service.csproj"; Prefix = "src_CamBridge.Service"},
    @{Path = "src\CamBridge.Service\AssemblyInfo.cs"; Prefix = "src_CamBridge.Service"},
    @{Path = "src\CamBridge.Service\Program.cs"; Prefix = "src_CamBridge.Service"},
    @{Path = "src\CamBridge.Service\Worker.cs"; Prefix = "src_CamBridge.Service"},
    @{Path = "src\CamBridge.Service\CamBridgeHealthCheck.cs"; Prefix = "src_CamBridge.Service"},
    @{Path = "src\CamBridge.Service\appsettings.json"; Prefix = "src_CamBridge.Service"},
    @{Path = "src\CamBridge.Service\appsettings.Development.json"; Prefix = "src_CamBridge.Service"},
    @{Path = "src\CamBridge.Service\mappings.json"; Prefix = "src_CamBridge.Service"},
    
    # Service Properties
    @{Path = "src\CamBridge.Service\Properties\launchSettings.json"; Prefix = "src_CamBridge.Service_Properties"}
)

# Test-Dateien wenn gewünscht
if ($IncludeTests) {
    $files += @(
        @{Path = "tests\CamBridge.Infrastructure.Tests\CamBridge.Infrastructure.Tests.csproj"; Prefix = "tests_CamBridge.Infrastructure.Tests"},
        # Hier weitere Test-Dateien ergänzen...
    )
}

# Dateien kopieren
foreach ($file in $files) {
    if (Copy-SourceFile -SourcePath $file.Path -Prefix $file.Prefix) {
        $copiedCount++
    } else {
        $missingCount++
    }
}

# Zusammenfassung
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Fertig!" -ForegroundColor Green
Write-Host "Kopiert: $copiedCount Dateien" -ForegroundColor Green
Write-Host "Fehlend: $missingCount Dateien" -ForegroundColor Yellow
Write-Host "Zielordner: $((Resolve-Path $TargetDir).Path)" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

# Index-Datei erstellen
$indexPath = Join-Path $TargetDir "_INDEX.txt"
$indexContent = @"
CamBridge Source Collection
© 2025 Claude's Improbably Reliable Software Solutions
========================================
Erstellt: $(Get-Date -Format "yyyy-MM-dd HH:mm:ss")
Dateien: $copiedCount
========================================

DATEIINDEX:
"@

Get-ChildItem $TargetDir -File | Where-Object { $_.Name -ne "_INDEX.txt" } | ForEach-Object {
    $originalPath = $_.Name.Replace("_", "\")
    if ($originalPath -match "^src\\|^tests\\") {
        $indexContent += "`n$($_.Name) -> $originalPath"
    }
}

$indexContent | Out-File $indexPath -Encoding UTF8

# Optional: Explorer öffnen
if ($OpenExplorer) {
    Start-Process explorer.exe -ArgumentList $((Resolve-Path $TargetDir).Path)
}