# WISDOM_META.md - CamBridge Master Reference
**Version**: 0.7.13  
**Purpose**: Consolidated technical & operational wisdom  
**Philosophy**: Sources First, KISS, Tab-Complete Everything

## 🚀 QUICK REFERENCE

### Critical Constants
```yaml
Port: 5111 (EVERYWHERE! Not 5050!)
Service Name: CamBridgeService (no space!)
Config Path: %ProgramData%\CamBridge\appsettings.json
Config Format: V2 with CamBridge wrapper
Output Org Values: [None, ByPatient, ByDate, ByPatientAndDate]
```

### Essential Commands
```powershell
# Tab-Complete Build System
0[TAB]   # Build without ZIP (20 sec)
1[TAB]   # Deploy & Start Service  
2[TAB]   # Open Config Tool
9[TAB]   # Quick Test (no build)

# Service Control
Get-Service CamBridgeService
Stop-Service CamBridgeService -Force
Start-Service CamBridgeService

# API Testing
Invoke-RestMethod "http://localhost:5111/api/status/version"
Invoke-RestMethod "http://localhost:5111/api/pipelines"

# Config Validation
.\Debug-CamBridgeJson.ps1
```

## 🗺️ COMPLETE CODE MAP v2.0

### System Overview
```
CamBridge Solution (14,350 LOC total)
├── CamBridge.Core (~3,200 LOC)
├── CamBridge.Infrastructure (~4,800 LOC)  
├── CamBridge.Service (~2,100 LOC)
└── CamBridge.Config (~3,900 LOC)
```

### 📁 CAMBRIDGE.CORE - Complete File List

#### Configuration & Settings
```yaml
ConfigurationPaths.cs ⭐ [CRITICAL - Single Source of Truth]
  - GetPrimaryConfigPath(): string
  - GetPipelineConfigDirectory(): string
  - GetMappingRulesDirectory(): string
  - GetErrorDirectory(): string  
  - InitializePrimaryConfig(): bool [v0.7.13 FIXED!]
  - EnsureDirectoriesExist(): void
  - PrimaryConfigExists(): bool

CamBridgeSettings.cs [V1 - Legacy]
  - DefaultDicomSettings: DicomSettings
  - DefaultOutputFolder: string
  - WatchFolders: List<FolderConfiguration>
  - ExifToolPath: string

CamBridgeSettingsV2.cs [V2 - Current]
  - Version: string = "2.0"
  - Service: ServiceSettings
  - Pipelines: List<PipelineConfiguration>
  - MappingSets: List<MappingSet>
  - DicomDefaults: DicomSettings
  - DefaultProcessingOptions: ProcessingOptions

SystemSettings.cs [3-Layer Architecture]
  - Service: ServiceConfiguration
  - Core: CoreConfiguration  
  - Logging: LoggingConfiguration
  - DicomDefaults: DicomDefaultSettings
  - Notifications: NotificationSettings

PipelineConfiguration.cs
  - Id: Guid
  - Name: string
  - Enabled: bool
  - WatchSettings: PipelineWatchSettings
  - ProcessingOptions: ProcessingOptions
  - DicomOverrides: DicomOverrides?
  - MappingSetId: Guid?

ProcessingOptions.cs
  - OutputOrganization: enum [None, ByPatient, ByDate, ByPatientAndDate]
  - SuccessAction: enum [Delete, Archive, Move]
  - FailureAction: enum [Leave, Move]
  - CreateBackup: bool
  - MaxConcurrentProcessing: int
```

#### Entities
```yaml
PatientInfo.cs
  - Id: PatientId
  - Name: PatientName
  - BirthDate: DateTime?
  - Gender: Gender?
  - AdditionalInfo: Dictionary<string,string>

StudyInfo.cs  
  - Id: StudyId
  - PatientId: PatientId
  - AccessionNumber: string?
  - StudyDate: DateTime?
  - StudyDescription: string?
  - ReferringPhysician: string?

ProcessedImage.cs
  - Id: Guid
  - OriginalPath: string
  - DicomPath: string
  - ProcessedAt: DateTime
  - Success: bool
  - ErrorMessage: string?
```

#### Value Objects
```yaml
PatientId.cs: string wrapper
StudyId.cs: string wrapper  
DicomUid.cs: string wrapper with validation
PatientName.cs: FirstName, LastName, MiddleName
Gender.cs: enum [Male, Female, Other, Unknown]
ImageDimensions.cs: Width, Height, BitsPerPixel
```

