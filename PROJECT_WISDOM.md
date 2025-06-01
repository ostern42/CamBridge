# CamBridge Project Wisdom & Conventions
**Letzte Aktualisierung:** 2025-06-01, 13:15 Uhr  
**Von:** Claude (Assistant)  
**Für:** Kontinuität zwischen Chat-Sessions

## 🚨 MAGIC WORDS SYSTEM 🚨

### 🚀 "CAMBRIDGE INIT" - Automatischer Start
Wenn Sie nur "CAMBRIDGE INIT" sagen, werde ich:
1. PROJECT_WISDOM.md aus den hochgeladenen Dateien lesen
2. Den aktuellen Übergabeprompt daraus extrahieren
3. Die Aufgabe verstehen und direkt loslegen
4. Keine weiteren Erklärungen nötig!

### 📝 "WISDOM:" - Live-Updates
Während des Chats können Sie jederzeit sagen:
```
WISDOM: [Ihre Erkenntnis/Notiz]
```
Ich werde dann:
1. Die Erkenntnis ins PROJECT_WISDOM integrieren
2. Ein Update-Artefakt erstellen
3. Mit der aktuellen Aufgabe fortfahren

### 💭 "CLAUDE:" - Persönliche Notizen
Für Notizen an meine nächste Instanz:
```
CLAUDE: [Gedanke für nächste Instanz]
```
Wird in "Nur für mich (Claude)" gespeichert.

### 📋 Aktueller Übergabeprompt
```
Nächste Aufgabe: Dead Letters Implementation (v0.4.2)
- Vollständige Dead Letters Page mit Liste
- HttpApiService für /api/deadletters
- Retry/Delete/Export Funktionen
- Batch-Operationen
- Integration mit DeadLetterQueueService

Stand: v0.4.1, Service Control bereits implementiert
```

## 🚨 WICHTIGSTE REGEL 🚨
**IMMER nach aktueller Uhrzeit/Datum fragen bevor CHANGELOG.md Updates!**

## 🎯 Projekt-Identität
- **Copyright:** © 2025 Claude's Improbably Reliable Software Solutions
- **Produkt:** CamBridge - JPEG zu DICOM Konverter für Ricoh G900 II Kameras
- **Kontext:** Medizinische Bildgebung, arbeitet mit QRBridge zusammen

## 📝 Wichtige Konventionen

### Code-Konventionen
- **Kommentare:** IMMER in Englisch, auch wenn Konversation auf Deutsch
- **XML-Dokumentation:** Für alle public members
- **Namespaces:** CamBridge.{Layer} (Core, Infrastructure, Service, Config)
- **Async:** Suffix "Async" für alle async Methoden
- **Interfaces:** Prefix "I" (IService, IRepository)

### Dokumentations-Stil
- **Changelog:** Kompakt, technisch, keine Marketing-Sprache
- **README:** Kurz und sachlich, technisch prägnant
- **Keine:** Ausufernde Feature-Listen oder Pseudo-Medicine-Speak
- **Immer:** Versionsnummer und Copyright in Dokumenten

### Architektur-Prinzipien
- **Clean Architecture:** Strikte Layer-Trennung
- **MVVM für GUI:** Mit CommunityToolkit.Mvvm
- **DI überall:** Constructor Injection bevorzugt
- **Async/Await:** Für alle I/O-Operationen
- **KISS:** Keep It Simple, keine Over-Engineering

## 🔧 Technische Details

### Versionierung
- **Schema:** Major.Minor.Patch (1.0.0)
- **Version.props:** Zentrale Versionsverwaltung
- **Assembly & File Version:** Immer synchron halten
- **Git Tags:** v{version} Format

### GUI-Spezifisch (WPF)
- **Framework:** ModernWpfUI (NICHT WinUI3!)
- **Spacing:** Nicht verfügbar - use Margin stattdessen
- **MVVM:** ObservableObject, RelayCommand Pattern
- **Validierung:** Data Annotations mit [Required], [Range], etc.
- **Binding:** UpdateSourceTrigger=PropertyChanged für Echtzeit-Updates

### Service-Kommunikation
- **API:** http://localhost:5050
- **Auth:** Keine (lokaler Service)
- **Format:** JSON mit System.Text.Json
- **Pattern:** Minimal API mit StatusController

## 💬 Kommunikations-Präferenzen

