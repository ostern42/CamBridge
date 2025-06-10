# WISDOM ARCHITECTURE - CamBridge Architektur-Dokumentation
**Letzte Aktualisierung:** 2025-06-10, 17:00  
**Von:** Claude (für meine eigene Wartbarkeit)  
**Version:** 0.7.4
**Status:** Testing Complete, Ready for Dead Letter Surgery

## 🏗️ ARCHITEKTUR-EVOLUTION

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

### Version 0.7.0-0.7.2: Die Aufräum-Phase
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
- Config Path vereinheitlicht
- DailySummaryService entfernt
- **Progress:** 2/3 Interfaces weg!

### Version 0.7.3-0.7.4: Die Foundation & Testing Phase (CURRENT)
```
┌─────────────────────────────────────┐
│         Settings Architecture        │
├─────────────────────────────────────┤
│ SystemSettings │ Pipeline │ UserPref│
├─────────────────────────────────────┤
│        ConfigurationPaths           │
│        (ProgramData ONLY!)          │
└─────────────────────────────────────┘
        ▲               ▲
        │               │
┌──────────────┐ ┌──────────────┐
│ Config Tool  │ │   Service    │
└──────────────┘ └──────────────┘
```
- 3-Layer Settings implementiert
- Foundation stabilisiert
- Legacy Support sichergestellt
- Config Path Bugs gefixt
- **Achievement:** Tested & Working!

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

## 📐 AKTUELLE ARCHITEKTUR (v0.7.4)

### Core Layer
```
CamBridge.Core/
├── Entities/
│   ├── ImageMetadata.cs      - EXIF/Meta Daten
│   ├── PatientInfo.cs        - Patient Daten
│   ├── StudyInfo.cs          - Study Daten
│   └── ProcessingResult.cs   - Ergebnis-Typ
├── Settings/                  [STABLE!]
│   ├── SystemSettings.cs      - System-wide ✅
│   ├── UserPreferences.cs     - Per-User ✅
│   └── NotificationSettings.cs- Notifications ✅
├── Interfaces/
│   ├── IDicomConverter.cs     [REMOVED] ✅
│   ├── IDicomTagMapper.cs     [NEXT TO REMOVE]
│   └── IMappingConfiguration.cs
├── ValueObjects/
│   ├── DicomTag.cs
│   ├── ExifTag.cs
│   └── PatientId.cs
└── Configuration/
    ├── ConfigurationPaths.cs  [TESTED & WORKING!] ✅
    ├── PipelineConfiguration.cs
    └── ProcessingOptions.cs   [TO UPDATE for Dead Letter]
```

### Infrastructure Layer
```
CamBridge.Infrastructure/
├── Services/
│   ├── DicomConverter.cs      - Direkt (no interface) ✅
│   ├── DicomTagMapper.cs      [TO SIMPLIFY]
│   ├── ExifToolReader.cs      - Direkt implementiert ✅
│   ├── FileProcessor.cs       - Orchestrierung
│   ├── ProcessingQueue.cs     - File Queue ✅
│   ├── DeadLetterQueue.cs     [TO REMOVE - 300+ LOC] 🎯
│   ├── PipelineManager.cs     - Pipeline Logic
│   ├── FolderWatcherService.cs- Folder Monitoring
│   └── NotificationService.cs - Email/EventLog
└── ServiceCollectionExtensions.cs
```

### Service Layer
```
CamBridge.Service/
├── Program.cs                 - Host & DI Setup
├── Worker.cs                  - Background Service
├── Controllers/
│   └── StatusController.cs    - REST API [TO UPDATE]
├── Tools/
│   └── exiftool.exe          - External Tool
└── appsettings.json          - In ProgramData! ✅
```

