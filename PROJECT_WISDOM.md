# CamBridge Project Wisdom & Conventions
**Letzte Aktualisierung:** 2025-06-03, 23:58 Uhr  
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

### 🚀 "VOGON INIT" - Automatischer Start
Wenn Sie nur "VOGON INIT" sagen, werde ich:
1. PROJECT_WISDOM.md aus den hochgeladenen Dateien lesen
2. Den aktuellen Übergabeprompt daraus extrahieren
3. Die Aufgabe verstehen und direkt loslegen
4. Keine weiteren Erklärungen nötig!

### 🔒 [CORE] GitHub Integration - FUNKTIONIERT!
**Stand 02.06.2025:**
- ✅ Repository public unter: https://github.com/ostern42/CamBridge
- ✅ Direkte File-Links funktionieren mit web_fetch
- ✅ 70% Token-Ersparnis möglich
- ✅ URLs müssen EXPLIZIT vom Nutzer bereitgestellt werden

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

## ⚡ [URGENT] AKTUELLER STATUS & NÄCHSTE SCHRITTE (v0.5.21)

### 📍 WAS IST GERADE DRAN?
**Status:** ExifTool Pipeline KOMPILIERT und ExifTool FINDET Barcode-Daten!

**Konkret heißt das:**
- ✅ Alle Entity-Probleme behoben (v0.5.20)
- ✅ Infrastructure kompiliert erfolgreich
- ✅ ExifTool v13.30 funktioniert
- ✅ Barcode-Feld gefunden: `EX002|Schmidt, Maria|1985-03-15|F|R÷ntgenáThorax`
- ❌ Volle Pipeline noch nicht getestet!

### 🎯 [MILESTONE] ERFOLG: ExifTool Integration funktioniert!
**03.06.2025 23:58:**
- ExifTool findet das Barcode-Feld in Ricoh G900 II Bildern
- QRBridge-Daten erfolgreich im EXIF gefunden
- Encoding-Probleme wie erwartet (÷ = ö)
- Infrastructure kompiliert ohne Fehler

### 📋 [URGENT] NÄCHSTE SCHRITTE

#### SCHRITT 1: Vollständiger Pipeline-Test (30 Min)
```plaintext
1. TestConsole reparieren oder neues Test-Projekt
2. Mit R0010168.JPG die volle Pipeline testen
3. DICOM-Datei erstellen
4. Validierung prüfen
```

#### SCHRITT 2: Encoding-Fix implementieren (30 Min)
- CleanBarcodeData() in ExifToolReader verbessern
- UTF-8/Latin-1 Konfusion beheben
- Tests mit verschiedenen Umlauten

#### SCHRITT 3: Edge Cases testen (1 Std)
- Bilder ohne QR-Code
- Unvollständige QRBridge-Daten (nur 3 Felder)
- Verschiedene QRBridge-Formate

### 📍 [URGENT] ÜBERGABEPROMPT FÜR NÄCHSTEN CHAT
```
🔧 v0.5.21 - ExifTool funktioniert! Pipeline muss getestet werden!

STATUS:
✅ Infrastructure kompiliert
✅ ExifTool findet Barcode-Daten
✅ Test-Bild: R0010168.JPG vorhanden
❌ Volle Pipeline noch nicht getestet

TESTDATEN:
Barcode: EX002|Schmidt, Maria|1985-03-15|F|R÷ntgenáThorax

AUFGABE:
1. Vollständigen Pipeline-Test mit R0010168.JPG
2. DICOM erstellen und validieren
3. Encoding-Probleme fixen (÷→ö)

KEINE NEUEN FEATURES! NUR PIPELINE TESTEN!
```

## 🏗️ [MILESTONE] PIPELINE-ARCHITEKTUR (BEWÄHRT!)

