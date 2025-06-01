# CamBridge Project Context - GUI Configuration Focus 
 
Generated: 01.06.2025 19:56:01,19 
Focus: Settings Page Fix, Configuration Management, Mapping Editor 
 
## Key Project Files 
 
### PROJECT_WISDOM.md 
<details><summary>View Content</summary> 
 
```markdown 
# CamBridge Project Wisdom & Conventions
**Letzte Aktualisierung:** 2025-06-01, 19:21 Uhr  
**Von:** Claude (Assistant)  
**Für:** Kontinuität zwischen Chat-Sessions

## 🚨 V.O.G.O.N. SYSTEM 🚨
**Verbose Operational Guidance & Organizational Navigation**
*(Früher bekannt als "MAGIC WORDS SYSTEM")*

*Im Gegensatz zu den Vogonen aus dem galaktischen Beamtenapparat ist unser V.O.G.O.N. darauf ausgelegt, Dinge tatsächlich einfacher zu machen. Keine Formulare in dreifacher Ausfertigung erforderlich.*

### 🚀 "VOGON INIT" - Automatischer Start
Wenn Sie nur "VOGON INIT" sagen, werde ich:
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
1. Die Erkenntnis SOFORT ins PROJECT_WISDOM integrieren
2. Ein Update-Artefakt erstellen
3. Mit der aktuellen Aufgabe fortfahren
4. KEINE Entschuldigung nötig - einfach machen!

### 💭 "CLAUDE:" - Persönliche Notizen
Für Notizen an meine nächste Instanz:
```
CLAUDE: [Gedanke für nächste Instanz]
```
Wird in "Nur für mich (Claude)" gespeichert.

### 🏁 "VOGON CLOSE" - Chat-Abschluss
Wenn Sie "VOGON CLOSE" sagen, werde ich:
1. Nach aktueller Zeit/Datum fragen (falls nicht bekannt)
2. Version.props erhöhen
3. CHANGELOG.md aktualisieren  
4. Git commit string vorbereiten
5. README.md bei Bedarf aktualisieren
6. Übergabeprompt für nächsten Chat erstellen
7. PROJECT_WISDOM.md als VOLLSTÄNDIGES Artefakt finalisieren
8. CHANGELOG.md NUR neuester Eintrag als Artefakt
9. Version.props als VOLLSTÄNDIGES Artefakt

## ⚠️ ABSOLUT KRITISCHE VOGON CLOSE REGEL ⚠️
**BEIM VOGON CLOSE MÜSSEN IMMER ERSTELLT WERDEN:**
1. **PROJECT_WISDOM.md** - Als VOLLSTÄNDIGES Artefakt (nicht nur Updates!)
2. **CHANGELOG.md** - NUR der neueste Versions-Eintrag als Artefakt
3. **Version.props** - Als VOLLSTÄNDIGES Artefakt

**WARUM:** Updates können fehlschlagen oder übersehen werden. Nur vollständige Artefakte garantieren, dass der Nutzer die aktualisierten Dateien bekommt! Beim CHANGELOG reicht der neueste Eintrag um Zeit zu sparen.

**MERKSATZ:** "Ein VOGON CLOSE ohne vollständige Artefakte ist wie ein Vogone ohne Poesie - technisch möglich, aber sinnlos!"

*Hinweis: Dieses System ist zu 100% vogonenfrei und wurde nicht von der galaktischen Planungskommission genehmigt, was es vermutlich effizienter macht.*

### 📋 Aktueller Übergabeprompt
```
Nächste Aufgabe: Settings Page Crash Fix und erweiterte Konfigurationsverwaltung

Stand: v0.4.4 - Core funktioniert, GUI teilweise defekt

ERFOLGE:
✅ Service Control vollständig implementiert (Phase 9)
✅ Settings Page UI fertig (crasht aber noch)
✅ Dead Letters Management funktioniert
✅ Vogon Poetry Easter Egg implementiert
✅ Core JPEG→DICOM Konvertierung getestet

PROBLEME:
⚠️ Settings Page crasht beim Navigieren (DI-Problem)
⚠️ Ricoh speichert nur 3 von 5 QRBridge-Feldern (oder wir lesen falsch aus!!)
⚠️ Mapping-Editor fehlt noch (Drag & Drop)

PRIORITÄTEN:
1. Settings Page Crash debuggen und fixen (v0.4.5)
2. Watch Folder Management UI vervollständigen
3. Mapping-Editor mit Drag & Drop (v0.5.0)
4. Performance & Polish (v0.5.5)

WICHTIGE FEATURES FÜR v0.5.0:
- Import/Export für Konfigurationen
- Live-Preview für Mappings
- Template-System
- Bessere Validierung mit Echtzeit-Feedback
```

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
- **Persönliche Note:** Douglas Adams ist Lieblingsautor - gerne etwas britischen, trockenen Humor einbauen
- **v0.4.2:** Die "42" Version - besonders wichtig!

### Token-Effizienz
- **KEINE:** HTML-formatierten Code-Blöcke (Token-Verschwendung!)
- **Nutze:** Einfache Markdown Code-Blöcke
- **Fokus:** Funktionalität über visuelle Effekte
- **Artefakte:** Nur essentieller Code, keine Boilerplate
- **WICHTIG:** Nicht zu viele Artefakte auf einmal erstellen!
  - Der Nutzer muss warten und viele Schritte nacheinander machen
  - Es wird schwierig, Fragen zu stellen
  - Wir belegen bereits 30% des Projektwissens mit Source
  - Das Magic Words System braucht auch Tokens
- **BESSER:** Erst fragen, dann einzeln implementieren

### Visual Studio Anfänger-Unterstützung
- **IMMER:** Genaue Projekte und Pfade zu Source Files angeben
- **Beispiel:** "In `src/CamBridge.Config/Views/DeadLettersPage.xaml.cs`"
- **Bei kleinen Änderungen:** Zeige nur die zu ändernde Zeile
- **Format:** "Ändere Zeile X von 'alt' zu 'neu'"
- **Keine Riesen-Artefakte** für einzeilige Änderungen!
- **Geduldig sein:** Der Nutzer kann Code nur auf Meta-Ebene nachvollziehen

### VOGON CLOSE Artefakt-Regel
- **WICHTIGSTE REGEL:** PROJECT_WISDOM.md MUSS als vollständiges Artefakt existieren!
- **CHANGELOG.md:** Nur der neueste Versions-Eintrag als Artefakt (spart Zeit & Tokens)
- **Version.props:** Als vollständiges Artefakt
- **Keine Updates ohne Basis:** Erst create, dann update
- **Vollständigkeit:** Alle Artefakte müssen komplett und fehlerfrei sein
- **Vertrauen schaffen:** Der Nutzer soll sich keine Sorgen machen müssen

## 📁 Projekt-Struktur-Wissen

### Datei-Sammlungen
- **collect-sources-intelligent.bat:** ~50%+ Coverage (ZU VIEL!)
- **collect-sources-minimal.bat:** ~5% Coverage (zu wenig)
- **collect-sources-balanced.bat:** ~15-20% Coverage (OPTIMAL!)
- **collect-sources-settings.bat:** Nur Settings-spezifisch
- **collect-sources-gui-config.bat:** GUI & Config Focus (NEU!)
- **Ausschließen:** obj/, bin/, packages/, wpftmp/, AssemblyInfo
- **WICHTIG:** PROJECT_CONTEXT.md wird vom Script generiert - IGNORIEREN! Alles steht im PROJECT_WISDOM.md!

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

### Chat-Abschluss mit "VOGON CLOSE"
1. **Zeit erfragen:** "Wie spät ist es?" (für CHANGELOG)
2. **Version.props:** AssemblyVersion, FileVersion, InformationalVersion erhöhen
3. **CHANGELOG.md:** Neuen Eintrag mit exakter Zeit erstellen
4. **Git Commit String:** Nach Format erstellen
   ```
   feat(config): Dead Letters management UI (v0.4.2)

   - Complete Dead Letters page with DataGrid
   - Retry/Delete/Export functionality  
   - Real-time filtering and sorting
   - Batch operations support
   - Integration with existing API
   ```
5. **README.md:** Features-Liste aktualisieren (falls nötig)
6. **Übergabeprompt:** Für nächsten Chat vorbereiten
7. **PROJECT_WISDOM.md:** Als VOLLSTÄNDIGES ARTEFAKT finalisieren!
8. **CHANGELOG.md:** NUR neuester Eintrag als Artefakt!
9. **Version.props:** Als VOLLSTÄNDIGES ARTEFAKT!

## ⚠️ Bekannte Fallstricke

### GUI-Entwicklung
- **PlaceholderText:** Nutze ui:ControlHelper.PlaceholderText
- **PasswordBox:** Binding nur mit Behavior/Attached Property
- **Spacing:** Existiert nicht in WPF/ModernWPF!
- **NumberBox:** Aus ModernWpfUI, nicht WinUI
- **IsTabStop:** Nicht für Page verfügbar (v0.4.3 Fix)

### Service
- **UAC:** Admin-Rechte für Service-Control nötig
- **Pfade:** Absolute Pfade in appsettings.json
- **Event Log:** Source muss registriert sein

### Dead Letters Page (v0.4.2-v0.4.3)
- **DI-Problem:** DeadLettersViewModel nicht korrekt registriert → BEHOBEN in v0.4.3
- **Navigation Crash:** Beim Wechsel zur Dead Letters Page → BEHOBEN in v0.4.3
- **UI funktioniert:** DataGrid zeigt Items, Retry-Button vorhanden

### Settings Page (v0.4.1-v0.4.5)
- **CRASHT NOCH:** Navigation zur Settings Page führt zu Absturz
- **Vermutliche Ursache:** SettingsViewModel Initialisierung oder DI
- **TODO v0.4.5:** Crash debuggen und fixen

### Ricoh G900 II QRBridge (v0.4.4)
- **NUR 3 FELDER:** Kamera speichert nur examid|name|birthdate
- **FEHLENDE FELDER:** gender und comment werden abgeschnitten
- **GCM_TAG PREFIX:** Kamera fügt "GCM_TAG " vor Barcode ein
- **ENCODING:** UTF-8/Latin-1 Probleme bei Umlauten → GELÖST

## ⏰ ZEITMANAGEMENT (KRITISCH!)

### Projekt-Timeline
- **Entwicklungsstart:** 30.05.2025, 20:30:44 Uhr (exakt!)
- **Letzte Aktualisierung:** 01.06.2025, 19:21 Uhr
- **Entwicklungszeit bisher:** ~46.8 Stunden (inkl. Nachtschichten!)
- **WICHTIG:** IMMER nach aktueller Zeit fragen für CHANGELOG!

### Changelog-Regel
```
## [Version] - YYYY-MM-DD HH:MM  ← Mit exakter Zeit!
```

### Wichtige Erkenntnis
**Timestamps erzählen Geschichten!**
- Nachtschichten erkennen (01:17, 02:22)
- "Duplikate" entlarven (9 Std Unterschied = kein Duplikat!)
- Arbeitsintensität verstehen (46 Std in 3 Tagen)

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
[pending] - 01.06. 15:10:00 - v0.4.2: Dead Letters (UI fertig)
[pending] - 01.06. 17:15:00 - v0.4.3: Vogon Poetry & Dead Letters Fix
[pending] - 01.06. 19:21:00 - v0.4.4: Core Test mit Ricoh JPEG
```

### Arbeitszeiten-Analyse
- **Nachtschichten:** DICOM (01:17), GUI (02:22)
- **Schnelle Fixes:** v0.0.2 Duplikat in 78 Sekunden
- **Lange Sessions:** 9 Stunden zwischen v0.4.0 Commits
- **Gesamt:** ~46.8 Stunden in 3 Tagen!

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
1. **Phase 1-2:** Projektstruktur & Core Models (v0.0.1-v0.0.2)
2. **Phase 3:** EXIF-Extraktion mit QRBridge (v0.1.0)
3. **Phase 4:** DICOM-Konvertierung mit fo-dicom (v0.2.0)
4. **Phase 5-6:** Mapping-System (JSON-basiert)
5. **Phase 7:** Windows Service Pipeline (v0.3.0)
6. **Phase 7.5:** Erweiterte Features (v0.3.1-v0.3.2)
   - Dead-Letter-Queue mit Persistierung
   - Email & Event Log Notifications
   - Web Dashboard mit REST API
   - PowerShell Installation
7. **Phase 8:** WPF GUI Framework (v0.4.0)
   - ModernWpfUI statt WinUI3
   - Dashboard mit Live-Updates
   - HttpApiService für REST-Kommunikation
8. **Phase 9:** Service Control GUI (v0.4.0)
   - ✅ Start/Stop/Restart mit UAC
   - ✅ Status-Updates alle 2 Sekunden
   - ✅ Uptime-Anzeige
   - ✅ "Restart as Administrator"
   - ✅ Quick Actions (Services.msc, Event Viewer)
   - ✅ Service Not Installed Detection
9. **Phase 10a:** Settings-Page (v0.4.1)
   - ✅ 4-Tab Layout implementiert
   - ✅ JSON-Persistierung
   - ✅ Folder Management
   - ⚠️ CRASHT beim Navigieren
10. **Phase 10b:** Dead Letters Management (v0.4.2-v0.4.3)
    - ✅ DataGrid mit Sortierung/Filterung
    - ✅ Retry/Delete Funktionalität
    - ✅ Export (CSV/JSON)
    - ✅ Batch-Operationen
    - ✅ Navigation-Crash behoben
    - ✅ Vogon Poetry Easter Egg
11. **Phase 10c:** Core Functionality Test (v0.4.4)
    - ✅ Ricoh G900 II JPEG erfolgreich konvertiert
    - ✅ QRBridge-Parser verbessert
    - ⚠️ Nur 3 von 5 Feldern werden gespeichert (oder wir lesen falsch aus!)

