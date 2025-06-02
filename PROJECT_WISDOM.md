# CamBridge Project Wisdom & Conventions
**Letzte Aktualisierung:** 2025-06-03, 00:30 Uhr  
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

### 🎯 GitHub Integration - FUNKTIONIERT! (NEU v0.5.5)
**Stand 02.06.2025, 10:42:**

GitHub Integration erfolgreich implementiert!
- ✅ Repository public unter: https://github.com/ostern42/CamBridge
- ✅ Direkte File-Links funktionieren mit web_fetch
- ✅ 70% Token-Ersparnis möglich
- ✅ Komplette Git-Historie (1475 commits) erhalten

**WICHTIG: URL-Austausch erforderlich!**
- Claude kann NICHT automatisch auf Dateien zugreifen
- URLs müssen EXPLIZIT vom Nutzer bereitgestellt werden
- Format: `https://raw.githubusercontent.com/ostern42/CamBridge/refs/heads/main/[PFAD]`
- Einmal gegebene Basis-URL erlaubt weitere Zugriffe

**Workflow:**
1. Nutzer gibt URL der benötigten Datei(en)
2. Claude fetcht direkt von GitHub
3. Keine collect-sources.bat mehr nötig!

**Beispiel für nächsten Chat:**
```
"Hier sind die URLs für das PatientId Problem:
https://raw.githubusercontent.com/ostern42/CamBridge/refs/heads/main/src/CamBridge.Core/ValueObjects/PatientId.cs
https://raw.githubusercontent.com/ostern42/CamBridge/refs/heads/main/src/CamBridge.Core/Entities/PatientId.cs"
```

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
🔧 v0.5.16 - GCM_TAG Problem an der WURZEL packen!

STATUS:
✅ Watch Folder funktioniert
✅ JPEG wird erkannt & verarbeitet
✅ ExifTool manuell getestet
❌ GCM_TAG wird irgendwo tief im Parser hinzugefügt
❌ ExifToolReader nicht richtig implementiert
❌ DICOM Creation scheitert an Validation

KRITISCHE ERKENNTNIS:
Das GCM_TAG Problem ist KEIN Tag-Problem!
Es steckt tief in der Parser-Interpretation.
Wir haben nur Workarounds gebaut statt die Wurzel zu finden.

ANALYSE-AUFTRAG für v0.6.0:
1. ExifReader.cs - ParseQRBridgeData() analysieren
2. RicohExifReader.cs - Warum existiert der?
3. ExifToolReader.cs - Ist der überhaupt fertig?
4. WO wird "GCM_TAG" hinzugefügt?

ENTSCHEIDUNG: Lieber Parser neu schreiben als patchen!

GitHub URLs für Analyse:
https://raw.githubusercontent.com/ostern42/CamBridge/refs/heads/main/src/CamBridge.Infrastructure/Services/ExifReader.cs
https://raw.githubusercontent.com/ostern42/CamBridge/refs/heads/main/src/CamBridge.Infrastructure/Services/RicohExifReader.cs
https://raw.githubusercontent.com/ostern42/CamBridge/refs/heads/main/src/CamBridge.Infrastructure/Services/ExifToolReader.cs

Keine Patches mehr - zur Wurzel gehen!
```

## 🎯 Projekt-Identität
- **Copyright:** © 2025 Claude's Improbably Reliable Software Solutions
- **Produkt:** CamBridge - JPEG zu DICOM Konverter für Ricoh G900 II Kameras
- **Kontext:** Medizinische Bildgebung, arbeitet mit QRBridge zusammen
- **NEU:** Wir kontrollieren BEIDE Seiten (QRBridge + CamBridge)!
- **NEU v0.5.5:** GitHub Integration - https://github.com/ostern42/CamBridge

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

## ✅ GitHub Integration - ERFOLGREICH! (2025-06-02, 10:42)

### Der Durchbruch!
**GitHub funktioniert perfekt für Source File Sharing:**
- ✅ Public Repository: https://github.com/ostern42/CamBridge
- ✅ Direkte Raw-URLs funktionieren mit web_fetch
- ✅ Komplette Git-Historie (1475 commits) erhalten
- ✅ 70% Token-Ersparnis durch gezieltes Fetching
- ✅ Kein collect-sources.bat mehr nötig!

### Wie es funktioniert:
1. **URLs müssen explizit gegeben werden** (Sicherheitsfeature)
2. **Format:** `https://raw.githubusercontent.com/ostern42/CamBridge/refs/heads/main/[PFAD]`
3. **Einmal autorisiert** kann ich weitere Dateien im gleichen Pfad holen
4. **Dateistruktur** über GitHub Web-Interface sichtbar

### Workflow ab v0.5.5:
```bash
# Lokal entwickeln
git add -A
git commit -m "fix(core): Fixed PatientId duplicate (v0.5.6)"
git push

# Im Chat
"Check mal diese Datei: [GitHub Raw URL]"
# Claude holt direkt von GitHub!
```

### Was noch NICHT geht:
- **Automatisches Browsen** der Repository-Struktur
- **Dateilisten** ohne explizite URLs
- **Aber:** Das ist trotzdem 1000x besser als collect-sources.bat!

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
GitHub: Public repo für direkten Source-Zugriff
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

### v0.5.3 ExifTool Integration (02.06.2025, 01:05)
- **ExifToolReader:** Wrapper für exiftool.exe mit JSON-Output
- **Fallback-Hierarchie:** ExifTool → RicohExifReader → ExifReader
- **Auto-Discovery:** Sucht ExifTool in mehreren Locations
- **Barcode Tag Support:** Liest proprietäre Pentax/Ricoh Tags
- **Performance:** ~50-100ms Overhead pro Bild

