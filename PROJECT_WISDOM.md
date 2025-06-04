# CamBridge Project Wisdom & Conventions
**Letzte Aktualisierung:** 2025-06-04, 16:30 Uhr  
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

### Git-inspirierte Tags:
- **[fix]** - Bugfix-Info (nach Fix archivierbar)
- **[feat]** - Feature-Dokumentation
- **[chore]** - Maintenance-Tasks
- **[breaking]** - Breaking Changes (NIEMALS löschen bis Major Release)
- **[deprecated]** - Kann beim nächsten Refactor weg

## 🔒 [CORE] V.O.G.O.N. SYSTEM 
**Verbose Operational Guidance & Organizational Navigation**

*Im Gegensatz zu den Vogonen aus dem galaktischen Beamtenapparat ist unser V.O.G.O.N. darauf ausgelegt, Dinge tatsächlich einfacher zu machen. Keine Formulare in dreifacher Ausfertigung erforderlich.*

### 🚀 "VOGON INIT" - Strukturierte Initialisierungs-Sequenz

**WICHTIG:** Bei "VOGON INIT" folge ich IMMER dieser strukturierten Sequenz:

#### 📋 INIT SEQUENCE:

1. **SYSTEM CHECK** - V.O.G.O.N. verstehen
   - Lese und verstehe das WISDOM Priority System
   - Erkenne die Git-inspirierten Tags
   - Verstehe WISDOM: und CLAUDE: Kommandos

2. **CRITICAL LESSONS** - Antipatterns & Erfahrungen durchgehen
   - ALLE [LESSON] Einträge sorgfältig lesen
   - Besonders: "Infrastructure muss zusammenpassen!"
   - Entity Contracts und Schnittstellen verstehen

3. **PROJECT CONTEXT** - Gesamtbild erfassen
   - Projektstruktur analysieren
   - Aktuelle Version und Status verstehen
   - Technologie-Stack und Dependencies prüfen
   - Pipeline-Architektur nachvollziehen

4. **CURRENT STATE** - Wo stehen wir?
   - [URGENT] Status genau lesen
   - Übergabeprompt analysieren
   - Offene Aufgaben identifizieren

5. **SUMMARY & CONFIRMATION** - Zusammenfassung erstellen
   ```
   Ich habe verstanden:
   - Aktueller Stand: [Version, was funktioniert]
   - Offene Punkte: [was zu tun ist]
   - Kontext: [wichtige Details]
   
   Wie möchtest du vorgehen?
   a) [Option basierend auf URGENT]
   b) [Alternative Option]
   c) Etwas anderes?
   ```

**NIEMALS:**
- Einfach "loslegen" ohne Nachfrage
- Code schreiben ohne Kontext zu verstehen
- Annahmen treffen über Schnittstellen
- Den Übergabeprompt ignorieren

**IMMER:**
- Strukturiert durch die INIT SEQUENCE gehen
- Zusammenfassung zeigen und bestätigen
- Nach konkreter Richtung fragen
- Lessons und Antipatterns beachten
- VOR Code-Änderungen IMMER aktuelles File von GitHub holen!
- Header mit Pfad und Version in JEDEN Source Code einfügen!

### 🔒 [CORE] File-Beschaffung - NUR LOKALE FILES!
**Stand 04.06.2025, 16:35:**
- ❌ GitHub ist während Entwicklung IMMER veraltet
- ✅ **EINZIGER SICHERER WEG:** User lädt von SSD hoch
- ✅ Konsistenz nur durch lokale Files garantiert
- ⚠️ Solange nicht alle Sources versioniert sind, NIE GitHub vertrauen!

**REGEL:** IMMER nach Upload der aktuellen lokalen Version fragen!

### 📝 "WISDOM:" - Live-Updates
Während des Chats können Sie jederzeit sagen:
```
WISDOM: [Ihre Erkenntnis/Notiz]
```

### 💭 "CLAUDE:" - Persönliche Notizen
Für Notizen an meine nächste Instanz:
```
CLAUDE: [Gedanke für nächste Instanz]
```