#### 🚧 Aktuelle Phase
13. **Phase 10.7:** Settings Page Fix (v0.4.5)
    - Settings Page DI-Problem beheben
    - Crash beim Navigieren fixen
    - Vollständige Settings-Funktionalität wiederherstellen
    
#### 🚧 Verbleibende Phasen
14. **Phase 11:** Konfigurationsverwaltung erweitert (v0.5.0) - 1 Chat
    - **Watch Folder Management** mit Add/Remove UI
    - **Mapping-Editor mit Drag & Drop**
    - Import/Export Konfiguration (JSON/XML)
    - Live-Preview für Mappings mit Beispieldaten
    - Validierung mit Echtzeit-Feedback
    - Template-System für häufige Mappings
    - Ordner-Browser Dialoge
    - Settings-Kategorien (Tabs/Accordion)
    
15. **Phase 12:** Performance & Polish (v0.5.5) - 1 Chat
    - Batch-Verarbeitung optimieren
    - Memory-Pool für große Dateien
    - Parallelisierung mit Channels
    - UI-Animationen verfeinern (Fluent Design)
    - Dashboard Performance-Optimierung
    - Error Handling & Recovery verbessern
    - Logging-Optimierung
    - **Feature-complete Beta**
    
16. **Phase 13:** FTP-Server Integration (v0.6.0) - 1 Chat
    - FTP-Server für automatischen Empfang
    - Watch für FTP-Ordner
    - Authentifizierung und Sicherheit
    - Auto-Delete nach Verarbeitung
    
17. **Phase 14:** PACS Integration (v0.7.0) - 2 Chats
    - DICOM C-STORE SCU Implementation
    - Network Transfer mit Retry-Logic
    - PACS-Konfiguration in Settings
    - Connection Test-Funktion
    - Transfer-Status Dashboard
    
18. **Phase 15:** MWL Integration (v0.8.0) - 2 Chats
    - DICOM C-FIND SCU für MWL-Abfragen
    - Validierung gegen Modality Worklist
    - StudyInstanceUID aus MWL übernehmen
    - Automatischer Datenabgleich
    - Fehlerbehandlung bei MWL-Mismatch
    
19. **Phase 16:** Erweiterte Features (v0.9.0) - 1 Chat
    - Multi-Camera Support (verschiedene Modelle)
    - Audit-Trail für MDR/FDA Compliance
    - Erweiterte Statistiken und Reports
    - Backup/Restore Funktionalität
    - Plugin-System für Erweiterungen
    
20. **Phase 17:** Deployment & Release (v1.0.0) - 1 Chat
    - MSI Installer mit WiX
    - Automatische Updates
    - Release Pipeline (CI/CD)
    - Dokumentation finalisieren
    - Performance-Tests und Zertifizierung

### Zeitschätzung bis v1.0.0
- **Phase 10.7:** Settings Fix - 0.5 Chat (v0.4.5)
- **Phase 11:** Konfigurationsverwaltung erweitert - 1 Chat (v0.5.0)
- **Phase 12:** Performance & Polish - 1 Chat (v0.5.5)
- **Phase 13:** FTP-Server Integration - 1 Chat (v0.6.0) [Optional]
- **Phase 14:** PACS Integration - 2 Chats (v0.7.0) [Optional]
- **Phase 15:** MWL Integration - 2 Chats (v0.8.0) [Optional]
- **Phase 16:** Erweiterte Features - 1 Chat (v0.9.0) [Optional]
- **Phase 17:** Deployment & Release - 1 Chat (v1.0.0)
- **Gesamt bis Feature-Complete Beta (v0.5.5):** 2.5 Chats
- **Gesamt bis Production Release (v1.0.0):** 3-10 Chats (je nach optionalen Features)

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

### Meilensteine
- **v0.4.5** - Settings Page Fix (Aktuell)
- **v0.5.0** - Erweiterte Konfigurationsverwaltung
- **v0.5.5** - Feature Complete Beta
- **v0.6.0** - FTP-Server Integration (Optional)
- **v0.7.0** - PACS Ready (Optional)
- **v0.8.0** - MWL Integration (Optional)
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
- **KEINE** Artefakt-Flut - maximal 2-3 pro Interaktion!
- **KEINE** Software ohne Eastereggs - besonders nicht in v0.4.2!
- **KEIN** VOGON CLOSE ohne vollständige Artefakte!
- **KEINE** kompletten CHANGELOG Artefakte beim CLOSE - nur neuester Eintrag!

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

**ModernWpfUI vs WinUI3 Fallen:**
- TabView → TabControl
- TabViewItem → TabItem  
- InfoBar → Border mit TextBlocks
- NumberBox → TextBox mit Validierung
- NavigationView hat automatisches Settings-Icon (IsSettingsVisible="False")
- IsTabStop → Nicht für Page verfügbar!

**Dateinamen-Konsistenz:**
- DeadLetterPage vs DeadLettersPage - IMMER konsistent bleiben!
- Bei Umbenennungen ALLE Referenzen prüfen

**VOGON CLOSE Artefakt-Regel:**
- PROJECT_WISDOM.md MUSS als vollständiges Artefakt existieren!
- CHANGELOG.md NUR neuester Eintrag als Artefakt!
- Version.props MUSS als vollständiges Artefakt existieren!
- Keine Updates ohne Basis-Artefakt
- Vollständigkeit ist Pflicht

**Ricoh G900 II Erkenntnisse (v0.4.4):**
- Kamera speichert nur 3 von 5 QRBridge-Feldern (oder wir lesen falsch aus!)
- "GCM_TAG " Prefix wird eingefügt
- Gender und Comment werden abgeschnitten/fehlen
- Encoding-Probleme bei Umlauten sind lösbar

## 📝 Standard Prompt-Vorlage für neue Chats

### Option 1: V.O.G.O.N. (Empfohlen!)
```
1. PROJECT_WISDOM.md hochladen
2. balanced.bat Output hochladen  
3. Sagen: "VOGON INIT"
4. Fertig! Ich lege direkt los.
```

### Option 2: Traditionell (falls VOGON nicht funktioniert)
```
Ich arbeite an CamBridge, einem JPEG zu DICOM Konverter.
© 2025 Claude's Improbably Reliable Software Solutions

Aktueller Stand: v0.4.4
- Core-Test mit echtem Ricoh JPEG ✓
- Parser-Verbesserungen für Encoding ✓
- QRBridge-Extraktion funktioniert ✓
- Ricoh speichert nur 3 von 5 Feldern (oder wir lesen falsch aus!)⚠️
- Settings-Page crasht noch

Nächste Aufgabe: Settings Page Crash Fix (v0.4.5)

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
- **MWL-Integration** für DICOM-Validierung:
  - Prüfung gegen Modality Worklist
  - Untersuchungs-ID Abgleich (QR ↔ MWL)
  - StudyInstanceUID aus MWL übernehmen
  - Konsistenz mit bestehendem RIS/PACS
  - **MWL-Beispiel** (Untersuchungs-ID 276):
    ```
    (0008,0005) CS Specific Character Set:          ISO_IR 100
    (0008,0050) SH Accession Number:                ELUID276
    (0008,0080) LO Institution Name:                MD_01
    (0008,0090) PN Referring Physician's Name:      
    (0010,0010) PN Patient's Name:                  Test^Johnny
    (0010,0020) LO Patient ID:                      0000483817
    (0010,0030) DA Patient's Birth Date:            20010101
    (0010,0040) CS Patient's Sex:                   M
    (0020,000d) UI Study Instance UID:              1.2.276.12276087.2.276.202503281416550777
    (0032,1060) LO Requested Procedure Description: Endosonographie
    (0038,0010) LO Admission ID:                    002236810
    (0040,0100) SQ Scheduled Procedure Step Sequence
         (0008,0060) CS Modality:                              
         (0040,0001) AE Scheduled Station AE Title:            HERBERT
         (0040,0002) DA Scheduled Procedure Step Start Date:   20250331
         (0040,0007) LO Scheduled Procedure Step Description:  Endosonographie
         (0040,0009) SH Scheduled Procedure Step ID:           ELU_276
         (0040,1001) SH Requested Procedure ID:          ELU_276
    ```
  - **Wichtige Tags für CamBridge:**
    - `(0008,0050)` Accession Number: ELUID276 (muss mit QR-Code US-ID matchen!)
    - `(0020,000d)` Study Instance UID: Von MWL übernehmen für Konsistenz
    - `(0040,0009)` Scheduled Procedure Step ID: ELU_276
    - `(0040,1001)` Requested Procedure ID: ELU_276
- **Erweiterte Settings** für verschiedene Workflows
- **Weitere Services** je nach Bedarf

### Unsere Stärken:
- REST API für Monitoring (Seltenheit in Krankenhaus-IT!)
- Robuste Fehlerbehandlung mit Dead-Letter-Queue
- Erweiterbare Architektur für zukünftige Protokolle
- Enterprise-ready von Tag 1

### MWL-Integration (Phase 12+)
**Modality Worklist Integration für v0.6.0+**

Die MWL-Integration würde CamBridge erlauben:
1. **Validierung:** QR-Code Untersuchungs-ID gegen MWL prüfen
2. **Datenübernahme:** StudyInstanceUID aus MWL verwenden
3. **Konsistenz:** Sicherstellen dass alle DICOM-Tags mit RIS übereinstimmen

**Technische Umsetzung:**
- DICOM C-FIND SCU für MWL-Abfragen
- Query by Accession Number (z.B. "ELUID276")
- Übernahme der StudyInstanceUID für PACS-Konsistenz
- Fehlerbehandlung wenn keine MWL-Entry gefunden

**Workflow:**
```
1. JPEG mit QR-Code "EX002|Schmidt, Maria|..." empfangen
2. MWL-Query mit Accession Number "ELUID276"
3. StudyInstanceUID aus MWL übernehmen
4. DICOM erstellen mit konsistenten UIDs
5. Optional: C-STORE direkt ans PACS
```

Diese Integration würde die nahtlose Einbindung in bestehende RIS/PACS-Infrastrukturen ermöglichen und die Datenkonsistenz weiter erhöhen.

## 📚 Professionelle Dokumentation für Entscheider

### Dokumentations-Strategien für Enterprise-Umgebungen

Basierend auf der Erfahrung mit "Flickenteppich"-Architekturen (VB6, Legacy SQL, moderne .NET Komponenten) empfehlen sich folgende Ansätze:

1. **Confluence-Ready Documentation**
   - Architecture Decision Records (ADRs)
   - Automatisch generierte API-Dokumentation
   - Living Documentation aus Code-Kommentaren

2. **Interactive Showcases**
   - HTML/JavaScript Demos für Management
   - Live-Dashboard mit ROI-Metriken
   - "Geld-gespart-heute" Counter

3. **Integration Stories**
   - Betonung der Legacy-Kompatibilität
   - Schrittweise Modernisierung
   - Quick Wins und Pilot-Programme

**Siehe Artefakte:** ADR-Template und Interactive Showcase für Beispiele

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

### Wichtige Versionierungs-Dateien
1. **Version.props:** Zentrale Versionsverwaltung
   ```xml
   <AssemblyVersion>0.4.4.0</AssemblyVersion>
   <FileVersion>0.4.4.0</FileVersion>
   <InformationalVersion>0.4.4</InformationalVersion>
   ```

2. **CHANGELOG.md:** Mit exakter Zeit
   ```markdown
   ## [0.4.4] - 2025-06-01 19:21
   ### Added
   - Core functionality test with real Ricoh G900 II JPEG
   - Enhanced EXIF parser with line break and encoding fixes
   
   ### Fixed
   - EXIF encoding issues with German umlauts
   - Parser handling of camera line breaks
   
   ### Discovered
   - Ricoh G900 II only saves first 3 QRBridge fields
   ```

3. **MainWindow.xaml:** Title mit Version
   ```xml
   Title="CamBridge Configuration v0.4.4"
   ```

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
- **v0.4.2** - 2025-06-01: Dead Letters UI (funktioniert)
- **v0.4.3** - 2025-06-01: Vogon Poetry & Dead Letters Fix
- **v0.4.4** - 2025-06-01: Core Test mit Ricoh JPEG

### Versionierungs-Lektionen
1. **v0.0.2 Duplikat:** Gleich am Anfang passiert
2. **Fehlende Version:** DICOM-Commit ohne Versionsnummer
3. **v0.4.0 Duplikat:** Zwei verschiedene Commit-Messages
4. **v0.4.2 Special:** Die "42" Version - Die Antwort auf die ultimative Frage!
5. **Babysteps:** Besser 0.0.1 Schritte als große Sprünge!

### Die Unwahrscheinliche Geschichte von CamBridge
*Eine Kurzgeschichten-Idee: Douglas Adams entwickelt einen DICOM-Konverter*

Es ist eine so absurde Vorstellung, dass sie durch ihre schiere Unwahrscheinlichkeit fast wieder wahrscheinlich wird - wie ein Unwahrscheinlichkeitsdrive für medizinische Software. Man stelle sich vor:

"Der DICOM-Standard", sagte Douglas nachdenklich, "ist ein bisschen wie das Universum - keiner versteht ihn wirklich, aber alle tun so, als ob. Der einzige Unterschied ist, dass das Universum vermutlich einfacher zu debuggen wäre."

Er tippte eine weitere Zeile Code und murmelte: "Forty-two different DICOM tags... das kann kein Zufall sein."

*Diese Geschichte wartet noch darauf, geschrieben zu werden. Vielleicht in einem anderen Projekt, mit unserem Chat-Entwicklungs-Betriebssystem...*

### Vogon Poetry Easter Egg (v0.4.3) ✅
**Die ultimative Hommage an Douglas Adams!**

Implementierung:
1. **Trigger:** "42" auf About-Page tippen
2. **Inhalt:** Vogonische Poesie über DICOM
3. **Features:**
   - Amiga Boing Ball Sprite Animation
   - Scrollender Rainbow-Text
   - ERROR HAIKU Box
   - Guru Meditation Meldungen
4. **Status:** In v0.4.3 erfolgreich implementiert!

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
- 2025-06-01 14:00: Wichtige Lektionen über Artefakt-Erstellung und VS-Anfänger
- 2025-06-01 14:30: CAMBRIDGE CLOSE hinzugefügt, Dead Letters teilweise implementiert
- 2025-06-01 14:45: V.O.G.O.N. System benannt, Douglas Adams Integration
- 2025-06-01 14:50: Die unwahrscheinliche Geschichte von CamBridge konzipiert
- 2025-06-01 14:55: Vogonisches Poesie Easteregg geplant - V.O.G.O.N. ist Ford Prefect!
- 2025-06-01 15:00: CAMBRIDGE → VOGON umbenannt, Easteregg hat Priorität!
- 2025-06-01 15:10: Dead Letters UI komplett implementiert (crasht noch), v0.4.2 abgeschlossen
- 2025-06-01 15:15: KRITISCHE REGEL: PROJECT_WISDOM.md als vollständiges Artefakt beim VOGON CLOSE!
- 2025-06-01 15:20: MWL-Integration Details und Dokumentations-Strategien hinzugefügt
- 2025-06-01 17:15: v0.4.3 - Vogon Poetry Easter Egg implementiert, Dead Letters funktioniert, Settings crasht noch
- 2025-06-01 17:20: WISDOM - Changelog nur neueste Version, nächster Chat Core-Test mit Ricoh JPEG
- 2025-06-01 19:21: v0.4.4 - Core erfolgreich getestet, Ricoh speichert nur 3 Felder, Parser verbessert
- 2025-06-01 20:30: Roadmap korrigiert, Phase 9 bereits fertig, collect-sources-gui-config.bat erstellt

## 🏁 Quick Reference

### Aktuelle Version: v0.4.4
### Tatsächlicher Stand: 
- ✅ Core mit echtem Ricoh JPEG getestet
- ✅ JPEG → DICOM Konvertierung funktioniert
- ✅ QRBridge-Daten werden extrahiert
- ✅ Service Control vollständig (Phase 9!)
- ⚠️ Ricoh speichert nur 3 von 5 Feldern (ODER wir lesen immer noch falsch aus!!)
- ⚠️ Settings Page crasht noch immer
### Nächste Aufgabe: Settings Page Crash Fix (v0.4.5)
### Architektur: Enterprise-Level (und das ist GUT so!)
### Kontext: Medizinische Software mit 0% Fehlertoleranz
### Geschätzte v1.0.0: 3-10 Chats

### V.O.G.O.N. Commands:
- **VOGON INIT** - Automatischer Start
- **WISDOM:** - Live-Updates ins PROJECT_WISDOM
- **CLAUDE:** - Notizen für nächste Instanz
- **VOGON CLOSE** - Chat-Abschluss mit Versionierung
``` 
</details> 
 