### Datenfluss durch die Pipeline:
```
JPEG File → ExifToolReader → ImageMetadata → FileProcessor → DicomConverter → DICOM File
              ↓                                     ↓              ↓
         Raw EXIF Data                    DicomTagMapper    DicomTagMapper
              ↓                                                   ↓
         QRBridge Parser                                   mappings.json
```

### 🔧 [CONFIG] ENTITY CONTRACT TRACKER (NEU 03.06.2025!)

**WISDOM: Entity-Interfaces sind HEILIG! Immer erst checken, dann anpassen!**

```csharp
// PatientInfo Constructor
new PatientInfo(
    id: PatientId,           // NICHT "patientId"!
    name: string,
    birthDate: DateTime?,
    gender: Gender           // Enum, nicht string!
)

// StudyInfo Properties
StudyInfo.StudyId        // NICHT "Id"!
StudyInfo.Description    // NICHT "StudyDescription"!
StudyInfo.ExamId         // Neu in v0.5.20

// ImageMetadata MUSS HABEN:
- ExifData: Dictionary<string, string>
- InstanceNumber: int
- InstanceUid: string
- TechnicalData: ImageTechnicalData
```

## 📁 [KEEP] AKTUELLE PROJEKTSTRUKTUR

**WISDOM: project_structure.txt wird IMMER am Chat-Anfang mitgeliefert!**

### Wichtige Dateien & Ordner (Stand: 03.06.2025)
```
CamBridge.sln
Version.props (v0.5.21)
CHANGELOG.md
PROJECT_WISDOM.md
README.md

src/
├── CamBridge.Core/              # Domain Layer ✅
│   ├── Entities/               
│   │   ├── ImageMetadata.cs    ✅ v0.5.20
│   │   ├── ImageTechnicalData.cs ✅ v0.5.20
│   │   ├── PatientInfo.cs      ✅
│   │   ├── ProcessingResult.cs ✅
│   │   └── StudyInfo.cs        ✅ v0.5.20
│   └── Interfaces/             ✅
│
├── CamBridge.Infrastructure/    # Implementation Layer ✅
│   ├── Services/               
│   │   ├── ExifToolReader.cs   ✅ v0.5.20
│   │   ├── FileProcessor.cs    ✅ v0.5.20
│   │   ├── DicomConverter.cs   ✅ v0.5.20
│   │   └── ... (alle anderen)  ✅
│   └── ServiceCollectionExtensions.cs ✅
│
├── CamBridge.Service/           # Windows Service (Fehler ignorierbar)
└── CamBridge.Config/            # WPF GUI (Fehler ignorierbar)

tests/
├── CamBridge.TestConsole/      ❌ Hat noch alte API-Calls
└── ExifToolQuickTest/          ✅ NEU! Funktioniert!

Tools/
├── exiftool.exe                ✅ v13.30
└── exiftool_files/perl.exe     ✅

TESTDATEN:
R0010168.JPG                    ✅ Ricoh-Bild mit QRBridge-Daten
```

## 🚀 [MILESTONE] ENTWICKLUNGSFAHRPLAN

### Sprint 1: ExifTool Integration (v0.5.x) ← 90% FERTIG!
- ✅ v0.5.19: Pipeline implementiert
- ✅ v0.5.20: Entities gefixt & kompiliert
- ✅ ExifTool findet Barcode-Daten
- [ ] Volle Pipeline Test
- [ ] Encoding-Fix
- [ ] Edge Cases & Stabilisierung

### Sprint 2: Mapping Engine (v0.6.x)
**Nach Sprint 1! Nicht vorher anfangen!**
- Transform Functions erweitern
- UI Integration für Mapping Editor
- Mapping Validation

### Sprint 3: DICOM Creation (v0.7.x)
- DICOM Module korrekt befüllen
- PACS Kompatibilität testen
- Batch Processing

### Sprint 4: Production Ready (v0.8.x)
- Logging & Monitoring verbessern
- Email Notifications
- Installer
- Load Testing

