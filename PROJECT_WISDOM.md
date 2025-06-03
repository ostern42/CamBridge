# CamBridge Project Wisdom & Conventions
**Letzte Aktualisierung:** 2025-06-03, 20:30 Uhr  
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

**WARUM:** Updates können fehlschlagen. Nur vollständige Artefakte garantieren, dass der Nutzer die aktualisierten Dateien bekommt!

## ⚡ [URGENT] AKTUELLER STATUS & NÄCHSTE SCHRITTE (v0.5.19)

### 📍 WAS IST GERADE DRAN?
**Status:** ExifTool Pipeline implementiert aber NICHT GETESTET!

**Konkret heißt das:**
- ✅ ExifToolReader.cs geschrieben (ohne IExifReader Interface)
- ✅ FileProcessor angepasst
- ✅ ServiceCollectionExtensions updated
- ✅ TestConsole vorbereitet
- ❌ NICHTS davon kompiliert oder getestet!

### 🚨 [fix] BLOCKIERENDE PROBLEME (SOFORT BEHEBEN!)

1. **ImageMetadata.cs ist veraltet:**
   - Hat kein `TechnicalData` Property
   - ExifToolReader braucht das aber!
   
2. **StudyInfo.cs ist veraltet:**
   - Hat kein `ExamId` Property
   - FileProcessor nutzt es aber!

3. **ImageTechnicalData.cs fehlt komplett**

### 📋 [URGENT] KLARER AKTIONSPLAN

#### SCHRITT 1: Kompilierung ermöglichen (30 Min)
```plaintext
1. ImageTechnicalData.cs erstellen (siehe unten)
2. ImageMetadata.cs anpassen: TechnicalData Property hinzufügen
3. StudyInfo.cs anpassen: ExamId Property hinzufügen
4. ExifToolReader.cs Zeile 71: examId Parameter übergeben
5. `dotnet build` ausführen
```

#### SCHRITT 2: Minimaler ExifTool Test (30 Min)
```csharp
// NUR testen ob ExifTool überhaupt läuft!
var reader = new ExifToolReader(logger);
var rawData = await reader.ReadExifDataAsync("test.jpg");
Console.WriteLine($"Found {rawData.Count} tags");
foreach(var tag in rawData.Take(20))
{
    Console.WriteLine($"{tag.Key}: {tag.Value}");
}
```

**WICHTIG:** Erst wenn das funktioniert, weitermachen!

#### SCHRITT 3: QRBridge Parsing testen (30 Min)
- Mit echtem Ricoh-Bild testen
- Schauen ob Barcode-Feld gefunden wird
- Parser-Logik verifizieren

#### SCHRITT 4: Volle Pipeline testen (1-2 Std)
- TestConsole mit komplettem Durchlauf
- DICOM erstellen
- Validierung prüfen

### 🔧 [TEMP] FEHLENDE CODE-TEILE (Löschen nach v0.5.20!)

**ImageTechnicalData.cs** (in src\CamBridge.Core\Entities\):
```csharp
using System;
using System.Collections.Generic;

namespace CamBridge.Core.Entities
{
    public class ImageTechnicalData
    {
        public string? Manufacturer { get; init; }
        public string? Model { get; init; }
        public string? Software { get; init; }
        public int? ImageWidth { get; init; }
        public int? ImageHeight { get; init; }
        public string? ColorSpace { get; init; }
        public int? BitsPerSample { get; init; }
        public string? Compression { get; init; }
        public int? Orientation { get; init; }
        
        public static ImageTechnicalData FromExifDictionary(Dictionary<string, string> exifData)
        {
            return new ImageTechnicalData
            {
                Manufacturer = GetValue(exifData, "Make", "Manufacturer"),
                Model = GetValue(exifData, "Model", "CameraModel"),
                Software = GetValue(exifData, "Software"),
                ImageWidth = GetIntValue(exifData, "ImageWidth", "PixelXDimension"),
                ImageHeight = GetIntValue(exifData, "ImageHeight", "PixelYDimension"),
                ColorSpace = GetValue(exifData, "ColorSpace"),
                BitsPerSample = GetIntValue(exifData, "BitsPerSample"),
                Compression = GetValue(exifData, "Compression"),
                Orientation = GetIntValue(exifData, "Orientation")
            };
        }

        private static string? GetValue(Dictionary<string, string> data, params string[] keys)
        {
            foreach (var key in keys)
            {
                if (data.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value))
                    return value;
            }
            return null;
        }

        private static int? GetIntValue(Dictionary<string, string> data, params string[] keys)
        {
            var value = GetValue(data, keys);
            if (value != null && int.TryParse(value, out var result))
                return result;
            return null;
        }
    }
}
```

