# WISDOM_META.md - Intelligent Code Navigation & Architecture
**Version**: 0.8.12  
**Purpose**: Navigate 15,750+ LOC efficiently with treasure map precision  
**Updated**: Session 114 - PACS integrated, Correlation IDs complete  
**Philosophy**: "The map IS the territory (when it's accurate)"

## 🗺️ PROJECT OVERVIEW

```yaml
Total: 15,750+ LOC across 5 projects + tests
Architecture: Pipeline-isolated medical imaging system
Core Flow: JPEG → EXIF → DICOM → PACS
Major Victories: Transform system, DICOM pipeline, PACS upload, Correlation IDs

Projects by Size:
  CamBridge.Config: ~5,400 LOC (34%) - UI & Monster ViewModels
  CamBridge.Infrastructure: ~5,200 LOC (33%) - Core + PACS
  CamBridge.Core: ~3,200 LOC (20%) - Domain models  
  CamBridge.Service: ~2,100 LOC (13%) - Windows Service
  CamBridge.QRBridge: ~350 LOC (2%) - QR generator
  CamBridge.PacsTest: ~220 LOC - Test program ⭐

Monster Files Alert:
  LogViewerViewModel.cs: 1543 lines! (Session 112)
  PipelineConfigViewModel.cs: 1400+ lines
  MappingEditorViewModel.cs: 1190 lines
```

## 🔍 FEATURE FINDER - Quick Navigation

### "I need to..." → "Look here..."

```yaml
Upload to PACS:
  → DicomStoreService.cs (Real implementation)
  → PacsUploadQueue.cs - Per-pipeline queue with retry
  → Check logs after "Starting C-STORE" - BLACK HOLE! (Session 114)
  → Integration: FileProcessor ~line 190

Test DICOM connection:
  → tests/CamBridge.PacsTest/Program.cs
  → Working example with fo-dicom 5.2.2
  → Shows C-ECHO and C-STORE

Configure PACS:
  → PipelineConfiguration.PacsConfiguration (nested class!)
  → PipelineConfigPage.xaml - Tab #3
  → PipelineConfigViewModel - Lines ~800-900
  → NO DeleteAfterUpload property (Session 109)

Process images:
  → FileProcessor.ProcessFileAsync() - Entry point
  → Now with PacsUploadQueue integration!
  → Uses LogContext for correlation IDs

Extract EXIF data:
  → ExifToolReader.ExtractMetadataAsync()
  → CRITICAL: Handle "RMETA:Barcode" prefix!
  → Check multiple key variants

Find hidden features:
  → Search enums first (capability lists)
  → ValueTransform.cs - 11 transform types!
  → ProcessingQueue.cs - HashSet deduplication!
  → TreeView - 95% implemented! (Session 98)
  → Check switch statements

Handle Property Names:
  → ImageTechnicalData: ImageWidth NOT Width!
  → Always check exact source file
  → 45-minute lesson from Session 87
  → Constructor vs property init patterns

Service keeps restarting:
  → Check logs for timing pattern (2-4 min = startup crash)
  → PipelineManager.cs line ~226 (output path resolution)
  → Check for empty strings vs null
  → Quick fix: Set in config
  → Code fix: IsNullOrWhiteSpace

GUI field not saving right:
  → PipelineConfigPage.xaml (Folders tab)
  → PipelineConfigViewModel.cs (find Output property)
  → Check binding: WatchSettings.OutputPath or ProcessingOptions.ArchiveFolder?
  → Test save/load cycle
  → BackupFolder property exists but NO UI!

Correlation IDs missing/wrong:
  → Check WISDOM_CORRELATIONID_PATTERNS.md first!
  → Use ONLY approved prefixes
  → TreeView expects exact format
  → Session 110: All services now have IDs
```

## 📁 PROJECT STRUCTURE - Detailed