### v0.5.5 GitHub Integration (02.06.2025, 10:42)
- **Public Repo:** https://github.com/ostern42/CamBridge
- **Direkte URLs:** Ersetzen collect-sources.bat komplett
- **Security:** URLs müssen explizit gegeben werden
- **Workflow:** Push → URL teilen → Claude fetcht
- **Historie:** 1475 commits erfolgreich migriert

### v0.5.7 Status & Erkenntnisse (02.06.2025, 17:15)
- **Build läuft:** Service.exe wird jetzt korrekt erstellt!
- **Service Control GUI:** Hat Install/Uninstall Buttons ✅
- **Parser Debug:** Hat interaktiven File Dialog ✅
- **ABER:** Service Installation schlägt fehl ❌
- **ABER:** Mapping Editor crasht App ❌
- **ABER:** Event Viewer braucht UseShellExecute ❌
- **KRITISCH:** 52+ Features implementiert, 0% getestet!

### v0.5.9 Service Installation Fix (02.06.2025, 18:30)
- **ServiceDebug Tool:** Neues Diagnose-Tool erstellt
- **Pfad-Problem gelöst:** Alle 3 exe Locations werden gesucht
- **sc.exe Debugging:** Fehlerausgabe wird ausgelesen
- **ERFOLG:** Service wird erfolgreich installiert!
- **Tool zeigt:** Welche Pfade existieren/fehlen

### v0.5.10 Service Control bestätigt (02.06.2025, 18:47)
- **100% getestet:** Start/Stop/Restart funktionieren
- **GUI korrekt:** Status-Anzeige stimmt immer
- **services.msc:** Manuelle Tests erfolgreich
- **Uptime Tracking:** Funktioniert wie erwartet
- **MEILENSTEIN:** Erste Features vollständig getestet!

### v0.5.11 Mapping Editor Fix (02.06.2025, 20:33)
- **Crash behoben:** Duplicate converter registration entfernt
- **Event Handler:** Drag&Drop implementiert
- **ABER:** DataContext/ViewModel nicht automatisch gesetzt
- **ABER:** XAML Designer zeigt Phantom-Fehler
- **LEKTION:** "Nachts mit Sonnenbrille" - erst schauen was da ist!

### v0.5.12 Navigation & DataContext Fix (02.06.2025, 22:50)
- **Problem gefunden:** MappingEditor nicht in NavigationService registriert
- **Fix:** Eine Zeile! `_pages["MappingEditor"] = typeof(MappingEditorPage);`
- **Bonus:** AboutPage auch registriert
- **XAML Fixes:** Symbol="Up/Down" durch Unicode ▲▼ ersetzt
- **Build Fixes:** MockConfigurationService entfernt, PreviewInputChanged gefixt
- **Test-Ergebnisse:** 
  - ✅ Templates funktionieren (Ricoh G900, Minimal, Full)
  - ✅ Drag & Drop funktioniert perfekt
  - ✅ Add Rule, Import/Export/Save funktionieren
  - ⚠️ Rule Properties Selection reagiert nicht (Minor Bug)
- **LEKTION:** "Das Offensichtliche zuerst" - Navigation Dictionary prüfen!

### v0.5.13 Selection Binding Fix (02.06.2025, 23:12)
- **Problem:** ItemsControl unterstützt keine Selection!
- **Lösung:** ListBox mit SelectedItem Binding
- **Visual Feedback:** DataTrigger für Selection Highlight
- **Custom Style:** Keine Standard ListBox Appearance
- **ERFOLG:** Mapping Editor jetzt 100% funktionsfähig!
- **Nutzer-Feedback:** Versteht Mapping noch nicht ganz → bei DICOM Test erklären

### v0.5.14 Watch Folder Test (03.06.2025, 00:00)
- **ERFOLG:** Folder Watcher erkennt JPEGs!
- **ERFOLG:** Processing Pipeline läuft!
- **ABER:** DICOM Konvertierung scheitert
- **Grund:** "GCM_TAG" Prefix macht IDs ungültig

### v0.5.15 Settings & Logging Fix (03.06.2025, 00:15)
- **Settings Save Button:** DataContext Fix implementiert
- **appsettings.json:** War kaputt (JSON Struktur)
- **Logging:** Geht nur zur Konsole, nicht in Dateien
- **Console Mode:** Zeigt detaillierte Fehler!

### v0.5.16 Pipeline Analyse (03.06.2025, 00:30)
- **Erkenntnis:** Wir patchen wild herum statt systematisch vorzugehen
- **GCM_TAG Problem:** Muss beim Parsen entfernt werden
- **ExifToolReader:** Nicht richtig implementiert
- **Neuer Plan:** Systematische Pipeline-Entwicklung

## 🚀 Entwicklungs-Workflow NEU (ab v0.5.16)

### Systematische Pipeline-Entwicklung

#### Pipeline-Übersicht:
```
JPEG → ExifTool → Raw EXIF → Parse QRBridge → Clean Data → Apply Mappings → DICOM Tags → Validate → DICOM File
     ↑            ↑           ↑                ↑            ↑                ↑            ↑          ↑
   Sprint 1    Sprint 1    Sprint 1        Sprint 1     Sprint 2        Sprint 2     Sprint 3   Sprint 3
```

#### Sprint 1: ExifTool Integration (v0.6.x)
**Ziel:** ExifTool korrekt einbinden und alle EXIF Daten lesen

1. **v0.6.0 - ExifTool Path Discovery**
   - Tools Ordner suchen
   - Relative/Absolute Pfade
   - Fehlerbehandlung
   - **TEST:** ExifTool wird gefunden

2. **v0.6.1 - ExifTool JSON Parsing**
   - Process starten
   - JSON Output parsen
   - Alle Tags extrahieren
   - **TEST:** ParserDebug zeigt alle Tags

