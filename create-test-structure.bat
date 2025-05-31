@echo off
echo Erstelle Test-Verzeichnisstruktur...

REM Basis Test-Verzeichnis
if not exist "tests" mkdir "tests"
if not exist "tests\CamBridge.Infrastructure.Tests" mkdir "tests\CamBridge.Infrastructure.Tests"

REM Services Unterordner (für ExifReaderTests.cs)
if not exist "tests\CamBridge.Infrastructure.Tests\Services" mkdir "tests\CamBridge.Infrastructure.Tests\Services"

echo.
echo Verzeichnisstruktur erstellt:
echo - tests\
echo   - CamBridge.Infrastructure.Tests\
echo     - Services\
echo.

REM Zeige vorhandene Test-Dateien
echo Vorhandene Test-Dateien:
if exist "tests\CamBridge.Infrastructure.Tests\Services\ExifReaderTests.cs" (
    echo [OK] ExifReaderTests.cs gefunden
) else (
    echo [!!] ExifReaderTests.cs nicht gefunden - bitte manuell verschieben
)

pause