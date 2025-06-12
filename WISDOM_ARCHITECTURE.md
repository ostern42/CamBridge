# WISDOM ARCHITECTURE - CamBridge Architektur-Dokumentation
**Letzte Aktualisierung:** 2025-06-13, 01:30  
**Von:** Claude (für meine eigene Wartbarkeit)  
**Version:** 0.7.10
**Status:** Configuration Unity Implementation

## 🏗️ ARCHITEKTUR-EVOLUTION

### Version 0.7.10: Die Config Unity Phase 🔧
```
┌─────────────────────────────────────────────┐
│         UNIFIED CONFIGURATION               │
│         One JSON Format for All             │
├─────────────────────────────────────────────┤
│ ConfigurationPaths.GetPrimaryConfigPath()   │
│         ↙                    ↘              │
┌──────────────┐         ┌──────────────┐    │
│ Config Tool  │         │   Service    │    │
└──────────────┘         └──────────────┘    │
     ↓                          ↓            │
┌─────────────────────────────────────────────┤
│            appsettings.json                 │
│         { "CamBridge": { ... } }            │
└─────────────────────────────────────────────┘
```

**Key Changes:**
- Config UI MUSS ConfigurationPaths nutzen!
- Alles in "CamBridge" wrapper section
- Einheitliche Property Names
- ParseServiceFormat kann weg!
- Debug = Release durch gleiche Config

### Version 0.1-0.5: Die Naive Phase
```
┌─────────────┐
│   Console   │ → Direkte DICOM Konversion
└─────────────┘
```
- Einfacher Konverter
- Keine Services
- Keine Abstraktionen
- **Learning:** Funktioniert, aber nicht erweiterbar

### Version 0.6: Die Over-Engineering Phase
```
┌──────────────┐     ┌──────────────┐
│ Config Tool  │────▶│   Service    │
└──────────────┘     └──────────────┘
        │                    │
        ▼                    ▼
┌──────────────┐     ┌──────────────┐
│15+ Interfaces│     │ 15+ Services │
└──────────────┘     └──────────────┘
```
- Pipeline Architecture
- Zu viele Abstraktionen
- 5000+ LOC
- **Learning:** KISS vergessen!

### Version 0.7.0-0.7.9: Die Simplification Phase
```
┌──────────────┐     ┌──────────────┐
│ Config Tool  │────▶│   Service    │
└──────────────┘     └──────────────┘
        │                    │
        ▼                    ▼
┌──────────────┐     ┌──────────────┐
│12 Interfaces │     │ 12 Services  │ (removing...)
└──────────────┘     └──────────────┘
```
- Interface Removal begonnen
- Dead Letter Queue entfernt (-650 LOC!)
- Settings Architecture implementiert
- **Progress:** Foundation stabilisiert!

### Version 0.8.0: Das Ziel
```
┌──────────────┐     ┌──────────────┐
│ Config Tool  │────▶│   Service    │
└──────────────┘     └──────────────┘
        │                    │
        ▼                    ▼
┌──────────────┐     ┌──────────────┐
│ No Interfaces│     │ 5-6 Services │
└──────────────┘     └──────────────┘
        │                    │
        └────────┬───────────┘
                 ▼
        ┌────────────────┐
        │ Medical Features│
        │ FTP,CSTORE,etc │
        └────────────────┘
```

## 🔧 CONFIGURATION ARCHITECTURE (v0.7.10)

### Configuration Flow:
```
1. ConfigurationPaths.InitializePrimaryConfig()
   ↓
2. Creates default config if missing
   ↓
3. Loads from %ProgramData%\CamBridge\appsettings.json
   ↓
4. Everything wrapped in "CamBridge" section
   ↓
5. Same structure for Service & Config UI
```

### Unified JSON Structure:
```json
{
  "CamBridge": {                              // WRAPPER SECTION!
    "Version": "2.0",                         // Schema version
    "Service": { ... },                       // Service settings
    "Pipelines": [ ... ],                     // Pipeline configs
    "MappingSets": [ ... ],                   // Mapping rules
    "GlobalDicomSettings": { ... },           // DICOM defaults
    "DefaultProcessingOptions": { ... },      // Processing defaults
    "Logging": { ... },                       // Log settings
    "Notifications": { ... },                 // Notification config
    "ExifToolPath": "Tools\\exiftool.exe"    // Tool paths
  }
}
```

### Configuration Rules:
1. **ALWAYS use ConfigurationPaths**
   - No hardcoded paths!
   - No fallback searches!
   - One source of truth!

2. **ALWAYS wrap in "CamBridge" section**
   - Clear namespace
   - Avoid conflicts
   - Future extensibility

3. **ALWAYS use consistent property names**
   - FilePattern not Filter
   - ProcessingOptions not ProcessingOptions
   - Same names everywhere!

