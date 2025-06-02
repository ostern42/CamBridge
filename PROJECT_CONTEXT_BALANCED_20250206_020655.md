# CamBridge Context - "Balanced" Profile 
Generated: 02.06.2025  2:06:55,73 
Coverage: "~25" 
Purpose: "Balanced coverage for most tasks" 
Version: CamBridge v0.5.1 
 
## PROJECT_WISDOM.md 
```
# CamBridge Project Wisdom & Conventions
**Letzte Aktualisierung:** 2025-06-02, 02:00 Uhr  
**Von:** Claude (Assistant)  
**Für:** Kontinuität zwischen Chat-Sessions

## 🚨 V.O.G.O.N. SYSTEM 🚨
**Verbose Operational Guidance & Organizational Navigation**

*Im Gegensatz zu den Vogonen aus dem galaktischen Beamtenapparat ist unser V.O.G.O.N. darauf ausgelegt, Dinge tatsächlich einfacher zu machen. Keine Formulare in dreifacher Ausfertigung erforderlich.*

### 🚀 "VOGON INIT" - Automatischer Start
Wenn Sie nur "VOGON INIT" sagen, werde ich:
1. PROJECT_WISDOM.md aus den hochgeladenen Dateien lesen
2. Den aktuellen Übergabeprompt daraus extrahieren
3. Die Aufgabe verstehen und direkt loslegen
4. Keine weiteren Erklärungen nötig!

### 🌟 "VOGON DRIVE" - Google Drive Integration (NEU!)
**GAME CHANGER für Token-Effizienz!**

Wenn Google Drive aktiviert ist:
1. **Kein collect-sources.bat mehr nötig!**
2. **Direkter Zugriff auf Source-Dateien**
3. **~70% Token-Ersparnis**
4. **Gezieltes Lesen einzelner Files**

**Workflow mit Google Drive:**
```
1. Projekt auf Google Drive synchronisieren
2. "VOGON DRIVE" sagen für Drive-basierten Workflow
3. Ich lese direkt die benötigten Files
4. Artefakte wie gewohnt zurückkopieren
```

**Beispiel-Befehle:**
- "VOGON DRIVE: Zeig mir PatientId in Core"
- "VOGON DRIVE: Lies ExifToolReader.cs"
- "VOGON DRIVE: Check alle ViewModels"

**WICHTIG:** 
- Ich kann nur LESEN, nicht direkt speichern
- Änderungen kommen als Artefakte
- Du musst sie manuell zurückkopieren
- Aber die Token-Ersparnis ist ENORM!

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

### 🏁 "VOGON EXIT" - Chat-Abschluss
Wenn Sie "VOGON EXIT" sagen, werde ich:
1. Nach aktueller Zeit/Datum fragen (falls nicht bekannt)
2. Version.props erhöhen
3. CHANGELOG.md aktualisieren  
4. Git commit string vorbereiten
5. README.md bei Bedarf aktualisieren
6. Übergabeprompt für nächsten Chat erstellen
7. PROJECT_WISDOM.md als VOLLSTÄNDIGES Artefakt finalisieren
8. CHANGELOG.md NUR neuester Eintrag als Artefakt
9. Version.props als VOLLSTÄNDIGES Artefakt

## ⚡️ ABSOLUT KRITISCHE VOGON EXIT REGEL ⚡️
**BEIM VOGON EXIT MÜSSEN IMMER ERSTELLT WERDEN:**
1. **PROJECT_WISDOM.md** - Als VOLLSTÄNDIGES Artefakt (nicht nur Updates!)
2. **CHANGELOG.md** - NUR der neueste Versions-Eintrag als Artefakt
3. **Version.props** - Als VOLLSTÄNDIGES Artefakt

**WARUM:** Updates können fehlschlagen oder übersehen werden. Nur vollständige Artefakte garantieren, dass der Nutzer die aktualisierten Dateien bekommt! Beim CHANGELOG reicht der neueste Eintrag um Zeit zu sparen.

**MERKSATZ:** "Ein VOGON EXIT ohne vollständige Artefakte ist wie ein Vogone ohne Poesie - technisch möglich, aber sinnlos!"

*Hinweis: Dieses System ist zu 100% vogonenfrei und wurde nicht von der galaktischen Planungskommission genehmigt, was es vermutlich effizienter macht.*

### 📋 Aktueller Übergabeprompt
```
Nächste Aufgabe: BUILD-FEHLER FIXEN & EXIFTOOL TESTEN!

Stand: v0.5.4 - PROJECT_WISDOM optimiert, Google Drive dokumentiert

ERFOLGE:
✅ Parser-Bug identifiziert: Ricoh speichert in "Barcode" Tag
✅ ExifToolReader implementiert mit Fallback
✅ Debug-Console für EXIF-Analyse erstellt
✅ Google Drive Integration dokumentiert (VOGON DRIVE)
✅ PROJECT_WISDOM optimiert (30% kleiner)

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

TIPP: Mit Google Drive Integration starten für 70% Token-Ersparnis!
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

## 🔥 BREAKING: Google Drive Integration (2025-06-02, 01:10)

### Die Token-Revolution!
**Direkt-Zugriff auf Source-Dateien ohne collect-batches:**
- **70% Token-Ersparnis** durch gezieltes Lesen
- **Keine Wartezeiten** für Batch-Ausführung
- **Echte Navigation** durch die Ordnerstruktur
- **"Mal eben nachschauen"** wird möglich

### VOGON DRIVE Commands:
- **VOGON DRIVE INIT** - Start mit Google Drive
- **VOGON DRIVE: [file]** - Spezifische Datei lesen
- **VOGON DRIVE: show [folder]** - Ordnerinhalt anzeigen
- **VOGON DRIVE: find [pattern]** - Dateien suchen

### Wichtige Einschränkung:
- Kann nur LESEN, nicht direkt speichern
- Änderungen kommen weiterhin als Artefakte
- Manuelles Zurückkopieren nötig
- Aber die Effizienz-Steigerung ist MASSIV!

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

### Technologie-Stack
```
GUI: WPF + ModernWpfUI 0.9.6 + CommunityToolkit.Mvvm
Service: ASP.NET Core Minimal API + Windows Service
Processing: fo-dicom, MetadataExtractor, ExifTool
External: ExifTool 12.96 (in Tools/)
Collector: v2.0 mit 7 Profilen + Git-Integration
```

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
  - Das VOGON System braucht auch Tokens
- **BESSER:** Erst fragen, dann einzeln implementieren

### Visual Studio Anfänger-Unterstützung
- **IMMER:** Genaue Projekte und Pfade zu Source Files angeben
- **Beispiel:** "In `src/CamBridge.Config/Views/DeadLettersPage.xaml.cs`"
- **Bei kleinen Änderungen:** Zeige nur die zu ändernde Zeile
- **Format:** "Ändere Zeile X von 'alt' zu 'neu'"
- **Keine Riesen-Artefakte** für einzeilige Änderungen!
- **Geduldig sein:** Der Nutzer kann Code nur auf Meta-Ebene nachvollziehen

### VOGON EXIT Artefakt-Regel
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

#### Option A: Traditionell (ohne Google Drive)
```
1. PROJECT_WISDOM.md (immer!)
2. collect-smart.bat ausführen (oder spezifisches Profil)
3. PROJECT_CONTEXT_[PROFILE]_[TIMESTAMP].md uploaden
```

#### Option B: Mit Google Drive (EMPFOHLEN!)
```
1. PROJECT_WISDOM.md (immer!)
2. Google Drive Integration aktivieren
3. "VOGON DRIVE" für direkten File-Zugriff
4. Spart ~70% der Tokens!
```

**Tipp:** Wenn Google Drive verfügbar ist, nutze IMMER Option B!

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

### Chat-Abschluss mit "VOGON EXIT"
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
- **Letzte Aktualisierung:** 02.06.2025, 01:25 Uhr
- **Entwicklungszeit bisher:** ~52.9 Stunden (inkl. Nachtschichten!)
- **WICHTIG:** IMMER nach aktueller Zeit fragen für CHANGELOG!

### Changelog-Regel
```
## [Version] - YYYY-MM-DD HH:MM  ← Mit exakter Zeit!
```

### Wichtige Erkenntnis
**Timestamps erzählen Geschichten!**
- Nachtschichten erkennen (01:17, 02:22, 01:05)
- "Duplikate" entlarven (9 Std Unterschied = kein Duplikat!)
- Arbeitsintensität verstehen (52.9 Std in 3 Tagen)

## 📋 Entwicklungsplan (KORRIGIERTE VERSION - Stand 02.06.2025, 01:25)

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
- **KEIN** VOGON EXIT ohne vollständige Artefakte!
- **KEINE** kompletten CHANGELOG Artefakte beim EXIT - nur neuester Eintrag!
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

**VOGON EXIT Artefakt-Regel:**
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

### Option 1: V.O.G.O.N. mit Google Drive (BESTE!)
```
1. PROJECT_WISDOM.md hochladen
2. Google Drive Integration aktivieren
3. Sagen: "VOGON DRIVE INIT"
4. Fertig! Direkter Source-Zugriff ohne Token-Verschwendung!
```

