@echo off
setlocal enabledelayedexpansion

:: CamBridge Intelligent Source Collector
:: © 2025 Claude's Improbably Reliable Software Solutions
:: Ein Script, viele Profile!

:: === KONFIGURATION ===
set VERSION=2.0
set DEFAULT_PROFILE=balanced
set OUTPUT_DIR=PROJECT_CONTEXT
set TIMESTAMP=%date:~-4%%date:~-10,2%%date:~-7,2%_%time:~0,2%%time:~3,2%%time:~6,2%
set TIMESTAMP=!TIMESTAMP: =0!

:: === PARAMETER VERARBEITUNG ===
set PROFILE=%1
if "%PROFILE%"=="" set PROFILE=%DEFAULT_PROFILE%

:: === HAUPTLOGIK ===
cls
call :show_header

if "%PROFILE%"=="list" (
    call :list_profiles
    goto :end
)

if "%PROFILE%"=="help" (
    call :show_help
    goto :end
)

:: Prüfe ob Profil existiert
set "PROFILE_EXISTS="
for %%P in (minimal core gui balanced mapping full custom) do (
    if "%%P"=="%PROFILE%" set PROFILE_EXISTS=1
)

if not defined PROFILE_EXISTS (
    echo [ERROR] Unknown profile: %PROFILE%
    echo.
    call :list_profiles
    goto :end
)

:: Führe Collection aus
call :collect_%PROFILE%
goto :end

:: ========================================
:: MINIMAL PROFILE (~5%)
:: ========================================
:collect_minimal
set OUTPUT_FILE=%OUTPUT_DIR%_MINIMAL_%TIMESTAMP%.md
set /a COUNT=0

call :start_output "Minimal" "~5%%" "Quick overview"

:: Nur absolute Basics
call :collect_files "basics" "PROJECT_WISDOM.md,Version.props,CHANGELOG.md,README.md"
call :collect_files "solution" "CamBridge.sln"

call :create_summary "Minimal collection for quick checks"
call :show_stats
exit /b

:: ========================================
:: CORE PROFILE (~15%)
:: ========================================
:collect_core
set OUTPUT_FILE=%OUTPUT_DIR%_CORE_%TIMESTAMP%.md
set /a COUNT=0

call :start_output "Core" "~15%%" "Core functionality without GUI"

:: Basics + Core + Infrastructure
call :collect_files "basics" "PROJECT_WISDOM.md,Version.props,CHANGELOG.md"
call :collect_pattern "core" "src\CamBridge.Core\*.cs"
call :collect_pattern "models" "src\CamBridge.Core\Models\*.cs"
call :collect_pattern "interfaces" "src\CamBridge.Core\Interfaces\*.cs"
call :collect_pattern "infrastructure" "src\CamBridge.Infrastructure\Services\*Parser*.cs"
call :collect_pattern "extractors" "src\CamBridge.Infrastructure\Extractors\*.cs"
call :collect_pattern "converters" "src\CamBridge.Infrastructure\Converters\*.cs"

call :create_summary "Core components for JPEG to DICOM processing"
call :show_stats
exit /b

:: ========================================
:: GUI PROFILE (~20%)
:: ========================================
:collect_gui
set OUTPUT_FILE=%OUTPUT_DIR%_GUI_%TIMESTAMP%.md
set /a COUNT=0

call :start_output "GUI" "~20%%" "GUI components and configuration"

:: Basics + All GUI
call :collect_files "basics" "PROJECT_WISDOM.md,Version.props,CHANGELOG.md"
call :collect_recursive "gui" "src\CamBridge.Config" "*.cs,*.xaml,*.xaml.cs,*.csproj"
call :collect_files "settings" "src\CamBridge.Core\CamBridgeSettings.cs,src\CamBridge.Core\ProcessingOptions.cs"
call :collect_files "notifications" "src\CamBridge.Core\NotificationSettings.cs"

call :create_summary "GUI development with Settings focus"
call :show_stats
exit /b

:: ========================================
:: BALANCED PROFILE (~25%)
:: ========================================
:collect_balanced
set OUTPUT_FILE=%OUTPUT_DIR%_BALANCED_%TIMESTAMP%.md
set /a COUNT=0

call :start_output "Balanced" "~25%%" "Balanced coverage for most tasks"