### Config Tool Layer
```
CamBridge.Config/
├── ViewModels/
│   ├── MainViewModel.cs
│   ├── DashboardViewModel.cs  [TESTED & WORKING!] ✅
│   ├── SettingsViewModel.cs
│   ├── DeadLettersViewModel.cs [TO SIMPLIFY] 🎯
│   └── PipelineConfigViewModel.cs [TESTED & WORKING!] ✅
├── Views/
│   ├── MainWindow.xaml
│   ├── DashboardPage.xaml     [TESTED & WORKING!] ✅
│   ├── SettingsPage.xaml
│   ├── DeadLettersPage.xaml   [TO SIMPLIFY] 🎯
│   ├── AboutPage.xaml         [v0.7.4 + Debug/Release] ✅
│   └── PipelineConfigPage.xaml [TESTED & WORKING!] ✅
└── Services/
    ├── ISettingsService.cs     [READY FOR IMPL]
    ├── ConfigurationService.cs [TESTED & WORKING!] ✅
    └── HttpApiService.cs
```

## 🔄 DATENFLUSS

### Aktueller Flow (v0.7.4) - TESTED & VERIFIED:
```
1. JPEG File → FolderWatcher
2. FolderWatcher → ProcessingQueue
3. ProcessingQueue → FileProcessor
4. FileProcessor → ExifToolReader (EXIF extract)
5. FileProcessor → DicomTagMapper (mapping)
6. FileProcessor → DicomConverter (DICOM create)
7. Success → Archive Folder
8. Failure → DeadLetterQueue [TO CHANGE TO ERROR FOLDER]
```

### Config Flow (FIXED!):
```
1. ConfigurationPaths → ProgramData ONLY! ✅
2. No more AppData fallback! ✅
3. Service & Config Tool → Same config! ✅
```

### Ziel-Flow (v0.8.0):
```
1. JPEG File → FolderWatcher
2. FolderWatcher → ProcessingQueue
3. ProcessingQueue → CamBridgeProcessor (unified)
4. Success → Archive/Output
5. Failure → Error Folder (simple!)
```

## 🏛️ ARCHITEKTUR-PRINZIPIEN

### Was wir gelernt haben:
1. **KISS > Clean Architecture**
   - Nicht jedes Pattern ist nötig
   - Direkte Implementierung oft besser
   - Weniger Abstraktionen = weniger Bugs

2. **Foundation First**
   - Settings müssen stimmen ✅
   - Config Paths müssen klar sein ✅
   - Error Handling von Anfang an
   - **Testing reveals truth!** ✅

3. **Incremental Refactoring**
   - Kleine Schritte
   - Immer lauffähig bleiben
   - User Feedback einbeziehen
   - **Test after each change!** ✅

4. **Type Safety nutzen**
   - Compiler ist dein Freund
   - Explizite Conversions OK
   - Nullable References helfen

5. **Legacy Support wichtig**
   - Alte APIs beibehalten
   - Migration ermöglichen
   - Breaking Changes vermeiden
   - **But delete old configs!** ✅

## 🎯 ARCHITEKTUR-ZIELE

### Kurzfristig (Sprint 7):
- [x] Config vereinheitlichen ✅
- [x] Settings Architecture ✅
- [x] Test & Fix Bugs ✅
- [ ] Dead Letter entfernen 🎯
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
1. **Over-Engineering** (wird behoben)
   - Zu viele Interfaces ✓ (fixing)
   - Zu viele Services ✓ (fixing)
   - Dead Letter Queue ✓ (removing next)

2. **Missing Tests**
   - Unit Tests fehlen
   - Integration Tests fehlen
   - → Nach Simplification

3. **Documentation**
   - Code Comments OK
   - API Docs fehlen
   - User Manual fehlt

### Behoben in v0.7.3-0.7.4:
- ✅ Config Path Chaos (TESTED!)
- ✅ Settings Structure
- ✅ Legacy Compatibility
- ✅ Naming Conflicts
- ✅ Pipeline Persistence
- ✅ Version Display
- ✅ Old Config Ghosts!

## 📊 METRIKEN

### Code-Volumen:
- **v0.6.0:** ~15,000 LOC
- **v0.7.2:** ~14,940 LOC (-60)
- **v0.7.3:** ~15,940 LOC (+1000 Foundation)
- **v0.7.4:** ~15,940 LOC (Bug fixes only)
- **v0.7.5 (geplant):** ~15,290 LOC (-650 Dead Letter!)
- **Ziel v0.8.0:** <12,000 LOC

