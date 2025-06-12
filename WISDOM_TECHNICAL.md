# WISDOM Technical - Entwicklung & Technische Details
**Letzte Aktualisierung:** 2025-06-13, 01:15  
**Von:** Claude (Assistant)  
**Für:** Technische Kontinuität & Entwicklungsplan
**Version:** 0.7.10 🔧
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
- ✂️ **[SURGERY]** - Code Removal Operations!
- 🔧 **[CONFIG-UNITY]** - Configuration Consistency Mission!

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

## 🔧 [CONFIG-UNITY] Session 60 - Configuration Consistency Mission

### Das Config-Chaos Analysiert:
1. **Service JSON Format:**
   ```json
   {
     "ServiceSettings": { ... },
     "Pipelines": [ 
       {
         "WatchSettings": { "Path", "Filter", ... },
         "OutputSettings": { ... },
         "ProcessingOptions": { ... }
       }
     ]
   }
   ```

2. **Core erwartet V2 Format:**
   ```json
   {
     "CamBridge": {
       "Version": "2.0",
       "Pipelines": [
         {
           "WatchSettings": { "Path", "FilePattern", ... },
           "ProcessingOptions": { ... }
         }
       ]
     }
   }
   ```

3. **Config UI Problem:**
   - Nutzt KEINEN ConfigurationPaths in App.xaml.cs!
   - ParseServiceFormat als Workaround
   - Findet Config nicht weil falscher Pfad

4. **Multiple Settings-Systeme:**
   - CamBridgeSettings (V1)
   - CamBridgeSettingsV2 (Pipeline-basiert)
   - SystemSettings (3-Layer Architecture)
   - Custom Service JSON

### Die Lösung: ONE CONFIG TO RULE THEM ALL!

#### Phase 1: Einheitliches JSON Format
```json
{
  "CamBridge": {
    "Version": "2.0",
    "Service": {
      "ApiPort": 5111,
      "EnableHealthChecks": true,
      "HealthCheckInterval": "00:01:00"
    },
    "Pipelines": [
      {
        "Id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
        "Name": "Radiology Pipeline",
        "Enabled": true,
        "WatchSettings": {
          "Path": "C:\\CamBridge\\Watch\\Radiology",
          "FilePattern": "*.jpg;*.jpeg",
          "IncludeSubdirectories": false,
          "MinimumFileAgeSeconds": 2
        },
        "ProcessingOptions": {
          "DeleteSourceAfterSuccess": false,
          "ProcessExistingOnStartup": true,
          "MaxRetryAttempts": 3,
          "RetryDelaySeconds": 30,
          "ErrorFolder": "C:\\CamBridge\\Errors\\Radiology",
          "ArchiveFolder": "C:\\CamBridge\\Output\\Radiology",
          "BackupFolder": "C:\\CamBridge\\Archive\\Radiology",
          "CreateBackup": true,
          "MaxConcurrentProcessing": 5,
          "OutputFilePattern": "{PatientId}_{StudyDate}_{Counter:0000}.dcm"
        },
        "MappingSetId": "00000000-0000-0000-0000-000000000001"
      }
    ],
    "MappingSets": [
      {
        "Id": "00000000-0000-0000-0000-000000000001",
        "Name": "Ricoh Default",
        "Rules": [ ... ]
      }
    ],
    "GlobalDicomSettings": { ... },
    "DefaultProcessingOptions": { ... },
    "Logging": { ... },
    "Notifications": { ... },
    "ExifToolPath": "Tools\\exiftool.exe"
  }
}
```

#### Phase 2: Config UI Fix
```csharp
// App.xaml.cs - ADD THIS!
protected override void OnStartup(StartupEventArgs e)
{
    base.OnStartup(e);
    
    // CRITICAL: Initialize config path like Service does!
    ConfigurationPaths.InitializePrimaryConfig();
    
    // Rest of startup...
}
```

#### Phase 3: Service Program.cs Update
```csharp
// Use CamBridge section!
services.Configure<CamBridgeSettingsV2>(configuration.GetSection("CamBridge"));
// NOT root level!
```

#### Phase 4: Remove ParseServiceFormat
Das war ein Workaround! Mit einheitlichem Format nicht mehr nötig.

