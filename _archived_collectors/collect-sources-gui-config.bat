@echo off
setlocal enabledelayedexpansion

:: CamBridge Source Collection - GUI Configuration Focus
:: © 2025 Claude's Improbably Reliable Software Solutions
:: Fokus: Settings Page Fix & Konfigurationsverwaltung (v0.4.5 - v0.5.0)

set OUTPUT_FILE=PROJECT_CONTEXT_GUI_CONFIG.md
set COUNTER=0

echo # CamBridge Project Context - GUI Configuration Focus > %OUTPUT_FILE%
echo. >> %OUTPUT_FILE%
echo Generated: %date% %time% >> %OUTPUT_FILE%
echo Focus: Settings Page Fix, Configuration Management, Mapping Editor >> %OUTPUT_FILE%
echo. >> %OUTPUT_FILE%

:: === 1. KRITISCHE DOKUMENTATION ===
echo ## Key Project Files >> %OUTPUT_FILE%
echo. >> %OUTPUT_FILE%

:: PROJECT_WISDOM.md ist ESSENTIELL
if exist PROJECT_WISDOM.md (
    echo ### PROJECT_WISDOM.md >> %OUTPUT_FILE%
    echo ^<details^>^<summary^>View Content^</summary^> >> %OUTPUT_FILE%
    echo. >> %OUTPUT_FILE%
    echo ```markdown >> %OUTPUT_FILE%
    type PROJECT_WISDOM.md >> %OUTPUT_FILE%
    echo ``` >> %OUTPUT_FILE%
    echo ^</details^> >> %OUTPUT_FILE%
    echo. >> %OUTPUT_FILE%
    set /a COUNTER+=1
)

:: Version.props für aktuelle Version
if exist Version.props (
    echo ### Version.props >> %OUTPUT_FILE%
    echo ```xml >> %OUTPUT_FILE%
    type Version.props >> %OUTPUT_FILE%
    echo ``` >> %OUTPUT_FILE%
    echo. >> %OUTPUT_FILE%
    set /a COUNTER+=1
)

:: === 2. GUI HAUPTKOMPONENTEN ===
echo ## GUI Main Components >> %OUTPUT_FILE%
echo. >> %OUTPUT_FILE%

:: App.xaml.cs - DI Setup (KRITISCH für Settings Crash!)
set "FILE=src\CamBridge.Config\App.xaml.cs"
if exist "%FILE%" (
    echo ### %FILE% - Dependency Injection Setup >> %OUTPUT_FILE%
    echo ```csharp >> %OUTPUT_FILE%
    type "%FILE%" >> %OUTPUT_FILE%
    echo ``` >> %OUTPUT_FILE%
    echo. >> %OUTPUT_FILE%
    set /a COUNTER+=1
)

:: MainWindow - Navigation
set "FILE=src\CamBridge.Config\MainWindow.xaml.cs"
if exist "%FILE%" (
    echo ### %FILE% - Navigation Logic >> %OUTPUT_FILE%
    echo ```csharp >> %OUTPUT_FILE%
    type "%FILE%" >> %OUTPUT_FILE%
    echo ``` >> %OUTPUT_FILE%
    echo. >> %OUTPUT_FILE%
    set /a COUNTER+=1
)

:: === 3. SETTINGS PAGE (PROBLEM-FOKUS) ===
echo ## Settings Page Components (CRASH ISSUE) >> %OUTPUT_FILE%
echo. >> %OUTPUT_FILE%

:: Settings Page XAML
set "FILE=src\CamBridge.Config\Views\SettingsPage.xaml"
if exist "%FILE%" (
    echo ### %FILE% >> %OUTPUT_FILE%
    echo ```xml >> %OUTPUT_FILE%
    type "%FILE%" >> %OUTPUT_FILE%
    echo ``` >> %OUTPUT_FILE%
    echo. >> %OUTPUT_FILE%
    set /a COUNTER+=1
)

:: Settings Page Code-Behind
set "FILE=src\CamBridge.Config\Views\SettingsPage.xaml.cs"
if exist "%FILE%" (
    echo ### %FILE% >> %OUTPUT_FILE%
    echo ```csharp >> %OUTPUT_FILE%
    type "%FILE%" >> %OUTPUT_FILE%
    echo ``` >> %OUTPUT_FILE%
    echo. >> %OUTPUT_FILE%
    set /a COUNTER+=1
)