:: Alles wichtige aber nicht alles
call :collect_files "basics" "PROJECT_WISDOM.md,Version.props,CHANGELOG.md,README.md"
call :collect_recursive "gui" "src\CamBridge.Config" "*.cs,*.xaml"
call :collect_pattern "core" "src\CamBridge.Core\*.cs"
call :collect_pattern "services" "src\CamBridge.Infrastructure\Services\*Service.cs"
call :collect_pattern "interfaces" "src\CamBridge.Infrastructure\Services\I*Service.cs"
call :collect_files "config" "src\CamBridge.Service\appsettings.json,src\CamBridge.Service\mappings.json"
call :collect_files "service" "src\CamBridge.Service\Program.cs,src\CamBridge.Service\Worker.cs"

call :create_summary "Balanced collection for general development"
call :show_stats
exit /b

:: ========================================
:: MAPPING PROFILE (~20%)
:: ========================================
:collect_mapping
set OUTPUT_FILE=%OUTPUT_DIR%_MAPPING_%TIMESTAMP%.md
set /a COUNT=0

call :start_output "Mapping" "~20%%" "Mapping Editor development"

:: Mapping-spezifisch
call :collect_files "basics" "PROJECT_WISDOM.md,Version.props"
call :collect_pattern "mapping" "src\CamBridge.Core\*Mapping*.cs"
call :collect_pattern "rules" "src\CamBridge.Core\MappingRule.cs"
call :collect_pattern "dicom" "src\CamBridge.Core\ValueObjects\DicomTag*.cs"
call :collect_pattern "loader" "src\CamBridge.Infrastructure\Services\*Mapping*.cs"
call :collect_files "config" "src\CamBridge.Service\mappings.json"
call :collect_files "editor" "src\CamBridge.Config\Views\MappingEditorPage.*"
call :collect_files "viewmodel" "src\CamBridge.Config\ViewModels\MappingEditorViewModel.cs"

call :create_summary "Mapping Editor implementation focus"
call :show_stats
exit /b

:: ========================================
:: FULL PROFILE (~50%)
:: ========================================
:collect_full
set OUTPUT_FILE=%OUTPUT_DIR%_FULL_%TIMESTAMP%.md
set /a COUNT=0

echo [WARNING] Full profile collects ~50%% of the project!
echo This might exceed token limits. Continue? (Y/N)
set /p CONFIRM=
if /i not "%CONFIRM%"=="Y" exit /b

call :start_output "Full" "~50%%" "Complete project overview"

:: Wirklich alles wichtige
call :collect_recursive "src" "src" "*.cs,*.xaml,*.csproj,*.json"
call :collect_files "docs" "*.md"
call :collect_files "scripts" "*.bat,*.ps1"

call :create_summary "Full collection - USE WITH CAUTION"
call :show_stats
exit /b

:: ========================================
:: CUSTOM PROFILE
:: ========================================
:collect_custom
echo Custom profile - specify pattern:
echo Example: *.cs,*.xaml under src\CamBridge.Config
echo.
set /p PATTERN=Pattern: 
set /p PATH=Path: 

set OUTPUT_FILE=%OUTPUT_DIR%_CUSTOM_%TIMESTAMP%.md
set /a COUNT=0

call :start_output "Custom" "Variable" "User-defined collection"
call :collect_recursive "custom" "%PATH%" "%PATTERN%"
call :create_summary "Custom collection"
call :show_stats
exit /b

:: ========================================
:: HILFSFUNKTIONEN
:: ========================================
:collect_files
:: %1 = category name, %2 = comma-separated file list
echo [%1] Collecting specific files...
for %%F in (%~2) do (
    if exist "%%~F" (
        echo ## %%~F >> "%OUTPUT_FILE%"
        echo ```>> "%OUTPUT_FILE%"
        type "%%~F" >> "%OUTPUT_FILE%" 2>nul
        echo ```>> "%OUTPUT_FILE%"
        echo. >> "%OUTPUT_FILE%"
        set /a COUNT+=1
        echo     [+] %%~F
    )
)
exit /b

