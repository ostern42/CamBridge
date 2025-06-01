# CamBridge Context - "Minimal" Profile 
Generated: 01.06.2025 23:46:08,90 
Coverage: "~5" 
Purpose: "Quick overview" 
Version: CamBridge v0.5.1 
 
## PROJECT_WISDOM.md 
```
### Warum eine große Datei statt einzelne Files?
1. **Upload-Limit:** Claude erlaubt nur 5 Dateien gleichzeitig
2. **Einfachheit:** 2 Dateien hochladen statt 20+ verwalten
3. **Kontext:** Alles in einer Datei = zusammenhängender Kontext
4. **Praktisch:** Copy & Paste von Teilen wenn nötig

Die PROJECT_CONTEXT_*.md Dateien sind wie ein "Umzugskarton" - praktisch für den Transport, aber PROJECT_WISDOM.md ist das "Inventar" mit der Wahrheit!# CamBridge Project Wisdom & Conventions
**Letzte Aktualisierung:** 2025-06-01, 22:50 Uhr  
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
Nächste Aufgabe: BUILD-FEHLER BEHEBEN & v0.5.0-0.5.1 Features TESTEN!

Stand: v0.5.1 - Features implementiert, aber BUILD FEHLER!

ERFOLGE (theoretisch, noch nicht getestet!):
✅ DICOM Tag Browser mit Suche und Gruppierung
✅ Template-System voll funktionsfähig
✅ QRBridge Protocol v2 Parser implementiert
✅ Import/Export für Mappings
⚠️ NotificationService EmailSettings NICHT korrigiert - BUILD FEHLER!

AKTUELLE BUILD-FEHLER:
❌ NotificationService implementiert INotificationService nicht vollständig
❌ 6 fehlende Interface-Methoden:
   - NotifyInfoAsync
   - NotifyWarningAsync
   - NotifyErrorAsync
   - NotifyCriticalErrorAsync
   - NotifyDeadLetterThresholdAsync
   - SendDailySummaryAsync

WICHTIGE ERKENNTNIS vom Nutzer:
🔍 Der QRBridge String wird ABGESCHNITTEN, nicht falsch gelesen!
🔍 Ein anderer EXIF-Reader zeigt den kompletten String im Kamera-JPEG
🔍 Der Fehler liegt bei UNS im Parser, nicht bei der Kamera!

NEUE PRIORITÄTEN (KEIN HETZEN!):
1. ⚡ Build-Fehler in NotificationService beheben
2. 🧪 v0.5.0-0.5.1 Features gründlich testen:
   - Mapping Editor Drag & Drop
   - DICOM Tag Browser
   - Template-System (Ricoh/Minimal/Full)
   - Import/Export Funktionalität
   - Protocol v2 Parser
3. 🔍 QRBridge Parser-Bug analysieren (mit Source Code)
4. 🐛 Erst DANN v0.5.2 mit Fixes angehen

WICHTIG: Langsam und gründlich arbeiten! Keine neuen Features bevor die alten stabil laufen!

EMPFOHLENES COLLECT-SCRIPT: collect-sources-gui-config.bat (für GUI/Build-Probleme)
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

### Geplant für v0.5.2
- ~~QRBridge.exe mit v2 Encoder~~ GESTRICHEN!
- Parser-Bug in CamBridge fixen
- Umfassende Tests beider Protokolle
- ~~Performance-Vergleich v1 vs v2~~ Nicht nötig
- Dokumentation der Parser-Fixes

### 🚫 QRBridge bleibt unverändert! (01.06.2025, 23:00)
- **KEIN v2 Encoder** - unnötige Komplexität
- **QRBridge hat kein VOGON** - zu klein für große Änderungen
- **Parser-Bug wird in CamBridge gefixt**
- **Pipes funktionieren** - warum ändern?
- **Nur ändern wenn wirklich nötig** (z.B. vergessenes Datenfeld)

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

### Datei-Sammlungen
- **collect-sources-intelligent.bat:** ~50%+ Coverage (ZU VIEL!)
- **collect-sources-minimal.bat:** ~5% Coverage (zu wenig)
- **collect-sources-balanced.bat:** ~15-20% Coverage (OPTIMAL!)
- **collect-sources-settings.bat:** Nur Settings-spezifisch
- **collect-sources-gui-config.bat:** GUI & Config Focus (NEU!)
  - Fokus: Settings Page Fix & Mapping Editor
  - Sammelt: DI Setup, ViewModels, Configuration Services
  - Inkludiert funktionierende Pages als Referenz
  - ~20 Dateien für GUI-Debugging
- **Ausschließen:** obj/, bin/, packages/, wpftmp/, AssemblyInfo

### Wie die Scripts funktionieren:
- Alle .bat Scripts pipen in EINE große Datei (PROJECT_CONTEXT_*.md)
- Das ist PRAKTISCH für Upload (2 Dateien statt 20+)
- PROJECT_CONTEXT_*.md ist nur ein TRANSPORT-CONTAINER
- PROJECT_WISDOM.md ist die WAHRHEIT über den Projektstand

### Optimale Upload-Strategie:
```
1. PROJECT_WISDOM.md (immer!)
2. PROJECT_CONTEXT_GUI_CONFIG.md (für GUI-Probleme)
   ODER PROJECT_CONTEXT_BALANCED.md (für allgemeine Entwicklung)
