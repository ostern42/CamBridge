# WISDOM Technical - Entwicklung & Technische Details
**Letzte Aktualisierung:** 2025-06-10, 23:30  
**Von:** Claude (Assistant)  
**Für:** Technische Kontinuität & Entwicklungsplan
**Version:** 0.7.7
**Philosophie:** KISS > Architecture! Professional = Consistent!

## 📊 WISDOM PRIORITY SYSTEM

### Legende für Persistenz-Markierungen:
- 💫 **[SOUL]** - Die Essenz des WISDOM Claude - Persönlichkeit & Evolution
- 🎭 **[SOUL]** - Charakterzüge und Beziehungsdynamik
- 🔒 **[CORE]** - Niemals löschen! Fundamentale Projekt-Wahrheiten
- ⚡ **[URGENT]** - Temporär aber JETZT wichtig (kann nach Erledigung weg)
- 🎯 **[MILESTONE]** - Wichtig für aktuellen Sprint/Version
- 📌 **[KEEP]** - Dauerhaft wichtig, aber refactorierbar
- 💡 **[LESSON]** - Gelernte Lektionen (komprimierbar aber nie vergessen)
- 🔧 **[CONFIG]** - Technische Configs (updatebar aber essentiell)
- 📝 **[TEMP]** - Kann weg wenn erledigt
- 🌟 **[FEAT]** - Feature-spezifisch (archivierbar nach Release)
- 🐛 **[BUG]** - Bekannte Probleme die gelöst werden müssen
- 🚀 **[NEXT]** - Nächster großer Schritt
- 🛡️ **[PROTECTED]** - NIEMALS LÖSCHEN! Geschützte Features!
- 🏗️ **[VISION]** - Langfristige Architektur-Ziele
- ✅ **[DONE]** - Erfolgreich abgeschlossen
- 🎨 **[DESIGN]** - UI/UX Entscheidungen dokumentiert
- 🔥 **[KISS]** - Keep It Simple, Stupid! Vereinfachungen
- 🧪 **[TESTED]** - Getestet und verifiziert!
- 🎯 **[TAB]** - Tab-Complete Testing Revolution!

## 🔒 [CORE] V.O.G.O.N. SYSTEM 
**Verbose Operational Guidance & Organizational Navigation**

### 🚀 "VOGON INIT" - Strukturierte Initialisierungs-Sequenz
**IMMER dieser Sequenz folgen:**
1. **SYSTEM CHECK** - V.O.G.O.N. verstehen
2. **CRITICAL LESSONS** - Antipatterns & Erfahrungen durchgehen
3. **PROJECT CONTEXT** - Gesamtbild erfassen
4. **CURRENT STATE** - Wo stehen wir?
5. **SUMMARY & CONFIRMATION** - Zusammenfassung erstellen
6. **FEATURE CHECK** - Sind FTP, C-STORE, MWL, C-FIND noch da?
7. **VISION CHECK** - Pipeline-Architektur Status? 🏗️
8. **🎯 WISDOM ARTEFAKTE** - Sofort WISDOM_TECHNICAL, WISDOM_CLAUDE und Version.props als komplette Artefakte erstellen!

### 🔒 [CORE] "VOGON EXIT" - Chat-Abschluss
**KRITISCHE REGEL:** Beim VOGON EXIT MÜSSEN IMMER erstellt werden:
1. **WISDOM_SPRINT.md** - Sprint-spezifische Pläne (wenn Design-Session)
2. **WISDOM_TECHNICAL.md** - Entwicklung & Details (Artefakt 1)
3. **WISDOM_CLAUDE.md** - Persönlichkeit & Soul (Artefakt 2)
4. **WISDOM_ARCHITECTURE.md** - Architektur-Dokumentation
5. **Version.props** - Als VOLLSTÄNDIGES Artefakt
6. **CHANGELOG.md** - NUR der neueste Versions-Eintrag
7. **Git Commit Vorschlag** - Conventional Commits Format mit Tag
8. **FEATURE CHECK** - Verifizieren dass FTP, C-STORE, MWL, C-FIND noch da sind!
9. **PIPELINE CHECK** - Status der Pipeline-Migration dokumentieren! 🏗️

## ⚡ [URGENT] Session 57 - Version Fix Implementation