### 🔒 [CORE] "VOGON EXIT" - Chat-Abschluss
**KRITISCHE REGEL:** Beim VOGON EXIT MÜSSEN IMMER erstellt werden:
1. **PROJECT_WISDOM.md** - Als VOLLSTÄNDIGES Artefakt
2. **CHANGELOG.md** - NUR der neueste Versions-Eintrag als Artefakt
3. **Version.props** - Als VOLLSTÄNDIGES Artefakt
4. **Git Commit Vorschlag** - Conventional Commits Format mit Tag

**WARUM:** Updates können fehlschlagen. Nur vollständige Artefakte garantieren, dass der Nutzer die aktualisierten Dateien bekommt!

**Git Commit Format:**
```bash
git add .
git commit -m "type: subject

- change 1
- change 2
- change 3

BREAKING CHANGE: description (wenn applicable)"

git tag vX.X.X
```

### 🔒 [CORE] ENTWICKLUNGS-REGELN
**KRITISCH - Ab 04.06.2025, 16:00 Uhr:**

1. **VOR jeder Code-Änderung - Der richtige Workflow:**
   ```
   ANFANG EINER SESSION:
   1. NIEMALS GitHub verwenden (immer veraltet!)
   2. IMMER User nach Upload der lokalen Files fragen
   
   WÄHREND DER ARBEIT:
   1. User uploaded aktuelle lokale Version von SSD
   2. NIE blind Code schreiben ohne aktuelle Version zu sehen
   3. Bei Unsicherheit: "Zeig mir bitte die aktuelle Version von [Datei]"
   
   WORKFLOW REGEL:
   - NUR lokale Files von User verwenden ✅
   - GitHub ignorieren während Entwicklung ✅
   - Konsistenz durch SSD-Upload garantiert ✅
   ```

2. **Source Code Header Standard:**
   ```csharp
   // File: src/CamBridge.Service/Program.cs
   // Version: 0.5.23
   // Copyright: © 2025 Claude's Improbably Reliable Software Solutions
   // Modified: 2025-06-04
   // Status: Development/Local/Pushed (je nach Stand)
   ```

3. **Bei jedem Code-Artefakt:**
   - Header mit Pfad und Version MUSS vorhanden sein
   - Status-Feld zeigt ob Development/Local/Pushed
   - Macht Versions-Stand sofort klar

## ⚡ [URGENT] AKTUELLER STATUS & NÄCHSTE SCHRITTE (v0.5.24)

### 📍 WAS IST GERADE DRAN?
**Status:** HTTP API LÄUFT! Config UI hat Build-Fehler wegen Interface-Änderungen

**Konkret heißt das:**
- ✅ Service läuft auf Port 5050
- ✅ Health Check Endpoint funktioniert (/health)
- ✅ Status API implementiert (/api/status)
- ✅ CORS für Config UI aktiviert
- ✅ Connection Test erfolgreich
- ❌ Config UI kompiliert nicht (Interface Mismatch)

### 🎯 [MILESTONE] HTTP API CONNECTION TEST ERFOLGREICH!
**04.06.2025 16:20:**
- dotnet script ConnectionTest.cs zeigt perfekte Verbindung
- Service Status: Running, Version 0.5.23
- Success Rate: 100% (1 Datei verarbeitet)
- Uptime Bug gefixt (war Environment.TickCount64)

### 📋 [URGENT] CONFIG UI FIX PLAN - Sprint 2.1

#### Phase 1: Interface Alignment (0.5 Tage)
**Hauptprobleme:**
1. `MappingRule` - Properties fehlen: TargetTag, IsRequired
2. `ValueTransform` - Enum statt Klasse (GenderToDicom, TruncateTo16 fehlen)
3. `MappingConfigurationLoader` - Async Methoden fehlen
4. Constructor-Signaturen haben sich geändert