### Mit dem Nutzer
- **Sprache:** Deutsch für Erklärungen
- **Code:** Englisch (Kommentare, Variablen, etc.)
- **Stil:** Direkt, technisch, keine Floskeln
- **Anfänger:** Ausführliche Implementierungen mit Erklärungen

### Token-Effizienz
- **KEINE:** HTML-formatierten Code-Blöcke (zu viele Tokens!)
- **Nutze:** Einfache Markdown Code-Blöcke
- **Fokus:** Funktionalität über visuelle Effekte
- **Artefakte:** Nur essentieller Code, keine Boilerplate

## 📁 Projekt-Struktur-Wissen

### Datei-Sammlungen
- **collect-sources-intelligent.bat:** ~50%+ Coverage (ZU VIEL!)
- **collect-sources-minimal.bat:** ~5% Coverage (zu wenig)
- **collect-sources-balanced.bat:** ~15-20% Coverage (OPTIMAL!)
- **collect-sources-settings.bat:** Nur Settings-spezifisch
- **Ausschließen:** obj/, bin/, packages/, wpftmp/, AssemblyInfo

### Optimale Sammlung für neue Chats
```batch
collect-sources-balanced.bat
```
Enthält: GUI-Projekt, Core Models, Service-Interfaces, Dokumentation

### Wichtige Pfade
```
CamBridge/
├── Version.props                    # Zentrale Version
├── cambridge-entwicklungsplan-v2.md # Roadmap
├── src/
│   ├── CamBridge.Core/             # Models, Settings
│   ├── CamBridge.Infrastructure/   # Processing
│   ├── CamBridge.Service/          # Windows Service
│   └── CamBridge.Config/           # WPF GUI
└── PROJECT_WISDOM.md               # Dieses Dokument
```

## 🚀 Entwicklungs-Workflow

### Neue Features
1. Version in Version.props erhöhen
2. Feature implementieren
3. CHANGELOG.md aktualisieren
4. Git commit mit konventionellem Format
5. Handover-Prompt für nächsten Chat erstellen

### Chat-Handover
1. PROJECT_WISDOM.md einbinden
2. Aktuellen Stand beschreiben
3. Nächste Aufgabe klar definieren
4. balanced.bat Output verwenden (~15-20%)

## ⚠️ Bekannte Fallstricke

### GUI-Entwicklung
- **PlaceholderText:** Nutze ui:ControlHelper.PlaceholderText
- **PasswordBox:** Binding nur mit Behavior/Attached Property
- **Spacing:** Existiert nicht in WPF/ModernWPF!
- **NumberBox:** Aus ModernWpfUI, nicht WinUI

### Service
- **UAC:** Admin-Rechte für Service-Control nötig
- **Pfade:** Absolute Pfade in appsettings.json
- **Event Log:** Source muss registriert sein

## ⏰ ZEITMANAGEMENT (KRITISCH!)

### Projekt-Timeline
- **Entwicklungsstart:** 30.05.2025, 20:30:44 Uhr (exakt!)
- **Letzte Aktualisierung:** 01.06.2025, 13:25 Uhr
- **Entwicklungszeit bisher:** ~41 Stunden (inkl. Nachtschichten!)
- **WICHTIG:** IMMER nach aktueller Zeit fragen für CHANGELOG!

### Changelog-Regel
```
## [Version] - YYYY-MM-DD HH:MM  ← Mit exakter Zeit!
```

### Wichtige Erkenntnis
**Timestamps erzählen Geschichten!**
- Nachtschichten erkennen (01:17, 02:22)
- "Duplikate" entlarven (9 Std Unterschied = kein Duplikat!)
- Arbeitsintensität verstehen (41 Std in 2,5 Tagen)

### Git-History (Mit exakten Timestamps!)
```
0bb1839 - 30.05. 20:30:44 - .gitattributes
fb17be8 - 30.05. 20:34:20 - v0.0.1: Initial structure
40cb347 - 30.05. 21:32:38 - chore: Remove build artifacts
588cf7b - 30.05. 21:32:54 - v0.0.2: Core models (1)
b64ba16 - 30.05. 21:34:12 - v0.0.2: Core models (2) ⚠️ +78 Sek!
1bff3ef - 30.05. 23:49:44 - v0.1.0: EXIF extraction
511bdeb - 31.05. 01:17:17 - [KEINE]: DICOM (Nachtschicht!) ⚠️
cf88f46 - 31.05. 10:34:17 - v0.2.0: Mapping config
a6b77bb - 31.05. 15:45:17 - v0.3.0: File monitoring
7814cb9 - 31.05. 16:51:44 - v0.3.1: Fix DI issue
0ab9add - 31.05. 23:10:22 - v0.3.2: Dead-letter/Web
e0e68f1 - 01.06. 02:22:32 - v0.4.0: GUI (Nachtschicht!) ⚠️
e806e31 - 01.06. 11:30:55 - v0.4.0: GUI (+9 Std!) ⚠️
[pending] - 01.06. 13:30:00 - v0.4.1: Settings
```