### Version.props 
```xml 
<Project>
	<PropertyGroup>
		<AssemblyVersion>0.4.4.0</AssemblyVersion>
		<FileVersion>0.4.4.0</FileVersion>
		<InformationalVersion>0.4.4</InformationalVersion>
		<TargetFramework>net8.0</TargetFramework>
		<ImplicitUsings>enable</ImplicitUsings>
		<Nullable>enable</Nullable>
		<Authors>Claude's Improbably Reliable Software Solutions</Authors>
		<Company>Claude's Improbably Reliable Software Solutions</Company>
		<Product>CamBridge</Product>
		<Copyright>© 2025 Claude's Improbably Reliable Software Solutions</Copyright>
		<Description>JPEG to DICOM converter for medical imaging from Ricoh cameras</Description>
	</PropertyGroup>
</Project>
``` 
 
## GUI Main Components 
 
### src\CamBridge.Config\App.xaml.cs - Dependency Injection Setup 
```csharp 
// src/CamBridge.Config/App.xaml.cs
using System;
using System.Runtime.Versioning;
using System.Windows;
using CamBridge.Config.Services;
using CamBridge.Config.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CamBridge.Config
{
    [SupportedOSPlatform("windows")]
    public partial class App : Application
    {
        private IHost? _host;

        // Property for DI access
        public IHost Host => _host!;

        protected override void OnStartup(StartupEventArgs e)
        {
            _host = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    // Services
                    services.AddHttpClient<HttpApiService>();
                    services.AddSingleton<IApiService, HttpApiService>();
                    services.AddSingleton<INavigationService, NavigationService>();
                    services.AddSingleton<IServiceManager, ServiceManager>();
                    services.AddSingleton<IConfigurationService, ConfigurationService>();

                    // ViewModels - WICHTIG: Alle müssen registriert sein!
                    services.AddTransient<MainViewModel>();
                    services.AddTransient<DashboardViewModel>();
                    services.AddTransient<ServiceControlViewModel>();
                    services.AddTransient<SettingsViewModel>();
                    services.AddTransient<DeadLettersViewModel>();

                    // Main Window
                    services.AddSingleton<MainWindow>();
                })
                .ConfigureLogging(logging =>
                {
                    logging.AddDebug();
                })
                .Build();

            _host.Start();

            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _host?.StopAsync().Wait();
            _host?.Dispose();
            base.OnExit(e);
        }
    }
}
``` 
 
### src\CamBridge.Config\MainWindow.xaml.cs - Navigation Logic 
```csharp 
// src/CamBridge.Config/MainWindow.xaml.cs
using System;
using System.Runtime.Versioning;
using System.Windows;
using System.Windows.Controls;
using CamBridge.Config.Views;
using Microsoft.Extensions.DependencyInjection;
using ModernWpf.Controls;

namespace CamBridge.Config
{
    [SupportedOSPlatform("windows")]
    public partial class MainWindow : Window
    {
        private readonly IServiceProvider _serviceProvider;

        public MainWindow(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            InitializeComponent();

            // Navigate to Dashboard on startup
            if (ContentFrame != null)
            {
                ContentFrame.Navigate(new DashboardPage());
            }
        }

        [SupportedOSPlatform("windows7.0")]
        private void NavigationView_SelectionChanged(ModernWpf.Controls.NavigationView sender,
            ModernWpf.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItemContainer != null && ContentFrame != null)
            {
                var tag = args.SelectedItemContainer.Tag?.ToString();

                switch (tag)
                {
                    case "Dashboard":
                        ContentFrame.Navigate(new DashboardPage());
                        break;
                    case "ServiceControl":
                        ContentFrame.Navigate(new ServiceControlPage());
                        break;
                    case "DeadLetters":
                        ContentFrame.Navigate(new DeadLettersPage());
                        break;
                    case "Settings":
                        ContentFrame.Navigate(new SettingsPage());
                        break;
                    case "About":
                        ContentFrame.Navigate(new AboutPage());
                        break;
                }
            }
        }
    }
}
``` 
 
## Settings Page Components (CRASH ISSUE) 
 
