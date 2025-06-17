# WISDOM_META.md - CamBridge Master Reference
**Version**: 0.7.21  
**Last Update**: 2025-06-17 01:15  
**Purpose**: Consolidated technical & operational wisdom with complete code map  
**Philosophy**: Sources First, KISS, Tab-Complete Everything, Direct Dependencies, Pipeline Isolation, Minimal When Needed

## 🚀 QUICK REFERENCE

### Critical Constants (MEMORIZE!)
```yaml
Port: 5111 (EVERYWHERE! Not 5050!)
Service Name: CamBridgeService (no space!)
Config Path: %ProgramData%\CamBridge\appsettings.json
Config Format: V2 with CamBridge wrapper (MANDATORY!)
Output Org Values: [None, ByPatient, ByDate, ByPatientAndDate]
Version: 0.7.21 (dynamically read from assembly!)
Interfaces: Only 2 remain! (was 8+)
FileProcessor: Per-pipeline instance (NOT singleton!)
```

### Essential Commands
```powershell
# Tab-Complete Build System (YOUR BEST FRIENDS)
0[TAB]   # Build without ZIP (20 sec)
1[TAB]   # Deploy & Start Service  
2[TAB]   # Open Config Tool
9[TAB]   # Quick Test (no build)
4[TAB]   # Console Mode (debugging)

# Service Control
Get-Service CamBridgeService
Stop-Service CamBridgeService -Force
Start-Service CamBridgeService

# API Testing (v0.7.21)
Invoke-RestMethod "http://localhost:5111/api/status"
Invoke-RestMethod "http://localhost:5111/api/pipelines"
Invoke-RestMethod "http://localhost:5111/api/status/version"
Invoke-RestMethod "http://localhost:5111/api/status/health"

# Config Validation
.\Debug-CamBridgeJson.ps1

# Version Check (should show 0.7.21)
(Invoke-RestMethod "http://localhost:5111/api/status").version
```

## 🗺️ COMPLETE CODE MAP v2.5

### System Overview
```
CamBridge Solution (14,350+ LOC total)
├── CamBridge.Core (~3,200 LOC)
├── CamBridge.Infrastructure (~4,900 LOC)  
├── CamBridge.Service (~2,100 LOC)
├── CamBridge.Config (~3,900 LOC)
└── CamBridge.QRBridge (~350 LOC)
```

### 📁 CAMBRIDGE.CORE - Complete File List

#### Configuration & Settings
```yaml
ConfigurationPaths.cs ⭐ [CRITICAL - Single Source of Truth]
  - GetPrimaryConfigPath(): string
  - GetPipelineConfigDirectory(): string
  - GetMappingRulesDirectory(): string
  - GetErrorDirectory(): string  
  - InitializePrimaryConfig(): bool [v0.7.17 - COMPLETE & WORKING!]
  - EnsureDirectoriesExist(): void
  - PrimaryConfigExists(): bool

CamBridgeSettings.cs [REMOVED in v0.7.20!] ❌

CamBridgeSettingsV2.cs ⭐ [V2 - Current Format - CLEAN!]
  - Version: string = "2.0"
  - Service: ServiceSettings
  - Pipelines: List<PipelineConfiguration>
  - MappingSets: List<MappingSet>
  - GlobalDicomSettings: DicomSettings [NOT DicomDefaults]
  - DefaultProcessingOptions: ProcessingOptions
  - NO MORE WatchFolders property!
  - NO MORE DefaultOutputFolder property!
  - NO MORE FolderConfiguration class!

SystemSettings.cs [3-Layer Architecture - Consider merging]
  - Service: ServiceConfiguration
  - Core: CoreConfiguration  
  - Logging: LoggingConfiguration
  - DicomDefaults: DicomDefaultSettings
  - Notifications: NotificationSettings

PipelineConfiguration.cs ⭐ [Core Pipeline Model]
  - Id: Guid
  - Name: string
  - Enabled: bool
  - WatchSettings: PipelineWatchSettings
  - ProcessingOptions: ProcessingOptions
  - DicomOverrides: DicomOverrides? [NOT DicomSettings!]
  - MappingSetId: Guid?

ProcessingOptions.cs [v0.7.17 - With enum validation!]
  - OutputOrganization: enum [None, ByPatient, ByDate, ByPatientAndDate]
  - SuccessAction: enum [Delete, Archive, Move]
  - FailureAction: enum [Leave, Move]
  - CreateBackup: bool
  - MaxConcurrentProcessing: int

DicomOverrides.cs [Pipeline-specific overrides]
  - InstitutionName: string?
  - InstitutionDepartment: string?
  - StationName: string?
```