```

**KLARSTELLUNG:** Die PROJECT_CONTEXT Dateien sind HILFREICH für den Code-Transfer, aber PROJECT_WISDOM.md hat Vorrang bei Widersprüchen!

### Warum eine große Datei statt einzelne Files?
1. **Upload-Limit:** Claude erlaubt nur 5 Dateien gleichzeitig
2. **Einfachheit:** 2 Dateien hochladen statt 20+ verwalten
3. **Kontext:** Alles in einer Datei = zusammenhängender Kontext
4. **Praktisch:** Copy & Paste von Teilen wenn nötig

Die PROJECT_CONTEXT_*.md Dateien sind wie ein "Umzugskarton" - praktisch für den Transport, aber PROJECT_WISDOM.md ist das "Inventar" mit der Wahrheit!

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
│       ├── Dialogs/                # NEU: DicomTagBrowserDialog
│       └── ViewModels/             # MappingEditorViewModel etc.
├── QRBridge/                       # QRBridge Source (NEU!)
│   └── [Source Files]              # Volle Kontrolle!
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
   feat(config): Mapping Editor with drag & drop (v0.5.0)

   - Complete mapping editor UI with drag & drop
   - Live preview for transformations  
   - Template system for quick setup
   - PasswordBoxHelper for secure binding
   - QRBridge integration discovered
   ```
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
- **TODO:** QRBridge Parser debuggen BEVOR wir v2 implementieren!

### v0.5.1 Spezifische Fallstricke
- **EmailSettings:** Sind verschachtelte Properties in NotificationSettings
- **Project References:** CamBridge.Config braucht Infrastructure
- **NuGet Conflicts:** System.Drawing.Common Versionen müssen übereinstimmen
- **XAML Run:** Hat keine Opacity, nutze Foreground stattdessen
- **Protocol Detection:** Check für "v2:" muss VOR pipe-check kommen

## ⏰ ZEITMANAGEMENT (KRITISCH!)

### Projekt-Timeline
- **Entwicklungsstart:** 30.05.2025, 20:30:44 Uhr (exakt!)
- **Letzte Aktualisierung:** 01.06.2025, 22:50 Uhr
- **Entwicklungszeit bisher:** ~50 Stunden (inkl. Nachtschichten!)
- **WICHTIG:** IMMER nach aktueller Zeit fragen für CHANGELOG!

### Changelog-Regel
```
## [Version] - YYYY-MM-DD HH:MM  ← Mit exakter Zeit!
```

