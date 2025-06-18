# WISDOM_META.md - CamBridge Code Map & Architecture
**Version**: 0.7.26  
**Last Update**: 2025-06-18 15:42  
**Purpose**: Complete code map, architecture, classes - the WHAT  
**Philosophy**: Medical imaging pipeline with KISS implementation

## 🚀 QUICK FACTS

```yaml
Total LOC: 14,850+ (all by Claude!)
Projects: 5 (Core, Infrastructure, Service, Config, QRBridge)
Interfaces: 2 remaining (was 12+)
Build Time: 20 seconds
API Port: 5111
Config Format: V2 with CamBridge wrapper
Architecture: Pipeline-isolated processing
Current Focus: Transform Editor complete! (Session 74)
```

## 🗺️ PROJECT STRUCTURE

```
CamBridge Solution
├── CamBridge.Core (~3,200 LOC) - Domain models & interfaces
├── CamBridge.Infrastructure (~4,900 LOC) - Services & implementation
├── CamBridge.Service (~2,100 LOC) - Windows Service & API
├── CamBridge.Config (~4,500 LOC) - WPF Configuration UI [+600 LOC]
└── CamBridge.QRBridge (~350 LOC) - QR Code generator tool
```

## 📁 CAMBRIDGE.CORE - Domain Layer

### Configuration Classes
```yaml
ConfigurationPaths.cs ⭐ [CRITICAL]
  - GetPrimaryConfigPath(): string
  - GetPipelineConfigDirectory(): string
  - GetMappingRulesDirectory(): string
  - GetErrorDirectory(): string  
  - InitializePrimaryConfig(): bool
  - EnsureDirectoriesExist(): void

CamBridgeSettingsV2.cs ⭐ [Current Config Format]
  - Version: string = "2.0"
  - Service: ServiceSettings
  - Pipelines: List<PipelineConfiguration>
  - MappingSets: List<MappingSet>
  - GlobalDicomSettings: DicomSettings
  - DefaultProcessingOptions: ProcessingOptions

PipelineConfiguration.cs ⭐
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
  - MaxConcurrentProcessing: int
  - ErrorFolder: string
  - DeadLetterFolder: string? [TO BE REMOVED!]
```

### Domain Entities
```yaml
PatientInfo.cs
  - Id: PatientId
  - Name: string
  - BirthDate: DateTime?
  - Gender: Gender (enum)

StudyInfo.cs  
  - StudyId: StudyId
  - Description: string?
  - StudyDate: DateTime
  - AccessionNumber: string?

ImageMetadata.cs
  - SourceFilePath: string
  - CaptureDateTime: DateTime
  - Patient: PatientInfo
  - Study: StudyInfo
  - TechnicalData: ImageTechnicalData
  - ExifData: Dictionary<string,string>

MappingSet.cs ⭐ [Updated in v0.7.26]
  - Id: Guid
  - Name: string
  - Description: string?
  - Rules: List<MappingRule>
  - IsSystemDefault: bool
  - CreatedDate: DateTime
  - ModifiedDate: DateTime
  - CreatedAt: DateTime
  - UpdatedAt: DateTime
```

### Interfaces (Only 2 Left!)
```yaml
IMappingConfiguration.cs
  - GetMappingRules(): IReadOnlyList<MappingRule>
  - LoadConfigurationAsync(string?): Task<bool>

IDicomTagMapper.cs
  - ApplyTransform(value, transform): string?
  - MapToDataset(dataset, sourceData, rules): void
```

### Value Objects
```yaml
DicomTag.cs ⭐ [NEMA-compliant constants]
  - PatientModule: Name, ID, BirthDate, Sex, Comments
  - StudyModule: UID, Date, Time, ID, AccessionNumber, Description
  - SeriesModule: UID, Number, Date, Time, Description, Modality
  - InstanceModule: SOPInstanceUID, InstanceNumber, ContentDate/Time
  - EquipmentModule: Manufacturer, InstitutionName, ModelName

ValueTransform.cs [Hidden Treasure - Session 74!]
  - None
  - DateToDicom
  - TimeToDicom
  - DateTimeToDicom
  - MapGender
  - RemovePrefix
  - ExtractDate
  - ExtractTime
  - ToUpperCase
  - ToLowerCase
  - Trim
  [11 transforms - all working!]
```

## 📁 CAMBRIDGE.INFRASTRUCTURE - Implementation Layer

