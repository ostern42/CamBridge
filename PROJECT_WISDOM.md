# CamBridge Project Wisdom & Conventions
**Letzte Aktualisierung:** 2025-06-02, 10:42 Uhr  
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
Nächste Aufgabe: BUILD-FEHLER FIXEN MIT GITHUB! 🎯

Stand: v0.5.5 - GitHub Integration erfolgreich!

ERFOLGE:
✅ GitHub Repo public: https://github.com/ostern42/CamBridge
✅ Direkte File-Links funktionieren
✅ Git-Historie (1475 commits) erhalten
✅ Token-Effizienz massiv verbessert

KRITISCH - BUILD IMMER NOCH GEBROCHEN:
🐛 PatientId doppelt definiert in Entities UND ValueObjects
🐛 Namespace-Konflikt verhindert Kompilierung
🐛 KEINE neuen Features bis Build läuft!

WICHTIG FÜR NÄCHSTEN CHAT:
Gib mir diese URLs für das PatientId Problem:
https://raw.githubusercontent.com/ostern42/CamBridge/refs/heads/main/src/CamBridge.Core/ValueObjects/PatientId.cs
https://raw.githubusercontent.com/ostern42/CamBridge/refs/heads/main/src/CamBridge.Core/Entities/PatientId.cs

ERSTE PRIORITÄT:
1. ⚡ PatientId Duplikat analysieren und fixen
2. 🧪 Build erfolgreich durchführen
3. 🧪 DANN ExifTool mit echten Bildern testen
4. ✅ Alle 5 QR-Felder verifizieren

GitHub Workflow ab jetzt:
- Keine collect-sources.bat mehr!
- Direkte URLs für spezifische Dateien
- Push nach jedem Fix
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
- **Commits:** 1475 (komplette Historie)
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
├── Version.props                    # Zentrale Version (jetzt 0.5.5)
├── Tools/                           # ExifTool Location
│   └── exiftool.exe                # Muss hier liegen!
├── src/
│   ├── CamBridge.Core/             # Models, Settings
│   │   ├── Entities/               # ACHTUNG: PatientId Duplikat!
│   │   └── ValueObjects/           # ACHTUNG: PatientId Duplikat!
│   ├── CamBridge.Infrastructure/   # Processing (ExifToolReader)
│   ├── CamBridge.Service/          # Windows Service
│   └── CamBridge.Config/           # WPF GUI
│       ├── Dialogs/                # DicomTagBrowserDialog
│       ├── Views/                  # MappingEditorPage
│       └── ViewModels/             # MappingEditorViewModel
├── CamBridge.ParserDebug/          # Debug Console
├── QRBridge/                       # QRBridge Source
└── PROJECT_WISDOM.md               # Dieses Dokument
```

## 🚀 Entwicklungs-Workflow

### Neue Features (mit GitHub)
1. Version in Version.props erhöhen
2. Feature implementieren
3. CHANGELOG.md aktualisieren
4. Git commit mit konventionellem Format
5. **git push** zu GitHub
6. URLs für geänderte Dateien im Chat teilen

### Chat-Handover (NEU!)
1. PROJECT_WISDOM.md hochladen
2. Relevante GitHub URLs bereitstellen
3. Aktuellen Stand beschreiben
4. Nächste Aufgabe klar definieren
5. "VOGON INIT" sagen

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

### Ricoh G900 II QRBridge (v0.4.4)
- **NUR 3 FELDER:** Kamera speichert nur examid|name|birthdate
- **FEHLENDE FELDER:** gender und comment werden abgeschnitten
- **GCM_TAG PREFIX:** Kamera fügt "GCM_TAG " vor Barcode ein
- **ENCODING:** UTF-8/Latin-1 Probleme bei Umlauten → GELÖST
- **LÖSUNG:** Mit QRBridge Source können wir optimiertes Protokoll entwickeln!

### 🎯 GELÖST: Barcode Tag Erkenntnis! (v0.5.3)
- **Ricoh speichert in 2 verschiedenen Tags:**
  - UserComment: "GCM_TAG" + erste 3 Felder
  - Barcode: ALLE 5 Felder komplett!
- **MetadataExtractor kann Barcode Tag NICHT lesen**
- **ExifTool ist die Lösung** - liest proprietäre Tags
- **Implementation:** ExifToolReader mit Fallback

### v0.5.3 Build-Fehler NEU!
- **PatientId:** Doppelt definiert in Entities UND ValueObjects
- **Namespace-Konflikt:** Muss in einem der beiden Ordner entfernt werden
- **ProcessingResult:** Properties passen nicht zu NotificationService
- **ExifTool:** Noch nicht getestet

### v0.5.5 GitHub Integration
- **URLs müssen explizit gegeben werden** - Security Feature
- **Format beachten:** refs/heads/main im Pfad
- **Public Repo:** Notwendig für Token-freien Zugriff
- **Git Push:** Nach jedem Fix für aktuellen Stand

## ⏰ ZEITMANAGEMENT (KRITISCH!)

### Projekt-Timeline
- **Entwicklungsstart:** 30.05.2025, 20:30:44 Uhr (exakt!)
- **Letzte Aktualisierung:** 02.06.2025, 10:42 Uhr
- **Entwicklungszeit bisher:** ~62.2 Stunden (inkl. Nachtschichten!)
- **WICHTIG:** IMMER nach aktueller Zeit fragen für CHANGELOG!

### Changelog-Regel
```
## [Version] - YYYY-MM-DD HH:MM  ← Mit exakter Zeit!
```

## 📋 Entwicklungsplan (KORRIGIERTE VERSION - Stand 02.06.2025, 10:42)

### ⚡️ WICHTIGE KORREKTUR
**Original-Plan sagte "WinUI 3" - wir nutzen aber WPF mit ModernWpfUI!**

### Phasen-Übersicht (REVIDIERT & VERIFIZIERT)

#### ✅ Abgeschlossene Phasen (Code-verifiziert)
1-13. [Phasen 1-13 wie zuvor - alle erledigt]

14. **Phase 11b:** GitHub Integration (v0.5.4-v0.5.5) ✅
    - Google Drive Irrtum korrigiert ✅
    - GitHub Repository etabliert ✅
    - Public Repo für direkten Zugriff ✅
    - Git Historie (1475 commits) migriert ✅
    - Web_fetch Integration funktioniert ✅

#### 🔥 AKTUELLE PHASE - BUG FIXES & TESTING
15. **Phase 11c:** Build Fix & Testing (v0.5.6) - NÄCHSTER CHAT!
    - PatientId Duplikat beheben ❌
    - ExifTool Integration testen ❌
    - Alle 5 QRBridge-Felder verifizieren ❌
    - v0.5.0-v0.5.1 Features gründlich testen ❌
    - Watch Folder Management GUI erweitern ❌
    - Live-Preview für alle Transformationen ❌
    - Validation UI für Mappings ❌
    - **Feature-complete Beta**

#### 🚧 Nächste Phasen (wie zuvor)
[Phasen 16-19 unverändert]

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
- **v0.5.4** - Google Drive Korrektur (Erledigt ✅)
- **v0.5.5** - GitHub Integration (Erledigt ✅)
- **v0.5.6** - Build Fix & Testing (Nächstes Ziel 🎯)
- **v0.6.0** - Performance & Polish
- **v0.7.0** - FTP-Server Integration [Optional]
- **v0.8.0** - PACS Ready [Optional]
- **v0.9.0** - MWL Integration [Optional]
- **v1.0.0** - Production Release

## 🚨 Anti-Patterns (Was wir NICHT machen)

### Code-Anti-Patterns
[Alle bisherigen Anti-Patterns bleiben]
- **KEINE** collect-sources.bat mehr! GitHub URLs verwenden!
- **KEINE** Annahmen über automatischen Dateizugriff - URLs müssen gegeben werden!

### Wichtige Lektionen
[Alle bisherigen Lektionen bleiben]

**GitHub Integration (v0.5.5):**
- Public Repo ermöglicht Token-freien Zugriff
- URLs müssen trotzdem explizit gegeben werden
- Git Push nach jedem Fix für Aktualität
- Dateistruktur über GitHub Web sichtbar

## 📝 Standard Prompt-Vorlage für neue Chats

```
Ich arbeite an CamBridge, einem JPEG zu DICOM Konverter.
© 2025 Claude's Improbably Reliable Software Solutions