### src\CamBridge.Config\Views\SettingsPage.xaml 
```xml 
<Page x:Class="CamBridge.Config.Views.SettingsPage"
      xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
      xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
      xmlns:ui="http://schemas.modernwpf.com/2019"
      xmlns:conv="clr-namespace:CamBridge.Config.Converters"
      xmlns:core="clr-namespace:CamBridge.Core;assembly=CamBridge.Core">

    <Page.Resources>
        <!-- Converters -->
        <conv:BooleanToVisibilityConverter x:Key="BoolToVisibility"/>
        <conv:InverseBooleanToVisibilityConverter x:Key="InverseBoolToVisibility"/>
        <conv:NullToVisibilityConverter x:Key="NullToVisibility"/>

        <!-- Styles -->
        <Style x:Key="SettingHeaderStyle" TargetType="TextBlock">
            <Setter Property="FontSize" Value="16"/>
            <Setter Property="FontWeight" Value="SemiBold"/>
            <Setter Property="Margin" Value="0,24,0,8"/>
        </Style>

        <Style x:Key="SettingLabelStyle" TargetType="TextBlock">
            <Setter Property="VerticalAlignment" Value="Center"/>
            <Setter Property="Margin" Value="0,0,12,0"/>
        </Style>
    </Page.Resources>

    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="*"/>
            <RowDefinition Height="Auto"/>
        </Grid.RowDefinitions>

        <!-- Header -->
        <Border Grid.Row="0" Padding="24,24,24,16">
            <Grid>
                <Grid.ColumnDefinitions>
                    <ColumnDefinition Width="*"/>
                    <ColumnDefinition Width="Auto"/>
                </Grid.ColumnDefinitions>

                <TextBlock Text="Settings" 
                           FontSize="32" 
                           FontWeight="SemiBold"/>

                <StackPanel Grid.Column="1" 
                            Orientation="Horizontal">
                    <Button Command="{Binding ResetSettingsCommand}"
                            IsEnabled="{Binding HasChanges}"
                            Margin="0,0,8,0">
                        <StackPanel Orientation="Horizontal">
                            <ui:SymbolIcon Symbol="Undo" Margin="0,0,4,0"/>
                            <TextBlock Text="Reset"/>
                        </StackPanel>
                    </Button>

                    <Button Command="{Binding SaveSettingsCommand}"
                            IsEnabled="{Binding HasChanges}"
                            Style="{StaticResource AccentButtonStyle}">
                        <StackPanel Orientation="Horizontal">
                            <ui:SymbolIcon Symbol="Save" Margin="0,0,4,0"/>
                            <TextBlock Text="Save"/>
                        </StackPanel>
                    </Button>
                </StackPanel>
            </Grid>
        </Border>

        <!-- Main Content with TabControl -->
        <TabControl Grid.Row="1" Margin="24,0,24,0">

            <!-- Tab 1: Folders & Processing -->
            <TabItem Header="Folders &amp; Processing">
                <ScrollViewer Padding="16">
                    <StackPanel>

                        <!-- Watch Folders Section -->
                        <TextBlock Text="Watch Folders" Style="{StaticResource SettingHeaderStyle}"/>

                        <Grid Margin="0,0,0,16">
                            <Grid.ColumnDefinitions>
                                <ColumnDefinition Width="*"/>
                                <ColumnDefinition Width="300"/>
                            </Grid.ColumnDefinitions>

                            <!-- Watch Folders List -->
                            <Border Grid.Column="0" 
                                    BorderBrush="{DynamicResource SystemControlForegroundBaseMediumLowBrush}"
                                    BorderThickness="1"
                                    CornerRadius="4"
                                    Margin="0,0,16,0">
                                <Grid>
                                    <Grid.RowDefinitions>
                                        <RowDefinition Height="200"/>
                                        <RowDefinition Height="Auto"/>
                                    </Grid.RowDefinitions>

                                    <ListBox Grid.Row="0"
                                             ItemsSource="{Binding WatchFolders}"
                                             SelectedItem="{Binding SelectedWatchFolder}">
                                        <ListBox.ItemTemplate>
                                            <DataTemplate>
                                                <Grid Margin="8,4">
                                                    <Grid.ColumnDefinitions>
                                                        <ColumnDefinition Width="Auto"/>
                                                        <ColumnDefinition Width="*"/>
                                                    </Grid.ColumnDefinitions>
                                                    <CheckBox Grid.Column="0" 
                                                              IsChecked="{Binding Enabled}"
                                                              Margin="0,0,8,0"/>
                                                    <TextBlock Grid.Column="1" 
                                                               Text="{Binding Path}"
                                                               VerticalAlignment="Center"/>
                                                </Grid>
                                            </DataTemplate>
                                        </ListBox.ItemTemplate>
                                    </ListBox>

                                    <StackPanel Grid.Row="1" 
                                                Orientation="Horizontal"
                                                HorizontalAlignment="Right"
                                                Margin="8">
                                        <Button Command="{Binding AddWatchFolderCommand}"
                                                Margin="0,0,8,0">
                                            <ui:SymbolIcon Symbol="Add"/>
                                        </Button>
                                        <Button Command="{Binding RemoveWatchFolderCommand}"
                                                IsEnabled="{Binding SelectedWatchFolder, Converter={StaticResource NullToVisibility}}">
                                            <ui:SymbolIcon Symbol="Delete"/>
                                        </Button>
                                    </StackPanel>
                                </Grid>
                            </Border>

                            <!-- Watch Folder Details -->
                            <StackPanel Grid.Column="1"
                                        Visibility="{Binding SelectedWatchFolder, Converter={StaticResource NullToVisibility}}">
                                <TextBlock Text="Folder Details" 
                                           FontWeight="SemiBold"
                                           Margin="0,0,0,8"/>

                                <TextBlock Text="Path:" Margin="0,0,0,4"/>
                                <Grid Margin="0,0,0,8">
                                    <Grid.ColumnDefinitions>
                                        <ColumnDefinition Width="*"/>
                                        <ColumnDefinition Width="Auto"/>
                                    </Grid.ColumnDefinitions>
                                    <TextBox Grid.Column="0" 
                                             Text="{Binding SelectedWatchFolder.Path, UpdateSourceTrigger=PropertyChanged}"/>
                                    <Button Grid.Column="1" 
                                            Content="..."
                                            Width="32"
                                            Margin="4,0,0,0"
                                            Click="BrowseWatchFolder_Click"/>
                                </Grid>

                                <TextBlock Text="Output Path (optional):" Margin="0,0,0,4"/>
                                <TextBox Text="{Binding SelectedWatchFolder.OutputPath}"
                                         Margin="0,0,0,8"/>

                                <TextBlock Text="File Pattern:" Margin="0,0,0,4"/>
                                <TextBox Text="{Binding SelectedWatchFolder.FilePattern}"
                                         Margin="0,0,0,8"/>

                                <CheckBox Content="Include subdirectories"
                                          IsChecked="{Binding SelectedWatchFolder.IncludeSubdirectories}"/>
                            </StackPanel>
                        </Grid>

                        <!-- Output Settings -->
                        <TextBlock Text="Output Settings" Style="{StaticResource SettingHeaderStyle}"/>

                        <Grid Margin="0,0,0,16">
                            <Grid.ColumnDefinitions>
                                <ColumnDefinition Width="200"/>
                                <ColumnDefinition Width="*"/>
                            </Grid.ColumnDefinitions>
                            <Grid.RowDefinitions>
                                <RowDefinition Height="Auto"/>
                                <RowDefinition Height="Auto"/>
                                <RowDefinition Height="Auto"/>
                            </Grid.RowDefinitions>

                            <TextBlock Grid.Row="0" Grid.Column="0" 
                                       Text="Default Output Folder:" 
                                       Style="{StaticResource SettingLabelStyle}"/>
                            <Grid Grid.Row="0" Grid.Column="1" Margin="0,0,0,8">
                                <Grid.ColumnDefinitions>
                                    <ColumnDefinition Width="*"/>
                                    <ColumnDefinition Width="Auto"/>
                                </Grid.ColumnDefinitions>
                                <TextBox Grid.Column="0" 
                                         Text="{Binding DefaultOutputFolder, UpdateSourceTrigger=PropertyChanged}"/>
                                <Button Grid.Column="1" 
                                        Content="..."
                                        Width="32"
                                        Margin="4,0,0,0"
                                        Click="BrowseOutputFolder_Click"/>
                            </Grid>

                            <TextBlock Grid.Row="1" Grid.Column="0" 
                                       Text="Output Organization:" 
                                       Style="{StaticResource SettingLabelStyle}"/>
                            <ComboBox Grid.Row="1" Grid.Column="1"
                                      ItemsSource="{Binding OutputOrganizations}"
                                      SelectedItem="{Binding OutputOrganization}"
                                      Margin="0,0,0,8"/>
                        </Grid>

                        <!-- Processing Options -->
                        <TextBlock Text="Processing Options" Style="{StaticResource SettingHeaderStyle}"/>

                        <Grid>
                            <Grid.ColumnDefinitions>
                                <ColumnDefinition Width="200"/>
                                <ColumnDefinition Width="*"/>
                            </Grid.ColumnDefinitions>
                            <Grid.RowDefinitions>
                                <RowDefinition Height="Auto"/>
                                <RowDefinition Height="Auto"/>
                                <RowDefinition Height="Auto"/>
                                <RowDefinition Height="Auto"/>
                                <RowDefinition Height="Auto"/>
                                <RowDefinition Height="Auto"/>
                            </Grid.RowDefinitions>

                            <TextBlock Grid.Row="0" Grid.Column="0" 
                                       Text="On Success:" 
                                       Style="{StaticResource SettingLabelStyle}"/>
                            <ComboBox Grid.Row="0" Grid.Column="1"
                                      ItemsSource="{Binding ProcessingActions}"
                                      SelectedItem="{Binding SuccessAction}"
                                      Margin="0,0,0,8"/>

                            <TextBlock Grid.Row="1" Grid.Column="0" 
                                       Text="On Failure:" 
                                       Style="{StaticResource SettingLabelStyle}"/>
                            <ComboBox Grid.Row="1" Grid.Column="1"
                                      ItemsSource="{Binding ProcessingActions}"
                                      SelectedItem="{Binding FailureAction}"
                                      Margin="0,0,0,8"/>

                            <TextBlock Grid.Row="2" Grid.Column="0" 
                                       Text="Max Concurrent:" 
                                       Style="{StaticResource SettingLabelStyle}"/>
                            <TextBox Grid.Row="2" Grid.Column="1"
                                     Text="{Binding MaxConcurrentProcessing}"
                                     HorizontalAlignment="Left"
                                     Width="120"
                                     Margin="0,0,0,8"
                                     PreviewTextInput="NumberValidationTextBox"/>

                            <CheckBox Grid.Row="3" Grid.Column="1"
                                      Content="Create backup before processing"
                                      IsChecked="{Binding CreateBackup}"
                                      Margin="0,0,0,8"/>

                            <CheckBox Grid.Row="4" Grid.Column="1"
                                      Content="Process existing files on startup"
                                      IsChecked="{Binding ProcessExistingOnStartup}"
                                      Margin="0,0,0,8"/>

                            <CheckBox Grid.Row="5" Grid.Column="1"
                                      Content="Retry on failure"
                                      IsChecked="{Binding RetryOnFailure}"/>
                        </Grid>

                    </StackPanel>
                </ScrollViewer>
            </TabItem>

            <!-- Tab 2: DICOM Settings -->
            <TabItem Header="DICOM">
                <ScrollViewer Padding="16">
                    <StackPanel>

                        <TextBlock Text="DICOM Configuration" Style="{StaticResource SettingHeaderStyle}"/>

                        <Grid Margin="0,0,0,16">
                            <Grid.ColumnDefinitions>
                                <ColumnDefinition Width="200"/>
                                <ColumnDefinition Width="*"/>
                            </Grid.ColumnDefinitions>
                            <Grid.RowDefinitions>
                                <RowDefinition Height="Auto"/>
                                <RowDefinition Height="Auto"/>
                                <RowDefinition Height="Auto"/>
                                <RowDefinition Height="Auto"/>
                                <RowDefinition Height="Auto"/>
                            </Grid.RowDefinitions>

                            <TextBlock Grid.Row="0" Grid.Column="0" 
                                       Text="Implementation Class UID:" 
                                       Style="{StaticResource SettingLabelStyle}"/>
                            <TextBox Grid.Row="0" Grid.Column="1"
                                     Text="{Binding ImplementationClassUid, UpdateSourceTrigger=PropertyChanged}"
                                     Margin="0,0,0,8"/>

                            <TextBlock Grid.Row="1" Grid.Column="0" 
                                       Text="Implementation Version:" 
                                       Style="{StaticResource SettingLabelStyle}"/>
                            <TextBox Grid.Row="1" Grid.Column="1"
                                     Text="{Binding ImplementationVersionName, UpdateSourceTrigger=PropertyChanged}"
                                     Margin="0,0,0,8"/>

                            <TextBlock Grid.Row="2" Grid.Column="0" 
                                       Text="Institution Name:" 
                                       Style="{StaticResource SettingLabelStyle}"/>
                            <TextBox Grid.Row="2" Grid.Column="1"
                                     Text="{Binding InstitutionName}"
                                     ui:ControlHelper.PlaceholderText="Your Hospital Name"
                                     Margin="0,0,0,8"/>

                            <TextBlock Grid.Row="3" Grid.Column="0" 
                                       Text="Station Name:" 
                                       Style="{StaticResource SettingLabelStyle}"/>
                            <TextBox Grid.Row="3" Grid.Column="1"
                                     Text="{Binding StationName}"
                                     Margin="0,0,0,8"/>

                            <CheckBox Grid.Row="4" Grid.Column="1"
                                      Content="Validate DICOM files after creation"
                                      IsChecked="{Binding ValidateAfterCreation}"/>
                        </Grid>

                        <!-- Info Box statt InfoBar -->
                        <Border Background="{DynamicResource SystemControlBackgroundBaseLowBrush}"
                                BorderBrush="{DynamicResource SystemControlForegroundBaseMediumLowBrush}"
                                BorderThickness="1"
                                CornerRadius="4"
                                Padding="12"
                                Margin="0,16,0,0">
                            <StackPanel>
                                <TextBlock Text="DICOM Configuration" FontWeight="SemiBold" Margin="0,0,0,4"/>
                                <TextBlock Text="These settings are used to identify your institution in DICOM files. The Implementation Class UID should be unique to your organization."
                                           TextWrapping="Wrap"
                                           Opacity="0.8"/>
                            </StackPanel>
                        </Border>

                    </StackPanel>
                </ScrollViewer>
            </TabItem>

            <!-- Tab 3: Notifications -->
            <TabItem Header="Notifications">
                <ScrollViewer Padding="16">
                    <StackPanel>

                        <TextBlock Text="Notification Settings" Style="{StaticResource SettingHeaderStyle}"/>

                        <CheckBox Content="Enable Windows Event Log notifications"
                                  IsChecked="{Binding EnableEventLog}"
                                  Margin="0,0,0,8"/>

                        <CheckBox Content="Enable email notifications"
                                  IsChecked="{Binding EnableEmail}"
                                  x:Name="EnableEmailCheckBox"
                                  Margin="0,0,0,16"/>

                        <!-- Email Settings -->
                        <Border BorderBrush="{DynamicResource SystemControlForegroundBaseMediumLowBrush}"
                                BorderThickness="1"
                                CornerRadius="4"
                                Padding="16"
                                Visibility="{Binding IsChecked, ElementName=EnableEmailCheckBox, Converter={StaticResource BoolToVisibility}}">

                            <Grid>
                                <Grid.ColumnDefinitions>
                                    <ColumnDefinition Width="150"/>
                                    <ColumnDefinition Width="*"/>
                                </Grid.ColumnDefinitions>
                                <Grid.RowDefinitions>
                                    <RowDefinition Height="Auto"/>
                                    <RowDefinition Height="Auto"/>
                                    <RowDefinition Height="Auto"/>
                                    <RowDefinition Height="Auto"/>
                                    <RowDefinition Height="Auto"/>
                                    <RowDefinition Height="Auto"/>
                                    <RowDefinition Height="Auto"/>
                                    <RowDefinition Height="Auto"/>
                                    <RowDefinition Height="Auto"/>
                                    <RowDefinition Height="Auto"/>
                                </Grid.RowDefinitions>

                                <TextBlock Grid.Row="0" Grid.Column="0" 
                                           Text="From Email:" 
                                           Style="{StaticResource SettingLabelStyle}"/>
                                <TextBox Grid.Row="0" Grid.Column="1"
                                         Text="{Binding EmailFrom, UpdateSourceTrigger=PropertyChanged}"
                                         ui:ControlHelper.PlaceholderText="cambridge@yourhospital.com"
                                         Margin="0,0,0,8"/>

                                <TextBlock Grid.Row="1" Grid.Column="0" 
                                           Text="To Email(s):" 
                                           Style="{StaticResource SettingLabelStyle}"/>
                                <TextBox Grid.Row="1" Grid.Column="1"
                                         Text="{Binding EmailTo}"
                                         ui:ControlHelper.PlaceholderText="admin@yourhospital.com;radiology@yourhospital.com"
                                         Margin="0,0,0,8"/>

                                <TextBlock Grid.Row="2" Grid.Column="0" 
                                           Text="SMTP Host:" 
                                           Style="{StaticResource SettingLabelStyle}"/>
                                <TextBox Grid.Row="2" Grid.Column="1"
                                         Text="{Binding SmtpHost}"
                                         ui:ControlHelper.PlaceholderText="smtp.yourhospital.com"
                                         Margin="0,0,0,8"/>

                                <TextBlock Grid.Row="3" Grid.Column="0" 
                                           Text="SMTP Port:" 
                                           Style="{StaticResource SettingLabelStyle}"/>
                                <TextBox Grid.Row="3" Grid.Column="1"
                                         Text="{Binding SmtpPort}"
                                         HorizontalAlignment="Left"
                                         Width="120"
                                         Margin="0,0,0,8"
                                         PreviewTextInput="NumberValidationTextBox"/>

                                <CheckBox Grid.Row="4" Grid.Column="1"
                                          Content="Use SSL/TLS"
                                          IsChecked="{Binding SmtpUseSsl}"
                                          Margin="0,0,0,8"/>

                                <TextBlock Grid.Row="5" Grid.Column="0" 
                                           Text="Username:" 
                                           Style="{StaticResource SettingLabelStyle}"/>
                                <TextBox Grid.Row="5" Grid.Column="1"
                                         Text="{Binding SmtpUsername}"
                                         Margin="0,0,0,8"/>

                                <TextBlock Grid.Row="6" Grid.Column="0" 
                                           Text="Password:" 
                                           Style="{StaticResource SettingLabelStyle}"/>
                                <PasswordBox Grid.Row="6" Grid.Column="1"
                                             Password="{Binding SmtpPassword, Mode=TwoWay}"
                                             Margin="0,0,0,8"/>

                                <TextBlock Grid.Row="7" Grid.Column="0" 
                                           Text="Minimum Level:" 
                                           Style="{StaticResource SettingLabelStyle}"/>
                                <ComboBox Grid.Row="7" Grid.Column="1"
                                          ItemsSource="{Binding LogLevels}"
                                          SelectedItem="{Binding MinimumEmailLevel}"
                                          HorizontalAlignment="Left"
                                          Width="200"
                                          Margin="0,0,0,8"/>

                                <CheckBox Grid.Row="8" Grid.Column="1"
                                          Content="Send daily summary email"
                                          IsChecked="{Binding SendDailySummary}"
                                          x:Name="SendDailySummaryCheckBox"
                                          Margin="0,0,0,8"/>

                                <TextBlock Grid.Row="9" Grid.Column="0" 
                                           Text="Summary Hour:" 
                                           Style="{StaticResource SettingLabelStyle}"
                                           Visibility="{Binding IsChecked, ElementName=SendDailySummaryCheckBox, Converter={StaticResource BoolToVisibility}}"/>
                                <TextBox Grid.Row="9" Grid.Column="1"
                                         Text="{Binding DailySummaryHour}"
                                         HorizontalAlignment="Left"
                                         Width="120"
                                         PreviewTextInput="NumberValidationTextBox"
                                         Visibility="{Binding IsChecked, ElementName=SendDailySummaryCheckBox, Converter={StaticResource BoolToVisibility}}"/>
                            </Grid>
                        </Border>

                    </StackPanel>
                </ScrollViewer>
            </TabItem>

            <!-- Tab 4: Logging & Service -->
            <TabItem Header="Logging &amp; Service">
                <ScrollViewer Padding="16">
                    <StackPanel>

                        <!-- Logging Settings -->
                        <TextBlock Text="Logging Configuration" Style="{StaticResource SettingHeaderStyle}"/>

                        <Grid Margin="0,0,0,16">
                            <Grid.ColumnDefinitions>
                                <ColumnDefinition Width="200"/>
                                <ColumnDefinition Width="*"/>
                            </Grid.ColumnDefinitions>
                            <Grid.RowDefinitions>
                                <RowDefinition Height="Auto"/>
                                <RowDefinition Height="Auto"/>
                                <RowDefinition Height="Auto"/>
                                <RowDefinition Height="Auto"/>
                                <RowDefinition Height="Auto"/>
                                <RowDefinition Height="Auto"/>
                            </Grid.RowDefinitions>

                            <TextBlock Grid.Row="0" Grid.Column="0" 
                                       Text="Log Level:" 
                                       Style="{StaticResource SettingLabelStyle}"/>
                            <ComboBox Grid.Row="0" Grid.Column="1"
                                      ItemsSource="{Binding LogLevels}"
                                      SelectedItem="{Binding LogLevel}"
                                      HorizontalAlignment="Left"
                                      Width="200"
                                      Margin="0,0,0,8"/>

                            <TextBlock Grid.Row="1" Grid.Column="0" 
                                       Text="Log Folder:" 
                                       Style="{StaticResource SettingLabelStyle}"/>
                            <Grid Grid.Row="1" Grid.Column="1" Margin="0,0,0,8">
                                <Grid.ColumnDefinitions>
                                    <ColumnDefinition Width="*"/>
                                    <ColumnDefinition Width="Auto"/>
                                </Grid.ColumnDefinitions>
                                <TextBox Grid.Column="0" 
                                         Text="{Binding LogFolder, UpdateSourceTrigger=PropertyChanged}"/>
                                <Button Grid.Column="1" 
                                        Content="..."
                                        Width="32"
                                        Margin="4,0,0,0"
                                        Click="BrowseLogFolder_Click"/>
                            </Grid>

                            <CheckBox Grid.Row="2" Grid.Column="1"
                                      Content="Enable file logging"
                                      IsChecked="{Binding EnableFileLogging}"
                                      Margin="0,0,0,8"/>

                            <CheckBox Grid.Row="3" Grid.Column="1"
                                      Content="Enable Windows Event Log"
                                      IsChecked="{Binding EnableServiceEventLog}"
                                      Margin="0,0,0,8"/>

                            <TextBlock Grid.Row="4" Grid.Column="0" 
                                       Text="Max Log File Size (MB):" 
                                       Style="{StaticResource SettingLabelStyle}"/>
                            <TextBox Grid.Row="4" Grid.Column="1"
                                     Text="{Binding MaxLogFileSizeMB}"
                                     HorizontalAlignment="Left"
                                     Width="120"
                                     Margin="0,0,0,8"
                                     PreviewTextInput="NumberValidationTextBox"/>

                            <TextBlock Grid.Row="5" Grid.Column="0" 
                                       Text="Max Log Files:" 
                                       Style="{StaticResource SettingLabelStyle}"/>
                            <TextBox Grid.Row="5" Grid.Column="1"
                                     Text="{Binding MaxLogFiles}"
                                     HorizontalAlignment="Left"
                                     Width="120"
                                     PreviewTextInput="NumberValidationTextBox"/>
                        </Grid>

                        <!-- Service Settings -->
                        <TextBlock Text="Service Configuration" Style="{StaticResource SettingHeaderStyle}"/>

                        <Grid>
                            <Grid.ColumnDefinitions>
                                <ColumnDefinition Width="200"/>
                                <ColumnDefinition Width="*"/>
                            </Grid.ColumnDefinitions>
                            <Grid.RowDefinitions>
                                <RowDefinition Height="Auto"/>
                                <RowDefinition Height="Auto"/>
                            </Grid.RowDefinitions>

                            <TextBlock Grid.Row="0" Grid.Column="0" 
                                       Text="Startup Delay (seconds):" 
                                       Style="{StaticResource SettingLabelStyle}"/>
                            <TextBox Grid.Row="0" Grid.Column="1"
                                     Text="{Binding StartupDelaySeconds}"
                                     HorizontalAlignment="Left"
                                     Width="120"
                                     Margin="0,0,0,8"
                                     PreviewTextInput="NumberValidationTextBox"/>

                            <TextBlock Grid.Row="1" Grid.Column="0" 
                                       Text="File Processing Delay (ms):" 
                                       Style="{StaticResource SettingLabelStyle}"/>
                            <TextBox Grid.Row="1" Grid.Column="1"
                                     Text="{Binding FileProcessingDelayMs}"
                                     HorizontalAlignment="Left"
                                     Width="120"
                                     PreviewTextInput="NumberValidationTextBox"/>
                        </Grid>

                        <!-- Info Box statt InfoBar -->
                        <Border Background="{DynamicResource SystemControlBackgroundBaseLowBrush}"
                                BorderBrush="{DynamicResource SystemControlForegroundBaseMediumLowBrush}"
                                BorderThickness="1"
                                CornerRadius="4"
                                Padding="12"
                                Margin="0,16,0,0">
                            <StackPanel>
                                <TextBlock Text="Service Timing" FontWeight="SemiBold" Margin="0,0,0,4"/>
                                <TextBlock Text="Startup delay prevents the service from starting before the system is fully ready. File processing delay prevents rapid file system changes from causing issues."
                                           TextWrapping="Wrap"
                                           Opacity="0.8"/>
                            </StackPanel>
                        </Border>

                    </StackPanel>
                </ScrollViewer>
            </TabItem>

        </TabControl>

        <!-- Status Bar -->
        <Border Grid.Row="2" 
                Background="{DynamicResource SystemControlBackgroundChromeMediumBrush}"
                Padding="24,8">
            <Grid>
                <Grid.ColumnDefinitions>
                    <ColumnDefinition Width="Auto"/>
                    <ColumnDefinition Width="*"/>
                    <ColumnDefinition Width="Auto"/>
                </Grid.ColumnDefinitions>

                <!-- Loading Indicator -->
                <ui:ProgressRing Grid.Column="0"
                                 IsActive="{Binding IsLoading}"
                                 Width="16"
                                 Height="16"
                                 Margin="0,0,8,0"/>

                <!-- Status Message -->
                <TextBlock Grid.Column="1"
                           Text="{Binding StatusMessage}"
                           VerticalAlignment="Center">
                    <TextBlock.Style>
                        <Style TargetType="TextBlock">
                            <Style.Triggers>
                                <DataTrigger Binding="{Binding IsError}" Value="True">
                                    <Setter Property="Foreground" Value="{DynamicResource SystemControlErrorTextForegroundBrush}"/>
                                </DataTrigger>
                            </Style.Triggers>
                        </Style>
                    </TextBlock.Style>
                </TextBlock>

                <!-- Changes Indicator -->
                <TextBlock Grid.Column="2"
                           Text="Changes pending"
                           FontStyle="Italic"
                           Foreground="{DynamicResource SystemControlForegroundBaseMediumBrush}"
                           VerticalAlignment="Center"
                           Visibility="{Binding HasChanges, Converter={StaticResource BoolToVisibility}}"/>
            </Grid>
        </Border>
    </Grid>
</Page>
``` 
 
