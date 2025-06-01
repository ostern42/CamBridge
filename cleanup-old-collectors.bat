@echo off
:: cleanup-old-collectors.bat - Archiviert alte Collection Scripts
:: © 2025 Claude's Improbably Reliable Software Solutions

echo ============================================
echo CamBridge Collector Cleanup
echo ============================================
echo.
echo This will archive old collection scripts.
echo The new unified collector replaces:
echo - collect-sources-balanced.bat
echo - collect-sources-gui-config.bat
echo - collect-sources-mapping-editor.bat
echo - collect-sources-test.bat
echo.
echo Continue? (Y/N)
set /p CONFIRM=

if /i not "%CONFIRM%"=="Y" (
    echo Cleanup cancelled.
    exit /b
)

:: Erstelle Archiv-Ordner
set ARCHIVE_DIR=_archived_collectors
if not exist "%ARCHIVE_DIR%" mkdir "%ARCHIVE_DIR%"

echo.
echo Archiving old collectors...

:: Liste der zu archivierenden Scripts
set OLD_SCRIPTS=collect-sources-balanced.bat collect-sources-gui-config.bat collect-sources-mapping-editor.bat collect-sources-test.bat

:: Archiviere jeden
for %%F in (%OLD_SCRIPTS%) do (
    if exist "%%F" (
        echo Moving %%F to archive...
        move "%%F" "%ARCHIVE_DIR%\%%F" >nul 2>&1
        if !errorlevel! equ 0 (
            echo     [OK] %%F archived
        ) else (
            echo     [ERROR] Could not move %%F
        )
    )
)

:: Archiviere auch alte Output-Ordner
if exist "collected_sources_balanced" (
    echo Moving collected_sources_balanced folder...
    move "collected_sources_balanced" "%ARCHIVE_DIR%\" >nul 2>&1
)

if exist "collected_sources_core_test" (
    echo Moving collected_sources_core_test folder...
    move "collected_sources_core_test" "%ARCHIVE_DIR%\" >nul 2>&1
)

if exist "collected_sources_mapping_editor" (
    echo Moving collected_sources_mapping_editor folder...
    move "collected_sources_mapping_editor" "%ARCHIVE_DIR%\" >nul 2>&1
)

echo.
echo ============================================
echo Cleanup complete!
echo.
echo Old scripts archived in: %ARCHIVE_DIR%
echo.
echo New usage:
echo   collect-sources.bat         (uses 'balanced' profile)
echo   collect-sources.bat list    (show all profiles)
echo   collect-sources.bat gui     (GUI development)
echo   collect-sources.bat core    (Core testing)
echo   collect-smart.bat           (auto-detect profile)
echo ============================================
echo.
pause