@echo off
REM CamBridge Source Collector - Final Version
REM © 2025 Claude's Improbably Reliable Software Solutions
REM Optimierte Version ohne obj/bin Dateien

setlocal enabledelayedexpansion

set "TARGET_DIR=collected_sources"
set /a COUNT=0

if exist "%TARGET_DIR%" rmdir /s /q "%TARGET_DIR%"
mkdir "%TARGET_DIR%"

echo ======================================
echo CamBridge Source Collector (Final)
echo ======================================
echo.

REM Root-Dateien
echo Sammle Root-Dateien...
for %%F in (*.sln *.props *.md LICENSE .gitignore .gitattributes .editorconfig) do (
    if exist "%%F" (
        copy "%%F" "%TARGET_DIR%\%%F" >nul 2>&1
        if !errorlevel! equ 0 (
            echo [OK] %%F
            set /a COUNT+=1
        )
    )
)

REM Alle CS-Dateien (ohne obj/bin)
echo.
echo Sammle C# Dateien...
for /r src %%F in (*.cs) do (
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

REM Alle CSPROJ-Dateien
echo.
echo Sammle Projekt-Dateien...
for /r src %%F in (*.csproj *.props) do (
    set "FILE=%%F"
    set "REL=!FILE:%CD%\=!"
    set "NAME=!REL:\=_!"
    copy "%%F" "%TARGET_DIR%\!NAME!" >nul 2>&1
    if !errorlevel! equ 0 (
        echo [OK] !REL!
        set /a COUNT+=1
    )
)

REM Alle XAML-Dateien
echo.
echo Sammle XAML-Dateien...
for /r src %%F in (*.xaml) do (
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

REM Konfigurationsdateien
echo.
echo Sammle Konfigurations-Dateien...
for /r src %%F in (*.json *.manifest *.config) do (
    echo %%F | findstr /i "\\obj\\ \\bin\\ \\packages\\" >nul
    if !errorlevel! neq 0 (
        set "FILE=%%F"
        set "REL=!FILE:%CD%\=!"
        
        REM Spezialbehandlung für wichtige Config-Dateien
        echo !REL! | findstr /i "appsettings mappings launchSettings global.json app.manifest" >nul
        if !errorlevel! equ 0 (
            set "NAME=!REL:\=_!"
            copy "%%F" "%TARGET_DIR%\!NAME!" >nul 2>&1
            if !errorlevel! equ 0 (
                echo [OK] !REL!
                set /a COUNT+=1
            )
        )
    )
)

REM HTML/CSS/JS Dateien
echo.
echo Sammle Web-Dateien...
for /r src %%F in (*.html *.css *.js) do (
    set "FILE=%%F"
    set "REL=!FILE:%CD%\=!"
    set "NAME=!REL:\=_!"
    copy "%%F" "%TARGET_DIR%\!NAME!" >nul 2>&1
    if !errorlevel! equ 0 (
        echo [OK] !REL!
        set /a COUNT+=1
    )
)

REM Test-Dateien
echo.
echo Sammle Test-Dateien...
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

REM Scripts (nur im Root)
echo.
echo Sammle Script-Dateien...
for %%F in (*.ps1 *.bat) do (
    if exist "%%F" (
        echo %%F | findstr /i "collected_sources" >nul
        if !errorlevel! neq 0 (
            copy "%%F" "%TARGET_DIR%\%%F" >nul 2>&1
            if !errorlevel! equ 0 (
                echo [OK] %%F
                set /a COUNT+=1
            )
        )
    )
)

REM Dokumentation
echo.
echo Sammle Dokumentation...
for %%F in (docs\*.md *.md) do (
    if exist "%%F" (
        set "FILE=%%F"
        set "NAME=!FILE:\=_!"
        copy "%%F" "%TARGET_DIR%\!NAME!" >nul 2>&1
        if !errorlevel! equ 0 (
            echo [OK] %%F
            set /a COUNT+=1
        )
    )
)

REM Spezielle Service-Dateien
echo.
echo Sammle Service-Dateien...
if exist "src\CamBridge.Service\Controllers\StatusController.cs" (
    copy "src\CamBridge.Service\Controllers\StatusController.cs" "%TARGET_DIR%\src_CamBridge.Service_Controllers_StatusController.cs" >nul 2>&1
    if !errorlevel! equ 0 (
        echo [OK] src\CamBridge.Service\Controllers\StatusController.cs
        set /a COUNT+=1
    )
)

echo.
echo ======================================
echo Fertig! !COUNT! Dateien gesammelt
echo.
echo Wichtige Dateien:
echo   - C# Source Files (ohne obj/bin)
echo   - XAML UI Dateien  
echo   - Projekt-Dateien (.csproj)
echo   - Konfigurations-Dateien
echo   - Test-Dateien
echo   - Scripts und Dokumentation
echo.
echo Alle Dateien in: %TARGET_DIR%
echo ======================================
echo.
pause