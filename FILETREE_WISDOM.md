# WISDOM FILE TREE v0.8.10
**Generated**: 2025-07-02 01:01  
**Total Files**: 230  
**Your Daily Code Dashboard** - Everything important at a glance!

## RECENTLY MODIFIED (Last 7 days)

### Today [HOT]
- **src\CamBridge.Infrastructure\Services\PacsUploadQueue.cs** [Classes: PacsUploadQueue]
- **src\CamBridge.Infrastructure\Services\MappingConfigurationLoader.cs** [Classes: MappingConfigurationLoader]
- **src\CamBridge.Infrastructure\Services\DicomStoreService.cs** [Classes: StoreResult, DicomStoreService]
- **session110-correlation-sources.txt**
- **src\CamBridge.Infrastructure\Services\DicomConverter.cs** [Classes: DicomConverter, ConversionResult, ValidationResult]
- **src\CamBridge.Infrastructure\Services\DicomTagMapper.cs** [Classes: DicomTagMapper]
- **src\CamBridge.Infrastructure\Services\ExifToolReader.cs** [MONSTER] [Classes: ExifToolReader]
- **src\CamBridge.Infrastructure\Services\ProcessingQueue.cs** [Classes: ProcessingQueue, ProcessingStatistics, ProcessingItemStatus]
- **src\CamBridge.Infrastructure\Services\FileProcessor.cs** [MONSTER] [Classes: FileProcessor, FileProcessingEventArgs, FileProcessingErrorEventArgs, FileProcessingResult]
- **session109-sources.txt**
- **FILETREE_WISDOM.md**
- **src\CamBridge.Service\Program.cs** [Classes: Program]
- **docs\sources\SOURCES_INDEX.md**
- **docs\sources\SOURCES_SERVICE.txt**
- **docs\sources\SOURCES_INFRASTRUCTURE.txt**
- **docs\sources\SOURCES_CORE.txt**
- **docs\sources\SOURCES_CONFIG.txt**

### Yesterday
- src\CamBridge.Infrastructure\ServiceCollectionExtensions.cs
- src\CamBridge.Infrastructure\Services\PipelineManager.cs
- src\CamBridge.Core\Interfaces\IMappingConfiguration.cs
- src\CamBridge.Config\Services\IPipelineSettingsService.cs
- src\CamBridge.Config\Services\PipelineSettingsService.cs
- src\CamBridge.Service\appsettings.json
- src\CamBridge.Core\ConfigurationPaths.cs
- deadletter-references.txt
- pipesettings-source.txt
- src\CamBridge.Config\Views\PipelineConfigPage.xaml
- src\CamBridge.Config\Views\PipelineConfigPage.xaml.cs
- src\CamBridge.Core\ProcessingOptions.cs
- Version.props
- prio0-sources.txt
- CHANGELOG.md
- src\CamBridge.Config\Views\LogViewerPage.xaml
- src\CamBridge.Config\ViewModels\LogViewerViewModel.cs
- src\CamBridge.Config\Converters\ValueConverters.cs
- session106-sources.txt
- src\CamBridge.Core\Enums\ProcessingStage.cs
- worker-source.txt
- logviewer-old.xaml
- session105-sample-log.txt
- session105-sources.txt
- src\CamBridge.Config\Views\LogViewerPage.xaml.cs
- session104-logviewer-sources.txt
- src\CamBridge.Core\Logging\LogContext.cs
- src\CamBridge.Service\Worker.cs
- di-debug-sources.txt
- src\CamBridge.Service\Controllers\StatusController.cs

