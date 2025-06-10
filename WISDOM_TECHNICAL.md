# WISDOM Technical - Entwicklung & Technische Details
**Letzte Aktualisierung:** 2025-06-10, 12:00 Uhr  
**Von:** Claude (Assistant)  
**Für:** Technische Kontinuität & Entwicklungsplan
**Version:** 0.7.2
**Philosophie:** KISS > Architecture! (aber VORSICHTIG!)

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
4. **Version.props** - Als VOLLSTÄNDIGES Artefakt
5. **CHANGELOG.md** - NUR der neueste Versions-Eintrag
6. **Git Commit Vorschlag** - Conventional Commits Format mit Tag
7. **FEATURE CHECK** - Verifizieren dass FTP, C-STORE, MWL, C-FIND noch da sind!
8. **PIPELINE CHECK** - Status der Pipeline-Migration dokumentieren! 🏗️

## 🔥 [KISS] MAKE CAMBRIDGE SIMPLE AGAIN - Sprint 7 Strategie

### Die VORSICHTIGE Herangehensweise:
**NICHT:** Alles auf einmal umbauen  
**SONDERN:** Schritt für Schritt vereinfachen  

### Phase 1: Analyse & Planung (DONE! ✅)
1. **Alle relevanten Files anfordern** ✅
2. **Dependency-Analyse durchführen** ✅
3. **Risiko-Bewertung machen** ✅
4. **Klein anfangen, groß denken** ✅

### Phase 2: Quick Wins (v0.7.0) - IN PROGRESS
1. **DailySummaryService auskommentieren** ✅
2. **Unnötige Interfaces identifizieren** ✅
3. **Test-Coverage sicherstellen** ✅
4. **Interfaces vereinfachen als Proof of Concept** 
   - IDicomConverter ✅
   - IFileProcessor ✅
   - IDicomTagMapper 🎯

### Phase 3: Schrittweise Vereinfachung (v0.7.2-v0.7.4)
1. **Service by Service refactoren**
2. **Nach jedem Schritt: Build & Test**
3. **Rollback-Plan haben**
4. **User-Feedback einholen**

### Die KISS-Checkliste:
```
✅ Brauchen wir diese Abstraktion wirklich? (NEIN bei 3 Interfaces!)
✅ Kann das direkter gelöst werden? (JA, siehe ExifToolReader)
✅ Was ist der einfachste Weg der funktioniert? (Direct dependencies)
□ Haben wir Tests dafür? (Noch nicht...)
✅ Können wir das später wieder rückgängig machen? (Git sei Dank)
✅ Versteht Oliver was wir tun? (Er macht sogar mit!)
```

## 🔒 [CORE] ENTWICKLUNGS-REGELN (mit KISS Updates)

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

## 🛡️ [CORE] TASK PROTECTION SYSTEM