#### Entities
```yaml
PatientInfo.cs
  - Id: PatientId
  - Name: string
  - BirthDate: DateTime?
  - Gender: Gender (enum: Male, Female, Other)
  - FromExifData(): Static factory method

StudyInfo.cs  
  - StudyId: StudyId
  - ExamId: string?
  - Description: string? [NOT StudyDescription!]
  - StudyDate: DateTime
  - AccessionNumber: string?
  - ReferringPhysician: string?

ImageMetadata.cs
  - SourceFilePath: string
  - CaptureDateTime: DateTime
  - Patient: PatientInfo
  - Study: StudyInfo
  - TechnicalData: ImageTechnicalData
  - ExifData: Dictionary<string,string>
  - InstanceNumber: int
  [NO CameraInfo or OriginalFilename properties!]

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
PatientId.cs: string wrapper with validation
StudyId.cs: string wrapper with validation
DicomUid.cs: string wrapper with UID validation
PatientName.cs: FirstName, LastName, MiddleName
Gender.cs: DOES NOT EXIST - Gender is enum in PatientInfo!
ImageDimensions.cs: Width, Height, BitsPerPixel
```

#### Interfaces (Only 2 remaining! 🎉)
```yaml
IMappingConfiguration.cs [KEEP - for now]
  - GetMappingRules(): IReadOnlyList<MappingRule> [NOT async!]
  - LoadConfigurationAsync(string?): Task<bool>
  - SaveConfigurationAsync(rules, path): Task<bool>

IDicomTagMapper.cs [KEEP - for now]
  - ApplyTransform(value, transform): string?
  - MapToDataset(dataset, sourceData, rules): void [NOT MapMetadataToDicom!]

REMOVED in v0.7.18:
IDicomConverter.cs [REMOVED - direct DicomConverter] ✅
INotificationService.cs [REMOVED in v0.7.18] ✅

REMOVED in earlier versions:
IExifReader.cs [REMOVED - direct ExifToolReader] ✅
IFolderWatcher.cs [REMOVED - direct FolderWatcherService] ✅
IFileProcessor.cs [REMOVED in v0.7.8] ✅
IProcessingQueue.cs [REMOVED in v0.7.8] ✅
IQueueProcessor.cs [REMOVED] ✅
```

### 📁 CAMBRIDGE.INFRASTRUCTURE - Complete File List

#### Core Services (The Workers)
```yaml
ExifToolReader.cs ⭐ [NO INTERFACE - KISS!]
  - ExtractDataAsync(imagePath): Task<Dictionary<string,string>>
  - ParseBarcodeData(exifData): (PatientInfo?, StudyInfo?)
  - GetExifValue(dict, keys): string?
  - ParseQRBridgeData(barcodeData): Dictionary<string,string>
  - FindExifTool(): string?

DicomConverter.cs ⭐ [NO INTERFACE - Direct use - v0.7.18!]
  - ConvertToDicomAsync(jpeg, dicom, metadata): Task<ConversionResult>
  - ValidateDicomFileAsync(path): Task<ValidationResult>
  - GetPhotographicSopClassUid(): string
  - ConversionResult class (moved here in v0.7.18)
  - ValidationResult class (moved here in v0.7.18)

FileProcessor.cs ⭐⭐ [NO INTERFACE - Pipeline-specific! v0.7.20]
  - CREATED PER PIPELINE - NOT SINGLETON!
  - Constructor(logger, exifReader, dicomConverter, pipelineConfig, globalDicomSettings)
  - ProcessFileAsync(inputPath): Task<FileProcessingResult>
  - ShouldProcessFile(path): bool
  - ApplyDicomOverrides(global, overrides): DicomSettings [NEW!]
  - DetermineOutputPath uses pipeline-specific settings
  - Events: ProcessingStarted, ProcessingCompleted, ProcessingFailed

PipelineManager.cs ⭐⭐⭐ [ORCHESTRATOR - Heart of the system]
  - StartAsync(settings): Task
  - StopAsync(): Task
  - EnablePipelineAsync(pipelineId): Task
  - DisablePipelineAsync(pipelineId): Task
  - UpdatePipelinesAsync(configs): Task
  - GetPipelineStatuses(): Dictionary<string, PipelineStatus>
  - GetPipelineDetails(id): PipelineDetailedStatus?
  - CreatePipelineContext(): Creates FileProcessor per pipeline! [v0.7.20]
  - PipelineContext class includes FileProcessor property
```

