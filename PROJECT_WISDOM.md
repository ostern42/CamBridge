# CamBridge Project Wisdom & Conventions
**Letzte Aktualisierung:** 2025-06-03, 18:00 Uhr  
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

### 🎯 GitHub Integration - FUNKTIONIERT!
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

### 🏁 "VOGON EXIT" - Chat-Abschluss
**KRITISCHE REGEL:** Beim VOGON EXIT MÜSSEN IMMER erstellt werden:
1. **PROJECT_WISDOM.md** - Als VOLLSTÄNDIGES Artefakt
2. **CHANGELOG.md** - NUR der neueste Versions-Eintrag als Artefakt
3. **Version.props** - Als VOLLSTÄNDIGES Artefakt

**WARUM:** Updates können fehlschlagen. Nur vollständige Artefakte garantieren, dass der Nutzer die aktualisierten Dateien bekommt!

## 🎯 AKTUELLER ENTWICKLUNGSFAHRPLAN (PROMINENT!)

### 📍 WIR SIND HIER: v0.5.19 - ExifTool Pipeline Implementation
**Status:** Pipeline theoretisch implementiert, praktische Umsetzung steht aus

**Was wurde gemacht:**
- ✅ Alte ExifReader gelöscht (ExifReader, RicohExifReader, CompositeExifReader)
- ✅ Neuer ExifToolReader designed (nutzt bestehende ImageMetadata)
- ✅ FileProcessor angepasst
- ❌ Code noch nicht getestet

**NÄCHSTER SCHRITT für neuen Chat:**
1. ExifToolReader.cs in Services-Ordner implementieren
2. FileProcessor.cs updaten
3. DI-Registrierung anpassen
4. Mit ParserDebug.exe testen
5. Service im Console-Mode testen

### 🚀 Entwicklungsfahrplan bis v1.0

#### Sprint 1: ExifTool Integration (v0.5.x) ← CURRENT
- ✅ Pipeline analysiert und vereinfacht
- [ ] **v0.5.19: Neue Pipeline implementieren & testen**
- [ ] Edge Cases & Stabilisierung

#### Sprint 2: Mapping Engine (v0.6.x)
**Ziel:** Flexible Mapping von Source zu DICOM Tags
- Transform Functions (DateToDicom, etc.)
- Validation & UI Integration
- Advanced Transforms

#### Sprint 3: DICOM Creation (v0.7.x)
**Ziel:** Valide DICOM Dateien erstellen
- Minimal valid DICOM
- Image Integration
- PACS Compatibility
- Batch Processing

#### Sprint 4: Production Ready (v0.8.x)
**Ziel:** Stabil für Krankenhaus-Einsatz
- Logging & Monitoring
- Email Notifications
- Installer & Documentation
- Load Testing

#### Release: v1.0.0 (Ziel: Ende Juli 2025)

### 📋 Aktueller Übergabeprompt
```
🔧 v0.5.19 - ExifTool Pipeline theoretisch implementiert

STATUS:
✅ Alte ExifReader gelöscht  
✅ Neuer ExifToolReader designed
✅ FileProcessor Anpassungen geplant
❌ Implementation & Tests ausstehend

NÄCHSTE AUFGABE:
1. ExifToolReader.cs implementieren (Code aus vorherigem Chat)
2. FileProcessor.cs updaten
3. ServiceCollectionExtensions.cs anpassen
4. Mit echtem Ricoh-Bild testen!

GitHub URLs wenn nötig:
https://raw.githubusercontent.com/ostern42/CamBridge/refs/heads/main/src/CamBridge.Infrastructure/Services/

Hauptziel: Pipeline zum Laufen bringen!
```

## 🎯 Projekt-Identität
- **Copyright:** © 2025 Claude's Improbably Reliable Software Solutions
- **Produkt:** CamBridge - JPEG zu DICOM Konverter für Ricoh G900 II Kameras
- **Kontext:** Medizinische Bildgebung, arbeitet mit QRBridge zusammen
- **NEU:** Wir kontrollieren BEIDE Seiten (QRBridge + CamBridge)!
- **GitHub:** https://github.com/ostern42/CamBridge

## 🔥 BREAKING: QRBridge Integration

### Wir haben den QRBridge Source Code!
Das bedeutet:
- **Volle Kontrolle** über QR-Code Generierung UND Dekodierung
- **Protokoll-Evolution** möglich - nicht auf Pipes festgelegt!
- **Optimierung** für Ricoh-Limitierungen (nur 3-4 Felder)

### QRBridge bleibt unverändert!
- **KEIN v2 Encoder** - unnötige Komplexität
- **Parser-Bug wird in CamBridge gefixt**
- **Pipes funktionieren** - warum ändern?

### 🔍 KRITISCHE ERKENNTNIS: Barcode Tag!
- **Ricoh speichert ALLE 5 Felder** im proprietären "Barcode" EXIF-Tag
- **UserComment enthält nur** "GCM_TAG" als Marker
- **ExifTool ist die einzige Lösung** für vollständige Daten

## ✅ GitHub Integration - ERFOLGREICH!