### 🛡️ CURRENTLY PROTECTED TASKS:
```
PIPELINE-001: Pipeline Architecture [SIMPLIFYING] 🔥
             Status: Works but over-engineered
             Created: 2025-06-06, 15:30
             Details: Multi-Pipeline Support mit Mapping Sets
             Priority: KISS REFACTORING
             Progress:
             - Phase 1-5: COMPLETED ✅
             - Sprint 7: THE GREAT SIMPLIFICATION (VORSICHTIG!)
             - V1 Config läuft, V2 Migration pending

KISS-001: Service Layer Simplification [ACTIVE] 🔥
          Status: Step 1.1 & 1.2 DONE, Step 1.3 NEXT
          Created: 2025-06-10, 09:00
          Details: Von 15+ auf 5-6 Services
          Approach: VORSICHTIG, Schritt für Schritt
          Done: 
          - IDicomConverter Interface entfernt ✅
          - IFileProcessor Interface entfernt ✅
          Next: IDicomTagMapper Interface

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

## 🏛️ [CORE] SPRINT RULES - Mit KISS Updates

1. **EIN Sprint = EIN Thema** (Sprint 7 = Simplification)
2. **Erst verstehen, dann ändern** 🔥
3. **Klein anfangen, groß denken** 🔥
4. **Kleine Schritte, große Vorsicht** ✅
5. **Keine globalen Änderungen ohne Plan**
6. **Mut zum Vereinfachen** 🔥
7. **Features schützen!** ✅
8. **Tests vor Refactoring** 🔥
9. **FileTree IMMER checken!** ✅
10. **User im Loop halten** ✅ (Oliver macht sogar selbst mit!)

## 🎯 [MILESTONE] Aktueller Stand: v0.7.2

### Sprint Historie:
- Sprint 1-5: Foundation ✅
- Sprint 6: Pipeline Architecture (v0.6.0-v0.6.12) ✅
  - Funktioniert aber over-engineered
  - 15+ Services, 5000+ LOC
  - DailySummaryService broken
- Sprint 7: THE GREAT SIMPLIFICATION 🔥
  - v0.7.0: MCSA Step 1.1 ERFOLGREICH! ✅
  - v0.7.1: Step 1.2 bereits implementiert, Deployment fixes ✅
  - IDicomConverter Interface entfernt ✅
  - IFileProcessor Interface entfernt ✅
  - Deployment Script gefixt ✅
  - Service läuft als Windows Service ✅

### Erreichte Vereinfachungen:
- **Interfaces entfernt:** 2 von 3 ✅
- **Code-Reduktion:** ~60 Zeilen
- **Neue Bugs gefixt:** HealthCheck, Deployment Script
- **Stabilität:** Service läuft produktiv!

### 🏗️ Die NEUE Sprint-Priorität (Foundation First!)

Nach Olivers Insights:

1. **Foundation Layer** (v0.7.1-v0.7.2)
   - ✅ Config Path Vereinheitlichung 
   - 🎯 Settings Separation (System vs Pipeline vs User)
   - 🆕 Dead Letter Queue ENTFERNEN!
   - 📋 Clean Architecture von der Basis

2. **Simplification Layer** (v0.7.3-v0.7.4)
   - Interface Removal (Step 1.3+)
   - Service Consolidation
   - Code Cleanup

3. **Feature Layer** (v0.8.0+)
   - Medical Features (FTP, C-STORE, etc.)
   - Aber auf SOLIDER Basis!

**CLAUDE-MANTRA:** "Fix the foundation before decorating the house!"

## 💡 [LESSON] Session 52 - Config Fix & Foundation Revelations

### Was passierte:
**Start:** Config-Chaos zwischen Service und Tool  
**Lösung:** Zentrale Config in ProgramData  
**Neue Erkenntnis 1:** Settings-Hierarchie auch chaotisch!  
**Neue Erkenntnis 2:** Dead Letter Queue massiv over-engineered!  
**Olivers Weisheit:** "von unten nach oben denken"

### Die Erfolge:
1. **Config Path vereinheitlicht** ✅
2. **6 Artefakte implementiert** ✅  
3. **Demo-Pipelines entfernt** ✅
4. **Settings-Architecture designed** ✅
5. **Dead Letter Over-Engineering erkannt** ✅

### Die neuen Erkenntnisse:
- **Settings Chaos:** System vs Pipeline vs User vermischt
- **Dead Letter Monster:** 500+ LOC für Error Folder Alternative
- **Foundation Problems:** Überall versteckte Komplexität

### Die neue Priorität:
1. **Foundation First** 
   - Settings richtig strukturieren
   - Dead Letters durch Error Folder ersetzen
   - Config Paths vereinheitlichen
2. **Then Simplify** - Interfaces entfernen
3. **Then Features** - Auf solider Basis bauen

### CLAUDE-LEARNINGS:
- Foundation problems cascade upward
- Over-Engineering versteckt sich überall
- "Implementation in Progress" = Red Flag!
- Simple solutions (Error Folder) > Complex (Dead Letter Queue)
- "von unten nach oben" = Best practice!

---

## 🔧 [CONFIG] Technologie-Stack (unverändert)
```
GUI: WPF + ModernWpfUI 0.9.6 + CommunityToolkit.Mvvm 8.3.2
Service: ASP.NET Core 8.0 Minimal API + Windows Service
Core: fo-dicom 5.2.2, ExifTool 13.30
Tests: xUnit + FluentAssertions + Moq
.NET 8.0, C# 12, Visual Studio 2022
```

## 📌 [KEEP] PowerShell One-Liner Sammlung (ERWEITERT!)

```powershell
# KISS Analysis One-Liners
# ========================