:: Settings ViewModel (KRITISCH!)
set "FILE=src\CamBridge.Config\ViewModels\SettingsViewModel.cs"
if exist "%FILE%" (
    echo ### %FILE% - ViewModel (CHECK DI REGISTRATION!) >> %OUTPUT_FILE%
    echo ```csharp >> %OUTPUT_FILE%
    type "%FILE%" >> %OUTPUT_FILE%
    echo ``` >> %OUTPUT_FILE%
    echo. >> %OUTPUT_FILE%
    set /a COUNTER+=1
)

:: === 4. FUNKTIONIERENDE PAGES ALS REFERENZ ===
echo ## Working Pages for Reference >> %OUTPUT_FILE%
echo. >> %OUTPUT_FILE%

:: Dead Letters Page (funktioniert!)
set "FILE=src\CamBridge.Config\Views\DeadLettersPage.xaml.cs"
if exist "%FILE%" (
    echo ### %FILE% - Working Example >> %OUTPUT_FILE%
    echo ```csharp >> %OUTPUT_FILE%
    type "%FILE%" >> %OUTPUT_FILE%
    echo ``` >> %OUTPUT_FILE%
    echo. >> %OUTPUT_FILE%
    set /a COUNTER+=1
)

:: Dead Letters ViewModel
set "FILE=src\CamBridge.Config\ViewModels\DeadLettersViewModel.cs"
if exist "%FILE%" (
    echo ### %FILE% - Working ViewModel >> %OUTPUT_FILE%
    echo ```csharp >> %OUTPUT_FILE%
    type "%FILE%" >> %OUTPUT_FILE%
    echo ``` >> %OUTPUT_FILE%
    echo. >> %OUTPUT_FILE%
    set /a COUNTER+=1
)

:: === 5. CONFIGURATION SERVICES ===
echo ## Configuration Services >> %OUTPUT_FILE%
echo. >> %OUTPUT_FILE%

:: ConfigurationService
set "FILE=src\CamBridge.Config\Services\ConfigurationService.cs"
if exist "%FILE%" (
    echo ### %FILE% >> %OUTPUT_FILE%
    echo ```csharp >> %OUTPUT_FILE%
    type "%FILE%" >> %OUTPUT_FILE%
    echo ``` >> %OUTPUT_FILE%
    echo. >> %OUTPUT_FILE%
    set /a COUNTER+=1
)

:: IConfigurationService Interface
set "FILE=src\CamBridge.Config\Services\IConfigurationService.cs"
if exist "%FILE%" (
    echo ### %FILE% >> %OUTPUT_FILE%
    echo ```csharp >> %OUTPUT_FILE%
    type "%FILE%" >> %OUTPUT_FILE%
    echo ``` >> %OUTPUT_FILE%
    echo. >> %OUTPUT_FILE%
    set /a COUNTER+=1
)

:: === 6. MAPPING CONFIGURATION (für v0.5.0) ===
echo ## Mapping Configuration Classes >> %OUTPUT_FILE%
echo. >> %OUTPUT_FILE%

:: MappingRule
set "FILE=src\CamBridge.Core\MappingRule.cs"
if exist "%FILE%" (
    echo ### %FILE% >> %OUTPUT_FILE%
    echo ```csharp >> %OUTPUT_FILE%
    type "%FILE%" >> %OUTPUT_FILE%
    echo ``` >> %OUTPUT_FILE%
    echo. >> %OUTPUT_FILE%
    set /a COUNTER+=1
)

:: IMappingConfiguration
set "FILE=src\CamBridge.Core\Interfaces\IMappingConfiguration.cs"
if exist "%FILE%" (
    echo ### %FILE% >> %OUTPUT_FILE%
    echo ```csharp >> %OUTPUT_FILE%
    type "%FILE%" >> %OUTPUT_FILE%
    echo ``` >> %OUTPUT_FILE%
    echo. >> %OUTPUT_FILE%
    set /a COUNTER+=1
)

:: MappingConfigurationLoader
set "FILE=src\CamBridge.Infrastructure\Services\MappingConfigurationLoader.cs"
if exist "%FILE%" (
    echo ### %FILE% >> %OUTPUT_FILE%
    echo ```csharp >> %OUTPUT_FILE%
    type "%FILE%" >> %OUTPUT_FILE%
    echo ``` >> %OUTPUT_FILE%
    echo. >> %OUTPUT_FILE%
    set /a COUNTER+=1
)

:: === 7. MODELS UND VALUE OBJECTS ===
echo ## Core Models >> %OUTPUT_FILE%
echo. >> %OUTPUT_FILE%