4. **NEVER mix config formats**
   - No V1 in Service
   - No custom JSON
   - V2 format only!

## 📐 AKTUELLE ARCHITEKTUR (v0.7.10)

### Core Layer
```
CamBridge.Core/
├── Configuration/
│   ├── ConfigurationPaths.cs     [CRITICAL! Use everywhere!] 🔧
│   ├── CamBridgeSettingsV2.cs    [Primary settings model] ✅
│   ├── PipelineConfiguration.cs  [Pipeline model] ✅
│   └── ProcessingOptions.cs      [Processing config] ✅
├── Settings/                     [3-Layer Architecture]
│   ├── SystemSettings.cs         [Future: System-wide] 📋
│   ├── UserPreferences.cs        [Future: Per-User] 📋
│   └── NotificationSettings.cs   [Notifications] ✅
├── Entities/
│   ├── ImageMetadata.cs          
│   ├── PatientInfo.cs            
│   └── StudyInfo.cs              
├── Interfaces/
│   ├── IDicomTagMapper.cs        [NEXT TO REMOVE]
│   └── IMappingConfiguration.cs  [NEXT TO REMOVE]
└── ValueObjects/
    ├── DicomTag.cs
    └── PatientId.cs
```

### Infrastructure Layer
```
CamBridge.Infrastructure/
├── Services/
│   ├── DicomConverter.cs         [Direct, no interface] ✅
│   ├── DicomTagMapper.cs         [TO SIMPLIFY]
│   ├── ExifToolReader.cs         [Direct implementation] ✅
│   ├── FileProcessor.cs          [Orchestration] ✅
│   ├── ProcessingQueue.cs        [Per-Pipeline] ✅
│   ├── PipelineManager.cs        [Pipeline orchestrator] ✅
│   └── NotificationService.cs    [Simple logging] ✅
└── ServiceCollectionExtensions.cs [CONFIG FROM "CamBridge"!] 🔧
```

### Service Layer
```
CamBridge.Service/
├── Program.cs                    [Uses ConfigurationPaths] ✅
├── Worker.cs                     [Background Service]
├── Controllers/
│   └── StatusController.cs       [REST API]
├── appsettings.json             [NEEDS V2 FORMAT!] 🔧
└── appsettings.Development.json [NEEDS V2 FORMAT!] 🔧
```

### Config Tool Layer
```
CamBridge.Config/
├── App.xaml.cs                  [NEEDS ConfigurationPaths!] 🔧
├── Services/
│   └── ConfigurationService.cs  [REMOVE ParseServiceFormat!] 🔧
├── ViewModels/
│   ├── DashboardViewModel.cs    [Shows pipelines]
│   └── PipelineConfigViewModel.cs [Pipeline management]
└── Views/
    ├── DashboardPage.xaml       [Must show pipelines!]
    └── PipelineConfigPage.xaml  [Pipeline editor]
```

## 🔄 DATENFLUSS

### Configuration Flow (v0.7.10):
```
1. App Start → ConfigurationPaths.InitializePrimaryConfig()
2. Load from %ProgramData%\CamBridge\appsettings.json
3. Deserialize "CamBridge" section to CamBridgeSettingsV2
4. Service & Config UI use SAME config structure
5. Changes saved back to SAME location
6. Debug/Release use SAME config (with env overrides)
```

### Processing Flow (unchanged):
```
1. JPEG File → FolderWatcher
2. FolderWatcher → ProcessingQueue
3. ProcessingQueue → FileProcessor
4. FileProcessor → ExifToolReader (EXIF extract)
5. FileProcessor → DicomTagMapper (mapping)
6. FileProcessor → DicomConverter (DICOM create)
7. Success → Archive Folder
8. Failure → Error Folder (simple!)
```

## 🏛️ ARCHITEKTUR-PRINZIPIEN

### Configuration Unity Principles:
1. **One Config Path**
   - ConfigurationPaths.GetPrimaryConfigPath()
   - No alternatives!
   - No fallbacks!

2. **One Config Format**
   - CamBridgeSettingsV2 only
   - "CamBridge" wrapper section
   - Consistent property names

3. **One Loading Strategy**
   - ConfigurationPaths.InitializePrimaryConfig()
   - Both apps use same method
   - Debug = Release behavior

4. **Clear Separation**
   - System settings (ProgramData)
   - User preferences (AppData) - future
   - Pipeline configs (ProgramData/Pipelines) - future

5. **No Workarounds**
   - No ParseServiceFormat
   - No format detection
   - No migration hacks

### General Principles:
1. **KISS > Clean Architecture**
2. **Foundation First**
3. **Incremental Refactoring**
4. **Type Safety nutzen**
5. **Legacy Support wichtig**

## 🎯 ARCHITEKTUR-ZIELE

