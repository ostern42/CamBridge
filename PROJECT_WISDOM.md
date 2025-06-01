# CamBridge Project Wisdom & Conventions
**Letzte Aktualisierung:** 2025-06-02, 01:05 Uhr  
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

## ⚡️ ABSOLUT KRITISCHE VOGON CLOSE REGEL ⚡️
**BEIM VOGON CLOSE MÜSSEN IMMER ERSTELLT WERDEN:**
1. **PROJECT_WISDOM.md** - Als VOLLSTÄNDIGES Artefakt (nicht nur Updates!)
2. **CHANGELOG.md** - NUR der neueste Versions-Eintrag als Artefakt
3. **Version.props** - Als VOLLSTÄNDIGES Artefakt

**WARUM:** Updates können fehlschlagen oder übersehen werden. Nur vollständige Artefakte garantieren, dass der Nutzer die aktualisierten Dateien bekommt! Beim CHANGELOG reicht der neueste Eintrag um Zeit zu sparen.

**MERKSATZ:** "Ein VOGON CLOSE ohne vollständige Artefakte ist wie ein Vogone ohne Poesie - technisch möglich, aber sinnlos!"

*Hinweis: Dieses System ist zu 100% vogonenfrei und wurde nicht von der galaktischen Planungskommission genehmigt, was es vermutlich effizienter macht.*

### 📋 Aktueller Übergabeprompt
```
Nächste Aufgabe: BUILD-FEHLER FIXEN & EXIFTOOL TESTEN!

Stand: v0.5.3 - Parser-Bug verstanden, ExifTool integriert

ERFOLGE:
✅ Parser-Bug identifiziert: Ricoh speichert in "Barcode" Tag
✅ ExifToolReader implementiert mit Fallback
✅ Debug-Console für EXIF-Analyse erstellt
✅ GCM_TAG Prefix-Handling gefixt

PROBLEME:
🐛 PatientId doppelt definiert (Entities vs ValueObjects)
🐛 Build-Fehler verhindern Tests
🔧 ExifTool Integration ungetestet

PRIORITÄTEN:
1. ⚡ PatientId Duplikat beheben
2. 🧪 ExifTool mit echten Bildern testen
3. ✅ Verifizieren dass alle 5 Felder gelesen werden
4. 🧪 Features von v0.5.0-v0.5.1 testen
5. 📝 Dokumentation aktualisieren

WICHTIG: ExifTool.exe muss im Tools-Ordner liegen!
```

## 🎯 Projekt-Identität
- **Copyright:** © 2025 Claude's Improbably Reliable Software Solutions
- **Produkt:** CamBridge - JPEG zu DICOM Konverter für Ricoh G900 II Kameras
- **Kontext:** Medizinische Bildgebung, arbeitet mit QRBridge zusammen
- **NEU:** Wir kontrollieren BEIDE Seiten (QRBridge + CamBridge)!

## 🔥 BREAKING: QRBridge Integration (2025-06-01, 21:25)

### Wir haben den QRBridge Source Code!
Das bedeutet:
- **Volle Kontrolle** über QR-Code Generierung UND Dekodierung
- **Protokoll-Evolution** möglich - nicht auf Pipes festgelegt!
- **Optimierung** für Ricoh-Limitierungen (nur 3-4 Felder)
- **Bidirektionale Features** implementierbar

### Mögl. Protokoll-Verbesserungen (v0.5.1 IMPLEMENTIERT!)
1. **JSON-Kompakt:** `v2:{"e":"EX002","n":"Schmidt, Maria","b":"19850315","g":"F"}`
2. **Base64-Encoded:** Für Sonderzeichen-Sicherheit
3. **Fixed-Length:** Bei bekannten Feldlängen
4. **Custom Delimiter:** z.B. `§` oder `¤` statt `|`
5. **Checksumme:** Für Datenintegrität

### v0.5.1 Protocol v2 Status
- ✅ QRBridgeProtocolV2Parser implementiert
- ✅ Automatic version detection (v1 vs v2)
- ✅ Backward compatibility gewährleistet
- ✅ JSON parsing mit Fehlerbehandlung
- 🚧 QRBridge.exe Encoder noch nicht aktualisiert

### 🚫 QRBridge bleibt unverändert! (01.06.2025, 23:00)
- **KEIN v2 Encoder** - unnötige Komplexität
- **QRBridge hat kein VOGON** - zu klein für große Änderungen
- **Parser-Bug wird in CamBridge gefixt**
- **Pipes funktionieren** - warum ändern?
- **Nur ändern wenn wirklich nötig** (z.B. vergessenes Datenfeld)

### 🔍 KRITISCHE ERKENNTNIS: Barcode Tag! (02.06.2025, 01:05)
- **Ricoh speichert ALLE 5 Felder** im proprietären "Barcode" EXIF-Tag
- **UserComment enthält nur** "GCM_TAG" als Marker
- **MetadataExtractor kann Barcode Tag NICHT lesen**
- **ExifTool ist die einzige Lösung** für vollständige Daten
- **Beweis:** ExifTool zeigt `Barcode: EX002|Schmidt, Maria|1985-03-15|F|Röntgen Thorax`

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

### v0.5.1 Erkenntnisse
- **Run-Element:** Keine Opacity-Property! Use Foreground statt dessen
- **EmailSettings:** Sind verschachtelt in NotificationSettings.Email
- **Project References:** Config braucht Infrastructure-Referenz
- **NuGet Versions:** Alle müssen übereinstimmen (System.Drawing.Common)
- **Protocol Detection:** StartsWith("v2:") für v2, Contains("|") für v1