### This Week
- pipelinemanager-original.txt *(2 days ago)*
- session99-treeview-debug-sources.txt *(2 days ago)*
- session98-logviewer-sources.txt *(2 days ago)*
- src\CamBridge.Core\CamBridge.Core.csproj *(2 days ago)*
- src\CamBridge.Config\ViewModels\ServiceControlViewModel.cs *(2 days ago)*
- src\CamBridge.Core\CamBridgeSettingsV2.cs *(2 days ago)*
- session97-logging-sources.txt *(2 days ago)*
- session96-logviewer-source.txt *(2 days ago)*
- session96-logcontext-source.txt *(2 days ago)*
- session96-logcontext-sources.txt *(2 days ago)*
- session96-correlation-sources.txt *(2 days ago)*
- src\CamBridge.Config\Views\ServiceControlPage.xaml.cs *(2 days ago)*
- src\CamBridge.Core\Enums\LogVerbosity.cs *(2 days ago)*
- src\CamBridge.Config\Views\ServiceControlPage.xaml *(2 days ago)*
- service_control_sources.txt *(2 days ago)*
- session96-servicecontrol-sources.txt *(2 days ago)*
- session96-log-sources.txt *(2 days ago)*
- session95-colors-sources.txt *(2 days ago)*
- src\CamBridge.Config\Views\DashboardPage.xaml *(2 days ago)*
- src\CamBridge.Config\ViewModels\PacsConfigViewModel.cs *(2 days ago)*
- session95-ui-sources.txt *(2 days ago)*
- src\CamBridge.Config\App.xaml.cs *(2 days ago)*
- src\CamBridge.Config\ViewModels\PipelineConfigViewModel.cs *(2 days ago)*
- refactoring-sources-95.txt *(2 days ago)*
- CamBridge.sln *(2 days ago)*
- WISDOM_DEBT.md *(2 days ago)*

## MONSTER FILES (Need Refactoring!)

File | Lines | Classes | Status
-----|-------|---------|--------
src\CamBridge.Config\ViewModels\LogViewerViewModel.cs | **1567** | FilePositionInfo, PipelineSelection, LogViewerViewModel, LogEntry, CorrelationGroup, StageGroup | CRITICAL
src\CamBridge.Config\ViewModels\MappingEditorViewModel.cs | **1190** | MappingEditorViewModel, SourceFieldInfo, MappingRuleViewModel | CRITICAL
src\CamBridge.Infrastructure\Services\PipelineManager.cs | **870** | PipelineManager, PipelineInfo, PipelineStatusInfo | HIGH
src\CamBridge.Config\ViewModels\ServiceControlViewModel.cs | **777** | ServiceControlViewModel | HIGH
src\CamBridge.Infrastructure\Services\FileProcessor.cs | **718** | FileProcessor, FileProcessingEventArgs, FileProcessingErrorEventArgs, FileProcessingResult | HIGH
src\CamBridge.Config\Converters\ValueConverters.cs | **669** | IntToVisibilityConverter, BooleanToVisibilityConverter, InverseBooleanToVisibilityConverter, GreaterThanZeroConverter, NullToVisibilityConverter, ZeroToVisibilityConverter, ErrorCountToColorConverter, InverseBooleanConverter, EmptyStringToVisibilityConverter, ServiceStatusToColorConverter, SecondsToMillisecondsConverter, EnumToBooleanConverter, FileSelectConverter, MultiBooleanOrConverter, EnumToCollectionConverter, FileSizeConverter, TimeSpanToStringConverter, MultiBooleanAndConverter, NullBooleanAndConverter, BoolToColorConverter, TransformToSymbolConverter, TransformToDescriptionConverter, CombineStagesConverter, ColorToBrushConverter | MEDIUM
CamBridge.ParserDebug\Program.cs | **610** | - | MEDIUM
src\CamBridge.Config\ViewModels\PipelineConfigViewModel.cs | **579** | PipelineConfigViewModel | MEDIUM
src\CamBridge.Infrastructure\Services\ExifToolReader.cs | **563** | ExifToolReader | MEDIUM
src\CamBridge.Config\Dialogs\TransformEditorDialog.xaml.cs | **524** | TransformEditorDialog | MEDIUM

**Total Monster Lines**: 8067 lines to refactor!

## QUICK CLASS FINDER

<details>
<summary>Click to expand class/interface list</summary>

**ConnectionTest\ConnectionTest.cs**
  - Classes: ServiceStatusDto, ConfigInfo

**src\CamBridge.Config\App.xaml.cs**
  - Classes: App

**src\CamBridge.Config\Converters\ValueConverters.cs**
  - Classes: IntToVisibilityConverter, BooleanToVisibilityConverter, InverseBooleanToVisibilityConverter, GreaterThanZeroConverter, NullToVisibilityConverter, ZeroToVisibilityConverter, ErrorCountToColorConverter, InverseBooleanConverter, EmptyStringToVisibilityConverter, ServiceStatusToColorConverter, SecondsToMillisecondsConverter, EnumToBooleanConverter, FileSelectConverter, MultiBooleanOrConverter, EnumToCollectionConverter, FileSizeConverter, TimeSpanToStringConverter, MultiBooleanAndConverter, NullBooleanAndConverter, BoolToColorConverter, TransformToSymbolConverter, TransformToDescriptionConverter, CombineStagesConverter, ColorToBrushConverter

