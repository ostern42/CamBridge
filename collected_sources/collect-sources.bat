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

REM ==================================================
REM Core Projekt
REM ==================================================
call :CopyFile "src\CamBridge.Core\CamBridge.Core.csproj" "src_CamBridge.Core"
call :CopyFile "src\CamBridge.Core\GlobalUsings.cs" "src_CamBridge.Core"
call :CopyFile "src\CamBridge.Core\AssemblyInfo.cs" "src_CamBridge.Core"
call :CopyFile "src\CamBridge.Core\CamBridgeSettings.cs" "src_CamBridge.Core"
call :CopyFile "src\CamBridge.Core\ProcessingOptions.cs" "src_CamBridge.Core"
call :CopyFile "src\CamBridge.Core\MappingRule.cs" "src_CamBridge.Core"
call :CopyFile "src\CamBridge.Core\NotificationSettings.cs" "src_CamBridge.Core"
call :CopyFile "src\CamBridge.Core\ProcessingSummary.cs" "src_CamBridge.Core"

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

REM ==================================================
REM Infrastructure Projekt
REM ==================================================
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
call :CopyFile "src\CamBridge.Infrastructure\Services\DeadLetterQueue.cs" "src_CamBridge.Infrastructure_Services"
call :CopyFile "src\CamBridge.Infrastructure\Services\NotificationService.cs" "src_CamBridge.Infrastructure_Services"

REM ==================================================
REM Service Projekt
REM ==================================================
call :CopyFile "src\CamBridge.Service\CamBridge.Service.csproj" "src_CamBridge.Service"
call :CopyFile "src\CamBridge.Service\AssemblyInfo.cs" "src_CamBridge.Service"
call :CopyFile "src\CamBridge.Service\Program.cs" "src_CamBridge.Service"
call :CopyFile "src\CamBridge.Service\Worker.cs" "src_CamBridge.Service"
call :CopyFile "src\CamBridge.Service\CamBridgeHealthCheck.cs" "src_CamBridge.Service"
call :CopyFile "src\CamBridge.Service\DailySummaryService.cs" "src_CamBridge.Service"
call :CopyFile "src\CamBridge.Service\appsettings.json" "src_CamBridge.Service"
call :CopyFile "src\CamBridge.Service\appsettings.Development.json" "src_CamBridge.Service"
call :CopyFile "src\CamBridge.Service\mappings.json" "src_CamBridge.Service"
call :CopyFile "src\CamBridge.Service\dashboard.html" "src_CamBridge.Service"

REM Service Controllers
call :CopyFile "src\CamBridge.Service\Controllers\StatusController.cs" "src_CamBridge.Service_Controllers"

REM Service Properties
call :CopyFile "src\CamBridge.Service\Properties\launchSettings.json" "src_CamBridge.Service_Properties"

REM ==================================================
REM Config Projekt (WPF GUI)
REM ==================================================
call :CopyFile "src\CamBridge.Config\CamBridge.Config.csproj" "src_CamBridge.Config"
call :CopyFile "src\CamBridge.Config\CamBridge.Config.csproj.user" "src_CamBridge.Config"
call :CopyFile "src\CamBridge.Config\AssemblyInfo.cs" "src_CamBridge.Config"
call :CopyFile "src\CamBridge.Config\App.xaml" "src_CamBridge.Config"
call :CopyFile "src\CamBridge.Config\App.xaml.cs" "src_CamBridge.Config"
call :CopyFile "src\CamBridge.Config\MainWindow.xaml" "src_CamBridge.Config"
call :CopyFile "src\CamBridge.Config\MainWindow.xaml.cs" "src_CamBridge.Config"
call :CopyFile "src\CamBridge.Config\Package.appxmanifest" "src_CamBridge.Config"
call :CopyFile "src\CamBridge.Config\app.manifest" "src_CamBridge.Config"
call :CopyFile "src\CamBridge.Config\Directory.Build.props" "src_CamBridge.Config"
call :CopyFile "src\CamBridge.Config\global.json" "src_CamBridge.Config"

REM Config Properties
call :CopyFile "src\CamBridge.Config\Properties\launchSettings.json" "src_CamBridge.Config_Properties"

REM Config ViewModels
call :CopyFile "src\CamBridge.Config\ViewModels\ViewModelBase.cs" "src_CamBridge.Config_ViewModels"
call :CopyFile "src\CamBridge.Config\ViewModels\MainViewModel.cs" "src_CamBridge.Config_ViewModels"
call :CopyFile "src\CamBridge.Config\ViewModels\DashboardViewModel.cs" "src_CamBridge.Config_ViewModels"
call :CopyFile "src\CamBridge.Config\ViewModels\ServiceControlViewModel.cs" "src_CamBridge.Config_ViewModels"
call :CopyFile "src\CamBridge.Config\ViewModels\SettingsViewModel.cs" "src_CamBridge.Config_ViewModels"