## 🔥 [KISS] MAKE CAMBRIDGE SIMPLE AGAIN - Sprint 7 Status

### Phase Progress:
1. **Foundation** (v0.7.1-v0.7.4) ✅ COMPLETE!
2. **Testing Tools** (v0.7.5+tools) ✅ COMPLETE!
3. **Version Consistency** (v0.7.6) ✅ COMPLETE!
4. **Build Fixes** (v0.7.7) ✅ COMPLETE!
5. **Dead Letter Removal** (v0.7.8-v0.7.9) ✅ COMPLETE!
6. **Config Unity** (v0.7.10) 🔧 IN PROGRESS!
7. **Interface Cleanup** (v0.8.0) 🚀 NEXT!
8. **Service Consolidation** (v0.8.1+) 📋 FUTURE!
9. **Test & Stabilize** (v0.9.0) 🧪 THEN!

### Erreichte Vereinfachungen:
- **Interfaces entfernt:** 2 von 15 ✅
- **Dead Letter entfernt:** -650 LOC! ✅
- **Foundation gelegt:** Settings Architecture ✅
- **Testing revolutioniert:** Tab-Complete System ✅
- **Build optimiert:** No-ZIP option ✅
- **Version vereinheitlicht:** Directory.Build.props ✅
- **Error Handling:** Simple folder approach ✅
- **Config Unity:** One format for all! 🔧

## 🔒 [CORE] ENTWICKLUNGS-REGELN (Session 60 erweitert!)

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
24. **✂️ SURGERY-REGEL:** Code entfernen ist besser als Code refactoren!
25. **🆕 GENERATEASSEMBLYINFO-REGEL:** NIE auf false setzen!
26. **🔧 CONFIG-UNITY-REGEL:** Eine Config-Struktur für ALLE Modi!
27. **🔧 CONFIG-PATH-REGEL:** ConfigurationPaths ÜBERALL nutzen!
28. **🔧 JSON-WRAPPER-REGEL:** Alles in "CamBridge" section!

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
BUG-006: Config UI loads empty [IDENTIFIED] 🔧

DEADLETTER-001: Dead Letter Queue Removal [DONE] ✅
                Status: Successfully removed!
                Session: 58
                Result: -650 LOC removed
                Replacement: Simple Error Folder

CONFIG-UNITY-001: Configuration Consistency [IN PROGRESS] 🔧
                Status: Root cause found
                Problem: 3+ config systems
                Solution: One JSON format for all
                Session: 60
                Expected: Debug = Release behavior

INTERFACE-001: Interface Removal Phase 2 [NEXT] 🎯
                Status: Ready after Config Unity
                Target: Remove remaining 13 interfaces
                Approach: Step by step
                Expected: More simplification!

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

## 🎯 [MILESTONE] Aktueller Stand: v0.7.10 🔧

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
  - v0.7.8-0.7.9: Dead Letter Removal ✅ DONE!
  - v0.7.10: Configuration Unity 🔧 IN PROGRESS!

### Session 60 Focus:
- **Problem:** 3+ Config-Systeme parallel
- **Root Cause:** Config UI nutzt keinen ConfigurationPaths
- **Solution:** Einheitliches JSON Format + ConfigurationPaths überall
- **Testing:** Debug = Release muss identisch sein!

## 💡 [LESSON] Session 60 - Configuration Unity Insights

### Was wir lernen:
1. **Multiple Config-Systeme = Chaos!**
2. **ConfigurationPaths ist der Schlüssel**
3. **JSON-Struktur muss überall gleich sein**
4. **"CamBridge" wrapper section für alles**
5. **ParseServiceFormat war ein Workaround**
6. **Config UI braucht ConfigurationPaths.InitializePrimaryConfig()**
7. **Service JSON muss zu Core passen**
8. **Echte GUIDs statt 11111111**

