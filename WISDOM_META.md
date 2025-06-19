# WISDOM_META.md - Intelligent Code Navigation & Architecture
**Version**: 0.7.26  
**Purpose**: WHAT exists, WHERE to find it, HOW components connect  
**Goal**: Navigate 30,000+ LOC efficiently, find hidden features fast  
**Reality**: 14,850 LOC with hidden treasures waiting

## 🗺️ PROJECT OVERVIEW

```yaml
Total: 14,850+ LOC across 5 projects
Architecture: Pipeline-isolated medical imaging system
Key Flow: JPEG → EXIF extraction → DICOM conversion → PACS
Hidden Gems Found: Transform system (11 types)
More Hidden?: Probably! That's why we need this map

Projects by Size:
  CamBridge.Config: ~5,100 LOC (34%) - Most UI & features
  CamBridge.Infrastructure: ~4,900 LOC (33%) - Core logic
  CamBridge.Core: ~3,200 LOC (22%) - Domain models
  CamBridge.Service: ~2,100 LOC (14%) - Windows Service
  CamBridge.QRBridge: ~350 LOC (2%) - QR generator
```

## 🔍 FEATURE FINDER - "I need to..." Guide

### Core Features
```yaml
Process JPEG images:
  → FileProcessor.ProcessFileAsync() 
  → Infrastructure/Services/FileProcessor.cs (Line ~45)
  → Entry point for all processing

Extract QR/Barcode data:
  → ExifToolReader.ExtractDataAsync() (Line ~30)
  → ExifToolReader.ParseBarcodeData() (Line ~85)
  → ExifToolReader.ParseQRBridgeData() (Line ~120)
  → Infrastructure/Services/ExifToolReader.cs

Convert to DICOM:
  → DicomConverter.ConvertToDicomAsync() (Line ~40)
  → Infrastructure/Services/DicomConverter.cs
  → Uses fo-dicom library extensively

Transform data (HIDDEN TREASURE!):
  → ValueTransform enum - Core/Models/ValueTransform.cs
  → DicomTagMapper.ApplyTransform() - Infrastructure/Services/DicomTagMapper.cs
  → 11 types: DateToDicom, MapGender, ExtractDate, etc.

Map fields to DICOM tags:
  → MappingRule class - Core/Models/MappingRule.cs
  → DicomTagMapper.MapToDataset() - Infrastructure/Services/DicomTagMapper.cs
  → Supports drag & drop UI

Validate configuration:
  → ConfigurationService.ValidateEnumValues() (Line ~150)
  → Config/Services/ConfigurationService.cs
  → Clear error messages for invalid values

Manage pipelines:
  → PipelineManager - Infrastructure/Services/PipelineManager.cs
  → CreatePipelineContext() (Line ~80) - CRITICAL!
  → Each pipeline completely isolated

Handle errors:
  → ProcessingOptions.ErrorFolder - Core/Configuration/ProcessingOptions.cs
  → Simple folder approach (no complex queue)
  → FileProcessor.MoveToErrorFolder() (Line ~200)
```

### UI Features
```yaml
Show dialogs:
  → TransformEditorDialog - Config/Dialogs/TransformEditorDialog.xaml
  → DicomTagBrowserDialog - Config/Dialogs/DicomTagBrowserDialog.xaml
  → Pattern: Window.ShowDialog() vs ContentDialog.ShowAsync()

Drag & Drop mapping:
  → MappingEditorPage.xaml/cs - Config/Pages/
  → Event handlers must be connected in code-behind!
  → See SourceField_MouseMove() implementation

Display service status:
  → DashboardPage/ViewModel - Config/Pages/ & ViewModels/
  → Uses direct HttpClient (minimal pattern)
  → Refreshes every 5 seconds

Configure pipelines:
  → PipelineConfigPage/ViewModel - Config/Pages/ & ViewModels/
  → Supports multiple isolated pipelines
  → Each with own watch/output folders

Browse DICOM tags:
  → DicomTagBrowserDialog - Config/Dialogs/
  → 3-column NEMA-compliant display
  → Search includes descriptions
```