### src\CamBridge.Config\Views\SettingsPage.xaml.cs 
```csharp 
// src/CamBridge.Config/Views/SettingsPage.xaml.cs
using System.Runtime.Versioning;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CamBridge.Config.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;

namespace CamBridge.Config.Views
{
    [SupportedOSPlatform("windows")]
    public partial class SettingsPage : Page
    {
        private readonly SettingsViewModel _viewModel;

        public SettingsPage()
        {
            InitializeComponent();

            // Get ViewModel from DI
            _viewModel = ((App)Application.Current).Host.Services.GetRequiredService<SettingsViewModel>();
            DataContext = _viewModel;

            // Initialize the view model
            _ = _viewModel.InitializeAsync();
        }

        // Number validation for TextBox inputs
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        // Browse folder dialogs
        private void BrowseWatchFolder_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Select Watch Folder",
                CheckFileExists = false,
                CheckPathExists = true,
                FileName = "Select Folder",
                Filter = "Folder|*.none",
                ValidateNames = false
            };

            if (dialog.ShowDialog() == true)
            {
                string? folderPath = System.IO.Path.GetDirectoryName(dialog.FileName);
                if (_viewModel.SelectedWatchFolder != null && !string.IsNullOrEmpty(folderPath))
                {
                    _viewModel.SelectedWatchFolder.Path = folderPath;
                }
            }
        }

        private void BrowseOutputFolder_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Select Output Folder",
                CheckFileExists = false,
                CheckPathExists = true,
                FileName = "Select Folder",
                Filter = "Folder|*.none",
                ValidateNames = false
            };

            if (dialog.ShowDialog() == true)
            {
                string? folderPath = System.IO.Path.GetDirectoryName(dialog.FileName);
                if (!string.IsNullOrEmpty(folderPath))
                {
                    _viewModel.DefaultOutputFolder = folderPath;
                }
            }
        }

        private void BrowseLogFolder_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Select Log Folder",
                CheckFileExists = false,
                CheckPathExists = true,
                FileName = "Select Folder",
                Filter = "Folder|*.none",
                ValidateNames = false
            };

            if (dialog.ShowDialog() == true)
            {
                string? folderPath = System.IO.Path.GetDirectoryName(dialog.FileName);
                if (!string.IsNullOrEmpty(folderPath))
                {
                    _viewModel.LogFolder = folderPath;
                }
            }
        }
    }
}
``` 
 