#### Interfaces (8 remaining, target: 4)
```yaml
IExifReader.cs [TARGET: Remove]
IDicomConverter.cs [TARGET: Remove]
IFileProcessor.cs [REMOVED in v0.7.8]
IFolderWatcher.cs [TARGET: Remove]
IMappingConfiguration.cs [KEEP - for now]
INotificationService.cs [TARGET: Remove]
IProcessingQueue.cs [REMOVED in v0.7.8]
IQueueProcessor.cs [TARGET: Remove]
```

### 📁 CAMBRIDGE.INFRASTRUCTURE - Complete File List

#### Core Services
```yaml
ExifToolReader.cs [NO INTERFACE]
  - ExtractDataAsync(imagePath): Task<Dictionary<string,string>>
  - ParseBarcodeData(exifData): (PatientInfo?, StudyInfo?)
  - GetExifValue(dict, keys): string?
  - ParseQRBridgeData(barcodeData): Dictionary<string,string>
  - FindExifTool(): string?

DicomConverter.cs [NO INTERFACE]
  - ConvertJpegToDicomAsync(jpeg, patient, study, output): Task<ProcessedImage>
  - CreateDicomDataset(jpeg, patient, study, exif): DicomDataset
  - AddPatientModule(dataset, patient): void
  - AddStudyModule(dataset, study): void
  - AddImageModule(dataset, dimensions, exif): void

FileProcessor.cs [NO INTERFACE]
  - ProcessFileAsync(inputPath, outputPath): Task<ProcessedImage>
  - ValidateInputFile(path): void
  - DetermineOutputPath(input, output, patient, study): string
  - HandleProcessingSuccess(input, output, options): Task
  - HandleProcessingFailure(input, error, options): Task
  - Events: ProcessingStarted, ProcessingCompleted, ProcessingFailed

PipelineManager.cs ⭐ [ORCHESTRATOR]
  - StartAsync(settings): Task
  - StopAsync(): Task
  - EnablePipelineAsync(pipelineId): Task
  - DisablePipelineAsync(pipelineId): Task
  - UpdatePipelinesAsync(configs): Task
  - GetPipelineStatuses(): Dictionary<string, PipelineStatus>
  - GetPipelineDetails(id): PipelineDetailedStatus?
  Private:
  - CreatePipelineContext(config): PipelineContext
  - StartPipelineAsync(context): Task
  - StopPipelineAsync(context): Task
```

#### Supporting Services
```yaml
MappingConfigurationLoader.cs
  - GetMappingRules(): IReadOnlyList<MappingRule>
  - LoadConfigurationAsync(filePath?): Task<bool>
  - SaveConfigurationAsync(rules, filePath?): Task<bool>
  - LoadDefaultMappings(): void

DicomTagMapper.cs
  - MapExifToDicom(exifData, mappingRules): Dictionary<DicomTag, object>
  - ApplyMapping(rule, value): object?
  - GetDicomTag(tagString): DicomTag?

ProcessingQueue.cs
  - TryEnqueue(filePath): bool
  - QueueLength: int
  - ActiveProcessing: int
  - GetStatistics(): QueueStatistics
  - GetActiveItems(): List<ProcessingItemStatus>
  Private:
  - ProcessQueueAsync(): Task
  - ProcessItemAsync(item): Task

ServiceCollectionExtensions.cs
  - AddInfrastructure(services, config): IServiceCollection
  - AddInfrastructureForConfig(services): IServiceCollection
  - ValidateInfrastructure(provider): IServiceProvider
```

### 📁 CAMBRIDGE.SERVICE - Complete File List

```yaml
Program.cs [Entry Point]
  - Main(): creates WebApplication
  - ConfigureServices(): DI setup
  - ConfigureEndpoints(): Minimal API
  - Port: 5111 (hardcoded)
  API Endpoints:
    GET /api/status/version
    GET /api/status/health
    GET /api/pipelines
    GET /api/pipelines/{id}
    PUT /api/pipelines/{id}
    GET /api/statistics

Worker.cs [Background Service]
  - ExecuteAsync(stoppingToken): Task
  - StartAsync(cancellationToken): Task  
  - StopAsync(cancellationToken): Task
  Dependencies:
    - PipelineManager
    - IConfiguration

appsettings.json [Local only]
  - Serilog configuration
  - Not used for actual config!
```

### 📁 CAMBRIDGE.CONFIG - Complete File List

#### Core Application Files
```yaml
App.xaml.cs ⭐ [v0.7.13 FIX]
  - Host: IHost property [CRITICAL!]
  - OnStartup(): Configure DI
  - ConfigureServices(): Register all services
  - OnExit(): Cleanup

MainWindow.xaml.cs
  - Navigation frame
  - Window chrome
  - Event handlers

AssemblyInfo.cs
  - Version attributes
  - Company info
```