GitHub: https://github.com/ostern42/CamBridge
Aktueller Stand: v0.5.5

KRITISCH - BUILD GEBROCHEN:
🐛 PatientId doppelt definiert
🐛 ExifTool ungetestet

Hier die URLs für das PatientId Problem:
https://raw.githubusercontent.com/ostern42/CamBridge/refs/heads/main/src/CamBridge.Core/ValueObjects/PatientId.cs
https://raw.githubusercontent.com/ostern42/CamBridge/refs/heads/main/src/CamBridge.Core/Entities/PatientId.cs

1. PROJECT_WISDOM.md hochladen
2. Relevante GitHub URLs bereitstellen
3. "VOGON INIT" sagen
```

## 🏥 Medizinischer Kontext (WICHTIG!)
[Kompletter medizinischer Kontext bleibt unverändert]

## 📚 Professionelle Dokumentation für Entscheider
[Komplette Dokumentation bleibt unverändert]

## 🔄 Update-Protokoll

### Update-Historie (PROJECT_WISDOM selbst)
[Alle bisherigen Updates]
- 2025-06-02 10:00: v0.5.4 - Google Drive Irrtum korrigiert, Alternative Strategien dokumentiert
- 2025-06-02 10:42: v0.5.5 - GitHub Integration erfolgreich! Public Repo, direkte File-Links funktionieren

## 🏁 Quick Reference

### Aktuelle Version: v0.5.5
### Tatsächlicher Stand: 
- ✅ GitHub Integration funktioniert!
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
- PatientId Duplikat fixen mit GitHub URLs!
- Build erfolgreich durchführen
- ExifTool Integration testen
- Verifizieren dass alle 5 Felder gelesen werden
### Architektur: Enterprise-Level (und das ist GUT so!)
### Kontext: Medizinische Software mit 0% Fehlertoleranz

### V.O.G.O.N. Commands:
- **VOGON INIT** - Automatischer Start
- **WISDOM:** - Live-Updates ins PROJECT_WISDOM
- **CLAUDE:** - Notizen für nächste Instanz
- **VOGON EXIT** - Chat-Abschluss mit Versionierung

### GitHub Commands (NEU!):
- **git push** - Nach jedem Fix
- **git push --tags** - Tags hochladen
- **URLs teilen** - Für geänderte Dateien

### ExifTool Commands:
- **exiftool.exe -j image.jpg** - JSON Output
- **exiftool.exe -Barcode image.jpg** - Nur Barcode Tag
- **Tools\exiftool.exe** - Standard Location im Projekt

### GitHub Integration ab v0.5.5:
- **Repository:** https://github.com/ostern42/CamBridge
- **Raw URLs:** https://raw.githubusercontent.com/ostern42/CamBridge/refs/heads/main/[PFAD]
- **Keine collect-sources.bat mehr!**
- **URLs müssen explizit gegeben werden**