### Release: v1.0.0 (Ziel: Ende Juli 2025)

## 🔒 [CORE] Projekt-Identität
- **Copyright:** © 2025 Claude's Improbably Reliable Software Solutions
- **Produkt:** CamBridge - JPEG zu DICOM Konverter für Ricoh G900 II Kameras
- **Kontext:** Medizinische Bildgebung, arbeitet mit QRBridge zusammen
- **NEU:** Wir kontrollieren BEIDE Seiten (QRBridge + CamBridge)!
- **GitHub:** https://github.com/ostern42/CamBridge

## 🔥 [breaking] QRBridge Integration

### Barcode-Feld erfolgreich gefunden!
**03.06.2025:** ExifTool findet das Ricoh-spezifische Barcode-Feld:
```
Barcode: EX002|Schmidt, Maria|1985-03-15|F|R÷ntgenáThorax
UserComment: GCM_TAG
```

### QRBridge Protokoll bestätigt:
- **Pipe-delimited Format** funktioniert
- **Alle 5 Felder** im Barcode gespeichert
- **Encoding-Probleme** müssen gefixt werden

## 📌 [KEEP] Wichtige Konventionen

### Code-Konventionen
- **Kommentare:** IMMER in Englisch
- **Namespaces:** CamBridge.{Layer} (Core, Infrastructure, Service, Config)
- **Async:** Suffix "Async" für alle async Methoden
- **Interfaces:** Prefix "I" (IRepository, IService)

### Dokumentations-Stil
- **Changelog:** Kompakt, technisch, keine Marketing-Sprache, IMMER IN ENGLISCH!
- **README:** Kurz und sachlich, technisch prägnant
- **Keine:** Ausufernde Feature-Listen oder Pseudo-Medicine-Speak
- **Immer:** Versionsnummer und Copyright in Dokumenten

### Architektur-Prinzipien
- **Clean Architecture:** Strikte Layer-Trennung
- **MVVM für GUI:** Mit CommunityToolkit.Mvvm
- **DI überall:** Constructor Injection bevorzugt
- **Async/Await:** Für alle I/O-Operationen
- **KISS:** Keep It Simple, keine Over-Engineering

## 🔧 [CONFIG] Technische Details

### Versionierung
- **Schema:** Major.Minor.Patch (1.0.0)
- **Version.props:** Zentrale Versionsverwaltung
- **Git Tags:** v{version} Format

### Technologie-Stack
```
GUI: WPF + ModernWpfUI 0.9.6 + CommunityToolkit.Mvvm
Service: ASP.NET Core Minimal API + Windows Service
Processing: fo-dicom, ExifTool 13.30
.NET 8.0, C# 12
```

### [CORE] Kritische Erkenntnisse
- **GCM_TAG:** Marker in UserComment
- **Barcode:** Enthält alle QRBridge-Daten
- **Ricoh G900 II:** Speichert erfolgreich alle 5 Felder
- **ExifTool:** v13.30 funktioniert perfekt

## 🚀 [KEEP] Entwicklungs-Workflow

### Ein Feature = Eine Version = Ein Chat
1. **Implementieren → Testen → Debuggen → Commit**
2. **KEINE neuen Features bevor das aktuelle läuft**
3. **Console Mode vor Service Mode**
4. **Systematisch vorgehen**

## 💡 [LESSON] Gelernte Lektionen (03.06.2025)

### "Git Commits bei VOGON EXIT nicht vergessen!" (NEU!)
IMMER einen Git Commit Vorschlag mit Conventional Commits Format machen. Tag nicht vergessen!

### "Entity Contracts sind heilig!" (NEU!)
IMMER erst die existierenden Entities prüfen bevor neue Versionen erstellt werden. Der Compiler zeigt was wirklich existiert!

### "ExifTool findet alles!" (NEU!)
Das Barcode-Feld wird von ExifTool v13.30 erfolgreich gefunden. Ricoh G900 II speichert tatsächlich alle QRBridge-Daten!

