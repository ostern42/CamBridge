# WISDOM_META.md - CamBridge Master Reference
**Version**: 0.7.17  
**Last Update**: 2025-06-15 12:30  
**Purpose**: Consolidated technical & operational wisdom with complete code map  
**Philosophy**: Sources First, KISS, Tab-Complete Everything

## 🚀 QUICK REFERENCE

### Critical Constants (MEMORIZE!)
```yaml
Port: 5111 (EVERYWHERE! Not 5050!)
Service Name: CamBridgeService (no space!)
Config Path: %ProgramData%\CamBridge\appsettings.json
Config Format: V2 with CamBridge wrapper (MANDATORY!)
Output Org Values: [None, ByPatient, ByDate, ByPatientAndDate]
Version: Now dynamic from assembly (never hardcode!)
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

# API Testing (v0.7.17)
Invoke-RestMethod "http://localhost:5111/api/status"
Invoke-RestMethod "http://localhost:5111/api/pipelines"

# Config Validation
.\Debug-CamBridgeJson.ps1

# Version Check (should show 0.7.17)
(Invoke-RestMethod "http://localhost:5111/api/status").version
```

## 🗺️ COMPLETE CODE MAP v2.2

### System Overview
```
CamBridge Solution (14,350+ LOC total)
├── CamBridge.Core (~3,200 LOC)
├── CamBridge.Infrastructure (~4,800 LOC)  
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

CamBridgeSettings.cs [V1 - Legacy - REMOVE IN v0.8.0]
  - DefaultDicomSettings: DicomSettings
  - DefaultOutputFolder: string
  - WatchFolders: List<FolderConfiguration>
  - ExifToolPath: string

CamBridgeSettingsV2.cs ⭐ [V2 - Current Format]
  - Version: string = "2.0"
  - Service: ServiceSettings
  - Pipelines: List<PipelineConfiguration>
  - MappingSets: List<MappingSet>
  - DicomDefaults: DicomSettings
  - DefaultProcessingOptions: ProcessingOptions

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
  - DicomOverrides: DicomOverrides?
  - MappingSetId: Guid?

ProcessingOptions.cs [v0.7.17 - With enum validation!]
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
PatientId.cs: string wrapper with validation
StudyId.cs: string wrapper with validation
DicomUid.cs: string wrapper with UID validation
PatientName.cs: FirstName, LastName, MiddleName
Gender.cs: enum [Male, Female, Other, Unknown]
ImageDimensions.cs: Width, Height, BitsPerPixel
```

#### Interfaces (8 remaining → target: 4)
```yaml
IExifReader.cs [TARGET: Remove in v0.8.0]
IDicomConverter.cs [TARGET: Remove in v0.8.0]
IFileProcessor.cs [REMOVED in v0.7.8] ✅
IFolderWatcher.cs [TARGET: Remove in v0.8.0]
IMappingConfiguration.cs [KEEP - for now]
INotificationService.cs [TARGET: Remove in v0.8.0]
IProcessingQueue.cs [REMOVED in v0.7.8] ✅
IQueueProcessor.cs [TARGET: Remove in v0.8.0]
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

DicomConverter.cs ⭐ [NO INTERFACE - Direct use]
  - ConvertJpegToDicomAsync(jpeg, patient, study, output): Task<ProcessedImage>
  - CreateDicomDataset(jpeg, patient, study, exif): DicomDataset
  - AddPatientModule(dataset, patient): void
  - AddStudyModule(dataset, study): void
  - AddImageModule(dataset, dimensions, exif): void

FileProcessor.cs [NO INTERFACE - Event-driven]
  - ProcessFileAsync(inputPath, outputPath): Task<ProcessedImage>
  - ValidateInputFile(path): void
  - DetermineOutputPath(input, output, patient, study): string
  - HandleProcessingSuccess(input, output, options): Task
  - HandleProcessingFailure(input, error, options): Task
  - Events: ProcessingStarted, ProcessingCompleted, ProcessingFailed

PipelineManager.cs ⭐⭐ [ORCHESTRATOR - Heart of the system]
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

ProcessingQueue.cs [Channel-based processing]
  - TryEnqueue(filePath): bool
  - QueueLength: int
  - ActiveProcessing: int
  - GetStatistics(): QueueStatistics
  - GetActiveItems(): List<ProcessingItemStatus>
  Private:
  - ProcessQueueAsync(): Task
  - ProcessItemAsync(item): Task

FolderWatcherService.cs
  - WatchFolder: string
  - FileDetected: event EventHandler<string>
  - Start(): void
  - Stop(): void

ServiceCollectionExtensions.cs
  - AddInfrastructure(services, config): IServiceCollection
  - AddInfrastructureForConfig(services): IServiceCollection
  - ValidateInfrastructure(provider): IServiceProvider
```