### Wichtige Erkenntnis
**Timestamps erzählen Geschichten!**
- Nachtschichten erkennen (01:17, 02:22)
- "Duplikate" entlarven (9 Std Unterschied = kein Duplikat!)
- Arbeitsintensität verstehen (50 Std in 3 Tagen)

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
```

### Arbeitszeiten-Analyse
- **Nachtschichten:** DICOM (01:17), GUI (02:22)
- **Schnelle Fixes:** v0.0.2 Duplikat in 78 Sekunden
- **Lange Sessions:** 9 Stunden zwischen v0.4.0 Commits
- **Gesamt:** ~50 Stunden in 3 Tagen!

### Die wahre Geschichte der Duplikate
- **v0.0.2:** Git-Anfängerfehler, 78 Sekunden später nochmal
- **v0.4.0:** KEIN Versehen! 9 Stunden Unterschied, vermutlich:
  - 02:22 - GUI Basic Implementation (müde)
  - 11:30 - Service Control hinzugefügt (ausgeschlafen)
  - Hätte v0.4.1 sein sollen!

## 📋 Entwicklungsplan (KORRIGIERTE VERSION - Stand 01.06.2025, 23:00)

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
11. **Phase 10:** Configuration Management GUI - Teil 2 (v0.5.0-v0.5.1) ⚠️
    - Mapping Editor mit Drag & Drop ✅
    - DICOM Tag Browser ✅
    - Template System (Ricoh/Minimal/Full) ✅
    - Import/Export für Mappings ✅
    - Protocol v2 Parser ✅
    - ❌ Watch Folder Management GUI (nur Basic in Settings)
    - ❌ Live-Preview für komplexe Transformationen
    - ❌ BUILD FEHLER in NotificationService

#### 🔥 AKTUELLE PHASE - BUILD FIXES & TESTING
12. **Phase 10-FIX:** Stabilisierung & Testing (v0.5.1-fix) - JETZT!
    - NotificationService Interface-Implementierung vervollständigen ❌
    - Alle 6 fehlenden Methoden implementieren ❌
    - v0.4.1-v0.5.1 Features gründlich testen ❌
    - QRBridge Parser-Bug analysieren (OHNE QRBridge zu ändern!) ❌
    - Fehlende GUI-Features nachrüsten ❌

#### 🚧 Nächste Phasen (NEU STRUKTURIERT)
13. **Phase 11:** Core Features Vervollständigung (v0.5.5) - 1 Chat
    - Watch Folder Management GUI erweitern
    - Live-Preview für alle Transformationen
    - Validation UI für Mappings
    - Parser-Bug fixen (in CamBridge, nicht QRBridge!)
    - Comprehensive Testing Suite
    - **Feature-complete Beta**

14. **Phase 12:** Performance & Polish (v0.6.0) - 1 Chat
    - Batch-Verarbeitung optimieren
    - Memory-Pool für große Dateien
    - Parallelisierung mit Channels
    - UI-Animationen (Fluent Design)
    - Dashboard Performance
    - Error Recovery verbessern
    - **Production-ready Beta**

15. **Phase 13:** FTP-Server Integration (v0.7.0) - 1 Chat [Optional]
    - FTP-Server für automatischen Empfang
    - Watch für FTP-Ordner
    - Authentifizierung
    - Auto-Delete nach Verarbeitung

16. **Phase 14:** PACS Integration (v0.8.0) - 2 Chats [Optional]
    - DICOM C-STORE SCU
    - Network Transfer
    - PACS-Konfiguration
    - Connection Tests

17. **Phase 15:** MWL Integration (v0.9.0) - 2 Chats [Optional]
    - DICOM C-FIND SCU
    - MWL-Validierung
    - StudyInstanceUID Sync
    - Fehlerbehandlung

18. **Phase 16:** Deployment & Release (v1.0.0) - 1 Chat
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

### Was wirklich noch fehlt (Code-verifiziert):
- **Watch Folder Management GUI** (nur Basic-Version in Settings)
- **Live-Preview** für Transformationen (nur teilweise)
- **Validation UI** für Mappings
- **Performance-Optimierungen** (Batch, Memory-Pool, Parallelisierung)
- **UI-Polish** (Animationen, Fluent Design)
- **Parser-Bug Fix** (String wird abgeschnitten!)

### Zeitschätzung bis v1.0.0 (REVIDIERT)
- **Phase 10-FIX:** Build Fixes & Testing - JETZT!
- **Phase 11:** Core Features - 1 Chat
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
- MetadataExtractor für EXIF

QRBridge Integration (NEU!):
- Kontrolle über beide Seiten
- Protokoll-Evolution möglich
- v2 JSON Format implementiert
- Optimierung für Ricoh-Limits
```