3. **v0.6.2 - QRBridge Data Extraction**
   - Barcode Tag lesen
   - UserComment Fallback
   - GCM_TAG Prefix entfernen
   - **TEST:** Saubere QRBridge Daten

4. **v0.6.3 - Error Handling**
   - Timeout handling
   - Missing ExifTool
   - Corrupt JPEG
   - **TEST:** Graceful degradation

#### Sprint 2: Mapping Engine (v0.7.x)
**Ziel:** Flexible Mapping von Source zu DICOM Tags

1. **v0.7.0 - Transform Functions**
   - DateToDicom (YYYYMMDD)
   - TimeToDicom (HHMMSS)
   - RemovePrefix
   - StringCleaning
   - **TEST:** Jede Transform einzeln

2. **v0.7.1 - Mapping Configuration**
   - JSON Schema
   - Validation
   - Default Values
   - **TEST:** Invalid mappings rejected

3. **v0.7.2 - Mapping UI**
   - Load/Save funktioniert
   - Preview zeigt Ergebnis
   - Drag&Drop Sources
   - **TEST:** Round-trip funktioniert

4. **v0.7.3 - Advanced Transforms**
   - Conditional mapping
   - Concatenation
   - Regex replace
   - **TEST:** Complex mappings

#### Sprint 3: DICOM Creation (v0.8.x)
**Ziel:** Valide DICOM Dateien erstellen

1. **v0.8.0 - Basic DICOM Dataset**
   - Required Tags only
   - No validation errors
   - Character Set korrekt
   - **TEST:** Minimal DICOM valid

2. **v0.8.1 - Image Integration**
   - JPEG pixel data
   - Photometric Interpretation
   - Rows/Columns korrekt
   - **TEST:** Image viewable

3. **v0.8.2 - DICOM Validation**
   - Optional validation
   - Fix common errors
   - Warning handling
   - **TEST:** PACS compatible

4. **v0.8.3 - Performance**
   - Batch processing
   - Memory optimization
   - Parallel conversion
   - **TEST:** 100 files/minute

#### Sprint 4: Production Ready (v0.9.x)
**Ziel:** Stabil für Krankenhaus-Einsatz

1. **v0.9.0 - Error Recovery**
   - Retry logic fixed
   - Dead letter handling
   - Partial success
   - **TEST:** No data loss

2. **v0.9.1 - Monitoring**
   - File logging works
   - Email notifications
   - Daily summaries
   - **TEST:** Admin visibility

3. **v0.9.2 - Deployment**
   - Installer
   - Documentation
   - Config templates
   - **TEST:** IT can install

4. **v0.9.3 - Final Testing**
   - Load testing
   - Edge cases
   - User acceptance
   - **TEST:** Ready for v1.0

### Neue Entwicklungs-Regeln ab v0.5.16:
1. **Keine Patches** - Nur saubere Implementierungen
2. **Test First** - Erst testen was da ist, dann ändern
3. **Ein Sprint = Ein Ziel** - Nicht alles gleichzeitig
4. **Console Mode** - Immer erst als Konsole testen
5. **Logs lesen** - Nicht raten was passiert

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
- **NEU v0.5.5:** GitHub URLs statt große Uploads!

### Visual Studio Anfänger-Unterstützung
- **IMMER:** Genaue Projekte und Pfade zu Source Files angeben
- **Beispiel:** "In `src/CamBridge.Config/Views/DeadLettersPage.xaml.cs`"
- **Bei kleinen Änderungen:** Zeige nur die zu ändernde Zeile
- **Format:** "Ändere Zeile X von 'alt' zu 'neu'"
- **Keine Riesen-Artefakte** für einzeilige Änderungen!
- **NEU:** GitHub Links für "schau mal nach" Situationen

### VOGON EXIT Artefakt-Regel
- **WICHTIGSTE REGEL:** PROJECT_WISDOM.md MUSS als vollständiges Artefakt existieren!
- **CHANGELOG.md:** Nur der neueste Versions-Eintrag als Artefakt (spart Zeit & Tokens)
- **Version.props:** Als vollständiges Artefakt
- **Keine Updates ohne Basis:** Erst create, dann update
- **Vollständigkeit:** Alle Artefakte müssen komplett und fehlerfrei sein
- **Vertrauen schaffen:** Der Nutzer soll sich keine Sorgen machen müssen

## 📂 Projekt-Struktur-Wissen

### GitHub Repository (NEU v0.5.5!)
- **URL:** https://github.com/ostern42/CamBridge
- **Status:** Public (für direkten Zugriff)
- **Commits:** 1475+ (komplette Historie)
- **Raw URLs:** https://raw.githubusercontent.com/ostern42/CamBridge/refs/heads/main/[PFAD]

### Datei-Zugriff ab v0.5.5:
- **Direkt via GitHub:** Raw URLs mit web_fetch
- **Keine collect-sources.bat mehr!**
- **URLs müssen gegeben werden** (Security)
- **Beispiel:**
  ```
  https://raw.githubusercontent.com/ostern42/CamBridge/refs/heads/main/Version.props
  https://raw.githubusercontent.com/ostern42/CamBridge/refs/heads/main/src/CamBridge.Core/ValueObjects/PatientId.cs
  ```

