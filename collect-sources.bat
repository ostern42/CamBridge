@echo off
REM CamBridge Source Collector
REM © 2025 Claude's Improbably Reliable Software Solutions
REM Sammelt alle relevanten Dateien in einen flachen Ordner mit Pfadnamen

setlocal enabledelayedexpansion

REM Zielordner für gesammelte Dateien
set "TARGET_DIR=collected_sources"

REM Lösche und erstelle Zielordner
if exist "%TARGET_DIR%" rmdir /s /q "%TARGET_DIR%"
mkdir "%TARGET_DIR%"

echo Sammle CamBridge Quelldateien...
echo.

REM Solution-Dateien
call :CopyFile "CamBridge.sln" ""
call :CopyFile "Version.props" ""
call :CopyFile "README.md" ""
call :CopyFile "CHANGELOG.md" ""
call :CopyFile "LICENSE" ""
call :CopyFile ".gitignore" ""
call :CopyFile ".gitattributes" ""
call :CopyFile ".editorconfig" ""

REM Core Projekt
call :CopyFile "src\CamBridge.Core\CamBridge.Core.csproj" "src_CamBridge.Core"
call :CopyFile "src\CamBridge.Core\GlobalUsings.cs" "src_CamBridge.Core"
call :CopyFile "src\CamBridge.Core\AssemblyInfo.cs" "src_CamBridge.Core"
call :CopyFile "src\CamBridge.Core\CamBridgeSettings.cs" "src_CamBridge.Core"
call :CopyFile "src\CamBridge.Core\ProcessingOptions.cs" "src_CamBridge.Core"
call :CopyFile "src\CamBridge.Core\MappingRule.cs" "src_CamBridge.Core"

REM Core Entities
call :CopyFile "src\CamBridge.Core\Entities\ImageMetadata.cs" "src_CamBridge.Core_Entities"
call :CopyFile "src\CamBridge.Core\Entities\PatientInfo.cs" "src_CamBridge.Core_Entities"
call :CopyFile "src\CamBridge.Core\Entities\StudyInfo.cs" "src_CamBridge.Core_Entities"

REM Core Interfaces
call :CopyFile "src\CamBridge.Core\Interfaces\IDicomConverter.cs" "src_CamBridge.Core_Interfaces"
call :CopyFile "src\CamBridge.Core\Interfaces\IExifReader.cs" "src_CamBridge.Core_Interfaces"
call :CopyFile "src\CamBridge.Core\Interfaces\IFileProcessor.cs" "src_CamBridge.Core_Interfaces"
call :CopyFile "src\CamBridge.Core\Interfaces\IMappingConfiguration.cs" "src_CamBridge.Core_Interfaces"

REM Core ValueObjects
call :CopyFile "src\CamBridge.Core\ValueObjects\DicomTag.cs" "src_CamBridge.Core_ValueObjects"
call :CopyFile "src\CamBridge.Core\ValueObjects\ExifTag.cs" "src_CamBridge.Core_ValueObjects"
call :CopyFile "src\CamBridge.Core\ValueObjects\PatientId.cs" "src_CamBridge.Core_ValueObjects"
call :CopyFile "src\CamBridge.Core\ValueObjects\StudyId.cs" "src_CamBridge.Core_ValueObjects"

REM Infrastructure Projekt
call :CopyFile "src\CamBridge.Infrastructure\CamBridge.Infrastructure.csproj" "src_CamBridge.Infrastructure"
call :CopyFile "src\CamBridge.Infrastructure\GlobalUsings.cs" "src_CamBridge.Infrastructure"
call :CopyFile "src\CamBridge.Infrastructure\AssemblyInfo.cs" "src_CamBridge.Infrastructure"
call :CopyFile "src\CamBridge.Infrastructure\ServiceCollectionExtensions.cs" "src_CamBridge.Infrastructure"

REM Infrastructure Services
call :CopyFile "src\CamBridge.Infrastructure\Services\DicomConverter.cs" "src_CamBridge.Infrastructure_Services"
call :CopyFile "src\CamBridge.Infrastructure\Services\DicomTagMapper.cs" "src_CamBridge.Infrastructure_Services"
call :CopyFile "src\CamBridge.Infrastructure\Services\ExifReader.cs" "src_CamBridge.Infrastructure_Services"
call :CopyFile "src\CamBridge.Infrastructure\Services\RicohExifReader.cs" "src_CamBridge.Infrastructure_Services"
call :CopyFile "src\CamBridge.Infrastructure\Services\FileProcessor.cs" "src_CamBridge.Infrastructure_Services"
call :CopyFile "src\CamBridge.Infrastructure\Services\FolderWatcherService.cs" "src_CamBridge.Infrastructure_Services"
call :CopyFile "src\CamBridge.Infrastructure\Services\ProcessingQueue.cs" "src_CamBridge.Infrastructure_Services"
call :CopyFile "src\CamBridge.Infrastructure\Services\MappingConfigurationLoader.cs" "src_CamBridge.Infrastructure_Services"

REM Service Projekt
call :CopyFile "src\CamBridge.Service\CamBridge.Service.csproj" "src_CamBridge.Service"
call :CopyFile "src\CamBridge.Service\AssemblyInfo.cs" "src_CamBridge.Service"
call :CopyFile "src\CamBridge.Service\Program.cs" "src_CamBridge.Service"
call :CopyFile "src\CamBridge.Service\Worker.cs" "src_CamBridge.Service"
call :CopyFile "src\CamBridge.Service\CamBridgeHealthCheck.cs" "src_CamBridge.Service"
call :CopyFile "src\CamBridge.Service\appsettings.json" "src_CamBridge.Service"
call :CopyFile "src\CamBridge.Service\appsettings.Development.json" "src_CamBridge.Service"
call :CopyFile "src\CamBridge.Service\mappings.json" "src_CamBridge.Service"

REM Service Properties
call :CopyFile "src\CamBridge.Service\Properties\launchSettings.json" "src_CamBridge.Service_Properties"

REM Tests Projekt
call :CopyFile "tests\CamBridge.Infrastructure.Tests\CamBridge.Infrastructure.Tests.csproj" "tests_CamBridge.Infrastructure.Tests"
call :CopyFile "tests\CamBridge.Infrastructure.Tests\ProcessingQueueTests.cs" "tests_CamBridge.Infrastructure.Tests"
call :CopyFile "tests\CamBridge.Infrastructure.Tests\GlobalUsings.cs" "tests_CamBridge.Infrastructure.Tests"
call :CopyFile "tests\CamBridge.Infrastructure.Tests\Services\ExifReaderTests.cs" "tests_CamBridge.Infrastructure.Tests_Services"

echo.
echo Fertig! Alle Dateien befinden sich im Ordner: %TARGET_DIR%
echo.
pause
goto :eof

REM Funktion zum Kopieren und Umbenennen
:CopyFile
set "SOURCE_FILE=%~1"
set "PREFIX=%~2"

if exist "%SOURCE_FILE%" (
    REM Ersetze Backslashes durch Underscores für den Dateinamen
    set "FILENAME=%SOURCE_FILE:\=_%"
    
    REM Wenn ein Prefix angegeben ist, nutze ihn
    if not "%PREFIX%"=="" (
        set "FILENAME=%PREFIX%_%~nx1"
    )
    
    copy "%SOURCE_FILE%" "%TARGET_DIR%\!FILENAME!" >nul 2>&1
    echo [OK] %SOURCE_FILE%
) else (
    echo [--] %SOURCE_FILE% (nicht gefunden)
)
goto :eof