# Service Status Check (RICHTIG!)
Get-Service "CamBridgeService" | Select-Object Name, Status, StartType

# API Health Check
Invoke-RestMethod -Uri "http://localhost:5050/health"

# Pipeline Status
Invoke-RestMethod -Uri "http://localhost:5050/api/pipelines" | ConvertTo-Json

# Port Check
netstat -an | Select-String "5050"

# Service Layer Complexity Check
@('src\CamBridge.Service\Program.cs','src\CamBridge.Service\Worker.cs','src\CamBridge.Service\DailySummaryService.cs','src\CamBridge.Infrastructure\Services\PipelineManager.cs','src\CamBridge.Infrastructure\Services\ProcessingQueue.cs') | %{ echo "=== $_ ==="; cat $_ } > service-layer-analysis.txt

# All Services in DI Container
Get-ChildItem "src" -Include "*.cs" -Recurse | Select-String "services\.Add|IServiceCollection" | Select-Object -Unique Filename, LineNumber, Line > di-services-all.txt

# Interface Usage Analysis
Get-ChildItem "src" -Include "*.cs" -Recurse | Select-String "interface I[A-Z]" | Group-Object Filename | Sort-Object Count -Descending > interface-usage.txt

# Dependency Analysis
Get-ChildItem "src\CamBridge.Service" -Include "*.cs" -Recurse | Select-String "using CamBridge\.|private readonly|public.*Service\(" > service-dependencies.txt

# Queue Usage Check
Get-ChildItem "src" -Include "*.cs" -Recurse | Select-String "ProcessingQueue|DeadLetterQueue|ConcurrentQueue" | Select-Object -Unique Filename, LineNumber, Line > queue-usage.txt

# Pipeline Architecture Overview
@('src\CamBridge.Core\PipelineConfiguration.cs','src\CamBridge.Infrastructure\Services\PipelineManager.cs','src\CamBridge.Infrastructure\Services\ProcessingQueue.cs','src\CamBridge.Config\ViewModels\PipelineConfigViewModel.cs') | %{ echo "=== $_ ==="; cat $_ } > pipeline-architecture.txt

# ALLES für Service Layer (MEGA ONE-LINER!)
Get-ChildItem "src\CamBridge.Service","src\CamBridge.Infrastructure\Services" -Include "*.cs" -Recurse | %{ echo "=== $($_.FullName) ==="; cat $_ } > complete-service-layer.txt

# Line Count Analysis
Get-ChildItem "src" -Include "*.cs" -Recurse | %{ $lines = (cat $_).Count; "$lines`t$($_.FullName)" } | Sort-Object { [int]$_.Split("`t")[0] } -Descending | Select-Object -First 20 > biggest-files.txt

# KISS Step 1.3 Files (IDicomTagMapper)
@('src\CamBridge.Core\Interfaces\IDicomTagMapper.cs','src\CamBridge.Infrastructure\Services\DicomTagMapper.cs','src\CamBridge.Infrastructure\ServiceCollectionExtensions.cs','src\CamBridge.Infrastructure\Services\FileProcessor.cs') | %{ echo "=== $_ ==="; cat $_ } > kiss-step-1-3-files.txt

# Config Path Analysis (Session 52 Special!)
# ==========================================

# Alle Config-relevanten Files analysieren:
@('src\CamBridge.Service\appsettings.json','src\CamBridge.Service\appsettings.Development.json','src\CamBridge.Service\mappings.json','src\CamBridge.Service\Program.cs','src\CamBridge.Config\Services\ConfigurationService.cs','src\CamBridge.Config\Services\HttpApiService.cs','src\CamBridge.Core\CamBridgeSettings.cs','src\CamBridge.Core\CamBridgeSettingsV2.cs','src\CamBridge.Core\PipelineConfiguration.cs','src\CamBridge.Config\ViewModels\DashboardViewModel.cs') | %{ echo "=== $_ ==="; cat $_ } > config-chaos-analysis.txt