:collect_pattern
:: %1 = category name, %2 = file pattern
echo [%1] Collecting pattern %2...
for %%F in (%2) do (
    if exist "%%F" (
        echo ## %%~nxF >> "%OUTPUT_FILE%"
        echo ```>> "%OUTPUT_FILE%"
        type "%%F" >> "%OUTPUT_FILE%" 2>nul
        echo ```>> "%OUTPUT_FILE%"
        echo. >> "%OUTPUT_FILE%"
        set /a COUNT+=1
        echo     [+] %%~nxF
    )
)
exit /b

:collect_recursive
:: %1 = category, %2 = path, %3 = patterns
echo [%1] Collecting from %2...
for %%E in (%~3) do (
    for /r "%~2" %%F in (%%E) do (
        echo %%F | findstr /i "\\obj\\ \\bin\\ \\packages\\ wpftmp \\publish\\" >nul
        if !errorlevel! neq 0 (
            echo ## %%~nxF >> "%OUTPUT_FILE%"
            echo Path: %%F >> "%OUTPUT_FILE%"
            echo ```>> "%OUTPUT_FILE%"
            type "%%F" >> "%OUTPUT_FILE%" 2>nul
            echo ```>> "%OUTPUT_FILE%"
            echo. >> "%OUTPUT_FILE%"
            set /a COUNT+=1
            echo     [+] %%~nxF
        )
    )
)
exit /b

:start_output
:: %1 = profile name, %2 = coverage, %3 = description
echo # CamBridge Context - %1 Profile > "%OUTPUT_FILE%"
echo Generated: %date% %time% >> "%OUTPUT_FILE%"
echo Coverage: %2 >> "%OUTPUT_FILE%"
echo Purpose: %3 >> "%OUTPUT_FILE%"
echo Version: CamBridge v0.5.1 >> "%OUTPUT_FILE%"
echo. >> "%OUTPUT_FILE%"
exit /b

:create_summary
echo. >> "%OUTPUT_FILE%"
echo ## Summary >> "%OUTPUT_FILE%"
echo - Profile: %PROFILE% >> "%OUTPUT_FILE%"
echo - Files collected: %COUNT% >> "%OUTPUT_FILE%"
echo - Purpose: %~1 >> "%OUTPUT_FILE%"
echo - Next steps: Check PROJECT_WISDOM.md for current tasks >> "%OUTPUT_FILE%"
exit /b

:show_header
echo ============================================
echo CamBridge Intelligent Source Collector v%VERSION%
echo © 2025 Claude's Improbably Reliable Software Solutions
echo ============================================
echo.
exit /b

:show_stats
echo.
echo ============================================
echo Collection Complete!
echo - Files collected: %COUNT%
echo - Output file: %OUTPUT_FILE%
echo - Size: 
for %%F in ("%OUTPUT_FILE%") do echo     %%~zF bytes
echo ============================================
echo.
echo Next steps:
echo 1. Upload PROJECT_WISDOM.md (always!)
echo 2. Upload %OUTPUT_FILE%
echo 3. Use VOGON INIT or explain your task
echo.
pause
exit /b

:list_profiles
echo Available profiles:
echo.
echo   minimal   - ~5%%   - Quick overview (PROJECT_WISDOM + basics)
echo   core      - ~15%%  - Core functionality without GUI
echo   gui       - ~20%%  - GUI components and configuration  
echo   balanced  - ~25%%  - General development (DEFAULT)
echo   mapping   - ~20%%  - Mapping Editor focus
echo   full      - ~50%%  - Complete overview (CAUTION: Token limit!)
echo   custom    - ???   - Define your own pattern
echo.
echo Usage: collect-sources.bat [profile]
echo Example: collect-sources.bat gui
echo.
exit /b

:show_help
echo CamBridge Source Collector - Help
echo.
echo This tool creates focused snapshots of the project for AI assistance.
echo.
echo USAGE:
echo   collect-sources.bat [profile]
echo.
echo COMMANDS:
echo   list     Show all available profiles
echo   help     Show this help
echo.
echo PROFILES:
echo   Run 'collect-sources.bat list' for details
echo.
echo OUTPUT:
echo   Creates PROJECT_CONTEXT_[PROFILE]_[TIMESTAMP].md
echo.
echo TIPS:
echo   - Use 'minimal' for quick chats
echo   - Use 'balanced' for most development (default)
echo   - Use 'full' only when necessary (token limit!)
echo   - Always upload PROJECT_WISDOM.md first
echo   - Use VOGON INIT for automatic start
echo.
exit /b

:end
endlocal