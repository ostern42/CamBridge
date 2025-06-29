# WISDOM_META.md - Intelligent Code Navigation & Architecture
**Version**: 0.8.2  
**Purpose**: Navigate 15,670+ LOC efficiently with treasure map precision  
**Updated**: Session 93 - PACS integrated, monsters documented  
**Philosophy**: "The map IS the territory (when it's accurate)"

## 🗺️ PROJECT OVERVIEW

```yaml
Total: 15,670+ LOC across 5 projects + tests
Architecture: Pipeline-isolated medical imaging system
Core Flow: JPEG → EXIF → DICOM → PACS
Major Victories: Transform system, DICOM pipeline, PACS upload

Projects by Size:
  CamBridge.Config: ~5,400 LOC (34%) - UI & Monster ViewModels
  CamBridge.Infrastructure: ~5,200 LOC (33%) - Core + PACS
  CamBridge.Core: ~3,200 LOC (20%) - Domain models  
  CamBridge.Service: ~2,100 LOC (13%) - Windows Service
  CamBridge.QRBridge: ~350 LOC (2%) - QR generator
  CamBridge.PacsTest: ~220 LOC - Test program ⭐ NEW!
```

## 🔍 FEATURE FINDER - Quick Navigation

### "I need to..." → "Look here..."

```yaml
Upload to PACS:
  → DicomStoreService.cs ⭐ NEW! (Real implementation)
  → PacsUploadQueue.cs - Per-pipeline queue with retry
  → Infrastructure/Services/
  → Integration: FileProcessor ~line 190

Test DICOM connection:
  → tests/CamBridge.PacsTest/Program.cs ⭐ NEW!
  → Working example with fo-dicom 5.2.2
  → Shows C-ECHO and C-STORE

Configure PACS:
  → PipelineConfiguration.PacsConfiguration (nested class)
  → PipelineConfigPage.xaml - Tab #3 ⭐
  → PipelineConfigViewModel - Lines ~800-900 (in 1400 line monster!)

Process images:
  → FileProcessor.ProcessFileAsync() - Entry point
  → Now with PacsUploadQueue integration!

Extract EXIF data:
  → ExifToolReader.ExtractMetadataAsync()
  → CRITICAL: Handle "RMETA:Barcode" prefix!
  → Check multiple key variants

Find hidden features:
  → Search enums first (capability lists)
  → ValueTransform.cs - 11 transform types!
  → ProcessingQueue.cs - HashSet deduplication!
  → Check switch statements

Handle Property Names:
  → ImageTechnicalData: ImageWidth NOT Width!
  → Always check exact names in source
  → 45-minute lesson from Session 87
```

## 🦖 MONSTER FILES WARNING

### The Beasts That Need Taming
```yaml
PipelineConfigViewModel.cs:
  Lines: 1400+ 😱
  Problems: Does EVERYTHING
  Contains: All tabs logic, PACS test, validation
  Refactor Priority: HIGH
  Estimated: 6-8 hours to split

MappingEditorViewModel.cs:
  Lines: 500+
  Problems: Drag/drop + transforms + browse
  Refactor Priority: MEDIUM
  
Solution: See WISDOM_DEBT.md for refactor plan!
```

## 📁 PROJECT STRUCTURE - Detailed

### CamBridge.Core (~3,200 LOC)
```yaml
Configuration/:
  ConfigurationPaths.cs ⭐⭐⭐ [SINGLE SOURCE OF TRUTH]
    - ALL paths derive from here
    - GetPrimaryConfigPath() → %ProgramData%\CamBridge\appsettings.json
    
  PipelineConfiguration.cs:
    - Now includes PacsConfiguration! ⭐ NEW
    - Nested class pattern
    
  PacsConfiguration.cs (nested): ⭐ NEW
    - All PACS settings
    - Validation via IsValid()
    
Models/:
  PatientInfo.cs - Constructor only! No object init
  ImageTechnicalData.cs - ImageWidth, not Width!
  ValueTransform.cs - 11 hidden treasures
  
Interfaces/: 
  Only 2 left! (was 12+)
```

### CamBridge.Infrastructure (~5,200 LOC)
```yaml
Services/:
  DicomStoreService.cs ⭐ NEW!
    - Real C-STORE implementation
    - StoreFileAsync() with retry
    - TestConnectionAsync() via C-ECHO
    
  PacsUploadQueue.cs ⭐ NEW!
    - Channel-based queue per pipeline
    - Retry logic with backoff
    - Integrates with FileProcessor
    
  FileProcessor.cs:
    - MODIFIED: PacsUploadQueue integration
    - Line ~190: Queue after DICOM creation
    - Constructor takes optional queue
    
  PipelineManager.cs:
    - MODIFIED: Creates PacsUploadQueue if enabled
    - Manages queue lifecycle
    
  ExifToolReader.cs:
    - ExtractMetadataAsync() - NOT ExtractDataAsync!
    - Handle RMETA: prefixes!
```