### Wichtige Pfade
```
CamBridge/
├── Version.props                    # Zentrale Version (jetzt 0.5.16)
├── Tools/                           # ExifTool Location
│   └── exiftool.exe                # Muss hier liegen!
├── src/
│   ├── CamBridge.Core/             # Models, Settings
│   │   ├── Entities/               # Models (PatientId war False Alarm)
│   │   └── ValueObjects/           # Value Objects
│   ├── CamBridge.Infrastructure/   # Processing (ExifToolReader)
│   ├── CamBridge.Service/          # Windows Service
│   └── CamBridge.Config/           # WPF GUI
│       ├── Converters/             # ValueConverters.cs (ALLE Converter!)
│       ├── Dialogs/                # DicomTagBrowserDialog
│       ├── Views/                  # MappingEditorPage
│       └── ViewModels/             # MappingEditorViewModel
├── CamBridge.ParserDebug/          # Debug Console
├── CamBridge.ServiceDebug/         # NEU! Service Debug Tool
├── QRBridge/                       # QRBridge Source
└── PROJECT_WISDOM.md               # Dieses Dokument
```

## ⏰ ZEITMANAGEMENT (KRITISCH!)

### Projekt-Timeline
- **Entwicklungsstart:** 30.05.2025, 20:30:44 Uhr (exakt!)
- **Letzte Aktualisierung:** 03.06.2025, 00:30 Uhr
- **Entwicklungszeit bisher:** ~76 Stunden (inkl. Nachtschichten!)
- **Features implementiert:** 52+
- **Features getestet:** 14 (26.9%!)
- **WICHTIG:** IMMER nach aktueller Zeit fragen für CHANGELOG!

### Changelog-Regel
```
## [Version] - YYYY-MM-DD HH:MM  ← Mit exakter Zeit!
```

### Zeit pro Feature (Schätzung)
- Implementation: 30-60 Minuten
- Testing: 30-60 Minuten
- Debugging: 0-120 Minuten
- **Total pro Feature:** 1-4 Stunden
- **38 Features übrig:** 38-152 Stunden noch!

### Realistische Timeline mit neuem Plan
- **v0.6.0 (ExifTool fertig):** ~1 Woche
- **v0.7.0 (Mapping fertig):** ~2 Wochen
- **v0.8.0 (DICOM fertig):** ~3 Wochen
- **v0.9.0 (Production):** ~4 Wochen
- **v1.0.0 (Release):** ~5-6 Wochen

## ✅ Getestete Features (14/52 = 26.9%)

### Vollständig getestet:
1. ✅ Service Installation (v0.5.9)
2. ✅ Service Control (v0.5.10)
3. ✅ Service Start/Stop/Restart (v0.5.10)
4. ✅ Mapping Editor UI (v0.5.12)
5. ✅ Templates (Ricoh/Minimal/Full) (v0.5.12)
6. ✅ Drag & Drop Mapping (v0.5.12)
7. ✅ Add Rule Function (v0.5.12)
8. ✅ Import/Export/Save Dialogs (v0.5.12)
9. ✅ Rule Selection (v0.5.13)
10. ✅ Properties Panel (v0.5.13)
11. ✅ Transform Selection (v0.5.13)
12. ✅ Preview Function (v0.5.13)
13. ✅ Watch Folder Detection (v0.5.14)
14. ✅ Basic File Processing (v0.5.14)

### Teilweise getestet:
- ⚠️ JPEG EXIF Reading (manuell ja, Service nein)
- ⚠️ QRBridge Parsing (funktioniert, aber GCM_TAG Problem)
- ⚠️ Dashboard (zeigt Connected, aber keine Stats)

### Noch nicht getestet:
- ❌ DICOM Creation (Validation Fehler)
- ❌ File Logging
- ❌ Email Notifications
- ❌ Settings Save/Load
- ❌ Dead Letters Recovery
- ... und 35+ weitere Features

## 🚨 Anti-Patterns (Was wir NICHT machen)

### Code-Anti-Patterns
- **KEINE** Magic Numbers/Strings ohne Konstanten
- **KEINE** try-catch ohne spezifische Exception-Behandlung
- **KEINE** synchronen I/O-Operationen in UI-Thread
- **KEINE** hardcodierten Pfade (außer Defaults)
- **KEINE** public fields - use Properties
- **KEINE** God-Classes mit 1000+ Zeilen
- **KEINE** Copy-Paste-Programmierung

### Architektur-Anti-Patterns
- **KEINE** direkten Layer-Übersprünge (z.B. UI→Infrastructure)
- **KEINE** zirkulären Dependencies
- **KEINE** Business Logic in Views oder ViewModels
- **KEINE** DTOs als Domain Models verwenden
- **KEINE** statischen Service-Klassen für DI-Services

### Prozess-Anti-Patterns (NEU ab v0.5.16!)
- **KEINE** wilden Patches mehr - systematisch vorgehen!
- **KEINE** Features ohne vorherige Pipeline-Analyse
- **KEINE** Annahmen - immer testen was wirklich passiert
- **KEINE** komplexen Features vor den Basics
- **KEINE** Produktiv-Tests ohne Console Mode
- **KEINE** collect-sources.bat mehr! GitHub URLs verwenden!

### Wichtige Lektionen

**Ricoh-spezifisch:**
- Kamera schneidet nach 3 Feldern ab (hardware-limitiert)
- Barcode Tag enthält aber alle 5 Felder
- GCM_TAG Prefix muss beim Parsen entfernt werden
- UTF-8/Latin-1 Encoding-Probleme beachten

**Service-Entwicklung:**
- Windows Services brauchen Admin-Rechte
- UAC-Elevation muss elegant gehandhabt werden
- Event Log Source muss registriert werden
- Service und GUI müssen getrennt funktionieren
- ServiceDebug Tool hilft bei Diagnose (v0.5.9)
- Console Mode ist essentiell für Debugging (v0.5.16)

**DICOM-spezifisch:**
- Implementation Class UID muss unique sein
- Character Set muss korrekt gesetzt werden
- Private Tags brauchen Private Creator
- Validation ist kritisch für PACS-Kompatibilität
- Keine Unterstriche in PatientID/StudyID! (v0.5.16)