**src\CamBridge.Config\Dialogs\DicomTagBrowserDialog.xaml.cs**
  - Classes: DicomTagBrowserDialog, DicomTagInfo

**src\CamBridge.Config\Dialogs\TransformEditorDialog.xaml.cs**
  - Classes: TransformEditorDialog

**src\CamBridge.Config\MainWindow.xaml.cs**
  - Classes: MainWindow

**src\CamBridge.Config\Models\DeadLetterModels.cs**
  - Classes: DeadLetterItemModel, DetailedStatisticsModel, PipelineStatistics

**src\CamBridge.Config\Models\ServiceStatusModel.cs**
  - Classes: ServiceStatusModel, ServiceInfo, EnvironmentInfo, ServiceStatistics, PipelineStatusData, ServiceConfigurationInfo

**src\CamBridge.Config\Services\ConfigurationService.cs**
  - Classes: ConfigurationService

**src\CamBridge.Config\Services\HttpApiService.cs**
  - Classes: HttpApiService

**src\CamBridge.Config\Services\IApiService.cs**
  - Interfaces: IApiService

**src\CamBridge.Config\Services\IConfigurationService.cs**
  - Interfaces: IConfigurationService

**src\CamBridge.Config\Services\INavigationService.cs**
  - Interfaces: INavigationService

**src\CamBridge.Config\Services\IPipelineSettingsService.cs**
  - Classes: ValidationResult
  - Interfaces: IPipelineSettingsService

**src\CamBridge.Config\Services\IServiceManager.cs**
  - Interfaces: IServiceManager

**src\CamBridge.Config\Services\ISettingsService.cs**
  - Classes: SettingsValidationResult, SettingsMigrationResult, SettingsBackupResult, SettingsHealthCheckResult, FileHealthStatus, SettingsChangedEventArgs, PipelineChangedEventArgs
  - Interfaces: ISettingsService

**src\CamBridge.Config\Services\NavigationService.cs**
  - Classes: NavigationService

**src\CamBridge.Config\Services\PipelineSettingsService.cs**
  - Classes: PipelineSettingsService

**src\CamBridge.Config\Services\ServiceManager.cs**
  - Classes: ServiceManager

**src\CamBridge.Config\ViewModels\DashboardViewModel.cs**
  - Classes: DashboardViewModel

**src\CamBridge.Config\ViewModels\DeadLettersViewModel.cs**
  - Classes: DeadLettersViewModel

**src\CamBridge.Config\ViewModels\LogViewerViewModel.cs**
  - Classes: FilePositionInfo, PipelineSelection, LogViewerViewModel, LogEntry, CorrelationGroup, StageGroup

**src\CamBridge.Config\ViewModels\MainViewModel.cs**
  - Classes: MainViewModel

**src\CamBridge.Config\ViewModels\MappingEditorViewModel.cs**
  - Classes: MappingEditorViewModel, SourceFieldInfo, MappingRuleViewModel

**src\CamBridge.Config\ViewModels\PacsConfigViewModel.cs**
  - Classes: PacsConfigViewModel

**src\CamBridge.Config\ViewModels\PipelineConfigViewModel.cs**
  - Classes: PipelineConfigViewModel

**src\CamBridge.Config\ViewModels\PipelineStatusViewModel.cs**
  - Classes: PipelineStatusViewModel

**src\CamBridge.Config\ViewModels\RecentActivityViewModel.cs**
  - Classes: RecentActivityViewModel

**src\CamBridge.Config\ViewModels\ServiceControlViewModel.cs**
  - Classes: ServiceControlViewModel

**src\CamBridge.Config\ViewModels\ViewModelBase.cs**
  - Classes: ViewModelBase

**src\CamBridge.Config\Views\AboutPage.xaml.cs**
  - Classes: AboutPage

**src\CamBridge.Config\Views\DashboardPage.xaml.cs**
  - Classes: DashboardPage

**src\CamBridge.Config\Views\DeadLettersPage.xaml.cs**
  - Classes: DeadLettersPage

**src\CamBridge.Config\Views\LogViewerPage.xaml.cs**
  - Classes: LogViewerPage

**src\CamBridge.Config\Views\MappingEditorPage.xaml.cs**
  - Classes: MappingEditorPage