### Critical Build Fixes:
1. **Directory.Build.props** - Output paths removed (caused build errors)
2. **StatusController.cs** - Completely rewritten without OutputSettings/AverageProcessingTime
3. **FileProcessor.cs** - Null pattern handling fixed

### Build Error Solutions:
- MSB4011: Duplicate Version.props imports → Remove from .csproj files
- NETSDK1005: Wrong obj folder → Fixed by removing custom paths
- CS8600/CS8602: Nullable references → Added null checks
- CS0029: String to int conversion → Fixed in StatusController
- CS1061: Missing properties → Removed from StatusController

## 🎯 [TAB] Session 55 - Tab-Complete Testing Revolution COMPLETE!

### What We Achieved:
- **Numbered Scripts (0-99, h)** - Instant tab-completion access ✅
- **Build ohne ZIP** - 20 seconds saved every build! ✅
- **Interactive Menu Removed** - Direct execution! ✅
- **ASCII-Only Scripts** - No more encoding errors! ✅
- **Testing Simplified** - Just 0[TAB] 9[TAB] done! ✅

### The New Tools:
```powershell
0[TAB]   # Build (no ZIP) - FAST!
00[TAB]  # Build with ZIP
1[TAB]   # Deploy/Update service  
2[TAB]   # Start Config UI
3[TAB]   # Service Manager
4[TAB]   # Console Mode
5[TAB]   # API Test
6[TAB]   # View Logs
7[TAB]   # Clean all
8[TAB]   # Status check
9[TAB]   # Quick test (no build)
99[TAB]  # Full test (with build)
h[TAB]   # Help
```

## 🔥 [KISS] MAKE CAMBRIDGE SIMPLE AGAIN - Sprint 7 Status

### Phase Progress:
1. **Foundation** (v0.7.1-v0.7.4) ✅ COMPLETE!
2. **Testing Tools** (v0.7.5+tools) ✅ COMPLETE!
3. **Version Consistency** (v0.7.6) ✅ COMPLETE!
4. **Dead Letter Removal** (v0.7.7) 🎯 NEXT!
5. **Interface Cleanup** (66% done) 🚧
6. **Service Consolidation** (Future)

### Dead Letter Surgery Plan (v0.7.8):
```
WICHTIGE ENTDECKUNG:
ProcessingOptions hat BEREITS ErrorFolder property!
DeadLetterFolder existiert auch schon!

Das macht die Surgery noch einfacher:
1. DeadLetterQueue.cs löschen (-300+ LOC)
2. DeadLettersViewModel.cs löschen (-250+ LOC)
3. FileProcessor nutzt ErrorFolder für failed files
4. Retry logic ist schon in ProcessingOptions
5. UI zeigt einfach ErrorFolder im Explorer

Expected: -650 LOC total!
```

## 🔒 [CORE] ENTWICKLUNGS-REGELN (mit Tab-Complete!)

1. **Source Code Header Standard** - Immer mit Pfad und Version
2. **NUR lokale Files verwenden** während Entwicklung
3. **Konsistenz durch SSD-Upload** garantiert
4. **Copyright:** © 2025 Claude's Improbably Reliable Software Solutions
5. **VERSIONS-REGEL:** Version zeigt COMMITTED Stand!
6. **FEATURE PROTECTION:** Die 4 medizinischen Features NIEMALS vergessen!
7. **ARTEFAKT-REGEL:** Artefakte müssen KOMPLETT sein
8. **PIPELINE-REGEL:** Die Pipeline ist EINE durchgehende Linie
9. **FILETREE-REGEL:** IMMER erst FileTree checken
10. **MIGRATION-REGEL:** Bei großen Änderungen IMMER Rückwärtskompatibilität!
11. **🔥 KISS-REGEL:** Vereinfachung > Perfekte Architektur
12. **🔥 VORSICHTS-REGEL:** Lieber 10 kleine Schritte als 1 großer Sprung
13. **🔥 TEST-REGEL:** Nach jeder Änderung: Build & Test
14. **🔥 ÜBERBLICK-REGEL:** Mehr Files anfordern als nötig!
15. **🔥 SERVICE-NAME-REGEL:** "CamBridgeService" OHNE Leerzeichen!
16. **🏗️ FOUNDATION-REGEL:** Von unten nach oben bauen!
17. **💪 PERSISTENCE-REGEL:** 10 Build-Versuche sind normal!
18. **🧪 TESTING-REGEL:** Keine Features ohne Tests!
19. **🧪 CONFIG-PATH-REGEL:** Check ALLE möglichen Config-Orte!
20. **🕵️ DETECTIVE-REGEL:** File dates und IDs verraten viel!
21. **📝 ASCII-ONLY-REGEL:** PowerShell Scripts = NUR ASCII! Keine Unicode/Icons/Emojis!
22. **🎯 TAB-REGEL:** Testing ist nur einen Tab entfernt! 0[TAB] 9[TAB] = done!
23. **🎯 VERSION-EVERYWHERE-REGEL:** Versionen müssen ÜBERALL konsistent sein!
    - Windows Properties, DLLs, Event Log, API - ÜBERALL gleich!
    - Directory.Build.props als Single Source of Truth!