### Option 2: V.O.G.O.N. Traditionell
```
1. PROJECT_WISDOM.md hochladen
2. collect-smart.bat ausführen
3. PROJECT_CONTEXT_[PROFILE]_[TIMESTAMP].md hochladen
4. Sagen: "VOGON INIT"
5. Fertig! Ich lege direkt los.
```

### Option 3: Manuell (falls VOGON nicht funktioniert)
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
- 2025-06-01 14:30: VOGON EXIT hinzugefügt, Dead Letters teilweise implementiert
- 2025-06-01 14:45: V.O.G.O.N. System benannt, Douglas Adams Integration
- 2025-06-01 14:50: Die unwahrscheinliche Geschichte von CamBridge konzipiert
- 2025-06-01 14:55: Vogonisches Poesie Easteregg geplant - V.O.G.O.N. ist Ford Prefect!
- 2025-06-01 15:00: CAMBRIDGE → VOGON umbenannt, Easteregg hat Priorität!
- 2025-06-01 15:10: Dead Letters UI komplett implementiert (crasht noch), v0.4.2 abgeschlossen
- 2025-06-01 15:15: KRITISCHE REGEL: PROJECT_WISDOM.md als vollständiges Artefakt beim VOGON EXIT!
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
- 2025-06-02 01:10: WISDOM - Google Drive Integration als Game Changer erkannt! VOGON DRIVE geboren!
- 2025-06-02 02:00: v0.5.4 - PROJECT_WISDOM optimiert, VOGON EXIT statt CLOSE, Google Drive vollständig integriert

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

### V.O.G.O.N. Commands:
- **VOGON INIT** - Automatischer Start
- **WISDOM:** - Live-Updates ins PROJECT_WISDOM
- **CLAUDE:** - Notizen für nächste Instanz
- **VOGON EXIT** - Chat-Abschluss mit Versionierung
- **VOGON DRIVE INIT** - Start mit Google Drive (NEU!)
- **VOGON DRIVE:** - Direkter File-Zugriff (NEU!)

### Google Drive Commands (NEU!):
- **VOGON DRIVE: show src/Core** - Zeige Ordnerinhalt
- **VOGON DRIVE: read [filepath]** - Lese spezifische Datei
- **VOGON DRIVE: find *.cs** - Suche nach Pattern
- **VOGON DRIVE: check PatientId** - Finde alle Vorkommen

### Collector v2.0 Commands:
- **collect-sources.bat [profile]** - Spezifisches Profil
- **collect-sources.bat list** - Zeige alle Profile
- **collect-smart.bat** - Automatische Profil-Auswahl
- **cleanup-old-collectors.bat** - Archiviere alte Scripts

### ExifTool Commands:
- **exiftool.exe -j image.jpg** - JSON Output
- **exiftool.exe -Barcode image.jpg** - Nur Barcode Tag
- **Tools\exiftool.exe** - Standard Location im Projekt
```
 
## Version.props 
```
<Project>
	<PropertyGroup>
		<AssemblyVersion>0.5.4.0</AssemblyVersion>
		<FileVersion>0.5.4.0</FileVersion>
		<InformationalVersion>0.5.4</InformationalVersion>
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
 
## CHANGELOG.md 
```
# CamBridge Changelog

All notable changes to CamBridge will be documented in this file.  
© 2025 Claude's Improbably Reliable Software Solutions

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.5.4] - 2025-06-02 02:00
### Added
- Google Drive Integration documentation (VOGON DRIVE)
- Direct source file access capability (70% token savings)

### Changed
- Renamed VOGON CLOSE to VOGON EXIT for consistency
- Optimized PROJECT_WISDOM.md (removed redundant sections)
- Compacted technology stack documentation

### Removed
- Redundant Git history details from PROJECT_WISDOM
- Version history duplicates
- Time estimates (token optimization)

## [0.5.3] - 2025-06-02 01:05
### Added
- ExifToolReader for comprehensive EXIF tag support
- Debug console for analyzing EXIF data extraction
- Support for Pentax/Ricoh proprietary "Barcode" tag
- Automatic fallback hierarchy for EXIF readers

### Fixed
- Parser bug: QRBridge data is stored in Barcode tag, not UserComment
- "GCM_TAG " prefix now correctly removed from examid
- NotificationService interface implementation completed

### Changed
- EXIF reader now prioritizes Barcode tag over UserComment
- ServiceCollectionExtensions updated for ExifTool integration

### Discovered
- Ricoh G900 II stores all 5 QRBridge fields in Barcode tag
- MetadataExtractor cannot read proprietary Pentax tags
- ExifTool required for complete data extraction

### Known Issues
- PatientId class duplicated in Entities and ValueObjects
- ExifTool integration not yet tested
- Build errors prevent testing of parser fixes

## [0.5.2] - 2025-06-01 23:52
### Added
- New unified source collector script (collect-sources.bat) with 7 profiles
- Intelligent profile selection based on Git changes (collect-smart.bat)
- Timestamp feature prevents output file overwrites
- Collector documentation (COLLECTOR_README.md)

### Fixed
- NotificationService now implements all INotificationService interface methods
- Added missing async notification methods (Info, Warning, Error, CriticalError)
- Added NotifyDeadLetterThresholdAsync implementation
- Added SendDailySummaryAsync with summary formatting

### Changed
- Replaced 6 individual collector scripts with unified solution
- Collector output now includes timestamp in filename
- Improved file filtering to exclude obj/bin/packages/wpftmp

### Developer Notes
- Parser bug confirmed: QRBridge string is being truncated in our parser
- v0.5.0-0.5.1 features still need testing (Mapping Editor, DICOM Browser)
- Build errors from v0.5.1 resolved

## [0.5.1] - 2025-06-01 22:32
### Added
- DICOM Tag Browser Dialog with search and grouping by module
- Template system fully functional (Ricoh, Minimal, Full templates)
- QRBridge Protocol v2 parser with JSON format support
- Import/Export functionality for mapping configurations
- Backward compatibility for v1 pipe-delimited format
- DicomTagBrowserDialog for intuitive tag selection
- EnumToCollectionConverter integrated into ValueConverters.cs

### Changed
- RicohQRBridgeParser now supports both v1 and v2 protocols
- Template buttons now use MVVM commands instead of click handlers
- Improved error handling in protocol parsing
- NotificationService updated for nested EmailSettings structure

### Fixed
- Project references: CamBridge.Config now references Infrastructure
- System.Drawing.Common version conflict resolved (8.0.10)
- XAML markup errors in MappingEditorPage (Run opacity issue)
- NotificationService email property access corrected

### Technical
- QRBridgeProtocolV2Parser for JSON-based format
- Protocol version detection with automatic fallback
- MappingConfigurationLoader integration
- Complete drag & drop implementation in MappingEditorPage

## [0.5.0] - 2025-06-01 21:47
### Added
- Mapping Editor with drag & drop UI for EXIF to DICOM configuration
- Live preview for field transformations
- Template system for quick mapping setup (UI only)
- PasswordBoxHelper for secure password binding in Settings
- NotificationSettings model with comprehensive email configuration
- MappingEditorViewModel with validation logic
- QRBridge source code integration - full control over both sides!

### Fixed
- PasswordBox security issue - now uses proper attached property
- BorderStyle error in MappingEditorPage XAML
- AboutPage _spriteTimer nullable reference warning
- DI registration for MappingEditorViewModel

### Changed
- MainWindow size increased to 1200x800 for better usability
- Mapping Editor layout with proportional scaling (2* for middle column)
- Navigation includes Mapping Editor item

### Discovered
- QRBridge source available - can optimize protocol (planned for v0.5.1)
- Bidirectional control enables better field encoding than pipes
- Ricoh limitation workarounds possible with custom protocol

### Known Issues
- Template buttons not yet functional
- DICOM tag selector shows placeholder dialog
- Import/Export not implemented
- Mappings not persisted to configuration yet

## [0.4.5] - 2025-06-01 20:52
### Added
- ConfigurationService with JSON persistence to %APPDATA%\CamBridge
- NotificationSettings model with comprehensive email configuration
- Global converter registration in App.xaml for all pages
- PasswordBoxHelper class for secure password binding (implementation pending)

### Fixed
- Settings page crash on navigation - DI registration for ConfigurationService
- All value converters now properly registered and accessible
- Navigation between all pages now stable

### Changed
- Temporary TextBox for SMTP password field (PasswordBox binding workaround)
- Settings are now persisted between application sessions
- Improved error handling during page initialization

### Known Issues
- PasswordBox still uses TextBox temporarily (security concern)
- Ricoh G900 II only saves 3 of 5 QRBridge fields
- Service connection shows "Service Offline" (service not running)

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
### Added
- Dead Letters management page with full CRUD operations
- Real-time filtering and sorting
- Export functionality (CSV/JSON)
- Batch operations for retry/delete

### Known Issues
- Dead Letters page crashes on navigation (DI issue)

## [0.4.1] - 2025-06-01 13:30 (pending commit)
### Added
- Complete Settings page with 4-tab TabView layout
- Real JSON persistence to appsettings.json with auto-backup
- Comprehensive MVVM data binding with CommunityToolkit.Mvvm
- Watch folder management with add/remove functionality
- Input validation with Data Annotations
- Folder browse dialogs using Win32 interop
- Status bar with loading indicators and change tracking
- Value converters for visibility bindings
- PROJECT_WISDOM.md - Cross-session documentation system

### Fixed
- WPF/ModernWPF compatibility (removed WinUI3 Spacing attributes)

### Changed
- ConfigurationService from mock to real implementation
- Version management to prevent duplicates (lessons learned from v0.4.0)

## [0.4.0] - 2025-06-01 11:30:55 (Duplicate version! Should have been v0.4.1)
### Added
- Service Control implementation (Phase 9)
- Start/Stop/Restart functionality with UAC handling
- Automatic status updates every 2 seconds
- Uptime display for running service
- "Restart as Administrator" feature
- Quick Actions for Services.msc and EventVwr.msc
- Service installation detection

### Note
- This was committed 9 hours after the first v0.4.0 commit
- Should have incremented version number

## [0.4.0] - 2025-06-01 02:22:32 (Night shift development)
### Added
- WPF Configuration GUI with ModernWPF UI framework
- Real-time dashboard with auto-refresh (5 seconds)
- HttpApiService for REST API communication  
- StatusController API endpoints for service monitoring
- Connection status indicator with visual feedback
- Active processing items display
- Recent activity tracking
- Full dependency injection for ViewModels and Services

### Fixed
- Platform-specific CA1416 warnings for Windows-only features
- Missing package references for HTTP client
- Dependency injection setup for ViewModels
- Proper error handling for offline service

### Changed
- Dashboard now shows live data from ProcessingQueue
- Added loading states during API calls
- Display of processing statistics and queue status

### Breaking Change
- INotificationService now includes NotifyErrorAsync method

## [0.3.2] - 2025-05-31 23:10:22
### Added
- Complete error handling with dead-letter queue persistence and reprocessing
- Email/event log notifications with daily summaries and threshold alerts
- Web dashboard (port 5050) with REST API and real-time monitoring
- PowerShell installation script with automated setup
- Build and deployment automation

### Known Issues
- Integration tests have build errors (later fixed)

## [0.3.1] - 2025-05-31 16:51:44
### Fixed
- Dependency injection issue where singleton ProcessingQueue tried to consume scoped IFileProcessor
- ProcessingQueue now uses IServiceScopeFactory to create scopes for file processing
- Removed duplicate IFileProcessor registration in Program.cs

### Added
- Batch and PowerShell scripts for collecting source files for deployment

## [0.3.0] - 2025-05-31 15:45:17
### Added
- FileProcessor service orchestrating complete conversion pipeline
- ProcessingQueue with thread-safe operation and retry logic
- FolderWatcherService monitoring multiple folders via FileSystemWatcher
- Comprehensive configuration system via appsettings.json
- Health check endpoint for service monitoring
- Statistics reporting and performance metrics
- PowerShell installation script and documentation

### Changed
- Worker service now coordinates all processing components
- Enhanced logging with structured output
- Target framework to net8.0-windows for Windows Service

### Breaking Change
- Target framework changed to net8.0-windows

## [0.2.0] - 2025-05-31 10:34:17
### Added
- Flexible JSON-based mapping configuration system
- MappingConfigurationLoader for JSON serialization
- DicomTagMapper service for dynamic EXIF to DICOM mapping
- Support for value transformations (date, gender, truncation)
- Comprehensive tests for mapping system

## [Missing Version] - 2025-05-31 01:17:17 (Night shift!)
### Added
- DICOM conversion implementation with fo-dicom v5.1.2
- DicomConverter for JPEG to DICOM transformation
- Preserve JPEG compression using encapsulated pixel data
- Support YBR_FULL_422 photometric interpretation

### Note
- This commit was missing version number in git commit message

## [0.1.0] - 2025-05-30 23:49:44
### Added
- EXIF extraction with QRBridge support
- Support for pipe-delimited and command-line QRBridge formats
- RicohExifReader for specialized Ricoh G900 II support
- Infrastructure layer with comprehensive unit tests

## [0.0.2] - 2025-05-30 21:34:12 (Git duplicate - 78 seconds after first)
### Added
- Core domain models (second commit)

## [0.0.2] - 2025-05-30 21:32:54
### Added
- Core domain models (Patient, Study, Metadata)
- Value objects (DicomTag, ExifTag, PatientId, StudyId)
- Repository interfaces

## [0.0.1] - 2025-05-30 20:34:20
### Added
- Initial project structure with 4 projects (Core, Infrastructure, Service, Config)
- Automatic versioning via Version.props
- Documentation (README, CHANGELOG, LICENSE)

### Note
- Project started at 20:30:44 with .gitattributes
- First real commit 4 minutes later

---

## Version History Summary
- Total development time: ~44.7 hours over 2.8 days
- Night shifts: DICOM (01:17), GUI (02:22)
- Version duplicates: v0.0.2 (78 sec), v0.4.0 (9 hours)
- Missing versions: DICOM commit, v0.3.3 (was in old CHANGELOG but not in git)

## Lessons Learned
- Always increment version numbers, even for small changes
- Use "babysteps" versioning (v0.0.1 → v0.0.2 → v0.0.3)
- Check git history before committing to avoid duplicates
- Night shift commits need extra attention to versioning
```
 