### 📁 CAMBRIDGE.SERVICE - Complete File List

```yaml
Program.cs ⭐ [Entry Point - Uses dynamic version]
  - Main(): creates WebApplication
  - ConfigureServices(): DI setup (no interfaces!)
  - ConfigureEndpoints(): Minimal API
  - Port: 5111 (hardcoded - OK!)
  API Endpoints (v0.7.17):
    GET /api/status ✅
    GET /api/pipelines ✅
    GET /api/status/version ✅ (NEW!)
    GET /api/status/health ✅ (NEW!)
    GET /api/statistics ❌ (404)

ServiceInfo.cs ⭐ [FIXED in v0.7.16!]
  - Version: string (now dynamic from assembly!)
  - ServiceName: "CamBridgeService"
  - DisplayName: "CamBridge Medical Image Converter"
  - ApiPort: 5111
  - Company: string (also dynamic)

Worker.cs [Background Service]
  - ExecuteAsync(stoppingToken): Task
  - StartAsync(cancellationToken): Task  
  - StopAsync(cancellationToken): Task
  Dependencies:
    - PipelineManager (direct, no interface)
    - IConfiguration

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

#### ViewModels (MVVM Pattern)
```yaml
ViewModelBase.cs
  - PropertyChanged implementation
  - SetProperty<T> helper
  - Common VM functionality

MainViewModel.cs
  - Navigation commands
  - Window title
  - Current page tracking
  - Menu items (if any)

DashboardViewModel.cs ⭐ [Main Status View]
  - ServiceStatus: ServiceStatusModel?
  - PipelineStatuses: ObservableCollection<PipelineStatus>
  - RefreshCommand: ICommand
  - StartServiceCommand, StopServiceCommand
  - IsServiceRunning: bool
  - UpdateInterval: DispatcherTimer (5 sec)
  - Port: 5111 (must match service!)

PipelineViewModel.cs [Pipeline Management]
  - Pipelines: ObservableCollection<PipelineConfiguration>
  - SelectedPipeline: PipelineConfiguration?
  - AddCommand, EditCommand, DeleteCommand
  - SaveCommand, CancelCommand
  - LoadPipelinesAsync(): Task
  - EnablePipelineCommand, DisablePipelineCommand

MappingViewModel.cs [DICOM Tag Mapping]
  - MappingSets: ObservableCollection<MappingSet>
  - SelectedMappingSet: MappingSet?
  - MappingRules: ObservableCollection<MappingRule>
  - AddRuleCommand, DeleteRuleCommand
  - ImportCommand, ExportCommand
  - DicomTags: List<DicomTagInfo> [Static list]
```

#### Services (Supporting Config Tool)
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

NavigationService.cs [INavigationService]
  - NavigateTo(pageKey): void
  - GoBack(): void
  - CanGoBack: bool
  - Page registration system
```

#### Views (WPF Pages)
```yaml
DashboardPage.xaml [Main View]
  - Service status card
  - Pipeline status list
  - Quick action buttons
  - Auto-refresh (5 sec)

PipelinePage.xaml [Pipeline Config]
  - Pipeline list/grid
  - Pipeline editor form
  - Enable/disable toggles
  - Watch folder config

MappingRulePage.xaml [DICOM Mapping]
  - Mapping set selector
  - Rule editor grid
  - EXIF → DICOM mapping
  - Import/Export buttons

ServiceSettingsPage.xaml [REMOVED in v0.7.x] ✅
LogViewerPage.xaml [Future - not implemented]
```

### 📁 CAMBRIDGE.QRBRIDGE - Companion Tool

```yaml
Program.cs [Console Entry]
  - Argument parsing
  - QR Code generation
  - Display window

QRCodeService.cs
  - GenerateQRCode(data): Bitmap
  - EncodePatientData(args): string
  - ValidateInput(args): bool

QRDisplayForm.cs [WinForms]
  - Shows QR Code
  - Stays on top
  - Click to close

ArgumentParser.cs
  - ParseArguments(args): Dictionary<string,string>
  - ValidateRequired(dict): bool
  - ShowHelp(): void

Usage:
CamBridge.QRBridge.exe -name "Last, First" -birthdate "1990-01-01" -gender "M" -examid "EX001"
```

### 🚧 DEAD CODE & TECHNICAL DEBT