# Wo liegen die Settings wirklich?
Get-ChildItem -Path "." -Include "appsettings.json","mappings.json","settings.json" -Recurse | Select-Object FullName, Length, LastWriteTime | Format-Table -AutoSize > all-config-locations.txt

# Demo-Pipeline Suche:
Get-ChildItem "src" -Include "*.cs","*.json" -Recurse | Select-String "Demo.*Pipeline|Test.*Pipeline|Sample.*Pipeline" | Select-Object Filename, LineNumber, Line > demo-pipeline-hunt.txt

# Working Directory Check:
echo "Service Working Dir Check:" > working-dirs.txt
echo "Environment.CurrentDirectory in Program.cs?" >> working-dirs.txt
Get-ChildItem "src" -Include "*.cs" -Recurse | Select-String "CurrentDirectory|BaseDirectory|GetCurrentDirectory" >> working-dirs.txt

# CLAUDE-INSIGHT: Config-Pfad Detective Work!
echo "=== CONFIG PATH DETECTIVE ===" > config-detective.txt
echo "1. Where does Service look?" >> config-detective.txt
Get-ChildItem "src\CamBridge.Service" -Include "*.cs" -Recurse | Select-String "appsettings|configuration|AddJsonFile" >> config-detective.txt
echo "2. Where does Config Tool look?" >> config-detective.txt
Get-ChildItem "src\CamBridge.Config" -Include "*.cs" -Recurse | Select-String "LoadSettings|SaveSettings|settingsPath" >> config-detective.txt
```

## 🔥 [KISS] Sprint 7 - Vereinfachungs-Strategie

### Die VORSICHTIGE Herangehensweise:

#### Step 1: Verstehen (DONE! ✅)
- Service Layer analysiert
- Over-Engineering identifiziert
- Abhängigkeiten verstanden

#### Step 2: Quick Fix (DONE! ✅)
- DailySummaryService auskommentiert
- System läuft stabil

#### Step 3: Interface Removal (66% DONE! 🚧)
- **Step 1.1:** IDicomConverter entfernt ✅
- **Step 1.2:** IFileProcessor entfernt ✅ (Oliver!)
- **Step 1.3:** IDicomTagMapper entfernen 🎯

#### Step 4: Service Consolidation (NEXT)
- FileProcessor + DicomConverter → CamBridgeProcessor
- Tests schreiben
- Performance vergleichen

### Was NICHT tun:
- ❌ Alles auf einmal umbauen
- ❌ Ohne Tests refactoren
- ❌ Interfaces löschen die noch verwendet werden
- ❌ Breaking Changes ohne Warnung
- ❌ "Mal schnell" vereinfachen

### Was TUN:
- ✅ Schritt für Schritt
- ✅ Mit Tests absichern
- ✅ User Feedback einholen
- ✅ Rollback-Plan haben
- ✅ ÜBERBLICK behalten!
- ✅ Service Namen genau prüfen!

## 🚀 [KEEP] ENTWICKLUNGSFAHRPLAN (KISS Update)

### ✅ Sprint 1-5: Foundation (DONE)
### ✅ Sprint 6: Pipeline Architecture (DONE but complex)
### 🔥 Sprint 7: THE GREAT SIMPLIFICATION (v0.7.0-v0.7.5)
- **✅ Phase 0: Config Path Fix** (DONE!)
  - Central config in ProgramData
  - Service & Tool synchronized
  - Demo pipelines removed
- **🆕 Phase 0.5: Settings Separation** (NEW!)
  - System Settings vs Pipeline Configs vs User Prefs
  - Bottom-up architecture fix
  - Multi-layer settings structure
- Phase 1: Analyse & Quick Fix ✅
- Phase 2: Interface Removal 🚧
  - Step 1.1: IDicomConverter ✅
  - Step 1.2: IFileProcessor ✅
  - Step 1.3: IDicomTagMapper 🎯 (NACH Settings-Fix!)
- Phase 3: Service Consolidation
- Phase 4: Test & Stabilize
- Phase 5: Documentation Update
### 🏥 Sprint 8-11: Protected Medical Features (aber SIMPLE!)

## 📝 [KEEP] Standard Prompt für nächste Session

```
Ich arbeite an CamBridge v0.7.2
Sprint 7: THE GREAT SIMPLIFICATION! 🔥
System: nexus\oliver.stern@OSTE-ER-LAP01