#### ViewModels
```yaml
ViewModelBase.cs
  - PropertyChanged implementation
  - SetProperty<T> helper

MainViewModel.cs
  - Navigation commands
  - Window title
  - Current page tracking

DashboardViewModel.cs
  - ServiceStatus: ServiceStatusModel?
  - PipelineStatuses: ObservableCollection<PipelineStatus>
  - RefreshCommand: ICommand
  - StartServiceCommand, StopServiceCommand
  - IsServiceRunning: bool
  - UpdateInterval: DispatcherTimer (5 sec)

PipelineViewModel.cs  
  - Pipelines: ObservableCollection<PipelineConfiguration>
  - SelectedPipeline: PipelineConfiguration?
  - AddCommand, EditCommand, DeleteCommand
  - SaveCommand, CancelCommand
  - LoadPipelinesAsync(): Task

MappingViewModel.cs
  - MappingSets: ObservableCollection<MappingSet>
  - SelectedMappingSet: MappingSet?
  - MappingRules: ObservableCollection<MappingRule>
  - AddRuleCommand, DeleteRuleCommand
  - ImportCommand, ExportCommand
  - DicomTags: List<DicomTagInfo> [Static list]
```

#### Services
```yaml
ConfigurationService.cs [IConfigurationService]
  - LoadConfigurationAsync<T>(): Task<T?>
  - SaveConfigurationAsync<T>(config): Task
  - Uses: ConfigurationPaths.GetPrimaryConfigPath()
  - Handles: V2 format with CamBridge wrapper

ApiService.cs [IApiService]
  - GetStatusAsync(): Task<ServiceStatusModel?>
  - GetStatisticsAsync(): Task<DetailedStatisticsModel?>
  - IsServiceAvailableAsync(): Task<bool>
  - HttpClient to localhost:5111

ServiceManager.cs [IServiceManager]
  - IsServiceInstalledAsync(): Task<bool>
  - GetServiceStatusAsync(): Task<ServiceStatus>
  - StartServiceAsync(): Task<bool>
  - StopServiceAsync(): Task<bool>
  - RestartServiceAsync(): Task<bool>
  - Uses: ServiceController

NavigationService.cs [INavigationService]
  - NavigateTo(pageKey): void
  - GoBack(): void
  - CanGoBack: bool
  - Page registration system
```

#### Views (Pages)
```yaml
DashboardPage.xaml
  - Service status card
  - Pipeline status list
  - Quick actions

PipelinePage.xaml
  - Pipeline list/grid
  - Pipeline editor
  - Enable/disable toggles

MappingRulePage.xaml
  - Mapping set selector
  - Rule editor grid
  - Import/Export buttons

ServiceSettingsPage.xaml [REMOVED in v0.7.x]
LogViewerPage.xaml [Future]
```

### 🚧 DEAD CODE / ZOMBIES

```yaml
Still Present but Unused:
- Some interface definitions in Core
- Old V1 config migration code  
- DeadLetterQueue references
- NotificationService (partial)
- Several [Obsolete] methods

Removed Successfully:
- IDeadLetterService & implementation
- DeadLetterQueue & processor
- Complex retry mechanisms
- Settings page from Config Tool
```

### 🔒 PROTECTED FEATURES [KEEP!]

```yaml
Protected for Future Sprints:
- FTP Server (Sprint 8)
- C-STORE SCP (Sprint 9)  
- Modality Worklist (Sprint 10)
- C-FIND SCP (Sprint 11)
- HL7 Integration (Sprint 12)

DO NOT IMPLEMENT YET!
```

## ⚠️ CRITICAL GOTCHAS

### The Config Format Trap
```json
// WRONG - Will fail silently!
{
  "Pipelines": [...]
}

// CORRECT - Must have wrapper!
{
  "CamBridge": {
    "Version": "2.0",
    "Pipelines": [...]
  }
}
```

### The OutputOrganization Enum
```csharp
// VALID VALUES ONLY!
"OutputOrganization": "None"              // ✅
"OutputOrganization": "ByPatient"         // ✅  
"OutputOrganization": "ByDate"            // ✅
"OutputOrganization": "ByPatientAndDate"  // ✅
"OutputOrganization": "PatientName"       // ❌ CRASH!
```

### The Port Mismatch
```yaml
Service expects: 5111
Config had: 5050
Result: Dashboard shows nothing
Fix: Replace ALL occurrences!
```

### The Missing Property
```csharp
// In App.xaml.cs - ONE LINE fixes 144 errors!
public IHost Host => _host;
```

## 🛠️ DEVELOPMENT WORKFLOW