### CamBridge.Core (~3,200 LOC)
```yaml
Configuration/:
  ConfigurationPaths.cs ⭐⭐⭐ [SINGLE SOURCE OF TRUTH]
    - ALL paths derive from here
    - GetPrimaryConfigPath() → %ProgramData%\CamBridge\appsettings.json
    
  PipelineConfiguration.cs:
    - Now includes PacsConfiguration!
    - Nested class pattern
    - ~line 300 for PacsConfiguration
    
  PacsConfiguration.cs (nested):
    - All PACS settings
    - MaxConcurrentUploads (NOT ConcurrentUploads!)
    - NO DeleteAfterUpload property
    - NO ErrorPath property
    
  CamBridgeSettingsV2.cs:
    - ServiceSettings is NESTED class!
    - LogVerbosity is ENUM not string!
    - ~line 264 for ServiceSettings
    
Models/:
  PatientInfo.cs - Constructor only! No object init
  ImageTechnicalData.cs - ImageWidth, not Width!
  ValueTransform.cs - 11 hidden treasures
  MappingRule.cs - ApplyTransform needs DateTime fix!
  CustomMappingConfiguration.cs - Was STUB, now complete!
  
Interfaces/:
  Only 2 left! (was 12+)
  IMappingConfiguration - Now properly implemented
  
Enums/ (NEW location!):
  LogVerbosity.cs - 4 levels (not in Logging namespace!)
  ProcessingStage.cs - 9 stages (separated from LogContext)
  
Logging/:
  LogContext.cs - Uses enums, doesn't define them!
```

### CamBridge.Infrastructure (~5,200 LOC)
```yaml
Services/:
  DicomStoreService.cs:
    - StoreFileAsync() with retry
    - TestConnectionAsync() via C-ECHO
    - MISSING: Success/Failure logging! (Session 114)
    
  PacsUploadQueue.cs:
    - Channel-based queue per pipeline
    - ProcessQueueAsync self-starts in constructor!
    - Dynamic correlation IDs (not static!)
    - public int QueueLength property
    
  FileProcessor.cs:
    - MODIFIED: PacsUploadQueue integration
    - Line ~190: Queue after DICOM creation
    - Uses LogContext for all logging
    - GenerateCorrelationId() ~line 640
    
  PipelineManager.cs:
    - Creates PacsUploadQueue if enabled
    - Line 189: PACS correlation ID fixed
    - Line 226: Output path bug ("" vs null)
    - Line 646: TaskCanceledException needs fix
    - Line ~170: Now uses CustomMappingConfiguration
    
  ExifToolReader.cs:
    - ExtractMetadataAsync() - NOT ExtractDataAsync!
    - Handle RMETA: prefixes!
    - Has correlation ID support now
    
  MappingConfigurationLoader.cs:
    - Loads from JSON files
    - Has correlation ID overloads
    
  DicomTagMapper.cs:
    - MapToDataset with correlation ID
    - Line 114: DateTime validation errors
    
  ProcessingQueue.cs:
    - Already perfect with correlation IDs!
```

### CamBridge.Service (~2,100 LOC)
```yaml
Program.cs:
  - Port 5111 EVERYWHERE
  - API endpoints (status, pipelines)
  - Hidden: GET /api/pipelines/{id}
  - Debug WRN messages need removal
  - Uses AddInfrastructure() not manual!
  
Worker.cs:
  - Main service loop
  - Has correlation IDs now
  
ServiceCollectionExtensions.cs:
  - AddInfrastructure() registers everything
  - Don't register manually!
```

### CamBridge.Config (~5,400 LOC) 
```yaml
ViewModels/:
  LogViewerViewModel.cs 🦖 MONSTER!
    - 1543 lines! (Session 112)
    - Tree View 95% implemented
    - CopyLineCommand implemented
    - Needs service extraction
    
  PipelineConfigViewModel.cs 🦖 MONSTER!
    - 1400+ lines of everything
    - PACS properties ~line 800
    - TestPacsConnectionCommand ~line 900
    - Needs urgent refactoring!
    
  ServiceControlViewModel.cs:
    - Uses LogVerbosity enum
    - Needs using CamBridge.Core.Enums
    
Views/Pages/:
  PipelineConfigPage.xaml:
    - PACS Upload is Tab #3
    - Output Folder confusion
    - Missing BackupFolder UI
    
  LogViewerPage.xaml:
    - DataGrid only (Session 112)
    - Tree View code exists but disabled
    
Converters/:
  ValueConverters.cs:
    - CombineStagesConverter added
    - ColorToBrushConverter added
```

