# CamBridge Entwicklungsplan v2.0

## Projektübersicht
**Produkt:** CamBridge - JPEG zu DICOM Konverter für medizinische Bildgebung  
**Copyright:** © 2025 Claude's Improbably Reliable Software Solutions  
**Technologie:** C# .NET 8, WinUI 3, Windows Service  
**Architektur:** Clean Architecture mit klarer Trennung von Concerns

## Versionierungsstrategie

- **0.0.x** - Grundstruktur und erste Komponenten
- **0.1.x** - EXIF-Verarbeitung funktionsfähig
- **0.2.x** - DICOM-Konvertierung implementiert
- **0.3.x** - Windows Service läuft
- **0.4.x** - GUI vollständig
- **0.5.x** - Feature-complete Beta
- **0.9.x** - Release Candidates
- **1.0.0** - Erstes stabiles Release

## Bisheriger Fortschritt (Stand: 31.05.2025, 23:00 Uhr)

### ✅ Phase 1-6: ABGESCHLOSSEN
- Projektstruktur ✓
- Core Domain Models ✓
- EXIF-Extraktion ✓
- DICOM-Konvertierung ✓
- Mapping-System ✓
- Windows Service Grundstruktur ✓

### ✅ Phase 7: Dateiverarbeitung Pipeline - ABGESCHLOSSEN (v0.3.0)
- Ordnerüberwachung (FolderWatcherService) ✓
- Datei-Queue System (ProcessingQueue) ✓
- Fehlerbehandlung mit Retry-Logic ✓
- Backup-Funktionalität ✓

### 🎯 Vorgezogene Features aus Phase 11 (v0.3.1-0.3.2)
- Dead-Letter-Queue ✓
- Email & Event Log Notifications ✓
- Web Dashboard & REST API ✓
- Statistiken & Monitoring ✓
- PowerShell Installation ✓

## Verbleibende Entwicklungsphasen

### 🐛 Phase 7.5: Test-Fixes & Stabilisierung (Sofort)
- Integration Test Build-Fehler beheben
- Test-Coverage sicherstellen
- Performance-Tests durchführen
- **Version: 0.3.3**

### 📱 Phase 8: WinUI 3 GUI Basis (1-2 Chats)
- CamBridge.Config Projekt erstellen
- Moderne UI mit Fluent Design
- Navigation-Framework (Frame-basiert)
- MVVM-Struktur mit CommunityToolkit
- Responsive Layout
- **Version: 0.4.0**

### 🎮 Phase 9: Service-Steuerung GUI (1 Chat)
- Service Installation/Deinstallation UI
- Start/Stop/Pause Controls
- Uptime & Status-Anzeige
- Admin-Rechte Handling (UAC)
- Event Log Viewer Integration
- **Version: 0.4.1**

### ⚙️ Phase 10: Konfigurationsverwaltung GUI (1 Chat)
- Watch Folder Management
- Mapping-Editor (Drag & Drop)
- Settings-UI mit Validierung
- Import/Export Konfiguration
- Live-Preview für Mappings
- **Version: 0.4.2**

### 🚀 Phase 11: Performance & Polish (1 Chat)
- Batch-Verarbeitung optimieren
- Memory-Pool für große Dateien
- Parallelisierung (Channels)
- UI-Animationen verfeinern
- **Version: 0.5.0** (Feature-complete Beta)

### 🏥 Phase 12: PACS Integration (Optional, 2 Chats)
- DICOM C-STORE SCU
- AE Title Konfiguration
- Network Transfer
- PACS Query/Retrieve
- **Version: 0.6.0**

### 📦 Phase 13: Deployment & Release (1 Chat)
- Single-File Deployment
- MSI Installer (WiX)
- Dokumentation finalisieren
- Automated Release Pipeline
- **Version: 0.9.0 → 1.0.0**

## Technische Details für verbleibende Phasen

### WinUI 3 GUI Architektur
```
CamBridge.Config/
├── ViewModels/          # MVVM ViewModels
├── Views/               # XAML Views
├── Controls/            # Custom Controls
├── Services/            # UI Services
├── Converters/          # Value Converters
└── Resources/           # Styles, Templates
```

### Geplante GUI Features
- **Dashboard**: Echtzeit-Statistiken, Graphen
- **Service Control**: Start/Stop mit Animations
- **Configuration**: Tree-View für Mappings
- **Logs**: Virtualisierte Log-Anzeige
- **Dead Letters**: Interaktive Liste mit Actions

### Technologie-Stack GUI
- WinUI 3 (Windows App SDK 1.5)
- CommunityToolkit.Mvvm
- Win32 Interop für Service Control
- LiveCharts2 für Visualisierungen

## Prompt-Vorlage für neue Chats

```
Ich arbeite an CamBridge, einem JPEG zu DICOM Konverter.
© 2025 Claude's Improbably Reliable Software Solutions

Aktueller Stand: Version 0.3.2 - Windows Service mit Dead-Letter-Queue und Web Dashboard
Nächster Schritt: [Phase X - Beschreibung]

Bisherige Features:
- EXIF/DICOM Konvertierung funktioniert
- Windows Service läuft stabil
- Dead-Letter-Queue implementiert
- Web Dashboard unter http://localhost:5050
- Email/Event Log Notifications

Bitte implementiere [spezifische Aufgabe] unter Beachtung:
- Clean Architecture beibehalten
- Englische Code-Kommentare
- Tests nicht vergessen
- Versionierung aktualisieren

[Relevante Code-Dateien als Kontext einfügen]
```

## Wichtige Hinweise

### Code-Qualität
- KISS-Prinzip beachten
- Ausführliche XML-Dokumentation
- Minimum 80% Test-Coverage
- Performance im Auge behalten

### Versionierung
- Bei jedem Feature: Version in Version.props erhöhen
- CHANGELOG.md aktualisieren
- Git Tags nicht vergessen

### Testing
- Unit Tests für Business Logic
- Integration Tests für Services
- UI Tests für kritische Workflows
- Performance Tests vor Release

## Meilensteine

1. **v0.4.0** - GUI lauffähig (Phase 8)
2. **v0.5.0** - Feature Complete (Phase 11)
3. **v0.6.0** - PACS Ready (Phase 12, optional)
4. **v1.0.0** - Production Release (Phase 13)

## Geschätzte Zeitplanung

- Phase 7.5 (Bugfixes): 1-2 Stunden
- Phase 8-10 (GUI): 3-4 Chats
- Phase 11 (Performance): 1 Chat
- Phase 12 (PACS): 2 Chats (optional)
- Phase 13 (Release): 1 Chat

**Gesamt bis v1.0**: ca. 6-8 weitere Chats