### CLAUDE-LEARNINGS:
- **CLAUDE-TRAP:** Verschiedene JSON-Strukturen in verschiedenen Projekten!
- **CLAUDE-INSIGHT:** ConfigurationPaths löst alles!
- **CLAUDE-PATTERN:** Wrapper sections für Namespace-Klarheit
- **CLAUDE-TODO:** Config Unity implementieren
- **CLAUDE-WISDOM:** Debug = Release durch gleiche Config-Pfade!
- **CLAUDE-ACHIEVEMENT:** Root cause gefunden!

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
Surgery: Dead Letter Removal v1.0 ✅ COMPLETE!
Config: Unity Implementation v0.1 🔧 IN PROGRESS!
```

## 🔧 [CONFIG-UNITY] Implementation Plan

### 1. Neue appsettings.json (Unified V2 Format):
```json
{
  "CamBridge": {
    "Version": "2.0",
    "Service": {
      "ApiPort": 5111,
      "EnableHealthChecks": true,
      "HealthCheckInterval": "00:01:00"
    },
    "Pipelines": [
      {
        "Id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
        "Name": "Radiology Pipeline",
        "Enabled": true,
        "WatchSettings": {
          "Path": "C:\\CamBridge\\Watch\\Radiology",
          "FilePattern": "*.jpg;*.jpeg",
          "IncludeSubdirectories": false,
          "MinimumFileAgeSeconds": 2
        },
        "ProcessingOptions": {
          "DeleteSourceAfterSuccess": false,
          "ProcessExistingOnStartup": true,
          "MaxRetryAttempts": 3,
          "RetryDelaySeconds": 30,
          "ErrorFolder": "C:\\CamBridge\\Errors\\Radiology",
          "ArchiveFolder": "C:\\CamBridge\\Output\\Radiology",
          "BackupFolder": "C:\\CamBridge\\Archive\\Radiology",
          "CreateBackup": true,
          "MaxConcurrentProcessing": 5,
          "OutputFilePattern": "{PatientId}_{StudyDate}_{Counter:0000}.dcm"
        },
        "MappingSetId": "00000000-0000-0000-0000-000000000001"
      }
    ],
    "MappingSets": [
      {
        "Id": "00000000-0000-0000-0000-000000000001",
        "Name": "Ricoh Default",
        "Description": "Standard mapping for Ricoh G900 II",
        "IsSystemDefault": true,
        "Rules": [
          {
            "Name": "PatientName",
            "SourceField": "name",
            "DicomTag": "(0010,0010)",
            "Transform": "None",
            "Required": true
          }
        ]
      }
    ],
    "GlobalDicomSettings": {
      "ImplementationClassUid": "1.2.276.0.7230010.3.0.3.6.4",
      "ImplementationVersionName": "CAMBRIDGE_0710"
    },
    "DefaultProcessingOptions": {
      "ArchiveFolder": "C:\\CamBridge\\Output",
      "ErrorFolder": "C:\\CamBridge\\Errors"
    },
    "Logging": {
      "LogLevel": "Information",
      "LogFolder": "C:\\ProgramData\\CamBridge\\Logs"
    },
    "Notifications": {
      "Enabled": true,
      "DeadLetterThreshold": 100
    },
    "ExifToolPath": "Tools\\exiftool.exe"
  }
}
```

### 2. Code-Änderungen:

#### App.xaml.cs:
```csharp
protected override void OnStartup(StartupEventArgs e)
{
    base.OnStartup(e);
    
    // CRITICAL FIX: Initialize config like Service does!
    ConfigurationPaths.InitializePrimaryConfig();
    
    // Rest of startup...
}
```

#### ServiceCollectionExtensions.cs:
```csharp
// Register from CamBridge section!
services.Configure<CamBridgeSettingsV2>(configuration.GetSection("CamBridge"));
```

#### ConfigurationService.cs:
```csharp
// Remove ParseServiceFormat - not needed with unified format!
```

### 3. Migration Strategy:
1. Backup existing configs
2. Convert to unified format
3. Test with Tab-Complete
4. Verify pipelines show in UI
5. Celebrate consistency!

## 📌 [KEEP] PowerShell One-Liner Sammlung

```powershell
# TAB-COMPLETE TESTING
# ====================
0[TAB]     # Build ohne ZIP (schnell!)
00[TAB]    # Build mit ZIP
9[TAB]     # Quick test (no build)
99[TAB]    # Full test (with build)
h[TAB]     # Help

# Version Check
Invoke-RestMethod -Uri "http://localhost:5111/api/status/version" | ConvertTo-Json

# Pipeline Check (NEW!)
Invoke-RestMethod -Uri "http://localhost:5111/api/pipelines" | ConvertTo-Json