### src\CamBridge.Config\ViewModels\SettingsViewModel.cs - ViewModel (CHECK DI REGISTRATION
```csharp 
using CamBridge.Config.Services;
using CamBridge.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CamBridge.Config.ViewModels
{
    public partial class SettingsViewModel : ViewModelBase
    {
        private readonly IConfigurationService _configurationService;
        private CamBridgeSettings _originalSettings = new();

        // Collections for ComboBox bindings
        public ObservableCollection<string> LogLevels { get; } = new()
        {
            "Trace", "Debug", "Information", "Warning", "Error", "Critical"
        };

        public ObservableCollection<PostProcessingAction> ProcessingActions { get; } = new()
        {
            PostProcessingAction.Leave,
            PostProcessingAction.Archive,
            PostProcessingAction.Delete,
            PostProcessingAction.MoveToError
        };

        public ObservableCollection<OutputOrganization> OutputOrganizations { get; } = new()
        {
            OutputOrganization.None,
            OutputOrganization.ByPatient,
            OutputOrganization.ByDate,
            OutputOrganization.ByPatientAndDate
        };

        // Watch Folders
        [ObservableProperty] private ObservableCollection<FolderConfigurationViewModel> _watchFolders = new();
        [ObservableProperty] private FolderConfigurationViewModel? _selectedWatchFolder;

        // Processing Options
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Default output folder is required")]
        private string _defaultOutputFolder = @"C:\CamBridge\Output";

        [ObservableProperty] private PostProcessingAction _successAction = PostProcessingAction.Archive;
        [ObservableProperty] private PostProcessingAction _failureAction = PostProcessingAction.MoveToError;

        [ObservableProperty]
        [Required(ErrorMessage = "Archive folder is required")]
        private string _archiveFolder = @"C:\CamBridge\Archive";

        [ObservableProperty]
        [Required(ErrorMessage = "Error folder is required")]
        private string _errorFolder = @"C:\CamBridge\Errors";

        [ObservableProperty] private bool _createBackup = true;

        [ObservableProperty]
        [Required(ErrorMessage = "Backup folder is required")]
        private string _backupFolder = @"C:\CamBridge\Backup";

        [ObservableProperty]
        [Range(1, 10, ErrorMessage = "Max concurrent processing must be between 1 and 10")]
        private int _maxConcurrentProcessing = 2;

        [ObservableProperty] private bool _retryOnFailure = true;

        [ObservableProperty]
        [Range(1, 10, ErrorMessage = "Max retry attempts must be between 1 and 10")]
        private int _maxRetryAttempts = 3;

        [ObservableProperty] private OutputOrganization _outputOrganization = OutputOrganization.ByPatientAndDate;
        [ObservableProperty] private bool _processExistingOnStartup = true;
        [ObservableProperty] private int _maxFileAgeDays = 30;

        // DICOM Settings
        [ObservableProperty]
        [Required(ErrorMessage = "Implementation class UID is required")]
        private string _implementationClassUid = "1.2.276.0.7230010.3.0.3.6.4";

        [ObservableProperty]
        [Required(ErrorMessage = "Implementation version name is required")]
        private string _implementationVersionName = "CAMBRIDGE_001";

        [ObservableProperty] private string _institutionName = string.Empty;
        [ObservableProperty] private string _stationName = Environment.MachineName;
        [ObservableProperty] private bool _validateAfterCreation = true;

        // Notification Settings
        [ObservableProperty] private bool _enableEmail;
        [ObservableProperty] private bool _enableEventLog = true;

        [ObservableProperty]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        private string? _emailFrom;

        [ObservableProperty] private string? _emailTo;
        [ObservableProperty] private string? _smtpHost;

        [ObservableProperty]
        [Range(1, 65535, ErrorMessage = "SMTP port must be between 1 and 65535")]
        private int _smtpPort = 587;

        [ObservableProperty] private bool _smtpUseSsl = true;
        [ObservableProperty] private string? _smtpUsername;
        [ObservableProperty] private string? _smtpPassword;
        [ObservableProperty] private string _minimumEmailLevel = "Warning";
        [ObservableProperty] private bool _sendDailySummary = true;

        [ObservableProperty]
        [Range(0, 23, ErrorMessage = "Daily summary hour must be between 0 and 23")]
        private int _dailySummaryHour = 8;

        // Logging Settings
        [ObservableProperty] private string _logLevel = "Information";

        [ObservableProperty]
        [Required(ErrorMessage = "Log folder is required")]
        private string _logFolder = @"C:\CamBridge\Logs";

        [ObservableProperty] private bool _enableFileLogging = true;
        [ObservableProperty] private bool _enableServiceEventLog = true;

        [ObservableProperty]
        [Range(1, 1000, ErrorMessage = "Max log file size must be between 1 and 1000 MB")]
        private int _maxLogFileSizeMB = 10;

        [ObservableProperty]
        [Range(1, 100, ErrorMessage = "Max log files must be between 1 and 100")]
        private int _maxLogFiles = 10;

        // Service Settings
        [ObservableProperty]
        [Range(0, 300, ErrorMessage = "Startup delay must be between 0 and 300 seconds")]
        private int _startupDelaySeconds = 5;

        [ObservableProperty]
        [Range(100, 10000, ErrorMessage = "File processing delay must be between 100 and 10000 ms")]
        private int _fileProcessingDelayMs = 500;

        // Status properties
        [ObservableProperty] private bool _isLoading;
        [ObservableProperty] private bool _isSaving;
        [ObservableProperty] private bool _hasChanges;
        [ObservableProperty] private string? _statusMessage;
        [ObservableProperty] private bool _isError;

        public SettingsViewModel(IConfigurationService configurationService)
        {
            _configurationService = configurationService;

            PropertyChanged += (s, e) =>
            {
                if (e.PropertyName != nameof(HasChanges) &&
                    e.PropertyName != nameof(StatusMessage) &&
                    e.PropertyName != nameof(IsError) &&
                    e.PropertyName != nameof(IsLoading) &&
                    e.PropertyName != nameof(IsSaving))
                {
                    HasChanges = true;
                }
            };
        }

        public async Task InitializeAsync()
        {
            await LoadSettingsAsync();
        }

        [RelayCommand]
        private async Task LoadSettingsAsync()
        {
            try
            {
                IsLoading = true;
                IsError = false;
                StatusMessage = "Loading settings...";

                var settings = await _configurationService.LoadConfigurationAsync<CamBridgeSettings>();
                if (settings != null)
                {
                    _originalSettings = settings;
                    MapFromSettings(settings);
                    HasChanges = false;
                    StatusMessage = "Settings loaded successfully";
                }
                else
                {
                    StatusMessage = "Failed to load settings";
                    IsError = true;
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading settings: {ex.Message}";
                IsError = true;
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand(CanExecute = nameof(CanSave))]
        private async Task SaveSettingsAsync()
        {
            try
            {
                ValidateAllProperties();
                if (HasErrors)
                {
                    StatusMessage = "Please fix validation errors before saving";
                    IsError = true;
                    return;
                }

                IsSaving = true;
                IsError = false;
                StatusMessage = "Saving settings...";

                var settings = MapToSettings();
                await _configurationService.SaveConfigurationAsync(settings);

                _originalSettings = settings;
                HasChanges = false;
                StatusMessage = "Settings saved successfully";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error saving settings: {ex.Message}";
                IsError = true;
            }
            finally
            {
                IsSaving = false;
            }
        }

        private bool CanSave() => HasChanges && !IsLoading && !IsSaving;

        [RelayCommand(CanExecute = nameof(CanReset))]
        private void ResetSettings()
        {
            MapFromSettings(_originalSettings);
            HasChanges = false;
            StatusMessage = "Settings reset to last saved state";
            IsError = false;
        }

        private bool CanReset() => HasChanges && !IsLoading && !IsSaving;

        [RelayCommand]
        private void AddWatchFolder()
        {
            var newFolder = new FolderConfigurationViewModel
            {
                Path = @"C:\CamBridge\NewFolder",
                Enabled = true,
                FilePattern = "*.jpg;*.jpeg"
            };

            WatchFolders.Add(newFolder);
            SelectedWatchFolder = newFolder;
            HasChanges = true;
        }

        [RelayCommand(CanExecute = nameof(CanRemoveWatchFolder))]
        private void RemoveWatchFolder()
        {
            if (SelectedWatchFolder != null)
            {
                WatchFolders.Remove(SelectedWatchFolder);
                SelectedWatchFolder = WatchFolders.FirstOrDefault();
                HasChanges = true;
            }
        }

        private bool CanRemoveWatchFolder() => SelectedWatchFolder != null;

        private void MapFromSettings(CamBridgeSettings settings)
        {
            // Map all settings to view model properties
            // Implementation details omitted for brevity
        }

        private CamBridgeSettings MapToSettings()
        {
            // Map view model properties back to settings
            // Implementation details omitted for brevity
            return new CamBridgeSettings();
        }
    }

    public partial class FolderConfigurationViewModel : ObservableValidator
    {
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Path is required")]
        private string _path = string.Empty;

        [ObservableProperty] private string? _outputPath;
        [ObservableProperty] private bool _enabled = true;
        [ObservableProperty] private bool _includeSubdirectories;

        [ObservableProperty]
        [Required(ErrorMessage = "File pattern is required")]
        private string _filePattern = "*.jpg;*.jpeg";
    }
}
``` 
 
## Working Pages for Reference 
 
### src\CamBridge.Config\Views\DeadLettersPage.xaml.cs - Working Example 
```csharp 
using System.Runtime.Versioning;
using System.Windows;
using System.Windows.Controls;
using CamBridge.Config.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace CamBridge.Config.Views
{
    [SupportedOSPlatform("windows")]
    public partial class DeadLettersPage : Page
    {
        private DeadLettersViewModel? _viewModel;

        public DeadLettersPage()
        {
            InitializeComponent();

            // Get ViewModel from DI with null check
            try
            {
                var app = Application.Current as App;
                if (app?.Host != null)
                {
                    _viewModel = app.Host.Services.GetRequiredService<DeadLettersViewModel>();
                    DataContext = _viewModel;
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading DeadLettersViewModel: {ex.Message}");
                // Create a basic viewmodel if DI fails
                _viewModel = new DeadLettersViewModel(null!);
                DataContext = _viewModel;
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            _viewModel?.Cleanup();
        }
    }
}
``` 
 
### src\CamBridge.Config\ViewModels\DeadLettersViewModel.cs - Working ViewModel 
```csharp 
// src/CamBridge.Config/ViewModels/DeadLettersViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CamBridge.Config.Models;
using CamBridge.Config.Services;

namespace CamBridge.Config.ViewModels
{
    /// <summary>
    /// ViewModel for Dead Letters management - simplified for initial compilation
    /// </summary>
    public partial class DeadLettersViewModel : ViewModelBase
    {
        private readonly IApiService _apiService;

        [ObservableProperty] private bool _isConnected;
        [ObservableProperty] private string _connectionStatus = "Connecting...";

        public DeadLettersViewModel(IApiService apiService)
        {
            _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
        }

        public void Cleanup()
        {
            // Cleanup resources
        }
    }
}
``` 
 
## Configuration Services 
 
### src\CamBridge.Config\Services\ConfigurationService.cs 
```csharp 
using CamBridge.Core;
using System;
using System.Threading.Tasks;

namespace CamBridge.Config.Services
{
    /// <summary>
    /// Minimal implementation for testing
    /// </summary>
    public class ConfigurationService : IConfigurationService
    {
        public async Task<T?> LoadConfigurationAsync<T>() where T : class
        {
            await Task.Delay(100); // Simulate async work

            if (typeof(T) == typeof(CamBridgeSettings))
            {
                // Return default settings for testing
                var settings = new CamBridgeSettings
                {
                    DefaultOutputFolder = @"C:\CamBridge\Output",
                    Processing = new ProcessingOptions
                    {
                        ArchiveFolder = @"C:\CamBridge\Archive",
                        ErrorFolder = @"C:\CamBridge\Errors",
                        BackupFolder = @"C:\CamBridge\Backup"
                    },
                    Dicom = new DicomSettings
                    {
                        ImplementationClassUid = "1.2.276.0.7230010.3.0.3.6.4",
                        ImplementationVersionName = "CAMBRIDGE_001"
                    },
                    Logging = new LoggingSettings
                    {
                        LogFolder = @"C:\CamBridge\Logs"
                    },
                    Notifications = new NotificationSettings()
                };

                return settings as T;
            }

            return null;
        }

        public async Task SaveConfigurationAsync<T>(T configuration) where T : class
        {
            await Task.Delay(100); // Simulate async work
            // For testing, just pretend we saved
        }
    }
}
``` 
 
### src\CamBridge.Config\Services\IConfigurationService.cs 
```csharp 
using System.Threading.Tasks;

namespace CamBridge.Config.Services
{
    public interface IConfigurationService
    {
        Task<T?> LoadConfigurationAsync<T>() where T : class;
        Task SaveConfigurationAsync<T>(T configuration) where T : class;
    }
}
``` 
 
## Mapping Configuration Classes 
 
### src\CamBridge.Core\MappingRule.cs 
```csharp 
﻿using System;
using CamBridge.Core.ValueObjects;

namespace CamBridge.Core
{
    /// <summary>
    /// Defines a mapping rule from source data to DICOM tag
    /// </summary>
    public class MappingRule
    {
        public string Name { get; }
        public string SourceType { get; }
        public string SourceField { get; }
        public DicomTag TargetTag { get; }
        public ValueTransform Transform { get; }
        public bool IsRequired { get; }
        public string? DefaultValue { get; }

        public MappingRule(
            string name,
            string sourceType,
            string sourceField,
            DicomTag targetTag,
            ValueTransform transform = ValueTransform.None,
            bool isRequired = false,
            string? defaultValue = null)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            SourceType = sourceType ?? throw new ArgumentNullException(nameof(sourceType));
            SourceField = sourceField ?? throw new ArgumentNullException(nameof(sourceField));
            TargetTag = targetTag ?? throw new ArgumentNullException(nameof(targetTag));
            Transform = transform;
            IsRequired = isRequired;
            DefaultValue = defaultValue;
        }

        /// <summary>
        /// Applies the transformation to a value
        /// </summary>
        public string? ApplyTransform(string? value)
        {
            if (string.IsNullOrEmpty(value))
                return DefaultValue;

            return Transform switch
            {
                ValueTransform.None => value,
                ValueTransform.ToUpper => value.ToUpper(),
                ValueTransform.ToLower => value.ToLower(),
                ValueTransform.DateToDicom => TransformDateToDicom(value),
                ValueTransform.TimeToDicom => TransformTimeToDicom(value),
                ValueTransform.GenderToDicom => TransformGenderToDicom(value),
                ValueTransform.TruncateTo16 => value.Length > 16 ? value.Substring(0, 16) : value,
                ValueTransform.TruncateTo64 => value.Length > 64 ? value.Substring(0, 64) : value,
                _ => value
            };
        }

        private static string? TransformDateToDicom(string value)
        {
            // Convert various date formats to DICOM format (YYYYMMDD)
            if (DateTime.TryParse(value, out var date))
            {
                return date.ToString("yyyyMMdd");
            }
            return value;
        }

        private static string? TransformTimeToDicom(string value)
        {
            // Convert various time formats to DICOM format (HHMMSS.FFFFFF)
            if (DateTime.TryParse(value, out var time))
            {
                return time.ToString("HHmmss.ffffff");
            }
            return value;
        }

        private static string TransformGenderToDicom(string value)
        {
            // Convert gender to DICOM format (M, F, O)
            return value?.ToUpperInvariant() switch
            {
                "M" or "MALE" or "MANN" or "MÄNNLICH" => "M",
                "F" or "FEMALE" or "FRAU" or "WEIBLICH" => "F",
                "O" or "OTHER" or "ANDERE" or "DIVERS" => "O",
                _ => "O"
            };
        }
    }

    /// <summary>
    /// Value transformation types
    /// </summary>
    public enum ValueTransform
    {
        None,
        ToUpper,
        ToLower,
        DateToDicom,
        TimeToDicom,
        GenderToDicom,
        TruncateTo16,
        TruncateTo64
    }
}``` 
 