### Der Durchbruch!
- ✅ Public Repository: https://github.com/ostern42/CamBridge
- ✅ Direkte Raw-URLs funktionieren mit web_fetch
- ✅ 70% Token-Ersparnis durch gezieltes Fetching
- ✅ Kein collect-sources.bat mehr nötig!

### Workflow:
1. **URLs müssen explizit gegeben werden** (Sicherheitsfeature)
2. **Format:** `https://raw.githubusercontent.com/ostern42/CamBridge/refs/heads/main/[PFAD]`
3. **Einmal autorisiert** kann ich weitere Dateien im gleichen Pfad holen

## 📝 Wichtige Konventionen

### Code-Konventionen
- **Kommentare:** IMMER in Englisch
- **Namespaces:** CamBridge.{Layer} (Core, Infrastructure, Service, Config)
- **Async:** Suffix "Async" für alle async Methoden
- **Interfaces:** Prefix "I" (IRepository, IService)

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

### Pipeline radikal vereinfachen! (NEU 03.06.2025)
**KRITISCHE ERKENNTNIS:** Nur noch ExifTool! Keine Fallbacks, keine drei verschiedenen Reader!

**Neue Pipeline:**
- ExifToolService (einzige EXIF-Lösung)
- ImageMetadata (Domain Object)
- DICOM Converter

If ExifTool is not available, processing cannot continue. Period.

## 🔧 Technische Details

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

### Kritische Erkenntnisse
- **GCM_TAG:** Hat ZWEI Varianten (mit und ohne Space)
- **Ricoh G900 II:** Schneidet nach 3 Feldern ab (Hardware-Limit)
- **Barcode Tag:** Enthält aber alle 5 Felder
- **ExifTool:** Einzige Lösung für proprietäre Tags

## 🚀 Entwicklungs-Workflow

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

## 💬 Kommunikations-Präferenzen

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

## 📂 Projekt-Struktur-Wissen

### GitHub Repository
- **URL:** https://github.com/ostern42/CamBridge
- **Status:** Public (für direkten Zugriff)
- **Raw URLs:** https://raw.githubusercontent.com/ostern42/CamBridge/refs/heads/main/[PFAD]

### Wichtige Ordner
```
CamBridge/
├── Version.props                    # Zentrale Version (jetzt 0.5.19)
├── Tools/                           # ExifTool Location
│   └── exiftool.exe                # MUSS hier liegen!
├── src/
│   ├── CamBridge.Core/             # Entities, Interfaces
│   ├── CamBridge.Infrastructure/   # Services
│   ├── CamBridge.Service/          # Windows Service
│   └── CamBridge.Config/           # WPF GUI
```

## ⏰ ZEITMANAGEMENT (KRITISCH!)

### Projekt-Timeline
- **Entwicklungsstart:** 30.05.2025, 20:30:44 Uhr (exakt!)
- **Letzte Aktualisierung:** 03.06.2025, 18:00 Uhr
- **Features implementiert:** 60+
- **Features getestet:** ~27%
- **WICHTIG:** IMMER nach aktueller Zeit fragen für CHANGELOG!

### Zeit pro Feature (Schätzung)
- Implementation: 30-60 Minuten
- Testing: 30-60 Minuten
- Debugging: 0-120 Minuten
- **Total pro Feature:** 1-4 Stunden

## ✅ Getestete Features (Status v0.5.18)

### Vollständig getestet:
1. ✅ Service Installation/Control
2. ✅ Mapping Editor UI (komplett)
3. ✅ Watch Folder Detection
4. ✅ Basic File Processing
5. ✅ Core/Infrastructure Build

### Noch zu testen:
- ❌ Neue ExifTool Pipeline
- ❌ DICOM Creation (Validation Fehler)
- ❌ File Logging
- ❌ Email Notifications
- ... und viele mehr

## 🚨 Anti-Patterns (Was wir NICHT machen)

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

## 🏥 Medizinischer Kontext

### Warum CamBridge existiert
- **Problem:** Ricoh G900 II macht JPEGs, PACS braucht DICOM
- **Lösung:** Automatische Konvertierung mit Metadaten-Übernahme
- **Nutzer:** Radiologen, MTAs, Krankenhaus-IT

### Typischer Workflow
1. **QRBridge:** Generiert QR-Code mit Patientendaten
2. **Kamera:** Scannt QR-Code, speichert in EXIF
3. **CamBridge:** Konvertiert JPEG→DICOM automatisch
4. **PACS:** Archiviert und verteilt Bilder

## 💬 Nur für mich (Claude) - Wichtige Lektionen

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

## 📝 Standard Prompt-Vorlage für neue Chats

```
Ich arbeite an CamBridge v0.5.19.
© 2025 Claude's Improbably Reliable Software Solutions

STATUS: ExifTool Pipeline theoretisch implementiert
AUFGABE: Pipeline praktisch umsetzen und testen

GitHub: https://github.com/ostern42/CamBridge

1. PROJECT_WISDOM.md hochladen
2. Relevante Source-URLs bereitstellen
3. "VOGON INIT" sagen

Fokus: ExifToolReader implementieren & testen!
```

---
*Hinweis: Dieses Dokument enthält das gesammelte Projekt-Wissen. Bei Platzmangel können alte Micro-Versionen gekürzt werden, aber Lektionen und Erkenntnisse müssen erhalten bleiben!*
