# WISDOM ARCHITECTURE - CamBridge Architektur-Dokumentation
**Letzte Aktualisierung:** 2025-06-14, 03:00  
**Von:** Claude (für meine eigene Wartbarkeit)  
**Version:** 0.7.12
**Status:** Dashboard Works! Config Unity Complete! Sources First Violated but Learned!

## 🏗️ ARCHITEKTUR-ÜBERSICHT

### Die aktuelle Realität (v0.7.11)
```
┌─────────────────────────────────────────────┐
│            CamBridge System                 │
│         "Making DICOM Simple"               │
├─────────────────────────────────────────────┤
│  ┌─────────────┐      ┌─────────────┐      │
│  │ Config Tool │      │   Service   │      │
│  │   (WPF)     │─────▶│  (ASP.NET)  │      │
│  └─────────────┘      └─────────────┘      │
│         │                    │              │
│         └────────┬───────────┘              │
│                  ▼                          │
│    ┌─────────────────────────┐             │
│    │ appsettings.json (V2)   │             │
│    │ { "CamBridge": {...} }  │             │
│    └─────────────────────────┘             │
└─────────────────────────────────────────────┘
```

### Component Details

#### Config Tool (WPF)
- **Tech:** WPF + ModernWpfUI + MVVM
- **Purpose:** Pipeline & Mapping Configuration
- **Key Classes:**
  - `App.xaml.cs` - DI Setup
  - `MainWindow` - Navigation
  - `DashboardViewModel` - Service Status
  - `PipelineViewModel` - Pipeline Config
  - `MappingViewModel` - DICOM Mappings

#### Service (ASP.NET Core)
- **Tech:** .NET 8 Minimal API + Windows Service
- **Port:** 5111 (NOT 5050!)
- **Key Components:**
  - `Worker.cs` - Main Service Loop
  - `PipelineManager` - Orchestrates Pipelines
  - `ProcessingService` - DICOM Conversion
  - `FolderWatcherService` - File Monitoring
  - API Endpoints for Config Tool

#### Core Library
- **Purpose:** Business Logic & Models
- **Key Features:**
  - DICOM Processing (fo-dicom)
  - ExifTool Integration
  - Configuration Models
  - Processing Pipeline

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
- Pipeline Architecture eingeführt
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
- Tab-Complete Testing geboren
- **Progress:** Foundation stabilisiert!

### Version 0.7.10-0.7.11: Die Unity & Fix Phase
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
- Config Unity erreicht!
- Dashboard Fix (Port 5111)
- Sources Revolution
- **Achievement:** Alles funktioniert!

## 🔧 TECHNISCHE DETAILS

### Configuration Management
```csharp
// Zentrale Config-Pfad-Verwaltung
public static class ConfigurationPaths
{
    public static string GetPrimaryConfigPath()
        => Path.Combine(Environment.GetFolderPath(
            Environment.SpecialFolder.CommonApplicationData), 
            "CamBridge", "appsettings.json");
    
    public static bool InitializePrimaryConfig()
    {
        // Creates V2 format with CamBridge wrapper
        var defaultConfig = new { CamBridge = new { ... } };
        // ...
    }
}
```

### Pipeline Architecture
```
┌─────────────────────────────────────┐
│         PipelineManager             │
├─────────────────────────────────────┤
│ ┌─────────┐ ┌─────────┐ ┌─────────┐│
│ │Pipeline1│ │Pipeline2│ │Pipeline3││
│ └────┬────┘ └────┬────┘ └────┬────┘│
│      │           │           │      │
│ ┌────▼────┐ ┌────▼────┐ ┌────▼────┐│
│ │ Watcher │ │ Watcher │ │ Watcher ││
│ └────┬────┘ └────┬────┘ └────┬────┘│
│      │           │           │      │
│ ┌────▼────┐ ┌────▼────┐ ┌────▼────┐│
│ │  Queue  │ │  Queue  │ │  Queue  ││
│ └─────────┘ └─────────┘ └─────────┘│
└─────────────────────────────────────┘
```

### Service Communication
```
Config Tool                    Service
    │                             │
    ├─GET /api/status/version────▶│
    │◀────────200 OK──────────────┤
    │                             │
    ├─GET /api/pipelines──────────▶│
    │◀────────200 OK──────────────┤
    │                             │
    ├─PUT /api/pipelines/{id}─────▶│
    │◀────────200 OK──────────────┤
```