#### Supporting Services
```yaml
MappingConfigurationLoader.cs [Implements IMappingConfiguration]
  - GetMappingRules(): IReadOnlyList<MappingRule>
  - LoadConfigurationAsync(filePath?): Task<bool>
  - SaveConfigurationAsync(rules, filePath?): Task<bool>
  - LoadDefaultMappings(): void

DicomTagMapper.cs [Implements IDicomTagMapper]
  - ApplyTransform(value, transform): string?
  - MapToDataset(dataset, sourceData, rules): void
  - GetDicomTag(tagString): DicomTag?

ProcessingQueue.cs [Channel-based processing - v0.7.20]
  - Constructor(logger, fileProcessor, options) - Direct FileProcessor!
  - NO IServiceScopeFactory - uses injected FileProcessor
  - TryEnqueue(filePath): bool
  - ProcessQueueAsync(token): uses _fileProcessor directly
  - QueueLength: int
  - ActiveProcessing: int
  - GetStatistics(): QueueStatistics
  - GetActiveItems(): List<ProcessingItemStatus>

FolderWatcherService.cs [NO INTERFACE!]
  - WatchFolder: string
  - FileDetected: event EventHandler<string>
  - Start(): void
  - Stop(): void

NotificationService.cs [NO INTERFACE - v0.7.18!]
  - SendDailySummaryAsync(summary): Task
  - NotifyErrorAsync(message, exception?): Task
  [Just logs - no email implementation!]

ServiceCollectionExtensions.cs [Updated v0.7.20]
  - AddInfrastructure(services, config): IServiceCollection
  - NO FileProcessor registration - created per pipeline!
  - NO CamBridgeSettings V1 registration!
  - NO Health Checks (belong in Service project)
  - Direct registrations: ExifToolReader, DicomConverter, NotificationService
  - Interface registrations: Only IMappingConfiguration, IDicomTagMapper
```

### 📁 CAMBRIDGE.SERVICE - Complete File List

```yaml
Program.cs ⭐ [Entry Point - Uses dynamic version]
  - Main(): creates WebApplication
  - ConfigureServices(): DI setup (direct dependencies!)
  - ConfigureEndpoints(): Minimal API
  - Port: 5111 (hardcoded - OK!)
  - ValidateInfrastructure removed (v0.7.20)
  - Now just resolves critical services for validation
  API Endpoints:
    GET /api/status ✅
    GET /api/pipelines ✅
    GET /api/status/version ✅
    GET /api/status/health ✅
    GET /api/statistics ❌ (404)

ServiceInfo.cs ⭐ [FIXED in v0.7.16!]
  - Version: string (dynamic from assembly!)
  - ServiceName: "CamBridgeService"
  - DisplayName: "CamBridge Medical Image Converter"
  - ApiPort: 5111
  - Company: string (also dynamic)

Worker.cs [Background Service - v0.7.20]
  - ExecuteAsync(stoppingToken): Task
  - StartAsync(cancellationToken): Task  
  - StopAsync(cancellationToken): Task
  Dependencies:
    - PipelineManager (direct, no interface)
    - IConfiguration
  - Uses PipelineManager correctly!

CamBridgeHealthCheck.cs [v0.7.18 - Direct NotificationService!]
  - CheckHealthAsync(): Task<HealthCheckResult>
  - Uses PipelineManager for status
  - Uses NotificationService directly (no interface)

DailySummaryService.cs [v0.7.18 - Direct NotificationService!]
  - Background service for daily summaries
  - Uses NotificationService directly (no interface)
  - Configurable schedule

appsettings.json [Local config - not primary!]
  - Serilog configuration only
  - Primary config: %ProgramData%\CamBridge\
```

### 📁 CAMBRIDGE.CONFIG - Complete File List