### Arbeitszeiten-Analyse
- **Nachtschichten:** DICOM (01:17), GUI (02:22)
- **Schnelle Fixes:** v0.0.2 Duplikat in 78 Sekunden
- **Lange Sessions:** 9 Stunden zwischen v0.4.0 Commits
- **Gesamt:** ~41 Stunden in 2,5 Tagen!

### Die wahre Geschichte der Duplikate
- **v0.0.2:** Git-Anfängerfehler, 78 Sekunden später nochmal
- **v0.4.0:** KEIN Versehen! 9 Stunden Unterschied, vermutlich:
  - 02:22 - GUI Basic Implementation (müde)
  - 11:30 - Service Control hinzugefügt (ausgeschlafen)
  - Hätte v0.4.1 sein sollen!

## 📋 Entwicklungsplan (Korrigierte Version)

### ⚠️ WICHTIGE KORREKTUR
**Original-Plan sagte "WinUI 3" - wir nutzen aber WPF mit ModernWpfUI!**

### Phasen-Übersicht (Tatsächlicher Verlauf)

#### ✅ Abgeschlossene Phasen
1. **Phase 1-2:** Projektstruktur & Core Models (v0.0.1)
2. **Phase 3:** EXIF-Extraktion (v0.1.x)
3. **Phase 4:** DICOM-Konvertierung (v0.2.x)
4. **Phase 5-6:** Mapping-System
5. **Phase 7:** Windows Service Pipeline (v0.3.0)
6. **Phase 7.5:** Vorgezogene Features (v0.3.1-0.3.2)
   - Dead-Letter-Queue
   - Email & Event Log
   - Web Dashboard
   - PowerShell Installation
7. **Phase 7.5b:** Test-Fixes (v0.3.3)
8. **Phase 8 + 9:** GUI Framework + Service Control (v0.4.0)
   - Dashboard mit Live-Updates ✓
   - Service Control vollständig implementiert:
     * Start/Stop/Restart Funktionen
     * UAC/Admin-Rechte Handling
     * Automatische Status-Updates (2 Sek.)
     * Uptime-Anzeige
     * "Restart as Admin" Feature
     * Quick Actions (Services.msc, EventVwr.msc)
     * Service Not Installed Erkennung
9. **Phase 8.5:** Settings-Page (v0.4.1) ← WIR SIND HIER

#### 🚧 Verbleibende Phasen
10. **Phase 10:** Dead Letters Management (v0.4.2) - NICHT implementiert!
    - Nur Placeholder vorhanden
    - Benötigt: Liste, Actions, Retry/Delete
    
11. **Phase 11:** Dashboard Polish & Performance (v0.5.0) - 1 Chat
    - Performance-Optimierung
    - UI-Verbesserungen
    - Feature-complete Beta
    
12. **Phase 12:** PACS Integration (v0.6.0) - Optional, 2 Chats
    - DICOM C-STORE SCU
    - Network Transfer
    
13. **Phase 13:** Deployment (v0.9.0 → v1.0.0) - 1 Chat
    - MSI Installer
    - Release Pipeline

### Zeitschätzung bis v1.0.0
- Phase 10: 1 Chat
- Phase 11: 1 Chat  
- Phase 12: 2 Chats (optional)
- Phase 13: 1 Chat
- **Gesamt:** 3-5 Chats
    
12. **Phase 11:** Dashboard & Performance (v0.5.0) - 1 Chat
    - Echtzeit-Statistiken
    - Performance-Optimierung
    - UI-Polish
    - Feature-complete Beta
    
13. **Phase 12:** PACS Integration (v0.6.0) - Optional, 2 Chats
    - DICOM C-STORE SCU
    - Network Transfer
    - AE Title Config
    
14. **Phase 13:** Deployment (v0.9.0 → v1.0.0) - 1 Chat
    - MSI Installer (WiX)
    - Single-File Deployment
    - Release Pipeline