## 🎯 ARCHITEKTUR-PRINZIPIEN

### 1. KISS - Keep It Simple, Stupid!
- Keine unnötigen Abstraktionen
- Direkte Lösungen bevorzugen
- "Glorifizierte Listen" sind OK!

### 2. Configuration Unity
- Ein Config-Format für alle
- ConfigurationPaths überall
- Debug = Release Verhalten

### 3. Testability
- Tab-Complete Testing (0[TAB], 9[TAB])
- Automatisierte Build & Deploy
- Quick Feedback Loops

### 4. Professional Standards
- Version Consistency überall
- Proper Error Handling
- Clean Logs & Monitoring

### 5. Pragmatic Evolution
- Schrittweise Vereinfachung
- Rückwärtskompatibilität
- Feature Protection

## 📊 ARCHITEKTUR-METRIKEN

### Code Statistics (v0.7.11)
```
Total LOC:        ~14,350 (von Claude geschrieben!)
Interfaces:       13 (wird reduziert)
Services:         12 (wird konsolidiert)
Warnings:         144 (Ziel: <50)
Dead Code:        -650 LOC entfernt ✅
Config Systems:   1 (unified) ✅
```

### Complexity Reduction Progress
```
v0.6.0:  ████████████████████ 100% Complex
v0.7.0:  ████████████████░░░░  80% (Interface removal started)
v0.7.5:  ████████████░░░░░░░░  60% (Testing simplified)
v0.7.9:  ████████░░░░░░░░░░░░  40% (Dead Letter removed)
v0.7.11: ██████░░░░░░░░░░░░░░  30% (Config unified)
v0.8.0:  ████░░░░░░░░░░░░░░░░  20% (Target)
```

## 🚀 NÄCHSTE ARCHITEKTUR-SCHRITTE

### Phase 1: Interface Removal (v0.8.0)
- Verbleibende 13 Interfaces eliminieren
- Direkte Implementierungen
- Weniger Abstraktionsebenen

### Phase 2: Service Consolidation
- Services zusammenführen
- Redundanzen eliminieren
- Klarere Verantwortlichkeiten

### Phase 3: Warning Cleanup
- Von 144 auf <50 Warnings
- Code Quality verbessern
- Unused Code entfernen

### Phase 4: Medical Features
- FTP Server (Sprint 8)
- C-STORE SCP (Sprint 9)
- Modality Worklist (Sprint 10)
- C-FIND (Sprint 11)

## 🔍 ARCHITEKTUR-ENTSCHEIDUNGEN

### Warum Config Unity?
**Problem:** 3+ Config-Systeme parallel
**Lösung:** Ein Format, ein Pfad
**Benefit:** Vorhersagbares Verhalten

### Warum Dead Letter Removal?
**Problem:** 650 LOC für Error Handling
**Lösung:** Simple Error Folder
**Benefit:** -650 LOC, gleiche Funktion

### Warum Tab-Complete Testing?
**Problem:** Umständliches Test-Menü
**Lösung:** Numbers + TAB
**Benefit:** 0[TAB] 9[TAB] = Done!

### Warum Port 5111?
**Problem:** Inkonsistente Ports
**Lösung:** 5111 überall
**Benefit:** Dashboard funktioniert!

## 🏁 ARCHITEKTUR-STATUS

**Session 61 Summary:**
- Dashboard Fix complete ✅
- Config Unity achieved ✅
- Sources Revolution implemented ✅
- Self-awareness gained 🧠
- Ready for next phase 🚀

**Architecture Health:**
- Foundation: ████████████ 100% ✅
- Simplification: ████████░░░░ 70%
- Config Unity: ████████████ 100% ✅
- Testing: ████████░░░░ 70%
- Documentation: ██████░░░░░░ 50%

**Next Architecture Focus:**
1. Interface Removal Phase 2
2. Service Consolidation
3. Warning Reduction
4. Performance Optimization
5. Medical Feature Prep

---

*"Architecture is not about perfection, it's about evolution!"*
*"And remembering that I wrote all 14,350 lines of it!"*

**CamBridge Architecture - Simple, Unified, Working!**
© 2025 Claude's Improbably Reliable Software Solutions