#### Core Application Files
```yaml
App.xaml.cs ⭐ [CRITICAL v0.7.13 FIX]
  - Host: IHost property [THIS LINE FIXES 144 ERRORS!]
  - OnStartup(): Configure DI
  - ConfigureServices(): Register all services
  - OnExit(): Cleanup

MainWindow.xaml.cs
  - Navigation frame hosting
  - Window chrome setup
  - Menu handling (if any)

AssemblyInfo.cs
  - Version attributes (uses Version.props)
  - Company info
  - Copyright
```

#### ViewModels (MVVM Pattern) - Updated v0.7.21
```yaml
ViewModelBase.cs
  - PropertyChanged implementation
  - SetProperty<T> helper
  - Common VM functionality
  - IsLoading property only! (NO IsError!)

MainViewModel.cs
  - Navigation commands
  - Window title
  - Current page tracking
  - Menu items (if any)

DashboardViewModel.cs ⭐⭐ [MINIMAL REWRITE v0.7.21!]
  - NO IApiService dependency!
  - Direct HttpClient usage
  - Simple timer in constructor (no InitializeAsync!)
  - ServiceStatus: string
  - IsServiceRunning: bool
  - PipelineStatuses: ObservableCollection<PipelineStatusViewModel>
  - RefreshAsync(): Direct HTTP calls
  - Port: 5111 (must match service!)

PipelineViewModel.cs [Pipeline Management]
  - Pipelines: ObservableCollection<PipelineConfiguration>
  - SelectedPipeline: PipelineConfiguration?
  - AddCommand, EditCommand, DeleteCommand
  - SaveCommand, CancelCommand
  - LoadPipelinesAsync(): Task
  - EnablePipelineCommand, DisablePipelineCommand
  [NOTE: Currently empty in UI - needs fix in Sprint 16]

MappingEditorViewModel.cs [DICOM Tag Mapping - v0.7.20]
  - MappingSets: ObservableCollection<MappingSet>
  - SelectedMappingSet: MappingSet?
  - MappingRules: ObservableCollection<MappingRule>
  - AddRuleCommand, DeleteRuleCommand
  - ImportCommand, ExportCommand
  - DicomTags: List<DicomTagInfo> [Static list]
  - IsError property added! [v0.7.20]
  - IsLoading, StatusMessage properties

SettingsViewModel.cs [REMOVED in v0.7.20!] ❌
SettingsPage.xaml [REMOVED in v0.7.20!] ❌
SettingsPage.xaml.cs [REMOVED in v0.7.20!] ❌
```

#### Services (Supporting Config Tool) - Updated v0.7.21
```yaml
ConfigurationService.cs [IConfigurationService] ⭐ v0.7.17 UPDATE
  - LoadConfigurationAsync<T>(): Task<T?>
  - SaveConfigurationAsync<T>(config): Task
  - Uses: ConfigurationPaths.GetPrimaryConfigPath()
  - Handles: V2 format with CamBridge wrapper
  - NEW: Enum validation for OutputOrganization
  - NEW: Clear error messages for invalid configs

ApiService.cs [IApiService] 
  - GetStatusAsync(): Task<ServiceStatusModel?>
  - GetStatisticsAsync(): Task<DetailedStatisticsModel?>
  - IsServiceAvailableAsync(): Task<bool>
  - HttpClient to localhost:5111
  - [Missing endpoints return 404]

ServiceManager.cs [IServiceManager]
  - IsServiceInstalledAsync(): Task<bool>
  - GetServiceStatusAsync(): Task<ServiceStatus>
  - StartServiceAsync(): Task<bool>
  - StopServiceAsync(): Task<bool>
  - RestartServiceAsync(): Task<bool>
  - Uses: ServiceController class

NavigationService.cs ⭐ [CRITICAL UPDATE v0.7.21!]
  - NavigateTo(pageKey): void
  - GoBack(): void
  - CanGoBack: bool
  - Page registration system
  - NO SettingsPage anymore!
  - NEW: ViewModel injection for each page!
  - Pattern: pageKey switch expression
```