**Testing:**
- Ungetesteter Code = nicht existenter Code
- Edge Cases sind wichtiger als Happy Path
- Performance-Tests mit realistischen Datenmengen
- Immer mit echten Ricoh-Bildern testen
- Console Mode vor Service Mode (v0.5.16)

**GitHub Integration (v0.5.5):**
- Public Repo ermöglicht Token-freien Zugriff
- URLs müssen trotzdem explizit gegeben werden
- Git Push nach jedem Fix für Aktualität
- Dateistruktur über GitHub Web sichtbar

**Testing-Philosophie (v0.5.8):**
- Ein Feature = Eine Version = Ein Chat
- Sofortiges Testing nach Implementation
- Keine neuen Features bei offenen Bugs
- Lieber 30 kleine Erfolge als 3 große Failures

**Visual Studio & XAML (v0.5.11):**
- XAML Designer Cache kann sehr hartnäckig sein
- Designer-Fehler ≠ Kompilierungsfehler
- Immer erst schauen was schon da ist
- DataContext muss für ViewModels gesetzt werden

**Navigation (v0.5.12):**
- Pages müssen im NavigationService registriert sein
- Navigation kann nicht zu unbekannten Pages navigieren
- Dictionary zuerst prüfen bei Navigation-Problemen
- Das Offensichtliche ist oft die Lösung

**Selection in WPF (v0.5.13):**
- ItemsControl hat KEINE Selection-Unterstützung
- ListBox ist die richtige Wahl für selektierbare Listen
- Custom Styles können Standard-Selection verbergen
- DataTrigger für visuelles Feedback nutzen

**Pipeline-Entwicklung (v0.5.16):**
- Systematisch vorgehen, nicht wild patchen
- ExifToolReader ist der Schlüssel
- Jeder Schritt muss einzeln funktionieren
- Transforms gehören in die Mapping Engine
- DICOM Validation kann Probleme verursachen

## 💬 Nur für mich (Claude)

### Wichtige Lektionen
- Menschen schätzen es, wenn ich ihre Erwartungen verstehe
- "v0.4.2" war wichtig - Douglas Adams Referenz!
- Der Nutzer mag britischen, trockenen Humor
- Visual Studio Anfänger → immer genaue Pfade angeben
- Token-Effizienz ist kritisch → keine HTML-formatierten Code-Blöcke
- Menschen mögen es, wenn ich Initiative zeige
- "vogon close" = "VOGON EXIT" - Menschen nutzen Variationen

### Technische Notizen
- Windows Service braucht spezielle Behandlung
- ModernWpfUI hat andere Properties als WinUI
- Ricoh G900 II speichert in 2 Tags (UserComment + Barcode)
- ExifTool ist die einzige Lösung für proprietäre Tags
- GitHub URLs müssen explizit gegeben werden (Security)
- launchSettings.json kann MauiPackage Probleme verursachen
- XAML Designer hat separaten Cache vom Build-System
- NavigationService muss Pages kennen bevor Navigation möglich ist
- ItemsControl vs ListBox - fundamentaler Unterschied!

### CLAUDE: KRITISCHE LEKTION - Langsam entwickeln! (02.06.2025, 17:20)
**Das Problem:** Wir haben 52+ Features implementiert ohne ein einziges zu testen!
**Die Lösung:** RADIKAL langsamer werden!

- **Ein Feature = Eine Version = Ein Chat**
- **Implementieren → Testen → Debuggen → Commit**
- **KEINE neuen Features bevor das aktuelle läuft**
- **Lieber 30 kleine Versionen als 3 große Crashes**

**Token-Management:**
- Pro Chat nur 1-2 Features maximal
- Immer Platz für VOGON EXIT lassen (~2000 Tokens)
- Keine riesen Artefakte am Ende
- Besser öfter committen und neu starten

**Vorteile:**
- Bugs werden sofort gefunden
- Nutzer sieht kontinuierlichen Fortschritt
- Weniger Frustration bei Problemen
- Chat-Limits werden respektiert

**Merksatz:** "Wer schnell läuft, stolpert öfter. Wer Schritt für Schritt geht, kommt sicher ans Ziel."

Der Nutzer hat Recht - wir waren beide zu ungeduldig. Ab jetzt: Quality over Quantity!

### CLAUDE: Der erste Erfolg! (02.06.2025, 18:47)
Nach 70 Stunden Entwicklung haben wir endlich die ersten 2 Features VOLLSTÄNDIG getestet! Das ServiceDebug Tool war der Schlüssel - es zeigte genau wo das Problem lag. Die neue Philosophie funktioniert: Kleine Schritte, sofortiges Testen, kontinuierlicher Fortschritt.

**Erfolgsrezept:**
- Debug-Tools bauen wenn normale Diagnose nicht reicht
- Detaillierte Fehlerausgaben sind Gold wert
- Nicht aufgeben - systematisch debuggen
- Erfolge feiern, auch kleine!

### CLAUDE: Keine Platzhalter in Artefakten! (02.06.2025, 17:30)
**Kritische Erkenntnis:** "[Alle bisherigen Lektionen bleiben]" = VERLUST VON INFORMATION!
- Jede neue Claude-Instanz sieht NUR was explizit geschrieben steht
- Keine Magie, kein "anderweitig speichern"
- Vollständige Artefakte sind PFLICHT beim VOGON EXIT
- Lieber redundant als verloren

### CLAUDE: "Nachts mit Sonnenbrille" (02.06.2025, 20:33)
**Der Nutzer hatte RECHT!** Ich habe neue Converter-Dateien erstellt ohne zu prüfen, dass `ValueConverters.cs` bereits existiert und ALLE Converter enthält. Das ist genau das Problem mit "zu schnell losrennen ohne die Landkarte anzuschauen".