### tests/CamBridge.PacsTest
```yaml
Program.cs (~220 LOC):
  - Minimal test program
  - Shows fo-dicom 5.2.2 API
  - C-ECHO and C-STORE examples
  - Saved hours in Session 91!
```

## 🔗 COMPONENT CONNECTIONS

### PACS Integration Flow (with Session 114 Black Hole!)
```
FileProcessor.ProcessFileAsync()
    ↓ (successful DICOM creation)
if (PacsConfiguration.Enabled)
    ↓
PacsUploadQueue.EnqueueAsync(dicomPath)
    ↓ (async processing)
DicomStoreService.StoreFileWithRetryAsync()
    ↓ (fo-dicom 5.2.2)
"Starting C-STORE to 127.0.0.1:4242"
    ↓
❌ BLACK HOLE - No success/failure log! (Session 114)
    ↓
PACS Server (Orthanc port 4242)
```

### Correlation ID Flow (Complete!)
```yaml
All Patterns Documented:
  Service: S{timestamp}-Service
  PipelineManager: PM{timestamp}-{action}
  Pipeline: P{timestamp}-{pipeline}
  File: F{timestamp}-{file}
  Error: PE{timestamp}-{pipeline}
  Watcher Error: WE{timestamp}-{pipeline}
  Recovery: PR{timestamp}-{pipeline}
  PACS: PM{timestamp}-PACS-{pipeline}
  Queue: PM{timestamp}-QUEUE-{pipeline}
  
NEVER INVENT NEW PREFIXES! (Session 108 lesson)
```

### Dependency Injection Chain
```yaml
Program.cs:
  - services.AddInfrastructure() ← USE THIS!
  - NOT manual registration (Session 94 bug)

ServiceCollectionExtensions.cs:
  - Registers ALL infrastructure services
  - Including DicomStoreService
  
Critical: Registration order matters!
```

## 💎 HIDDEN TREASURES FOUND

### Completed Discoveries
```yaml
Transform System (Session 74):
  - 11 types fully implemented
  - Just needed UI
  - Saved weeks!

ProcessingQueue Dedup (Session 87):
  - HashSet tracking already there
  - Prevents duplicate processing
  - Just worked!

PACS Upload (Sessions 89-92):
  - Backend in 30 minutes
  - UI in 15 minutes  
  - Real implementation via test-first

Tree View (Session 98):
  - 95% complete already!
  - Just needed activation
  - Past-Claude strikes again!

CustomMappingConfiguration (Session 113):
  - Was empty STUB
  - Now complete with Ricoh defaults
  - 8 line change in PipelineManager
```

### Potential Treasures
```yaml
Still Hidden?:
  - More validation rules?
  - Debug utilities in #if DEBUG?
  - Commented features?
  - Email notification stubs?
  - FTP server preparations?
```

## 🎯 NAVIGATION SHORTCUTS

### By Problem Type
```yaml
"PACS not uploading":
  → DicomStoreService - check for missing logs!
  → PacsUploadQueue status
  → Check PacsConfiguration.Enabled
  → Port 4242 for Orthanc!
  → Session 114: After "Starting C-STORE" = silence

"Service restart loop":
  → PipelineManager line 226
  → Check for "" vs null
  → Use IsNullOrWhiteSpace
  → Add to config as workaround

"Binding not working":
  → Check for null objects (Session 90!)
  → PipelineConfigViewModel ~line 400
  → Initialize all nested objects

"Can't find feature":
  → Check enums first
  → Search switch statements  
  → Look for TODO comments
  → 95% chance already implemented!

"Property not found":
  → EXACT names only!
  → ImageTechnicalData.ImageWidth
  → Check actual source file
  → No guessing!

"DateTime validation error":
  → MappingRule.ApplyTransform
  → Need to handle YYYYMMDDHHMMSS
  → Session 114 discovery
```

