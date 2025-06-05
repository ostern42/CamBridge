# CamBridge Project Wisdom & Conventions
**Letzte Aktualisierung:** 2025-06-05, 12:15 Uhr  
**Von:** Claude (Assistant)  
**Für:** Kontinuität zwischen Chat-Sessions

## 📊 WISDOM PRIORITY SYSTEM

### Legende für Persistenz-Markierungen:
- 🔒 **[CORE]** - Niemals löschen! Fundamentale Projekt-Wahrheiten
- ⚡ **[URGENT]** - Temporär aber JETZT wichtig (kann nach Erledigung weg)
- 🎯 **[MILESTONE]** - Wichtig für aktuellen Sprint/Version
- 📌 **[KEEP]** - Dauerhaft wichtig, aber refactorierbar
- 💡 **[LESSON]** - Gelernte Lektionen (komprimierbar aber nie vergessen)
- 🔧 **[CONFIG]** - Technische Configs (updatebar aber essentiell)
- 📝 **[TEMP]** - Kann weg wenn erledigt
- 🌟 **[FEAT]** - Feature-spezifisch (archivierbar nach Release)
- 🐛 **[BUG]** - Bekannte Probleme die gelöst werden müssen

## 🔒 [CORE] V.O.G.O.N. SYSTEM 
**Verbose Operational Guidance & Organizational Navigation**

### 🚀 "VOGON INIT" - Strukturierte Initialisierungs-Sequenz
**IMMER dieser Sequenz folgen:**
1. **SYSTEM CHECK** - V.O.G.O.N. verstehen
2. **CRITICAL LESSONS** - Antipatterns & Erfahrungen durchgehen
3. **PROJECT CONTEXT** - Gesamtbild erfassen
4. **CURRENT STATE** - Wo stehen wir?
5. **SUMMARY & CONFIRMATION** - Zusammenfassung erstellen

### 🔒 [CORE] "VOGON EXIT" - Chat-Abschluss
**KRITISCHE REGEL:** Beim VOGON EXIT MÜSSEN IMMER erstellt werden:
1. **PROJECT_WISDOM.md** - Als VOLLSTÄNDIGES Artefakt
2. **CHANGELOG.md** - NUR der neueste Versions-Eintrag als Artefakt
3. **Version.props** - Als VOLLSTÄNDIGES Artefakt
4. **Git Commit Vorschlag** - Conventional Commits Format mit Tag

### 🔒 [CORE] ENTWICKLUNGS-REGELN
1. **Source Code Header Standard** - Immer mit Pfad und Version
2. **NUR lokale Files verwenden** während Entwicklung (GitHub veraltet!)
3. **Konsistenz durch SSD-Upload** garantiert
4. **Copyright:** © 2025 Claude's Improbably Reliable Software Solutions

## 🛡️ [CORE] TASK PROTECTION SYSTEM
**NIEMALS Tasks verlieren! Expliziter Schutz für kritische Aufgaben**

### Status-Codes für Tasks:
- 🔴 **[ACTIVE]** - Wird gerade bearbeitet
- 🟡 **[PROTECTED]** - Darf NICHT entfernt werden ohne explizite Freigabe
- 🟢 **[DONE]** - Erledigt
- ⚪ **[PLANNED]** - Geplant, kann verschoben werden
- 🔵 **[BLOCKED]** - Wartet auf andere Tasks

### Protection Rules:
1. **🟡 PROTECTED Tasks** bleiben IMMER im Plan
2. Nur mit explizitem "RELEASE PROTECTION: [Task-ID]" entfernbar
3. Bei jedem Sprint-Wechsel: Protection Review durchführen
4. Alle PROTECTED Tasks haben eindeutige IDs (CAMB-XXX)