**src\CamBridge.Config\Views\PipelineConfigPage.xaml.cs**
  - Classes: PipelineConfigPage

**src\CamBridge.Config\Views\ServiceControlPage.xaml.cs**
  - Classes: ServiceControlPage

**src\CamBridge.Config\Views\VogonPoetryWindow.xaml.cs**
  - Classes: VogonPoetryWindow

**src\CamBridge.Core\CamBridgeSettingsV2.cs**
  - Classes: CamBridgeSettingsV2, DicomSettings, LoggingSettings, ServiceSettings, NotificationSettings, EmailSettings, EventLogSettings, WebhookSettings, NotificationRules, NotificationTriggers, DailySummarySettings, BatchNotificationSettings

**src\CamBridge.Core\CustomMappingConfiguration.cs**
  - Classes: CustomMappingConfiguration

**src\CamBridge.Core\DeadLetterStatistics.cs**
  - Classes: DeadLetterStatistics

**src\CamBridge.Core\Entities\ImageMetadata.cs**
  - Classes: ImageMetadata

**src\CamBridge.Core\Entities\ImageTechnicalData.cs**
  - Classes: ImageTechnicalData

**src\CamBridge.Core\Entities\PatientInfo.cs**
  - Classes: PatientInfo

**src\CamBridge.Core\Entities\ProcessingResult.cs**
  - Classes: ProcessingResult

**src\CamBridge.Core\Entities\StudyInfo.cs**
  - Classes: StudyInfo

**src\CamBridge.Core\Interfaces\IDicomTagMapper.cs**
  - Interfaces: IDicomTagMapper

**src\CamBridge.Core\Interfaces\IMappingConfiguration.cs**
  - Interfaces: IMappingConfiguration

**src\CamBridge.Core\Logging\LogContext.cs**
  - Classes: LogContext

**src\CamBridge.Core\MappingRule.cs**
  - Classes: MappingRule

**src\CamBridge.Core\PipelineConfiguration.cs**
  - Classes: PipelineConfiguration, PipelineWatchSettings, DicomOverrides, MappingSet, PacsConfiguration

**src\CamBridge.Core\ProcessingOptions.cs**
  - Classes: ProcessingOptions

**src\CamBridge.Core\ProcessingSummary.cs**
  - Classes: ProcessingSummary

**src\CamBridge.Core\SystemSettings.cs**
  - Classes: SystemSettings, CoreConfiguration, ServiceConfiguration, LoggingConfiguration, DicomDefaultSettings

**src\CamBridge.Core\UserPreferences.cs**
  - Classes: UserPreferences, WindowPosition

**src\CamBridge.Core\ValueObjects\PatientId.cs**
  - Classes: PatientId

**src\CamBridge.Infrastructure\Services\DicomConverter.cs**
  - Classes: DicomConverter, ConversionResult, ValidationResult

**src\CamBridge.Infrastructure\Services\DicomStoreService.cs**
  - Classes: StoreResult, DicomStoreService

**src\CamBridge.Infrastructure\Services\DicomTagMapper.cs**
  - Classes: DicomTagMapper

**src\CamBridge.Infrastructure\Services\ExifToolReader.cs**
  - Classes: ExifToolReader

**src\CamBridge.Infrastructure\Services\FileProcessor.cs**
  - Classes: FileProcessor, FileProcessingEventArgs, FileProcessingErrorEventArgs, FileProcessingResult

**src\CamBridge.Infrastructure\Services\MappingConfigurationLoader.cs**
  - Classes: MappingConfigurationLoader

**src\CamBridge.Infrastructure\Services\NotificationService.cs**
  - Classes: NotificationService

**src\CamBridge.Infrastructure\Services\PacsUploadQueue.cs**
  - Classes: PacsUploadQueue

**src\CamBridge.Infrastructure\Services\PipelineManager.cs**
  - Classes: PipelineManager, PipelineInfo, PipelineStatusInfo

**src\CamBridge.Infrastructure\Services\ProcessingQueue.cs**
  - Classes: ProcessingQueue, ProcessingStatistics, ProcessingItemStatus

**src\CamBridge.QRBridge\Forms\QRDisplayForm.cs**
  - Classes: QRDisplayForm

**src\CamBridge.QRBridge\Services\ArgumentParser.cs**
  - Classes: ArgumentParser