## 📁 DETAILED PROJECT BREAKDOWN

### CamBridge.Core (~3,200 LOC) - The Foundation
**Purpose**: Domain models, contracts, value objects - the WHAT

```yaml
Configuration/ (~800 LOC)
  ConfigurationPaths.cs ⭐⭐⭐ [SINGLE SOURCE OF TRUTH]
    - GetPrimaryConfigPath() → %ProgramData%\CamBridge\appsettings.json
    - GetPipelineConfigDirectory() → %ProgramData%\CamBridge\Pipelines
    - GetMappingRulesDirectory() → %ProgramData%\CamBridge\Mappings
    - InitializePrimaryConfig() → Creates default config
    - ALL paths derive from here!
    
  CamBridgeSettingsV2.cs [CURRENT CONFIG FORMAT]
    - Root wrapper: { "CamBridge": { ... } }
    - Version: "2.0" (only valid version)
    - Service: ServiceSettings
    - Pipelines: List<PipelineConfiguration>
    - GlobalDicomSettings: DicomSettings
    
  PipelineConfiguration.cs ⭐
    - Id: Guid (unique identifier)
    - Name: string (display name)
    - WatchSettings: Input folder config
    - ProcessingOptions: How to handle files
    - DicomOverrides: Pipeline-specific DICOM
    - MappingSetId: Which mapping rules to use
    
  ProcessingOptions.cs
    - OutputOrganization: enum [None, ByPatient, ByDate, ByPatientAndDate]
    - SuccessAction: enum [Delete, Archive, Move]
    - MaxConcurrentProcessing: int (default 5)
    - ErrorFolder: string (simple approach!)

Models/ (~1,000 LOC)
  PatientInfo.cs, StudyInfo.cs, ImageMetadata.cs
    - Domain entities for medical data
    - Clean, simple POCOs
    
  MappingRule.cs, MappingSet.cs
    - Field to DICOM tag mapping
    - Supports transforms!
    
  ValueTransform.cs [HIDDEN TREASURE!]
    - 11 transform types already implemented
    - Just needed UI (Session 74)

Interfaces/ (Only 2 left!)
  IMappingConfiguration.cs
    - GetMappingRules()
    - LoadConfigurationAsync()
    
  IDicomTagMapper.cs
    - ApplyTransform()
    - MapToDataset()

Constants/
  DicomTag.cs ⭐ [NEMA PS3.6 compliant]
    - All standard DICOM tags as constants
    - Grouped by module (Patient, Study, Series, etc.)
    - Used throughout the system
```

### CamBridge.Infrastructure (~4,900 LOC) - The Engine
**Purpose**: Core services, business logic - the HOW

```yaml
Services/ (~3,500 LOC)
  ExifToolReader.cs ⭐ [NO INTERFACE!]
    - ExtractDataAsync(imagePath) → EXIF as Dictionary
    - ParseBarcodeData(exifData) → Patient/Study info
    - ParseQRBridgeData(barcode) → Structured data
    - Uses ExifTool.exe via Process class
    - Handles timeout (30 seconds)
    
  DicomConverter.cs ⭐ [NO INTERFACE!]
    - ConvertToDicomAsync(jpeg, dicom, metadata) → Result
    - ValidateDicomFileAsync(path) → Validation result
    - Uses fo-dicom for all DICOM operations
    - Handles pixel data, metadata, validation
    
  FileProcessor.cs ⭐⭐⭐ [PER PIPELINE!]
    - NOT a singleton anymore!
    - ProcessFileAsync(inputPath) → Complete flow
    - Created by PipelineManager for each pipeline
    - Owns: EXIF → Metadata → DICOM → Output
    - Error handling via simple folder move
    
  PipelineManager.cs ⭐⭐⭐ [ORCHESTRATOR]
    - StartAsync(settings) → Starts all pipelines
    - StopAsync() → Graceful shutdown
    - CreatePipelineContext() → Per-pipeline isolation!
    - GetPipelineStatuses() → For dashboard
    - Manages lifecycle of all components
    
  ProcessingQueue.cs [Channel-based]
    - Uses System.Threading.Channels
    - TryEnqueue(filePath) → bool
    - ProcessQueueAsync(token) → Processing loop
    - Injected FileProcessor (pipeline-specific)
    - Configurable concurrency
    
  DicomTagMapper.cs
    - MapToDataset(dataset, sourceData, rules)
    - ApplyTransform(value, transform) → All 11 types!
    - Smart field detection
    
  NotificationService.cs [NO INTERFACE!]
    - SendDailySummaryAsync(summary)
    - NotifyErrorAsync(message, exception)
    - Currently just logs (no email yet)

Configuration/
  MappingConfigurationLoader.cs
    - Implements IMappingConfiguration
    - Loads from JSON files
    - Supports hot reload

Extensions/
  ServiceCollectionExtensions.cs
    - DI registration
    - Note: NO FileProcessor registration!
```