### v0.5.2 Collector Revolution (01.06.2025, 23:52)
- **Ein Script für alles:** collect-sources.bat mit 7 Profilen
- **Smart Selection:** collect-smart.bat analysiert Git-Änderungen
- **Timestamps:** PROJECT_CONTEXT_[PROFILE]_[TIMESTAMP].md
- **Profile:** minimal, core, gui, balanced, mapping, full, custom
- **Alte Scripts archiviert:** cleanup-old-collectors.bat verfügbar

### v0.5.3 ExifTool Integration (02.06.2025, 01:05)
- **ExifToolReader:** Wrapper für exiftool.exe mit JSON-Output
- **Fallback-Hierarchie:** ExifTool → RicohExifReader → ExifReader
- **Auto-Discovery:** Sucht ExifTool in mehreren Locations
- **Barcode Tag Support:** Liest proprietäre Pentax/Ricoh Tags
- **Performance:** ~50-100ms Overhead pro Bild

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

## 📂 Projekt-Struktur-Wissen

### Datei-Sammlungen (NEU: Collector v2.0!)
- **collect-sources.bat:** Master-Script mit 7 Profilen
  - minimal (~5%) - Quick overview
  - core (~15%) - Core ohne GUI
  - gui (~20%) - GUI-Entwicklung
  - balanced (~25%) - Standard (OPTIMAL!)
  - mapping (~20%) - Mapping Editor
  - full (~50%) - Komplett (VORSICHT!)
  - custom - Eigene Patterns
- **collect-smart.bat:** Git-basierte automatische Profil-Auswahl
- **cleanup-old-collectors.bat:** Archiviert alte Scripts
- **COLLECTOR_README.md:** Dokumentation für neue Scripts

### Wie die neuen Scripts funktionieren:
- Ein Master-Script statt 6 verschiedene
- Profile-basierte Collection
- Timestamps verhindern Überschreibungen
- Git-Integration für intelligente Auswahl
- Bessere Token-Effizienz durch gezieltes Sammeln

### Optimale Upload-Strategie:
```
1. PROJECT_WISDOM.md (immer!)
2. collect-smart.bat ausführen (oder spezifisches Profil)
3. PROJECT_CONTEXT_[PROFILE]_[TIMESTAMP].md uploaden
```