**src\CamBridge.QRBridge\Services\QRCodeService.cs**
  - Classes: QRCodeService
  - Interfaces: IQRCodeService

**src\CamBridge.Service\CamBridgeHealthCheck.cs**
  - Classes: CamBridgeHealthCheck

**src\CamBridge.Service\Program.cs**
  - Classes: Program

**src\CamBridge.Service\Worker.cs**
  - Classes: Worker

**tests\CamBridge.PipelineTest\Program.cs**
  - Classes: ConsoleLogger

**tests\CamBridge.TestConsole\Program.cs**
  - Classes: RicohTestRunner

</details>

## PROJECT STRUCTURE

### [CamBridge.Config]
*55 files, 9959 lines, 77 classes*
 **WARNING: 6 monster files**

- src\CamBridge.Config\App.xaml
- src\CamBridge.Config\App.xaml.cs [HAS-CLASSES]
- src\CamBridge.Config\CamBridge.Config.csproj
- src\CamBridge.Config\Converters\ValueConverters.cs [MONSTER] [HAS-CLASSES]
- src\CamBridge.Config\Dialogs\DicomTagBrowserDialog.xaml
- src\CamBridge.Config\Dialogs\DicomTagBrowserDialog.xaml.cs [HAS-CLASSES]
- src\CamBridge.Config\Dialogs\TransformEditorDialog.xaml
- src\CamBridge.Config\Dialogs\TransformEditorDialog.xaml.cs [MONSTER] [HAS-CLASSES]
- src\CamBridge.Config\Extensions\MappingConfigurationExtensions.cs
- src\CamBridge.Config\Extensions\MappingRuleExtensions.cs
- src\CamBridge.Config\Helpers\PasswordBoxHelper.cs
- src\CamBridge.Config\MainWindow.xaml
- src\CamBridge.Config\MainWindow.xaml.cs [HAS-CLASSES]
- src\CamBridge.Config\Models\DeadLetterModels.cs [HAS-CLASSES]
- src\CamBridge.Config\Models\ServiceStatusModel.cs [HAS-CLASSES]
- src\CamBridge.Config\Properties\launchSettings.json
- src\CamBridge.Config\Services\ConfigurationService.cs [HAS-CLASSES]
- src\CamBridge.Config\Services\HttpApiService.cs [HAS-CLASSES]
- src\CamBridge.Config\Services\IApiService.cs
- src\CamBridge.Config\Services\IConfigurationService.cs
- src\CamBridge.Config\Services\INavigationService.cs
- src\CamBridge.Config\Services\IPipelineSettingsService.cs [HAS-CLASSES]
- src\CamBridge.Config\Services\IServiceManager.cs
- src\CamBridge.Config\Services\ISettingsService.cs [HAS-CLASSES]
- src\CamBridge.Config\Services\NavigationService.cs [HAS-CLASSES]
- src\CamBridge.Config\Services\PipelineSettingsService.cs [HAS-CLASSES]
- src\CamBridge.Config\Services\ServiceManager.cs [HAS-CLASSES]
- src\CamBridge.Config\ViewModels\DashboardViewModel.cs [HAS-CLASSES]
- src\CamBridge.Config\ViewModels\DeadLettersViewModel.cs [HAS-CLASSES]
- src\CamBridge.Config\ViewModels\LogViewerViewModel.cs [MONSTER] [HAS-CLASSES]
- src\CamBridge.Config\ViewModels\MainViewModel.cs [HAS-CLASSES]
- src\CamBridge.Config\ViewModels\MappingEditorViewModel.cs [MONSTER] [HAS-CLASSES]
- src\CamBridge.Config\ViewModels\PacsConfigViewModel.cs [HAS-CLASSES]
- src\CamBridge.Config\ViewModels\PipelineConfigViewModel.cs [MONSTER] [HAS-CLASSES]
- src\CamBridge.Config\ViewModels\PipelineStatusViewModel.cs [HAS-CLASSES]
- src\CamBridge.Config\ViewModels\RecentActivityViewModel.cs [HAS-CLASSES]
- src\CamBridge.Config\ViewModels\ServiceControlViewModel.cs [MONSTER] [HAS-CLASSES]
- src\CamBridge.Config\ViewModels\ServiceControlViewModelExtension.cs
- src\CamBridge.Config\ViewModels\ViewModelBase.cs [HAS-CLASSES]
- src\CamBridge.Config\Views\AboutPage.xaml
- src\CamBridge.Config\Views\AboutPage.xaml.cs [HAS-CLASSES]
- src\CamBridge.Config\Views\DashboardPage.xaml
- src\CamBridge.Config\Views\DashboardPage.xaml.cs [HAS-CLASSES]
- src\CamBridge.Config\Views\DeadLettersPage.xaml
- src\CamBridge.Config\Views\DeadLettersPage.xaml.cs [HAS-CLASSES]
- src\CamBridge.Config\Views\LogViewerPage.xaml
- src\CamBridge.Config\Views\LogViewerPage.xaml.cs [HAS-CLASSES]
- src\CamBridge.Config\Views\MappingEditorPage.xaml
- src\CamBridge.Config\Views\MappingEditorPage.xaml.cs [HAS-CLASSES]
- src\CamBridge.Config\Views\PipelineConfigPage.xaml
- src\CamBridge.Config\Views\PipelineConfigPage.xaml.cs [HAS-CLASSES]
- src\CamBridge.Config\Views\ServiceControlPage.xaml
- src\CamBridge.Config\Views\ServiceControlPage.xaml.cs [HAS-CLASSES]
- src\CamBridge.Config\Views\VogonPoetryWindow.xaml
- src\CamBridge.Config\Views\VogonPoetryWindow.xaml.cs [HAS-CLASSES]