### CamBridge.Service (~2,100 LOC) - The Host
**Purpose**: Windows Service, API endpoints - the WHERE

```yaml
Program.cs ⭐ [Entry Point - Line 1-150]
  - Port 5111 (hardcoded everywhere!)
  - Minimal API configuration
  - Service registration
  - Swagger in development
  
  API Endpoints:
    GET /api/status → Full service status
    GET /api/pipelines → Pipeline configurations  
    GET /api/status/version → Version string only
    GET /api/status/health → Simple health check
    GET /api/statistics → TODO (returns 404)
    
Worker.cs [Background Service - Line 20-80]
  - Inherits from BackgroundService
  - Starts PipelineManager
  - Handles graceful shutdown
  - Logs lifecycle events

Controllers/ (Using minimal API pattern)
  StatusController.cs (embedded in Program.cs)
    - GetStatus() → Complete status object
    - GetPipelines() → Pipeline list
    - GetStatistics() → Not implemented
    
Models/
  ServiceInfo.cs
    - Dynamic version from assembly
    - Service metadata
    
  ServiceStatus.cs, PipelineStatus.cs
    - DTOs for API responses
    - Clean JSON serialization
```

### CamBridge.Config (~5,100 LOC) - The UI
**Purpose**: WPF Configuration Tool - the USER INTERFACE

```yaml
App.xaml.cs ⭐ [Application Entry - CRITICAL!]
  - Host property → Fixes 144 build errors!
  - DI container setup
  - Service registration for ViewModels
  - Navigation service initialization

MainWindow.xaml/cs [Shell - Line 1-200]
  - NavigationView (left menu)
  - Frame for content (NavigationUIVisibility="Hidden")
  - Navigation handling
  - Window state persistence

ViewModels/ (~1,500 LOC) [MVVM Pattern]
  ViewModelBase.cs
    - INotifyPropertyChanged base
    - SetProperty<T> helper
    
  DashboardViewModel.cs [MINIMAL PATTERN!]
    - Direct HttpClient (no abstractions)
    - Simple timer refresh (5 seconds)
    - ServiceStatus, PipelineStatuses properties
    - NO complex initialization!
    
  PipelineConfigViewModel.cs ⭐
    - Pipelines: ObservableCollection<PipelineConfiguration>
    - Add/Edit/Delete/Save commands
    - Validation logic
    - [DeadLetterFolder still referenced!]
    
  MappingEditorViewModel.cs ⭐⭐ [MOST COMPLEX]
    - Drag & drop support
    - Transform editing (NEW!)
    - DICOM tag browsing
    - Import/Export functionality
    - Smart field detection
    - Save with success feedback

Views/Pages/ (~1,200 LOC)
  DashboardPage.xaml/cs
    - Service status cards
    - Pipeline status list
    - Error count display
    - Auto-refresh UI
    
  PipelineConfigPage.xaml/cs
    - Pipeline CRUD operations
    - Watch/Output folder config
    - Processing options
    - [DeadLetterFolder in UI!]
    
  MappingEditorPage.xaml/cs ⭐⭐⭐ [MOST ENHANCED]
    - Source fields list (left)
    - Mapping rules (center - expanded!)
    - NO cheat sheet (removed Session 73)
    - Drag & drop event handlers
    - Transform indicators
    
  ServiceControlPage.xaml/cs
    - Start/Stop service
    - View Event Log
    - Service status
    
  DeadLettersPage.xaml/cs
    - Error folder viewer
    - [Needs enhancement]

Dialogs/ (~800 LOC)
  TransformEditorDialog.xaml/cs ⭐ [NEW in v0.7.26!]
    - Multi-view preview (Normal/Special/HEX)
    - Encoding detection
    - Transform-specific test data
    - DICOM compliance hints
    - ContentDialog pattern
    
  DicomTagBrowserDialog.xaml/cs [ENHANCED v0.7.25]
    - 3-column layout (Tag|Name|Description)
    - NEMA-compliant descriptions
    - Search all columns
    - Module grouping
    - Window pattern

Converters/ (~400 LOC)
  ValueConverters.cs [UI Magic]
    - BoolToVisibilityConverter (+ Inverse)
    - NullToVisibilityConverter (+ Inverse)
    - EnumToStringConverter
    - TransformToSymbolConverter (→, 📅→, ♂♀→)
    - TransformToDescriptionConverter
    - [Encoding issue: © shows as Â©]

Services/ (UI Support)
  NavigationService.cs ⭐
    - Page registration & navigation
    - ViewModel injection pattern
    - Frame management
    - History clearing
    
  ConfigurationService.cs
    - Load/Save JSON configs
    - V2 format enforcement
    - Enum validation
    - Path management
    
  ServiceManager.cs
    - Windows Service control
    - Start/Stop/Status via ServiceController
```