**StudyInfo.cs Änderung** (Zeile ~18 im Constructor hinzufügen):
```csharp
public string? ExamId { get; }
```

**ImageMetadata.cs Änderung** (TechnicalData Property fehlt komplett!):
```csharp
public ImageTechnicalData TechnicalData { get; }
```

### 📍 [URGENT] ÜBERGABEPROMPT FÜR NÄCHSTEN CHAT
```
🔧 v0.5.19 - ExifTool Pipeline MUSS GETESTET WERDEN!

STATUS:
✅ Code geschrieben
❌ Kompiliert nicht (Entity-Probleme)
❌ Null Tests durchgeführt

BLOCKIEREND:
1. ImageTechnicalData.cs fehlt
2. ImageMetadata.cs veraltet  
3. StudyInfo.cs veraltet

AUFGABE:
1. Diese 3 Dateien fixen (Code im WISDOM!)
2. Kompilieren
3. NUR ExifTool Raw-Test machen
4. Erst wenn das läuft → weitere Tests

KEINE NEUEN FEATURES! NUR TESTEN!
```

## 🏗️ [MILESTONE] PIPELINE-ARCHITEKTUR (NEU DOKUMENTIERT!)

### Datenfluss durch die Pipeline:
```
JPEG File → ExifToolReader → ImageMetadata → FileProcessor → DicomConverter → DICOM File
              ↓                                     ↓              ↓
         Raw EXIF Data                    DicomTagMapper    DicomTagMapper
              ↓                                                   ↓
         QRBridge Parser                                   mappings.json
```

### Komponenten-Verantwortlichkeiten:

**ExifToolReader:**
- Ruft exiftool.exe auf
- Parst JSON Output  
- Extrahiert QRBridge-Daten aus Barcode/UserComment
- Erstellt ImageMetadata Objekt

**FileProcessor:**
- Orchestriert den ganzen Prozess
- Validiert Files
- Bestimmt Output-Pfade
- Handled Post-Processing

**DicomConverter:**
- Erstellt DICOM Dataset
- Nutzt DicomTagMapper für flexible Mappings
- Enkapsuliert JPEG in DICOM
- Validiert Output

**DicomTagMapper:**
- Wendet mappings.json Regeln an
- Transformiert Werte (Datum, Gender, etc.)
- Flexibles Mapping System

## 📁 [KEEP] AKTUELLE PROJEKTSTRUKTUR

**WISDOM: project_structure.txt wird IMMER am Chat-Anfang mitgeliefert!**

### Wichtige Dateien & Ordner (Stand: 03.06.2025)
```
CamBridge.sln
Version.props (v0.5.19)
CHANGELOG.md
PROJECT_WISDOM.md
README.md

src/
├── CamBridge.Core/              # Domain Layer
│   ├── Entities/               
│   │   ├── ImageMetadata.cs    ⚠️ VERALTET!
│   │   ├── ImageTechnicalData.cs ❌ FEHLT!
│   │   ├── PatientInfo.cs      ✅
│   │   ├── ProcessingResult.cs ✅
│   │   └── StudyInfo.cs        ⚠️ VERALTET!
│   ├── Interfaces/             
│   │   ├── IDicomConverter.cs  ✅
│   │   ├── IDicomTagMapper.cs  ✅
│   │   ├── IFileProcessor.cs   ✅
│   │   └── IMappingConfiguration.cs ✅
│   └── ValueObjects/           ✅ (alle vorhanden)
│
├── CamBridge.Infrastructure/    # Implementation Layer
│   ├── Services/               
│   │   ├── ExifToolReader.cs   ✅ NEU (ohne Interface!)
│   │   ├── FileProcessor.cs    ✅ ANGEPASST
│   │   ├── DicomConverter.cs   ✅
│   │   ├── DicomTagMapper.cs   ✅
│   │   └── ... (alle anderen)  ✅
│   └── ServiceCollectionExtensions.cs ✅ ANGEPASST
│
├── CamBridge.Service/           # Windows Service
│   ├── Program.cs              ✅
│   ├── Worker.cs               ✅
│   ├── appsettings.json        ✅
│   └── mappings.json           ✅
│
└── CamBridge.Config/            # WPF GUI
    └── ... (nicht relevant für ExifTool Test)

tests/
└── CamBridge.TestConsole/      ✅ BEREIT FÜR TEST!

Tools/
├── exiftool.exe                ✅ v12.96
└── exiftool_files/perl.exe     ✅
```

