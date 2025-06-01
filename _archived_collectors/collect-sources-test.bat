@echo off
REM CamBridge Source Collector - Core Test Version
REM © 2025 Claude's Improbably Reliable Software Solutions
REM Optimiert für Core-Funktionalität Tests ohne GUI

setlocal enabledelayedexpansion

set "TARGET_DIR=collected_sources_core_test"
set /a COUNT=0

if exist "%TARGET_DIR%" rmdir /s /q "%TARGET_DIR%"
mkdir "%TARGET_DIR%"

echo ======================================
echo CamBridge Core Test Source Collector
echo Version: Core Testing Focus
echo Coverage: ~10-15%% (no GUI)
echo ======================================
echo.

REM === PROJEKT-GRUNDLAGEN ===
echo [1/7] Sammle Projekt-Grundlagen...
for %%F in (CamBridge.sln Version.props PROJECT_WISDOM.md CHANGELOG.md README.md) do (
    if exist "%%F" (
        copy "%%F" "%TARGET_DIR%\%%F" >nul 2>&1
        if !errorlevel! equ 0 (
            echo [OK] %%F
            set /a COUNT+=1
        )
    )
)

REM === CORE PROJEKT ===
echo.
echo [2/7] Sammle Core Models und Interfaces...
for %%F in (
    "src\CamBridge.Core\CamBridge.Core.csproj"
    "src\CamBridge.Core\*.cs"
    "src\CamBridge.Core\Models\*.cs"
    "src\CamBridge.Core\Interfaces\*.cs"
) do (
    if exist %%F (
        for %%G in (%%F) do (
            set "FILE=%%~G"
            set "NAME=!FILE:\=_!"
            copy "%%~G" "%TARGET_DIR%\!NAME!" >nul 2>&1
            if !errorlevel! equ 0 (
                echo [OK] %%~G
                set /a COUNT+=1
            )
        )
    )
)

REM === INFRASTRUCTURE IMPLEMENTATION ===
echo.
echo [3/7] Sammle Infrastructure Implementation...
for %%F in (
    "src\CamBridge.Infrastructure\CamBridge.Infrastructure.csproj"
    "src\CamBridge.Infrastructure\Extractors\*.cs"
    "src\CamBridge.Infrastructure\Converters\*.cs"
    "src\CamBridge.Infrastructure\Services\*.cs"
    "src\CamBridge.Infrastructure\Mappers\*.cs"
) do (
    if exist %%F (
        for %%G in (%%F) do (
            set "FILE=%%~G"
            set "NAME=!FILE:\=_!"
            copy "%%~G" "%TARGET_DIR%\!NAME!" >nul 2>&1
            if !errorlevel! equ 0 (
                echo [OK] %%~G
                set /a COUNT+=1
            )
        )
    )
)

REM === TESTS ===
echo.
echo [4/7] Sammle alle Tests...
for /r tests %%F in (*.cs *.csproj) do (
    echo %%F | findstr /i "\\obj\\ \\bin\\" >nul
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

REM === TEST HELPERS ===
echo.
echo [5/7] Sammle Test Helpers und Fixtures...
for %%F in (
    "tests\TestHelpers\*.cs"
    "tests\Fixtures\*.cs"
) do (
    if exist %%F (
        for %%G in (%%F) do (
            set "FILE=%%~G"
            set "NAME=!FILE:\=_!"
            copy "%%~G" "%TARGET_DIR%\!NAME!" >nul 2>&1
            if !errorlevel! equ 0 (
                echo [OK] %%~G
                set /a COUNT+=1
            )
        )
    )
)

REM === KONFIGURATIONS-DATEIEN ===
echo.
echo [6/7] Sammle Test-relevante Konfiguration...
for %%F in (
    "src\CamBridge.Service\mappings.json"
    "src\CamBridge.Service\appsettings.json"
    "tests\TestData\*.*"
) do (
    if exist %%F (
        for %%G in (%%F) do (
            set "FILE=%%~G"
            set "NAME=!FILE:\=_!"
            copy "%%~G" "%TARGET_DIR%\!NAME!" >nul 2>&1
            if !errorlevel! equ 0 (
                echo [OK] %%~G
                set /a COUNT+=1
            )
        )
    )
)

REM === CONTEXT DOKUMENT ===
echo.
echo [7/7] Erstelle Context-Dokument...
(
echo # CamBridge Core Test Context
echo © 2025 Claude's Improbably Reliable Software Solutions
echo.
echo ## Ziel dieses Chats
echo Test der Core-Funktionalität:
echo - JPEG mit QRBridge-Daten verarbeiten
echo - EXIF-Extraktion prüfen
echo - DICOM-Konvertierung testen
echo - Mapping-Konfiguration validieren
echo.
echo ## Wichtige Test-Klassen
echo - JpegToExifIntegrationTests.cs
echo - DicomConverterTests.cs
echo - FileProcessorTests.cs
echo - MappingConfigurationTests.cs
echo.
echo ## Test-Daten
echo Suche nach TestData Ordner oder Sample JPEGs
echo.
echo ## Keine GUI!
echo Dieser Chat fokussiert auf Core ohne GUI-Komponenten
) > "%TARGET_DIR%\CORE_TEST_CONTEXT.md"
set /a COUNT+=1

echo.
echo ======================================
echo Core Test Collection abgeschlossen!
echo !COUNT! Dateien gesammelt
echo.
echo Fokus: Core-Funktionalität testen
echo Ausgeschlossen: GUI, Config Tool
echo ======================================
echo.
pause