### CamBridge.QRBridge (~350 LOC) - The Helper
**Purpose**: QR code generator for camera integration

```yaml
Program.cs (Console App)
  - Parses command line: -examid "X" -name "Y" etc.
  - Generates QR code using QRCoder
  - Outputs as image file
  
Models/
  QRBridgeData.cs
    - Patient info structure
    - Serialization format
    
Critical: Camera writes QR payload to EXIF Barcode field!
This is how patient data gets into images.
```

## 🔗 COMPONENT CONNECTION MAP

### Data Flow Architecture
```
[Ricoh Camera] → QR Code → EXIF Barcode
                              ↓
[JPEG File] → FileSystemWatcher → ProcessingQueue
                                        ↓
                              FileProcessor.ProcessFileAsync()
                                        ↓
                              ExifToolReader.ExtractDataAsync()
                                   ↓              ↓
                        ParseBarcodeData()    Raw EXIF data
                              ↓                    ↓
                         Patient/Study ←→ ImageMetadata
                                        ↓
                              DicomConverter.ConvertToDicomAsync()
                                        ↓
                              [DICOM File] → Output Folder
                                               ↓
                                          [PACS System]
```

### Service Dependencies
```yaml
Startup Chain:
  Program.cs → Worker.cs → PipelineManager
                              ↓
                    For each pipeline:
                      Create FileProcessor
                      Create ProcessingQueue  
                      Create FileSystemWatcher
                      Start processing loop

Shared Services (Stateless - OK to share):
  - ExifToolReader (reads files)
  - DicomConverter (converts files)
  - NotificationService (logs only)
  
Pipeline-Specific (MUST be isolated):
  - FileProcessor (holds config)
  - ProcessingQueue (holds state)
  - FileSystemWatcher (monitors folder)
```

### API Communication
```yaml
Config Tool → Service API:
  DashboardViewModel → HttpClient → http://localhost:5111/api/status
                                 → http://localhost:5111/api/pipelines
  
  Refresh Pattern:
    Timer (5 sec) → RefreshAsync() → HTTP GET → Update UI
    
Service → Config Tool:
  No direct communication
  Config Tool polls for status
  Service writes to Event Log
```

