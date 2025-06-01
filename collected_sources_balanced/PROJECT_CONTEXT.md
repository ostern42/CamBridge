# CamBridge Context v0.4.2
© 2025 Claude's Improbably Reliable Software Solutions

## Aktueller Stand
- v0.4.0: GUI Framework mit Dashboard (2x committed)
- v0.4.1: Settings-Page komplett implementiert
- Nächste: Dead Letters Implementation (v0.4.2)

## PROJECT_WISDOM.md
Enthält das MAGIC WORDS SYSTEM:
- "CAMBRIDGE INIT" für automatischen Start
- "WISDOM: [text]" für Live-Updates
- "CLAUDE: [text]" für persönliche Notizen

## Projekt-Struktur
- src/CamBridge.Core: Domain Models, Settings
- src/CamBridge.Infrastructure: EXIF/DICOM Processing
- src/CamBridge.Service: Windows Service mit Web API
- src/CamBridge.Config: WPF GUI (MVVM mit ModernWPF)

## Tech Stack
- .NET 8, C# 12
- WPF mit ModernWpfUI 0.9.9 (NICHT WinUI3)
- CommunityToolkit.Mvvm 8.2.2
- ASP.NET Core Minimal API
- fo-dicom für DICOM

## Nächste Aufgabe: Dead Letters (v0.4.2)
- Vollständige Dead Letters Page mit Liste
- HttpApiService für /api/deadletters
- Retry/Delete/Export Funktionen
- Batch-Operationen
- Integration mit DeadLetterQueueService

## Wichtige Patterns
- MVVM mit ObservableObject
- Async/Await für I/O
- Dependency Injection überall
- Clean Architecture
- KISS-Prinzip beachten