### Standard Build & Test Cycle
```powershell
# 1. Build without ZIP (fast)
0[TAB]

# 2. Deploy and start service
1[TAB]

# 3. Open config tool
2[TAB]

# 4. Test functionality
9[TAB]
```

### Debugging Workflow
```powershell
# 1. Check service status
Get-Service CamBridgeService

# 2. Check event log
Get-EventLog -LogName Application -Source CamBridge* -Newest 20

# 3. Test API
Invoke-RestMethod "http://localhost:5111/api/status/version"

# 4. Validate config
.\Debug-CamBridgeJson.ps1
```

### Config Reset Procedure
```powershell
Stop-Service CamBridgeService -Force
Remove-Item "$env:ProgramData\CamBridge\appsettings.json"
Start-Service CamBridgeService
# Service creates fresh config automatically
```

## 🏗️ ARCHITECTURE DECISIONS

### Why No Interfaces? (KISS)
```csharp
// Before (overengineered)
public interface IExifReader { }
public class ExifToolReader : IExifReader { }
services.AddSingleton<IExifReader, ExifToolReader>();

// After (simple)
public class ExifToolReader { }
services.AddSingleton<ExifToolReader>();
```

### Why V2 Config Format?
- V1: Flat structure, limited
- V2: Supports multiple pipelines
- Wrapper: Allows future versions
- Migration: Handled automatically

### Why Port 5111?
- Original: Random (5050)
- Problem: Inconsistent usage
- Solution: Global constant
- Learning: Details matter!

### Why Remove Dead Letter?
- Original: 650 LOC complex queue
- Reality: Never properly used
- Solution: Simple error folder
- Saving: Huge complexity reduction

## 📚 KEY LEARNINGS (Distilled)

### Technical Wisdoms
1. **KISS beats SOLID** - Every time
2. **Delete > Refactor** - Less code = less bugs
3. **Constants everywhere** - One source of truth
4. **Tab-complete everything** - Efficiency matters
5. **Working > Perfect** - Ship it!

### Process Wisdoms  
1. **Sources First** - Check what exists before coding
2. **User knows best** - Oliver's ideas consistently better
3. **Small fixes matter** - One line can fix everything
4. **Document decisively** - Future you will thank you
5. **Test immediately** - Build success ≠ feature works

### Debugging Wisdoms
1. **Check the obvious** - Port numbers, service names
2. **Read the error** - It usually tells you exactly
3. **Validate assumptions** - Is config loaded? Is service running?
4. **Use the tools** - Event Log, API endpoints
5. **When stuck, restart** - Clean slate often helps

## 🎯 CURRENT PRIORITIES

### Immediate (v0.7.14)
1. Fix OutputOrganization enum usage
2. Complete InitializePrimaryConfig()
3. Test full pipeline flow

### Short Term (v0.8.0)
1. Reduce interfaces (8 → 4)
2. Consolidate duplicate code
3. Improve error messages

### Medium Term (v0.9.0)
1. Config redesign (maybe v3?)
2. Performance optimization
3. Better logging

### Long Term (v1.0.0)
1. Medical features (protected)
2. Auto-discovery
3. Zero-config setup

## 🔧 MAINTENANCE NOTES

### Known Issues
- Add Mapping Rule button (UI)
- 144 build warnings
- Config complexity
- Some zombie code remains

### Quick Fixes
```csharp
// If dashboard empty
Check port 5111 everywhere

// If config won't load
Check for CamBridge wrapper

// If service won't start
Check OutputOrganization values

// If build fails with 144 errors
Add Host property to App.xaml.cs
```

### Performance Tips
- Build without ZIP saves 3 minutes
- Use tab-complete for everything
- Restart service for config changes
- Check logs before debugging

## 🚨 EMERGENCY PROCEDURES

### Service Won't Start
```powershell
# 1. Check event log
Get-EventLog -LogName Application -Source CamBridge* -Newest 10

# 2. Reset config
Remove-Item "$env:ProgramData\CamBridge\appsettings.json" -Force

# 3. Reinstall service
sc.exe delete CamBridgeService
sc.exe create CamBridgeService binPath="C:\CamBridge\Service\CamBridge.Service.exe"
```

### Config Corrupted
```powershell
# Run the JSON fixer
.\VOGON-EXIT-Fix-CamBridge-JSON.ps1

# Or manually fix wrapper
$json = Get-Content "$env:ProgramData\CamBridge\appsettings.json" | ConvertFrom-Json
$wrapped = @{ CamBridge = $json }
$wrapped | ConvertTo-Json -Depth 10 | Set-Content "$env:ProgramData\CamBridge\appsettings.json"
```

---

*"Making the improbable reliably simple - one tab-complete at a time!"*