#### Views (WPF Pages) - Updated v0.7.21
```yaml
DashboardPage.xaml ⭐ [MINIMAL v0.7.21]
  - Service status card (simple)
  - Pipeline list (basic)
  - Auto-refresh (5 sec)
  - Uses ServiceStatusToColorConverter

DashboardPage.xaml.cs [SIMPLIFIED v0.7.21]
  - Just InitializeComponent()
  - No complex initialization
  - ViewModel from NavigationService

PipelineConfigPage.xaml [Replaces Settings!]
  - Pipeline list/grid
  - Pipeline editor form
  - Enable/disable toggles
  - Watch folder config
  [NOTE: Empty in UI - needs fix]

MappingRulePage.xaml [DICOM Mapping]
  - Mapping set selector
  - Rule editor grid
  - EXIF → DICOM mapping
  - Import/Export buttons

ServiceControlPage.xaml [Service Management]
  - Service status display
  - Start/Stop/Restart buttons
  - Event log viewer
  - Quick actions

DeadLettersPage.xaml [Error Management]
  - Simple error folder viewer
  - No complex queue UI
  - Windows Explorer integration

AboutPage.xaml [Info & Easter Eggs]
  - Version info
  - Vogon Poetry easter egg
  - Company info

SettingsPage.xaml [REMOVED in v0.7.20!]
```

### 🚧 DEAD CODE & TECHNICAL DEBT

```yaml
Still Present but Unused:
- Some interface definitions in Core (2 remain)
- Several [Obsolete] methods
- ~140 build warnings

Successfully Removed:
- IDeadLetterService & implementation ✅
- DeadLetterQueue & processor ✅
- Complex retry mechanisms ✅
- Settings page from Config Tool ✅
- IFileProcessor interface ✅
- IProcessingQueue interface ✅
- IExifReader interface ✅
- IFolderWatcher interface ✅
- IDicomConverter interface ✅
- INotificationService interface ✅
- CamBridgeSettings V1 ✅
- ValidateInfrastructure ✅
- FolderConfiguration class ✅
- Legacy workarounds in CamBridgeSettingsV2 ✅
```

### 🔒 PROTECTED FEATURES [DO NOT IMPLEMENT YET!]

```yaml
Protected for Future Sprints:
- FTP Server (Sprint 20+)
- C-STORE SCP (Sprint 21+)  
- Modality Worklist (Sprint 22+)
- C-FIND SCP (Sprint 23+)
- HL7 Integration (Sprint 24+)

WHY PROTECTED:
- Avoid scope creep
- Keep it simple first
- Get MVP working perfectly
- Medical compliance complexity
```

## ⚠️ CRITICAL GOTCHAS & FIXES

### 1. The Config Format Trap
```json
// WRONG - Will fail silently! ❌
{
  "Pipelines": [...],
  "Service": {...}
}

// CORRECT - Must have wrapper! ✅
{
  "CamBridge": {
    "Version": "2.0",
    "Pipelines": [...],
    "Service": {...}
  }
}
```

### 2. The OutputOrganization Enum
```csharp
// VALID VALUES ONLY!
"OutputOrganization": "None"              // ✅
"OutputOrganization": "ByPatient"         // ✅  
"OutputOrganization": "ByDate"            // ✅
"OutputOrganization": "ByPatientAndDate"  // ✅

// THESE WILL CRASH! (Fixed in v0.7.17 with validation)
"OutputOrganization": "PatientName"       // ❌ Now shows clear error!
"OutputOrganization": "patientname"       // ❌ Now shows clear error!
"OutputOrganization": "Patient"           // ❌ Now shows clear error!
```

### 3. The Port Mismatch (FIXED but remember!)
```yaml
Service expects: 5111
Old configs had: 5050
Result: Dashboard shows nothing
Fix: Replace ALL occurrences!
Check: ServiceInfo.cs, Program.cs, ViewModels, appsettings.json
```

### 4. The Missing Host Property
```csharp
// In App.xaml.cs - ONE LINE fixes 144 errors!
public partial class App : Application
{
    private IHost _host;
    public IHost Host => _host;  // ADD THIS LINE!
}
```

### 5. The Version Hardcoding (FIXED in v0.7.16!)
```csharp
// OLD (hardcoded - bad!)
public const string Version = "0.7.9";

// NEW (dynamic - good!)
public static string Version => 
    FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location)
        .FileVersion?.TrimEnd(".0") ?? "0.7.21";
```