### Configuration Flow
```yaml
Source of Truth:
  Version.props → Assembly → ServiceInfo.Version → API/UI
  
Config Paths:
  ConfigurationPaths.GetPrimaryConfigPath()
    → %ProgramData%\CamBridge\appsettings.json
    → Loaded by ConfigurationService
    → Used by all components
    
Pipeline Configs:
  Main config references pipeline files
  → %ProgramData%\CamBridge\Pipelines\*.json
  → Loaded on demand
  
Mapping Rules:
  Referenced by pipeline config
  → %ProgramData%\CamBridge\Mappings\*.json
  → Loaded by MappingConfigurationLoader
```

## 💎 HIDDEN FEATURES & DISCOVERIES

### Already Found (Session 74)
```yaml
Transform System:
  Location: Core/Models/ValueTransform.cs
  
  public enum ValueTransform
  {
      None,
      DateToDicom,       // "20240618" → DA format
      TimeToDicom,       // "143022" → TM format
      DateTimeToDicom,   // Full DT format
      MapGender,         // "M/F" → DICOM format
      RemovePrefix,      // Remove string prefix
      ExtractDate,       // Extract date portion
      ExtractTime,       // Extract time portion
      ToUpperCase,       // Convert to upper
      ToLowerCase,       // Convert to lower
      Trim              // Remove whitespace
  }
  
  Implementation: DicomTagMapper.ApplyTransform()
  Status: Fully working, just needed UI
  UI Added: TransformEditorDialog in v0.7.26
```

### Potential Hidden Features (Not Yet Explored)
```yaml
Search Patterns:
  1. Enums often list all capabilities
  2. Switch statements reveal features
  3. TODO/HACK comments hint at more
  4. #if DEBUG blocks may have tools
  5. Test methods might be useful
  
Areas to Investigate:
  - More validation rules?
  - Additional converters?
  - Debug-only utilities?
  - Performance monitoring?
  - Hidden API endpoints?
  - Configuration options without UI?
```

### Search Commands for Treasure Hunt
```powershell
# Find all enums (capability lists)
Get-ChildItem -Recurse *.cs | Select-String "enum\s+\w+" -Context 0,15

# Find switch statements (feature implementations)  
Get-ChildItem -Recurse *.cs | Select-String "switch.*{" -Context 0,20

# Find TODO/HACK comments
Get-ChildItem -Recurse *.cs | Select-String "TODO|HACK|FIXME" -Context 2,5

# Find test/sample data
Get-ChildItem -Recurse *.cs | Select-String "Test|Sample|Demo" -Context 1,10

# Find disabled features
Get-ChildItem -Recurse *.cs,*.xaml | 
  Select-String 'Visibility="Collapsed"|IsEnabled="False"|if \(false\)'
```

## 🎯 NAVIGATION STRATEGIES

### Finding Features by Pattern
```yaml
"I need validation":
  → Search: "Validate", "Valid", "Check"
  → Look in: Services, ViewModels
  → Check: ConfigurationService.ValidateEnumValues()

"I need UI for X":
  → Search: "Dialog", "Page", "View"
  → Look in: Config/Dialogs, Config/Pages
  → Pattern: Window vs ContentDialog

"I need to process X":
  → Search: "Process", "Handle", "Convert"
  → Look in: Infrastructure/Services
  → Start with: FileProcessor

"I need configuration for X":
  → Start: ConfigurationPaths
  → Then: Core/Configuration
  → Check: JSON examples in code
```

### Code Archaeology Tips
```yaml
When exploring unknown feature:
  1. Find the enum/constants
  2. Search for enum usage
  3. Find switch statements
  4. Look for interface implementations
  5. Check for UI bindings
  6. Trace through call stack

Example - Finding Transform UI:
  1. Found ValueTransform enum
  2. Searched "ValueTransform" → DicomTagMapper
  3. Searched "ApplyTransform" → MappingRule
  4. Searched UI for "Transform" → Found commands
  5. Added TransformEditorDialog
```

### Performance Hotspots
```yaml
Know where time is spent:
  
Heavy Operations:
  - ExifTool.exe calls (process spawn)
  - DICOM file writing (I/O)
  - Large image processing
  
Optimized Areas:
  - Channel-based queue (async)
  - Pipeline isolation (parallel)
  - Direct dependencies (no DI overhead)
  
Monitoring Points:
  - ProcessingQueue depth
  - FileProcessor timing
  - API response time
```