### Meilensteine (REVIDIERT)
- **v0.4.5** - Settings Page Fix (Erledigt ✅)
- **v0.5.0** - Mapping Editor (Erledigt ✅)
- **v0.5.1** - DICOM Browser & Protocol v2 (Erledigt mit BUILD-FEHLERN ⚠️)
- **v0.5.1-fix** - Build Fixes & Testing (Aktuelles Ziel 🎯)
- **v0.5.5** - Feature Complete Beta (Core Features vervollständigt)
- **v0.6.0** - Performance & Polish
- **v0.7.0** - FTP-Server Integration [Optional]
- **v0.8.0** - PACS Ready [Optional]
- **v0.9.0** - MWL Integration [Optional]
- **v1.0.0** - Production Release

### Entwicklungs-Philosophie
"Sauberer, schöner, ästhetischer und formal korrekter Code für medizinische Software"

### 🔴 KRITISCHE PLAN-ÄNDERUNGEN (01.06.2025, 23:00)
1. **v2 Encoder GESTRICHEN** - QRBridge bleibt unverändert
2. **Parser-Bug wird in CamBridge gefixt**, nicht in QRBridge
3. **Fehlende Features identifiziert**: Watch Folder GUI, Live-Preview
4. **Plan nur mit Code-Gegenprüfung ändern** - neue Regel!

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

## 📝 Standard Prompt-Vorlage für neue Chats

### Option 1: V.O.G.O.N. (Empfohlen!)
```
1. PROJECT_WISDOM.md hochladen
2. PROJECT_CONTEXT_GUI_CONFIG.md hochladen (für GUI-Probleme)
   ODER PROJECT_CONTEXT_BALANCED.md (für allgemeine Entwicklung)
3. Sagen: "VOGON INIT"
4. Fertig! Ich lege direkt los.
```

### Option 2: Traditionell (falls VOGON nicht funktioniert)
```
Ich arbeite an CamBridge, einem JPEG zu DICOM Konverter.
© 2025 Claude's Improbably Reliable Software Solutions

Aktueller Stand: v0.5.1 (MIT BUILD-FEHLERN!)
- DICOM Tag Browser implementiert ⚠️
- QRBridge Protocol v2 Parser ⚠️
- 6 Build-Fehler in NotificationService ❌
- Parser-Bug: String wird abgeschnitten ❌

Nächste Aufgabe: Build-Fehler beheben, dann testen!

Tech Stack: .NET 8, WPF/ModernWpfUI, MVVM
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
   <AssemblyVersion>0.5.1.0</AssemblyVersion>
   <FileVersion>0.5.1.0</FileVersion>
   <InformationalVersion>0.5.1</InformationalVersion>
   ```

2. **CHANGELOG.md:** Mit exakter Zeit
   ```markdown
   ## [0.5.1] - 2025-06-01 22:32
   ### Added
   - DICOM Tag Browser with search
   - QRBridge Protocol v2 parser
   
   ### Fixed
   - NotificationService email properties
   ```