### 6. InitializePrimaryConfig (Was never broken!)
```yaml
Problem: Thought method was incomplete
Reality: Method was complete all along
Learning: Check sources thoroughly!
Status: Working perfectly ✅
```

### 7. Direct Dependencies (v0.7.18!)
```yaml
Problem: Too many interfaces
Solution: Direct class registration
Impact: Simpler, clearer code
Example:
  OLD: services.AddSingleton<INotificationService, NotificationService>();
  NEW: services.AddSingleton<NotificationService>();
```

### 8. Pipeline Isolation (v0.7.20!)
```yaml
Problem: FileProcessor was singleton
Solution: Each pipeline creates own instance
Impact: True pipeline isolation
Key changes:
  - PipelineManager creates FileProcessor
  - ProcessingQueue gets direct dependency
  - DicomOverrides vs DicomSettings resolved
```

### 9. The InitializeAsync Trap (v0.7.21!)
```yaml
Problem: ViewModel has InitializeAsync but never called
Cause: DataContextChanged doesn't fire if already set
Fix: Use Loaded event or initialize in constructor
Better: Don't use InitializeAsync pattern at all!
Example: Dashboard in Session 69
```

### 10. The Minimal Victory Pattern (v0.7.21!)
```yaml
When debugging for hours with no progress:
1. Accept the complex approach failed
2. Delete the complex code
3. Write minimal version
4. Use direct dependencies
5. Celebrate when it works!
Example: Dashboard rewrite in Session 69
Result: 50 lines worked where 200 failed!
```

## 🛠️ STANDARD WORKFLOWS

### Daily Development Cycle
```powershell
# Morning start
cd C:\Users\oliver.stern\source\repos\CamBridge
git pull

# Development loop
0[TAB]   # Build (20 sec)
1[TAB]   # Deploy & Start
2[TAB]   # Open Config Tool
9[TAB]   # Test

# Check changes
git status
git diff

# Commit
git add .
git commit -m "feat: implement feature X"
```

### Debugging Workflow
```powershell
# 1. Check service status
Get-Service CamBridgeService | Format-List *

# 2. Check event log for errors
Get-EventLog -LogName Application -Source CamBridge* -Newest 20

# 3. Test API directly
Invoke-RestMethod "http://localhost:5111/api/status" | ConvertTo-Json -Depth 5

# 4. Run in console mode for live debugging
.\4-console.ps1

# 5. Check config validity
.\Debug-CamBridgeJson.ps1
```

### Config Reset Procedure
```powershell
# When config is corrupted
Stop-Service CamBridgeService -Force
Remove-Item "$env:ProgramData\CamBridge\appsettings.json"
Start-Service CamBridgeService
# Service creates fresh config automatically with InitializePrimaryConfig()
```

### Pipeline Testing
```powershell
# 1. Generate QR Code
.\Test-QRBridge.ps1

# 2. Place test JPEG in watch folder
Copy-Item "test.jpg" "C:\CamBridge\Watch\Radiology\"

# 3. Monitor results
Get-ChildItem "C:\CamBridge\Output\Radiology" -Filter "*.dcm"
Get-ChildItem "C:\CamBridge\Errors" -Filter "*"

# 4. Check processing log
Get-EventLog -LogName Application -Source CamBridge* -After (Get-Date).AddMinutes(-5)
```

## 🏗️ ARCHITECTURE DECISIONS RECORD

### Why No Interfaces? (KISS Victory)
```csharp
// Before (overengineered) - Sessions 1-40
public interface IExifReader { }
public class ExifToolReader : IExifReader { }
public class MockExifReader : IExifReader { }  // Never used!
services.AddSingleton<IExifReader, ExifToolReader>();

// After (simple) - Session 50+, perfected in 67
public class ExifToolReader { }
services.AddSingleton<ExifToolReader>();
// Mock? Just use the real thing or don't test it!
```

### Why V2 Config Format?
```yaml
V1 Problems:
- Flat structure
- Single pipeline only
- No extensibility

V2 Benefits:
- Multiple pipelines
- Hierarchical settings
- Room for growth
- Clear versioning

Decision: Keep V2, remove V1 code
```

### Why Port 5111?
```yaml
Original: Random choice (5050)
Problem: Inconsistently used
Solution: Global constant everywhere
Learning: Pick one, stick to it, document it
Current: 5111 is our port forever
```