### CamBridge.Service (~2,100 LOC)
```yaml
Program.cs:
  - Port 5111 EVERYWHERE
  - API endpoints (status, pipelines)
  - Hidden: GET /api/pipelines/{id}
  
ServiceCollectionExtensions.cs:
  - MODIFIED: DicomStoreService registration ⭐
```

### CamBridge.Config (~5,400 LOC) 
```yaml
ViewModels/:
  PipelineConfigViewModel.cs 🦖 MONSTER!
    - 1400+ lines of everything
    - PACS properties ~line 800
    - TestPacsConnectionCommand ~line 900
    - Needs urgent refactoring!
    
Views/Pages/:
  PipelineConfigPage.xaml:
    - PACS Upload is Tab #3 ⭐
    - Full configuration UI
    - Test connection button
```

### tests/CamBridge.PacsTest ⭐ NEW!
```yaml
Program.cs (~220 LOC):
  - Minimal test program
  - Shows fo-dicom 5.2.2 API
  - C-ECHO and C-STORE examples
  - Saved hours in Session 91!
```

## 🔗 COMPONENT CONNECTIONS

### PACS Integration Flow ⭐ NEW
```
FileProcessor.ProcessFileAsync()
    ↓ (successful DICOM creation)
if (PacsConfiguration.Enabled)
    ↓
PacsUploadQueue.EnqueueAsync(dicomPath)
    ↓ (async processing)
DicomStoreService.StoreFileWithRetryAsync()
    ↓ (fo-dicom 5.2.2)
PACS Server (Orthanc port 4242)
```

### Dependency Injection Updates
```yaml
NEW Services:
  - DicomStoreService (singleton)
  - PacsUploadQueue (created per pipeline)
  
Modified:
  - FileProcessor (takes optional queue)
  - PipelineManager (creates queues)
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
```

### Potential Treasures
```yaml
Still Hidden?:
  - More validation rules?
  - Debug utilities in #if DEBUG?
  - Commented features?
  - Check TODO/HACK comments!
```

## 🎯 NAVIGATION SHORTCUTS

### By Problem Type
```yaml
"PACS not uploading":
  → DicomStoreService logs
  → PacsUploadQueue status
  → Check PacsConfiguration.Enabled
  → Port 4242 for Orthanc!

"Binding not working":
  → Check for null objects (Session 90!)
  → PipelineConfigViewModel ~line 400
  → Initialize all nested objects

"Can't find feature":
  → Check enums first
  → Search switch statements  
  → Look for TODO comments
  → Maybe already implemented!

"Property not found":
  → EXACT names only!
  → ImageTechnicalData.ImageWidth
  → Check actual source file
  → No guessing!
```

### By Technology
```yaml
fo-dicom 5.2.2:
  → See WISDOM_TECHNICAL_APIS.md ⭐
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
Current: 15,670 LOC of working medical software
```

### Complexity Hotspots
```yaml
URGENT:
  - PipelineConfigViewModel (1400 lines!)
  - Needs Tab-based split
  
STABLE:
  - FileProcessor (complex but works)
  - PipelineManager (good isolation)
  - DicomConverter (DICOM compliant)
  
NEW & CLEAN:
  - DicomStoreService
  - PacsUploadQueue
  - Test-first development!
```

## 🚀 QUICK REFERENCE

### Find It Fast
```yaml
Version? → Version.props
Port? → 5111 (everywhere!)
PACS Test? → Tab #3 in Pipeline Config
Monster File? → PipelineConfigViewModel
Test Program? → tests/CamBridge.PacsTest
Breaking Changes? → WISDOM_TECHNICAL_APIS.md
Refactor Plans? → WISDOM_DEBT.md
```

### Common Locations
```yaml
C:\CamBridge\            # Binaries
%ProgramData%\CamBridge\ # Configs & Logs
Port 5111                # API
Port 4242                # Orthanc DICOM
Port 8042                # Orthanc Web
```

---

**Remember**: This map evolved over 92 sessions. When lost, check enums first, then switches, then TODOs. Hidden treasures are everywhere! 🏴‍☠️

**Latest Discovery**: Test programs save hours. One artifact per file prevents errors. Monster ViewModels still work (but need love).

*"The best code is code you can find quickly!"* 🗺️