:: CamBridgeSettings
set "FILE=src\CamBridge.Core\CamBridgeSettings.cs"
if exist "%FILE%" (
    echo ### %FILE% >> %OUTPUT_FILE%
    echo ```csharp >> %OUTPUT_FILE%
    type "%FILE%" >> %OUTPUT_FILE%
    echo ``` >> %OUTPUT_FILE%
    echo. >> %OUTPUT_FILE%
    set /a COUNTER+=1
)

:: ProcessingOptions
set "FILE=src\CamBridge.Core\ProcessingOptions.cs"
if exist "%FILE%" (
    echo ### %FILE% >> %OUTPUT_FILE%
    echo ```csharp >> %OUTPUT_FILE%
    type "%FILE%" >> %OUTPUT_FILE%
    echo ``` >> %OUTPUT_FILE%
    echo. >> %OUTPUT_FILE%
    set /a COUNTER+=1
)

:: === 8. PROJECT FILES ===
echo ## Project Files >> %OUTPUT_FILE%
echo. >> %OUTPUT_FILE%

:: Config Project File
set "FILE=src\CamBridge.Config\CamBridge.Config.csproj"
if exist "%FILE%" (
    echo ### %FILE% >> %OUTPUT_FILE%
    echo ```xml >> %OUTPUT_FILE%
    type "%FILE%" >> %OUTPUT_FILE%
    echo ``` >> %OUTPUT_FILE%
    echo. >> %OUTPUT_FILE%
    set /a COUNTER+=1
)

:: === 9. BEISPIEL KONFIGURATIONEN ===
echo ## Sample Configurations >> %OUTPUT_FILE%
echo. >> %OUTPUT_FILE%

:: mappings.json
if exist "src\CamBridge.Service\mappings.json" (
    echo ### mappings.json >> %OUTPUT_FILE%
    echo ```json >> %OUTPUT_FILE%
    type "src\CamBridge.Service\mappings.json" >> %OUTPUT_FILE%
    echo ``` >> %OUTPUT_FILE%
    echo. >> %OUTPUT_FILE%
    set /a COUNTER+=1
)

:: appsettings.json (nur erste 50 Zeilen)
if exist "src\CamBridge.Service\appsettings.json" (
    echo ### appsettings.json (excerpt) >> %OUTPUT_FILE%
    echo ```json >> %OUTPUT_FILE%
    powershell -Command "Get-Content 'src\CamBridge.Service\appsettings.json' | Select-Object -First 50" >> %OUTPUT_FILE%
    echo ... >> %OUTPUT_FILE%
    echo ``` >> %OUTPUT_FILE%
    echo. >> %OUTPUT_FILE%
    set /a COUNTER+=1
)

:: === 10. LETZTE CHANGELOG EINTRÄGE ===
echo ## Recent Changes >> %OUTPUT_FILE%
echo. >> %OUTPUT_FILE%

if exist CHANGELOG.md (
    echo ### CHANGELOG.md (last 3 versions) >> %OUTPUT_FILE%
    echo ```markdown >> %OUTPUT_FILE%
    powershell -Command "$content = Get-Content 'CHANGELOG.md'; $versionCount = 0; foreach ($line in $content) { Write-Output $line; if ($line -match '^## \[[\d\.]+\]') { $versionCount++; if ($versionCount -ge 3) { break } } }" >> %OUTPUT_FILE%
    echo ``` >> %OUTPUT_FILE%
    echo. >> %OUTPUT_FILE%
    set /a COUNTER+=1
)

:: === ZUSAMMENFASSUNG ===
echo. >> %OUTPUT_FILE%
echo ## Summary >> %OUTPUT_FILE%
echo - Total files collected: %COUNTER% >> %OUTPUT_FILE%
echo - Focus: Settings Page DI/Navigation issues >> %OUTPUT_FILE%
echo - Next: Mapping Editor with Drag and Drop >> %OUTPUT_FILE%
echo - Target versions: v0.4.5 (Fix) and v0.5.0 (Config Management) >> %OUTPUT_FILE%

echo.
echo ========================================
echo GUI Configuration Context Collection Complete!
echo ========================================
echo Total files collected: %COUNTER%
echo Output file: %OUTPUT_FILE%
echo.
echo Focus areas:
echo - Settings Page crash investigation
echo - Dependency Injection setup
echo - Working pages for comparison
echo - Configuration services
echo - Mapping system classes
echo.
echo Next steps:
echo 1. Fix Settings Page navigation crash (v0.4.5)
echo 2. Implement Mapping Editor UI (v0.5.0)
echo 3. Add Import/Export functionality
echo.
pause