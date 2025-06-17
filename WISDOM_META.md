# WISDOM_META.md - CamBridge Code Map & Architecture
**Version**: 0.7.24  
**Last Update**: 2025-06-17 22:20  
**Purpose**: Complete code map, architecture, classes - the WHAT  
**Philosophy**: Medical imaging pipeline with KISS implementation

## 🚀 QUICK FACTS

```yaml
Total LOC: 14,350+ (all by Claude!)
Projects: 5 (Core, Infrastructure, Service, Config, QRBridge)
Interfaces: 2 remaining (was 12+)
Build Time: 20 seconds
API Port: 5111
Config Format: V2 with CamBridge wrapper
Architecture: Pipeline-isolated processing
```

## 🗺️ PROJECT STRUCTURE

```
CamBridge Solution
├── CamBridge.Core (~3,200 LOC) - Domain models & interfaces
├── CamBridge.Infrastructure (~4,900 LOC) - Services & implementation
├── CamBridge.Service (~2,100 LOC) - Windows Service & API
├── CamBridge.Config (~3,900 LOC) - WPF Configuration UI
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

## 📁 CAMBRIDGE.CONFIG - WPF UI

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

MappingEditorViewModel.cs
  - MappingSets management
  - Drag & drop support
  - DICOM tag browser
  - [ISSUES IN v0.7.24!]

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

ConfigurationService.cs
  - Load/Save JSON configs
  - Enum validation
  - V2 format with wrapper

ServiceManager.cs
  - Windows Service control
  - Start/Stop/Status
```

### Views (Pages)
```yaml
DashboardPage.xaml [UPDATED v0.7.24]
  - Service status card
  - Pipeline list with error counts
  - Auto-refresh every 5 seconds

PipelineConfigPage.xaml
  - Pipeline management
  - Watch folder settings
  - [DeadLetterFolder still present!]

MappingEditorPage.xaml [BROKEN v0.7.24]
  - Drag & Drop not working
  - Browse tags not working
  - New set naming missing

ServiceControlPage.xaml
  - Service start/stop
  - Event log viewer
  - Works perfectly

DeadLettersPage.xaml
  - Simple error folder
  - [Needs enhancement]
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

## 📊 METRICS & STATUS

### Code Quality
- Warnings: ~140 (mostly nullable)
- Deleted: 700+ LOC (Dead Letter)
- Interfaces: 2 (from 12+)
- Pipeline isolation: Complete

### Feature Status
```yaml
Core Pipeline: Working ✅
Multi-Pipeline: Working ✅
Config UI: Mostly working ⚠️
Mapping Editor: Broken ❌
Error Handling: Basic ⚠️
Service Control: Perfect ✅
```

### Known Issues
- Mapping Editor drag & drop
- Dead Letter UI references
- Enhanced error management pending
- ~140 build warnings

## 🚧 TECHNICAL DEBT

```yaml
To Remove:
- DeadLetterFolder property & UI
- Last 2 interfaces (maybe)
- Build warnings

To Enhance:
- Error management UI
- Mapping Editor fixes
- Performance optimization
```

## 🔒 PROTECTED FEATURES

Future sprints (DO NOT START):
- FTP Server
- C-STORE SCP
- Modality Worklist
- HL7 Integration

---

*"Complete code map for CamBridge v0.7.24 - know your territory!"*
