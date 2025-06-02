# CamBridge Projekt-Struktur
**Stand:** 2025-06-02, 11:45 Uhr  
**Zweck:** Referenz für Datei-Locations und Projekt-Übersicht

## Tree Command für Updates
```powershell
tree /F src | Select-String -Pattern "\.cs$|\.xaml$|\.csproj$|\.json$|\.md$|\+---"
```

## Projekt-Struktur (src Ordner)

```
src/
├── CamBridge.Config/          # WPF GUI Anwendung
│   ├── App.xaml
│   ├── App.xaml.cs
│   ├── MainWindow.xaml
│   ├── MainWindow.xaml.cs
│   ├── CamBridge.Config.csproj
│   │
│   ├── Assets/               # UI Assets
│   │
│   ├── Converters/
│   │   └── ValueConverters.cs
│   │
│   ├── Dialogs/
│   │   ├── DicomTagBrowserDialog.xaml
│   │   └── DicomTagBrowserDialog.xaml.cs
│   │
│   ├── Helpers/
│   │   └── PasswordBoxHelper.cs
│   │
│   ├── Models/
│   │   └── ServiceStatusModel.cs
│   │
│   ├── Services/
│   │   ├── ConfigurationService.cs
│   │   ├── HttpApiService.cs
│   │   ├── IApiService.cs
│   │   ├── IConfigurationService.cs
│   │   ├── INavigationService.cs
│   │   ├── IServiceManager.cs
│   │   ├── NavigationService.cs
│   │   └── ServiceManager.cs
│   │
│   ├── ViewModels/
│   │   ├── DashboardViewModel.cs
│   │   ├── DeadLettersViewModel.cs
│   │   ├── MainViewModel.cs
│   │   ├── MappingEditorViewModel.cs
│   │   ├── ServiceControlViewModel.cs
│   │   ├── SettingsViewModel.cs
│   │   └── ViewModelBase.cs
│   │
│   └── Views/
│       ├── AboutPage.xaml[.cs]
│       ├── DashboardPage.xaml[.cs]
│       ├── DeadLettersPage.xaml[.cs]
│       ├── MappingEditorPage.xaml[.cs]
│       ├── ServiceControlPage.xaml[.cs]
│       ├── SettingsPage.xaml[.cs]
│       └── VogonPoetryWindow.xaml[.cs]
│
├── CamBridge.Core/           # Domain Models & Interfaces
│   ├── CamBridge.Core.csproj
│   ├── CamBridgeSettings.cs
│   ├── DeadLetterStatistics.cs
│   ├── MappingRule.cs
│   ├── NotificationSettings.cs
│   ├── ProcessingOptions.cs
│   ├── ProcessingSummary.cs
│   │
│   ├── Entities/
│   │   ├── ImageMetadata.cs
│   │   ├── PatientInfo.cs
│   │   ├── ProcessingResult.cs    # ⚠️ Namenskonflikt mit Interfaces!
│   │   └── StudyInfo.cs
│   │
│   ├── Exceptions/            # (Leer)
│   │
│   ├── Interfaces/
│   │   ├── IDicomConverter.cs
│   │   ├── IExifReader.cs
│   │   ├── IFileProcessor.cs      # ⚠️ Enthält FileProcessingResult (früher ProcessingResult)
│   │   └── IMappingConfiguration.cs
│   │
│   ├── Services/              # (Leer)
│   │
│   └── ValueObjects/
│       ├── DicomTag.cs
│       ├── ExifTag.cs
│       ├── PatientId.cs           # ✅ Nur hier, NICHT in Entities!
│       └── StudyId.cs
│
├── CamBridge.Infrastructure/  # Implementierungen
│   ├── CamBridge.Infrastructure.csproj
│   ├── GlobalUsings.cs
│   ├── ServiceCollectionExtensions.cs
│   │
│   ├── Configuration/         # (Dateien unbekannt)
│   ├── Dicom/                # (Dateien unbekannt)
│   ├── Exif/                 # (Dateien unbekannt)
│   ├── FileSystem/           # (Dateien unbekannt)
│   ├── Properties/           # (Dateien unbekannt)
│   │
│   └── Services/
│       ├── DeadLetterQueue.cs
│       ├── DicomConverter.cs
│       ├── DicomTagMapper.cs
│       ├── ExifReader.cs
│       ├── ExifToolReader.cs      # NEU in v0.5.3
│       ├── FileProcessor.cs       # Implementiert IFileProcessor
│       ├── FolderWatcherService.cs
│       ├── INotificationService.cs
│       ├── MappingConfigurationLoader.cs
│       ├── NotificationService.cs
│       ├── ProcessingQueue.cs
│       └── RicohExifReader.cs
│
└── CamBridge.Service/        # Windows Service
    ├── CamBridge.Service.csproj
    ├── appsettings.json
    ├── appsettings.Development.json
    ├── mappings.json
    ├── Program.cs
    ├── Worker.cs
    ├── CamBridgeHealthCheck.cs
    ├── DailySummaryService.cs
    │
    ├── Controllers/
    │   └── StatusController.cs
    │
    └── Properties/
        └── launchSettings.json
```

## Root-Verzeichnis Struktur

```
CamBridge/
├── .git/
├── .vs/
├── docs/                     # (Falls vorhanden)
├── src/                      # (Siehe oben)
├── tests/                    # (Falls vorhanden)
├── Tools/
│   └── exiftool.exe         # v12.96 - KRITISCH für Barcode Tag!
│
├── CamBridge.ParserDebug/    # Debug Console App
│   ├── CamBridge.ParserDebug.csproj
│   └── Program.cs
│
├── QRBridge/                 # QRBridge Source Code
│
├── .gitignore
├── CamBridge.sln
├── CHANGELOG.md
├── Directory.Build.props
├── LICENSE
├── PROJECT_WISDOM.md         # Dieses Dokument
├── CAMBRIDGE_TREE_STRUCTURE.md  # Diese Datei
├── README.md
└── Version.props            # Zentrale Versionsverwaltung
```

## Wichtige Erkenntnisse aus der Struktur

1. **KEIN PatientId Duplikat!** - PatientId existiert nur in ValueObjects, nicht in Entities
2. **ProcessingResult Konflikt** - Existiert in Entities UND wurde in Interfaces definiert
3. **ExifToolReader** - Neue Klasse in Infrastructure/Services (v0.5.3)
4. **Tools Ordner** - Enthält ExifTool.exe, kritisch für Barcode Tag Lesung
5. **Clean Architecture** - Klare Trennung zwischen Core, Infrastructure und Service

## GitHub Integration
Basis-URL für Raw-Dateien:
```
https://raw.githubusercontent.com/ostern42/CamBridge/refs/heads/main/
```

Beispiel-URLs:
- Version.props: `[BASIS-URL]Version.props`
- PatientId.cs: `[BASIS-URL]src/CamBridge.Core/ValueObjects/PatientId.cs`
- FileProcessor.cs: `[BASIS-URL]src/CamBridge.Infrastructure/Services/FileProcessor.cs`