# Config Path Check
Get-Content "$env:ProgramData\CamBridge\appsettings.json" | ConvertFrom-Json | ConvertTo-Json -Depth 10

# Test Debug vs Release
# 1. Build Debug: 0[TAB]
# 2. Test Config UI: 2[TAB]
# 3. Build Release: 00[TAB]  
# 4. Test Config UI again: 2[TAB]
# Both must show same pipelines!
```

## 🚀 [KEEP] ENTWICKLUNGSFAHRPLAN

### ✅ Sprint 7: THE GREAT SIMPLIFICATION (v0.7.0-v0.8.0)
- **✅ Phase 0: Config Path Fix** (v0.7.2)
- **✅ Phase 1: Settings Architecture** (v0.7.3)
- **✅ Phase 2: Testing & Bug Fixes** (v0.7.4)
- **✅ Phase 3: Tab-Complete Testing** (v0.7.5+tools)
- **✅ Phase 4: Version Consistency** (v0.7.6)
- **✅ Phase 5: Build Fixes** (v0.7.7)
- **✅ Phase 6: Dead Letter Removal** (v0.7.8-v0.7.9) DONE!
- **🔧 Phase 7: Config Unity** (v0.7.10) IN PROGRESS!
- **🎯 Phase 8: Interface Removal Complete** (v0.8.0)
- **🧪 Phase 9: Test & Stabilize** (v0.9.0)

### 🏥 Sprint 8-11: Protected Medical Features
- Sprint 8: FTP Server (SIMPLE!)
- Sprint 9: C-STORE SCP
- Sprint 10: Modality Worklist
- Sprint 11: C-FIND SCP

## 📝 [KEEP] Standard Prompt für nächste Session

```
Ich arbeite an CamBridge v0.7.10
Sprint 7: THE GREAT SIMPLIFICATION! 🔥
System: nexus\oliver.stern@OSTE-ER-LAP01

VOGON INIT

STATUS: 
- Dead Letter Removal COMPLETE! ✅
- -650 LOC removed! 🎉
- Config Unity IN PROGRESS! 🔧
- Root cause: Config UI nutzt keinen ConfigurationPaths!

ERKENNTNISSE:
- Multiple Config-Systeme = Chaos!
- ConfigurationPaths ist die Lösung!
- JSON muss einheitlich sein!
- "CamBridge" wrapper für alles!

NÄCHSTE SCHRITTE:
1. Config UI mit ConfigurationPaths fixen
2. JSON auf einheitliches V2 Format
3. Test Debug vs Release (0[TAB] vs 00[TAB])
4. Pipelines müssen in Config UI erscheinen!

TESTING:
- 0[TAB] = Build ohne ZIP (Debug)
- 00[TAB] = Build mit ZIP (Release)
- 2[TAB] = Config UI starten
- Beide Modi müssen gleich funktionieren!

PHILOSOPHIE: 
- KISS > Architecture!
- One Config Format for All!
- Debug = Release!
- ConfigurationPaths everywhere!

FEATURE CHECK: Sind FTP, C-STORE, MWL, C-FIND noch geschützt?
```

## 🤖 CLAUDE-NOTES: Meine persönlichen Markierungen

### Session 60 CLAUDE-INSIGHTS:
- **CLAUDE-DISCOVERY:** 3+ Config-Systeme parallel!
- **CLAUDE-ROOT-CAUSE:** Config UI missing ConfigurationPaths!
- **CLAUDE-SOLUTION:** Unified JSON + ConfigurationPaths everywhere!
- **CLAUDE-LEARNING:** ParseServiceFormat was a workaround!

### Session 60 CLAUDE-STATS:
- **Config systems found:** 3+
- **JSON formats:** Service vs Core vs V1
- **Root cause clarity:** 100%
- **Solution complexity:** Simple!
- **Expected outcome:** Debug = Release!

## 🏁 ENDE DES WISDOM_TECHNICAL

**Sprint 7: THE GREAT SIMPLIFICATION - Config Unity Mission!**

Session 60 Mission: One Config Format to Rule Them All!
Next Mission: Implement Config Unity, then Interface Removal!
Philosophy: Professional through Consistency!

*"Making the improbable reliably consistent through configuration unity!"*
© 2025 Claude's Improbably Reliable Software Solutions