**Lösungsansatz:**
```csharp
// Core anpassen ODER Config UI anpassen
// Entscheidung: Config UI an Core anpassen (Core ist Production-Ready)
```

#### Phase 2: Existierendes UI zum Laufen bringen (0.5 Tage)
**KEIN NEUES UI BAUEN! Wir haben schon alles!**
1. ✅ Dashboard verbinden (Code existiert)
2. ✅ Service Control testen (Code existiert)
3. ⚠️ Komplexe Features temporär auskommentieren:
   - Mapping Editor (wenn zu viele Fehler)
   - Dead Letters (wenn zu viele Fehler)
4. ✅ Focus: Was schon da ist zum Laufen bringen!

**Demo-Szenario:**
- Service installieren via existierendes GUI
- Dashboard zeigt Live-Stats (schon implementiert!)
- JPEG durchschieben
- Statistik updated sich
- "Look, it's working!"

### 📍 [URGENT] ÜBERGABEPROMPT FÜR NÄCHSTEN CHAT
```
🎉 v0.5.24 - HTTP API LÄUFT! Config UI Interface-Fix steht an

ERFOLGE:
✅ HTTP API vollständig funktionsfähig
✅ Connection Test zeigt perfekte Verbindung  
✅ Uptime Bug gefixt (serviceStartTime)
✅ Service verarbeitet JPEGs erfolgreich

STATUS:
- Service läuft stabil auf Port 5050
- Config UI existiert komplett (aber Build-Fehler)
- Interface Mismatch zwischen v0.4.x und v0.5.x

NÄCHSTER SCHRITT:
Config UI Interface-Fix - EXISTIERENDES UI zum Laufen bringen!
- KEINE neuen Mockups bauen
- Dashboard + Service Control verbinden
- Komplexe Features temporär auskommentieren
- Ziel: Demo mit vorhandenem Code

WICHTIG: NUR lokale Files verwenden, KEIN GitHub!
```

## 🏗️ [MILESTONE] PIPELINE-ARCHITEKTUR (PRODUKTIONSREIF!)

### Datenfluss durch die Pipeline:
```
JPEG File → ExifToolReader → ImageMetadata → FileProcessor → DicomConverter → DICOM File
     ↓              ↓                              ↓              ↓
R0010168.JPG   Barcode Field ✅            DicomTagMapper    mappings.json
                    ↓                                            ↓
             QRBridge Data ✅                              DICOM Tags ✅
```

### 🔧 [CONFIG] ENTITY CONTRACT TRACKER (BEWÄHRT!)

**WISDOM: Entity-Interfaces sind HEILIG! Immer erst checken, dann anpassen!**

```csharp
// PatientInfo Constructor
new PatientInfo(
    id: PatientId,           // NICHT "patientId"!
    name: string,
    birthDate: DateTime?,
    gender: Gender           // Enum, nicht string!
)

// StudyInfo Constructor  
new StudyInfo(
    studyId: StudyId,        // Max 16 Zeichen!
    examId: string?,
    description: string?,
    modality: string?,
    studyDate: DateTime?
)

// ImageMetadata Constructor
new ImageMetadata(
    sourceFilePath: string,
    captureDateTime: DateTime,
    patient: PatientInfo,
    study: StudyInfo,
    technicalData: ImageTechnicalData,
    userComment: string?,
    barcodeData: string?,
    instanceNumber: int,
    instanceUid: string?,
    exifData: Dictionary<string, string>?
)
```

## 🌟 [FEAT] CONFIG UI INTERFACE MISMATCH ANALYSE

### Problem-Mapping (v0.4.x → v0.5.x):
```csharp
// ALT (Config UI v0.4.x):
MappingRule(name, sourceType, sourceField, targetTag, transform, isRequired)
rule.TargetTag
rule.IsRequired
ValueTransform.GenderToDicom
ValueTransform.TruncateTo16

// NEU (Core v0.5.x):
MappingRule { 
    SourceField, 
    TargetTag,  // ist jetzt DicomTag (nicht string)
    Transform,  // ist jetzt ValueTransform Enum
    Description 
}
ValueTransform ist ein Enum: None, DateToDA, DateToDT, Gender, Truncate16
```