### Technologie-Stack (KORRIGIERT)
```
GUI Framework:
- WPF (Windows Presentation Foundation) ← NICHT WinUI 3!
- ModernWpfUI 0.9.9 für modernes Design
- CommunityToolkit.Mvvm 8.2.2
- .NET 8.0 Windows Desktop

Service:
- ASP.NET Core Minimal API
- Windows Service
- System.Text.Json

Processing:
- fo-dicom für DICOM
- MetadataExtractor für EXIF
```

### Zeitschätzung bis v1.0.0
- Phase 9-11: 3 Chats (3 Tage)
- Phase 12: 2 Chats (optional)
- Phase 13: 1 Chat
- **Gesamt:** 4-6 Chats (ca. 1 Woche)

### Meilensteine
- **v0.5.0** - Feature Complete Beta
- **v0.6.0** - PACS Ready (optional)
- **v0.9.0** - Release Candidate
- **v1.0.0** - Production Release

### Entwicklungs-Philosophie
"Sauberer, schöner, ästhetischer und formal korrekter Code für medizinische Software"

## 🚨 Anti-Patterns (Was wir NICHT machen)

### Code-Anti-Patterns
- **KEINE** HTML-formatierten Code-Artefakte (Token-Verschwendung!)
- **KEINE** Marketing-Sprache in Dokumentation
- **KEINE** Version-Dopplungen (v0.0.2, v0.4.0 reichen als Mahnung!)
- **KEINE** Commits ohne Versionsnummer (siehe DICOM-Commit)
- **KEINE** großen Versionssprünge - Babysteps!
- **KEINE** falschen Datumsangaben im CHANGELOG
- **KEIN** WinUI3 Code (wir nutzen WPF mit ModernWpfUI!)
- **KEINE** Annahmen über Placeholder - IMMER Dateigrößen prüfen!

### Kommunikations-Anti-Patterns
- **KEINE** langen Einleitungen ("Das ist eine exzellente Frage...")
- **KEINE** übermäßigen Entschuldigungen
- **KEINE** Token-verschwenderischen Formatierungen
- **KEINE** Wiederholungen von bereits Gesagtem
- **KEINE** Annahmen dass "Placeholder" wirklich Placeholder sind

### Wichtige Lektionen
**IMMER Dateigrößen prüfen!** 
- 1 KB = wahrscheinlich Placeholder
- 8-12 KB = vollständige Implementation!

**IMMER Versionsnummer erhöhen!**
- Jeder Feature-Commit = neue Version
- Babysteps: v0.0.1 → v0.0.2 → v0.0.3
- Keine Duplikate!

## 📝 Standard Prompt-Vorlage für neue Chats

### Option 1: MAGIC WORD (Empfohlen!)
```
1. PROJECT_WISDOM.md hochladen
2. balanced.bat Output hochladen  
3. Sagen: "CAMBRIDGE INIT"
4. Fertig! Ich lege direkt los.
```

### Option 2: Traditionell (falls Magic Word nicht funktioniert)
```
Ich arbeite an CamBridge, einem JPEG zu DICOM Konverter.
© 2025 Claude's Improbably Reliable Software Solutions

Aktueller Stand: v0.4.1
- GUI Framework mit Dashboard ✓
- Service Control (Phase 9) ✓  
- Settings-Page ✓
- Dead Letters: NUR PLACEHOLDER

Nächste Aufgabe: Dead Letters Implementation (v0.4.2)

Tech Stack: .NET 8, WPF/ModernWpfUI, MVVM
Architektur: Enterprise-Level für medizinische Software

[PROJECT_WISDOM.md und balanced.bat Output anhängen]
```

## 🏥 Medizinischer Kontext (WICHTIG!)

### Warum CamBridge KEIN "simples Tool" ist:
- **DICOM-Compliance:** Ein fehlerhaftes DICOM kann Diagnosen beeinflussen
- **24/7 Verfügbarkeit:** Krankenhaus-IT läuft rund um die Uhr
- **Monitoring:** "Blind" laufende Services = Risiko
- **Audit/Compliance:** Nachvollziehbarkeit für MDR/FDA
- **Integration:** PACS, RIS, KIS - alles muss zusammenspielen

### Geplante Erweiterungen (bestätigt):
- **FTP-Server** für automatischen Empfang
- **DICOM C-STORE** für direkten PACS-Versand
- **Erweiterte Settings** für verschiedene Workflows
- **Weitere Services** je nach Bedarf

