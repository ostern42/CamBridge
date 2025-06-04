# CamBridge Project Wisdom & Conventions
**Letzte Aktualisierung:** 2025-06-04, 11:15 Uhr  
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
**Status:** DURCHBRUCH! Barcode-Feld gefunden! ExifToolReader muss nur noch angepasst werden!

**Konkret heißt das:**
- ✅ ExifTool Test erfolgreich ausgeführt
- ✅ QRBridge-Daten im **Barcode** Feld gefunden (nicht UserComment!)
- ✅ Format bestätigt: `EX002|Schmidt, Maria|1985-03-15|F|R�ntgen�Thorax`
- ✅ Encoding-Problem sichtbar (� statt ö/ä)
- ⏳ ExifToolReader muss Barcode-Feld lesen

### 🎯 [MILESTONE] ERFOLG: Barcode-Feld identifiziert!
**04.06.2025 11:15:**
- Ricoh G900SE II speichert QRBridge-Daten im **Barcode** EXIF-Feld
- UserComment enthält nur "GCM_TAG" (Marker)
- Alle 5 Felder erfolgreich übertragen
- Encoding-Probleme wie erwartet

### 📋 [URGENT] NÄCHSTE SCHRITTE

#### SCHRITT 1: ExifToolReader anpassen (15 Min)
- Barcode-Feld statt UserComment lesen
- Kompletter Code im Artefakt "ExifToolReader.cs - Complete with Barcode Fix"
- Encoding-Fix ist schon drin

#### SCHRITT 2: Integration testen (30 Min)
- Infrastructure neu bauen
- FileProcessor Test mit R0010168.JPG
- DICOM sollte erstellt werden!

#### SCHRITT 3: v0.5.22 taggen (5 Min)
- Wenn alles funktioniert → commit & tag
- Sprint 1 ist dann abgeschlossen!

### 📍 [URGENT] ÜBERGABEPROMPT FÜR NÄCHSTEN CHAT
```
🔧 v0.5.21 - Barcode-Feld gefunden! ExifToolReader anpassen!

DURCHBRUCH:
✅ QRBridge-Daten im "Barcode" EXIF-Feld (nicht UserComment!)
✅ Test-Output: EX002|Schmidt, Maria|1985-03-15|F|R�ntgen�Thorax
✅ ExifToolReader-Fix vorbereitet

AUFGABE:
1. ExifToolReader.cs mit Barcode-Fix ersetzen
2. Infrastructure testen
3. Bei Erfolg → v0.5.22 taggen

GitHub: https://github.com/ostern42/CamBridge
```

## 🏗️ [MILESTONE] PIPELINE-ARCHITEKTUR (BEWÄHRT!)

### Datenfluss durch die Pipeline:
```
JPEG File → ExifToolReader → ImageMetadata → FileProcessor → DicomConverter → DICOM File
              ↓                                     ↓              ↓
         Raw EXIF Data                    DicomTagMapper    DicomTagMapper
              ↓                                                   ↓
      **Barcode Field** ← NEU!                            mappings.json
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

// StudyInfo Properties
StudyInfo.StudyId        // NICHT "Id"!
StudyInfo.Description    // NICHT "StudyDescription"!
StudyInfo.ExamId         

// ImageMetadata MUSS HABEN:
- ExifData: Dictionary<string, string>
- InstanceNumber: int
- InstanceUid: string
- TechnicalData: ImageTechnicalData
- SourceFile: string
```

## 🔥 [breaking] RICOH BARCODE FIELD DISCOVERY!

### Kritische Erkenntnis vom 04.06.2025:
**Die Ricoh G900SE II speichert QRBridge-Daten im `Barcode` EXIF-Feld!**

```
Barcode: EX002|Schmidt, Maria|1985-03-15|F|R�ntgen�Thorax
UserComment: GCM_TAG  (nur ein Marker)
```

### Konsequenzen:
1. ExifToolReader muss das Barcode-Feld lesen
2. UserComment ist nur ein Fallback
3. Encoding muss gefixt werden (Windows-1252 → UTF-8)