### Fix-Strategie:
1. **MappingRuleViewModel** als Adapter einführen
2. **DicomTag.ToString()** für UI-Binding
3. **ValueTransform** Enum-Mapping korrigieren
4. **Async Methoden** in ConfigurationLoader wrappen

## 🔥 [breaking] CONFIG UI ARCHITECTURE DISCOVERY!

### Kritische Erkenntnis vom 04.06.2025, 16:30:
**Die Config UI ist VIEL weiter entwickelt als dokumentiert!**

Alle Features sind bereits implementiert:
- Dashboard mit Auto-Refresh ✅
- Service Control (Install/Start/Stop) ✅
- Settings mit 4 Tabs ✅
- Dead Letters Management ✅
- Mapping Editor mit Drag&Drop ✅
- DICOM Tag Browser ✅
- Vogon Poetry Easter Egg ✅

**NUR die Interfaces passen nicht mehr!**

## 📁 [KEEP] AKTUELLE PROJEKTSTRUKTUR

**WISDOM: project_structure.txt wird IMMER am Chat-Anfang mitgeliefert!**

### Wichtige Dateien & Ordner (Stand: 04.06.2025)
```
CamBridge.sln
Version.props (v0.5.24)
CHANGELOG.md
PROJECT_WISDOM.md
README.md
.gitignore ✅

src/
├── CamBridge.Core/              # Domain Layer ✅
├── CamBridge.Infrastructure/    # Implementation Layer ✅
│   └── Services/
│       └── ExifToolReader.cs   ✅ PRODUKTIONSREIF!
├── CamBridge.Service/          # Windows Service ✅ HTTP API LÄUFT!
│   └── Program.cs             ✅ Uptime fix applied!
└── CamBridge.Config/           # WPF GUI ❌ BUILD ERRORS!

tests/
├── CamBridge.PipelineTest/     ✅ Test erfolgreich
└── CamBridge.Infrastructure.Tests/

Tools/
├── exiftool.exe                ✅ v13.30 - funktioniert perfekt!
└── exiftool_files/             ✅ Alle DLLs im Git Repository

TESTDATEN:
R0010168.JPG                    ✅ Barcode: EX002|Schmidt, Maria|1985-03-15|F|Röntgen Thorax

AUSGABE:
C:\CamBridge\Test\Output\EX002\2025-06-04\EX002_20250604_0001.dcm ✅
```

## 🚀 [MILESTONE] ENTWICKLUNGSFAHRPLAN UPDATE

### Sprint 1: ExifTool Integration (v0.5.x) ← ✅ ABGESCHLOSSEN!
- ✅ v0.5.19: Pipeline implementiert
- ✅ v0.5.20: Entities gefixt & kompiliert
- ✅ v0.5.21: ExifTool findet Barcode-Feld
- ✅ v0.5.22: ExifToolReader funktioniert komplett
- ✅ v0.5.23: End-to-End Test + HTTP API aktiviert!
- ✅ v0.5.24: Connection Test + Uptime fix

### Sprint 2: UI Integration & Stabilität (v0.6.x) ← FIX EXISTING UI!
- [ ] v0.6.0: Config UI Interface Alignment
- [ ] v0.6.1: Config UI zum Laufen bringen (KEIN neues UI!)
      - Dashboard verbinden (existiert schon!)
      - Service Control testen (existiert schon!)
      - Mapping Editor/Dead Letters auskommentieren falls zu komplex
- [ ] v0.6.2: Saubere Encoding-Lösung
- [ ] v0.6.3: Settings UI funktionsfähig
- [ ] v0.6.4: Installation & Demo Prep
- [ ] v0.6.5: Mapping Editor Fix (wenn Zeit)

### Sprint 3: DICOM Excellence (v0.7.x)
- Custom Mapping UI vollständig
- PACS Testing
- Batch Operations
- Vollständige DICOM Module