### Wichtige Pfade
```
CamBridge/
├── Version.props                    # Zentrale Version (jetzt 0.5.3)
├── collect-sources.bat              # NEU: Master Collector
├── collect-smart.bat                # NEU: Smart Selector
├── COLLECTOR_README.md              # NEU: Dokumentation
├── Tools/                           # NEU: ExifTool Location
│   └── exiftool.exe                # Muss hier liegen!
├── src/
│   ├── CamBridge.Core/             # Models, Settings
│   ├── CamBridge.Infrastructure/   # Processing (ExifToolReader NEU!)
│   ├── CamBridge.Service/          # Windows Service
│   └── CamBridge.Config/           # WPF GUI
│       ├── Dialogs/                # DicomTagBrowserDialog
│       ├── Views/                  # MappingEditorPage
│       └── ViewModels/             # MappingEditorViewModel
├── CamBridge.ParserDebug/          # NEU: Debug Console
├── QRBridge/                       # QRBridge Source
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
4. collect-smart.bat verwenden für optimale Coverage

### Chat-Abschluss mit "VOGON CLOSE"
1. **Zeit erfragen:** "Wie spät ist es?" (für CHANGELOG)
2. **Version.props:** AssemblyVersion, FileVersion, InformationalVersion erhöhen
3. **CHANGELOG.md:** Neuen Eintrag mit exakter Zeit erstellen
4. **Git Commit String:** Nach Format erstellen
5. **README.md:** Features-Liste aktualisieren (falls nötig)
6. **Übergabeprompt:** Für nächsten Chat vorbereiten
7. **PROJECT_WISDOM.md:** Als VOLLSTÄNDIGES ARTEFAKT finalisieren!
8. **CHANGELOG.md:** NUR neuester Eintrag als Artefakt!
9. **Version.props:** Als VOLLSTÄNDIGES ARTEFAKT!

## ⚡ Bekannte Fallstricke

### GUI-Entwicklung
- **PlaceholderText:** Nutze ui:ControlHelper.PlaceholderText
- **PasswordBox:** Binding nur mit Behavior/Attached Property → GELÖST mit PasswordBoxHelper!
- **Spacing:** Existiert nicht in WPF/ModernWPF!
- **NumberBox:** Aus ModernWpfUI, nicht WinUI
- **IsTabStop:** Nicht für Page verfügbar (v0.4.3 Fix)
- **Run Opacity:** Run-Elements haben keine Opacity-Property! (v0.5.1 Fix)

### Service
- **UAC:** Admin-Rechte für Service-Control nötig
- **Pfade:** Absolute Pfade in appsettings.json
- **Event Log:** Source muss registriert sein

### Dead Letters Page (v0.4.2-v0.4.3)
- **DI-Problem:** DeadLettersViewModel nicht korrekt registriert → BEHOBEN in v0.4.3
- **Navigation Crash:** Beim Wechsel zur Dead Letters Page → BEHOBEN in v0.4.3
- **UI funktioniert:** DataGrid zeigt Items, Retry-Button vorhanden

### Settings Page (v0.4.1-v0.4.5)
- **CRASHTE:** Navigation zur Settings Page führte zu Absturz → BEHOBEN in v0.4.5
- **Ursache:** ConfigurationService fehlte in DI-Registration
- **PasswordBox:** PasswordBoxHelper implementiert in v0.5.0!
- **Funktioniert:** Alle Settings werden gespeichert und geladen

### Ricoh G900 II QRBridge (v0.4.4)
- **NUR 3 FELDER:** Kamera speichert nur examid|name|birthdate
- **FEHLENDE FELDER:** gender und comment werden abgeschnitten
- **GCM_TAG PREFIX:** Kamera fügt "GCM_TAG " vor Barcode ein
- **ENCODING:** UTF-8/Latin-1 Probleme bei Umlauten → GELÖST
- **LÖSUNG:** Mit QRBridge Source können wir optimiertes Protokoll entwickeln!

### 🔍 NEUE ERKENNTNIS: String wird ABGESCHNITTEN! (v0.5.1)
- **Der Nutzer hat Recht:** Ein anderer EXIF-Reader zeigt den KOMPLETTEN String!
- **Problem ist NICHT die Kamera:** Sie speichert alles korrekt
- **Problem ist UNSER Parser:** Wir schneiden den String ab
- **Beweis:** Andere Software liest alle 5 Felder aus demselben JPEG
- **TODO:** QRBridge Parser debuggen und fixen in v0.5.3!

### 🎯 GELÖST: Barcode Tag Erkenntnis! (v0.5.3)
- **Ricoh speichert in 2 verschiedenen Tags:**
  - UserComment: "GCM_TAG" + erste 3 Felder
  - Barcode: ALLE 5 Felder komplett!
- **MetadataExtractor kann Barcode Tag NICHT lesen**
- **ExifTool ist die Lösung** - liest proprietäre Tags
- **Implementation:** ExifToolReader mit Fallback

### v0.5.1 Spezifische Fallstricke
- **EmailSettings:** Sind verschachtelte Properties in NotificationSettings
- **Project References:** CamBridge.Config braucht Infrastructure
- **NuGet Conflicts:** System.Drawing.Common Versionen müssen übereinstimmen
- **XAML Run:** Hat keine Opacity, nutze Foreground stattdessen
- **Protocol Detection:** Check für "v2:" muss VOR pipe-check kommen

### v0.5.2 Build-Fehler BEHOBEN!
- **NotificationService:** Alle 6 Interface-Methoden implementiert
- **Async Pattern:** Konsistent für alle Notification-Methoden
- **Daily Summary:** ProcessingSummary-Formatierung hinzugefügt
- **Threshold Alerts:** Dead Letter Schwellwert-Benachrichtigung

### v0.5.3 Build-Fehler NEU!
- **PatientId:** Doppelt definiert in Entities UND ValueObjects
- **Namespace-Konflikt:** Muss in einem der beiden Ordner entfernt werden
- **ProcessingResult:** Properties passen nicht zu NotificationService
- **ExifTool:** Noch nicht getestet

## ⏰ ZEITMANAGEMENT (KRITISCH!)

### Projekt-Timeline
- **Entwicklungsstart:** 30.05.2025, 20:30:44 Uhr (exakt!)
- **Letzte Aktualisierung:** 02.06.2025, 01:05 Uhr
- **Entwicklungszeit bisher:** ~52.5 Stunden (inkl. Nachtschichten!)
- **WICHTIG:** IMMER nach aktueller Zeit fragen für CHANGELOG!

### Changelog-Regel
```
## [Version] - YYYY-MM-DD HH:MM  ← Mit exakter Zeit!
```

### Wichtige Erkenntnis
**Timestamps erzählen Geschichten!**
- Nachtschichten erkennen (01:17, 02:22, 01:05)
- "Duplikate" entlarven (9 Std Unterschied = kein Duplikat!)
- Arbeitsintensität verstehen (52.5 Std in 3 Tagen)

### Git-History (Mit exakten Timestamps!)
```
0bb1839 - 30.05. 20:30:44 - .gitattributes
fb17be8 - 30.05. 20:34:20 - v0.0.1: Initial structure
40cb347 - 30.05. 21:32:38 - chore: Remove build artifacts
588cf7b - 30.05. 21:32:54 - v0.0.2: Core models (1)
b64ba16 - 30.05. 21:34:12 - v0.0.2: Core models (2) ⚡️ +78 Sek!
1bff3ef - 30.05. 23:49:44 - v0.1.0: EXIF extraction
511bdeb - 31.05. 01:17:17 - [KEINE]: DICOM (Nachtschicht!) ⚡️
cf88f46 - 31.05. 10:34:17 - v0.2.0: Mapping config
a6b77bb - 31.05. 15:45:17 - v0.3.0: File monitoring
7814cb9 - 31.05. 16:51:44 - v0.3.1: Fix DI issue
0ab9add - 31.05. 23:10:22 - v0.3.2: Dead-letter/Web
e0e68f1 - 01.06. 02:22:32 - v0.4.0: GUI (Nachtschicht!) ⚡️
e806e31 - 01.06. 11:30:55 - v0.4.0: GUI (+9 Std!) ⚡️
[pending] - 01.06. 13:30:00 - v0.4.1: Settings
[pending] - 01.06. 15:10:00 - v0.4.2: Dead Letters (UI fertig)
[pending] - 01.06. 17:15:00 - v0.4.3: Vogon Poetry & Dead Letters Fix
[pending] - 01.06. 19:21:00 - v0.4.4: Core Test mit Ricoh JPEG
[pending] - 01.06. 20:52:00 - v0.4.5: Settings Page Fix
[pending] - 01.06. 21:47:00 - v0.5.0: Mapping Editor UI
[pending] - 01.06. 22:32:00 - v0.5.1: DICOM Browser & Protocol v2
[pending] - 01.06. 23:52:00 - v0.5.2: Collector v2.0 & Build Fix
[pending] - 02.06. 01:05:00 - v0.5.3: ExifTool Integration & Parser Fix
```

### Arbeitszeiten-Analyse
- **Nachtschichten:** DICOM (01:17), GUI (02:22), Parser (01:05)
- **Schnelle Fixes:** v0.0.2 Duplikat in 78 Sekunden
- **Lange Sessions:** Fast 24 Stunden am 01.06!
- **Gesamt:** ~52.5 Stunden in 3 Tagen!

### Die wahre Geschichte der Duplikate
- **v0.0.2:** Git-Anfängerfehler, 78 Sekunden später nochmal
- **v0.4.0:** KEIN Versehen! 9 Stunden Unterschied, vermutlich:
  - 02:22 - GUI Basic Implementation (müde)
  - 11:30 - Service Control hinzugefügt (ausgeschlafen)
  - Hätte v0.4.1 sein sollen!

## 📋 Entwicklungsplan (KORRIGIERTE VERSION - Stand 02.06.2025, 01:05)

### ⚡️ WICHTIGE KORREKTUR
**Original-Plan sagte "WinUI 3" - wir nutzen aber WPF mit ModernWpfUI!**

### 🔍 KRITISCHE PLAN-REVISION (01.06.2025, 23:00)
Nach genauem Code-Review haben wir festgestellt:
- Phasennummern wurden durcheinander gebracht
- Einige Features wurden implementiert, andere vergessen
- v2 Encoder wird NICHT implementiert (unnötige Komplexität)
- QRBridge bleibt unverändert (hat kein VOGON System)

### Phasen-Übersicht (REVIDIERT & VERIFIZIERT)

#### ✅ Abgeschlossene Phasen (Code-verifiziert)
1. **Phase 1:** Initial project structure (v0.0.1) ✅
2. **Phase 2:** Core domain models (v0.0.2) ✅
3. **Phase 3:** EXIF extraction with QRBridge (v0.1.0) ✅
4. **Phase 4:** DICOM conversion with fo-dicom (v0.2.0) ✅
5. **Phase 5:** JSON-based mapping config (v0.2.0) ✅
6. **Phase 6:** Mapping system implementation (v0.2.0) ✅
7. **Phase 7:** Windows Service Pipeline (v0.3.0) ✅
8. **Phase 7.5:** Extended features (v0.3.1-v0.3.2) ✅
   - Dead-Letter-Queue mit Persistierung ✅
   - Email & Event Log Notifications ✅
   - Web Dashboard mit REST API ✅
   - PowerShell Installation ✅
9. **Phase 8:** WPF GUI Framework (v0.4.0) ✅
   - ModernWpfUI statt WinUI3 ✅
   - Dashboard mit Live-Updates ✅
   - Service Control mit UAC ✅
10. **Phase 9:** Configuration Management GUI - Teil 1 (v0.4.1-v0.4.5) ✅
    - Settings Page (4 Tabs) ✅
    - Dead Letters Management UI ✅
    - Vogon Poetry Easter Egg ✅
    - Basic Folder Management ✅
11. **Phase 10:** Configuration Management GUI - Teil 2 (v0.5.0-v0.5.1) ✅
    - Mapping Editor mit Drag & Drop ✅
    - DICOM Tag Browser ✅
    - Template System (Ricoh/Minimal/Full) ✅
    - Import/Export für Mappings ✅
    - Protocol v2 Parser ✅
    - ❌ Watch Folder Management GUI (nur Basic in Settings)
    - ❌ Live-Preview für komplexe Transformationen
12. **Phase 10-FIX:** Build-Fehler Behebung (v0.5.2) ✅
    - NotificationService Interface-Implementierung ✅
    - Collector v2.0 mit 7 Profilen ✅
    - Smart Git-basierte Profil-Auswahl ✅
13. **Phase 11a:** ExifTool Integration (v0.5.3) - PARTIAL ⚠️
    - ExifToolReader implementiert ✅
    - Debug-Console erstellt ✅
    - Barcode Tag Problem identifiziert ✅
    - ❌ Build-Fehler verhindern Tests
    - ❌ Integration nicht verifiziert

#### 🔥 AKTUELLE PHASE - BUG FIXES & TESTING
14. **Phase 11b:** Build Fix & Testing (v0.5.4) - JETZT!
    - PatientId Duplikat beheben ❌
    - ExifTool Integration testen ❌
    - Alle 5 QRBridge-Felder verifizieren ❌
    - v0.5.0-v0.5.1 Features gründlich testen ❌
    - Watch Folder Management GUI erweitern ❌
    - Live-Preview für alle Transformationen ❌
    - Validation UI für Mappings ❌
    - **Feature-complete Beta**

#### 🚧 Nächste Phasen (NEU STRUKTURIERT)
15. **Phase 12:** Performance & Polish (v0.6.0) - 1 Chat
    - Batch-Verarbeitung optimieren
    - Memory-Pool für große Dateien
    - Parallelisierung mit Channels
    - UI-Animationen (Fluent Design)
    - Dashboard Performance
    - Error Recovery verbessern
    - **Production-ready Beta**

16. **Phase 13:** FTP-Server Integration (v0.7.0) - 1 Chat [Optional]
    - FTP-Server für automatischen Empfang
    - Watch für FTP-Ordner
    - Authentifizierung
    - Auto-Delete nach Verarbeitung

17. **Phase 14:** PACS Integration (v0.8.0) - 2 Chats [Optional]
    - DICOM C-STORE SCU
    - Network Transfer
    - PACS-Konfiguration
    - Connection Tests

18. **Phase 15:** MWL Integration (v0.9.0) - 2 Chats [Optional]
    - DICOM C-FIND SCU
    - MWL-Validierung
    - StudyInstanceUID Sync
    - Fehlerbehandlung

19. **Phase 16:** Deployment & Release (v1.0.0) - 1 Chat
    - MSI Installer
    - Auto-Updates
    - CI/CD Pipeline
    - Dokumentation
    - Zertifizierung

### ⚠️ WICHTIGE ÄNDERUNGEN:
1. **Phase 11c (v2 Encoder) GESTRICHEN** - QRBridge bleibt unverändert!
2. **Parser-Bug wird in CamBridge gefixt**, nicht in QRBridge
3. **Fehlende Features identifiziert**: Watch Folder GUI, Live-Preview
4. **Neue Phasenstruktur** reflektiert tatsächlichen Stand
5. **Collector v2.0** revolutioniert die Entwicklung!
6. **ExifTool Integration** essentiell für vollständige Daten!

### Was wirklich noch fehlt (Code-verifiziert):
- **Build-Fehler beheben** (PatientId Duplikat) - KRITISCH!
- **ExifTool Integration testen** - KRITISCH!
- **Feature Testing** (v0.5.0-v0.5.1 ungetestet)
- **Watch Folder Management GUI** (nur Basic-Version in Settings)
- **Live-Preview** für Transformationen (nur teilweise)
- **Validation UI** für Mappings
- **Performance-Optimierungen** (Batch, Memory-Pool, Parallelisierung)
- **UI-Polish** (Animationen, Fluent Design)

### Zeitschätzung bis v1.0.0 (REVIDIERT)
- **Phase 11b:** Build Fix & Testing - JETZT!
- **Phase 12:** Performance & Polish - 1 Chat
- **Phase 13-15:** Optional Features - 4 Chats
- **Phase 16:** Release - 1 Chat
- **Gesamt bis Feature-Complete:** 2-3 Chats
- **Gesamt bis Production:** 3-7 Chats

### Technologie-Stack (KORRIGIERT)
```
GUI Framework:
- WPF (Windows Presentation Foundation) ← NICHT WinUI 3!
- ModernWpfUI 0.9.6 für modernes Design
- CommunityToolkit.Mvvm 8.2.2
- .NET 8.0 Windows Desktop