### src\CamBridge.Core\Interfaces\IMappingConfiguration.cs 
```csharp 
﻿using System.Collections.Generic;
using CamBridge.Core.ValueObjects;

namespace CamBridge.Core.Interfaces
{
    /// <summary>
    /// Interface for configuring EXIF to DICOM tag mappings
    /// </summary>
    public interface IMappingConfiguration
    {
        /// <summary>
        /// Gets all configured mapping rules
        /// </summary>
        IReadOnlyList<MappingRule> GetMappingRules();

        /// <summary>
        /// Gets mapping rules for a specific source type
        /// </summary>
        IReadOnlyList<MappingRule> GetRulesForSource(string sourceType);

        /// <summary>
        /// Adds a new mapping rule
        /// </summary>
        void AddRule(MappingRule rule);

        /// <summary>
        /// Removes a mapping rule
        /// </summary>
        bool RemoveRule(string ruleName);

        /// <summary>
        /// Gets the default mapping configuration
        /// </summary>
        static IMappingConfiguration GetDefault() => new DefaultMappingConfiguration();
    }

    /// <summary>
    /// Default implementation with standard mappings
    /// </summary>
    internal class DefaultMappingConfiguration : IMappingConfiguration
    {
        private readonly List<MappingRule> _rules = new();

        public DefaultMappingConfiguration()
        {
            InitializeDefaultRules();
        }

        public IReadOnlyList<MappingRule> GetMappingRules() => _rules.AsReadOnly();

        public IReadOnlyList<MappingRule> GetRulesForSource(string sourceType)
            => _rules.Where(r => r.SourceType == sourceType).ToList().AsReadOnly();

        public void AddRule(MappingRule rule)
        {
            if (rule == null) throw new ArgumentNullException(nameof(rule));
            _rules.Add(rule);
        }

        public bool RemoveRule(string ruleName)
            => _rules.RemoveAll(r => r.Name == ruleName) > 0;

        private void InitializeDefaultRules()
        {
            // Patient data mappings
            _rules.Add(new MappingRule(
                "PatientName",
                "QRBridge",
                "name",
                DicomTag.PatientModule.PatientName,
                ValueTransform.None
            ));

            _rules.Add(new MappingRule(
                "PatientID",
                "QRBridge",
                "patientid",
                DicomTag.PatientModule.PatientID,
                ValueTransform.None
            ));

            _rules.Add(new MappingRule(
                "PatientBirthDate",
                "QRBridge",
                "birthdate",
                DicomTag.PatientModule.PatientBirthDate,
                ValueTransform.DateToDicom
            ));

            _rules.Add(new MappingRule(
                "PatientSex",
                "QRBridge",
                "gender",
                DicomTag.PatientModule.PatientSex,
                ValueTransform.GenderToDicom
            ));

            // Study data mappings
            _rules.Add(new MappingRule(
                "StudyDescription",
                "QRBridge",
                "comment",
                DicomTag.StudyModule.StudyDescription,
                ValueTransform.None
            ));

            _rules.Add(new MappingRule(
                "StudyID",
                "QRBridge",
                "examid",
                DicomTag.StudyModule.StudyID,
                ValueTransform.TruncateTo16
            ));

            // Equipment mappings
            _rules.Add(new MappingRule(
                "Manufacturer",
                "EXIF",
                "Make",
                DicomTag.EquipmentModule.Manufacturer,
                ValueTransform.None
            ));

            _rules.Add(new MappingRule(
                "Model",
                "EXIF",
                "Model",
                DicomTag.EquipmentModule.ManufacturerModelName,
                ValueTransform.None
            ));
        }
    }
}``` 
 
### src\CamBridge.Infrastructure\Services\MappingConfigurationLoader.cs 
```csharp 
using CamBridge.Core;
using CamBridge.Core.Interfaces;
using CamBridge.Core.ValueObjects;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CamBridge.Infrastructure.Services
{
    /// <summary>
    /// Service for loading and saving mapping configurations from/to JSON files
    /// </summary>
    public class MappingConfigurationLoader
    {
        private readonly ILogger<MappingConfigurationLoader> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public MappingConfigurationLoader(ILogger<MappingConfigurationLoader> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters =
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase),
                    new DicomTagJsonConverter()
                }
            };
        }

        /// <summary>
        /// Loads mapping configuration from a JSON file
        /// </summary>
        public async Task<IMappingConfiguration> LoadFromFileAsync(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path cannot be null or empty", nameof(filePath));

            try
            {
                _logger.LogInformation("Loading mapping configuration from {FilePath}", filePath);

                if (!File.Exists(filePath))
                {
                    _logger.LogWarning("Configuration file not found at {FilePath}, using default configuration", filePath);
                    return IMappingConfiguration.GetDefault();
                }

                var json = await File.ReadAllTextAsync(filePath);
                var config = JsonSerializer.Deserialize<MappingConfigurationDto>(json, _jsonOptions);

                if (config?.Mappings == null || config.Mappings.Count == 0)
                {
                    _logger.LogWarning("No mappings found in configuration file, using default configuration");
                    return IMappingConfiguration.GetDefault();
                }

                var mappingConfig = new CustomMappingConfiguration();
                foreach (var mapping in config.Mappings)
                {
                    var rule = ConvertToMappingRule(mapping);
                    mappingConfig.AddRule(rule);
                }

                _logger.LogInformation("Successfully loaded {Count} mapping rules from {FilePath}",
                    config.Mappings.Count, filePath);

                return mappingConfig;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Invalid JSON in configuration file {FilePath}", filePath);
                throw new InvalidOperationException($"Failed to parse mapping configuration from {filePath}", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading mapping configuration from {FilePath}", filePath);
                throw;
            }
        }

        /// <summary>
        /// Saves mapping configuration to a JSON file
        /// </summary>
        public async Task SaveToFileAsync(IMappingConfiguration configuration, string filePath)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path cannot be null or empty", nameof(filePath));

            try
            {
                _logger.LogInformation("Saving mapping configuration to {FilePath}", filePath);

                var rules = configuration.GetMappingRules();
                var dto = new MappingConfigurationDto
                {
                    Version = "1.0",
                    Description = "CamBridge EXIF to DICOM mapping configuration",
                    Mappings = rules.Select(ConvertToDto).ToList()
                };

                var json = JsonSerializer.Serialize(dto, _jsonOptions);

                var directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                await File.WriteAllTextAsync(filePath, json);

                _logger.LogInformation("Successfully saved {Count} mapping rules to {FilePath}",
                    rules.Count, filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving mapping configuration to {FilePath}", filePath);
                throw;
            }
        }

        /// <summary>
        /// Creates a sample configuration file with common mappings
        /// </summary>
        public async Task CreateSampleConfigurationAsync(string filePath)
        {
            _logger.LogInformation("Creating sample mapping configuration at {FilePath}", filePath);

            var sampleConfig = new MappingConfigurationDto
            {
                Version = "1.0",
                Description = "Sample CamBridge mapping configuration for Ricoh cameras with QRBridge",
                Mappings = new List<MappingRuleDto>
                {
                    // Patient mappings
                    new() { Name = "PatientName", SourceType = "QRBridge", SourceField = "name",
                           TargetTag = "(0010,0010)", Transform = "None", Required = true },
                    new() { Name = "PatientID", SourceType = "QRBridge", SourceField = "patientid",
                           TargetTag = "(0010,0020)", Transform = "None", Required = true },
                    new() { Name = "PatientBirthDate", SourceType = "QRBridge", SourceField = "birthdate",
                           TargetTag = "(0010,0030)", Transform = "DateToDicom" },
                    new() { Name = "PatientSex", SourceType = "QRBridge", SourceField = "gender",
                           TargetTag = "(0010,0040)", Transform = "GenderToDicom" },

                    // Study mappings
                    new() { Name = "StudyDescription", SourceType = "QRBridge", SourceField = "comment",
                           TargetTag = "(0008,1030)", Transform = "None" },
                    new() { Name = "StudyID", SourceType = "QRBridge", SourceField = "examid",
                           TargetTag = "(0020,0010)", Transform = "TruncateTo16" },

                    // Equipment mappings from EXIF
                    new() { Name = "Manufacturer", SourceType = "EXIF", SourceField = "Make",
                           TargetTag = "(0008,0070)", Transform = "None" },
                    new() { Name = "Model", SourceType = "EXIF", SourceField = "Model",
                           TargetTag = "(0008,1090)", Transform = "None" },
                    new() { Name = "Software", SourceType = "EXIF", SourceField = "Software",
                           TargetTag = "(0018,1020)", Transform = "None" },

                    // Additional patient identifiers (German specific)
                    new() { Name = "PatientInsuranceNumber", SourceType = "QRBridge", SourceField = "versichertennummer",
                           TargetTag = "(0010,1000)", Transform = "None" },
                    new() { Name = "PatientCase", SourceType = "QRBridge", SourceField = "fallnummer",
                           TargetTag = "(0010,1001)", Transform = "None" }
                }
            };

            var json = JsonSerializer.Serialize(sampleConfig, _jsonOptions);

            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            await File.WriteAllTextAsync(filePath, json);
            _logger.LogInformation("Sample configuration created successfully");
        }

        private MappingRule ConvertToMappingRule(MappingRuleDto dto)
        {
            var targetTag = DicomTag.Parse(dto.TargetTag);
            var transform = Enum.Parse<ValueTransform>(dto.Transform ?? "None", ignoreCase: true);

            return new MappingRule(
                dto.Name,
                dto.SourceType,
                dto.SourceField,
                targetTag,
                transform,
                dto.Required ?? false,
                dto.DefaultValue
            );
        }

        private MappingRuleDto ConvertToDto(MappingRule rule)
        {
            return new MappingRuleDto
            {
                Name = rule.Name,
                SourceType = rule.SourceType,
                SourceField = rule.SourceField,
                TargetTag = rule.TargetTag.ToString(),
                Transform = rule.Transform.ToString(),
                Required = rule.IsRequired,
                DefaultValue = rule.DefaultValue
            };
        }

        // DTOs for JSON serialization
        private class MappingConfigurationDto
        {
            public string Version { get; set; } = "1.0";
            public string? Description { get; set; }
            public List<MappingRuleDto> Mappings { get; set; } = new();
        }

        private class MappingRuleDto
        {
            public string Name { get; set; } = string.Empty;
            public string SourceType { get; set; } = string.Empty;
            public string SourceField { get; set; } = string.Empty;
            public string TargetTag { get; set; } = string.Empty;
            public string? Transform { get; set; }
            public bool? Required { get; set; }
            public string? DefaultValue { get; set; }
        }

        /// <summary>
        /// Custom JSON converter for DicomTag serialization
        /// </summary>
        private class DicomTagJsonConverter : JsonConverter<DicomTag>
        {
            public override DicomTag Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                var tagString = reader.GetString();
                return DicomTag.Parse(tagString!);
            }

            public override void Write(Utf8JsonWriter writer, DicomTag value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(value.ToString());
            }
        }
    }

    /// <summary>
    /// Custom implementation of IMappingConfiguration that can be modified at runtime
    /// </summary>
    public class CustomMappingConfiguration : IMappingConfiguration
    {
        private readonly List<MappingRule> _rules = new();

        public IReadOnlyList<MappingRule> GetMappingRules() => _rules.AsReadOnly();

        public IReadOnlyList<MappingRule> GetRulesForSource(string sourceType)
            => _rules.Where(r => r.SourceType.Equals(sourceType, StringComparison.OrdinalIgnoreCase))
                     .ToList()
                     .AsReadOnly();

        public void AddRule(MappingRule rule)
        {
            if (rule == null) throw new ArgumentNullException(nameof(rule));
            _rules.Add(rule);
        }

        public bool RemoveRule(string ruleName)
            => _rules.RemoveAll(r => r.Name.Equals(ruleName, StringComparison.OrdinalIgnoreCase)) > 0;

        /// <summary>
        /// Validates the configuration for consistency and completeness
        /// </summary>
        public ValidationResult Validate()
        {
            var errors = new List<string>();

            // Check for duplicate rule names
            var duplicateNames = _rules
                .GroupBy(r => r.Name, StringComparer.OrdinalIgnoreCase)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key);

            foreach (var name in duplicateNames)
            {
                errors.Add($"Duplicate rule name: {name}");
            }

            // Check for duplicate target tags from same source
            var duplicateTags = _rules
                .GroupBy(r => new { r.SourceType, r.SourceField, r.TargetTag })
                .Where(g => g.Count() > 1)
                .Select(g => g.Key);

            foreach (var dup in duplicateTags)
            {
                errors.Add($"Duplicate mapping: {dup.SourceType}.{dup.SourceField} -> {dup.TargetTag}");
            }

            // Check required patient identifiers
            var hasPatientName = _rules.Any(r => r.TargetTag.Equals(DicomTag.PatientModule.PatientName));
            var hasPatientId = _rules.Any(r => r.TargetTag.Equals(DicomTag.PatientModule.PatientID));

            if (!hasPatientName)
                errors.Add("Missing required mapping for Patient Name (0010,0010)");
            if (!hasPatientId)
                errors.Add("Missing required mapping for Patient ID (0010,0020)");

            return errors.Count == 0
                ? new ValidationResult { IsValid = true }
                : new ValidationResult { IsValid = false, Errors = errors };
        }

        public class ValidationResult
        {
            public bool IsValid { get; set; }
            public List<string> Errors { get; set; } = new();
        }
    }
}
``` 
 
## Core Models 
 
### src\CamBridge.Core\CamBridgeSettings.cs 
```csharp 
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CamBridge.Core
{
    /// <summary>
    /// Main configuration settings for CamBridge
    /// </summary>
    public class CamBridgeSettings
    {
        /// <summary>
        /// Folders to monitor for new JPEG files
        /// </summary>
        public List<FolderConfiguration> WatchFolders { get; set; } = new();

        /// <summary>
        /// Default output folder for DICOM files
        /// </summary>
        public string DefaultOutputFolder { get; set; } = @"C:\CamBridge\Output";

        /// <summary>
        /// Path to mapping configuration file
        /// </summary>
        public string MappingConfigurationFile { get; set; } = "mappings.json";

        /// <summary>
        /// Whether to use Ricoh-specific EXIF reader
        /// </summary>
        public bool UseRicohExifReader { get; set; } = true;

        /// <summary>
        /// Processing options
        /// </summary>
        public ProcessingOptions Processing { get; set; } = new();

        /// <summary>
        /// DICOM specific settings
        /// </summary>
        public DicomSettings Dicom { get; set; } = new();

        /// <summary>
        /// Logging configuration
        /// </summary>
        public LoggingSettings Logging { get; set; } = new();

        /// <summary>
        /// Service-specific settings
        /// </summary>
        public ServiceSettings Service { get; set; } = new();

        /// <summary>
        /// Notification settings
        /// </summary>
        public NotificationSettings Notifications { get; set; } = new();
    }

    public class FolderConfiguration
    {
        public string Path { get; set; } = string.Empty;
        public string? OutputPath { get; set; }
        public bool Enabled { get; set; } = true;
        public bool IncludeSubdirectories { get; set; } = false;
        public string FilePattern { get; set; } = "*.jpg;*.jpeg";

        [JsonIgnore]
        public bool IsValid => !string.IsNullOrWhiteSpace(Path) &&
                              System.IO.Directory.Exists(Path);
    }

    public class DicomSettings
    {
        /// <summary>
        /// Implementation class UID prefix for this institution
        /// </summary>
        public string ImplementationClassUid { get; set; } = "1.2.276.0.7230010.3.0.3.6.4";

        /// <summary>
        /// Implementation version name
        /// </summary>
        public string ImplementationVersionName { get; set; } = "CAMBRIDGE_001";

        /// <summary>
        /// Default institution name
        /// </summary>
        public string InstitutionName { get; set; } = string.Empty;

        /// <summary>
        /// Station name
        /// </summary>
        public string StationName { get; set; } = Environment.MachineName;

        /// <summary>
        /// Whether to validate DICOM files after creation
        /// </summary>
        public bool ValidateAfterCreation { get; set; } = true;
    }

    public class LoggingSettings
    {
        public string LogLevel { get; set; } = "Information";
        public string LogFolder { get; set; } = @"C:\CamBridge\Logs";
        public bool EnableFileLogging { get; set; } = true;
        public bool EnableEventLog { get; set; } = true;
        public int MaxLogFileSizeMB { get; set; } = 10;
        public int MaxLogFiles { get; set; } = 10;

        /// <summary>
        /// Whether to include patient data in debug logs (CAUTION!)
        /// </summary>
        public bool IncludePatientDataInDebugLogs { get; set; } = false;
    }

    public class ServiceSettings
    {
        public string ServiceName { get; set; } = "CamBridgeService";
        public string DisplayName { get; set; } = "CamBridge JPEG to DICOM Converter";
        public string Description { get; set; } = "Monitors folders for JPEG files from Ricoh cameras and converts them to DICOM format";
        public int StartupDelaySeconds { get; set; } = 5;
        public int FileProcessingDelayMs { get; set; } = 500;
    }
}
``` 
 
