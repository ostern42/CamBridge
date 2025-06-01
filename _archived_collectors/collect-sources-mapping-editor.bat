@echo off
REM CamBridge Source Collector - Mapping Editor Focus
REM © 2025 Claude's Improbably Reliable Software Solutions
REM Optimiert für Mapping Editor Entwicklung (v0.5.0)

setlocal enabledelayedexpansion

set "TARGET_DIR=collected_sources_mapping_editor"
set /a COUNT=0

if exist "%TARGET_DIR%" rmdir /s /q "%TARGET_DIR%"
mkdir "%TARGET_DIR%"

echo ======================================
echo CamBridge Mapping Editor Source Collector
echo Version: v0.5.0 Development
echo Coverage: ~20-25%% (Mapping Focus)
echo ======================================
echo.

REM === PROJEKT-GRUNDLAGEN (MIT PROJECT_WISDOM!) ===
echo [1/10] Sammle Projekt-Grundlagen...
for %%F in (CamBridge.sln Version.props PROJECT_WISDOM.md cambridge-entwicklungsplan-v2.md CHANGELOG.md README.md) do (
    if exist "%%F" (
        copy "%%F" "%TARGET_DIR%\%%F" >nul 2>&1
        if !errorlevel! equ 0 (
            echo [OK] %%F
            set /a COUNT+=1
        )
    )
)

REM === GUI VIEWS & VIEWMODELS (Fokus auf Settings als Referenz) ===
echo.
echo [2/10] Sammle GUI Views und ViewModels...
for %%F in (
    "src\CamBridge.Config\Views\SettingsPage.xaml"
    "src\CamBridge.Config\Views\SettingsPage.xaml.cs"
    "src\CamBridge.Config\ViewModels\SettingsViewModel.cs"
    "src\CamBridge.Config\Views\DashboardPage.xaml"
    "src\CamBridge.Config\Views\DashboardPage.xaml.cs"
    "src\CamBridge.Config\ViewModels\DashboardViewModel.cs"
    "src\CamBridge.Config\MainWindow.xaml"
    "src\CamBridge.Config\MainWindow.xaml.cs"
    "src\CamBridge.Config\App.xaml"
    "src\CamBridge.Config\App.xaml.cs"
) do (
    if exist %%F (
        set "FILE=%%~F"
        set "NAME=!FILE:\=_!"
        copy %%F "%TARGET_DIR%\!NAME!" >nul 2>&1
        if !errorlevel! equ 0 (
            echo [OK] %%~F
            set /a COUNT+=1
        )
    )
)

REM === MAPPING CORE MODELS ===
echo.
echo [3/10] Sammle Mapping Core Models...
for %%F in (
    "src\CamBridge.Core\MappingRule.cs"
    "src\CamBridge.Core\DicomMappingConfiguration.cs"
    "src\CamBridge.Core\CamBridgeSettings.cs"
    "src\CamBridge.Core\ProcessingOptions.cs"
    "src\CamBridge.Core\Models\DicomTag.cs"
    "src\CamBridge.Core\Models\ProcessingResult.cs"
    "src\CamBridge.Core\Interfaces\IMappingConfiguration.cs"
    "src\CamBridge.Core\CamBridge.Core.csproj"
) do (
    if exist %%F (
        set "FILE=%%~F"
        set "NAME=!FILE:\=_!"
        copy %%F "%TARGET_DIR%\!NAME!" >nul 2>&1
        if !errorlevel! equ 0 (
            echo [OK] %%~F
            set /a COUNT+=1
        )
    )
)

REM === MAPPING INFRASTRUCTURE SERVICES ===
echo.
echo [4/10] Sammle Mapping Infrastructure Services...
for %%F in (
    "src\CamBridge.Infrastructure\Services\MappingConfigurationLoader.cs"
    "src\CamBridge.Infrastructure\Mappers\DicomMapper.cs"
    "src\CamBridge.Infrastructure\Services\FileProcessingService.cs"
    "src\CamBridge.Infrastructure\Services\JsonMappingConfigurationLoader.cs"
    "src\CamBridge.Infrastructure\CamBridge.Infrastructure.csproj"
) do (
    if exist %%F (
        set "FILE=%%~F"
        set "NAME=!FILE:\=_!"
        copy %%F "%TARGET_DIR%\!NAME!" >nul 2>&1
        if !errorlevel! equ 0 (
            echo [OK] %%~F
            set /a COUNT+=1
        )
    )
)