### By Technology
```yaml
fo-dicom 5.2.2:
  → See WISDOM_TECHNICAL_APIS.md
  → Check PacsTest/Program.cs
  → Breaking changes documented!

MVVM/WPF:
  → Null binding pattern
  → Monster ViewModels need splitting
  → Check WISDOM_DEBT.md

DICOM Compliance:
  → UID format (numbers only!)
  → Transfer syntax critical
  → Dataset creation matters!
  → Private tags need VR!

Correlation IDs:
  → WISDOM_CORRELATIONID_PATTERNS.md
  → NEVER invent new ones!
  → TreeView expects exact format
```

## 📊 CODE METRICS & INSIGHTS

### Growth Timeline
```yaml
Session 1-30: Interface explosion (12+)
Session 31-60: The great cleanup
Session 61: "I wrote all this!" (14,350 LOC)
Session 74: Hidden treasures found
Session 87: DICOM pipeline complete
Session 89-92: PACS in 45 minutes!
Session 96-97: LogContext infrastructure
Session 98: Tree View discovery (95% done!)
Session 99: Sources First disaster (58%)
Session 100-109: Correlation ID journey
Session 110: Sources First triumph (100%)
Session 113: CustomMappingConfiguration complete
Current: 15,750 LOC of working medical software
```

### Complexity Hotspots
```yaml
URGENT:
  - LogViewerViewModel (1543 lines!)
  - PipelineConfigViewModel (1400 lines!)
  - Need Tab/Service based split
  
STABLE:
  - FileProcessor (complex but works)
  - PipelineManager (good isolation)
  - DicomConverter (DICOM compliant)
  
NEW & CLEAN:
  - DicomStoreService
  - PacsUploadQueue
  - CustomMappingConfiguration
  - Test-first development!
```

### Build Warning Status
```yaml
Total: ~140 warnings (unchanged)
Types:
  - Nullable reference warnings (majority)
  - Unused variables
  - Async without await
  - © encoding issues

Debug Logs in Production:
  - [WRN] [DEBUG ConfigureOptions]
  - [WRN] [DEBUG Final Check]
  - Need #if DEBUG wrapper
```

## 🚀 QUICK REFERENCE

### Find It Fast
```yaml
Version? → Version.props (0.8.12)
Port? → 5111 (everywhere!)
PACS Port? → 4242 (Orthanc)
PACS Test? → Tab #3 in Pipeline Config
Monster Files? → LogViewer (1543), PipelineConfig (1400+)
Test Program? → tests/CamBridge.PacsTest
Breaking Changes? → WISDOM_TECHNICAL_APIS.md
Refactor Plans? → WISDOM_DEBT.md
Correlation Patterns? → WISDOM_CORRELATIONID_PATTERNS.md
```

### Common Locations
```yaml
C:\CamBridge\            # Binaries
%ProgramData%\CamBridge\ # Configs & Logs
Port 5111                # API
Port 4242                # Orthanc DICOM
Port 8042                # Orthanc Web
```

### Emergency Fixes
```yaml
Service crashing → Add OutputPath to config
Restart loops → Check initialization code
Empty logs → Service not processing files
PACS silent → Check DicomStoreService logs
Missing IDs → Check correlation patterns
DateTime errors → MappingRule transforms
```

## 🗺️ SESSION DELTAS INTEGRATED

### Session 97: Enum Migration
- LogVerbosity moved to Core.Enums
- ProcessingStage separated from LogContext
- ViewModels need using directives

### Session 107: Service Restart Discovery
- Empty string vs null trap
- Config workaround pattern
- Quick fix documentation

### Session 110: Correlation Completion
- All services have IDs
- Dynamic vs static patterns
- 100% coverage achieved

### Session 113: Mapping Fix
- CustomMappingConfiguration complete
- PipelineManager simplified
- Ricoh defaults working

### Session 114: PACS Black Hole
- Upload starts but no completion log
- DicomStoreService needs investigation
- Critical for production monitoring

---

**Remember**: This map evolved over 114 sessions. When lost, check enums first, then switches, then TODOs. Hidden treasures are EVERYWHERE! 

**Latest Discovery**: PACS upload logging incomplete - the next treasure to fix!

*"The best code is code you can find quickly... and debug when it's silent!"* 🗺️