### [CamBridge.Core]
*27 files, 3247 lines, 36 classes*

- src\CamBridge.Core\CamBridge.Core.csproj
- src\CamBridge.Core\CamBridgeSettingsV2.cs [HAS-CLASSES]
- src\CamBridge.Core\ConfigurationPaths.cs
- src\CamBridge.Core\CustomMappingConfiguration.cs [HAS-CLASSES]
- src\CamBridge.Core\DeadLetterStatistics.cs [HAS-CLASSES]
- src\CamBridge.Core\Entities\ImageMetadata.cs [HAS-CLASSES]
- src\CamBridge.Core\Entities\ImageTechnicalData.cs [HAS-CLASSES]
- src\CamBridge.Core\Entities\PatientInfo.cs [HAS-CLASSES]
- src\CamBridge.Core\Entities\ProcessingResult.cs [HAS-CLASSES]
- src\CamBridge.Core\Entities\QRCodeRequest.cs
- src\CamBridge.Core\Entities\StudyInfo.cs [HAS-CLASSES]
- src\CamBridge.Core\Enums\LogVerbosity.cs
- src\CamBridge.Core\Enums\ProcessingStage.cs
- src\CamBridge.Core\Interfaces\IDicomTagMapper.cs
- src\CamBridge.Core\Interfaces\IMappingConfiguration.cs
- src\CamBridge.Core\Logging\LogContext.cs [HAS-CLASSES]
- src\CamBridge.Core\MappingRule.cs [HAS-CLASSES]
- src\CamBridge.Core\PipelineConfiguration.cs [HAS-CLASSES]
- src\CamBridge.Core\ProcessingOptions.cs [HAS-CLASSES]
- src\CamBridge.Core\ProcessingSummary.cs [HAS-CLASSES]
- src\CamBridge.Core\SystemSettings.cs [HAS-CLASSES]
- src\CamBridge.Core\UserPreferences.cs [HAS-CLASSES]
- src\CamBridge.Core\ValueObjects\DicomTag.cs
- src\CamBridge.Core\ValueObjects\ExifTag.cs
- src\CamBridge.Core\ValueObjects\PatientId.cs [HAS-CLASSES]
- src\CamBridge.Core\ValueObjects\StudyId.cs
- src\CamBridge.Core\ValueTransform.cs

### [CamBridge.Infrastructure]
*13 files, 4669 lines, 20 classes*
 **WARNING: 3 monster files**