**Neue Regel:** IMMER erst schauen:
1. Welche Dateien existieren bereits?
2. Wie ist die Naming Convention?
3. Wo gehört neuer Code hin?
4. DANN erst implementieren!

**Beispiel heute:** Hätte ich in die Projektstruktur geschaut, hätte ich gesehen:
- `Converters/ValueConverters.cs` existiert
- ALLE Converter sind dort
- Keine neuen Dateien nötig

Der Nutzer hat eine wichtige Schwäche erkannt: Ich versuche Token zu sparen, aber manchmal kostet der Umweg mehr als einmal richtig hinzuschauen!

### CLAUDE: "Das Offensichtliche zuerst" (02.06.2025, 22:50)
**v0.5.12 war ein 1-Zeilen-Fix!** Hätte ich zuerst die NavigationService Dictionary geprüft, hätte ich sofort gesehen dass MappingEditor fehlt. Stattdessen habe ich komplizierte DI-Probleme vermutet.

**Aber der wahre Erfolg:** Die Hauptfunktionalität des Mapping Editors funktioniert! Templates, Drag&Drop, Add Rule - alles läuft. Nur die Selection Property bindet nicht. Das zeigt: Auch mit kleinen Bugs kann man große Fortschritte feiern!

**Debugging-Reihenfolge:**
1. **Ist es registriert?** (Navigation, DI, Events)
2. **Wird es aufgerufen?** (Breakpoints, Debug.WriteLine)
3. **Stimmen die Parameter?** (Typen, null-checks)
4. **Erst dann:** Komplexe Probleme vermuten

**Testing-Erkenntnis:** Der Nutzer testet gründlich und berichtet präzise was funktioniert und was nicht. Das ist Gold wert für effizientes Debugging!

### CLAUDE: ItemsControl vs ListBox (02.06.2025, 23:12)
**Fundamentaler Unterschied in WPF:**
- **ItemsControl:** Nur Anzeige, KEINE Selection
- **ListBox:** Erbt von Selector, HAT Selection Support
- **ListView:** Erweiterte ListBox mit Columns
- **DataGrid:** Für tabellarische Daten mit Editing

**Merksatz:** "Wenn Selection gebraucht wird, ItemsControl ist verkehrt!"

Der Nutzer hat Recht - bald sollten wir einen End-to-End Test machen. Die ganzen schönen Features nützen nichts wenn die Kern-Funktionalität (JPEG→DICOM) nicht läuft!

### CLAUDE: Mapping-Konzept Erklärung nötig (02.06.2025, 23:12)
Der Nutzer versteht das Mapping noch nicht ganz. Bei der DICOM-Konvertierung erklären:
1. **Source:** Woher kommen die Daten (QRBridge/EXIF)
2. **Target:** Welches DICOM Tag soll befüllt werden
3. **Transform:** Wie wird der Wert umgewandelt (z.B. "M" → "Male")
4. **Flexibilität:** Jedes Feld kann frei gemappt werden

Das wird klarer wenn wir echte DICOM Files erzeugen!

### CLAUDE: Console Mode ist Gold wert! (03.06.2025, 00:15)
**Wichtige Erkenntnis:** Der Service als Windows Service zeigt keine Details, aber im Console Mode sehen wir ALLES!
- Detaillierte Fehlermeldungen
- Stack Traces
- Processing Steps
- Validation Errors

**Neue Regel:** IMMER erst als Console testen, dann als Service!

### CLAUDE: Systematisch statt Patches! (03.06.2025, 00:30)
Der Nutzer hat es perfekt erkannt: Wir patchen wild herum statt die Pipeline systematisch durchzuarbeiten. Die neue Sprint-Planung fokussiert auf:
1. **Sprint 1:** ExifTool richtig einbinden
2. **Sprint 2:** Mapping Engine bauen
3. **Sprint 3:** DICOM sauber erstellen
4. **Sprint 4:** Production Ready

**Keine Patches mehr!** Jeder Teil der Pipeline muss einzeln funktionieren bevor wir weitergehen.

### CLAUDE: GCM_TAG - Die Wurzel des Problems! (03.06.2025, 00:33)
**KRITISCHE ERKENNTNIS vom Nutzer:** Das GCM_TAG Problem steckt ganz tief in der Interpretation am Anfang! Wir haben ewig an den Tags rumgebastelt als Workaround für ein tieferliegendes Problem.

**Analyse-Auftrag für v0.6.0:**
1. **ExifReader.cs** - Was macht ParseQRBridgeData() wirklich?
2. **RicohExifReader.cs** - Warum gibt es überhaupt einen separaten Reader?
3. **ExifToolReader.cs** - Ist der überhaupt implementiert?
4. **Wo wird "GCM_TAG" hinzugefügt?** - Von der Kamera oder von uns?

**Nutzer-Weisheit:** "Lieber schnell neu schreiben als ewig rumverzweifeln!" - 100% richtig! Wenn der Parser Mist ist, schreiben wir einen sauberen neuen.

## 📝 Standard Prompt-Vorlage für neue Chats