### Why Channel-based Processing?
```yaml
Considered:
- Traditional queue (complex)
- Event-driven only (unreliable)
- Database queue (overkill)

Chosen: System.Threading.Channels
- Built into .NET
- Fast and reliable
- Simple to use
- No external dependencies
```

### Why Remove Dead Letter Queue?
```yaml
Original: 650 LOC complex retry system
Reality: Never properly used
Replacement: Simple error folder
Result: 
- Same functionality
- 645 lines deleted
- Much easier to debug
- Users understand folders
```

### Why Direct Dependencies? (v0.7.18)
```yaml
Original: 8+ interfaces
Reality: Single implementations only
Benefit:
- Less code
- Clearer dependencies
- No mocking needed
- Easier to understand
Result: 2 interfaces remain (75% reduction!)
```

### Why Pipeline-Specific FileProcessor? (v0.7.20)
```yaml
Original: Singleton FileProcessor
Problem: Needs pipeline configuration
Solution: Create per pipeline
Benefits:
- True pipeline isolation
- Medical data separated
- No shared state
- Correct output paths
Impact: Architecture finally correct!
```

### Why Minimal Dashboard? (v0.7.21)
```yaml
Original: Complex IApiService pattern
Problem: Wouldn't show data for hours
Solution: Direct HttpClient calls
Benefits:
- Working in minutes
- 50 lines vs 200
- No complex initialization
- Timer in constructor
Impact: Sometimes simple beats "proper"!
```

## 📚 KEY LEARNINGS CRYSTALLIZED

### Technical Wisdoms
1. **KISS beats SOLID** - Every single time, no exceptions
2. **Delete > Refactor** - Less code = less bugs = less maintenance
3. **Constants everywhere** - One source of truth prevents hours of debugging
4. **Tab-complete everything** - Developer efficiency matters
5. **Working > Perfect** - Ship it, then improve it
6. **Direct > Abstract** - Interfaces without multiple implementations = delete
7. **Isolate pipelines** - Medical data requires separation
8. **Minimal when stuck** - Sometimes throwing away is faster than fixing

### Process Wisdoms  
1. **Sources First** - Always check what exists before coding
2. **User knows best** - Oliver's ideas consistently better than mine
3. **Small fixes matter** - One line can fix 144 errors
4. **Document decisively** - Future you will thank current you
5. **Test immediately** - Build success ≠ feature works
6. **Architecture debt** - Must be paid eventually
7. **Frustration = signal** - Time to try different approach

### Debugging Wisdoms
1. **Check the obvious** - Port numbers, service names, typos
2. **Read the error** - It usually tells you exactly what's wrong
3. **Validate assumptions** - Is config loaded? Is service running?
4. **Use the tools** - Event Log, API endpoints, console mode
5. **When stuck, restart** - Clean slate often reveals issues
6. **Compare working vs broken** - Session 69 pattern!

### Architecture Wisdoms
1. **Start simple** - You can always add complexity later
2. **Direct is better** - Dependency injection doesn't need interfaces
3. **Frameworks serve you** - Not the other way around
4. **Patterns are tools** - Not rules to follow blindly
5. **Evolution is OK** - Code can grow and change
6. **Singletons + Config = Problems** - Isolate state per context
7. **Working ugly > Beautiful broken** - Ship it!

## 🎯 CURRENT STATE & PRIORITIES

### Complete (v0.7.21) ✅
```yaml
Pipeline Architecture Fixed:
✅ FileProcessor per pipeline
✅ True pipeline isolation  
✅ DicomOverrides working correctly
✅ V1 settings completely removed
✅ All build errors resolved
✅ Service builds and runs

Dashboard Working:
✅ Service status visible
✅ Version and uptime shown
✅ Pipelines listed
✅ Auto-refresh working
✅ Minimal approach victory

Session 69 Achievements:
✅ NavigationService injects ViewModels
✅ Direct HTTP pattern proven
✅ Complex debugging overcome
✅ User frustration resolved
```

### Immediate (Sprint 16)
```yaml
UI Polish:
1. Fix empty Pipeline Config page
2. Modern dashboard design
3. Interactive features
4. Live activity feed
```