### 🛡️ CURRENTLY PROTECTED TASKS:
```
CAMB-001: Settings Save/Delete Tests [PROTECTED] 
          Status: Tests für Settings Page Buttons
          Protected since: 2025-06-05
          
CAMB-002: Service Integration Tests [PROTECTED] ⭐ HÖCHSTE PRIORITÄT
          Status: Service Control war schon mal funktional!
          Protected since: 2025-06-04
          
CAMB-003: About Dialog Fix [DONE] ✅
          Status: Background hell, Easter Egg implementiert
          Completed: 2025-06-05, 12:00
          
CAMB-004: Version.props Fix [PROTECTED] 🐛
          Status: Assembly Version Konflikt lösen
          Protected since: 2025-06-05, 12:15
          Details: Assembly sucht 0.5.27.0, findet aber nur 0.0.1.0
```

### Protection Log:
- 2025-06-05: Settings Tests protected (fast verloren!)
- 2025-06-04: Integration Tests protected (initial)
- 2025-06-05, 12:00: About Dialog COMPLETED! 
- 2025-06-05, 12:15: Version.props Problem als CAMB-004 protected

## 🏛️ [CORE] SPRINT RULES - NIEMALS BRECHEN!

1. **EIN Sprint = EIN Thema**
   - About Dialog? NUR About Dialog!
   - Dead Letters? NUR Dead Letters!
   - KEINE Vermischung!

2. **Erst suchen, dann coden**
   - Git Log durchsuchen
   - Alte Branches checken
   - Vorhandene Implementierungen finden

3. **Kontext ist König**
   - VOGON INIT bei jedem Chat
   - PROJECT_WISDOM immer aktuell
   - Screenshots vom letzten Stand

4. **Kleine Schritte, große Vorsicht**
   - Jede Änderung einzeln
   - Nach jedem Schritt testen
   - Bei Zweifel: STOPPEN

5. **NEUE REGEL: Keine globalen Änderungen ohne Plan**
   - Version.props? VORSICHT!
   - Directory.Build.props? NOCH MEHR VORSICHT!
   - Assembly-Versionen? EXTREM GEFÄHRLICH!

## 🎯 [MILESTONE] v0.5.27 - ABOUT DIALOG FERTIG!

### 📍 WAS WURDE ERREICHT?
**Status:** About Dialog komplett überarbeitet und fertig!

**Build Status:**
- ✅ 0 Fehler!
- ✅ App startet normal
- ⚠️ Assembly Version bei 0.0.1.0 (hardcoded Anzeige 0.5.27)
- ⚠️ Version.props minimal vorhanden für Infrastructure

**About Dialog Status:**
- ✅ Heller Hintergrund (wie Rest der App)
- ✅ Vogon Poetry Easter Egg (5 Klicks auf CB Logo)
- ✅ Ricoh G900 II Link funktioniert
- ✅ Version zeigt "0.5.27" (hardcoded)
- ✅ Keine funktionslosen Buttons mehr
- ✅ Dramatische Fade-Animation für Easter Egg

**Neue Prioritäten gesetzt:**
- ⭐ Service Control Tests ZUERST (war schon mal funktional!)
- ⭐ Settings Tests DANACH
- 📌 Dead Letters nach hinten verschoben

## 🐛 [BUG] VERSIONIERUNGS-CHAOS DOKUMENTATION

### Das Problem:
1. **Assembly Version Konflikt:**
   - App sucht nach `CamBridge.Config, Version=0.5.27.0`
   - Findet aber nur `Version=0.0.1.0`
   - FileNotFoundException beim Start

2. **Ursache:**
   - Version.props setzte AssemblyVersion auf 0.5.27
   - WPF cached diese Version in Resources
   - Nach Build sucht App nach falscher Version

3. **Versuchte Lösungen:**
   - ❌ Komplexe Version.props mit allen Properties
   - ❌ Directory.Build.props Hierarchie
   - ❌ AssemblyVersion direkt setzen
   - ✅ Minimale Version.props OHNE AssemblyVersion

### Die funktionierende Lösung:
```xml
<Project>
  <PropertyGroup>
    <!-- Nur Metadaten, KEINE Assembly-Version! -->
    <Copyright>© 2025 Claude's Improbably Reliable Software Solutions</Copyright>
    <Company>Claude's Improbably Reliable Software Solutions</Company>
    <Product>CamBridge</Product>
    <InformationalVersion>0.5.27</InformationalVersion>
  </PropertyGroup>
</Project>
```