REM Config Views
call :CopyFile "src\CamBridge.Config\Views\DashboardPage.xaml" "src_CamBridge.Config_Views"
call :CopyFile "src\CamBridge.Config\Views\DashboardPage.xaml.cs" "src_CamBridge.Config_Views"
call :CopyFile "src\CamBridge.Config\Views\ServiceControlPage.xaml" "src_CamBridge.Config_Views"
call :CopyFile "src\CamBridge.Config\Views\ServiceControlPage.xaml.cs" "src_CamBridge.Config_Views"
call :CopyFile "src\CamBridge.Config\Views\DeadLetterPage.xaml" "src_CamBridge.Config_Views"
call :CopyFile "src\CamBridge.Config\Views\DeadLetterPage.xaml.cs" "src_CamBridge.Config_Views"
call :CopyFile "src\CamBridge.Config\Views\SettingsPage.xaml" "src_CamBridge.Config_Views"
call :CopyFile "src\CamBridge.Config\Views\SettingsPage.xaml.cs" "src_CamBridge.Config_Views"

REM Config Services
call :CopyFile "src\CamBridge.Config\Services\ConfigurationService.cs" "src_CamBridge.Config_Services"
call :CopyFile "src\CamBridge.Config\Services\IConfigurationService.cs" "src_CamBridge.Config_Services"
call :CopyFile "src\CamBridge.Config\Services\NavigationService.cs" "src_CamBridge.Config_Services"
call :CopyFile "src\CamBridge.Config\Services\INavigationService.cs" "src_CamBridge.Config_Services"
call :CopyFile "src\CamBridge.Config\Services\ServiceManager.cs" "src_CamBridge.Config_Services"
call :CopyFile "src\CamBridge.Config\Services\IServiceManager.cs" "src_CamBridge.Config_Services"

REM ==================================================
REM Tests Projekt
REM ==================================================
call :CopyFile "tests\CamBridge.Infrastructure.Tests\CamBridge.Infrastructure.Tests.csproj" "tests_CamBridge.Infrastructure.Tests"
call :CopyFile "tests\CamBridge.Infrastructure.Tests\GlobalUsings.cs" "tests_CamBridge.Infrastructure.Tests"
call :CopyFile "tests\CamBridge.Infrastructure.Tests\ProcessingQueueTests.cs" "tests_CamBridge.Infrastructure.Tests"
call :CopyFile "tests\CamBridge.Infrastructure.Tests\DicomConverterTests.cs" "tests_CamBridge.Infrastructure.Tests"
call :CopyFile "tests\CamBridge.Infrastructure.Tests\DicomTagMapperTests.cs" "tests_CamBridge.Infrastructure.Tests"
call :CopyFile "tests\CamBridge.Infrastructure.Tests\MappingConfigurationTests.cs" "tests_CamBridge.Infrastructure.Tests"
call :CopyFile "tests\CamBridge.Infrastructure.Tests\FileProcessorTests.cs" "tests_CamBridge.Infrastructure.Tests"
call :CopyFile "tests\CamBridge.Infrastructure.Tests\TestLoggerBase.cs" "tests_CamBridge.Infrastructure.Tests"

REM Test Services
call :CopyFile "tests\CamBridge.Infrastructure.Tests\Services\ExifReaderTests.cs" "tests_CamBridge.Infrastructure.Tests_Services"

REM Test IntegrationTests
call :CopyFile "tests\CamBridge.Infrastructure.Tests\IntegrationTests\ErrorHandlingTests.cs" "tests_CamBridge.Infrastructure.Tests_IntegrationTests"
call :CopyFile "tests\CamBridge.Infrastructure.Tests\IntegrationTests\JpegToDicomIntegrationTests.cs" "tests_CamBridge.Infrastructure.Tests_IntegrationTests"

REM Test TestHelpers
call :CopyFile "tests\CamBridge.Infrastructure.Tests\TestHelpers\JpegTestFileGenerator.cs" "tests_CamBridge.Infrastructure.Tests_TestHelpers"

REM ==================================================
REM PowerShell und andere Skripte
REM ==================================================
call :CopyFile "Install-CamBridge.ps1" "scripts"
call :CopyFile "Run-Tests.ps1" "scripts"
call :CopyFile "Build-Release.ps1" "scripts"

REM ==================================================
REM Entwicklungsdokumentation
REM ==================================================
call :CopyFile "cambridge-entwicklungsplan-v2.md" "docs"
call :CopyFile "handover-prompt-phase8.md" "docs"

echo.
echo ======================================
echo Sammlung abgeschlossen!
echo.
echo Gesammelte Dateien: %TARGET_DIR%
echo.
echo Die Dateien sind nach folgendem Schema benannt:
echo - Projekt_Unterordner_Dateiname.ext
echo.
echo Beispiele:
echo - src_CamBridge.Core_CamBridge.Core.csproj
echo - src_CamBridge.Config_Views_DashboardPage.xaml
echo ======================================
echo.
pause
goto :eof

REM ======================================
REM Funktion zum Kopieren und Umbenennen
REM ======================================
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
    if !errorlevel! equ 0 (
        echo [OK] %SOURCE_FILE%
    ) else (
        echo [FEHLER] %SOURCE_FILE% - Kopieren fehlgeschlagen
    )
) else (
    echo [--] %SOURCE_FILE% (nicht gefunden)
)
goto :eof