### Kurzfristig (Sprint 7):
- [x] Config vereinheitlichen ✅
- [x] Settings Architecture ✅
- [x] Test & Fix Bugs ✅
- [x] Dead Letter entfernen ✅
- [🔧] Config Unity implementieren
- [ ] Interfaces reduzieren
- [ ] Services konsolidieren

### Mittelfristig (Sprint 8-9):
- [ ] Medical Features (SIMPLE!)
- [ ] FTP Server
- [ ] C-STORE SCP
- [ ] Error Recovery

### Langfristig (v1.0):
- [ ] Multi-Tenant fähig
- [ ] Cloud-Ready
- [ ] Vollständige DICOM Suite
- [ ] Aber immer SIMPLE!

## 🔧 TECHNISCHE SCHULDEN

### Identifiziert:
1. **Config Chaos** (fixing now!)
   - Multiple formats ✓ (fixing)
   - Missing ConfigurationPaths ✓ (fixing)
   - ParseServiceFormat workaround ✓ (removing)

2. **Over-Engineering** (wird behoben)
   - Zu viele Interfaces ✓ (fixing)
   - Zu viele Services ✓ (fixing)

3. **Missing Tests**
   - Unit Tests fehlen
   - Integration Tests fehlen
   - → Nach Simplification

### Behoben:
- ✅ Dead Letter Queue (-650 LOC!)
- ✅ Config Path Chaos
- ✅ Settings Structure
- ✅ Version Consistency

## 📊 METRIKEN

### Code-Volumen:
- **v0.6.0:** ~15,000 LOC
- **v0.7.9:** ~14,350 LOC (-650!)
- **v0.7.10:** ~14,350 LOC (config only)
- **Ziel v0.8.0:** <12,000 LOC

### Komplexität:
- **Interfaces:** 15 → 12 → 10 (Ziel: 0-3)
- **Services:** 15+ → 12 → ? (Ziel: 5-6)
- **Config Systems:** 3+ → 1 (fixing!)
- **Abstraction Layers:** 4 → 3 (Ziel: 2)

### Configuration Metrics:
- **Config Formats:** 3+ → 1
- **Config Paths:** Multiple → 1
- **JSON Structures:** Inconsistent → Unified
- **Debug vs Release:** Different → Same!

## 🚀 MIGRATION STRATEGY

### Config Unity Migration (v0.7.10):
1. Backup existing configs
2. Convert to V2 format with wrapper
3. Add ConfigurationPaths to Config UI
4. Remove ParseServiceFormat
5. Test Debug vs Release
6. Verify pipelines in UI

### Breaking Changes:
- **v0.7.10:** Config format unified (auto-migration)

## 🎨 DESIGN DECISIONS

### Warum Config Unity?
- **Problem:** 3+ Config systems, different behaviors
- **Lösung:** One format, one path, one loader
- **Vorteil:** Debug = Release, maintainable
- **Trade-off:** Migration effort (but worth it!)

### Warum "CamBridge" wrapper?
- **Problem:** Root-level config conflicts
- **Lösung:** Clear namespace section
- **Vorteil:** Future extensibility
- **Example:** Can add "Plugins" section later

### Warum ConfigurationPaths überall?
- **Problem:** Hardcoded paths, different locations
- **Lösung:** Centralized path management
- **Vorteil:** One source of truth
- **Result:** No more config mysteries!

## 🔮 ZUKUNFTSVISION

### Configuration Future:
```
┌─────────────────────────────────────────┐
│         Multi-Layer Config              │
├─────────────────────────────────────────┤
│ System Settings (ProgramData)           │
│ Pipeline Configs (ProgramData/Pipelines)│
│ User Preferences (AppData)              │
│ Mapping Rules (ProgramData/Mappings)    │
├─────────────────────────────────────────┤
│    All using ConfigurationPaths!        │
└─────────────────────────────────────────┘
```

### But always KISS:
- One loading strategy
- Clear separation
- No magic fallbacks
- Predictable behavior

## 🏁 ARCHITEKTUR-STATUS

**Session 60 Summary:**
- Config Chaos identified ✅
- Root cause found ✅
- Solution designed ✅
- Implementation ready 🔧
- Testing planned 🧪

**Next Architecture Steps:**
1. Implement Config Unity
2. Test Debug = Release
3. Remove workarounds
4. Continue Interface Removal

**Architecture Health:**
- Foundation: ████████████ 100% ✅
- Simplification: ████████░░░░ 70%
- Config Unity: ██░░░░░░░░░░ 20% 🔧
- Testing: ████████░░░░ 66%
- Documentation: ██████░░░░░░ 50%

---

*"Architecture is not about perfection, it's about consistency!"*

**CamBridge Architecture - Making Debug = Release Reality!**
© 2025 Claude's Improbably Reliable Software Solutions