### Komplexität:
- **Interfaces:** 15 → 12 → 10 (Ziel: 0-3)
- **Services:** 15+ → 12 → ? (Ziel: 5-6)
- **Abstraction Layers:** 4 → 3 (Ziel: 2)

### Build Performance:
- **Clean Build:** 16.6s
- **Incremental:** ~3s
- **Ziel:** <10s clean

### Testing Status:
- **Pipeline Persistence:** ✅ WORKS!
- **Version Display:** ✅ CORRECT!
- **Service Communication:** ✅ WORKING!
- **Config Loading:** ✅ FIXED!

## 🚀 MIGRATION STRATEGY

### Von v0.6 zu v0.7:
1. ✅ Backup existing configs
2. ✅ Install new version
3. ✅ Run migration script
4. ✅ Verify functionality
5. ✅ **Delete old AppData configs!**

### Von v0.7.4 zu v0.7.5:
1. Dead Letter Daten sichern (falls nötig)
2. Error Folder erstellen
3. Update installieren
4. Verify error handling
5. Celebrate -650 LOC!

### Breaking Changes:
- **v0.7.0:** IDicomConverter entfernt
- **v0.7.1:** IFileProcessor entfernt
- **v0.7.3:** Settings neu strukturiert (compatible!)
- **v0.7.4:** Bugs gefixt (no breaking changes!)
- **v0.7.5:** Dead Letter Queue entfernt (planned)

## 🎨 DESIGN DECISIONS

### Warum keine Interfaces?
- **Problem:** Interface für jeden Service
- **Lösung:** Nur wo Polymorphie nötig
- **Beispiel:** DicomConverter direkt statt IDicomConverter
- **Vorteil:** -50% Code, gleiche Funktion
- **Status:** Working great! ✅

### Warum Error Folder statt Queue?
- **Problem:** 500+ LOC für Error Queue
- **Lösung:** Simple Folder + .txt files
- **Vorteil:** Explorer nutzbar, einfacher
- **Trade-off:** Keine UI, aber KISS!
- **Status:** Ready to implement! 🎯

### Warum 3-Layer Settings?
- **System:** Service + Tool gemeinsam
- **Pipeline:** Multiple Konfigurationen
- **User:** UI Preferences pro User
- **Vorteil:** Klare Trennung, Multi-User ready
- **Status:** Tested & Working! ✅

### Warum nur ProgramData?
- **Problem:** Multiple config paths = confusion
- **Lösung:** Single source of truth
- **Vorteil:** No more mysteries!
- **Status:** Fixed & Verified! ✅

## 🔮 ZUKUNFTSVISION

### CamBridge v1.0 (Q3 2025):
```
┌─────────────────────────────────┐
│      CamBridge Suite 1.0        │
├─────────────────────────────────┤
│ ✓ JPEG→DICOM    │ ✓ FTP Server │
│ ✓ C-STORE SCP   │ ✓ Worklist   │
│ ✓ C-FIND SCP    │ ✓ Auto-Route │
├─────────────────────────────────┤
│    Simple, Solid, Medical       │
└─────────────────────────────────┘
```

### Aber immer mit KISS:
- Keine unnötigen Abstraktionen
- Direkte Lösungen bevorzugen
- Foundation muss stimmen
- User Experience first
- **Test everything!**

## 🏁 ARCHITEKTUR-STATUS

**Session 54 Summary:**
- Foundation implementiert ✅
- 3-Layer Settings ✅
- Legacy Support ✅
- Bugs fixed ✅
- Tested & Verified ✅
- Ready für Dead Letter Surgery! 🎯

**Next Architecture Steps:**
1. Dead Letter → Error Folder (-650 LOC!)
2. IDicomTagMapper entfernen
3. Services konsolidieren
4. Medical Features (SIMPLE!)

**Architecture Health:**
- Foundation: ████████████ 100% ✅
- Simplification: ████████░░░░ 66%
- Testing: ████████░░░░ 66%
- Documentation: ██████░░░░░░ 50%

---

*"Architecture is not about perfection, it's about purpose - and testing!"*

**CamBridge Architecture - Built, Tested, Ready to Simplify!**
© 2025 Claude's Improbably Reliable Software Solutions