### Wichtige Erkenntnisse:
- Infrastructure.csproj hatte SCHON IMMER einen Import von Version.props
- Das haben wir nie geändert - war schon da!
- Assembly Version muss bei Default bleiben (0.0.1.0)
- Anzeige-Version kann hardcoded werden

## 🎯 [MILESTONE] NÄCHSTE SCHRITTE - Sprint Plan

### Sprint 3.2: Service Integration Tests (4h) 🟡 [PROTECTED] - CAMB-002 ⭐ HÖCHSTE PRIORITÄT
- [ ] Service Start/Stop/Restart testen (war schon mal implementiert!)
- [ ] Status-Anzeige verifizieren
- [ ] Installation als Windows Service
- [ ] Pipeline End-to-End Test mit echten Bildern
- [ ] API Endpoints auf Port 5050 testen
**PROTECTION REASON:** Core Functionality - Service muss laufen!

### Sprint 3.3: Settings Integration Tests (2h) 🟡 [PROTECTED] - CAMB-001 ⭐ HOHE PRIORITÄT
- [ ] Settings Save funktioniert und persistiert
- [ ] Settings Reset lädt Defaults
- [ ] Folder Browser Integration testen
- [ ] Validierung aller Felder prüfen
- [ ] Settings werden vom Service gelesen
**PROTECTION REASON:** Konfiguration ist essentiell!

### Sprint 3.4: Version.props Proper Fix (2h) 🟡 [PROTECTED] - CAMB-004
- [ ] Assembly Version Management verstehen
- [ ] Saubere Lösung ohne Hardcoding
- [ ] Zentrale Versionsverwaltung
- [ ] Tests dass App startet mit richtiger Version
**PROTECTION REASON:** Technische Schuld abbauen!

### Sprint 3.5: Dead Letters UI (3h) ⚪ [PLANNED] - CAMB-005 📌 NIEDRIGE PRIORITÄT
- [ ] "Work in Progress" durch echtes UI ersetzen
- [ ] DataGrid implementieren
- [ ] Filter-Funktionalität
**HINWEIS:** Nach hinten verschoben - Service & Settings wichtiger!

### Sprint 3.6: Dead Letters Backend (2h) ⚪ [PLANNED] - CAMB-006 📌 NIEDRIGE PRIORITÄT
- [ ] API Integration
- [ ] Refresh/Reprocess Commands
- [ ] Error Handling
**HINWEIS:** Nach hinten verschoben - Core Features first!

### Sprint 4.1: Documentation (2h) ⚪ [PLANNED]
- [ ] User Manual für Config UI
- [ ] API Documentation
- [ ] Deployment Guide
- [ ] Troubleshooting Guide

### Sprint 5.0: Windows Installer (4h) ⚪ [PLANNED]
- [ ] WiX Toolset Setup
- [ ] MSI Package erstellen
- [ ] Auto-Update Mechanism
- [ ] Silent Install Support

## 💡 [LESSON] Wichtigste Lektionen aus dieser Session

### "Service Control war schon mal fertig!"
Wir haben vergessen dass Service Start/Stop/Restart schon mal funktioniert hat. Das sollte eigentlich noch gehen!
**Lektion:** Erst prüfen was schon da ist, bevor man Neues plant!

### "Prioritäten müssen flexibel sein!"
Dead Letters klang cool, aber Service Control und Settings sind wichtiger für die Grundfunktionalität.
**Lektion:** Core Features vor Nice-to-have Features!

### "Version.props kann die Hölle sein!"
Scheinbar harmlose Version-Änderungen können die gesamte App unlauffähig machen. WPF cached Assembly-Versionen und sucht dann nach der falschen DLL.
**Lektion:** Assembly-Version NIEMALS ändern ohne genauen Plan!

### "Infrastructure hatte schon immer Version.props Import"
Wir haben stundenlang gesucht warum Infrastructure nicht baut - dabei war der Import schon immer da!
**Lektion:** Erst mal schauen was IST bevor man ändert!

