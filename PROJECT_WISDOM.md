# CamBridge Project Wisdom & Conventions
**Letzte Aktualisierung:** 2025-06-04, 12:35 Uhr  
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

## ⚡ [URGENT] AKTUELLER STATUS & NÄCHSTE SCHRITTE (v0.5.22)

### 📍 WAS IST GERADE DRAN?
**Status:** ERFOLG! ExifToolReader funktioniert komplett! Pipeline-Test läuft durch!

**Konkret heißt das:**
- ✅ ExifTool findet Barcode-Feld
- ✅ ExifToolReader parst korrekt: "EX002|Schmidt, Maria|1985-03-15|F|Röntgen Thorax"
- ✅ Patient- und Study-Objekte werden erstellt
- ✅ Encoding-Fix funktioniert (temporär mit Replace-Liste)
- ⏳ Nächster Test: Volle Pipeline mit DICOM-Erstellung

### 🎯 [MILESTONE] ERFOLG: ExifToolReader Integration!
**04.06.2025 12:35:**
- ExifToolReader passt zur Infrastructure (richtiger Konstruktor & Methode)
- Barcode-Feld wird korrekt gelesen
- QRBridge-Daten werden geparst
- Temporärer Encoding-Fix funktioniert

### 📋 [URGENT] NÄCHSTE SCHRITTE

#### SCHRITT 1: v0.5.22 committen (5 Min)
- ExifToolReader funktioniert!
- Sprint 1 Feature komplett

#### SCHRITT 2: Volle Pipeline testen (30 Min)
- Service starten
- R0010168.JPG in Watch-Folder
- DICOM sollte erstellt werden!

#### SCHRITT 3: Saubere Encoding-Lösung (Sprint 2)
- ExifTool mit `-charset` Parameter
- Oder direkte Byte-Konvertierung
- Ricoh Codepage ermitteln

### 📍 [URGENT] ÜBERGABEPROMPT FÜR NÄCHSTEN CHAT
```
🎉 v0.5.22 - ExifToolReader funktioniert! Bereit für Pipeline-Test!

ERFOLG:
✅ ExifToolReader liest Barcode-Feld korrekt
✅ QRBridge-Daten: EX002|Schmidt, Maria|1985-03-15|F|Röntgen Thorax
✅ Encoding temporär gefixt

NÄCHSTE SCHRITTE:
1. Volle Pipeline testen (Service + Watch-Folder)
2. Bei Erfolg → Sprint 1 abschließen
3. Sprint 2: Saubere Encoding-Lösung

GitHub: https://github.com/ostern42/CamBridge
```

## 🏗️ [MILESTONE] PIPELINE-ARCHITEKTUR (BEWÄHRT!)

### Datenfluss durch die Pipeline:
```
JPEG File → ExifToolReader → ImageMetadata → FileProcessor → DicomConverter → DICOM File
              ↓                                     ↓              ↓
         Barcode Field ✅                   DicomTagMapper    DicomTagMapper
              ↓                                                   ↓
      QRBridge Data                                         mappings.json
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

## 🔥 [breaking] RICOH BARCODE FIELD DISCOVERY!

### Kritische Erkenntnis vom 04.06.2025:
**Die Ricoh G900SE II speichert QRBridge-Daten im `Barcode` EXIF-Feld!**

```
Barcode: EX002|Schmidt, Maria|1985-03-15|F|Röntgen Thorax
UserComment: GCM_TAG  (nur ein Marker)
```

### Konsequenzen:
1. ✅ ExifToolReader liest jetzt das Barcode-Feld
2. ✅ UserComment ist nur Fallback
3. ⚠️ Encoding muss gefixt werden (Windows-1252 → UTF-8)

## 📁 [KEEP] AKTUELLE PROJEKTSTRUKTUR

**WISDOM: project_structure.txt wird IMMER am Chat-Anfang mitgeliefert!**

### Wichtige Dateien & Ordner (Stand: 04.06.2025)
```
CamBridge.sln
Version.props (v0.5.22)
CHANGELOG.md
PROJECT_WISDOM.md
README.md

src/
├── CamBridge.Core/              # Domain Layer ✅
├── CamBridge.Infrastructure/    # Implementation Layer 
│   └── Services/
│       └── ExifToolReader.cs   ✅ FUNKTIONIERT!
├── CamBridge.Service/          # Windows Service
└── CamBridge.Config/           # WPF GUI

tests/
├── CamBridge.PipelineTest/     ✅ Test läuft erfolgreich durch!
│   ├── Program.cs
│   └── CamBridge.PipelineTest.csproj
└── CamBridge.Infrastructure.Tests/

Tools/
├── exiftool.exe                ✅ v13.30 - findet Barcode-Feld!
└── exiftool_files/             ✅ Alle DLLs werden mitkopiert