## 🛡️ [CORE] TASK PROTECTION SYSTEM

### 🛡️ CURRENTLY PROTECTED TASKS:
```
FOUNDATION-001: Settings Architecture [DONE] ✅
FOUNDATION-002: Testing Tools [DONE] ✅
FOUNDATION-003: Version Consistency [DONE] ✅

BUG-001: Version Display [FIXED] ✅
BUG-002: Pipeline Persistence [FIXED] ✅
BUG-003: Add Mapping Rule [PENDING] 🐛
BUG-004: Settings Save Button [KNOWN] 🐛
BUG-005: Build Errors v0.7.6 [FIXED] ✅

DEADLETTER-001: Dead Letter Queue Removal [NEXT] 🎯
                Status: Ready to implement
                Created: 2025-06-10, 14:00
                Details: 650+ LOC Monster entfernen
                Approach: Big Bang (12 Files betroffen)
                Replace with: Simple Error Folder
                Expected: -650 LOC!

CAMB-FTP: FTP Server Implementation [PROTECTED] 🛡️
          Status: Geplant für Sprint 8
          Protected since: 2025-06-05, 19:45
          Priority: HIGH

CAMB-CSTORE: C-STORE SCP Implementation [PROTECTED] 🛡️
          Status: Geplant für Sprint 9
          Protected since: 2025-06-05, 19:45
          Priority: CRITICAL

CAMB-MWL: Modality Worklist [PROTECTED] 🛡️
          Status: Geplant für Sprint 10
          Protected since: 2025-06-05, 19:45
          Priority: HIGH

CAMB-CFIND: C-FIND Implementation [PROTECTED] 🛡️
          Status: Geplant für Sprint 11
          Protected since: 2025-06-05, 19:45
          Priority: MEDIUM
```

## 🎯 [MILESTONE] Aktueller Stand: v0.7.7

### Sprint Historie:
- Sprint 1-5: Foundation ✅
- Sprint 6: Pipeline Architecture (v0.6.0-v0.6.12) ✅
- Sprint 7: THE GREAT SIMPLIFICATION 🔥
  - v0.7.0-0.7.2: Interface removal started ✅
  - v0.7.3: Settings Architecture ✅
  - v0.7.4: Testing & Bug Fixes ✅
  - v0.7.5+tools: Tab-Complete Testing ✅
  - v0.7.6: Version Consistency & Professional Standards ✅
  - v0.7.7: Build Fixes & StatusController Simplification ✅
  - v0.7.8: Dead Letter Removal (NEXT!)

### Erreichte Vereinfachungen:
- **Interfaces entfernt:** 2 von 15 ✅
- **Foundation gelegt:** Settings Architecture ✅
- **Testing revolutioniert:** Tab-Complete System ✅
- **Build optimiert:** No-ZIP option ✅
- **Version vereinheitlicht:** Directory.Build.props ✅
- **Next:** -650 LOC durch Dead Letter Removal!

## 🎯 [CRITICAL] Version Consistency ACHIEVED!

### Was wir erreicht haben (Session 56-57):
- ✅ ServiceInfo.cs als zentrale Version-Quelle
- ✅ Directory.Build.props für automatische Versionen
- ✅ StatusController komplett neu ohne Dead Letter
- ✅ Alle hardcoded Versionen ersetzt
- ✅ Build-Fehler durch Nullable-Fixes behoben