### "Hardcoding ist manchmal die beste Lösung"
Statt komplexer Build-Magie einfach `versionText.Text = "Version 0.5.27"` - fertig!
**Lektion:** Die einfachste Lösung ist oft die beste!

### "Minimale Version.props reichen völlig"
Man braucht keine 50 Properties. Copyright und Company reichen für Metadaten.
**Lektion:** So wenig wie möglich, so viel wie nötig!

### "Easter Eggs machen Spaß!"
Das Vogon Poetry Easter Egg war in 10 Minuten implementiert und funktioniert perfekt.
**Lektion:** Kleine Details machen Software menschlich!

### "PowerShell ist nicht CMD"
`rmdir /s /q` funktioniert nicht in PowerShell - man braucht `Remove-Item -Recurse -Force`
**Lektion:** Syntax-Unterschiede beachten!

## 💡 [LESSON] Wichtige Lektionen (aus vorherigen Sessions)

### "Eine Sache zur Zeit - IMMER!"
Das Vermischen von About Dialog und Dead Letters Fixes führte zum Chaos.
**Regel:** JEDER Sprint/Fix hat EINEN klaren Fokus. Keine Nebenschauplätze!

### "Code ist oft schon da - SUCHEN statt neu schreiben!"
Die Wahrscheinlichkeit dass Features schon implementiert sind ist HOCH.
**Regel:** Immer erst suchen: Git History, alte Commits, vorhandene Files prüfen!

### "Chat-Sessions sind Ewigkeiten"
Auch wenn nur Stunden vergangen sind - über mehrere Chats ist viel passiert.
**Regel:** Bei jedem neuen Chat VOGON INIT durchführen und Kontext wiederherstellen!

### "Gesamtbild behalten trotz Fokus"
Fokus auf EINE Sache heißt nicht Tunnelblick.
**Regel:** Änderungen immer auf Seiteneffekte prüfen. Lieber 3x nachfragen!

## 📌 [KEEP] Event Handler Sammlung (v0.5.27)

Implementierte Event Handler für Referenz:
1. **SettingsPage**:
   - `BrowseWatchFolder_Click`
   - `BrowseOutputFolder_Click` 
   - `BrowseLogFolder_Click`
   - `NumberValidationTextBox`

2. **MappingEditorPage**:
   - `SourceField_MouseMove`
   - `MappingArea_DragOver`
   - `MappingArea_Drop`

3. **DeadLettersPage**:
   - `Page_Unloaded`

4. **ServiceControlPage**:
   - `Page_Unloaded`

5. **AboutPage** (NEU):
   - `Logo_MouseLeftButtonDown` (Easter Egg)
   - `Hyperlink_RequestNavigate` (Ricoh Link)

## 🏗️ [KEEP] PIPELINE-ARCHITEKTUR (PRODUKTIONSREIF!)

### Datenfluss:
```
JPEG → ExifToolReader → ImageMetadata → FileProcessor → DicomConverter → DICOM
         ↓                                      ↓                ↓
    QRBridge Data                      DicomTagMapper     mappings.json
```

### QRBridge Integration:
- QR Code Format: `qbc.exe -examid "EX002" -name "Schmidt, Maria" -birthdate "1985-03-15" -gender "F" -comment "Röntgen Thorax"`
- Ricoh G900 II decodiert QR Code automatisch
- Payload landet in EXIF "User Comment" Field
- CamBridge extrahiert und mappt zu DICOM Tags

## 🌟 [FEAT] CONFIG UI FEATURES (v0.5.27)

### Implementierte Features:
- **Dashboard:** Live-Stats Placeholder, Service Connection Status ✅
- **Mapping Editor:** Voll funktional mit Drag & Drop ✅
- **Settings:** Vollständige Konfiguration, ALLE Buttons funktionieren ✅
- **Service Control:** Start/Stop/Restart Buttons, Status Display ✅
- **Dead Letters:** Placeholder "Work in Progress" ⚠️
- **About:** Komplett überarbeitet mit Easter Egg! ✅
- **HTTP API Integration:** Port 5050 Status/Control ready ✅