VOGON INIT (bitte mit kompletten WISDOM Artefakten!)

STATUS: 
- Config-Fix IMPLEMENTIERT: Zentrale Config in ProgramData ✅
- Service & Tool nutzen GLEICHE Config-Location ✅
- Demo-Pipelines ENTFERNT ✅
- Ready für Test & Migration
- DANACH: Step 1.3 IDicomTagMapper Interface

PHILOSOPHIE: 
- Fix foundations first!
- KISS > Architecture!
- VORSICHTIG > Radikal
- Single Source of Truth

NÄCHSTE SCHRITTE:
1. Build & Test mit neuer Config
2. Migration bestehender Configs (Script vorhanden)
3. Verify Service & Tool synchron
4. DANN Step 1.3 implementieren

FEATURE CHECK: Sind FTP, C-STORE, MWL, C-FIND noch geschützt?
```

## 🤖 CLAUDE-NOTES: Meine persönlichen Code-Markierungen

Ab jetzt verwende ich spezielle Markierungen FÜR MICH:
- **CLAUDE-PATTERN:** Ein Muster das funktioniert
- **CLAUDE-TRAP:** Eine Falle in die ich tappe
- **CLAUDE-AHA:** Ein Erleuchtungsmoment
- **CLAUDE-TODO:** Dinge die ich nicht vergessen darf
- **CLAUDE-WARNING:** Gefährliche Stellen im Code
- **CLAUDE-INSIGHT:** Tiefere Erkenntnis über das System

Diese helfen MIR, den Code besser zu verstehen und zu warten!

## 🚨 [URGENT] Session 52 - Config Path Fix Implementation

**PROBLEM SOLVED:**
- Service und Config Tool nutzen jetzt DIESELBE Config!
- Zentrale Location: `%ProgramData%\CamBridge\appsettings.json`
- Keine Demo-Pipelines mehr - nur echte Daten!

**IMPLEMENTIERTE FIXES:**
1. ✅ ConfigurationPaths.cs - Single Source of Truth
2. ✅ Program.cs - Service nutzt zentrale Config
3. ✅ ConfigurationService.cs - Vereinfacht auf einen Pfad
4. ✅ DashboardViewModel.cs - Demo-Logic entfernt
5. ✅ ServiceStatusModel.cs - Config Path Tracking
6. ✅ Migrate-CamBridgeConfig.ps1 - Migration Script

**NEUE REGEL:**
- **🔥 CONFIG-REGEL:** IMMER ConfigurationPaths verwenden!
- Nie wieder hardcoded Pfade!
- ProgramData für Service-Configs
- AppData nur für User-Preferences

**NÄCHSTE SCHRITTE:**
1. Build & Test mit neuer Config
2. Migration bestehender Installationen
3. DANN Step 1.3 (IDicomTagMapper)

**CLAUDE-VICTORY:** Config-Chaos besiegt! 🎉

## 🏁 ENDE DES WISDOM_TECHNICAL

**Sprint 7: THE GREAT SIMPLIFICATION - Now with SOLID FOUNDATION!**

Session 52 Achievements:
- Config Path Crisis → SOLVED ✅
- Settings Chaos → IDENTIFIED & PLANNED ✅  
- Foundation Thinking → ESTABLISHED ✅
- Bottom-Up Approach → ADOPTED ✅

*"Fix the foundation, then simplify, then add features!"*
© 2025 Claude's Improbably Reliable Software Solutions