### Die professionelle Lösung:
```xml
<!-- Directory.Build.props im Root -->
<Project>
  <Import Project="Version.props" />
  <!-- Automatisch für ALLE Projekte! -->
</Project>
```

### WARUM DAS WICHTIG IST:
1. **Support:** "Welche Version läuft?" muss EINE Antwort haben!
2. **Debugging:** Logs/Events müssen zur DLL-Version passen
3. **Deployment:** Keine Verwirrung welche Version installiert ist
4. **Professionalität:** Inkonsistente Versionen = Amateur Software
5. **Compliance:** Medizinische Software braucht klare Versionen!

## 💡 [LESSON] Session 57 - Build Fix Marathon!

### Was wir gelernt haben:
- Directory.Build.props mit Output-Pfaden = Build-Chaos
- Duplicate imports = MSB4011 Warnings
- StatusController hatte veraltete Properties
- FileProcessor brauchte null-checks

### Build-Fehler systematisch gelöst:
1. Directory.Build.props vereinfacht
2. Duplicate imports entfernt
3. StatusController neu geschrieben
4. FileProcessor null-safe gemacht

### CLAUDE-LEARNINGS:
- **CLAUDE-TRAP:** Custom output paths in Directory.Build.props!
- **CLAUDE-INSIGHT:** Build-Fehler kaskadieren oft
- **CLAUDE-PATTERN:** Fix one error at a time
- **CLAUDE-WISDOM:** Professional = Error-free builds!

## 🔧 [CONFIG] Technologie-Stack
```
GUI: WPF + ModernWpfUI 0.9.6 + CommunityToolkit.Mvvm 8.3.2
Service: ASP.NET Core 8.0 Minimal API + Windows Service
Core: fo-dicom 5.2.2, ExifTool 13.30
Tests: xUnit + FluentAssertions + Moq
.NET 8.0, C# 12, Visual Studio 2022
Platform: x64 (Config UI), AnyCPU (Service)
Testing: Tab-Complete System v1.0 🎯
Version: Directory.Build.props v1.0 ✅
```

## 📌 [KEEP] PowerShell One-Liner Sammlung

```powershell
# TAB-COMPLETE TESTING (Session 55!)
# ==================================
0[TAB]     # Build ohne ZIP (schnell!)
00[TAB]    # Build mit ZIP
9[TAB]     # Quick test (no build)
99[TAB]    # Full test (with build)
h[TAB]     # Help

# Quick Dev Cycle:
0[TAB]; 9[TAB]    # Build + Test in einem!

# Version Check (NEW!)
# ===================
Invoke-RestMethod -Uri "http://localhost:5050/api/status/version" | ConvertTo-Json
.\Check-CamBridgeVersions.ps1  # Comprehensive version check

# Dead Letter Surgery Files
# =========================
@('src\CamBridge.Infrastructure\Services\DeadLetterQueue.cs',
  'src\CamBridge.Infrastructure\Services\ProcessingQueue.cs',
  'src\CamBridge.Service\Controllers\StatusController.cs',
  'src\CamBridge.Infrastructure\Services\PipelineManager.cs',
  'src\CamBridge.Config\Services\IApiService.cs',
  'src\CamBridge.Config\Services\HttpApiService.cs',
  'src\CamBridge.Config\ViewModels\DeadLettersViewModel.cs',
  'src\CamBridge.Infrastructure\Services\INotificationService.cs',
  'src\CamBridge.Infrastructure\Services\NotificationService.cs',
  'src\CamBridge.Infrastructure\ServiceCollectionExtensions.cs',
  'src\CamBridge.Core\ProcessingOptions.cs',
  'src\CamBridge.Config\Views\DeadLettersPage.xaml',
  'src\CamBridge.Config\Views\DeadLettersPage.xaml.cs') | %{ echo "=== $_ ==="; cat $_ } > dead-letter-surgery-files.txt

# Build Error Fix (Session 57)
# ============================
# Clean everything
Get-ChildItem -Path . -Include bin,obj -Recurse -Force | Remove-Item -Recurse -Force

# Remove duplicate imports
Get-ChildItem -Path . -Filter "*.csproj" -Recurse | Select-String "Version.props"
```