### Core Services
```yaml
ExifToolReader.cs ⭐ [NO INTERFACE]
  - ExtractDataAsync(imagePath): Task<Dictionary<string,string>>
  - ParseBarcodeData(exifData): (PatientInfo?, StudyInfo?)
  - ParseQRBridgeData(barcodeData): Dictionary<string,string>

DicomConverter.cs ⭐ [NO INTERFACE]
  - ConvertToDicomAsync(jpeg, dicom, metadata): Task<ConversionResult>
  - ValidateDicomFileAsync(path): Task<ValidationResult>

FileProcessor.cs ⭐⭐ [CREATED PER PIPELINE!]
  - Constructor(logger, exifReader, dicomConverter, pipelineConfig, globalDicomSettings)
  - ProcessFileAsync(inputPath): Task<FileProcessingResult>
  - NOT A SINGLETON - Each pipeline gets its own!

PipelineManager.cs ⭐⭐⭐ [ORCHESTRATOR]
  - StartAsync(settings): Task
  - StopAsync(): Task
  - EnablePipelineAsync(pipelineId): Task
  - GetPipelineStatuses(): Dictionary<string, PipelineStatus>
  - CreatePipelineContext(): Creates FileProcessor per pipeline!

ProcessingQueue.cs [Channel-based]
  - Constructor(logger, fileProcessor, options)
  - TryEnqueue(filePath): bool
  - ProcessQueueAsync(token): Task
  - Uses injected FileProcessor directly

NotificationService.cs [NO INTERFACE]
  - SendDailySummaryAsync(summary): Task
  - NotifyErrorAsync(message, exception?): Task
  - Just logs - no email implementation
```

### Service Registration
```csharp
// ServiceCollectionExtensions.cs
services.AddSingleton<ExifToolReader>();
services.AddSingleton<DicomConverter>();
services.AddSingleton<PipelineManager>();
services.AddSingleton<NotificationService>();
// NO FileProcessor - created per pipeline!
// Interfaces:
services.AddSingleton<IMappingConfiguration, MappingConfigurationLoader>();
services.AddSingleton<IDicomTagMapper, DicomTagMapper>();
```

## 📁 CAMBRIDGE.SERVICE - Windows Service

```yaml
Program.cs ⭐ [Entry Point]
  - Port: 5111
  - Minimal API endpoints
  - Service registration
  API Endpoints:
    GET /api/status ✅
    GET /api/pipelines ✅
    GET /api/status/version ✅
    GET /api/status/health ✅
    GET /api/statistics ❌ (404)

ServiceInfo.cs
  - Version: dynamic from assembly
  - ServiceName: "CamBridgeService"
  - ApiPort: 5111

Worker.cs [Background Service]
  - Uses PipelineManager
  - Starts/stops pipelines
  - Health monitoring

StatusController.cs
  - GetStatus(): Full service status
  - GetPipelines(): Pipeline configs
  - GetStatistics(): Processing stats
```

## 📁 CAMBRIDGE.CONFIG - WPF UI (ENHANCED!)

### Application Core
```yaml
App.xaml.cs ⭐
  - Host: IHost property [CRITICAL!]
  - DI container setup
  - Service registration

MainWindow.xaml/cs
  - NavigationView left menu
  - Frame for page content
  - Navigation handling
  - NavigationUIVisibility="Hidden" [Session 71!]
```

### ViewModels (MVVM)
```yaml
ViewModelBase.cs
  - PropertyChanged implementation
  - SetProperty<T> helper

DashboardViewModel.cs [MINIMAL v0.7.21]
  - Direct HttpClient usage
  - Simple timer refresh
  - ServiceStatus, PipelineStatuses
  - NO IApiService dependency!

PipelineConfigViewModel.cs
  - Pipelines: ObservableCollection<PipelineConfiguration>
  - Add/Edit/Delete commands
  - Save/Load functionality

MappingEditorViewModel.cs ⭐ [ENHANCED v0.7.26]
  - MappingSets: ObservableCollection<MappingSet>
  - SelectedMappingSet: MappingSet
  - MappingRules: ObservableCollection<MappingRule>
  - ImportCommand, ExportCommand, SaveCommand
  - ApplyTemplateCommand
  - EditTransformCommand [NEW!]
  - Drag & drop support [WORKING!]
  - DICOM tag browser [WORKING!]
  - AddRuleFromField method
  - Smart transform detection
  - Save success feedback [NEW!]

DeadLettersViewModel.cs
  - Simple error folder viewer
  - [TO BE ENHANCED]
```

### Services (UI Support)
```yaml
NavigationService.cs ⭐
  - Page registration & navigation
  - ViewModel injection for pages
  - Frame management
  - History clearing [Session 71!]

ConfigurationService.cs
  - Load/Save JSON configs
  - Enum validation
  - V2 format with wrapper

ServiceManager.cs
  - Windows Service control
  - Start/Stop/Status
```

### Views (Pages) - REDESIGNED!
```yaml
DashboardPage.xaml [UPDATED v0.7.24]
  - Service status card
  - Pipeline list with error counts
  - Auto-refresh every 5 seconds

PipelineConfigPage.xaml
  - Pipeline management
  - Watch folder settings
  - [DeadLetterFolder still present!]

MappingEditorPage.xaml [REDESIGNED v0.7.25] ⭐⭐⭐
  - NO MORE CHEAT SHEET!
  - Expanded mapping rules area
  - Source Fields list (EXIF/Barcode)
  - Mapping Rules with transform display
  - Drag & Drop working perfectly
  - Browse tags in header
  - Shows DICOM tag names
  - Name input for new sets
  - Transform edit buttons [NEW v0.7.26!]

ServiceControlPage.xaml
  - Service start/stop
  - Event log viewer
  - Works perfectly

DeadLettersPage.xaml
  - Simple error folder
  - [Needs enhancement]
```