- src\CamBridge.Infrastructure\CamBridge.Infrastructure.csproj
- src\CamBridge.Infrastructure\GlobalUsings.cs
- src\CamBridge.Infrastructure\ServiceCollectionExtensions.cs
- src\CamBridge.Infrastructure\Services\DicomConverter.cs [TODAY] [HAS-CLASSES]
- src\CamBridge.Infrastructure\Services\DicomStoreService.cs [TODAY] [HAS-CLASSES]
- src\CamBridge.Infrastructure\Services\DicomTagMapper.cs [TODAY] [HAS-CLASSES]
- src\CamBridge.Infrastructure\Services\ExifToolReader.cs [TODAY] [MONSTER] [HAS-CLASSES]
- src\CamBridge.Infrastructure\Services\FileProcessor.cs [TODAY] [MONSTER] [HAS-CLASSES]
- src\CamBridge.Infrastructure\Services\MappingConfigurationLoader.cs [TODAY] [HAS-CLASSES]
- src\CamBridge.Infrastructure\Services\NotificationService.cs [HAS-CLASSES]
- src\CamBridge.Infrastructure\Services\PacsUploadQueue.cs [TODAY] [HAS-CLASSES]
- src\CamBridge.Infrastructure\Services\PipelineManager.cs [MONSTER] [HAS-CLASSES]
- src\CamBridge.Infrastructure\Services\ProcessingQueue.cs [TODAY] [HAS-CLASSES]

### [CamBridge.PacsTest] [TEST]
*2 files, 202 lines, 0 classes*

- tests\CamBridge.PacsTest\CamBridge.PacsTest.csproj
- tests\CamBridge.PacsTest\Program.cs

### [CamBridge.PipelineTest] [TEST]
*2 files, 192 lines, 1 classes*

- tests\CamBridge.PipelineTest\CamBridge.PipelineTest.csproj
- tests\CamBridge.PipelineTest\Program.cs [HAS-CLASSES]

### [CamBridge.QRBridge]
*7 files, 743 lines, 3 classes*

- src\CamBridge.QRBridge\CamBridge.QRBridge.csproj
- src\CamBridge.QRBridge\Constants\QRBridgeConstants.cs
- src\CamBridge.QRBridge\Forms\QRDisplayForm.cs [HAS-CLASSES]
- src\CamBridge.QRBridge\GlobalUsings.cs
- src\CamBridge.QRBridge\Program.cs
- src\CamBridge.QRBridge\Services\ArgumentParser.cs [HAS-CLASSES]
- src\CamBridge.QRBridge\Services\QRCodeService.cs [HAS-CLASSES]

### [CamBridge.Service]
*12 files, 1392 lines, 3 classes*

- src\CamBridge.Service\appsettings.json
- src\CamBridge.Service\CamBridge.Service.csproj
- src\CamBridge.Service\CamBridgeHealthCheck.cs [HAS-CLASSES]
- src\CamBridge.Service\ConfigValidator.cs
- src\CamBridge.Service\Controllers\StatusController.cs
- src\CamBridge.Service\mappings.json
- src\CamBridge.Service\Program.cs [TODAY] [HAS-CLASSES]
- src\CamBridge.Service\Properties\launchSettings.json
- src\CamBridge.Service\ServiceInfo.cs
- src\CamBridge.Service\Tools\exiftool_files\readme_windows.txt
- src\CamBridge.Service\Tools\exiftool_files\windows_exiftool.txt
- src\CamBridge.Service\Worker.cs [HAS-CLASSES]

### [CamBridge.TestConsole] [TEST]
*3 files, 316 lines, 1 classes*

- tests\CamBridge.TestConsole\CamBridge.TestConsole.csproj
- tests\CamBridge.TestConsole\mappings.json
- tests\CamBridge.TestConsole\Program.cs [HAS-CLASSES]

### [ROOT]
*107 files, 1144 lines, 2 classes*