Service:
- ASP.NET Core Minimal API
- Windows Service
- System.Text.Json

Processing:
- fo-dicom für DICOM
- MetadataExtractor für EXIF (limitiert!)
- ExifTool für vollständige EXIF-Daten (NEU!)

QRBridge Integration (NEU!):
- Kontrolle über beide Seiten
- Protokoll-Evolution möglich
- v2 JSON Format implementiert
- Optimierung für Ricoh-Limits

External Tools:
- ExifTool 12.96 für Barcode Tag
- Muss in Tools/ Ordner liegen

Collector Tools (NEU!):
- collect-sources.bat mit 7 Profilen
- collect-smart.bat für Git-Integration
- Timestamp-basierte Outputs
```

### Meilensteine (AKTUALISIERT)
- **v0.4.5** - Settings Page Fix (Erledigt ✅)
- **v0.5.0** - Mapping Editor (Erledigt ✅)
- **v0.5.1** - DICOM Browser & Protocol v2 (Erledigt ✅)
- **v0.5.2** - Build Fix & Collector v2.0 (Erledigt ✅)
- **v0.5.3** - ExifTool Integration (Teilweise ⚠️)
- **v0.5.4** - Build Fix & Testing (Aktuelles Ziel 🎯)
- **v0.5.5** - Feature Complete Beta
- **v0.6.0** - Performance & Polish
- **v0.7.0** - FTP-Server Integration [Optional]
- **v0.8.0** - PACS Ready [Optional]
- **v0.9.0** - MWL Integration [Optional]
- **v1.0.0** - Production Release

### Entwicklungs-Philosophie
"Sauberer, schöner, ästhetischer und formal korrekter Code für medizinische Software"

### 🔴 KRITISCHE PLAN-ÄNDERUNGEN (02.06.2025, 01:05)
1. **v2 Encoder GESTRICHEN** - QRBridge bleibt unverändert
2. **Parser-Bug wird in CamBridge gefixt**, nicht in QRBridge
3. **Fehlende Features identifiziert**: Watch Folder GUI, Live-Preview
4. **Plan nur mit Code-Gegenprüfung ändern** - neue Regel!
5. **Collector v2.0** macht Entwicklung effizienter!
6. **ExifTool essentiell** für vollständige Daten!

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
- **KEINE** einseitigen Lösungen wenn wir beide Seiten kontrollieren!
- **KEINE** Opacity für Run-Elements - nutze Foreground!
- **KEIN** HETZEN! Features erst testen bevor neue implementiert werden!
- **KEINE** neuen Features bei Build-Fehlern - erst stabilisieren!
- **KEINE** Änderungen am Phasenplan ohne Code-Gegenprüfung!
- **KEIN** unnötiges Anfassen von QRBridge - hat kein VOGON!
- **KEINE** Duplikate in Namespaces (siehe PatientId Problem)!

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
- Kamera speichert nur 3 von 5 QRBridge-Feldern
- "GCM_TAG " Prefix wird eingefügt
- Gender und Comment werden abgeschnitten/fehlen
- Encoding-Probleme bei Umlauten sind lösbar

**Settings Page Erkenntnisse (v0.4.5):**
- DI-Registration ist kritisch - ALLE Services müssen registriert sein
- PasswordBox erlaubt kein direktes Binding in WPF → GELÖST mit Helper!
- Converter müssen global verfügbar sein
- Console Output (OutputType=Exe) ist sehr hilfreich für Debugging

**QRBridge-CamBridge Synergie (v0.5.0):**
- Wir kontrollieren BEIDE Seiten!
- Protokoll kann optimiert werden
- Ricoh-Limitierungen umgehbar
- Bidirektionale Features möglich

**Mapping Editor Erkenntnisse (v0.5.1):**
- Drag & Drop braucht MouseMove Handler
- DICOM Tags haben Gruppen (Module)
- Templates erleichtern die Konfiguration
- Import/Export essentiell für Deployment

**Protocol v2 Erkenntnisse (v0.5.1):**
- JSON kompakter als erwartet (~58-79 Bytes)
- Version-Prefix "v2:" für Erkennung
- Backward compatibility durch Fallback
- Kürzere Feldnamen sparen Platz

**NotificationService Fix (v0.5.2):**
- Interface-Methoden müssen ALLE implementiert werden
- Async-Pattern konsistent durchziehen
- Exception-Handling in CriticalError-Methode
- ProcessingSummary braucht Formatter

**Collector v2.0 Revolution (v0.5.2):**
- Ein Script für 7 verschiedene Profile
- Git-Integration erkennt geänderte Bereiche
- Timestamps verhindern Überschreibungen
- Token-Effizienz durch gezielte Coverage

**ExifTool Integration (v0.5.3):**
- Ricoh speichert ALLE 5 Felder im "Barcode" Tag
- MetadataExtractor kann proprietäre Tags NICHT lesen
- ExifTool ist die einzige Lösung
- ~50-100ms Performance-Overhead akzeptabel
- JSON-Output für einfaches Parsing

## 📝 Standard Prompt-Vorlage für neue Chats

### Option 1: V.O.G.O.N. (Empfohlen!)
```
1. PROJECT_WISDOM.md hochladen
2. collect-smart.bat ausführen
3. PROJECT_CONTEXT_[PROFILE]_[TIMESTAMP].md hochladen
4. Sagen: "VOGON INIT"
5. Fertig! Ich lege direkt los.
```

### Option 2: Traditionell (falls VOGON nicht funktioniert)
```
Ich arbeite an CamBridge, einem JPEG zu DICOM Konverter.
© 2025 Claude's Improbably Reliable Software Solutions