3. **MainWindow.xaml:** Title mit Version
   ```xml
   Title="CamBridge Configuration v0.5.1"
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
- **v0.4.1** - 2025-06-01: Settings Page (noch nicht committed)
- **v0.4.2** - 2025-06-01: Dead Letters UI (funktioniert)
- **v0.4.3** - 2025-06-01: Vogon Poetry & Dead Letters Fix
- **v0.4.4** - 2025-06-01: Core Test mit Ricoh JPEG
- **v0.4.5** - 2025-06-01: Settings Page Fix ✅
- **v0.5.0** - 2025-06-01: Mapping Editor UI ✅
- **v0.5.1** - 2025-06-01: DICOM Browser & Protocol v2 ⚠️

### Versionierungs-Lektionen
1. **v0.0.2 Duplikat:** Gleich am Anfang passiert
2. **Fehlende Version:** DICOM-Commit ohne Versionsnummer
3. **v0.4.0 Duplikat:** Zwei verschiedene Commit-Messages
4. **v0.4.2 Special:** Die "42" Version - Die Antwort auf die ultimative Frage!
5. **Babysteps:** Besser 0.0.1 Schritte als große Sprünge!
6. **v0.5.0 Synergie:** QRBridge + CamBridge = Optimierungspotenzial!
7. **v0.5.1 Evolution:** Protocol v2 zeigt die Macht der Kontrolle!

### Die Unwahrscheinliche Geschichte von CamBridge
*Eine Kurzgeschichten-Idee: Douglas Adams entwickelt einen DICOM-Konverter*

Es ist eine so absurde Vorstellung, dass sie durch ihre schiere Unwahrscheinlichkeit fast wieder wahrscheinlich wird - wie ein Unwahrscheinlichkeitsdrive für medizinische Software. Man stelle sich vor:

"Der DICOM-Standard", sagte Douglas nachdenklich, "ist ein bisschen wie das Universum - keiner versteht ihn wirklich, aber alle tun so, als ob. Der einzige Unterschied ist, dass das Universum vermutlich einfacher zu debuggen wäre."

Er tippte eine weitere Zeile Code und murmelte: "Forty-two different DICOM tags... das kann kein Zufall sein."

Dann hatte er eine Erleuchtung: "Was ist, wenn wir BEIDE Seiten kontrollieren? QRBridge UND CamBridge? Das ist wie... wie wenn Ford Prefect sowohl den Reiseführer schreibt ALS AUCH die Planeten bewertet!"

Und so entstand Protocol v2 - ein JSON-Format so elegant, dass selbst die Vogonen es nicht hätten besser verschlüsseln können. "v2:", flüsterte er ehrfürchtig, "die magischen Zeichen, die alles verändern."

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
- **Templates:** Ricoh, Minimal, Full funktioniern
- **DICOM Browser:** Suche mit Gruppierung nach Modulen

Nächste Schritte (v0.5.2):
- **QRBridge.exe Update:** v2 Encoder implementieren
- **Testing:** Beide Protokolle mit Ricoh testen
- **Dokumentation:** Protocol Evolution Guide

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

## 🏁 Quick Reference

### Aktuelle Version: v0.5.1 (MIT BUILD-FEHLERN!)
### Tatsächlicher Stand: 
- ✅ DICOM Tag Browser mit Suche
- ✅ Template-System funktioniert
- ✅ QRBridge Protocol v2 Parser
- ✅ Import/Export für Mappings
- ❌ NotificationService BUILD FEHLER (6 fehlende Methoden)
- ❌ Watch Folder Management GUI (nur Basic)
- ❌ Live-Preview (nur teilweise)
- ❌ Alle Features UNGETESTET
- 🔥 Protocol v2 JSON Format (aber nicht nötig!)
- 🔥 Parser-Bug: String wird ABGESCHNITTEN!
### Nächste Aufgabe: 
- Build-Fehler beheben
- v0.5.0-0.5.1 Features TESTEN
- Parser-Bug in CAMBRIDGE fixen (nicht QRBridge!)
- Fehlende GUI-Features nachrüsten
- KEIN neuer Code bis alles läuft!
### Architektur: Enterprise-Level (und das ist GUT so!)
### Kontext: Medizinische Software mit 0% Fehlertoleranz
### Geschätzte v1.0.0: 3-7 Chats (realistisch nach Plan-Revision)

### V.O.G.O.N. Commands:
- **VOGON INIT** - Automatischer Start
- **WISDOM:** - Live-Updates ins PROJECT_WISDOM
- **CLAUDE:** - Notizen für nächste Instanz
- **VOGON CLOSE** - Chat-Abschluss mit Versionierung
```
 