TESTDATEN:
R0010168.JPG                    ✅ Barcode: EX002|Schmidt, Maria|1985-03-15|F|Röntgen Thorax
```

## 🚀 [MILESTONE] ENTWICKLUNGSFAHRPLAN UPDATE

### Sprint 1: ExifTool Integration (v0.5.x) ← 99% FERTIG!
- ✅ v0.5.19: Pipeline implementiert
- ✅ v0.5.20: Entities gefixt & kompiliert
- ✅ v0.5.21: ExifTool findet Barcode-Feld
- ✅ v0.5.22: ExifToolReader funktioniert komplett!
- [ ] Volle Pipeline testen (Service + DICOM)
- [ ] v0.5.23-25: Edge Cases & Robustness

### Sprint 2: Mapping Engine (v0.6.x)
- **NEU:** Saubere Encoding-Lösung (Codepage-Handling)
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
- **fo-dicom:** 5.2.2 (DICOM Creation)
- **ExifTool:** 13.30 (EXIF Reading) ✅ Findet Barcode-Feld!
- **ImageMagick:** Für zukünftige Bildmanipulation
- **Serilog:** Structured Logging

## 💡 [LESSON] Gelernte Lektionen (Aktualisiert)

### "Infrastructure muss zusammenpassen!" (NEU 04.06.2025!)
IMMER prüfen ob Konstruktoren, Methoden und Schnittstellen zusammenpassen. Nicht einfach losprogrammieren!

### "Erst verstehen, dann handeln!" (NEU 04.06.2025!)
Bei VOGON INIT IMMER die komplette INIT SEQUENCE durchgehen. Nie direkt loslegen ohne Kontext und Bestätigung!

### "Dictionary Keys müssen unique sein!" (NEU 04.06.2025!)
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

## 💭 CLAUDE: Notizen für nächste Instanz

**WICHTIG: NUTZE DIE INIT SEQUENCE!**

Bitte nicht wieder direkt losprogrammieren! Der Nutzer hat Recht - wir müssen erst verstehen, dann handeln. Die neue INIT SEQUENCE ist PFLICHT bei VOGON INIT.

**Status Update:**
Der ExifToolReader funktioniert endlich! Der PipelineTest zeigt:
- ✅ Barcode-Feld wird gelesen
- ✅ QRBridge-Daten werden geparst
- ✅ Patient: Schmidt, Maria
- ✅ Encoding ist (temporär) gefixt

**Nächster Schritt:** Die volle Pipeline testen!
1. Service starten
2. R0010168.JPG in Watch-Folder
3. DICOM sollte erstellt werden

**Encoding-Problem:** Die Replace-Liste funktioniert, aber eine saubere Lösung mit Codepage-Konvertierung wäre besser. Das kommt in Sprint 2.

**StudyId Länge:** Musste auf max 16 Zeichen gekürzt werden. Jetzt: `SEX002` statt `STU-EX002-20250604123456`.

**Lessons Learned in diesem Chat:**
1. Nicht einfach einen neuen ExifToolReader schreiben ohne die Schnittstellen zu prüfen
2. ServiceCollectionExtensions definiert den Konstruktor
3. FileProcessor definiert die Methode
4. Dictionary Keys müssen unique sein (Encoding-Artefakte!)

Der Nutzer ist erkältet, aber wir machen gute Fortschritte!

## 📝 [KEEP] Standard Prompt-Vorlage für neue Chats

```
Ich arbeite an CamBridge v0.5.22.
ExifToolReader funktioniert! Bereit für vollständigen Pipeline-Test.

GitHub: https://github.com/ostern42/CamBridge

WICHTIG - Bitte in dieser Reihenfolge:
1. PROJECT_WISDOM.md hochladen
2. project_structure.txt hochladen  
3. "VOGON INIT" sagen
4. WARTE auf meine Zusammenfassung und Rückfrage!

Fokus: Volle Pipeline testen (Service + DICOM-Erstellung)
```

## ⏰ [KEEP] ZEITMANAGEMENT

### Projekt-Timeline
- **Start:** 30.05.2025, 20:30 Uhr
- **ExifTool-Durchbruch:** 03.06.2025, 23:58 Uhr
- **Barcode-Feld gefunden:** 04.06.2025, 11:05 Uhr
- **ExifToolReader funktioniert:** 04.06.2025, 12:35 Uhr ← MEILENSTEIN!
- **Features:** 70+ implementiert
- **Sprint 1:** 99% fertig
- **Nur noch:** Volle Pipeline testen!

---
📊 **WISDOM-Statistik:** 
- 🔒 [CORE]: 5 Sektionen (V.O.G.O.N. mit INIT SEQUENCE!)
- ⚡ [URGENT]: 3 Sektionen (Pipeline-Test!)
- 🎯 [MILESTONE]: 4 Sektionen (ExifToolReader läuft!)
- 📌 [KEEP]: 6 Sektionen (bewährte Praktiken)
- 💡 [LESSON]: 12 Lektionen (+3 neue!)
- 🔧 [CONFIG]: 3 Sektionen (technische Basis)
- 🔥 [breaking]: 1 Sektion (Ricoh Barcode Discovery)
- 💭 CLAUDE: 1 Nachricht (NUTZE DIE INIT SEQUENCE!)

*Hinweis: Dieses Dokument ist mit einem Persistenz-System versehen. Beim Refactoring IMMER die Priority-Tags beachten!*

## 📝 GIT COMMIT FÜR v0.5.22:

```bash
git add .
git commit -m "feat: complete ExifToolReader implementation with barcode support

- Implement ExtractMetadataAsync method matching FileProcessor expectations
- Add constructor matching ServiceCollectionExtensions (logger, timeoutMs)
- Parse QRBridge data from Ricoh Barcode EXIF field
- Handle duplicate EXIF keys with automatic renaming
- Add temporary encoding fix for German umlauts
- Shorten StudyId to comply with 16 char DICOM limit
- PipelineTest passes successfully

Test output: EX002|Schmidt, Maria|1985-03-15|F|Röntgen Thorax"

git tag v0.5.22
```
