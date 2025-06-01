# CamBridge Project Context - GUI Configuration Focus 
 
Generated: 01.06.2025 23:28:38,87 
Focus: Settings Page Fix, Configuration Management, Mapping Editor 
 
## Key Project Files 
 
### PROJECT_WISDOM.md 
<details><summary>View Content</summary> 
 
```markdown 
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
</details> 
 
### Version.props 
```xml 
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
                    services.AddSingleton<IConfigurationService, ConfigurationService>(); // FIXED: Added missing registration!

                    // ViewModels - WICHTIG: Alle müssen registriert sein!
                    services.AddTransient<MainViewModel>();
                    services.AddTransient<DashboardViewModel>();
                    services.AddTransient<ServiceControlViewModel>();
                    services.AddTransient<SettingsViewModel>();
                    services.AddTransient<DeadLettersViewModel>();
                    services.AddTransient<MappingEditorViewModel>();

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
                    case "MappingEditor":
                        ContentFrame.Navigate(new MappingEditorPage());
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
      xmlns:helpers="clr-namespace:CamBridge.Config.Helpers"
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
                                <!-- FIXED: PasswordBox with proper binding -->
                                <PasswordBox Grid.Row="6" Grid.Column="1"
                                             helpers:PasswordBoxHelper.BindPassword="True"
                                             helpers:PasswordBoxHelper.BoundPassword="{Binding SmtpPassword, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}"
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
using System;
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
        private SettingsViewModel? _viewModel;

        public SettingsPage()
        {
            InitializeComponent();

            // Defer ViewModel initialization to Loaded event
            Loaded += SettingsPage_Loaded;
        }

        private async void SettingsPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Only initialize once
            if (_viewModel != null) return;

            try
            {
                // Get ViewModel from DI with null safety
                var app = Application.Current as App;
                if (app?.Host != null)
                {
                    _viewModel = app.Host.Services.GetRequiredService<SettingsViewModel>();
                    DataContext = _viewModel;

                    // Initialize the view model after setting DataContext
                    await _viewModel.InitializeAsync();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("App.Host is null - DI not available");
                    ShowErrorMessage("Configuration service not available");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading SettingsViewModel: {ex.Message}");
                ShowErrorMessage($"Failed to load settings: {ex.Message}");
            }
        }

        private void ShowErrorMessage(string message)
        {
            // Create a simple error display
            var errorText = new TextBlock
            {
                Text = message,
                Margin = new Thickness(20),
                FontSize = 16,
                Foreground = System.Windows.Media.Brushes.Red
            };

            Content = new Grid { Children = { errorText } };
        }

        // Number validation for TextBox inputs
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        // Browse folder dialogs with better error handling
        private void BrowseWatchFolder_Click(object sender, RoutedEventArgs e)
        {
            try
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
                    if (_viewModel?.SelectedWatchFolder != null && !string.IsNullOrEmpty(folderPath))
                    {
                        _viewModel.SelectedWatchFolder.Path = folderPath;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error selecting folder: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BrowseOutputFolder_Click(object sender, RoutedEventArgs e)
        {
            try
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
                    if (_viewModel != null && !string.IsNullOrEmpty(folderPath))
                    {
                        _viewModel.DefaultOutputFolder = folderPath;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error selecting folder: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BrowseLogFolder_Click(object sender, RoutedEventArgs e)
        {
            try
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
                    if (_viewModel != null && !string.IsNullOrEmpty(folderPath))
                    {
                        _viewModel.LogFolder = folderPath;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error selecting folder: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
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
            try
            {
                await LoadSettingsAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing SettingsViewModel: {ex.Message}");
                StatusMessage = "Failed to load settings";
                IsError = true;
            }
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
            // Watch Folders
            WatchFolders.Clear();
            foreach (var folder in settings.WatchFolders)
            {
                var folderVm = new FolderConfigurationViewModel
                {
                    Path = folder.Path,
                    OutputPath = folder.OutputPath,
                    Enabled = folder.Enabled,
                    IncludeSubdirectories = folder.IncludeSubdirectories,
                    FilePattern = folder.FilePattern
                };

                // Subscribe to changes
                folderVm.PropertyChanged += (s, e) => HasChanges = true;
                WatchFolders.Add(folderVm);
            }

            // Processing Options
            DefaultOutputFolder = settings.DefaultOutputFolder;
            SuccessAction = settings.Processing.SuccessAction;
            FailureAction = settings.Processing.FailureAction;
            ArchiveFolder = settings.Processing.ArchiveFolder;
            ErrorFolder = settings.Processing.ErrorFolder;
            CreateBackup = settings.Processing.CreateBackup;
            BackupFolder = settings.Processing.BackupFolder;
            MaxConcurrentProcessing = settings.Processing.MaxConcurrentProcessing;
            RetryOnFailure = settings.Processing.RetryOnFailure;
            MaxRetryAttempts = settings.Processing.MaxRetryAttempts;
            OutputOrganization = settings.Processing.OutputOrganization;
            ProcessExistingOnStartup = settings.Processing.ProcessExistingOnStartup;

            if (settings.Processing.MaxFileAge.HasValue)
            {
                MaxFileAgeDays = (int)settings.Processing.MaxFileAge.Value.TotalDays;
            }

            // DICOM Settings
            ImplementationClassUid = settings.Dicom.ImplementationClassUid;
            ImplementationVersionName = settings.Dicom.ImplementationVersionName;
            InstitutionName = settings.Dicom.InstitutionName;
            StationName = settings.Dicom.StationName;
            ValidateAfterCreation = settings.Dicom.ValidateAfterCreation;

            // Notification Settings - safe navigation
            EnableEmail = settings.Notifications?.EnableEmail ?? false;
            EnableEventLog = settings.Notifications?.EnableEventLog ?? true;

            if (settings.Notifications?.Email != null)
            {
                EmailFrom = settings.Notifications.Email.From;
                EmailTo = settings.Notifications.Email.To;
                SmtpHost = settings.Notifications.Email.SmtpHost;
                SmtpPort = settings.Notifications.Email.SmtpPort;
                SmtpUseSsl = settings.Notifications.Email.UseSsl;
                SmtpUsername = settings.Notifications.Email.Username;
                SmtpPassword = settings.Notifications.Email.Password;
            }

            MinimumEmailLevel = settings.Notifications?.MinimumEmailLevel ?? "Warning";
            SendDailySummary = settings.Notifications?.SendDailySummary ?? false;
            DailySummaryHour = settings.Notifications?.DailySummaryHour ?? 8;

            // Logging Settings
            LogLevel = settings.Logging.LogLevel;
            LogFolder = settings.Logging.LogFolder;
            EnableFileLogging = settings.Logging.EnableFileLogging;
            EnableServiceEventLog = settings.Logging.EnableEventLog;
            MaxLogFileSizeMB = settings.Logging.MaxLogFileSizeMB;
            MaxLogFiles = settings.Logging.MaxLogFiles;

            // Service Settings
            StartupDelaySeconds = settings.Service.StartupDelaySeconds;
            FileProcessingDelayMs = settings.Service.FileProcessingDelayMs;
        }

        private CamBridgeSettings MapToSettings()
        {
            var settings = new CamBridgeSettings
            {
                DefaultOutputFolder = DefaultOutputFolder,
                Processing = new ProcessingOptions
                {
                    SuccessAction = SuccessAction,
                    FailureAction = FailureAction,
                    ArchiveFolder = ArchiveFolder,
                    ErrorFolder = ErrorFolder,
                    CreateBackup = CreateBackup,
                    BackupFolder = BackupFolder,
                    MaxConcurrentProcessing = MaxConcurrentProcessing,
                    RetryOnFailure = RetryOnFailure,
                    MaxRetryAttempts = MaxRetryAttempts,
                    OutputOrganization = OutputOrganization,
                    ProcessExistingOnStartup = ProcessExistingOnStartup,
                    MaxFileAge = TimeSpan.FromDays(MaxFileAgeDays)
                },
                Dicom = new DicomSettings
                {
                    ImplementationClassUid = ImplementationClassUid,
                    ImplementationVersionName = ImplementationVersionName,
                    InstitutionName = InstitutionName,
                    StationName = StationName,
                    ValidateAfterCreation = ValidateAfterCreation
                },
                Notifications = new NotificationSettings
                {
                    EnableEmail = EnableEmail,
                    EnableEventLog = EnableEventLog,
                    Email = new EmailSettings
                    {
                        From = EmailFrom,
                        To = EmailTo,
                        SmtpHost = SmtpHost,
                        SmtpPort = SmtpPort,
                        UseSsl = SmtpUseSsl,
                        Username = SmtpUsername,
                        Password = SmtpPassword
                    },
                    MinimumEmailLevel = MinimumEmailLevel,
                    SendDailySummary = SendDailySummary,
                    DailySummaryHour = DailySummaryHour
                },
                Logging = new LoggingSettings
                {
                    LogLevel = LogLevel,
                    LogFolder = LogFolder,
                    EnableFileLogging = EnableFileLogging,
                    EnableEventLog = EnableServiceEventLog,
                    MaxLogFileSizeMB = MaxLogFileSizeMB,
                    MaxLogFiles = MaxLogFiles
                },
                Service = new ServiceSettings
                {
                    StartupDelaySeconds = StartupDelaySeconds,
                    FileProcessingDelayMs = FileProcessingDelayMs
                }
            };

            // Map Watch Folders
            settings.WatchFolders.Clear();
            foreach (var folder in WatchFolders)
            {
                settings.WatchFolders.Add(new FolderConfiguration
                {
                    Path = folder.Path,
                    OutputPath = folder.OutputPath,
                    Enabled = folder.Enabled,
                    IncludeSubdirectories = folder.IncludeSubdirectories,
                    FilePattern = folder.FilePattern
                });
            }

            return settings;
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
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CamBridge.Config.Services
{
    /// <summary>
    /// Service for loading and saving configuration from JSON files
    /// </summary>
    public class ConfigurationService : IConfigurationService
    {
        private readonly string _configDirectory;
        private readonly JsonSerializerOptions _jsonOptions;

        public ConfigurationService()
        {
            // Use AppData for config storage
            _configDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "CamBridge");

            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                Converters =
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };

            // Ensure directory exists
            Directory.CreateDirectory(_configDirectory);
        }

        public async Task<T?> LoadConfigurationAsync<T>() where T : class
        {
            if (typeof(T) != typeof(CamBridgeSettings))
            {
                throw new NotSupportedException($"Configuration type {typeof(T).Name} is not supported");
            }

            var configFile = Path.Combine(_configDirectory, "appsettings.json");

            try
            {
                if (File.Exists(configFile))
                {
                    var json = await File.ReadAllTextAsync(configFile);
                    var settings = JsonSerializer.Deserialize<CamBridgeSettings>(json, _jsonOptions);
                    return settings as T;
                }
                else
                {
                    // Return default settings if file doesn't exist
                    return GetDefaultSettings() as T;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading configuration: {ex.Message}");
                // Return default settings on error
                return GetDefaultSettings() as T;
            }
        }

        public async Task SaveConfigurationAsync<T>(T configuration) where T : class
        {
            if (configuration is not CamBridgeSettings settings)
            {
                throw new NotSupportedException($"Configuration type {typeof(T).Name} is not supported");
            }

            var configFile = Path.Combine(_configDirectory, "appsettings.json");

            try
            {
                var json = JsonSerializer.Serialize(settings, _jsonOptions);
                await File.WriteAllTextAsync(configFile, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving configuration: {ex.Message}");
                throw new InvalidOperationException("Failed to save configuration", ex);
            }
        }

        private CamBridgeSettings GetDefaultSettings()
        {
            return new CamBridgeSettings
            {
                DefaultOutputFolder = @"C:\CamBridge\Output",
                MappingConfigurationFile = "mappings.json",
                UseRicohExifReader = true,
                WatchFolders =
                {
                    new FolderConfiguration
                    {
                        Path = @"C:\CamBridge\Input",
                        Enabled = true,
                        FilePattern = "*.jpg;*.jpeg"
                    }
                },
                Processing = new ProcessingOptions
                {
                    SuccessAction = PostProcessingAction.Archive,
                    FailureAction = PostProcessingAction.MoveToError,
                    ArchiveFolder = @"C:\CamBridge\Archive",
                    ErrorFolder = @"C:\CamBridge\Errors",
                    BackupFolder = @"C:\CamBridge\Backup",
                    CreateBackup = true,
                    MaxConcurrentProcessing = 2,
                    RetryOnFailure = true,
                    MaxRetryAttempts = 3,
                    OutputOrganization = OutputOrganization.ByPatientAndDate
                },
                Dicom = new DicomSettings
                {
                    ImplementationClassUid = "1.2.276.0.7230010.3.0.3.6.4",
                    ImplementationVersionName = "CAMBRIDGE_001",
                    StationName = Environment.MachineName,
                    ValidateAfterCreation = true
                },
                Logging = new LoggingSettings
                {
                    LogLevel = "Information",
                    LogFolder = @"C:\CamBridge\Logs",
                    EnableFileLogging = true,
                    EnableEventLog = true,
                    MaxLogFileSizeMB = 10,
                    MaxLogFiles = 10
                },
                Service = new ServiceSettings
                {
                    ServiceName = "CamBridgeService",
                    DisplayName = "CamBridge JPEG to DICOM Converter",
                    Description = "Monitors folders for JPEG files from Ricoh cameras and converts them to DICOM format",
                    StartupDelaySeconds = 5,
                    FileProcessingDelayMs = 500
                },
                Notifications = new NotificationSettings
                {
                    EnableEventLog = true,
                    EnableEmail = false,
                    Email = new EmailSettings
                    {
                        SmtpPort = 587,
                        UseSsl = true
                    },
                    MinimumEmailLevel = "Warning",
                    SendDailySummary = false,
                    DailySummaryHour = 8
                }
            };
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
﻿<!-- Temporäre Debug-Version - src/CamBridge.Config/CamBridge.Config.csproj -->
<Project Sdk="Microsoft.NET.Sdk">

	<PropertyGroup>
		<OutputType>Exe</OutputType>
		<!-- Changed to Exe for console output -->
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
		<PackageReference Include="Microsoft.Extensions.Logging.Console" Version="8.0.0" />
		<PackageReference Include="ModernWpfUI" Version="0.9.6" />
		<PackageReference Include="System.ServiceProcess.ServiceController" Version="8.0.0" />
		<PackageReference Include="System.Drawing.Common" Version="8.0.10" />
		<PackageReference Include="System.Text.Json" Version="8.0.5" />
	</ItemGroup>

	<ItemGroup>
		<ProjectReference Include="..\CamBridge.Core\CamBridge.Core.csproj" />
				<ProjectReference Include="..\CamBridge.Infrastructure\CamBridge.Infrastructure.csproj" />
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
``` 
 
 
## Summary 
- Total files collected: 20 
- Focus: Settings Page DI/Navigation issues 
- Next: Mapping Editor with Drag and Drop 
- Target versions: v0.4.5 (Fix) and v0.5.0 (Config Management) 