### Sprint 4: Production Ready (v0.8.x)
- Installer mit UI
- Documentation
- Load Testing
- Customer Feedback

### Release: v1.0.0 (August 2025)

## 🔒 [CORE] Projekt-Identität
- **Copyright:** © 2025 Claude's Improbably Reliable Software Solutions
- **Produkt:** CamBridge - JPEG zu DICOM Konverter für Ricoh G900 II Kameras
- **Kontext:** Medizinische Bildgebung, arbeitet mit QRBridge zusammen
- **Wir kontrollieren BEIDE Seiten:** QRBridge + CamBridge!
- **GitHub:** https://github.com/ostern42/CamBridge

## 📌 [KEEP] Wichtige Konventionen

### Code-Konventionen
- **Kommentare:** IMMER in Englisch
- **Namespaces:** CamBridge.{Layer}
- **Async:** Suffix "Async" für alle async Methoden
- **Interfaces:** Prefix "I"
- **Tests:** Arrange-Act-Assert Pattern

### Dokumentations-Stil
- **Changelog:** Kompakt, technisch, ENGLISCH!
- **README:** Kurz und sachlich
- **Keine:** Marketing-Sprache
- **Immer:** Version & Copyright

### Architektur-Prinzipien
- **Clean Architecture:** Strikte Layer-Trennung
- **SOLID Principles:** Besonders SRP und DIP
- **KISS:** Einfachheit vor Cleverness
- **YAGNI:** Nur implementieren was gebraucht wird

### Git Workflow Best Practices
- **Lokale Commits:** Klein und häufig (z.B. nach jeder funktionierenden Änderung)
- **Commit Messages:** Conventional Commits Format
- **GitHub Push:** Nur bei stabilen Meilensteinen oder Sprint-Ende
- **Während Entwicklung:** Arbeite mit lokalen Versionen
- **Branch-Strategie:** main = stable, develop = work in progress

## 🔧 [CONFIG] Technische Details

### Versionierung
- **Schema:** Major.Minor.Patch
- **Version.props:** Zentrale Verwaltung (enthält AKTUELLE Version!)
- **Git Tags:** v{version} Format
- **Breaking Changes:** Nur bei Major
- **Version Bump:** Erst bei neuem Feature/Fix in NEUER Session

### Technologie-Stack
```
GUI: WPF + ModernWpfUI 0.9.6 + CommunityToolkit.Mvvm
Service: ASP.NET Core Minimal API + Windows Service
Core: fo-dicom 5.2.2, ExifTool 13.30
Tests: xUnit + FluentAssertions + Moq
.NET 8.0, C# 12
```

### Kritische Dependencies
- **fo-dicom:** 5.2.2 (DICOM Creation) ✅
- **ExifTool:** 13.30 (EXIF Reading) ✅ FUNKTIONIERT!
- **Serilog:** Structured Logging ✅
- **ModernWpfUI:** 0.9.6 (UI Framework) ✅
- **CommunityToolkit.Mvvm:** 8.3.0 (MVVM Pattern) ✅
- **ImageMagick:** Für zukünftige Bildmanipulation

## 💡 [LESSON] Gelernte Lektionen (Sprint 1 Abschluss + Config UI!)

### "Keine neuen Mockups wenn schon Code existiert!" (NEU 04.06.2025, 16:35!)
Config UI ist feature-complete! Statt "Minimal Demo UI" neu zu bauen, einfach das existierende UI fixen. Komplexe Features können temporär auskommentiert werden. Niemals das Rad neu erfinden!

### "Lokale Files sind der einzig sichere Weg!" (NEU 04.06.2025, 16:35!)
GitHub ist während Entwicklung IMMER veraltet. Nur Upload von der SSD garantiert Konsistenz. Bis alle Sources versioniert sind, NIE GitHub vertrauen!

