@echo off
REM CamBridge Source Collector - Minimal Version für GUI-Entwicklung
REM © 2025 Claude's Improbably Reliable Software Solutions

setlocal enabledelayedexpansion

set "TARGET_DIR=collected_sources_minimal"
set /a COUNT=0

if exist "%TARGET_DIR%" rmdir /s /q "%TARGET_DIR%"
mkdir "%TARGET_DIR%"

echo ======================================
echo CamBridge Source Collector (Minimal)
echo Version: GUI Development (v0.4.x)
echo ======================================
echo.

REM === KRITISCHE PROJEKT-DATEIEN ===
echo [1/5] Sammle Projekt-Dateien...
for %%F in (*.sln Version.props cambridge-entwicklungsplan-v2.md) do (
    if exist "%%F" (
        copy "%%F" "%TARGET_DIR%\%%F" >nul 2>&1
        if !errorlevel! equ 0 set /a COUNT+=1
    )
)

REM === GUI CONFIG PROJEKT ===
echo [2/5] Sammle GUI-Dateien (CamBridge.Config)...
for /r src\CamBridge.Config %%F in (*.cs *.xaml *.csproj *.manifest) do (
    echo %%F | findstr /i "\\obj\\ \\bin\\ wpftmp" >nul
    if !errorlevel! neq 0 (
        set "FILE=%%F"
        set "REL=!FILE:%CD%\=!"
        set "NAME=!REL:\=_!"
        copy "%%F" "%TARGET_DIR%\!NAME!" >nul 2>&1
        if !errorlevel! equ 0 set /a COUNT+=1
    )
)

REM === KRITISCHE SERVICE-DATEIEN ===
echo [3/5] Sammle kritische Service-Dateien...
for %%F in (
    "src\CamBridge.Service\Program.cs"
    "src\CamBridge.Service\Worker.cs"
    "src\CamBridge.Service\Controllers\StatusController.cs"
    "src\CamBridge.Service\appsettings.json"
    "src\CamBridge.Service\CamBridge.Service.csproj"
) do (
    if exist %%F (
        set "FILE=%%~F"
        set "NAME=!FILE:\=_!"
        copy %%F "%TARGET_DIR%\!NAME!" >nul 2>&1
        if !errorlevel! equ 0 set /a COUNT+=1
    )
)

REM === CORE INTERFACES (für GUI) ===
echo [4/5] Sammle Core-Interfaces...
for %%F in (
    "src\CamBridge.Core\CamBridge.Core.csproj"
    "src\CamBridge.Core\CamBridgeSettings.cs"
    "src\CamBridge.Core\ProcessingOptions.cs"
    "src\CamBridge.Core\NotificationSettings.cs"
) do (
    if exist %%F (
        set "FILE=%%~F"
        set "NAME=!FILE:\=_!"
        copy %%F "%TARGET_DIR%\!NAME!" >nul 2>&1
        if !errorlevel! equ 0 set /a COUNT+=1
    )
)

REM === AKTUELLE DOKUMENTATION ===
echo [5/5] Sammle Dokumentation...
for %%F in (CHANGELOG.md README.md GUI-Readme.md) do (
    if exist "%%F" (
        copy "%%F" "%TARGET_DIR%\%%F" >nul 2>&1
        if !errorlevel! equ 0 set /a COUNT+=1
    )
)

echo.
echo ======================================
echo Minimal-Sammlung abgeschlossen!
echo !COUNT! Dateien gesammelt
echo.
echo Fokus: GUI-Entwicklung (Phase 8-10)
echo ======================================
echo.
pause