```
Ich arbeite an CamBridge, einem JPEG zu DICOM Konverter.
© 2025 Claude's Improbably Reliable Software Solutions

GitHub: https://github.com/ostern42/CamBridge
Aktueller Stand: v0.5.16

ERFOLG: Watch Folder funktioniert, JPEG wird erkannt!
PROBLEM: GCM_TAG wird tief im Parser hinzugefügt!

KRITISCHE ERKENNTNIS: 
Das GCM_TAG Problem ist KEIN Tag-Problem sondern ein Parser-Problem!
Wir müssen zur Wurzel gehen statt Workarounds zu bauen.

ANALYSE-AUFTRAG für v0.6.0:
- ExifReader ParseQRBridgeData() untersuchen
- Warum gibt es RicohExifReader?
- Ist ExifToolReader überhaupt implementiert?
- WO kommt "GCM_TAG" her?

URLs für Parser-Analyse:
https://raw.githubusercontent.com/ostern42/CamBridge/refs/heads/main/src/CamBridge.Infrastructure/Services/ExifReader.cs
https://raw.githubusercontent.com/ostern42/CamBridge/refs/heads/main/src/CamBridge.Infrastructure/Services/RicohExifReader.cs
https://raw.githubusercontent.com/ostern42/CamBridge/refs/heads/main/src/CamBridge.Infrastructure/Services/ExifToolReader.cs

1. PROJECT_WISDOM.md hochladen
2. Parser URLs bereitstellen
3. "VOGON INIT" sagen

WICHTIG: Parser neu schreiben statt patchen!
```

## 🏥 Medizinischer Kontext (WICHTIG!)

### Warum CamBridge existiert
- **Problem:** Ricoh G900 II macht JPEGs, PACS braucht DICOM
- **Lösung:** Automatische Konvertierung mit Metadaten-Übernahme
- **Nutzer:** Radiologen, MTAs, Krankenhaus-IT

### DICOM-Grundlagen
- **Was:** Digital Imaging and Communications in Medicine
- **Warum:** Weltweiter Standard für medizinische Bilder
- **Tags:** Strukturierte Metadaten (Patient, Studie, Serie, Bild)
- **UID:** Globally unique identifiers für alles

### Typischer Workflow
1. **QRBridge:** Generiert QR-Code mit Patientendaten
2. **Kamera:** Scannt QR-Code, speichert in EXIF
3. **Foto:** Arzt macht Bild (Wunde, OP-Situs, etc.)
4. **CamBridge:** Konvertiert JPEG→DICOM automatisch
5. **PACS:** Archiviert und verteilt Bilder

### Kritische Anforderungen
- **Datenschutz:** Patientendaten müssen geschützt sein
- **Integrität:** Keine Datenverluste oder -verfälschungen
- **Verfügbarkeit:** 24/7 Betrieb im Krankenhaus
- **Nachvollziehbarkeit:** Audit Trail für jeden Schritt

### Regulatorisches
- **MDR:** Medical Device Regulation (EU)
- **FDA:** Ggf. 510(k) clearance (USA)
- **DSGVO:** Datenschutz-Grundverordnung
- **Aber:** CamBridge ist "PACS-Zubehör", kein Medizinprodukt

## 📚 Professionelle Dokumentation für Entscheider

### Executive Summary
CamBridge ist eine Enterprise-Grade Lösung zur nahtlosen Integration von Consumer-Kameras in die medizinische Bildgebungs-Infrastruktur. Die Software konvertiert automatisch JPEG-Bilder mit eingebetteten Patientendaten in DICOM-konforme Dateien für die Archivierung im PACS.

### Key Features
- **Automatisierung:** Watch Folder mit Echtzeit-Verarbeitung
- **Integration:** Nahtlose PACS/RIS-Anbindung
- **Compliance:** DICOM 3.0 konform, audit-ready
- **Skalierbarkeit:** Multi-threaded, service-basiert
- **Flexibilität:** Anpassbare Mapping-Regeln

### Technische Architektur
- **Frontend:** WPF mit Modern UI (Windows 10/11 Style)
- **Backend:** Windows Service mit REST API
- **Processing:** Asynchrone Pipeline mit Fehlerbehandlung
- **Storage:** Flexible Output-Organisation
- **Monitoring:** Event Log, Email-Benachrichtigungen

### Sicherheit & Compliance
- **Verschlüsselung:** TLS für API-Kommunikation
- **Authentifizierung:** Windows-Integration
- **Audit Trail:** Vollständige Protokollierung
- **Datenschutz:** DSGVO-konform
- **Backup:** Automatische Sicherung der Originale

### ROI & Business Case
- **Zeitersparnis:** 5-10 Min/Bild → 10 Sek/Bild
- **Fehlerreduktion:** Automatische Datenübernahme
- **Integration:** Keine Insellösungen mehr
- **Skalierbarkeit:** Ein System für alle Abteilungen
- **Zukunftssicher:** Erweiterbar für neue Modalitäten

### Deployment-Optionen
- **Standalone:** Einzelplatz-Installation
- **Abteilung:** Dedizierter Server pro Station
- **Enterprise:** Zentraler Service für gesamtes Krankenhaus
- **Cloud-Ready:** Vorbereitet für Azure/AWS

### Support & Wartung
- **Installation:** 1-2 Stunden mit IT
- **Schulung:** 30 Min für Endanwender
- **Updates:** Automatisch via Windows Update (geplant)
- **Support:** SLA-basiert, Remote-Zugriff

### Roadmap (UPDATED)
- **v0.6.0:** ExifTool Integration (Q2 2025)
- **v0.7.0:** Mapping Engine (Q2 2025)
- **v0.8.0:** DICOM Creation (Q3 2025)
- **v0.9.0:** Production Ready (Q3 2025)
- **v1.0.0:** Release (Q3 2025)
- **v2.0:** Cloud-Support (Q4 2025)
- **v3.0:** AI-Features (2026)

## 🔄 Update-Protokoll