## 📁 [KEEP] AKTUELLE PROJEKTSTRUKTUR

**WISDOM: project_structure.txt wird IMMER am Chat-Anfang mitgeliefert!**

### Wichtige Dateien & Ordner (Stand: 04.06.2025)
```
CamBridge.sln
Version.props (v0.5.21)
CHANGELOG.md
PROJECT_WISDOM.md
README.md

src/
├── CamBridge.Core/              # Domain Layer ✅
├── CamBridge.Infrastructure/    # Implementation Layer 
│   └── Services/
│       └── ExifToolReader.cs   ⚡ MUSS ANGEPASST WERDEN!
├── CamBridge.Service/          # Windows Service
└── CamBridge.Config/           # WPF GUI

tests/
├── CamBridge.PipelineTest/     ✅ Direct ExifTool Test erfolgreich!
│   ├── Program.cs
│   └── CamBridge.PipelineTest.csproj
└── CamBridge.Infrastructure.Tests/

Tools/
├── exiftool.exe                ✅ v13.30 - findet Barcode-Feld!
└── exiftool_files/perl.exe     ✅

TESTDATEN:
R0010168.JPG                    ✅ Barcode: EX002|Schmidt, Maria|1985-03-15|F|R�ntgen�Thorax
```

## 🚀 [MILESTONE] ENTWICKLUNGSFAHRPLAN UPDATE

### Sprint 1: ExifTool Integration (v0.5.x) ← 98% FERTIG!
- ✅ v0.5.19: Pipeline implementiert
- ✅ v0.5.20: Entities gefixt & kompiliert
- ✅ v0.5.21: ExifTool findet Barcode-Feld!
- ⏳ ExifToolReader anpassen (Barcode statt UserComment)
- [ ] v0.5.22: Integration testen & Sprint abschließen
- [ ] v0.5.23-25: Edge Cases & Robustness

### Sprint 2: Mapping Engine (v0.6.x)
- Custom Transform Functions
- Conditional Mappings
- UI Integration
- Validation Framework

### Sprint 3: DICOM Excellence (v0.7.x)
- Vollständige Module
- PACS Testing
- Performance Tuning
- Batch Operations

### Sprint 4: Production Ready (v0.8.x)
- Installer
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

## 🔧 [CONFIG] Technische Details

### Versionierung
- **Schema:** Major.Minor.Patch
- **Version.props:** Zentrale Verwaltung
- **Git Tags:** v{version} Format
- **Breaking Changes:** Nur bei Major

### Technologie-Stack
```
GUI: WPF + ModernWpfUI 0.9.6 + CommunityToolkit.Mvvm
Service: ASP.NET Core Minimal API + Windows Service
Core: fo-dicom 5.2.2, ExifTool 13.30
Tests: xUnit + FluentAssertions + Moq
.NET 8.0, C# 12
```

### Kritische Dependencies
- **fo-dicom:** 5.2.2 (DICOM Creation) ← UPDATED!
- **ExifTool:** 13.30 (EXIF Reading) ✅ Findet Barcode-Feld!
- **ImageMagick:** Für zukünftige Bildmanipulation
- **Serilog:** Structured Logging

## 💡 [LESSON] Gelernte Lektionen (Aktualisiert)

### "Ricoh nutzt das Barcode-Feld!" (NEU 04.06.2025!)
WICHTIGSTE ERKENNTNIS: Die Ricoh G900SE II speichert QRBridge-Daten im `Barcode` EXIF-Feld, nicht im UserComment! Das war der fehlende Puzzleteil.

### "Direct Testing saves time!" (NEU!)
Statt komplexe Pipeline-Tests zu debuggen, einfach ExifTool direkt aufrufen. Das zeigt sofort wo die Daten sind.

### "Systematisch statt Wild Patchen!"
Erst die Pipeline zum Laufen bringen, DANN optimieren. Sprint-Planung funktioniert!