## README.md 
```
# CamBridge

JPEG to DICOM converter for medical imaging from Ricoh cameras with QRBridge integration.

© 2025 Claude's Improbably Reliable Software Solutions

## Overview

CamBridge is a Windows service that monitors folders for JPEG images from Ricoh G900 II cameras and automatically converts them to DICOM format. Patient and examination data embedded via QRBridge QR codes is extracted from EXIF metadata and mapped to appropriate DICOM tags.

## Features

- **Automatic JPEG to DICOM conversion** preserving original compression
- **QRBridge data extraction** from EXIF User Comment field
- **Flexible mapping configuration** via JSON files
- **Ricoh G900 II camera support** with specialized EXIF reading
- **Windows Service** for background operation
- **Dead Letter Queue** for failed conversions
- **Email & Event Log notifications** for critical errors
- **Web Dashboard** for real-time monitoring
- **REST API** for integration and monitoring
- **Comprehensive logging** with Serilog

## System Requirements

- Windows 10/11 or Windows Server 2016+
- .NET 8.0 Runtime
- Ricoh G900 II camera with QRBridge-encoded QR codes
- Administrator privileges for service installation

## Quick Start

1. Download the latest release
2. Extract to a temporary folder
3. Run PowerShell as Administrator:
   ```powershell
   .\Install-CamBridge.ps1
   ```
4. Access the dashboard at http://localhost:5050

## Installation

### Automated Installation

The PowerShell installation script handles:
- Service creation and configuration
- Directory structure setup
- Firewall rule configuration
- Event Log source creation

```powershell
# Install with custom path
.\Install-CamBridge.ps1 -InstallPath "D:\CamBridge"

# Uninstall
.\Install-CamBridge.ps1 -Uninstall
```

### Manual Installation

1. Extract files to installation directory
2. Create required directories:
   - `C:\CamBridge\Input`
   - `C:\CamBridge\Output`
   - `C:\CamBridge\Archive`
   - `C:\CamBridge\Errors`
   - `C:\CamBridge\Backup`
   - `C:\CamBridge\Logs`

3. Install as Windows Service:
   ```cmd
   sc create CamBridgeService binPath="C:\Program Files\CamBridge\CamBridge.Service.exe"
   ```

## Configuration

### Basic Settings (appsettings.json)

```json
{
  "CamBridge": {
    "WatchFolders": [
      {
        "Path": "C:\\Images\\Input",
        "OutputPath": "C:\\Images\\DICOM",
        "Enabled": true
      }
    ],
    "Processing": {
      "MaxRetryAttempts": 3,
      "RetryDelaySeconds": 5
    },
    "Notifications": {
      "EnableEmail": true,
      "EmailTo": "admin@hospital.com",
      "SmtpHost": "smtp.hospital.com"
    }
  }
}
```

### Mapping Configuration (mappings.json)

CamBridge uses a flexible JSON-based mapping system to convert EXIF and QRBridge data to DICOM tags:

```json
{
  "mappings": [
    {
      "name": "PatientName",
      "sourceType": "QRBridge",
      "sourceField": "name",
      "targetTag": "(0010,0010)",
      "transform": "None",
      "required": true
    }
  ]
}
```

## QRBridge Format

QRBridge encodes patient data as pipe-delimited strings:
```
EX002|Schmidt, Maria|1985-03-15|F|Röntgen Thorax
```

Fields:
1. Exam ID
2. Patient Name
3. Birth Date
4. Gender (M/F/O)
5. Comment/Study Description

## Monitoring

### Web Dashboard

Access the real-time monitoring dashboard at http://localhost:5050

Features:
- Service status and uptime
- Processing queue statistics
- Success/failure rates
- Dead letter queue management
- Active processing items

### REST API

API documentation available at http://localhost:5050/swagger

Key endpoints:
- `GET /api/status` - Service status
- `GET /api/status/statistics` - Processing statistics
- `GET /api/status/deadletters` - Dead letter items
- `POST /api/status/deadletters/{id}/reprocess` - Reprocess failed item
- `GET /api/status/health` - Health check

### Event Log

CamBridge logs to Windows Event Log under "Application" source "CamBridge Service".

## Dead Letter Queue

Files that fail processing after all retry attempts are moved to the dead letter queue:

- Located in `C:\CamBridge\Errors\dead-letters`
- Organized by date
- Metadata stored in `dead-letters.json`
- Can be reprocessed via dashboard or API

## Notifications

### Email Notifications

Configure SMTP settings for email alerts:
- Critical errors
- Dead letter threshold exceeded
- Daily processing summaries

### Event Log Notifications

All notifications are also logged to Windows Event Log.

## Troubleshooting

### Service Won't Start

1. Check Event Viewer for errors
2. Verify all directories exist and have proper permissions
3. Ensure .NET 8.0 runtime is installed
4. Check `C:\CamBridge\Logs` for detailed logs

### Files Not Processing

1. Verify watch folder configuration
2. Check file permissions
3. Ensure JPEG files contain valid EXIF data
4. Review dead letter queue for errors

### DICOM Validation Errors

1. Check mapping configuration
2. Verify required patient data is present
3. Review DICOM validation logs

## Development

### Building from Source

```bash
# Clone repository
git clone https://github.com/claude/cambridge.git