REM === CONFIGURATION SERVICES (für Settings-Integration) ===
echo.
echo [5/10] Sammle Configuration Services...
for %%F in (
    "src\CamBridge.Config\Services\ConfigurationService.cs"
    "src\CamBridge.Config\Services\IConfigurationService.cs"
    "src\CamBridge.Config\Services\HttpApiService.cs"
    "src\CamBridge.Config\Services\IHttpApiService.cs"
) do (
    if exist %%F (
        set "FILE=%%~F"
        set "NAME=!FILE:\=_!"
        copy %%F "%TARGET_DIR%\!NAME!" >nul 2>&1
        if !errorlevel! equ 0 (
            echo [OK] %%~F
            set /a COUNT+=1
        )
    )
)

REM === GUI HELPERS & CONVERTERS ===
echo.
echo [6/10] Sammle GUI Helpers und Converters...
for %%F in (
    "src\CamBridge.Config\Converters\BooleanToVisibilityConverter.cs"
    "src\CamBridge.Config\Converters\InverseBooleanConverter.cs"
    "src\CamBridge.Config\Converters\StringToVisibilityConverter.cs"
    "src\CamBridge.Config\Helpers\RelayCommand.cs"
    "src\CamBridge.Config\Helpers\ObservableObject.cs"
) do (
    if exist %%F (
        set "FILE=%%~F"
        set "NAME=!FILE:\=_!"
        copy %%F "%TARGET_DIR%\!NAME!" >nul 2>&1
        if !errorlevel! equ 0 (
            echo [OK] %%~F
            set /a COUNT+=1
        )
    )
)

REM === BEISPIEL MAPPINGS & KONFIGURATION ===
echo.
echo [7/10] Sammle Beispiel-Konfigurationen...
for %%F in (
    "src\CamBridge.Service\mappings.json"
    "src\CamBridge.Service\appsettings.json"
    "src\CamBridge.Config\CamBridge.Config.csproj"
) do (
    if exist %%F (
        set "FILE=%%~F"
        set "NAME=!FILE:\=_!"
        copy %%F "%TARGET_DIR%\!NAME!" >nul 2>&1
        if !errorlevel! equ 0 (
            echo [OK] %%~F
            set /a COUNT+=1
        )
    )
)

REM === DICOM TAG DEFINITIONEN (falls vorhanden) ===
echo.
echo [8/10] Sammle DICOM Tag Definitionen...
for %%F in (
    "src\CamBridge.Core\Constants\DicomTags.cs"
    "src\CamBridge.Core\Constants\StandardDicomTags.cs"
    "src\CamBridge.Infrastructure\Constants\DicomConstants.cs"
) do (
    if exist %%F (
        set "FILE=%%~F"
        set "NAME=!FILE:\=_!"
        copy %%F "%TARGET_DIR%\!NAME!" >nul 2>&1
        if !errorlevel! equ 0 (
            echo [OK] %%~F
            set /a COUNT+=1
        )
    )
)

REM === TEST MAPPINGS (falls vorhanden) ===
echo.
echo [9/10] Sammle Test Mappings...
for %%F in (
    "tests\TestData\test-mappings.json"
    "tests\TestData\sample-mappings.json"
    "tests\CamBridge.Core.Tests\MappingConfigurationTests.cs"
    "tests\CamBridge.Infrastructure.Tests\MappingTests.cs"
) do (
    if exist %%F (
        set "FILE=%%~F"
        set "NAME=!FILE:\=_!"
        copy %%F "%TARGET_DIR%\!NAME!" >nul 2>&1
        if !errorlevel! equ 0 (
            echo [OK] %%~F
            set /a COUNT+=1
        )
    )
)