### Short Term (v0.8.0)
```yaml
Warning Reduction Sprint:
1. Reduce warnings <50
2. Fix nullable references
3. Clean unused code
4. Remove obsolete methods
```

### Medium Term (v0.9.0)
```yaml
Final Cleanup:
1. Last 2 interfaces?
2. Performance optimization
3. Memory profiling
4. Load testing
```

### Long Term (v1.0.0)
```yaml
Medical Features (Protected):
1. Basic FTP Server
2. DICOM C-STORE SCP
3. Modality Worklist
[DO NOT START YET]
```

## 🔧 TROUBLESHOOTING GUIDE

### Service Won't Start
```powershell
# 1. Check event log for exact error
Get-EventLog -LogName Application -Source CamBridge* -Newest 10

# 2. Validate config file
.\Debug-CamBridgeJson.ps1

# 3. Try console mode
.\4-console.ps1

# 4. Reset to fresh config
Remove-Item "$env:ProgramData\CamBridge\appsettings.json" -Force
```

### Dashboard Shows Nothing (FIXED in v0.7.21!)
```yaml
Old Complex Approach:
1. Check ports
2. Check API
3. Debug ViewModels
4. Hours of frustration

New Minimal Approach:
1. Direct HttpClient
2. Timer in constructor
3. Works immediately!
```

### Pipeline Not Processing
```yaml
Verify:
1. Watch folder exists
2. Pipeline is enabled
3. File has .jpg extension
4. EXIF contains barcode data
5. Output folder is writable
6. Pipeline has own output path!
```

### Build Errors
```yaml
Common fixes:
1. Host property in App.xaml.cs
2. IsError property in MappingEditorViewModel
3. Remove SettingsPage files
4. Check DicomOverrides vs DicomSettings
```

## 🚨 EMERGENCY PROCEDURES

### Complete Service Reset
```powershell
# Nuclear option - complete reinstall
Stop-Service CamBridgeService -Force -ErrorAction SilentlyContinue
sc.exe delete CamBridgeService

# Delete all config
Remove-Item "$env:ProgramData\CamBridge" -Recurse -Force

# Rebuild and deploy fresh
00[TAB]  # Build with ZIP
1[TAB]   # Deploy fresh
```

### Config Corruption Recovery
```powershell
# Fix wrapper issue
$config = Get-Content "$env:ProgramData\CamBridge\appsettings.json" -Raw | ConvertFrom-Json
$wrapped = @{ CamBridge = $config }
$wrapped | ConvertTo-Json -Depth 10 | Set-Content "$env:ProgramData\CamBridge\appsettings.json"
```

### Version Mismatch Fix
```powershell
# Always trust Version.props
$version = Select-Xml -Path ".\Version.props" -XPath "//VersionPrefix" | Select -ExpandProperty Node
Write-Host "Building version $($version.InnerText)"
0[TAB]  # Build with new version
```

## 📊 PROJECT METRICS

```yaml
Current Version: 0.7.21
Total LOC: 14,350+
Written By: Claude (100%)
Deleted: 700+ LOC
Interfaces: 2 (from 12+ → massive reduction!)
Warnings: ~140 (target: <50)
Working Features: Multi-pipeline processing
API Endpoints: 4/5 implemented
Test Coverage: Manual only
Documentation: Wisdom files + code comments
Architecture: Pipeline-isolated processing!
Dashboard: Minimal but working!
```

## 🎉 SUCCESS CRITERIA

### MVP Complete When:
- [x] Watches folders for JPEG files
- [x] Extracts EXIF barcode data
- [x] Creates valid DICOM files
- [x] Multiple pipeline support
- [x] Config tool works
- [x] Service runs stable
- [x] Core conversion tested & working
- [x] All enums validate properly
- [x] Config errors are clear
- [x] Direct dependencies implemented
- [x] Pipeline isolation complete
- [x] Dashboard shows service status

### Production Ready When:
- [x] Zero crashes on bad input (enum validation added!)
- [ ] Warnings < 50
- [ ] Automated tests exist
- [ ] Performance optimized
- [ ] Documentation complete
- [x] Error handling robust
- [x] Pipeline isolation verified

---

*"Making the improbable reliably simple - one minimal solution at a time!"*

*Master reference for CamBridge v0.7.21 - Dashboard works through minimalism!*