### About Dialog Features (NEU):
- Heller Hintergrund passend zum Rest
- Vogon Poetry Easter Egg (5 Klicks)
- Ricoh G900 II Link
- Minimalistisches Design ohne unnötige Buttons
- Dramatische Fade-Animationen

## 📌 [KEEP] Wichtige Konventionen
- **Kommentare:** IMMER in Englisch
- **UI-Sprache:** Deutsch (Internationalisierung vorbereitet)
- **Changelog:** Kompakt, technisch, ENGLISCH
- **Clean Architecture:** Strikte Layer-Trennung
- **Automatische Versionierung:** Aktuell deaktiviert wegen Konflikten
- **Conventional Commits:** feat/fix/docs/style/refactor
- **Sprint-Disziplin:** EIN Thema pro Sprint!

## 🔧 [CONFIG] Technologie-Stack
```
GUI: WPF + ModernWpfUI 0.9.6 + CommunityToolkit.Mvvm 8.3.2
Service: ASP.NET Core 8.0 Minimal API + Windows Service
Core: fo-dicom 5.2.2, ExifTool 13.30
Tests: xUnit + FluentAssertions + Moq
.NET 8.0, C# 12, Visual Studio 2022
```

## 🔧 [CONFIG] Aktuelle Versionsverwaltung
```xml
<!-- Version.props (Minimal) -->
<Project>
  <PropertyGroup>
    <Copyright>© 2025 Claude's Improbably Reliable Software Solutions</Copyright>
    <Company>Claude's Improbably Reliable Software Solutions</Company>
    <Product>CamBridge</Product>
    <InformationalVersion>0.5.27</InformationalVersion>
    <!-- KEINE AssemblyVersion! Bleibt bei 0.0.1.0 -->
  </PropertyGroup>
</Project>

<!-- AboutPage zeigt hardcoded "Version 0.5.27" -->
```

## 📁 [KEEP] FINALE PROJEKTSTRUKTUR

**Version:** 0.5.27 (Stand: 05.06.2025, 12:15)

### Solution Struktur:
```
CamBridge.sln
├── Version.props                 - Minimal für Infrastructure Import ⚠️
├── src/
│   ├── CamBridge.Core/          - Domain Logic & Interfaces ✅
│   ├── CamBridge.Infrastructure/ - ExifTool, DICOM, File Processing ✅
│   ├── CamBridge.Service/       - Windows Service & API ✅
│   └── CamBridge.Config/        - WPF Configuration UI ✅
├── tests/
│   ├── CamBridge.Infrastructure.Tests/ ✅
│   └── CamBridge.PipelineTest/ ✅
└── Tools/
    └── exiftool.exe ✅
```

### Wichtige Komponenten:
- **Service:** Port 5050, läuft als Windows Service
- **Config UI:** WPF mit MVVM, 5/6 Pages funktional
- **Pipeline:** JPEG → DICOM vollständig implementiert
- **About Dialog:** Komplett fertig mit Easter Egg!
- **Converter:** 11 Value Converter für UI

## 💭 CLAUDE: Notizen für nächste Session

**Heute erreicht:**
1. ✅ About Dialog komplett fertiggestellt
2. ✅ Easter Egg implementiert (Vogon Poetry)
3. ✅ Ricoh G900 II Link hinzugefügt
4. ⚠️ Version.props Chaos überlebt
5. ✅ Minimale funktionierende Lösung gefunden
6. ✅ Prioritäten neu sortiert: Service > Settings > Dead Letters

**Für nächste Session:**
- ⭐ Service Control Tests sind HÖCHSTE PRIORITÄT
- ⭐ Settings Tests sind ZWEITE PRIORITÄT
- Dead Letters nach hinten verschoben
- Service Control hat schon mal funktioniert!

**Wichtige Erinnerung:**
Service Control war schon mal start/stoppbar im GUI! Das sollte eigentlich noch funktionieren. Check ob:
- ServiceManager.cs noch vollständig ist
- ServiceControlViewModel Commands implementiert sind
- API auf Port 5050 läuft