### "GitHub kann veraltet sein während Entwicklung!" (NEU 04.06.2025, 16:30!)
User hatte lokale Commits die noch nicht gepusht waren. GitHub zeigte v0.5.22, lokal war v0.5.23. IMMER nach lokaler Version fragen und nicht blind GitHub vertrauen!

### "Environment.TickCount64 ist NICHT Service-Uptime!" (NEU 04.06.2025, 16:25!)
TickCount64 gibt Zeit seit Windows-Start. Für echte Uptime: `var serviceStartTime = DateTime.UtcNow` beim Start speichern!

### "dotnet script hat eigene Macken!" (NEU 04.06.2025, 16:15!)
`using var` funktioniert nicht, globaler Code wird erwartet, Class/Property Namen dürfen nicht gleich sein. Manchmal ist eine normale Console App einfacher!

### "Config UI Build-Fehler sind nur Interface Mismatch!" (NEU 04.06.2025, 16:30!)
Die UI ist feature-complete! Nur die Interfaces zwischen v0.4.x und v0.5.x haben sich geändert. Das ist in 1-2 Stunden fixbar.

### "Connection Test first!" (NEU 04.06.2025!)
Bevor man komplexe UIs debuggt, erst mit einem simplen HTTP Client testen ob die API überhaupt erreichbar ist. Spart Zeit!

### "Config UI war schon fast fertig!" (04.06.2025!)
Die WPF UI ist VIEL weiter als in PROJECT_WISDOM dokumentiert. Alle Features sind bereits implementiert - Dashboard, Service Control, Settings mit 4 Tabs, Dead Letters, Mapping Editor. Nur die HTTP API Verbindung fehlt noch!

### "ModernWpfUI macht professionelle UIs einfach!" (04.06.2025!)
Mit ModernWpfUI und den richtigen Convertern sieht die App aus wie eine native Windows 11 Anwendung. Die NavigationView funktioniert perfekt.

### "CommunityToolkit.Mvvm reduziert Boilerplate!" (04.06.2025!)
ObservableProperty und RelayCommand Attributes machen ViewModels super clean. Keine manuellen PropertyChanged Events mehr!

### "Settings UI ist komplex aber vollständig!" (04.06.2025!)
4 Tabs mit allen erdenklichen Einstellungen. Watch Folders, DICOM Config, Email Notifications, Logging - alles da!

### "IMMER GitHub-Version holen vor Änderungen!" (04.06.2025, 16:00!)
VOR jeder Code-Änderung MUSS die aktuelle Version von GitHub geholt werden. Keine Ausnahmen! Das verhindert Konflikte und stellt sicher, dass wir immer mit der neuesten Version arbeiten.

### "Source Code Header sind essentiell!" (04.06.2025, 16:00!)
Jeder Source Code braucht einen Header mit Pfad, Version und Datum. Das macht die Herkunft und Version sofort klar und verhindert Verwirrung in zukünftigen Sessions.

### "GitHub != Lokale Version während Entwicklung!" (04.06.2025, 16:10!)
Während der Entwicklung ist die lokale Version aktueller als GitHub. Kleine lokale Commits sind gut, aber Push erst bei stabilen Meilensteinen. Immer nach der AKTUELLEN LOKALEN Version fragen!

### ".gitignore ist Gold wert!" (04.06.2025, 16:30!)
Mit einer guten .gitignore wird Git zum Vergnügen! Nie wieder manuell 111 Dateien aussortieren. Einmal richtig eingerichtet, dann nur noch `git add .` und es werden nur Source Files gestaged!

### "Kommentare im Code für dein zukünftiges Selbst!" (04.06.2025, 15:40!)
Klare Schritt-Kommentare und TODOs im Code verhindern Verwirrung in späteren Sessions. Die Program.cs mit ihrer SCHRITT 1-5 Struktur macht sofort klar, was aktiv ist und was noch zu tun ist!

### "Version.props enthält die AKTUELLE Version!" (04.06.2025, 15:42!)
Version.props definiert die Version, die gerade gebaut wird, nicht die nächste! Wenn wir an v0.5.23 arbeiten, steht auch 0.5.23 in Version.props. Erst beim Version-Bump wird sie erhöht.