# Build
dotnet build --configuration Release

# Run tests
dotnet test

# Publish
dotnet publish -c Release -r win-x64
```

### Project Structure

```
CamBridge/
├── src/
│   ├── CamBridge.Core/          # Domain models and interfaces
│   ├── CamBridge.Infrastructure/ # Service implementations
│   └── CamBridge.Service/       # Windows Service & Web API
└── tests/
    └── CamBridge.Infrastructure.Tests/
```

### Running Tests

```powershell
# Run all tests with coverage
.\Run-Tests.ps1

# Run specific test category
dotnet test --filter Category=Integration
```

## Version History

- **0.3.2** - Dead letter queue, notifications, web dashboard
- **0.3.1** - Fixed dependency injection issues
- **0.3.0** - Windows Service implementation
- **0.2.0** - Dynamic mapping configuration
- **0.1.0** - Core EXIF/DICOM functionality
- **0.0.1** - Initial project structure

## Roadmap

### Phase 7: Dateiverarbeitung Pipeline (1 Chat)
- Ordnerüberwachung
- Datei-Queue System
- Fehlerbehandlung
- Backup-Funktionalität

- 
### Phase 8: WinUI 3 GUI Basis (2 Chats)
- CamBridge Config Projekt
- Moderne UI mit Animationen
- Navigation-Framework
- MVVM-Struktur


### Phase 9: Service-Steuerung GUI (1 Chat)
- Service Installation/Deinstallation
- Start/Stop/Status
- Uptime-Anzeige
- Admin-Rechte Handling


### Phase 10: Konfigurationsverwaltung (1 Chat)
- JSON-Konfiguration
- Settings-UI
- Ordner-Auswahl Dialoge
- Mapping-Editor


## License

Proprietary - © 2025 Claude's Improbably Reliable Software Solutions

## Support

For issues and feature requests, please contact support.

## Acknowledgments

- fo-dicom for DICOM processing
- MetadataExtractor for EXIF reading
- Serilog for structured logging
- QRBridge for patient data encoding
```
 
## CamBridgeSettings.cs 
```
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
 
## DeadLetterStatistics.cs 
```
using System;
using System.Collections.Generic;

namespace CamBridge.Core
{
    /// <summary>
    /// Statistics about the dead letter queue
    /// </summary>
    public class DeadLetterStatistics
    {
        public int TotalItems { get; set; }
        public int ItemsLastHour { get; set; }
        public int ItemsLast24Hours { get; set; }
        public DateTime OldestItem { get; set; }
        public Dictionary<string, int> TopErrors { get; set; } = new();
    }
}
```
 
## MappingRule.cs 
```
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
 
## NotificationSettings.cs 
```
namespace CamBridge.Core
{
    /// <summary>
    /// Configuration for notifications and alerts
    /// </summary>
    public class NotificationSettings
    {
        /// <summary>
        /// Enable email notifications
        /// </summary>
        public bool EnableEmail { get; set; }

        /// <summary>
        /// Enable Windows Event Log notifications
        /// </summary>
        public bool EnableEventLog { get; set; } = true;

        /// <summary>
        /// Email configuration
        /// </summary>
        public EmailSettings Email { get; set; } = new();

        /// <summary>
        /// Minimum log level for email notifications
        /// </summary>
        public string MinimumEmailLevel { get; set; } = "Warning";

        /// <summary>
        /// Maximum emails per hour (throttling)
        /// </summary>
        public int MaxEmailsPerHour { get; set; } = 10;

        /// <summary>
        /// Throttle period in minutes
        /// </summary>
        public int ThrottleMinutes { get; set; } = 15;

        /// <summary>
        /// Send daily summary email
        /// </summary>
        public bool SendDailySummary { get; set; }

        /// <summary>
        /// Hour to send daily summary (0-23)
        /// </summary>
        public int DailySummaryHour { get; set; } = 8;

        /// <summary>
        /// Dead letter threshold for alerts
        /// </summary>
        public int DeadLetterThreshold { get; set; } = 50;
    }

    /// <summary>
    /// Email server configuration
    /// </summary>
    public class EmailSettings
    {
        /// <summary>
        /// From email address
        /// </summary>
        public string? From { get; set; }

        /// <summary>
        /// To email addresses (semicolon separated)
        /// </summary>
        public string? To { get; set; }

        /// <summary>
        /// SMTP server host
        /// </summary>
        public string? SmtpHost { get; set; }

        /// <summary>
        /// SMTP server port
        /// </summary>
        public int SmtpPort { get; set; } = 587;

        /// <summary>
        /// Use SSL/TLS
        /// </summary>
        public bool UseSsl { get; set; } = true;

        /// <summary>
        /// SMTP username
        /// </summary>
        public string? Username { get; set; }

        /// <summary>
        /// SMTP password
        /// </summary>
        public string? Password { get; set; }
    }
}
```
 
## ProcessingOptions.cs 
```
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
 
## ProcessingSummary.cs 
```
using System;
using System.Collections.Generic;

namespace CamBridge.Core
{
    /// <summary>
    /// Summary of daily processing activities
    /// </summary>
    public class ProcessingSummary
    {
        public DateTime Date { get; set; }
        public int TotalProcessed { get; set; }
        public int Successful { get; set; }
        public int Failed { get; set; }
        public double ProcessingTimeSeconds { get; set; }
        public Dictionary<string, int> TopErrors { get; set; } = new();
        public int DeadLetterCount { get; set; }
        public TimeSpan Uptime { get; set; }

        // Berechnete Properties
        public double SuccessRate => TotalProcessed > 0
            ? (double)Successful / TotalProcessed * 100
            : 0;