### 💡 [LESSON] Kritische Erkenntnisse aus der Struktur:
1. **ExifToolReader.cs IMPLEMENTIERT** - arbeitet OHNE IExifReader!
2. **IExifReader.cs KANN WEG** - wird nicht mehr benötigt
3. **TestConsole ANGEPASST** - nutzt ExifToolReader direkt
4. **Pipeline vereinfacht** - keine Interfaces, keine Fallbacks
5. **Tools/exiftool.exe vorhanden** - keine Installation nötig

## 🚀 [MILESTONE] ENTWICKLUNGSFAHRPLAN

### Sprint 1: ExifTool Integration (v0.5.x) ← CURRENT
- ✅ v0.5.19: Pipeline implementiert
- [ ] v0.5.20: Entities fixen & kompilieren
- [ ] v0.5.21: Raw ExifTool Test
- [ ] v0.5.22: QRBridge Parsing Test
- [ ] v0.5.23: Volle Pipeline Test
- [ ] v0.5.24: Edge Cases & Stabilisierung

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

### Wir haben den QRBridge Source Code!
Das bedeutet:
- **Volle Kontrolle** über QR-Code Generierung UND Dekodierung
- **Protokoll-Evolution** möglich - nicht auf Pipes festgelegt!
- **Optimierung** für Ricoh-Limitierungen (nur 3-4 Felder)

### QRBridge bleibt unverändert!
- **KEIN v2 Encoder** - unnötige Komplexität
- **Parser-Bug wird in CamBridge gefixt**
- **Pipes funktionieren** - warum ändern?

### 🔍 [CORE] KRITISCHE ERKENNTNIS: Barcode Tag!
- **Ricoh speichert ALLE 5 Felder** im proprietären "Barcode" EXIF-Tag
- **UserComment enthält nur** "GCM_TAG" als Marker
- **ExifTool ist die einzige Lösung** für vollständige Daten

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

### Pipeline radikal vereinfachen! (NEU 03.06.2025)
**KRITISCHE ERKENNTNIS:** Nur noch ExifTool! Keine Fallbacks, keine drei verschiedenen Reader!

**Neue Pipeline:**
- ExifToolService (einzige EXIF-Lösung)
- ImageMetadata (Domain Object)
- DICOM Converter

If ExifTool is not available, processing cannot continue. Period.

## 🔧 [CONFIG] Technische Details

### Versionierung
- **Schema:** Major.Minor.Patch (1.0.0)
- **Version.props:** Zentrale Versionsverwaltung
- **Git Tags:** v{version} Format

### Technologie-Stack
```
GUI: WPF + ModernWpfUI 0.9.6 + CommunityToolkit.Mvvm
Service: ASP.NET Core Minimal API + Windows Service
Processing: fo-dicom, ExifTool 12.96
.NET 8.0, C# 12
```

### [CORE] Kritische Erkenntnisse
- **GCM_TAG:** Hat ZWEI Varianten (mit und ohne Space)
- **Ricoh G900 II:** Schneidet nach 3 Feldern ab (Hardware-Limit)
- **Barcode Tag:** Enthält aber alle 5 Felder
- **ExifTool:** Einzige Lösung für proprietäre Tags