### Dialogs (ENHANCED!)
```yaml
TransformEditorDialog.xaml/cs [NEW v0.7.26!] ⭐⭐⭐
  - Multi-view preview (Normal/Special/HEX)
  - Encoding detection (UTF-8/Windows-1252)
  - DICOM compliance hints
  - Transform-aware preview inputs
  - Special character visualization ([CR], [LF], [TAB])
  - Hex dump view for debugging
  - Professional medical software quality

DicomTagBrowserDialog.xaml/cs [ENHANCED v0.7.25]
  - 3-column layout: Tag | Name | Description
  - NEMA PS3.6 compliant descriptions
  - Better search (includes descriptions)
  - Group by module
  - Tooltips with VR explanations
  - Professional medical software quality
```

### Converters (UPDATED!)
```yaml
ValueConverters.cs [ENHANCED v0.7.26]
  - All existing converters
  - TransformToSymbolConverter (→, 📅→, ♂♀→)
  - TransformToDescriptionConverter
  - Shows transform types visually
  - Proper encoding needed (© not Â©)!
```

## 🏗️ ARCHITECTURE PATTERNS

### Pipeline Isolation
Each pipeline has:
- Own FileProcessor instance
- Own ProcessingQueue
- Own FileSystemWatcher
- Own output folder
- Own error handling

### Direct Dependencies
- No unnecessary interfaces
- Direct class injection
- Simpler, clearer code
- Only 2 interfaces remain

### Configuration Flow
```
Version.props → Assembly → ServiceInfo → API/UI
ConfigurationPaths → appsettings.json → All components
```

### Processing Flow
```
JPEG → ExifToolReader → Metadata → DicomConverter → DICOM
         ↓                            ↑
    QRBridge Data ──────────────────→
```

### Event Handler Pattern (Session 72)
```
XAML → AllowDrop="True"
     → Drop="MappingRules_Drop"
     → DragOver="MappingRules_DragOver"
     
XAML.CS → Connect handlers in constructor
        → Handle drag data
        → Update ViewModel
```

### UI Clarity Pattern (Session 73)
```
User Question → Critical Analysis → Decision
"Do we need it?" → "What value?" → "DELETE!"
Result: Clean, focused, professional UI
```

### Hidden Treasure Pattern (Session 74)
```
User Need → Check Existing Code → Find It's There!
Example: Transform system fully implemented
Solution: Just add UI to expose it
Result: Professional Transform Editor
```

## 📊 METRICS & STATUS

### Code Quality
- Warnings: ~140 (mostly nullable)
- Deleted: 1000+ LOC (Dead Letter + UI clutter)
- Interfaces: 2 (from 12+)
- Pipeline isolation: Complete
- UI clarity: Much improved!
- Hidden features: Being discovered!

### Feature Status
```yaml
Core Pipeline: Working ✅
Multi-Pipeline: Working ✅
Config UI: Working ✅
Mapping Editor: Perfect ✅
Transform Editor: Complete ✅
Error Handling: Basic ⚠️
Service Control: Perfect ✅
Navigation: Fixed ✅
Dashboard: Working ✅
UI Design: Clean & Professional ✅
```

### Known Issues (Session 74)
```yaml
Minor:
  - Encoding (© vs Â©) in multiple files
  - Dead Letter UI references
  - Enhanced error management pending
  - ~140 build warnings

Major:
  - None! All major issues fixed!
```

## 🚧 TECHNICAL DEBT

```yaml
To Remove:
- DeadLetterFolder property & UI
- Last 2 interfaces (maybe)
- Build warnings
- Encoding issues

To Enhance:
- Error management UI
- Performance optimization

To Add:
- Missing API endpoint (/api/statistics)
```

## 🔒 PROTECTED FEATURES

Future sprints (DO NOT START):
- FTP Server
- C-STORE SCP
- Modality Worklist
- HL7 Integration

## 🎯 SESSION 74 ACHIEVEMENTS

```yaml
Transform Editor Complete:
✓ Hidden system discovered
✓ Professional dialog created
✓ Multi-view preview working
✓ Encoding detection implemented
✓ Transform-aware test inputs
✓ DICOM compliance hints
✓ XAML compatibility fixed
✓ Integration complete

Technical Improvements:
✓ ContentDialog vs Window patterns
✓ XAML property limitations understood
✓ Async command patterns
✓ Hidden features exposed
```

## 🎯 SESSION 73 ACHIEVEMENTS

```yaml
UI Redesign Complete:
✓ Cheat sheet removed (40% more space)
✓ DICOM tag names visible
✓ Transform indicators added
✓ NEMA-compliant browser
✓ 3-column layout with descriptions
✓ Professional medical software quality
✓ Clean, focused interface
✓ User request implemented perfectly

Technical Improvements:
✓ New converters for transforms
✓ Enhanced search in browser
✓ Proper XAML structure
✓ No more Run opacity issues
✓ Event handlers all working
```

---

*"Complete code map for CamBridge v0.7.26 - Session 74 Transform Editor complete!"*
*Hidden treasures discovered - Transform system exposed to users!*
