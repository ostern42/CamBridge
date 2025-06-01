@echo off
REM CamBridge Ricoh JPEG Test Script
REM © 2025 Claude's Improbably Reliable Software Solutions

REM Save current directory
set ORIGINAL_DIR=%CD%

echo.
echo ============================================
echo CamBridge Ricoh JPEG Test
echo ============================================
echo.

REM Check if parameter provided
if "%~1"=="" (
    echo ERROR: Please provide path to Ricoh JPEG file
    echo.
    echo Usage: test-ricoh-jpeg.bat [path-to-jpeg]
    echo Example: test-ricoh-jpeg.bat C:\Images\ricoh_test.jpg
    echo.
    cd /d "%ORIGINAL_DIR%"
    goto :end
)

REM Check if file exists
if not exist "%~1" (
    echo ERROR: File not found: %~1
    echo.
    cd /d "%ORIGINAL_DIR%"
    goto :end
)

REM Create test directory structure
set TEST_DIR=%~dp0tests\CamBridge.TestConsole
if not exist "%TEST_DIR%" (
    echo Creating test console directory...
    mkdir "%TEST_DIR%"
)

REM Build the test console
echo Building test console application...
cd /d "%TEST_DIR%"
dotnet build -c Release

if errorlevel 1 (
    echo.
    echo ERROR: Build failed!
    cd /d "%ORIGINAL_DIR%"
    goto :end
)

REM Copy mappings.json if exists
if exist "%~dp0src\CamBridge.Service\mappings.json" (
    echo Copying mappings.json...
    copy /Y "%~dp0src\CamBridge.Service\mappings.json" "%TEST_DIR%\bin\Release\net8.0-windows\" >nul 2>&1
    copy /Y "%~dp0src\CamBridge.Service\mappings.json" "%TEST_DIR%\bin\Release\net8.0\" >nul 2>&1
)

REM Run the test
echo.
echo Running test with: %~nx1
echo ============================================
cd bin\Release\net8.0-windows 2>nul || cd bin\Release\net8.0
CamBridge.TestConsole.exe "%~1" 2>nul || dotnet CamBridge.TestConsole.dll "%~1"

:end
REM Always return to original directory
cd /d "%ORIGINAL_DIR%"
pause