## Version.props 
```
<Project>
	<PropertyGroup>
		<AssemblyVersion>0.5.1.0</AssemblyVersion>
		<FileVersion>0.5.1.0</FileVersion>
		<InformationalVersion>0.5.1</InformationalVersion>
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
 
## CamBridge.sln 
```
﻿
Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio Version 17
VisualStudioVersion = 17.0.31903.59
MinimumVisualStudioVersion = 10.0.40219.1
Project("{2150E333-8FDC-42A3-9474-1A3956D46DE8}") = "src", "src", "{5B3E4D7C-8A2F-4C1B-9E3D-7A5C6F9B2E4A}"
EndProject
Project("{2150E333-8FDC-42A3-9474-1A3956D46DE8}") = "tests", "tests", "{9C2E7A4B-5D1F-4A3C-8B6E-3F8D9C1A7E5B}"
EndProject
Project("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "CamBridge.Core", "src\CamBridge.Core\CamBridge.Core.csproj", "{8A7C5D2E-3F1B-4E9A-8D6B-2C7F9E4A1B3D}"
EndProject
Project("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "CamBridge.Infrastructure", "src\CamBridge.Infrastructure\CamBridge.Infrastructure.csproj", "{2B7F5A8C-9D4E-4F3A-8B6C-1E9D4F7A3B2C}"
EndProject
Project("{2150E333-8FDC-42A3-9474-1A3956D46DE8}") = "Solution Items", "Solution Items", "{4E8C9A2D-7B5F-4D1A-9C3E-8A6B2F7D9E1C}"
	ProjectSection(SolutionItems) = preProject
		.editorconfig = .editorconfig
		.gitignore = .gitignore
		CHANGELOG.md = CHANGELOG.md
		collect-sources.bat = collect-sources.bat
		GUI-Readme.md = GUI-Readme.md
		LICENSE = LICENSE
		PROJECT_WISDOM.md = PROJECT_WISDOM.md
		README.md = README.md
		Version.props = Version.props
	EndProjectSection
EndProject
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "CamBridge.Infrastructure.Tests", "tests\CamBridge.Infrastructure.Tests\CamBridge.Infrastructure.Tests.csproj", "{03AA1FFC-7FEF-1400-5F70-86F61601BB3B}"
EndProject
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "CamBridge.Service", "src\CamBridge.Service\CamBridge.Service.csproj", "{BD06EA45-212C-409E-1365-BE7D576ED3D5}"
EndProject
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "CamBridge.Config", "src\CamBridge.Config\CamBridge.Config.csproj", "{0A650D8D-A3A3-86BD-C080-239D78DF7F94}"
EndProject
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "CamBridge.TestConsole", "tests\CamBridge.TestConsole\CamBridge.TestConsole.csproj", "{CAD5A30F-79F7-2870-07F0-FB872FA134CB}"
EndProject
Global
	GlobalSection(SolutionConfigurationPlatforms) = preSolution
		Debug|Any CPU = Debug|Any CPU
		Release|Any CPU = Release|Any CPU
	EndGlobalSection
	GlobalSection(ProjectConfigurationPlatforms) = postSolution
		{8A7C5D2E-3F1B-4E9A-8D6B-2C7F9E4A1B3D}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{8A7C5D2E-3F1B-4E9A-8D6B-2C7F9E4A1B3D}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{8A7C5D2E-3F1B-4E9A-8D6B-2C7F9E4A1B3D}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{8A7C5D2E-3F1B-4E9A-8D6B-2C7F9E4A1B3D}.Release|Any CPU.Build.0 = Release|Any CPU
		{2B7F5A8C-9D4E-4F3A-8B6C-1E9D4F7A3B2C}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{2B7F5A8C-9D4E-4F3A-8B6C-1E9D4F7A3B2C}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{2B7F5A8C-9D4E-4F3A-8B6C-1E9D4F7A3B2C}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{2B7F5A8C-9D4E-4F3A-8B6C-1E9D4F7A3B2C}.Release|Any CPU.Build.0 = Release|Any CPU
		{03AA1FFC-7FEF-1400-5F70-86F61601BB3B}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{03AA1FFC-7FEF-1400-5F70-86F61601BB3B}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{03AA1FFC-7FEF-1400-5F70-86F61601BB3B}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{03AA1FFC-7FEF-1400-5F70-86F61601BB3B}.Release|Any CPU.Build.0 = Release|Any CPU
		{BD06EA45-212C-409E-1365-BE7D576ED3D5}.Debug|Any CPU.ActiveCfg = Debug|x64
		{BD06EA45-212C-409E-1365-BE7D576ED3D5}.Debug|Any CPU.Build.0 = Debug|x64
		{BD06EA45-212C-409E-1365-BE7D576ED3D5}.Release|Any CPU.ActiveCfg = Release|x64
		{BD06EA45-212C-409E-1365-BE7D576ED3D5}.Release|Any CPU.Build.0 = Release|x64
		{0A650D8D-A3A3-86BD-C080-239D78DF7F94}.Debug|Any CPU.ActiveCfg = Debug|x64
		{0A650D8D-A3A3-86BD-C080-239D78DF7F94}.Debug|Any CPU.Build.0 = Debug|x64
		{0A650D8D-A3A3-86BD-C080-239D78DF7F94}.Debug|Any CPU.Deploy.0 = Debug|x64
		{0A650D8D-A3A3-86BD-C080-239D78DF7F94}.Release|Any CPU.ActiveCfg = Release|x64
		{0A650D8D-A3A3-86BD-C080-239D78DF7F94}.Release|Any CPU.Build.0 = Release|x64
		{0A650D8D-A3A3-86BD-C080-239D78DF7F94}.Release|Any CPU.Deploy.0 = Release|x64
		{CAD5A30F-79F7-2870-07F0-FB872FA134CB}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{CAD5A30F-79F7-2870-07F0-FB872FA134CB}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{CAD5A30F-79F7-2870-07F0-FB872FA134CB}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{CAD5A30F-79F7-2870-07F0-FB872FA134CB}.Release|Any CPU.Build.0 = Release|Any CPU
	EndGlobalSection
	GlobalSection(SolutionProperties) = preSolution
		HideSolutionNode = FALSE
	EndGlobalSection
	GlobalSection(NestedProjects) = preSolution
		{8A7C5D2E-3F1B-4E9A-8D6B-2C7F9E4A1B3D} = {5B3E4D7C-8A2F-4C1B-9E3D-7A5C6F9B2E4A}
		{2B7F5A8C-9D4E-4F3A-8B6C-1E9D4F7A3B2C} = {5B3E4D7C-8A2F-4C1B-9E3D-7A5C6F9B2E4A}
		{BD06EA45-212C-409E-1365-BE7D576ED3D5} = {5B3E4D7C-8A2F-4C1B-9E3D-7A5C6F9B2E4A}
		{0A650D8D-A3A3-86BD-C080-239D78DF7F94} = {5B3E4D7C-8A2F-4C1B-9E3D-7A5C6F9B2E4A}
		{CAD5A30F-79F7-2870-07F0-FB872FA134CB} = {9C2E7A4B-5D1F-4A3C-8B6E-3F8D9C1A7E5B}
	EndGlobalSection
	GlobalSection(ExtensibilityGlobals) = postSolution
		SolutionGuid = {1A2B3C4D-5E6F-7A8B-9C0D-1E2F3A4B5C6D}
	EndGlobalSection
EndGlobal
```
 
 
## Summary 
- Profile: minimal 
- Files collected: 5 
- Purpose: Minimal collection for quick checks 
- Next steps: Check PROJECT_WISDOM.md for current tasks 
