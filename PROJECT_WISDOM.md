# CamBridge Project Wisdom & Conventions
**Letzte Aktualisierung:** 2025-06-01, 17:15 Uhr  
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
Nächste Aufgabe: Core-Funktionalität Test mit Original Ricoh JPEG

Stand: v0.4.3 - GUI soweit entwickelt, aber Core nie getestet!

PRIORITÄTEN:
1. Original Ricoh JPEG mit QRBridge-Daten testen
2. JPEG → DICOM Konvertierung durchführen
3. Erzeugtes DICOM analysieren und validieren
4. Prüfen ob Patient/Study-Daten korrekt gemappt wurden

Danach:
- Settings Page Crash Fix (v0.4.4)
- Dashboard Polish (v0.5.0)

Notizen:
- GUI-Entwicklung pausieren, erst Core testen!
- Service sollte laufen für den Test
- Mapping-Konfiguration prüfen
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

### Settings Page (v0.4.1-v0.4.4)
- **CRASHT NOCH:** Navigation zur Settings Page führt zu Absturz
- **Vermutliche Ursache:** SettingsViewModel Initialisierung oder DI
- **TODO v0.4.4:** Crash debuggen und fixen

## ⏰ ZEITMANAGEMENT (KRITISCH!)

### Projekt-Timeline
- **Entwicklungsstart:** 30.05.2025, 20:30:44 Uhr (exakt!)
- **Letzte Aktualisierung:** 01.06.2025, 17:15 Uhr
- **Entwicklungszeit bisher:** ~44.7 Stunden (inkl. Nachtschichten!)
- **WICHTIG:** IMMER nach aktueller Zeit fragen für CHANGELOG!

### Changelog-Regel
```
## [Version] - YYYY-MM-DD HH:MM  ← Mit exakter Zeit!
```

### Wichtige Erkenntnis
**Timestamps erzählen Geschichten!**
- Nachtschichten erkennen (01:17, 02:22)
- "Duplikate" entlarven (9 Std Unterschied = kein Duplikat!)
- Arbeitsintensität verstehen (44 Std in 2,8 Tagen)

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
```

### Arbeitszeiten-Analyse
- **Nachtschichten:** DICOM (01:17), GUI (02:22)
- **Schnelle Fixes:** v0.0.2 Duplikat in 78 Sekunden
- **Lange Sessions:** 9 Stunden zwischen v0.4.0 Commits
- **Gesamt:** ~44.7 Stunden in 2,8 Tagen!

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
9. **Phase 8.5:** Settings-Page (v0.4.1) ← FERTIG aber crasht
10. **Phase 10:** Dead Letters Management (v0.4.2) - UI FERTIG
11. **Phase 10.5:** Dead Letters Fix & Easter Egg (v0.4.3) ← FERTIG

#### 🚧 Aktuelle Phase
12. **Phase 10.6:** Settings Page Fix (v0.4.4)
    - Settings Page DI-Problem beheben
    - Crash beim Navigieren fixen
    
#### 🚧 Verbleibende Phasen
13. **Phase 11:** Dashboard Polish & Performance (v0.5.0) - 1 Chat
    - Performance-Optimierung
    - UI-Verbesserungen
    - Feature-complete Beta
    
14. **Phase 12:** PACS Integration (v0.6.0) - Optional, 2 Chats
    - DICOM C-STORE SCU
    - Network Transfer
    
15. **Phase 13:** Deployment (v0.9.0 → v1.0.0) - 1 Chat
    - MSI Installer
    - Release Pipeline

### Zeitschätzung bis v1.0.0
- Phase 10.6: 0.5 Chat (v0.4.4)
- Phase 11: 1 Chat (v0.5.0)
- Phase 12: 2 Chats (optional)
- Phase 13: 1 Chat
- **Gesamt:** 3-5 Chats

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
- **v0.4.4** - Settings Page Fix
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
- CHANGELOG.md MUSS als vollständiges Artefakt existieren!
- Keine Updates ohne Basis-Artefakt
- Vollständigkeit ist Pflicht

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

Aktueller Stand: v0.4.3
- GUI Framework mit Dashboard ✓
- Service Control (Phase 9) ✓  
- Settings-Page (crasht noch)
- Dead Letters funktioniert ✓
- Vogon Poetry Easter Egg ✓

Nächste Aufgabe: Settings Page Crash Fix (v0.4.4)

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
   <AssemblyVersion>0.4.3.0</AssemblyVersion>
   <FileVersion>0.4.3.0</FileVersion>
   <InformationalVersion>0.4.3</InformationalVersion>
   ```

2. **CHANGELOG.md:** Mit exakter Zeit
   ```markdown
   ## [0.4.3] - 2025-06-01 17:15
   ### Added
   - Vogon Poetry Easter Egg - tribute to Douglas Adams
   - Dead Letters page basic functionality
   
   ### Fixed
   - Dead Letters navigation crash
   - AboutPage keyboard focus issues
   
   ### Known Issues
   - Settings page crashes on navigation
   ```

3. **MainWindow.xaml:** Title mit Version
   ```xml
   Title="CamBridge Configuration v0.4.3"
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

## 🏁 Quick Reference

### Aktuelle Version: v0.4.3
### Tatsächlicher Stand: 
- ✅ Service Control (Phase 9) in v0.4.0 erledigt
- ✅ Settings Page (v0.4.1) implementiert (crasht aber noch)
- ✅ Dead Letters UI (v0.4.2) implementiert
- ✅ Vogon Poetry Easter Egg (v0.4.3) funktioniert
- ✅ Dead Letters Navigation (v0.4.3) gefixt
### Nächste Aufgabe: Core-Funktionalität Test mit Ricoh JPEG
### Architektur: Enterprise-Level (und das ist GUT so!)
### Kontext: Medizinische Software mit 0% Fehlertoleranz
### Geschätzte v1.0.0: 3-5 Chats

### V.O.G.O.N. Commands:
- **VOGON INIT** - Automatischer Start
- **WISDOM:** - Live-Updates ins PROJECT_WISDOM
- **CLAUDE:** - Notizen für nächste Instanz
- **VOGON CLOSE** - Chat-Abschluss mit Versionierung
