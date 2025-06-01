@echo off
REM CamBridge Source Collector - Settings Implementation Only
REM © 2025 Claude's Improbably Reliable Software Solutions

setlocal enabledelayedexpansion

set "TARGET_DIR=collected_sources_settings"
set /a COUNT=0

if exist "%TARGET_DIR%" rmdir /s /q "%TARGET_DIR%"
mkdir "%TARGET_DIR%"

echo ======================================
echo CamBridge Settings Implementation
echo Source Collector v0.4.1
echo ======================================
echo.

REM === PROJEKT BASIS ===
echo [1/5] Sammle Projekt-Basis...
for %%F in (Version.props cambridge-entwicklungsplan-v2.md) do (
    if exist "%%F" (
        copy "%%F" "%TARGET_DIR%\%%F" >nul 2>&1
        if !errorlevel! equ 0 (
            echo [OK] %%F
            set /a COUNT+=1
        )
    )
)

REM === SETTINGS GUI DATEIEN ===
echo.
echo [2/5] Sammle Settings GUI-Dateien...
for %%F in (
    "src\CamBridge.Config\Views\SettingsPage.xaml"
    "src\CamBridge.Config\Views\SettingsPage.xaml.cs"
    "src\CamBridge.Config\ViewModels\SettingsViewModel.cs"
    "src\CamBridge.Config\Services\ConfigurationService.cs"
    "src\CamBridge.Config\Services\IConfigurationService.cs"
    "src\CamBridge.Config\MainWindow.xaml"
    "src\CamBridge.Config\Converters\ValueConverters.cs"
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

REM === CORE SETTINGS MODELLE ===
echo.
echo [3/5] Sammle Core Settings-Modelle...
for %%F in (
    "src\CamBridge.Core\CamBridgeSettings.cs"
    "src\CamBridge.Core\ProcessingOptions.cs"
    "src\CamBridge.Core\NotificationSettings.cs"
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

REM === REFERENZ DATEIEN ===
echo.
echo [4/5] Sammle Referenz-Konfiguration...
if exist "src\CamBridge.Service\appsettings.json" (
    copy "src\CamBridge.Service\appsettings.json" "%TARGET_DIR%\src_CamBridge.Service_appsettings.json" >nul 2>&1
    if !errorlevel! equ 0 (
        echo [OK] appsettings.json
        set /a COUNT+=1
    )
)

REM === HANDOVER PROMPT ===
echo.
echo [5/5] Erstelle Handover-Prompt...
(
echo # CamBridge Settings Implementation
echo.
echo Ich arbeite an CamBridge v0.4.0 - Settings Page Implementation.
echo © 2025 Claude's Improbably Reliable Software Solutions
echo.
echo Die GUI läuft bereits mit Dashboard und Service Control.
echo Jetzt soll die Settings Page von einem Platzhalter zu einer voll funktionsfähigen Konfigurationsoberfläche werden.
echo.
echo Bitte implementiere:
echo 1. SettingsPage.xaml mit TabView ^(4 Tabs^)
echo 2. SettingsViewModel mit vollständigem Data Binding
echo 3. ConfigurationService der wirklich appsettings.json liest/schreibt
echo 4. Validierung und Fehlerbehandlung
echo.
echo Die Settings sollen die gleiche Struktur wie appsettings.json haben.
echo Verwende ModernWPF Controls und CommunityToolkit.Mvvm.
echo Ausführliche Implementation für Anfänger bitte.
echo.
echo [Die gesammelten Dateien befinden sich im Ordner: %TARGET_DIR%]
) > "%TARGET_DIR%\HANDOVER_PROMPT.txt"

echo.
echo ======================================
echo Sammlung abgeschlossen!
echo !COUNT! Dateien für Settings-Implementation
echo.
echo Handover-Prompt: %TARGET_DIR%\HANDOVER_PROMPT.txt
echo ======================================
echo.
echo Nächste Schritte:
echo 1. Neuen Chat öffnen
echo 2. HANDOVER_PROMPT.txt kopieren
echo 3. Alle Dateien aus %TARGET_DIR% hochladen
echo 4. Settings implementieren lassen
echo.
pause