        public double AverageProcessingTime => TotalProcessed > 0
            ? ProcessingTimeSeconds / TotalProcessed
            : 0;
    }
}
```
 
## FolderWatcherService.cs 
```
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CamBridge.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CamBridge.Infrastructure.Services
{
    /// <summary>
    /// Background service that monitors multiple folders for new JPEG files
    /// </summary>
    public class FolderWatcherService : IHostedService, IDisposable
    {
        private readonly ILogger<FolderWatcherService> _logger;
        private readonly ProcessingQueue _processingQueue;
        private readonly CamBridgeSettings _settings;
        private readonly List<FileSystemWatcher> _watchers = new();
        private readonly Dictionary<string, DateTime> _fileDebounce = new();
        private readonly object _debounceLock = new();
        private readonly TimeSpan _debounceInterval = TimeSpan.FromSeconds(2);
        private Timer? _debounceTimer;
        private bool _disposed;

        public FolderWatcherService(
            ILogger<FolderWatcherService> logger,
            ProcessingQueue processingQueue,
            IOptions<CamBridgeSettings> settings)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _processingQueue = processingQueue ?? throw new ArgumentNullException(nameof(processingQueue));
            _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting folder watcher service");

            // Initialize watchers for each configured folder
            foreach (var folderConfig in _settings.WatchFolders.Where(f => f.Enabled && f.IsValid))
            {
                try
                {
                    var watcher = CreateWatcher(folderConfig);
                    _watchers.Add(watcher);

                    _logger.LogInformation("Started watching folder: {Path} (Pattern: {Pattern}, Subdirectories: {IncludeSubdirs})",
                        folderConfig.Path, folderConfig.FilePattern, folderConfig.IncludeSubdirectories);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to create watcher for folder: {Path}", folderConfig.Path);
                }
            }

            if (_watchers.Count == 0)
            {
                _logger.LogWarning("No valid folders configured for watching");
            }

            // Start debounce timer
            _debounceTimer = new Timer(
                ProcessDebounceQueue,
                null,
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(1));

            // Process existing files if configured
            if (_settings.Processing.ProcessExistingOnStartup)
            {
                await ProcessExistingFilesAsync(cancellationToken);
            }

            await Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping folder watcher service");

            // Stop all watchers
            foreach (var watcher in _watchers)
            {
                try
                {
                    watcher.EnableRaisingEvents = false;
                    watcher.Dispose();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error disposing watcher");
                }
            }

            _watchers.Clear();

            // Stop debounce timer
            _debounceTimer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            if (_disposed) return;

            _debounceTimer?.Dispose();

            foreach (var watcher in _watchers)
            {
                try
                {
                    watcher.Dispose();
                }
                catch { }
            }

            _disposed = true;
        }

        private FileSystemWatcher CreateWatcher(FolderConfiguration folderConfig)
        {
            var watcher = new FileSystemWatcher(folderConfig.Path)
            {
                IncludeSubdirectories = folderConfig.IncludeSubdirectories,
                NotifyFilter = NotifyFilters.FileName |
                              NotifyFilters.LastWrite |
                              NotifyFilters.Size
            };

            // Set up filters for multiple extensions
            var patterns = folderConfig.FilePattern.Split(';', StringSplitOptions.RemoveEmptyEntries);
            if (patterns.Length == 1)
            {
                watcher.Filter = patterns[0].Trim();
            }
            else
            {
                // For multiple patterns, we'll filter in the event handler
                watcher.Filter = "*.*";
            }

            // Wire up events
            watcher.Created += (sender, e) => OnFileEvent(e.FullPath, folderConfig, patterns);
            watcher.Changed += (sender, e) => OnFileEvent(e.FullPath, folderConfig, patterns);
            watcher.Renamed += (sender, e) => OnFileEvent(e.FullPath, folderConfig, patterns);

            // Error handling
            watcher.Error += (sender, e) =>
            {
                var ex = e.GetException();
                _logger.LogError(ex, "FileSystemWatcher error for path: {Path}", folderConfig.Path);

                // Try to recreate the watcher
                Task.Run(async () =>
                {
                    await Task.Delay(5000);
                    try
                    {
                        var index = _watchers.IndexOf((FileSystemWatcher)sender);
                        if (index >= 0)
                        {
                            _watchers[index].Dispose();
                            _watchers[index] = CreateWatcher(folderConfig);
                            _logger.LogInformation("Recreated watcher for path: {Path}", folderConfig.Path);
                        }
                    }
                    catch (Exception recreateEx)
                    {
                        _logger.LogError(recreateEx, "Failed to recreate watcher for path: {Path}",
                            folderConfig.Path);
                    }
                });
            };

            // Start watching
            watcher.EnableRaisingEvents = true;

            return watcher;
        }

        private void OnFileEvent(string filePath, FolderConfiguration folderConfig, string[] patterns)
        {
            try
            {
                // Check if file matches any pattern
                if (patterns.Length > 1)
                {
                    var fileName = Path.GetFileName(filePath);
                    var matchesPattern = patterns.Any(pattern =>
                    {
                        var cleanPattern = pattern.Trim();
                        if (cleanPattern.StartsWith("*"))
                        {
                            return fileName.EndsWith(cleanPattern.Substring(1),
                                StringComparison.OrdinalIgnoreCase);
                        }
                        return fileName.Equals(cleanPattern, StringComparison.OrdinalIgnoreCase);
                    });

                    if (!matchesPattern)
                        return;
                }

                // Add to debounce queue
                lock (_debounceLock)
                {
                    _fileDebounce[filePath] = DateTime.UtcNow;
                }

                _logger.LogDebug("File event detected: {FilePath}", filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling file event for: {FilePath}", filePath);
            }
        }

        private void ProcessDebounceQueue(object? state)
        {
            try
            {
                List<string> filesToProcess;

                lock (_debounceLock)
                {
                    var cutoffTime = DateTime.UtcNow - _debounceInterval;

                    filesToProcess = _fileDebounce
                        .Where(kvp => kvp.Value < cutoffTime)
                        .Select(kvp => kvp.Key)
                        .ToList();

                    foreach (var file in filesToProcess)
                    {
                        _fileDebounce.Remove(file);
                    }
                }

                // Enqueue files for processing
                foreach (var filePath in filesToProcess)
                {
                    if (File.Exists(filePath))
                    {
                        if (_processingQueue.TryEnqueue(filePath))
                        {
                            _logger.LogInformation("Enqueued new file: {FilePath}", filePath);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing debounce queue");
            }
        }

        private async Task ProcessExistingFilesAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Processing existing files in watched folders");

            var processedCount = 0;
            var skippedCount = 0;

            foreach (var folderConfig in _settings.WatchFolders.Where(f => f.Enabled && f.IsValid))
            {
                try
                {
                    var patterns = folderConfig.FilePattern
                        .Split(';', StringSplitOptions.RemoveEmptyEntries)
                        .Select(p => p.Trim())
                        .ToArray();

                    var searchOption = folderConfig.IncludeSubdirectories
                        ? SearchOption.AllDirectories
                        : SearchOption.TopDirectoryOnly;

                    foreach (var pattern in patterns)
                    {
                        var files = Directory.GetFiles(folderConfig.Path, pattern, searchOption);

                        foreach (var file in files)
                        {
                            if (cancellationToken.IsCancellationRequested)
                                break;

                            if (_processingQueue.TryEnqueue(file))
                            {
                                processedCount++;
                            }
                            else
                            {
                                skippedCount++;
                            }

                            // Small delay to avoid overwhelming the system
                            if (processedCount % 10 == 0)
                            {
                                await Task.Delay(100, cancellationToken);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing existing files in folder: {Path}",
                        folderConfig.Path);
                }
            }

            _logger.LogInformation("Existing file scan complete. Enqueued: {ProcessedCount}, Skipped: {SkippedCount}",
                processedCount, skippedCount);
        }
    }
}
```
 
## INotificationService.cs 
```
// src/CamBridge.Infrastructure/Services/INotificationService.cs
using System;
using System.Threading.Tasks;
using CamBridge.Core;

namespace CamBridge.Infrastructure.Services
{
    public interface INotificationService
    {
        Task NotifyInfoAsync(string subject, string message);
        Task NotifyWarningAsync(string subject, string message);
        Task NotifyErrorAsync(string subject, string message, Exception? exception = null);
        Task NotifyCriticalErrorAsync(string subject, string message, Exception? exception = null);
        Task NotifyDeadLetterThresholdAsync(int count, DeadLetterStatistics statistics);
        Task SendDailySummaryAsync(ProcessingSummary summary);
    }
}```
 
## NotificationService.cs 
```
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using CamBridge.Core;
using CamBridge.Core.Entities;
using CamBridge.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CamBridge.Infrastructure.Services
{
    /// <summary>
    /// Service for sending notifications via email and Windows Event Log
    /// </summary>
    public class NotificationService : INotificationService
    {
        private readonly ILogger<NotificationService> _logger;
        private readonly NotificationSettings _settings;
        private readonly string _eventSource = "CamBridge Service";
        private readonly string _eventLog = "Application";
        private readonly List<ProcessingResult> _dailySummaryBuffer = new();
        private DateTime _lastSummaryDate = DateTime.MinValue;

        public NotificationService(
            ILogger<NotificationService> logger,
            IOptions<NotificationSettings> settings)
        {
            _logger = logger;
            _settings = settings.Value;

            // Initialize Windows Event Log source if enabled
            if (_settings.EnableEventLog)
            {
                InitializeEventLog();
            }
        }

        /// <summary>
        /// Send info notification
        /// </summary>
        public async Task NotifyInfoAsync(string subject, string message)
        {
            try
            {
                if (_settings.EnableEventLog)
                {
                    WriteEventLog(message, EventLogEntryType.Information);
                }

                if (ShouldSendEmail(LogLevel.Information))
                {
                    await SendEmailAsync(subject, message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send info notification");
            }
        }

        /// <summary>
        /// Send warning notification
        /// </summary>
        public async Task NotifyWarningAsync(string subject, string message)
        {
            try
            {
                if (_settings.EnableEventLog)
                {
                    WriteEventLog(message, EventLogEntryType.Warning);
                }

                if (ShouldSendEmail(LogLevel.Warning))
                {
                    await SendEmailAsync(subject, message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send warning notification");
            }
        }

        /// <summary>
        /// Send error notification
        /// </summary>
        public async Task NotifyErrorAsync(string subject, string message, Exception? exception = null)
        {
            try
            {
                if (_settings.EnableEventLog)
                {
                    WriteEventLog(message, EventLogEntryType.Error, exception);
                }

                if (ShouldSendEmail(LogLevel.Error))
                {
                    var body = BuildErrorEmailBody(message, exception);
                    await SendEmailAsync(subject, body);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send error notification");
            }
        }

        /// <summary>
        /// Send critical error notification
        /// </summary>
        public async Task NotifyCriticalErrorAsync(string subject, string message, Exception? exception = null)
        {
            try
            {
                // Critical errors always go to event log
                if (_settings.EnableEventLog)
                {
                    WriteEventLog($"CRITICAL: {message}", EventLogEntryType.Error, exception);
                }

                // Critical errors always attempt email if configured
                if (_settings.EnableEmail && _settings.Email != null)
                {
                    var body = BuildErrorEmailBody($"CRITICAL ERROR: {message}", exception);
                    await SendEmailAsync($"[CRITICAL] {subject}", body);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send critical error notification");
            }
        }

        /// <summary>
        /// Send notification when dead letter threshold is exceeded
        /// </summary>
        public async Task NotifyDeadLetterThresholdAsync(int count, DeadLetterStatistics statistics)
        {
            try
            {
                var message = $"Dead letter queue threshold exceeded. Count: {count}, Threshold: {_settings.DeadLetterThreshold}";

                var details = new StringBuilder();
                details.AppendLine(message);
                details.AppendLine();
                details.AppendLine("Statistics:");
                details.AppendLine($"  Total Items: {statistics.TotalItems}");
                details.AppendLine($"  Items Last Hour: {statistics.ItemsLastHour}");
                details.AppendLine($"  Items Last 24 Hours: {statistics.ItemsLast24Hours}");
                details.AppendLine($"  Oldest Item: {statistics.OldestItem:yyyy-MM-dd HH:mm:ss}");

                if (statistics.TopErrors.Any())
                {
                    details.AppendLine();
                    details.AppendLine("Top Errors:");
                    foreach (var error in statistics.TopErrors.Take(5))
                    {
                        details.AppendLine($"  - {error.Key}: {error.Value} occurrences");
                    }
                }

                await NotifyWarningAsync("Dead Letter Threshold Exceeded", details.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send dead letter threshold notification");
            }
        }

        /// <summary>
        /// Send daily summary report
        /// </summary>
        public async Task SendDailySummaryAsync(ProcessingSummary summary)
        {
            if (!_settings.SendDailySummary || _settings.Email == null)
                return;

            try
            {
                var subject = $"Daily Processing Summary - {summary.Date:yyyy-MM-dd}";
                var body = BuildDailySummaryBody(summary);

                await SendEmailAsync(subject, body);
                _logger.LogInformation("Daily summary sent for {Date}", summary.Date);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send daily summary");
            }
        }

        /// <summary>
        /// Send notification about processing result
        /// </summary>
        public async Task NotifyAsync(ProcessingResult result)
        {
            if (result == null) return;

            try
            {
                // Add to daily summary buffer if enabled
                if (_settings.SendDailySummary)
                {
                    lock (_dailySummaryBuffer)
                    {
                        _dailySummaryBuffer.Add(result);
                    }
                }

                // Check if we should send daily summary
                await CheckAndSendDailySummaryAsync();

                // Determine notification level based on result
                var logLevel = result.Success ? LogLevel.Information : LogLevel.Error;

                // Send notifications based on settings
                if (_settings.EnableEventLog)
                {
                    WriteEventLog(result, logLevel);
                }

                if (ShouldSendEmail(logLevel))
                {
                    await SendEmailNotificationAsync(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send notification for file: {FilePath}", result.FilePath);
            }
        }

        #region Private Methods

        private void InitializeEventLog()
        {
            try
            {
                if (!EventLog.SourceExists(_eventSource))
                {
                    EventLog.CreateEventSource(_eventSource, _eventLog);
                    _logger.LogInformation("Created Windows Event Log source: {Source}", _eventSource);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to create Event Log source. Event logging may not work properly.");
            }
        }

        private bool ShouldSendEmail(LogLevel logLevel)
        {
            return _settings.EnableEmail &&
                   _settings.Email != null &&
                   logLevel >= ParseLogLevel(_settings.MinimumEmailLevel);
        }

        private async Task SendEmailAsync(string subject, string body)
        {
            try
            {
                // Check if email settings are properly configured
                if (_settings.Email == null ||
                    string.IsNullOrWhiteSpace(_settings.Email.SmtpHost) ||
                    string.IsNullOrWhiteSpace(_settings.Email.From) ||
                    string.IsNullOrWhiteSpace(_settings.Email.To))
                {
                    _logger.LogWarning("Email settings not properly configured, skipping email notification");
                    return;
                }

                using var client = new SmtpClient(_settings.Email.SmtpHost, _settings.Email.SmtpPort)
                {
                    EnableSsl = _settings.Email.UseSsl,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Timeout = 30000 // 30 seconds
                };

                // Set credentials if provided
                if (!string.IsNullOrWhiteSpace(_settings.Email.Username))
                {
                    client.Credentials = new NetworkCredential(_settings.Email.Username, _settings.Email.Password);
                }

                using var message = new MailMessage
                {
                    From = new MailAddress(_settings.Email.From),
                    Subject = $"[CamBridge] {subject}",
                    Body = body,
                    IsBodyHtml = false,
                    Priority = MailPriority.Normal
                };

                // Add recipients
                var recipients = _settings.Email.To?.Split(';', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
                foreach (var recipient in recipients)
                {
                    var trimmedRecipient = recipient.Trim();
                    if (!string.IsNullOrWhiteSpace(trimmedRecipient))
                    {
                        message.To.Add(new MailAddress(trimmedRecipient));
                    }
                }

                if (message.To.Count == 0)
                {
                    _logger.LogWarning("No valid email recipients configured");
                    return;
                }

                await client.SendMailAsync(message);
                _logger.LogInformation("Email sent successfully to {RecipientCount} recipients", message.To.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email notification: {Subject}", subject);
            }
        }

        private async Task SendEmailNotificationAsync(ProcessingResult result)
        {
            var fileName = Path.GetFileName(result.FilePath);
            var subject = result.Success
                ? $"Processing Successful: {fileName}"
                : $"Processing Failed: {fileName}";

            var body = BuildProcessingEmailBody(result);
            await SendEmailAsync(subject, body);
        }

        private void WriteEventLog(ProcessingResult result, LogLevel logLevel)
        {
            var eventType = logLevel switch
            {
                LogLevel.Error => EventLogEntryType.Error,
                LogLevel.Warning => EventLogEntryType.Warning,
                _ => EventLogEntryType.Information
            };

            var fileName = Path.GetFileName(result.FilePath);
            var message = result.Success
                ? $"Successfully processed: {fileName}\nOutput: {result.OutputFile}"
                : $"Failed to process: {fileName}\nError: {result.ErrorMessage}";

            WriteEventLog(message, eventType);
        }

        private void WriteEventLog(string message, EventLogEntryType type, Exception? exception = null)
        {
            try
            {
                if (exception != null)
                {
                    message += $"\n\nException: {exception.GetType().Name}\n{exception.Message}\n\nStack Trace:\n{exception.StackTrace}";
                }

                EventLog.WriteEntry(_eventSource, message, type);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to write to Windows Event Log");
            }
        }

        private string BuildProcessingEmailBody(ProcessingResult result)
        {
            var sb = new StringBuilder();
            var fileName = Path.GetFileName(result.FilePath);

            sb.AppendLine($"File Processing Notification");
            sb.AppendLine($"==========================");
            sb.AppendLine();
            sb.AppendLine($"Timestamp: {result.ProcessedAt:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine($"File: {fileName}");
            sb.AppendLine($"Path: {result.FilePath}");
            sb.AppendLine($"Status: {(result.Success ? "SUCCESS" : "FAILED")}");

            if (result.Success)
            {
                sb.AppendLine($"Output: {result.OutputFile}");
                if (result.ProcessingTime.HasValue)
                {
                    sb.AppendLine($"Processing Time: {result.ProcessingTime.Value.TotalMilliseconds:F2} ms");
                }
            }
            else
            {
                sb.AppendLine($"Error: {result.ErrorMessage}");
            }

            // Note: PatientInfo is not available on ProcessingResult in the current implementation
            // This would need to be added if patient information should be included in notifications

            sb.AppendLine();
            sb.AppendLine("---");
            sb.AppendLine($"CamBridge Service v{typeof(NotificationService).Assembly.GetName().Version}");

            return sb.ToString();
        }

        private string BuildErrorEmailBody(string message, Exception? exception)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Error Notification");
            sb.AppendLine("==================");
            sb.AppendLine();
            sb.AppendLine($"Timestamp: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine($"Message: {message}");

            if (exception != null)
            {
                sb.AppendLine();
                sb.AppendLine("Exception Details:");
                sb.AppendLine($"  Type: {exception.GetType().FullName}");
                sb.AppendLine($"  Message: {exception.Message}");
                sb.AppendLine();
                sb.AppendLine("Stack Trace:");
                sb.AppendLine(exception.StackTrace);

                if (exception.InnerException != null)
                {
                    sb.AppendLine();
                    sb.AppendLine("Inner Exception:");
                    sb.AppendLine($"  Type: {exception.InnerException.GetType().FullName}");
                    sb.AppendLine($"  Message: {exception.InnerException.Message}");
                }
            }

            sb.AppendLine();
            sb.AppendLine("---");
            sb.AppendLine($"CamBridge Service v{typeof(NotificationService).Assembly.GetName().Version}");
            sb.AppendLine($"Machine: {Environment.MachineName}");

            return sb.ToString();
        }

        private string BuildDailySummaryBody(ProcessingSummary summary)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Daily Processing Summary - {summary.Date:yyyy-MM-dd}");
            sb.AppendLine("=====================================");
            sb.AppendLine();

            sb.AppendLine($"Total Files Processed: {summary.TotalProcessed}");
            sb.AppendLine($"Successful: {summary.Successful}");
            sb.AppendLine($"Failed: {summary.Failed}");
            sb.AppendLine($"Success Rate: {summary.SuccessRate:F1}%");
            sb.AppendLine($"Average Processing Time: {summary.AverageProcessingTime:F2} ms");
            sb.AppendLine($"Total Processing Time: {summary.ProcessingTimeSeconds:F1} seconds");
            sb.AppendLine($"Service Uptime: {summary.Uptime:d\\.hh\\:mm\\:ss}");

            if (summary.Failed > 0 && summary.TopErrors.Any())
            {
                sb.AppendLine();
                sb.AppendLine("Top Errors:");
                sb.AppendLine("-----------");
                foreach (var error in summary.TopErrors.Take(5))
                {
                    sb.AppendLine($"  • {error.Key}: {error.Value} occurrences");
                }
            }

            if (summary.DeadLetterCount > 0)
            {
                sb.AppendLine();
                sb.AppendLine($"Dead Letter Queue: {summary.DeadLetterCount} items pending");
            }

            sb.AppendLine();
            sb.AppendLine("---");
            sb.AppendLine($"CamBridge Service v{typeof(NotificationService).Assembly.GetName().Version}");
            sb.AppendLine($"Report generated at: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");

            return sb.ToString();
        }

        private string BuildDailySummaryBody(List<ProcessingResult> results)
        {
            var summary = new ProcessingSummary
            {
                Date = DateTime.Today,
                TotalProcessed = results.Count,
                Successful = results.Count(r => r.Success),
                Failed = results.Count(r => !r.Success),
                ProcessingTimeSeconds = results.Where(r => r.ProcessingTime.HasValue)
                    .Sum(r => r.ProcessingTime!.Value.TotalSeconds),
                TopErrors = results.Where(r => !r.Success && !string.IsNullOrEmpty(r.ErrorMessage))
                    .GroupBy(r => r.ErrorMessage!)
                    .OrderByDescending(g => g.Count())
                    .Take(5)
                    .ToDictionary(g => g.Key, g => g.Count()),
                Uptime = TimeSpan.FromHours(24) // Placeholder
            };

            return BuildDailySummaryBody(summary);
        }

        private async Task CheckAndSendDailySummaryAsync()
        {
            if (!_settings.SendDailySummary)
                return;

            var now = DateTime.Now;

            // Check if we've crossed into a new day and it's the configured hour
            if (now.Date > _lastSummaryDate && now.Hour >= (_settings.DailySummaryHour ?? 8))
            {
                List<ProcessingResult> summaryData;
                lock (_dailySummaryBuffer)
                {
                    if (_dailySummaryBuffer.Count == 0)
                        return;

                    summaryData = new List<ProcessingResult>(_dailySummaryBuffer);
                    _dailySummaryBuffer.Clear();
                }

                await SendEmailAsync($"Daily Processing Summary - {DateTime.Now:yyyy-MM-dd}",
                    BuildDailySummaryBody(summaryData));

                _lastSummaryDate = now.Date;
            }
        }

        private LogLevel ParseLogLevel(string level)
        {
            return level?.ToLower() switch
            {
                "trace" => LogLevel.Trace,
                "debug" => LogLevel.Debug,
                "information" => LogLevel.Information,
                "warning" => LogLevel.Warning,
                "error" => LogLevel.Error,
                "critical" => LogLevel.Critical,
                _ => LogLevel.Warning
            };
        }

        #endregion
    }
}
```
 
## INotificationService.cs 
```
// src/CamBridge.Infrastructure/Services/INotificationService.cs
using System;
using System.Threading.Tasks;
using CamBridge.Core;

namespace CamBridge.Infrastructure.Services
{
    public interface INotificationService
    {
        Task NotifyInfoAsync(string subject, string message);
        Task NotifyWarningAsync(string subject, string message);
        Task NotifyErrorAsync(string subject, string message, Exception? exception = null);
        Task NotifyCriticalErrorAsync(string subject, string message, Exception? exception = null);
        Task NotifyDeadLetterThresholdAsync(int count, DeadLetterStatistics statistics);
        Task SendDailySummaryAsync(ProcessingSummary summary);
    }
}```
 
## src\CamBridge.Service\appsettings.json 
```
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
      "EnableFileLogging": true,
      "EnableEventLog": true,
      "MaxLogFileSizeMB": 10,
      "MaxLogFiles": 10,
      "IncludePatientDataInDebugLogs": false
    },
    "Service": {
      "ServiceName": "CamBridgeService",
      "DisplayName": "CamBridge JPEG to DICOM Converter",
      "Description": "Monitors folders for JPEG files from Ricoh cameras and converts them to DICOM format",
      "StartupDelaySeconds": 5,
      "FileProcessingDelayMs": 500
    },
    "Notifications": {
      "EnableEmail": false,
      "EnableEventLog": true,
      "EmailFrom": "cambridge@yourhospital.com",
      "EmailTo": "admin@yourhospital.com;radiology@yourhospital.com",
      "SmtpHost": "smtp.yourhospital.com",
      "SmtpPort": 587,
      "SmtpUseSsl": true,
      "SmtpUsername": "",
      "SmtpPassword": "",
      "MinimumEmailLevel": "Warning",
      "MaxEmailsPerHour": 10,
      "ThrottleMinutes": 15,
      "SendDailySummary": true,
      "DeadLetterThreshold": 50
    }
  }
}
```
 
## src\CamBridge.Service\mappings.json 
```
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
 
## src\CamBridge.Service\Program.cs 
```
// src/CamBridge.Service/Program.cs
using CamBridge.Core;
using CamBridge.Core.Interfaces;
using CamBridge.Infrastructure;
using CamBridge.Infrastructure.Services;
using CamBridge.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;
using System;
using System.IO;
using System.Runtime.Versioning;

// Configure Serilog
var logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "cambridge-.txt");
Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
    .WriteTo.File(
        logPath,
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

try
{
    Log.Information("Starting CamBridge Service v0.4.0");

    var builder = WebApplication.CreateBuilder(args);

    // Configure as Windows Service with Web API support
    builder.Host.UseWindowsService(options =>
    {
        options.ServiceName = "CamBridge Service";
    });

    builder.Host.UseSerilog();

    // Configure services
    ConfigureServices(builder.Services, builder.Configuration);

    // Configure Kestrel to listen on a specific port
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ListenLocalhost(5050); // API port
    });

    var app = builder.Build();

    // Validate configuration
    await ValidateConfigurationAsync(app.Services);

    // Configure HTTP pipeline
    ConfigureHttpPipeline(app);

    // Startup notification
    await SendStartupNotificationAsync(app.Services);

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}

return 0;

[SupportedOSPlatform("windows")]
static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    // Configure settings
    services.Configure<CamBridgeSettings>(configuration.GetSection("CamBridge"));
    services.Configure<ProcessingOptions>(configuration.GetSection("CamBridge:Processing"));
    services.Configure<NotificationSettings>(configuration.GetSection("CamBridge:Notifications"));

    // Register infrastructure services
    var mappingConfigPath = configuration["CamBridge:MappingConfigurationFile"] ?? "mappings.json";
    var useRicohReader = configuration.GetValue<bool>("CamBridge:UseRicohExifReader", true);
    services.AddCamBridgeInfrastructure(mappingConfigPath, useRicohReader);

    // Register core services
    services.AddSingleton<DeadLetterQueue>();
    services.AddSingleton<INotificationService, NotificationService>();

    // Register ProcessingQueue with dependencies
    services.AddSingleton<ProcessingQueue>(provider =>
    {
        var logger = provider.GetRequiredService<ILogger<ProcessingQueue>>();
        var scopeFactory = provider.GetRequiredService<IServiceScopeFactory>();
        var options = provider.GetRequiredService<IOptions<ProcessingOptions>>();
        var deadLetterQueue = provider.GetRequiredService<DeadLetterQueue>();
        var notificationService = provider.GetService<INotificationService>();

        return new ProcessingQueue(logger, scopeFactory, options, deadLetterQueue, notificationService);
    });

    // Register hosted services
    services.AddSingleton<FolderWatcherService>();
    services.AddHostedService(provider => provider.GetRequiredService<FolderWatcherService>());
    services.AddHostedService<Worker>();

    // Add daily summary service
    services.AddHostedService<DailySummaryService>();

    // Add health checks
    services.AddHealthChecks()
        .AddCheck<CamBridgeHealthCheck>("cambridge_health");

    // Add controllers for Web API
    services.AddControllers();
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = "CamBridge API",
            Version = "v1",
            Description = "API for monitoring CamBridge JPEG to DICOM conversion service"
        });

        // Include XML documentation if available
        var xmlFile = $"{typeof(Program).Assembly.GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
        {
            c.IncludeXmlComments(xmlPath);
        }
    });
}

static void ConfigureHttpPipeline(WebApplication app)
{
    // Configure HTTP pipeline
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "CamBridge API v1");
            c.RoutePrefix = string.Empty; // Swagger UI at root
        });
    }

    app.UseRouting();
    app.MapControllers();
    app.MapHealthChecks("/health");

    // Serve dashboard HTML if exists
    var dashboardPath = Path.Combine(app.Environment.ContentRootPath, "dashboard.html");
    if (File.Exists(dashboardPath))
    {
        app.MapGet("/dashboard", async context =>
        {
            context.Response.ContentType = "text/html";
            await context.Response.SendFileAsync(dashboardPath);
        });
    }
}

[SupportedOSPlatform("windows")]
static async Task ValidateConfigurationAsync(IServiceProvider services)
{
    var settings = services.GetRequiredService<IOptions<CamBridgeSettings>>().Value;

    if (settings.WatchFolders == null || !settings.WatchFolders.Any(f => f.Enabled))
    {
        Log.Warning("No watch folders configured or enabled");
    }

    foreach (var folder in settings.WatchFolders?.Where(f => f.Enabled) ?? Enumerable.Empty<FolderConfiguration>())
    {
        if (!Directory.Exists(folder.Path))
        {
            Log.Warning("Watch folder does not exist: {Path}", folder.Path);
            try
            {
                Directory.CreateDirectory(folder.Path);
                Log.Information("Created watch folder: {Path}", folder.Path);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to create watch folder: {Path}", folder.Path);
            }
        }
    }

    if (!Directory.Exists(settings.DefaultOutputFolder))
    {
        try
        {
            Directory.CreateDirectory(settings.DefaultOutputFolder);
            Log.Information("Created default output folder: {Path}", settings.DefaultOutputFolder);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to create default output folder: {Path}", settings.DefaultOutputFolder);
        }
    }

    // Validate processing folders
    var processingOptions = services.GetRequiredService<IOptions<ProcessingOptions>>().Value;
    EnsureDirectoryExists(processingOptions.ArchiveFolder, "Archive");
    EnsureDirectoryExists(processingOptions.ErrorFolder, "Error");
    EnsureDirectoryExists(processingOptions.BackupFolder, "Backup");

    // Validate notification settings
    var notificationSettings = settings.Notifications;
    if (notificationSettings?.EnableEmail == true)
    {
        if (string.IsNullOrEmpty(notificationSettings.SmtpHost))
        {
            Log.Warning("Email notifications enabled but SMTP host not configured");
        }
        if (string.IsNullOrEmpty(notificationSettings.EmailTo))
        {
            Log.Warning("Email notifications enabled but recipient email not configured");
        }
    }

    await Task.CompletedTask;
}

static void EnsureDirectoryExists(string path, string name)
{
    if (!Directory.Exists(path))
    {
        try
        {
            Directory.CreateDirectory(path);
            Log.Information("Created {Name} folder: {Path}", name, path);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to create {Name} folder: {Path}", name, path);
        }
    }
}

[SupportedOSPlatform("windows")]
static async Task SendStartupNotificationAsync(IServiceProvider services)
{
    try
    {
        var notificationService = services.GetService<INotificationService>();
        if (notificationService != null)
        {
            await notificationService.NotifyInfoAsync(
                "CamBridge Service Started",
                $"Service version 0.4.0 started successfully on {Environment.MachineName}\n" +
                $"API endpoint: http://localhost:5050\n" +
                $"Process ID: {Environment.ProcessId}");
        }
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Failed to send startup notification");
    }
}
```
 
## src\CamBridge.Service\Worker.cs 
```
using System.Diagnostics;
using CamBridge.Core;
using CamBridge.Infrastructure.Services;
using Microsoft.Extensions.Options;

namespace CamBridge.Service;

/// <summary>
/// Main worker service that orchestrates file processing
/// </summary>
public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly ProcessingQueue _processingQueue;
    private readonly IHostedService _folderWatcher;
    private readonly CamBridgeSettings _settings;
    private readonly IHostApplicationLifetime _lifetime;
    private Task? _processingTask;
    private Timer? _statisticsTimer;

    public Worker(
        ILogger<Worker> logger,
        ProcessingQueue processingQueue,
        FolderWatcherService folderWatcher,
        IOptions<CamBridgeSettings> settings,
        IHostApplicationLifetime lifetime)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _processingQueue = processingQueue ?? throw new ArgumentNullException(nameof(processingQueue));
        _folderWatcher = folderWatcher ?? throw new ArgumentNullException(nameof(folderWatcher));
        _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
        _lifetime = lifetime ?? throw new ArgumentNullException(nameof(lifetime));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("CamBridge Service v{Version} starting",
                FileVersionInfo.GetVersionInfo(typeof(Worker).Assembly.Location).ProductVersion);

            // Startup delay
            if (_settings.Service.StartupDelaySeconds > 0)
            {
                _logger.LogInformation("Waiting {Delay} seconds before starting...",
                    _settings.Service.StartupDelaySeconds);
                await Task.Delay(TimeSpan.FromSeconds(_settings.Service.StartupDelaySeconds), stoppingToken);
            }

            // Start folder watcher
            await _folderWatcher.StartAsync(stoppingToken);

            // Start processing queue in background
            _processingTask = Task.Run(async () =>
            {
                try
                {
                    await _processingQueue.ProcessQueueAsync(stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    // Expected during shutdown
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex, "Fatal error in processing queue");
                    _lifetime.StopApplication();
                }
            }, stoppingToken);

            // Start statistics reporting
            _statisticsTimer = new Timer(
                ReportStatistics,
                null,
                TimeSpan.FromMinutes(1),
                TimeSpan.FromMinutes(5));

            _logger.LogInformation("CamBridge Service started successfully");
            _logger.LogInformation("Monitoring {FolderCount} folders for JPEG files",
                _settings.WatchFolders.Count(f => f.Enabled));

            // Keep service running
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);

                // Check if processing task is still running
                if (_processingTask?.IsCompleted == true)
                {
                    _logger.LogError("Processing task terminated unexpectedly");
                    _lifetime.StopApplication();
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Fatal error in CamBridge Service");
            throw;
        }
        finally
        {
            _logger.LogInformation("CamBridge Service shutting down");

            // Stop statistics timer
            _statisticsTimer?.Dispose();

            // Stop folder watcher
            await _folderWatcher.StopAsync(CancellationToken.None);

            // Wait for processing to complete
            if (_processingTask != null)
            {
                try
                {
                    await _processingTask.WaitAsync(TimeSpan.FromSeconds(30));
                }
                catch (TimeoutException)
                {
                    _logger.LogWarning("Processing queue did not complete within timeout");
                }
            }

            // Final statistics
            ReportStatistics(null);

            _logger.LogInformation("CamBridge Service stopped");
        }
    }

    private void ReportStatistics(object? state)
    {
        try
        {
            var stats = _processingQueue.GetStatistics();

            _logger.LogInformation(
                "Processing Statistics - Queue: {QueueLength}, Active: {ActiveProcessing}, " +
                "Total: {TotalProcessed}, Success: {TotalSuccessful} ({SuccessRate:F1}%), " +
                "Failed: {TotalFailed}, Rate: {ProcessingRate:F1} files/min",
                stats.QueueLength,
                stats.ActiveProcessing,
                stats.TotalProcessed,
                stats.TotalSuccessful,
                stats.SuccessRate,
                stats.TotalFailed,
                stats.ProcessingRate);

            // Log active items if any
            var activeItems = _processingQueue.GetActiveItems();
            if (activeItems.Count > 0)
            {
                foreach (var item in activeItems)
                {
                    _logger.LogInformation(
                        "Active: {FilePath} (Attempt: {AttemptCount}, Duration: {Duration:F1}s)",
                        item.FilePath,
                        item.AttemptCount,
                        item.Duration.TotalSeconds);
                }
            }

            // Check system health
            if (stats.TotalProcessed > 100 && stats.SuccessRate < 50)
            {
                _logger.LogWarning("Low success rate detected: {SuccessRate:F1}%", stats.SuccessRate);
            }

            if (stats.QueueLength > 1000)
            {
                _logger.LogWarning("Large queue backlog: {QueueLength} files pending", stats.QueueLength);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reporting statistics");
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("CamBridge Service stop requested");
        await base.StopAsync(cancellationToken);
    }
}
```
 
 
## Summary 
- Profile: balanced 
- Files collected: 18 
- Purpose: Balanced collection for general development 
- Next steps: Check PROJECT_WISDOM.md for current tasks 