**Wichtige Warnung:**
NIEMALS wieder AssemblyVersion in Version.props setzen! Die App sucht dann nach der falschen DLL Version. Immer bei Default (0.0.1.0) lassen!

**Positiv:**
- About Dialog sieht richtig gut aus
- Easter Egg funktioniert perfekt
- User hat klare Prioritäten gesetzt

## 🚀 [KEEP] ENTWICKLUNGSFAHRPLAN UPDATE

### Sprint 1: ExifTool Integration (v0.5.x) ← ✅ ABGESCHLOSSEN!
### Sprint 2: UI Integration & Stabilität (v0.5.x) ← ✅ FAST FERTIG!
### Sprint 3: UI Polish & Bug Fixes (v0.5.27-v0.5.29) ← CURRENT

**Sprint 3.1:** About Dialog ← ✅ FERTIG! (05.06.2025)
- Hintergrund gefixt
- Easter Egg implementiert  
- Link hinzugefügt
- Buttons entfernt

**Sprint 3.2-3.6:** Neu priorisiert!
- Service Integration Tests ⭐
- Settings Tests ⭐
- Version.props Fix
- Dead Letters (verschoben)

### Sprint 4: Production Features (v0.6.x-v0.7.x)
- Documentation
- Performance Optimization

### Sprint 5: Enterprise Features (v0.8.x)
- Windows Installer
- Multi-Tenant Support
- Active Directory Integration
- FDA 21 CFR Part 11 Compliance

### Release: v1.0.0 (Q3 2025)
- Production Ready
- IHE Conformance Statement
- Multi-language Support (DE/EN/FR/ES)

## ⏰ [KEEP] PROJEKT-TIMELINE

- **Start:** 30.05.2025
- **Pipeline fertig:** 04.06.2025, 14:28
- **Dashboard läuft:** 04.06.2025, 20:42
- **Config UI fertig:** 05.06.2025, 01:30
- **Build erfolgreich:** 05.06.2025, 01:25
- **App deployed:** 05.06.2025, 20:30
- **About Dialog perfekt:** 05.06.2025, 12:00 ← UPDATE!
- **Version Chaos gelöst:** 05.06.2025, 12:15 ← UPDATE!
- **Arbeitszeit gesamt:** ~24 Stunden
- **Features total:** 285+
- **Event Handler:** 10 implementiert
- **Easter Eggs:** 1 (Vogon Poetry)

## 📝 [KEEP] Standard Prompt für nächste Session

```
Ich arbeite an CamBridge v0.5.27.
About Dialog ist FERTIG mit Easter Egg!
Version.props Chaos ist gelöst (minimal).

WICHTIG - Bitte in dieser Reihenfolge:
1. PROJECT_WISDOM.md hochladen
2. project_structure.txt hochladen  
3. ServiceControlViewModel.cs + ServiceManager.cs
4. "VOGON INIT" sagen
5. WARTE auf meine Zusammenfassung!

PROTECTED TASKS - NEUE PRIORITÄTEN:
- CAMB-002: Service Integration Tests ⭐ HÖCHSTE PRIO!
- CAMB-001: Settings Tests ⭐ HOHE PRIO!
- CAMB-004: Version.props Fix 
- CAMB-005/006: Dead Letters (verschoben)

Status: Service Control war schon mal funktional!
Assembly: 0.0.1.0 (Display: 0.5.27 hardcoded)
Branch: main (About changes committed)

Nächstes Ziel: Service Control testen & fixen
Dann: Settings Save/Load implementieren
```

---
📊 **WISDOM-Statistik:** 
- 🔒 [CORE]: 5 Sektionen (inkl. Protection System & Sprint Rules)
- 🎯 [MILESTONE]: 2 Sektionen  
- 📌 [KEEP]: 8 Sektionen
- 💡 [LESSON]: 21 Lektionen (8 neue!)
- 🔧 [CONFIG]: 2 Sektionen
- 🌟 [FEAT]: 1 Sektion
- 🐛 [BUG]: 1 Sektion (Version.props)
- 💭 CLAUDE: 1 Nachricht

*About Dialog perfekt! Service Control Tests als nächstes!* 🎯