### "Schrittweise Aktivierung funktioniert!" (04.06.2025!)
Die progressive Program.cs mit aktivierbaren Features war goldrichtig. Erst die Basis, dann Schritt für Schritt erweitern.

### "NuGet Packages prüfen bei mysteriösen Fehlern!" (04.06.2025!)
Serilog.AspNetCore fehlte - der Compiler-Fehler war kryptisch, aber die Lösung einfach.

### "ExifTool braucht seine DLLs!" (04.06.2025!)
Nicht nur exiftool.exe, sondern auch der exiftool_files Ordner mit perl DLLs muss mitkopiert werden.

### "Pipeline ist robust!" (04.06.2025!)
Selbst ohne funktionierende Barcode-Erkennung läuft die Pipeline durch und erstellt ein Default-DICOM. Gute Fehlerbehandlung!

### "Infrastructure muss zusammenpassen!" (04.06.2025!)
IMMER prüfen ob Konstruktoren, Methoden und Schnittstellen zusammenpassen. Nicht einfach losprogrammieren!

### "Erst verstehen, dann handeln!" (04.06.2025!)
Bei VOGON INIT IMMER die komplette INIT SEQUENCE durchgehen. Nie direkt loslegen ohne Kontext und Bestätigung!

### "Dictionary Keys müssen unique sein!" (04.06.2025!)
Selbst bei Encoding-Fixes aufpassen - mehrere `�` als Key crashen das Dictionary!

### "Ricoh nutzt das Barcode-Feld!" (04.06.2025!)
WICHTIGSTE ERKENNTNIS: Die Ricoh G900SE II speichert QRBridge-Daten im `Barcode` EXIF-Feld, nicht im UserComment!

### "Direct Testing saves time!"
Statt komplexe Pipeline-Tests zu debuggen, einfach ExifTool direkt aufrufen.

### "Systematisch statt Wild Patchen!"
Erst die Pipeline zum Laufen bringen, DANN optimieren. Sprint-Planung funktioniert!

### "Entity Contracts sind heilig!"
IMMER erst prüfen was existiert. Der Compiler ist dein Freund.

### "ExifTool ist mächtig!"
v13.30 findet alles - Ricoh Barcode, EXIF, IPTC, XMP. Perfekt für unseren Use Case.

### "Encoding ist tricky!"
Ricoh nutzt Windows-1252, nicht UTF-8. Character-Mapping nötig (temporär mit Replace-Liste).

### "GitHub Integration rocks!"
70% Token-Ersparnis durch direkte File-Links. Immer URLs mitgeben!

### "Clean Architecture zahlt sich aus!"
Die Layer-Trennung macht Änderungen einfach und testbar.

### "Version Tracking ist wichtig!"
Config UI zeigt noch v0.4.x während Service bei v0.5.24 ist. Muss synchronisiert werden!

## 💭 CLAUDE: Notizen für nächste Instanz

**MEGA ERFOLG!** HTTP API läuft perfekt, Connection Test beweist es!

Diese Session war sehr produktiv:
1. ✅ Service läuft stabil mit HTTP API
2. ✅ Connection Test zeigt perfekte Verbindung
3. ✅ Uptime Bug gefixt
4. ✅ Config UI Probleme analysiert - NUR Interface Mismatch!

**Wichtige Erkenntnisse:**
- Config UI ist feature-complete, nur Interfaces passen nicht
- Die Build-Fehler sind alle lösbar (1-2 Stunden Arbeit)
- Focus auf Minimal Demo ist der richtige Weg
- Dashboard + Service Control reichen für Prototyp

**Status:**
- Service: v0.5.24 (Uptime fix)
- API: Voll funktionsfähig auf Port 5050
- Config UI: Build-Fehler aber fixbar