### "CHANGELOG immer in Englisch!"
Alle CHANGELOG-Einträge müssen in Englisch geschrieben werden - keine Ausnahmen!

### "Pipeline-Architektur verstehen!"
Die neue Pipeline arbeitet OHNE IExifReader Interface! ExifToolReader liefert direkt ImageMetadata.

### "GitHub URLs funktionieren!"
Mit expliziten URLs vom Nutzer können wir Dateien direkt laden - 70% Token-Ersparnis!

### "Systematisch statt Patches!"
Wir patchen nicht mehr wild herum - die neue Sprint-Planung fokussiert auf systematisches Vorgehen.

### "Bei 0.5.x bleiben!"
Der Nutzer denkt systematisch - erst stabilisieren, dann neue Features.

## 💡 [LESSON] META-PROZESS-OPTIMIERUNGEN

### Warum Entity-Chaos entstand:
1. **Blind neue Versionen erstellt** → Jetzt IMMER erst prüfen!
2. **Annahmen über Properties** → Compiler-Fehler zeigen Wahrheit
3. **Fehlende Übersicht** → Entity Contract Tracker hilft

### CLAUDE: SELBST-INSTRUKTION für nächsten Chat:
**Du hast jetzt ein funktionierendes System!**
- ExifTool findet die Daten ✅
- Pipeline kompiliert ✅
- Testbild vorhanden ✅

**FOKUS:** NUR die volle Pipeline testen! Keine neuen Features, keine Refactorings - einfach nur R0010168.JPG durch die Pipeline jagen und schauen ob ein DICOM rauskommt!

## 📝 [KEEP] Standard Prompt-Vorlage für neue Chats

```
Ich arbeite an CamBridge v0.5.21.
STATUS: ExifTool findet Barcode! Pipeline kompiliert!

TESTDATEN:
- R0010168.JPG mit Barcode: EX002|Schmidt, Maria|1985-03-15|F|R÷ntgenáThorax

GitHub: https://github.com/ostern42/CamBridge

1. PROJECT_WISDOM.md hochladen
2. project_structure.txt hochladen
3. "VOGON INIT" sagen

Fokus: Volle Pipeline mit R0010168.JPG testen!
```

## ⏰ [KEEP] ZEITMANAGEMENT

### Projekt-Timeline
- **Entwicklungsstart:** 30.05.2025, 20:30:44 Uhr
- **Letzte Aktualisierung:** 03.06.2025, 23:58 Uhr
- **Durchbruch:** ExifTool funktioniert!
- **Features implementiert:** 60+
- **Features getestet:** ~30%

---
📊 **WISDOM-Statistik:** 
- 🔒 [CORE]: 5 Sektionen
- ⚡ [URGENT]: 3 Sektionen  
- 🎯 [MILESTONE]: 3 Sektionen
- 📌 [KEEP]: 6 Sektionen
- 💡 [LESSON]: 9 Lektionen
- 🔧 [CONFIG]: 3 Sektionen
- 🔥 [breaking]: 1 Sektion

*Hinweis: Dieses Dokument ist mit einem Persistenz-System versehen. Beim Refactoring IMMER die Priority-Tags beachten!*

## 📝 GIT COMMIT FÜR v0.5.20:

```bash
git add .
git commit -m "fix: resolve entity contract mismatches and enable compilation

- Fix PatientInfo constructor parameter naming (id vs patientId)
- Fix StudyInfo property references (StudyId, Description)
- Add missing ImageMetadata properties (ExifData, InstanceNumber, InstanceUid)
- Create ImageTechnicalData entity
- Fix Gender enum/string conversions
- Add null checks in DicomConverter
- Confirm ExifTool v13.30 finds Ricoh Barcode field

BREAKING CHANGE: Entity constructors and properties have changed"

git tag v0.5.20
```