- _archive\old_collectors\collect-sources.bat
- _archive\old_collectors\collect-sources-balanced.bat
- _archive\old_collectors\collect-sources-gui-config.bat
- _archive\old_collectors\collect-sources-mapping-editor.bat
- _archive\old_collectors\collect-sources-test.bat
- _archive\old_docs\cambridge-entwicklungsplan-v2.md
- _archive\README_ARCHIVE.md
- 00-build-zip.ps1
- 0-build.ps1
- 0-build-no-qr.ps1
- 1-deploy-update.ps1
- 2-config.ps1
- 3-service.ps1
- 4-console.ps1
- 5-test-api.ps1
- 6-logs.ps1
- 7-clean.ps1
- 8-status.ps1
- 99-testit-full.ps1
- 9-testit.ps1
- Build-QRBridgeUltraSlim.ps1
- CamBridge.ParserDebug\CamBridge.ParserDebug.csproj
- CamBridge.ParserDebug\Program.cs [MONSTER]
- CamBridge.ParserDebug\Properties\launchSettings.json
- CamBridge.ServiceDebug.csproj
- CamBridge.ServiceDebug\CamBridge.ServiceDebug.csproj
- CamBridge.ServiceDebug\Program.cs
- CamBridge.sln
- CAMBRIDGE_OVERVIEW.md
- CHANGELOG.md
- Cleanup-DeadLetter.ps1
- COLLECTOR_README.md
- ConnectionTest\ConnectionTest.cs [HAS-CLASSES]
- ConnectionTest\ConnectionTest.csproj
- Create-DeploymentPackage - 250609.ps1
- Create-DeploymentPackage - 250610.ps1
- Create-DeploymentPackage - Kopie.ps1
- Create-DeploymentPackage.ps1
- Create-DeploymentPackage_250624.ps1
- Create-NumberedTools.ps1
- deadletter-references.txt
- di-debug-sources.txt
- Directory.Build.props
- docs\sources\SOURCES_CONFIG.txt [TODAY]
- docs\sources\SOURCES_CORE.txt [TODAY]
- docs\sources\SOURCES_INDEX.md [TODAY]
- docs\sources\SOURCES_INFRASTRUCTURE.txt [TODAY]
- docs\sources\SOURCES_QRBRIDGE.txt
- docs\sources\SOURCES_SERVICE.txt [TODAY]
- Emergency-Fix-CamBridge.ps1
- ExifToolQuickTest\exif_output.json
- ExifToolQuickTest\ExifToolQuickTest.csproj
- ExifToolQuickTest\Program.cs
- ExifToolTest\ExifToolTest.csproj
- ExifToolTest\Program.cs
- FILETREE_WISDOM.md [TODAY]
- Get-WisdomProjectTree - 250624.ps1
- Get-WisdomProjectTree.ps1
- Get-WisdomSources.ps1
- Get-WisdomSources_Dynamic.ps1
- h-help.ps1
- logviewer-old.xaml
- Migrate-CamBridgeConfig.ps1
- pipelinemanager-original.txt
- pipesettings-source.txt
- prio0-sources.txt
- Program.cs
- PROJECT_WISDOM.md
- qrbridge-constants.txt
- README-Deployment.md
- refactoring-sources-95.txt
- service_control_sources.txt
- service-manager.ps1
- session104-logviewer-sources.txt
- session105-sample-log.txt
- session105-sources.txt
- session106-sources.txt
- session109-sources.txt [TODAY]
- session110-correlation-sources.txt [TODAY]
- session95-colors-sources.txt
- session95-ui-sources.txt
- session96-correlation-sources.txt
- session96-logcontext-source.txt
- session96-logcontext-sources.txt
- session96-log-sources.txt
- session96-logviewer-source.txt
- session96-servicecontrol-sources.txt
- session97-logging-sources.txt
- session98-logviewer-sources.txt
- session99-treeview-debug-sources.txt
- ShowVersion.targets
- test-api.ps1
- Test-CompletePipeline.ps1
- Test-QRBridge.ps1
- Version.props
- WISDOM_ARCHITECTURE.md
- WISDOM_CLAUDE - 250614.md
- WISDOM_CLAUDE.md
- WISDOM_DEBT.md
- WISDOM_META.md
- WISDOM_PO.md
- WISDOM_TECHNICAL - 250614.md
- WISDOM_TECHNICAL.md
- WISDOM_TECHNICAL_APIS.md
- WISDOM_TECHNICAL_FIXES.md
- WISDOM_TECHNICAL_PATTERNS.txt
- worker-source.txt

### [TOOLS]
*2 files, 0 lines, 0 classes*

- Tools\exiftool_files\readme_windows.txt
- Tools\exiftool_files\windows_exiftool.txt

## SUMMARY STATISTICS

Metric | Value
-------|-------
Total Files | **230**
Total Lines of Code | **21864**
Files Modified Today | **17**
Files Modified This Week | **73**
Monster Files (>500 lines) | **10**
Test Files | **7**
Total Classes Found | **143**
Total Interfaces Found | **9**

## WISDOM TIPS

- [TODAY] = Modified today
- [MONSTER] = Monster file (>500 lines)
- [HAS-CLASSES] = Contains classes/interfaces
- [TEST] = Test project

**Remember**: Before creating ANY file, search this document first!

---
*Generated by Get-WisdomFileTree.ps1 - Your daily code dashboard*