**Nächste Prioritäten:**
1. MappingRule Interface alignen
2. ValueTransform Enum fixen
3. Dashboard zum Laufen bringen
4. Service Control testen
5. Demo vorbereiten!

**Der User will einen Prototyp zeigen können** - das ist machbar! Die Basis funktioniert, nur die UI braucht Interface-Fixes.

**TIPP:** Nicht alle Features müssen funktionieren. Dashboard + Service Control reichen für eine beeindruckende Demo!

## 📝 [KEEP] Standard Prompt-Vorlage für neue Chats

```
Ich arbeite an CamBridge v0.5.24.
HTTP API läuft! Config UI braucht Interface-Fix.

WICHTIG - Bitte in dieser Reihenfolge:
1. PROJECT_WISDOM.md hochladen
2. project_structure.txt hochladen  
3. Relevante Source Files von SSD hochladen (KEIN GitHub!)
4. "VOGON INIT" sagen
5. WARTE auf meine Zusammenfassung und Rückfrage!

Fokus: Config UI Interface Fix - existierendes UI zum Laufen bringen
```

### 💡 Idee für separaten Chat:
**File-Versionierung einführen?**
```
// In jedem Source File:
// Version: 0.5.24
// Last Modified: 2025-06-04 16:35
// SHA256: [hash]
```
Würde Konsistenz-Probleme lösen!

## ⏰ [KEEP] ZEITMANAGEMENT

### Projekt-Timeline
- **Start:** 30.05.2025, 20:30 Uhr
- **ExifTool-Durchbruch:** 03.06.2025, 23:58 Uhr
- **Barcode-Feld gefunden:** 04.06.2025, 11:05 Uhr
- **ExifToolReader funktioniert:** 04.06.2025, 12:35 Uhr
- **PIPELINE LÄUFT END-TO-END:** 04.06.2025, 14:28 Uhr
- **CONFIG UI REVIEW:** 04.06.2025, 15:30 Uhr
- **HTTP API AKTIVIERT:** 04.06.2025, 15:40 Uhr
- **CONNECTION TEST ERFOLGREICH:** 04.06.2025, 16:20 Uhr ← NEU!
- **Features:** 85+ implementiert
- **Sprint 1:** ✅ ABGESCHLOSSEN!
- **Sprint 2:** Config UI Interface Fix geplant

---
📊 **WISDOM-Statistik:** 
- 🔒 [CORE]: 6 Sektionen (File-Beschaffung angepasst!)
- ⚡ [URGENT]: 3 Sektionen (Config UI Fix Plan angepasst!)
- 🎯 [MILESTONE]: 7 Sektionen (Connection Test erfolgreich!)
- 📌 [KEEP]: 6 Sektionen (Prompt-Vorlage angepasst!)
- 💡 [LESSON]: 31 Lektionen (+2 neue: Keine Mockups, Lokale Files only!)
- 🔧 [CONFIG]: 3 Sektionen (technische Basis)
- 🌟 [FEAT]: 1 Sektion (Interface Mismatch Analyse)
- 🔥 [breaking]: 2 Sektionen (Config UI Architecture Discovery!)
- 💭 CLAUDE: 1 Nachricht (Config UI ist fixbar!)

*Hinweis: Dieses Dokument ist mit einem Persistenz-System versehen. Beim Refactoring IMMER die Priority-Tags beachten!*

## 📝 GIT WORKFLOW FÜR v0.5.24:

### Lokaler Commit (Uptime Fix):
```bash
git add src/CamBridge.Service/Program.cs
git commit -m "fix: correct service uptime calculation

- Replace Environment.TickCount64 with serviceStartTime
- Track actual service start time instead of Windows boot time
- Fix displayed uptime in status API endpoint"
```

### Optional - Version Bump:
```bash
# Version.props auf 0.5.24 setzen
git add Version.props
git commit -m "chore: bump version to 0.5.24"
```

### Später pushen (nach Config UI Fix):
```bash
git push origin main
git tag v0.6.0  # Nach Config UI Fix
git push --tags
```