### "Entity Contracts sind heilig!"
IMMER erst prüfen was existiert. Der Compiler ist dein Freund.

### "ExifTool ist mächtig!"
v13.30 findet alles - Ricoh Barcode, EXIF, IPTC, XMP. Perfekt für unseren Use Case.

### "Encoding ist tricky!"
Ricoh nutzt Windows-1252, nicht UTF-8. Character-Mapping nötig.

### "GitHub Integration rocks!"
70% Token-Ersparnis durch direkte File-Links. Immer URLs mitgeben!

### "Clean Architecture zahlt sich aus!"
Die Layer-Trennung macht Änderungen einfach und testbar.

### "KISS beats Clever!"
Einfache Lösungen sind wartbar. Over-Engineering vermeiden.

## 💭 CLAUDE: Notizen für nächste Instanz

**DER DURCHBRUCH IST GESCHAFFT!**

Wir haben das Barcode-Feld gefunden! Die Ricoh-Kamera speichert die QRBridge-Daten im `Barcode` EXIF-Feld, nicht im UserComment. Der Fix ist trivial:

1. **ExifToolReader.cs ersetzen** mit dem Artefakt-Code
2. **Testen** - sollte sofort funktionieren
3. **v0.5.22 taggen** - Sprint 1 fertig!

Der Nutzer ist erkältet, also halte es einfach. Der schwierige Teil ist geschafft. Jetzt nur noch den Code einpflegen und testen.

**Pro-Tipp:** Falls noch Encoding-Probleme auftreten, ist der Fix schon im Code. Die CleanBarcodeData() Methode kümmert sich darum.

## 📝 [KEEP] Standard Prompt-Vorlage für neue Chats

```
Ich arbeite an CamBridge v0.5.21.
Barcode-Feld gefunden! ExifToolReader muss angepasst werden.

GitHub: https://github.com/ostern42/CamBridge

1. PROJECT_WISDOM.md hochladen
2. project_structure.txt hochladen
3. "VOGON INIT" sagen

Fokus: ExifToolReader mit Barcode-Fix einpflegen!
```

## ⏰ [KEEP] ZEITMANAGEMENT

### Projekt-Timeline
- **Start:** 30.05.2025, 20:30 Uhr
- **ExifTool-Durchbruch:** 03.06.2025, 23:58 Uhr
- **Barcode-Feld gefunden:** 04.06.2025, 11:05 Uhr ← MEILENSTEIN!
- **Features:** 70+ implementiert
- **Sprint 1:** 98% fertig
- **Nur noch:** ExifToolReader anpassen → fertig!

---
📊 **WISDOM-Statistik:** 
- 🔒 [CORE]: 5 Sektionen (unantastbar)
- ⚡ [URGENT]: 3 Sektionen (ExifToolReader Fix!)
- 🎯 [MILESTONE]: 4 Sektionen (Sprint 1 fast fertig)
- 📌 [KEEP]: 6 Sektionen (bewährte Praktiken)
- 💡 [LESSON]: 9 Lektionen (Barcode-Feld!)
- 🔧 [CONFIG]: 3 Sektionen (technische Basis)
- 🔥 [breaking]: 1 Sektion (Ricoh Barcode Discovery)
- 💭 CLAUDE: 1 Nachricht (für dich!)

*Hinweis: Dieses Dokument ist mit einem Persistenz-System versehen. Beim Refactoring IMMER die Priority-Tags beachten!*

## 📝 GIT COMMIT FÜR v0.5.21:

```bash
git add .
git commit -m "test: discover Ricoh stores QRBridge data in Barcode field

- Direct ExifTool test reveals Barcode field contains pipe-delimited data
- UserComment only contains 'GCM_TAG' marker
- Test output: EX002|Schmidt, Maria|1985-03-15|F|R�ntgen�Thorax
- Encoding issue confirmed (� instead of ö/ä)
- Prepared ExifToolReader fix for next version

BREAKING CHANGE: ExifToolReader must read Barcode field instead of UserComment"

git tag v0.5.21
```