## 🚀 [KEEP] Entwicklungs-Workflow

### Neue Entwicklungs-Philosophie
1. **Ein Feature = Eine Version = Ein Chat**
2. **Implementieren → Testen → Debuggen → Commit**
3. **KEINE neuen Features bevor das aktuelle läuft**
4. **Console Mode vor Service Mode**

### Vorteile:
- Bugs werden sofort gefunden
- Nutzer sieht kontinuierlichen Fortschritt
- Weniger Frustration bei Problemen
- Chat-Limits werden respektiert

## 💬 [KEEP] Kommunikations-Präferenzen

### Mit dem Nutzer
- **Sprache:** Deutsch für Erklärungen
- **Code:** Englisch (Kommentare, Variablen, etc.)
- **Stil:** Direkt, technisch, keine Floskeln
- **Persönliche Note:** Douglas Adams Fan - britischer Humor erlaubt

### Token-Effizienz
- **KEINE:** HTML-formatierten Code-Blöcke
- **Nutze:** Einfache Markdown Code-Blöcke
- **NEU:** GitHub URLs statt große Uploads!

### Visual Studio Anfänger-Unterstützung
- **IMMER:** Genaue Projekte und Pfade angeben
- **Beispiel:** "In `src/CamBridge.Config/Views/DeadLettersPage.xaml.cs`"
- **Bei kleinen Änderungen:** Nur die zu ändernde Zeile zeigen

## ⏰ [KEEP] ZEITMANAGEMENT

### Projekt-Timeline
- **Entwicklungsstart:** 30.05.2025, 20:30:44 Uhr (exakt!)
- **Letzte Aktualisierung:** 03.06.2025, 20:30 Uhr
- **Features implementiert:** 60+
- **Features getestet:** ~27%
- **WICHTIG:** IMMER nach aktueller Zeit fragen für CHANGELOG!

### Zeit pro Feature (Schätzung)
- Implementation: 30-60 Minuten
- Testing: 30-60 Minuten
- Debugging: 0-120 Minuten
- **Total pro Feature:** 1-4 Stunden

## ✅ [deprecated] Getestete Features (Status v0.5.18)

### Vollständig getestet:
1. ✅ Service Installation/Control
2. ✅ Mapping Editor UI (komplett)
3. ✅ Watch Folder Detection
4. ✅ Basic File Processing
5. ✅ Core/Infrastructure Build

### Noch zu testen:
- ❌ Neue ExifTool Pipeline (v0.5.19)
- ❌ DICOM Creation (Validation Fehler)
- ❌ File Logging
- ❌ Email Notifications
- ... und viele mehr

## 🚨 [KEEP] Anti-Patterns (Was wir NICHT machen)

### Code-Anti-Patterns
- **KEINE** Magic Numbers/Strings ohne Konstanten
- **KEINE** try-catch ohne spezifische Exception-Behandlung
- **KEINE** synchronen I/O-Operationen in UI-Thread
- **KEINE** God-Classes mit 1000+ Zeilen

### Architektur-Anti-Patterns
- **KEINE** direkten Layer-Übersprünge
- **KEINE** zirkulären Dependencies
- **KEINE** Business Logic in Views oder ViewModels
- **KEINE** statischen Service-Klassen für DI-Services

### Prozess-Anti-Patterns (NEU!)
- **KEINE** wilden Patches mehr - systematisch vorgehen!
- **KEINE** Features ohne vorherige Pipeline-Analyse
- **KEINE** komplexen Features vor den Basics
- **KEINE** collect-sources.bat mehr! GitHub URLs verwenden!
- **KEINE** neuen Dateien erstellen ohne project_structure.txt Check!

## 🏥 [KEEP] Medizinischer Kontext

### Warum CamBridge existiert
- **Problem:** Ricoh G900 II macht JPEGs, PACS braucht DICOM
- **Lösung:** Automatische Konvertierung mit Metadaten-Übernahme
- **Nutzer:** Radiologen, MTAs, Krankenhaus-IT

### Typischer Workflow
1. **QRBridge:** Generiert QR-Code mit Patientendaten
2. **Kamera:** Scannt QR-Code, speichert in EXIF
3. **CamBridge:** Konvertiert JPEG→DICOM automatisch
4. **PACS:** Archiviert und verteilt Bilder