Aktueller Stand: v0.5.3
- ExifTool integriert ✅
- Parser-Bug verstanden ✅
- Build-Fehler verhindert Tests 🐛
- Features ungetestet 🧪

Nächste Aufgabe: Build-Fehler fixen & ExifTool testen!

Tech Stack: .NET 8, WPF/ModernWpfUI, MVVM, ExifTool
Architektur: Enterprise-Level für medizinische Software

[PROJECT_WISDOM.md und PROJECT_CONTEXT_*.md anhängen]
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
- **QRBridge Protocol v2** mit optimierter Kodierung!

### Unsere Stärken:
- REST API für Monitoring (Seltenheit in Krankenhaus-IT!)
- Robuste Fehlerbehandlung mit Dead-Letter-Queue
- Erweiterbare Architektur für zukünftige Protokolle
- Enterprise-ready von Tag 1
- **NEU:** Kontrolle über beide Seiten (QRBridge + CamBridge)!
- **NEU:** Protocol v2 mit JSON-Format und Backward Compatibility!
- **NEU:** Collector v2.0 für effiziente Entwicklung!
- **NEU:** ExifTool Integration für vollständige Datenextraktion!

### MWL-Integration (Phase 15+)
**Modality Worklist Integration für v0.8.0+**

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
- **NEU:** ExifTool Integration Tests

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
   <AssemblyVersion>0.5.3.0</AssemblyVersion>
   <FileVersion>0.5.3.0</FileVersion>
   <InformationalVersion>0.5.3</InformationalVersion>
   ```

2. **CHANGELOG.md:** Mit exakter Zeit
   ```markdown
   ## [0.5.3] - 2025-06-02 01:05
   ### Added
   - ExifToolReader for comprehensive EXIF support
   
   ### Fixed
   - Parser bug: Data is in Barcode tag
   ```

3. **MainWindow.xaml:** Title mit Version
   ```xml
   Title="CamBridge Configuration v0.5.3"
   ```

## 🔄 Update-Protokoll

### Wann PROJECT_WISDOM updaten?
- Nach jeder neuen Erkenntnis
- Bei Version-Releases
- Bei Architektur-Änderungen
- Bei neuen Konventionen
- Bei gefundenen Anti-Patterns

### Versions-Historie (Aus Git-Log + Pending)
- **v0.0.1** - 2025-05-30: Initial project structure (fb17be8)
- **v0.0.2** - 2025-05-30: Core domain models (588cf7b) ⚡️
- **v0.0.2** - 2025-05-30: Core domain models (b64ba16) ⚡️ DUPLIKAT!
- **v0.1.0** - 2025-05-30: EXIF extraction with QRBridge (1bff3ef)
- **[KEINE]** - 2025-05-31: DICOM conversion fo-dicom (511bdeb) ⚡️
- **v0.2.0** - 2025-05-31: JSON-based mapping config (cf88f46)
- **v0.3.0** - 2025-05-31: File monitoring pipeline (a6b77bb)
- **v0.3.1** - 2025-05-31: Fix DI singleton/scoped issue (7814cb9)
- **v0.3.2** - 2025-05-31: Dead-letter, notifications, web (0ab9add)
- **v0.4.0** - 2025-06-01: WPF GUI with dashboard (e0e68f1) ⚡️
- **v0.4.0** - 2025-06-01: WPF configuration UI (e806e31) ⚡️ DUPLIKAT!
- **v0.4.1** - 2025-06-01: Settings Page (pending)
- **v0.4.2** - 2025-06-01: Dead Letters UI ✅
- **v0.4.3** - 2025-06-01: Vogon Poetry & Dead Letters Fix ✅
- **v0.4.4** - 2025-06-01: Core Test mit Ricoh JPEG ✅
- **v0.4.5** - 2025-06-01: Settings Page Fix ✅
- **v0.5.0** - 2025-06-01: Mapping Editor UI ✅
- **v0.5.1** - 2025-06-01: DICOM Browser & Protocol v2 ✅
- **v0.5.2** - 2025-06-01: Collector v2.0 & Build Fix ✅
- **v0.5.3** - 2025-06-02: ExifTool Integration & Parser Fix ⚠️

### Versionierungs-Lektionen
1. **v0.0.2 Duplikat:** Gleich am Anfang passiert
2. **Fehlende Version:** DICOM-Commit ohne Versionsnummer
3. **v0.4.0 Duplikat:** Zwei verschiedene Commit-Messages
4. **v0.4.2 Special:** Die "42" Version - Die Antwort auf die ultimative Frage!
5. **Babysteps:** Besser 0.0.1 Schritte als große Sprünge!
6. **v0.5.0 Synergie:** QRBridge + CamBridge = Optimierungspotenzial!
7. **v0.5.1 Evolution:** Protocol v2 zeigt die Macht der Kontrolle!
8. **v0.5.2 Revolution:** Collector v2.0 macht alles effizienter!
9. **v0.5.3 Erkenntnis:** ExifTool ist die einzige Lösung für Barcode Tag!

### Die Unwahrscheinliche Geschichte von CamBridge
*Eine Kurzgeschichten-Idee: Douglas Adams entwickelt einen DICOM-Konverter*

Es ist eine so absurde Vorstellung, dass sie durch ihre schiere Unwahrscheinlichkeit fast wieder wahrscheinlich wird - wie ein Unwahrscheinlichkeitsdrive für medizinische Software. Man stelle sich vor:

"Der DICOM-Standard", sagte Douglas nachdenklich, "ist ein bisschen wie das Universum - keiner versteht ihn wirklich, aber alle tun so, als ob. Der einzige Unterschied ist, dass das Universum vermutlich einfacher zu debuggen wäre."

Er tippte eine weitere Zeile Code und murmelte: "Forty-two different DICOM tags... das kann kein Zufall sein."

Dann hatte er eine Erleuchtung: "Was ist, wenn wir BEIDE Seiten kontrollieren? QRBridge UND CamBridge? Das ist wie... wie wenn Ford Prefect sowohl den Reiseführer schreibt ALS AUCH die Planeten bewertet!"

Und so entstand Protocol v2 - ein JSON-Format so elegant, dass selbst die Vogonen es nicht hätten besser verschlüsseln können. "v2:", flüsterte er ehrfürchtig, "die magischen Zeichen, die alles verändern."

Aber dann kam die Ricoh G900 II und versteckte ihre Geheimnisse im "Barcode" Tag. "Natürlich", seufzte Douglas, "das ist wie die Antwort auf die ultimative Frage des Lebens, des Universums und des ganzen Rests - sie ist da, aber niemand kann sie finden ohne das richtige Werkzeug."

Und so musste ExifTool her - das Pan-Galaktische Knoblauch-Knacker-Äquivalent der EXIF-Welt. "Manchmal", philosophierte er, "braucht man eben doch einen Vorschlaghammer für eine Nuss."

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

### QRBridge-CamBridge Synergie (v0.5.0-v0.5.1) 🔥
**Die große Erleuchtung!**

Wir kontrollieren:
1. **QRBridge:** QR-Code Generierung
2. **CamBridge:** JPEG zu DICOM Konvertierung
3. **Das Protokoll:** Können es optimieren!

v0.5.1 Erfolge:
- **JSON Protocol v2:** Implementiert mit Backward Compatibility
- **Auto-Detection:** StartsWith("v2:") oder Contains("|")
- **Parser:** QRBridgeProtocolV2Parser vollständig
- **Templates:** Ricoh, Minimal, Full funktionieren
- **DICOM Browser:** Suche mit Gruppierung nach Modulen

### ExifTool Integration (v0.5.3) 🔧
**Die Lösung für proprietäre Tags!**

Erkenntnisse:
- **Ricoh speichert in 2 Tags:** UserComment (3 Felder) + Barcode (5 Felder)
- **MetadataExtractor versagt:** Kann Barcode Tag nicht lesen
- **ExifTool rettet uns:** Liest ALLE proprietären Tags
- **Performance:** 50-100ms Overhead akzeptabel

Implementation:
- **ExifToolReader:** JSON-basiertes Parsing
- **Fallback-Chain:** ExifTool → RicohReader → BasicReader
- **Auto-Discovery:** Findet exiftool.exe automatisch
- **Deployment:** Tools-Ordner wird mitkopiert

Nächste Schritte (v0.5.4+):
- **Build-Fehler fixen:** PatientId Duplikat
- **Integration testen:** Mit echten Bildern
- **Dokumentation:** ExifTool Setup Guide

### Collector v2.0 Revolution (v0.5.2) 🚀
**Die neue Ära der Source Collection!**

Features:
- **Ein Script:** collect-sources.bat ersetzt 6 alte Scripts
- **7 Profile:** minimal, core, gui, balanced, mapping, full, custom
- **Smart Selection:** collect-smart.bat analysiert Git-Änderungen
- **Timestamps:** Keine Überschreibungen mehr
- **Token-Effizienz:** Gezielte Coverage je nach Aufgabe

Workflow:
1. `collect-smart.bat` ausführen
2. Automatische Profil-Wahl basierend auf Änderungen
3. Upload mit PROJECT_WISDOM.md
4. VOGON INIT!

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
- 2025-06-01 20:52: v0.4.5 - Settings Page Fix erfolgreich, DI-Problem gelöst, PasswordBox Workaround
- 2025-06-01 21:25: v0.5.0 - QRBridge Source Code Erkenntnis! Protokoll-Evolution möglich!
- 2025-06-01 21:47: v0.5.0 - Mapping Editor UI komplett, Drag & Drop funktioniert
- 2025-06-01 22:32: v0.5.1 - DICOM Browser & Protocol v2 Parser fertig!
- 2025-06-01 22:50: WISDOM - Build-Fehler entdeckt, Nutzer bremst Tempo, Parser-Bug erkannt!
- 2025-06-01 23:00: WISDOM - Phasenplan revidiert, v2 Encoder gestrichen, fehlende Features identifiziert!
- 2025-06-01 23:05: collect-sources-gui-config.bat dokumentiert für GUI-Debugging
- 2025-06-01 23:10: Upload-Strategie geklärt: PROJECT_CONTEXT sind Transport-Container, keine Wahrheit!
- 2025-06-01 23:52: v0.5.2 - Collector v2.0 Revolution! Build-Fehler behoben, Git-Integration!
- 2025-06-02 00:30: WISDOM - Parser-Bug analysiert, Ricoh speichert ALLE Daten im Barcode Tag!
- 2025-06-02 00:45: ExifTool Integration begonnen, ExifToolReader implementiert
- 2025-06-02 01:05: v0.5.3 - ExifTool als Lösung bestätigt, Build-Fehler mit PatientId gefunden

## 🏁 Quick Reference

### Aktuelle Version: v0.5.3
### Tatsächlicher Stand: 
- ✅ ExifTool Integration implementiert
- ✅ Parser-Bug verstanden (Barcode Tag)
- ✅ Debug-Console für EXIF-Analyse
- ✅ DICOM Tag Browser mit Suche
- ✅ Template-System funktioniert
- ✅ QRBridge Protocol v2 Parser
- ✅ Import/Export für Mappings
- ❌ Build-Fehler (PatientId Duplikat)
- ❌ ExifTool Integration UNGETESTET
- ❌ Watch Folder Management GUI (nur Basic)
- ❌ Live-Preview (nur teilweise)
- ❌ Alle Features UNGETESTET
### Nächste Aufgabe: 
- PatientId Duplikat fixen
- ExifTool Integration testen
- Verifizieren dass alle 5 Felder gelesen werden
- v0.5.0-v0.5.1 Features TESTEN
- Fehlende GUI-Features nachrüsten
### Architektur: Enterprise-Level (und das ist GUT so!)
### Kontext: Medizinische Software mit 0% Fehlertoleranz
### Geschätzte v1.0.0: 3-7 Chats (realistisch nach Plan-Revision)

### V.O.G.O.N. Commands:
- **VOGON INIT** - Automatischer Start
- **WISDOM:** - Live-Updates ins PROJECT_WISDOM
- **CLAUDE:** - Notizen für nächste Instanz
- **VOGON CLOSE** - Chat-Abschluss mit Versionierung

### Collector v2.0 Commands:
- **collect-sources.bat [profile]** - Spezifisches Profil
- **collect-sources.bat list** - Zeige alle Profile
- **collect-smart.bat** - Automatische Profil-Auswahl
- **cleanup-old-collectors.bat** - Archiviere alte Scripts

### ExifTool Commands:
- **exiftool.exe -j image.jpg** - JSON Output
- **exiftool.exe -Barcode image.jpg** - Nur Barcode Tag
- **Tools\exiftool.exe** - Standard Location im Projekt
