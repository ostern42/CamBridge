@echo off
REM CamBridge Source Collector - Balanced Version for GUI Development
REM © 2025 Claude's Improbably Reliable Software Solutions
REM Optimiert für ~15-20% Projektwissen - Fokus auf GUI-Entwicklung

setlocal enabledelayedexpansion

set "TARGET_DIR=collected_sources_balanced"
set /a COUNT=0

if exist "%TARGET_DIR%" rmdir /s /q "%TARGET_DIR%"
mkdir "%TARGET_DIR%"

echo ======================================
echo CamBridge Balanced Source Collector
echo Version: GUI Development v0.4.1+
echo Coverage: ~15-20%% of project
echo ======================================
echo.

REM === PROJEKT-GRUNDLAGEN ===
echo [1/8] Sammle Projekt-Grundlagen...
for %%F in (CamBridge.sln Version.props cambridge-entwicklungsplan-v2.md CHANGELOG.md README.md) do (
    if exist "%%F" (
        copy "%%F" "%TARGET_DIR%\%%F" >nul 2>&1
        if !errorlevel! equ 0 (
            echo [OK] %%F
            set /a COUNT+=1
        )
    )
)

REM === KOMPLETTES GUI PROJEKT ===
echo.
echo [2/8] Sammle GUI-Projekt (CamBridge.Config)...
for /r src\CamBridge.Config %%F in (*.cs *.xaml *.xaml.cs *.csproj *.manifest) do (
    echo %%F | findstr /i "\\obj\\ \\bin\\ wpftmp AssemblyInfo" >nul
    if !errorlevel! neq 0 (
        set "FILE=%%F"
        set "REL=!FILE:%CD%\=!"
        set "NAME=!REL:\=_!"
        copy "%%F" "%TARGET_DIR%\!NAME!" >nul 2>&1
        if !errorlevel! equ 0 (
            echo [OK] !REL!
            set /a COUNT+=1
        )
    )
)

REM === CORE SETTINGS & MODELS ===
echo.
echo [3/8] Sammle Core Settings und Models...
for %%F in (
    "src\CamBridge.Core\CamBridge.Core.csproj"
    "src\CamBridge.Core\CamBridgeSettings.cs"
    "src\CamBridge.Core\ProcessingOptions.cs"
    "src\CamBridge.Core\NotificationSettings.cs"
    "src\CamBridge.Core\DicomMappingConfiguration.cs"
    "src\CamBridge.Core\Models\ProcessingResult.cs"
    "src\CamBridge.Core\Models\ProcessingStatistics.cs"
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

REM === SERVICE INTERFACES & CONTROLLER ===
echo.
echo [4/8] Sammle Service-Schnittstellen...
for %%F in (
    "src\CamBridge.Service\CamBridge.Service.csproj"
    "src\CamBridge.Service\Program.cs"
    "src\CamBridge.Service\Worker.cs"
    "src\CamBridge.Service\Controllers\StatusController.cs"
    "src\CamBridge.Service\appsettings.json"
    "src\CamBridge.Service\Services\DeadLetterQueueService.cs"
    "src\CamBridge.Service\Services\ServiceControlService.cs"
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

REM === INFRASTRUCTURE INTERFACES ===
echo.
echo [5/8] Sammle Infrastructure-Interfaces...
for %%F in (
    "src\CamBridge.Infrastructure\CamBridge.Infrastructure.csproj"
    "src\CamBridge.Infrastructure\Services\IFileProcessingService.cs"
    "src\CamBridge.Infrastructure\Services\INotificationService.cs"
    "src\CamBridge.Infrastructure\Services\NotificationService.cs"
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

REM === KONFIGURATIONS-DATEIEN ===
echo.
echo [6/8] Sammle Konfigurations-Dateien...
for %%F in (
    "src\CamBridge.Service\Properties\launchSettings.json"
    "global.json"
    ".editorconfig"
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

REM === SCRIPTS ===
echo.
echo [7/8] Sammle wichtige Scripts...
for %%F in (Install-CamBridgeService.ps1 collect-sources-*.bat) do (
    if exist "%%F" (
        copy "%%F" "%TARGET_DIR%\%%F" >nul 2>&1
        if !errorlevel! equ 0 (
            echo [OK] %%F
            set /a COUNT+=1
        )
    )
)

REM === CONTEXT DOKUMENT ===
echo.
echo [8/8] Erstelle Context-Dokument...
(
echo # CamBridge Context v0.4.1
echo © 2025 Claude's Improbably Reliable Software Solutions
echo.
echo ## Aktueller Stand
echo - v0.4.0: GUI Framework mit Dashboard implementiert ^(2x committed^)
echo - v0.4.1: Settings-Page Implementation ^(aktuell^)
echo - Nächste: Service Control ^(v0.4.2^), Dead Letters ^(v0.4.3^)
echo.
echo ## Projekt-Struktur
echo - src/CamBridge.Core: Domain Models, Settings
echo - src/CamBridge.Infrastructure: EXIF/DICOM Processing
echo - src/CamBridge.Service: Windows Service mit Web API
echo - src/CamBridge.Config: WPF GUI ^(MVVM mit ModernWPF^)
echo.
echo ## Tech Stack
echo - .NET 8, C# 12
echo - WPF mit ModernWpfUI 0.9.9
echo - CommunityToolkit.Mvvm 8.2.2
echo - ASP.NET Core Minimal API
echo - fo-dicom für DICOM
echo.
echo ## GUI-Entwicklung Fokus
echo - MainWindow mit NavigationView
echo - Dashboard ^(funktioniert^)
echo - Settings ^(in Arbeit^)
echo - Service Control ^(nächste Phase^)
echo - Dead Letters ^(übernächste Phase^)
echo.
echo ## Wichtige Patterns
echo - MVVM mit ObservableObject
echo - Async/Await für I/O
echo - Dependency Injection
echo - Clean Architecture
) > "%TARGET_DIR%\PROJECT_CONTEXT.md"
set /a COUNT+=1

echo.
echo ======================================
echo Balanced Collection abgeschlossen!
echo !COUNT! Dateien gesammelt (~15-20%% Coverage)
echo.
echo Fokus: GUI-Entwicklung Phase 8-10
echo Version: v0.4.1 (Settings Implementation)
echo.
echo Inhalt:
echo - Komplettes GUI-Projekt
echo - Core Models & Settings
echo - Service-Schnittstellen
echo - Projekt-Dokumentation
echo - Context für Handover
echo ======================================
echo.
pause