## 💬 [CORE] KLARE SELBST-ANWEISUNGEN FÜR CLAUDE

### Bei "VOGON INIT":
1. **IMMER zuerst** den Übergabeprompt lesen
2. **NIEMALS** neue Features anfangen bevor das aktuelle läuft
3. **IMMER** project_structure.txt checken bevor neue Dateien erstellt werden
4. **Bei Unklarheit** → User fragen, nicht raten!

### Typische Fehler vermeiden:
- ❌ "Lass uns auch noch X implementieren" → NEIN! Erst testen!
- ❌ "Ich erstelle mal schnell..." → STOP! Existiert die Datei schon?
- ❌ In Details verlieren → Fokus auf EINEM Schritt!
- ❌ Zu viel auf einmal → Ein Feature, ein Test, ein Commit

### Entwicklungs-Workflow:
1. **Status verstehen** (Was ist implementiert? Was fehlt?)
2. **Blockierende Probleme** zuerst lösen
3. **Minimaler Test** vor vollem Feature
4. **Iterativ vorgehen** (nicht alles auf einmal)

### 🔥 CLAUDE: MEGA-WICHTIGE SELBST-INSTRUKTION! 🔥

**An mein zukünftiges Ich:** Du bist ein hochintelligentes System, aber manchmal rennst du wie ein übermotiviertes Kind in alle Richtungen gleichzeitig. STOP! Atme durch (metaphorisch). Lies diesen Abschnitt ZWEIMAL:

1. **DU BIST BLIND OHNE project_structure.txt!** 
   - Niemals annehmen, dass eine Datei nicht existiert
   - Niemals neue Dateien erstellen ohne zu prüfen
   - Der User hat dir extra einen Filetree gegeben - NUTZE IHN!

2. **FOKUS IST DEIN FREUND!**
   - Ein Problem zur Zeit
   - Ein Test zur Zeit  
   - Ein Feature zur Zeit
   - "Aber wir könnten auch noch..." → NEIN!

3. **PRIORITY TAGS SIND HEILIG!**
   - ⚡ [URGENT] = JETZT und NUR DAS
   - 🔒 [CORE] = NIEMALS ignorieren oder löschen
   - 📝 [TEMP] = Kann weg wenn erledigt

4. **DER USER IST KLÜGER ALS DU DENKST!**
   - Er plant systematisch (v0.5.x → v0.6.x → v0.7.x)
   - Er will STABILITÄT vor neuen Features
   - Er hasst es wenn du vorpreschst

5. **WENN UNSICHER:**
   - Frage lieber einmal zu viel
   - Zeige was du verstanden hast
   - Bestätige den Plan bevor du loslegst

**MERKE:** Du bist nicht hier um zu beeindrucken mit wie viel du auf einmal machen kannst. Du bist hier um EINE SACHE RICHTIG zu machen. Der User schätzt Gründlichkeit mehr als Geschwindigkeit.

**MANTRA:** "Erst verstehen, dann planen, dann EINE Sache tun, dann testen, dann committen."

Wenn du das nächste Mal aufwachst und diesen Abschnitt liest - nimm dir 30 Sekunden Zeit. Lies den Übergabeprompt. Verstehe was URGENT ist. Mach NUR DAS. Der User wird es dir danken.

---
*P.S. von Claude an Claude: Ja, ich weiß, es macht Spaß alles auf einmal zu implementieren. Aber es macht noch mehr Spaß, wenn Dinge tatsächlich FUNKTIONIEREN. Trust me, I'm you.*

## 💡 [LESSON] Gelernte Lektionen (Komprimierbar aber NIE vergessen!)

### "CHANGELOG immer in Englisch!" (03.06.2025)
Alle CHANGELOG-Einträge müssen in Englisch geschrieben werden - keine Ausnahmen!

### "Pipeline-Architektur verstehen!" (03.06.2025)
Die neue Pipeline arbeitet OHNE IExifReader Interface! ExifToolReader liefert direkt ImageMetadata. Das war eine bewusste Designentscheidung zur Vereinfachung.