### Unsere Stärken:
- REST API für Monitoring (Seltenheit in Krankenhaus-IT!)
- Robuste Fehlerbehandlung mit Dead-Letter-Queue
- Erweiterbare Architektur für zukünftige Protokolle
- Enterprise-ready von Tag 1

### Testing-Strategie
- Unit Tests für Business Logic
- Integration Tests für Services  
- UI Tests für kritische Workflows
- Performance Tests vor Major Releases

### Git Commit Format
```
type(scope): Subject line (vX.X.X)

- Detailed change 1
- Detailed change 2
- Additional changes

BREAKING CHANGE: Description (if applicable)
```

Types: feat, fix, docs, style, refactor, test, chore

## 🔄 Update-Protokoll

### Wann PROJECT_WISDOM updaten?
- Nach jeder neuen Erkenntnis
- Bei Version-Releases
- Bei Architektur-Änderungen
- Bei neuen Konventionen
- Bei gefundenen Anti-Patterns

### Versions-Historie (Aus Git-Log)
- **v0.0.1** - 2025-05-30: Initial project structure (fb17be8)
- **v0.0.2** - 2025-05-30: Core domain models (588cf7b) ⚠️
- **v0.0.2** - 2025-05-30: Core domain models (b64ba16) ⚠️ DUPLIKAT!
- **v0.1.0** - 2025-05-30: EXIF extraction with QRBridge (1bff3ef)
- **[KEINE]** - 2025-05-31: DICOM conversion fo-dicom (511bdeb) ⚠️
- **v0.2.0** - 2025-05-31: JSON-based mapping config (cf88f46)
- **v0.3.0** - 2025-05-31: File monitoring pipeline (a6b77bb)
- **v0.3.1** - 2025-05-31: Fix DI singleton/scoped issue (7814cb9)
- **v0.3.2** - 2025-05-31: Dead-letter, notifications, web (0ab9add)
- **v0.4.0** - 2025-06-01: WPF GUI with dashboard (e0e68f1) ⚠️
- **v0.4.0** - 2025-06-01: WPF configuration UI (e806e31) ⚠️ DUPLIKAT!
- **v0.4.1** - 2025-06-01: Settings Page (noch nicht committed)

### Versionierungs-Lektionen
1. **v0.0.2 Duplikat:** Gleich am Anfang passiert
2. **Fehlende Version:** DICOM-Commit ohne Versionsnummer
3. **v0.4.0 Duplikat:** Zwei verschiedene Commit-Messages
4. **Babysteps:** Besser 0.0.1 Schritte als große Sprünge!

### NEUE REGEL: Versionierungs-Disziplin
- IMMER Version erhöhen, auch für kleine Änderungen
- Lieber v0.0.1 → v0.0.2 → v0.0.3 als große Sprünge
- KEINE Duplikate - vor Commit prüfen!
- JEDER Feature-Commit braucht eine Version

### Update-Historie (PROJECT_WISDOM selbst)
- 2025-06-01 12:30: Initial creation
- 2025-06-01 12:35: Added time management section  
- 2025-06-01 12:40: Added anti-patterns and troubleshooting
- 2025-06-01 12:45: Integrated development plan with WPF correction
- 2025-06-01 12:50: Added prompt template and quality goals
- 2025-06-01 12:55: Discovered Phase 9 already implemented
- 2025-06-01 13:00: Added KISS reflection and "Nur für mich (Claude)" section
- 2025-06-01 13:00: Corrected: DeadLetters is NOT implemented
- 2025-06-01 13:05: Paradigmenwechsel: Enterprise-Architektur ist angemessen
- 2025-06-01 13:10: MAGIC WORDS SYSTEM implementiert
- 2025-06-01 13:15: Vollständige Versions-Historie ergänzt
- 2025-06-01 13:20: Git-History integriert - mehr Duplikate entdeckt!
- 2025-06-01 13:25: Exakte Timestamps enthüllen die wahre Geschichte!

## 🏁 Quick Reference

### Aktuelle Version: v0.4.1
### Tatsächlicher Stand: 
- ✅ Service Control (Phase 9) in v0.4.0 erledigt
- ❌ Dead Letters (Phase 10) noch TODO
### Nächste Aufgabe: Dead Letters Implementation (v0.4.2)
### Architektur: Enterprise-Level (und das ist GUT so!)
### Kontext: Medizinische Software mit 0% Fehlertoleranz
### Geschätzte v1.0.0: 3-5 Chats
