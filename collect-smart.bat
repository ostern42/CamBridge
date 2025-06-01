@echo off
:: collect-smart.bat - Intelligente Profil-Auswahl basierend auf Git-Änderungen
:: © 2025 Claude's Improbably Reliable Software Solutions

setlocal enabledelayedexpansion

echo ============================================
echo CamBridge Smart Collector
echo ============================================

:: Prüfe ob collect-sources.bat existiert
if not exist "collect-sources.bat" (
    echo.
    echo [ERROR] collect-sources.bat not found!
    echo.
    echo Please save the following files first:
    echo 1. collect-sources.bat - The main collector script
    echo 2. collect-smart.bat  - This smart selector
    echo.
    echo These are provided as artifacts in the Claude chat.
    echo Save them in: %CD%
    echo.
    pause
    exit /b 1
)

echo Analyzing recent changes...
echo.

:: Prüfe ob Git verfügbar ist
git --version >nul 2>&1
if errorlevel 1 (
    echo [WARNING] Git not found - using default profile
    call collect-sources.bat balanced
    exit /b
)

:: Sammle geänderte Dateien
set GUI_CHANGES=0
set MAPPING_CHANGES=0
set CORE_CHANGES=0
set SERVICE_CHANGES=0

:: Checke uncommitted changes
for /f "tokens=*" %%A in ('git status --porcelain 2^>nul') do (
    echo %%A | findstr /i "\.xaml ViewModel Views\\" >nul
    if !errorlevel! equ 0 set GUI_CHANGES=1
    
    echo %%A | findstr /i "Mapping MappingRule" >nul
    if !errorlevel! equ 0 set MAPPING_CHANGES=1
    
    echo %%A | findstr /i "Core\\" >nul
    if !errorlevel! equ 0 set CORE_CHANGES=1
    
    echo %%A | findstr /i "Service\\" >nul
    if !errorlevel! equ 0 set SERVICE_CHANGES=1
)

:: Checke letzte Commits
for /f "tokens=*" %%A in ('git diff --name-only HEAD~3..HEAD 2^>nul') do (
    echo %%A | findstr /i "\.xaml ViewModel Views\\" >nul
    if !errorlevel! equ 0 set GUI_CHANGES=1
    
    echo %%A | findstr /i "Mapping MappingRule" >nul
    if !errorlevel! equ 0 set MAPPING_CHANGES=1
    
    echo %%A | findstr /i "Core\\" >nul
    if !errorlevel! equ 0 set CORE_CHANGES=1
    
    echo %%A | findstr /i "Service\\" >nul
    if !errorlevel! equ 0 set SERVICE_CHANGES=1
)

:: Entscheide welches Profil
if !MAPPING_CHANGES! equ 1 (
    echo [SMART] Detected Mapping changes - using 'mapping' profile
    call collect-sources.bat mapping
    exit /b
)

if !GUI_CHANGES! equ 1 (
    echo [SMART] Detected GUI changes - using 'gui' profile
    call collect-sources.bat gui
    exit /b
)

if !CORE_CHANGES! equ 1 (
    if !SERVICE_CHANGES! equ 0 (
        echo [SMART] Detected Core changes - using 'core' profile
        call collect-sources.bat core
        exit /b
    )
)

:: Prüfe aktuelle Version für Hinweise
for /f "tokens=2 delims=<>" %%V in ('findstr "<AssemblyVersion>" Version.props 2^>nul') do (
    set VERSION=%%V
)

if defined VERSION (
    echo Current version: %VERSION%
    echo %VERSION% | findstr /i "0.5.1" >nul
    if !errorlevel! equ 0 (
        echo [SMART] v0.5.1 has build errors - checking NotificationService...
        if exist "src\CamBridge.Infrastructure\Services\NotificationService.cs" (
            echo [SMART] Using 'balanced' profile to include error context
            call collect-sources.bat balanced
            exit /b
        )
    )
)

:: Default
echo [SMART] No specific pattern detected - using 'balanced' profile
call collect-sources.bat balanced

:check_collector
:: Prüfe ob collect-sources.bat existiert
if not exist "collect-sources.bat" (
    echo.
    echo [ERROR] collect-sources.bat not found!
    echo.
    echo Please save the following files first:
    echo 1. collect-sources.bat
    echo 2. collect-smart.bat
    echo.
    echo These are provided as artifacts in the chat.
    echo.
    pause
    exit /b 1
)