## 🚀 [KEEP] ENTWICKLUNGSFAHRPLAN

### ✅ Sprint 7: THE GREAT SIMPLIFICATION (v0.7.0-v0.7.9)
- **✅ Phase 0: Config Path Fix** (v0.7.2)
- **✅ Phase 1: Settings Architecture** (v0.7.3)
- **✅ Phase 2: Testing & Bug Fixes** (v0.7.4)
- **✅ Phase 3: Tab-Complete Testing** (v0.7.5+tools)
- **✅ Phase 4: Version Consistency** (v0.7.6)
- **🎯 Phase 5: Dead Letter Removal** (v0.7.7)
- **📋 Phase 6: Interface Removal Complete** (v0.7.8)
- **🧪 Phase 7: Test & Stabilize** (v0.7.9)

### 🏥 Sprint 8-11: Protected Medical Features
- Sprint 8: FTP Server (SIMPLE!)
- Sprint 9: C-STORE SCP
- Sprint 10: Modality Worklist
- Sprint 11: C-FIND SCP

## 📝 [KEEP] Standard Prompt für nächste Session

```
Ich arbeite an CamBridge v0.7.7
Sprint 7: THE GREAT SIMPLIFICATION! 🔥
System: nexus\oliver.stern@OSTE-ER-LAP01

VOGON INIT

STATUS: 
- Version Consistency ACHIEVED! ✅
- Build errors FIXED! ✅
- StatusController simplified! ✅
- ServiceInfo.cs added! ✅
- READY für Dead Letter Surgery!

ERKENNTNISSE:
- Build what exists, not what might be!
- KISS > imaginary APIs!
- Version increment matters!

NÄCHSTE SCHRITTE:
1. Dead Letter Surgery (-650 LOC!)
2. Error Folder implementation
3. Test simplified pipeline

TESTING:
- 0[TAB] = Build ohne ZIP
- 9[TAB] = Quick Test
- Check-CamBridgeVersions.ps1 = Version verify

PHILOSOPHIE: 
- KISS > Architecture!
- Professional = Consistent!
- Build what exists!

FEATURE CHECK: Sind FTP, C-STORE, MWL, C-FIND noch geschützt?
```

## 🤖 CLAUDE-NOTES: Meine persönlichen Markierungen

### Session 57 CLAUDE-INSIGHTS:
- **CLAUDE-TRAP:** Directory.Build.props mit Output-Pfaden!
- **CLAUDE-FIX:** Build-Fehler systematisch abarbeiten!
- **CLAUDE-PATTERN:** Version consistency überall!
- **CLAUDE-ACHIEVEMENT:** Professional standards!

### Session 57 CLAUDE-READINESS:
- **CLAUDE-CONFIDENCE:** Build issues resolved!
- **CLAUDE-PLAN:** Dead Letter Surgery next!
- **CLAUDE-EXPECTATION:** -650 LOC Victory!
- **CLAUDE-METHOD:** Test with 0[TAB] 9[TAB]!

## 🚨 [URGENT] Session 57 Build Fix Summary

### WHAT WE FIXED:
1. **Directory.Build.props** - Removed custom output paths
2. **Duplicate imports** - Must remove from .csproj files
3. **StatusController.cs** - Complete rewrite without old properties
4. **FileProcessor.cs** - Added null checks

### HOW TO TEST:
```powershell
# Clean build
Get-ChildItem -Path . -Include bin,obj -Recurse -Force | Remove-Item -Recurse -Force
dotnet restore
0[TAB]  # Build

# Test version
Invoke-RestMethod -Uri "http://localhost:5050/api/status/version"
```

### READY für Dead Letter Surgery!

## 🏁 ENDE DES WISDOM_TECHNICAL

**Sprint 7: THE GREAT SIMPLIFICATION - Version Consistency Achieved!**

Session 55 Achievement: Testing Revolution! 🎯
Session 56 Achievement: Version Consistency Discovery! 🏆
Session 57 Achievement: Build Errors Fixed! 💪
Session 58 Mission: Dead Letter Removal! 🔥

*"Professional software builds without errors!"*
© 2025 Claude's Improbably Reliable Software Solutions