## 📊 CODE METRICS & INSIGHTS

### Complexity Distribution
```yaml
Most Complex:
  1. MappingEditorViewModel (~500 LOC)
     - Drag & drop logic
     - Transform handling
     - Save/Load operations
     
  2. FileProcessor (~400 LOC)
     - Complete processing pipeline
     - Error handling
     - Output organization
     
  3. PipelineManager (~350 LOC)
     - Orchestration logic
     - Lifecycle management
     - Status tracking

Most Critical:
  1. ConfigurationPaths (all paths)
  2. FileProcessor (core logic)
  3. PipelineManager (orchestration)
  4. App.xaml.cs (Host property!)

Most Enhanced:
  1. MappingEditorPage (Sessions 72-74)
  2. DicomTagBrowserDialog (Session 73)
  3. DashboardViewModel (Session 69)
  4. TransformEditorDialog (Session 74)
```

### Technical Debt Locations
```yaml
Known Issues:
  - DeadLetterFolder references (UI & config)
  - Encoding (© vs Â©) in multiple files
  - ~140 build warnings
  - Missing /api/statistics endpoint
  
Improvement Opportunities:
  - Email notifications (stubbed)
  - Performance monitoring
  - More error details in UI
  - Pipeline priorities
```

## 🚧 GROWTH PLANNING

### When Adding New Features
```yaml
Before Writing Code:
  1. Search existing implementations
  2. Check related enums/constants
  3. Look for TODO comments
  4. Verify not already hidden
  5. Consider pipeline isolation

Where to Add:
  Domain Models → Core/Models/
  Business Logic → Infrastructure/Services/
  API Endpoints → Service/Program.cs
  UI Pages → Config/Pages/
  UI Dialogs → Config/Dialogs/
  Converters → Config/Converters/

Naming Patterns:
  Services: [Feature]Service.cs
  ViewModels: [Page]ViewModel.cs
  Pages: [Feature]Page.xaml
  Dialogs: [Feature]Dialog.xaml
```

### Expected Growth Areas
```yaml
Near Term (v0.8.x):
  - Error management UI
  - Pipeline priorities
  - Email notifications
  - Performance metrics
  
Medical Features (v0.9.x):
  - FTP Server (Service/)
  - DICOM SCP (Infrastructure/)
  - Modality Worklist (Service/)
  - HL7 Interface (New project?)
  
Scale Features (v1.x):
  - Multi-site support
  - Cloud storage
  - REST API expansion
  - Mobile monitoring
```

## 🔮 NAVIGATION WISDOM

### Quick Jump Cheat Sheet
```yaml
Need to change version?
  → Version.props

Need to add config?
  → ConfigurationPaths.cs first
  → Then Core/Configuration/

Need to fix processing?
  → FileProcessor.ProcessFileAsync()

Need to add UI?
  → Config/Pages/ or Dialogs/
  → Don't forget ViewModel!

Need to debug service?
  → 4[TAB] for console mode
  → Check Event Log

Need to find feature?
  → Search enums first
  → Then switch statements
  → Then TODO comments
```

### Architecture Boundaries
```yaml
Core ← Infrastructure:
  - Core has no dependencies
  - Infrastructure uses Core models
  
Infrastructure ← Service:
  - Service hosts Infrastructure
  - No back-references
  
Config → Service:
  - HTTP API only
  - No direct references
  
QRBridge:
  - Standalone tool
  - No dependencies on main app
```

### The Map is the Territory
```yaml
This document is:
  - Navigation tool for 30k+ LOC future
  - Feature discovery guide
  - Architecture reference
  - Growth planning tool
  
Keep it updated when:
  - Finding hidden features
  - Adding new components
  - Discovering patterns
  - Learning navigation tricks
```

---

*"The best code is the code you can find quickly"*  
*Navigate with confidence - hidden treasures await!* 🗺️🏴‍☠️