### src\CamBridge.Core\ProcessingOptions.cs 
```csharp 
﻿using System;

namespace CamBridge.Core
{
    /// <summary>
    /// Options for file processing behavior
    /// </summary>
    public class ProcessingOptions
    {
        /// <summary>
        /// What to do with source files after successful conversion
        /// </summary>
        public PostProcessingAction SuccessAction { get; set; } = PostProcessingAction.Archive;

        /// <summary>
        /// What to do with source files after failed conversion
        /// </summary>
        public PostProcessingAction FailureAction { get; set; } = PostProcessingAction.Leave;

        /// <summary>
        /// Archive folder for processed files
        /// </summary>
        public string ArchiveFolder { get; set; } = @"C:\CamBridge\Archive";

        /// <summary>
        /// Error folder for failed files
        /// </summary>
        public string ErrorFolder { get; set; } = @"C:\CamBridge\Errors";

        /// <summary>
        /// Whether to create a backup before processing
        /// </summary>
        public bool CreateBackup { get; set; } = true;

        /// <summary>
        /// Backup folder location
        /// </summary>
        public string BackupFolder { get; set; } = @"C:\CamBridge\Backup";

        /// <summary>
        /// Maximum concurrent file processing tasks
        /// </summary>
        public int MaxConcurrentProcessing { get; set; } = 2;

        /// <summary>
        /// Retry failed conversions
        /// </summary>
        public bool RetryOnFailure { get; set; } = true;

        /// <summary>
        /// Number of retry attempts
        /// </summary>
        public int MaxRetryAttempts { get; set; } = 3;

        /// <summary>
        /// Delay between retry attempts in seconds
        /// </summary>
        public int RetryDelaySeconds { get; set; } = 5;

        /// <summary>
        /// Whether to process existing files on startup
        /// </summary>
        public bool ProcessExistingOnStartup { get; set; } = true;

        /// <summary>
        /// File age threshold - don't process files older than this
        /// </summary>
        public TimeSpan? MaxFileAge { get; set; } = TimeSpan.FromDays(30);

        /// <summary>
        /// Minimum file size in bytes
        /// </summary>
        public long MinimumFileSizeBytes { get; set; } = 1024; // 1KB

        /// <summary>
        /// Maximum file size in bytes
        /// </summary>
        public long MaximumFileSizeBytes { get; set; } = 100 * 1024 * 1024; // 100MB

        /// <summary>
        /// Output filename pattern
        /// </summary>
        public string OutputFilePattern { get; set; } = "{PatientID}_{StudyDate}_{InstanceNumber}.dcm";

        /// <summary>
        /// Whether to preserve original folder structure
        /// </summary>
        public bool PreserveFolderStructure { get; set; } = false;

        /// <summary>
        /// Organization of output files
        /// </summary>
        public OutputOrganization OutputOrganization { get; set; } = OutputOrganization.ByPatient;
    }

    public enum PostProcessingAction
    {
        /// <summary>
        /// Leave the file in place
        /// </summary>
        Leave,

        /// <summary>
        /// Move to archive folder
        /// </summary>
        Archive,

        /// <summary>
        /// Delete the file (use with caution!)
        /// </summary>
        Delete,

        /// <summary>
        /// Move to error folder
        /// </summary>
        MoveToError
    }

    public enum OutputOrganization
    {
        /// <summary>
        /// No organization, all files in output folder
        /// </summary>
        None,

        /// <summary>
        /// Organize by patient: OutputFolder/PatientID/files
        /// </summary>
        ByPatient,

        /// <summary>
        /// Organize by date: OutputFolder/YYYY-MM-DD/files
        /// </summary>
        ByDate,

        /// <summary>
        /// Organize by patient and date: OutputFolder/PatientID/YYYY-MM-DD/files
        /// </summary>
        ByPatientAndDate
    }
}``` 
 
## Project Files 
 
### src\CamBridge.Config\CamBridge.Config.csproj 
```xml 
﻿<!-- src/CamBridge.Config/CamBridge.Config.csproj -->
<Project Sdk="Microsoft.NET.Sdk">

	<PropertyGroup>
		<OutputType>WinExe</OutputType>
		<TargetFramework>net8.0-windows</TargetFramework>
		<UseWPF>true</UseWPF>
		<Platform>x64</Platform>
		<Platforms>x64</Platforms>
		<GenerateAssemblyInfo>false</GenerateAssemblyInfo>
		<Nullable>enable</Nullable>
		<ImplicitUsings>enable</ImplicitUsings>
		<ApplicationManifest>app.manifest</ApplicationManifest>
		<AllowUnsafeBlocks>true</AllowUnsafeBlocks>
	</PropertyGroup>

	<ItemGroup>
		<PackageReference Include="CommunityToolkit.Mvvm" Version="8.2.2" />
		<PackageReference Include="Microsoft.Extensions.Configuration" Version="8.0.0" />
		<PackageReference Include="Microsoft.Extensions.Configuration.Json" Version="8.0.0" />
		<PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="8.0.0" />
		<PackageReference Include="Microsoft.Extensions.Hosting" Version="8.0.0" />
		<PackageReference Include="Microsoft.Extensions.Http" Version="8.0.0" />
		<PackageReference Include="Microsoft.Extensions.Logging.Debug" Version="8.0.0" />
		<PackageReference Include="ModernWpfUI" Version="0.9.6" />
		<PackageReference Include="System.ServiceProcess.ServiceController" Version="8.0.0" />
		<PackageReference Include="System.Drawing.Common" Version="8.0.0" />
		<PackageReference Include="System.Text.Json" Version="8.0.5" />
	</ItemGroup>

	<ItemGroup>
		<ProjectReference Include="..\CamBridge.Core\CamBridge.Core.csproj" />
	</ItemGroup>

</Project>``` 
 
## Sample Configurations 
 
### mappings.json 
```json 
{
  "version": "1.0",
  "description": "CamBridge EXIF to DICOM mapping configuration for Ricoh G900 II with QRBridge",
  "mappings": [
    {
      "name": "PatientName",
      "sourceType": "QRBridge",
      "sourceField": "name",
      "targetTag": "(0010,0010)",
      "transform": "None",
      "required": true
    },
    {
      "name": "PatientID",
      "sourceType": "QRBridge",
      "sourceField": "patientid",
      "targetTag": "(0010,0020)",
      "transform": "None",
      "required": true
    },
    {
      "name": "PatientBirthDate",
      "sourceType": "QRBridge",
      "sourceField": "birthdate",
      "targetTag": "(0010,0030)",
      "transform": "DateToDicom",
      "required": false
    },
    {
      "name": "PatientSex",
      "sourceType": "QRBridge",
      "sourceField": "gender",
      "targetTag": "(0010,0040)",
      "transform": "GenderToDicom",
      "required": false,
      "defaultValue": "O"
    },
    {
      "name": "StudyDescription",
      "sourceType": "QRBridge",
      "sourceField": "comment",
      "targetTag": "(0008,1030)",
      "transform": "None",
      "required": false
    },
    {
      "name": "StudyID",
      "sourceType": "QRBridge",
      "sourceField": "examid",
      "targetTag": "(0020,0010)",
      "transform": "TruncateTo16",
      "required": false
    },
    {
      "name": "Manufacturer",
      "sourceType": "EXIF",
      "sourceField": "Make",
      "targetTag": "(0008,0070)",
      "transform": "None",
      "required": false
    },
    {
      "name": "ManufacturerModelName",
      "sourceType": "EXIF",
      "sourceField": "Model",
      "targetTag": "(0008,1090)",
      "transform": "None",
      "required": false
    },
    {
      "name": "SoftwareVersions",
      "sourceType": "EXIF",
      "sourceField": "Software",
      "targetTag": "(0018,1020)",
      "transform": "None",
      "required": false
    },
    {
      "name": "AcquisitionDateTime",
      "sourceType": "EXIF",
      "sourceField": "DateTimeOriginal",
      "targetTag": "(0008,002A)",
      "transform": "DateToDicom",
      "required": false
    },
    {
      "name": "PatientComments",
      "sourceType": "QRBridge",
      "sourceField": "comment",
      "targetTag": "(0010,4000)",
      "transform": "None",
      "required": false
    }
  ]
}
``` 
 
### appsettings.json (excerpt
```json 
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "CamBridge": {
    "WatchFolders": [
      {
        "Path": "C:\\CamBridge\\Input",
        "OutputPath": "C:\\CamBridge\\Output",
        "Enabled": true,
        "IncludeSubdirectories": false,
        "FilePattern": "*.jpg;*.jpeg"
      }
    ],
    "DefaultOutputFolder": "C:\\CamBridge\\Output",
    "MappingConfigurationFile": "mappings.json",
    "UseRicohExifReader": true,
    "Processing": {
      "SuccessAction": "Archive",
      "FailureAction": "MoveToError",
      "ArchiveFolder": "C:\\CamBridge\\Archive",
      "ErrorFolder": "C:\\CamBridge\\Errors",
      "CreateBackup": true,
      "BackupFolder": "C:\\CamBridge\\Backup",
      "MaxConcurrentProcessing": 2,
      "RetryOnFailure": true,
      "MaxRetryAttempts": 3,
      "RetryDelaySeconds": 5,
      "ProcessExistingOnStartup": true,
      "MaxFileAge": "30.00:00:00",
      "MinimumFileSizeBytes": 1024,
      "MaximumFileSizeBytes": 104857600,
      "OutputFilePattern": "{PatientID}_{StudyDate}_{InstanceNumber}.dcm",
      "PreserveFolderStructure": false,
      "OutputOrganization": "ByPatientAndDate"
    },
    "Dicom": {
      "ImplementationClassUid": "1.2.276.0.7230010.3.0.3.6.4",
      "ImplementationVersionName": "CAMBRIDGE_001",
      "InstitutionName": "",
      "StationName": "",
      "ValidateAfterCreation": true
    },
    "Logging": {
      "LogLevel": "Information",
      "LogFolder": "C:\\CamBridge\\Logs",
... 
``` 
 
## Recent Changes 
 
### CHANGELOG.md (last 3 versions
```markdown 
# CamBridge Changelog

All notable changes to CamBridge will be documented in this file.  
�� 2025 Claude's Improbably Reliable Software Solutions

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.4.4] - 2025-06-01 19:21
### Added
- Core functionality test with real Ricoh G900 II JPEG
- Enhanced EXIF parser with line break and encoding fixes
- Flexible QRBridge parser for incomplete data
- mappings.json for TestConsole
- Hex dump debugging in TestConsole

### Fixed
- EXIF encoding issues with German umlauts
- Parser handling of camera line breaks in barcode data
- NotificationService null reference warnings

### Changed
- Parser now handles incomplete QRBridge data (3 of 5 fields)
- Improved debug logging for QRBridge parsing

### Discovered
- Ricoh G900 II only saves first 3 QRBridge fields (gender/comment missing)
- Camera inserts "GCM_TAG " prefix in UserComment

### Known Issues
- Settings page still crashes on navigation
- QRBridge data truncation needs investigation

## [0.4.3] - 2025-06-01 17:15
### Added
- Vogon Poetry Easter Egg - tribute to Douglas Adams
  - Activated by typing "42" on About page
  - Amiga-style Boing Ball sprite animation (WritePixels implementation)
  - Scrolling rainbow text with retro effects
  - Vogonian poetry about DICOM with ERROR HAIKU
  - Guru Meditation error messages
- Dead Letters page basic functionality
  - DataGrid with items display
  - Connection status indicator
  - Retry functionality per item

### Fixed
- Dead Letters navigation crash - fixed DI registration
- AboutPage keyboard focus issues
- Removed unsafe code for better stability
- Fixed nullable reference warnings

### Known Issues
- Settings page crashes on navigation (ViewModel initialization)

## [0.4.2] - 2025-06-01 15:10
``` 
 
 
## Summary 
- Total files collected: 20 
- Focus: Settings Page DI/Navigation issues 
- Next: Mapping Editor with Drag and Drop 
- Target versions: v0.4.5 (Fix) and v0.5.0 (Config Management) 