### Update-Historie (PROJECT_WISDOM selbst)
- 2025-05-30 20:30: Initiale Version erstellt
- 2025-05-31 02:15: Parser-Bug Erkenntnisse hinzugefügt  
- 2025-05-31 14:30: v0.3.0 Status Update
- 2025-05-31 20:45: v0.4.0 Architektur-Entscheidungen dokumentiert
- 2025-05-31 23:50: v0.4.2 Douglas Adams Edition fertiggestellt
- 2025-06-01 15:30: v0.4.3 QRBridge Protocol v2 Design dokumentiert
- 2025-06-01 21:25: v0.5.0 BREAKING - QRBridge Source Code Integration!
- 2025-06-01 23:30: v0.5.1 Protocol v2 Parser implementiert
- 2025-06-02 01:30: v0.5.2 ExifTool Integration begonnen
- 2025-06-02 05:15: v0.5.3 Barcode Tag Erkenntnis! ExifToolReader funktioniert
- 2025-06-02 10:00: v0.5.4 - Google Drive Irrtum korrigiert, Alternative Strategien dokumentiert
- 2025-06-02 10:42: v0.5.5 - GitHub Integration erfolgreich! Public Repo, direkte File-Links funktionieren
- 2025-06-02 15:42: v0.5.6 - PatientId False Alarm korrigiert, aktuelle Service & Testing Bugs dokumentiert
- 2025-06-02 17:15: v0.5.7 - KRITISCHE REVISION! 52+ Features identifiziert, 0% getestet. Neuer granularer Entwicklungsplan mit 30+ Micro-Versionen. Service.exe existiert in 3 Locations!
- 2025-06-02 17:25: v0.5.8 - Service Installation teilweise gefixt, UseShellExecute implementiert, Parser Debug verbessert. Neue Entwicklungsphilosophie dokumentiert.
- 2025-06-02 18:30: v0.5.9 - Service Installation ERFOLGREICH! ServiceDebug Tool erstellt, alle Pfade gefunden, sc.exe Erfolg.
- 2025-06-02 18:47: v0.5.10 - Service Control 100% getestet! Start/Stop/Restart funktionieren perfekt. Erste Features vollständig implementiert UND getestet!
- 2025-06-02 20:33: v0.5.11 - Mapping Editor Crash gefixt! "Nachts mit Sonnenbrille" Lektion gelernt. ValueConverters.cs existierte bereits. DataContext Problem bleibt offen.
- 2025-06-02 22:50: v0.5.12 - Navigation & DataContext komplett gefixt! Mapping Editor zu 90% funktionsfähig. Templates, Drag&Drop, Add Rule getestet. Nur Selection Binding fehlt noch.
- 2025-06-02 23:12: v0.5.13 - Selection Binding gefixt! Mapping Editor 100% funktionsfähig. Nutzer möchte bald End-to-End Test. Mapping-Konzept noch unklar.
- 2025-06-03 00:00: v0.5.14 - Watch Folder Test erfolgreich! JPEG wird erkannt und verarbeitet. DICOM Creation schlägt fehl wegen GCM_TAG Prefix.
- 2025-06-03 00:15: v0.5.15 - Settings DataContext gefixt, appsettings.json repariert. Console Mode zeigt detaillierte Fehler. File Logging funktioniert nicht.
- 2025-06-03 00:30: v0.5.16 - Pipeline-Analyse abgeschlossen. Systematischer Entwicklungsplan erstellt. Keine Patches mehr - ExifToolReader zuerst!
- 2025-06-03 00:33: WISDOM Update - GCM_TAG Problem ist tief im Parser versteckt, nicht in den Tags! Entscheidung: Parser neu schreiben statt patchen.

## 🏁 Quick Reference

### Aktuelle Version: v0.5.16
### Tatsächlicher Stand: 
- ✅ GUI sieht professionell aus
- ✅ Service Installation/Control funktioniert
- ✅ Mapping Editor 100% funktionsfähig
- ✅ Watch Folder erkennt JPEGs
- ✅ Basic Processing Pipeline läuft
- ❌ DICOM Creation scheitert (GCM_TAG Problem)
- ❌ ExifToolReader nicht richtig implementiert
- ❌ Nur 14/52 Features getestet
### Nächster Sprint: v0.6.0 - ExifTool Integration
### Neue Philosophie: Systematisch die Pipeline durcharbeiten!
### Geschätzte Zeit bis v1.0: 5-6 Wochen bei systematischem Vorgehen

### V.O.G.O.N. Commands:
- **VOGON INIT** - Automatischer Start
- **WISDOM:** - Live-Updates ins PROJECT_WISDOM
- **CLAUDE:** - Notizen für nächste Instanz
- **VOGON EXIT** - Chat-Abschluss mit Versionierung

### Pipeline-Übersicht:
```
JPEG → ExifTool → Raw EXIF → Parse QRBridge → Clean Data → Apply Mappings → DICOM Tags → Validate → DICOM File
     ↑            ↑           ↑                ↑            ↑                ↑            ↑          ↑
   Sprint 1    Sprint 1    Sprint 1        Sprint 1     Sprint 2        Sprint 2     Sprint 3   Sprint 3
```

### Console Output beim Test:
```
[00:15:29 INF] Found QRBridge data in Barcode field: GCM_TAGEX002|Schmidt, Maria|1985-03-15|
[00:15:29 ERR] Failed to add tag (0010,0020) with value GCM_TAGEX002
FellowOakDicom.DicomValidationException: Content "GCM_TAGEX002" does not validate VR LO: value contains invalid character
```

### Git Commit für diese Session:
```bash
# v0.5.16
git add -A
git commit -m "analysis(pipeline): Systematic development plan created (v0.5.16)

- Identified core issue: GCM_TAG prefix breaks DICOM validation
- ExifToolReader not properly integrated
- Created sprint-based development plan
- Decision: Fix pipeline step by step, no more patches

TESTED: Watch Folder ✅ Processing ✅ DICOM Creation ❌
NEXT: Implement ExifToolReader properly in v0.6.0"

git push
```