REM === CONTEXT DOKUMENT ===
echo.
echo [10/10] Erstelle Context-Dokument...
(
echo # CamBridge Mapping Editor Context v0.5.0
echo © 2025 Claude's Improbably Reliable Software Solutions
echo.
echo ## Aktueller Stand
echo - v0.4.5: Settings Page vollständig funktionsfähig
echo - ConfigurationService mit JSON-Persistierung implementiert
echo - Alle Pages navigierbar ohne Crashes
echo - Core JPEG→DICOM Konvertierung getestet
echo.
echo ## Nächste Aufgabe: Mapping Editor ^(v0.5.0^)
echo.
echo ### Hauptziele
echo 1. **Mapping-Editor UI mit Drag & Drop**
echo    - TreeView für verfügbare DICOM Tags
echo    - ListView für aktuelle Mappings
echo    - Drag von TreeView zu ListView
echo    - Reorder innerhalb ListView
echo.
echo 2. **Live-Preview für Mappings**
echo    - Beispiel JPEG laden
echo    - Extrahierte Daten anzeigen
echo    - DICOM Output Preview
echo    - Validierung in Echtzeit
echo.
echo 3. **Import/Export Funktionalität**
echo    - JSON Import/Export
echo    - XML Import ^(optional^)
echo    - Mapping Templates
echo    - Versionierung von Configs
echo.
echo 4. **Template-System**
echo    - Vordefinierte Standard-Mappings
echo    - Benutzerdefinierte Templates
echo    - Quick-Apply Funktionen
echo.
echo 5. **Validierung mit Feedback**
echo    - DICOM Tag Validierung
echo    - Pflichtfeld-Prüfung
echo    - Wert-Validierung
echo    - Echtzeit-Fehleranzeige
echo.
echo ## UI-Konzept ^(Vorschlag^)
echo ```
echo +------------------------------------------+
echo ^| Available DICOM Tags ^| Current Mappings  ^|
echo ^|---------------------|-------------------^|
echo ^| [TreeView]          ^| [ListView]        ^|
echo ^| > Patient Info      ^| 1. PatientName    ^|
echo ^|   - Patient Name    ^| 2. PatientID      ^|
echo ^|   - Patient ID      ^| 3. StudyDate      ^|
echo ^| > Study Info        ^| [Buttons: ↑↓🗑]    ^|
echo ^|   - Study Date      ^|                   ^|
echo ^|---------------------|-------------------^|
echo ^| [Search Box]        ^| [Add Custom]      ^|
echo +------------------------------------------+
echo ^| Preview: [Load JPEG] [Show DICOM Output] ^|
echo +------------------------------------------+
echo ```
echo.
echo ## Technische Überlegungen
echo - WPF TreeView mit HierarchicalDataTemplate
echo - ListView mit ItemTemplate für Mappings
echo - Drag & Drop mit DragDrop.DoDragDrop^(^)
echo - ObservableCollection für Live-Updates
echo - ICommand Pattern für alle Actions
echo.
echo ## Integration mit Settings
echo - Mapping Editor als Tab in Settings
echo - Oder als eigene Page im NavigationView
echo - ConfigurationService erweitern für Mappings
echo - Speicherung in separater mappings.json
echo.
echo ## DICOM Tag Struktur
echo Hierarchische Organisation:
echo - Patient Level ^(0010,xxxx^)
echo - Study Level ^(0020,xxxx^)
echo - Series Level ^(0020,xxxx^)
echo - Instance Level ^(0008,xxxx^)
echo - Equipment Info ^(0008,xxxx^)
echo.
echo ## Mapping Rule Struktur
echo ```csharp
echo public class MappingRule
echo {
echo     public string DicomTag { get; set; }
echo     public string SourceField { get; set; }
echo     public string DefaultValue { get; set; }
echo     public bool IsRequired { get; set; }
echo     public string TransformExpression { get; set; }
echo }
echo ```
echo.
echo ## Bekannte Herausforderungen
echo - DICOM Tag Syntax ^(Gruppe,Element^)
echo - VR ^(Value Representation^) Handling
echo - Mehrwertige Tags ^(Multiplicity^)
echo - Character Set Encoding
echo - Ricoh speichert nur 3 von 5 Feldern! (kann aber auch sein, dass wir noch falsch auslesen!)
echo.
echo ## Test-Strategie
echo - Unit Tests für Mapping Logic
echo - UI Tests für Drag & Drop
echo - Integration Tests mit echten JPEGs
echo - Validierung gegen DICOM Standard
) > "%TARGET_DIR%\MAPPING_EDITOR_CONTEXT.md"
set /a COUNT+=1

echo.
echo ======================================
echo Mapping Editor Collection abgeschlossen!
echo !COUNT! Dateien gesammelt (~20-25%% Coverage)
echo.
echo Fokus: Mapping Editor Implementation
echo Version: v0.5.0 Development
echo.
echo Inhalt:
echo - PROJECT_WISDOM.md für Kontinuität
echo - GUI Framework und Referenz-Pages
echo - Mapping-spezifische Klassen
echo - Configuration Services
echo - Beispiel-Mappings
echo - Kontext-Dokument mit UI-Konzept
echo.
echo Nächste Schritte:
echo 1. UI-Design für Mapping Editor finalisieren
echo 2. TreeView/ListView Implementation
echo 3. Drag & Drop Funktionalität
echo 4. Live-Preview entwickeln
echo 5. Import/Export Features
echo ======================================
echo.
pause