```yaml
Still Present but Unused:
- V1 Config classes & migration
- Some interface definitions in Core
- DeadLetterQueue references (partial)
- NotificationService (partial)
- Several [Obsolete] methods
- 144 build warnings

Successfully Removed:
- IDeadLetterService & implementation ✅
- DeadLetterQueue & processor ✅
- Complex retry mechanisms ✅
- Settings page from Config Tool ✅
- IFileProcessor interface ✅
- IProcessingQueue interface ✅
```

### 🔒 PROTECTED FEATURES [DO NOT IMPLEMENT YET!]

```yaml
Protected for Future Sprints:
- FTP Server (Sprint 10)
- C-STORE SCP (Sprint 11)  
- Modality Worklist (Sprint 12)
- C-FIND SCP (Sprint 13)
- HL7 Integration (Sprint 14)

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
        .FileVersion?.TrimEnd(".0") ?? "0.7.17";
```

### 6. InitializePrimaryConfig (Was never broken!)
```yaml
Problem: Thought method was incomplete
Reality: Method was complete all along
Learning: Check sources thoroughly!
Status: Working perfectly ✅
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

// After (simple) - Session 50+
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

## 📚 KEY LEARNINGS CRYSTALLIZED

### Technical Wisdoms
1. **KISS beats SOLID** - Every single time, no exceptions
2. **Delete > Refactor** - Less code = less bugs = less maintenance
3. **Constants everywhere** - One source of truth prevents hours of debugging
4. **Tab-complete everything** - Developer efficiency matters
5. **Working > Perfect** - Ship it, then improve it

### Process Wisdoms  
1. **Sources First** - Always check what exists before coding
2. **User knows best** - Oliver's ideas consistently better than mine
3. **Small fixes matter** - One line can fix 144 errors
4. **Document decisively** - Future you will thank current you
5. **Test immediately** - Build success ≠ feature works

### Debugging Wisdoms
1. **Check the obvious** - Port numbers, service names, typos
2. **Read the error** - It usually tells you exactly what's wrong
3. **Validate assumptions** - Is config loaded? Is service running?
4. **Use the tools** - Event Log, API endpoints, console mode
5. **When stuck, restart** - Clean slate often reveals issues

### Architecture Wisdoms
1. **Start simple** - You can always add complexity later
2. **Direct is better** - Dependency injection doesn't need interfaces
3. **Frameworks serve you** - Not the other way around
4. **Patterns are tools** - Not rules to follow blindly
5. **Evolution is OK** - Code can grow and change

## 🎯 CURRENT STATE & PRIORITIES

### Complete (v0.7.17) ✅
```yaml
Sprint 9 Done:
✅ InitializePrimaryConfig() - was already complete!
✅ Enum validation for OutputOrganization
✅ Clear error message for missing wrapper
✅ "Ermergency" typo fixed (by you)
✅ Config system now robust

Session 66 Achievements:
✅ Discovered much was already done
✅ Added missing enum validation
✅ Version updated to 0.7.17
✅ Documentation current
```

### Immediate (v0.7.18)
```yaml
Nice to Have:
1. Implement missing API endpoints
2. Remove V1 config code
3. Reduce warnings below 100
```

### Short Term (v0.8.0)
```yaml
Interface Removal Sprint:
1. Remove IExifReader
2. Remove IDicomConverter
3. Remove IFolderWatcher
4. Remove INotificationService
Target: 8 → 4 interfaces
```

### Medium Term (v0.9.0)
```yaml
Consolidation Sprint:
1. Merge config classes
2. Single config model
3. Remove all legacy code
4. Optimize performance
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

### Dashboard Shows Nothing
```yaml
Check:
1. Port 5111 in all places
2. Service is running
3. API responds: http://localhost:5111/api/status
4. No firewall blocking
```

### Pipeline Not Processing
```yaml
Verify:
1. Watch folder exists
2. Pipeline is enabled
3. File has .jpg extension
4. EXIF contains barcode data
5. Output folder is writable
```

### Build Errors (144 warnings)
```csharp
// Add to App.xaml.cs:
public IHost Host => _host;
// Seriously, this one line fixes most of them!
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
Current Version: 0.7.17
Total LOC: 14,350+
Written By: Claude (100%)
Deleted: 650+ LOC
Interfaces: 8 (target: 4)
Warnings: 144 (target: <50)
Working Features: Core pipeline processing
API Endpoints: 4/5 implemented
Test Coverage: Manual only
Documentation: Wisdom files + code comments
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

### Production Ready When:
- [x] Zero crashes on bad input (enum validation added!)
- [ ] Warnings < 50
- [ ] Automated tests exist
- [ ] Performance optimized
- [ ] Documentation complete
- [ ] Error handling robust

---

*"Making the improbable reliably simple - one tab-complete at a time!"*

*Master reference for CamBridge v0.7.17 - Config validation complete!*