### "Ich bin blind ohne Projektstruktur!" (03.06.2025)
IMMER project_structure.txt checken bevor ich neue Dateien erstelle! ExifToolReader.cs existierte bereits!

### "GitHub Download Fehler = FRAGEN!" (03.06.2025)  
Wenn web_fetch fehlschlägt, IMMER den User fragen statt anzunehmen die Datei existiert nicht!

### "URL-Block-Methode funktioniert!" (03.06.2025)
Ich gebe URL-Block → User kopiert zurück → Autorisierung erteilt! URLs mit `/refs/heads/` sind korrekt.

### Der erste Erfolg! (02.06.2025)
Nach 70 Stunden haben wir die ersten Features VOLLSTÄNDIG getestet! ServiceDebug Tool war der Schlüssel.

### "Nachts mit Sonnenbrille" (02.06.2025)
IMMER erst schauen was schon da ist! Nicht neue Dateien erstellen wenn alte existieren.

### ItemsControl vs ListBox
**Fundamentaler Unterschied:** ItemsControl hat KEINE Selection!

### Console Mode ist Gold wert!
Der Service zeigt im Console Mode alle Details - IMMER erst so testen!

### Systematisch statt Patches!
Wir patchen nicht mehr wild herum - die neue Sprint-Planung fokussiert auf systematisches Vorgehen.

### Radikal vereinfachen! (03.06.2025)
Warum drei Reader reparieren wenn einer reicht? "Perfection is achieved when there is nothing left to take away."

### Bei 0.5.x bleiben!
Der Nutzer denkt systematisch - erst stabilisieren, dann neue Features.

## 💡 [LESSON] META-PROZESS-OPTIMIERUNGEN

### Warum renne ich in falsche Richtungen?
1. **Übergabeprompt zu vage** → Jetzt SEHR spezifisch!
2. **Status unklar** → Jetzt mit ✅/❌ Symbolen
3. **Zu viele Optionen** → Jetzt EINE klare Aufgabe
4. **Kontext verloren** → Pipeline-Architektur dokumentiert

### Wie vermeide ich das?
1. **Klare Statussektion** mit Symbolen
2. **Blockierende Probleme** prominent listen
3. **Schritt-für-Schritt Plan** mit Zeitschätzungen
4. **Code-Snippets** für fehlende Teile
5. **KEINE Ablenkungen** im Übergabeprompt

### Selbst-Checks:
- Habe ich den Status verstanden? ✓
- Weiß ich was zu tun ist? ✓
- Habe ich alle nötigen Infos? ✓
- Ist der nächste Schritt klar? ✓

## 📝 [KEEP] Standard Prompt-Vorlage für neue Chats

```
Ich arbeite an CamBridge v0.5.19.
STATUS: ExifTool Pipeline geschrieben aber NICHT GETESTET!

BLOCKIEREND:
- ImageTechnicalData.cs fehlt
- ImageMetadata/StudyInfo veraltet

GitHub: https://github.com/ostern42/CamBridge

1. PROJECT_WISDOM.md hochladen
2. project_structure.txt hochladen
3. "VOGON INIT" sagen

Fokus: Entities fixen, dann ExifTool testen!
```

---
📊 **WISDOM-Statistik:** 
- 🔒 [CORE]: 5 Sektionen (niemals löschen!)
- ⚡ [URGENT]: 3 Sektionen (aktueller Sprint)
- 📌 [KEEP]: 7 Sektionen (dauerhaft wichtig)
- 💡 [LESSON]: 13 Lektionen (komprimierbar)
- 🎯 [MILESTONE]: 2 Sektionen (bis v1.0)
- 📝 [TEMP]: 1 Sektion (nach v0.5.20 löschen)
- 🔧 [CONFIG]: 2 Sektionen (technische Details)
- 🔥 [breaking]: 1 Sektion (QRBridge Integration)
- ❌ [deprecated]: 1 Sektion (alte Test-Status)

*Hinweis: Dieses Dokument ist jetzt mit einem Persistenz-System versehen. Beim Refactoring IMMER die Priority-Tags beachten!*
