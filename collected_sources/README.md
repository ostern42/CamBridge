# CamBridge

JPEG to DICOM converter for medical imaging from Ricoh cameras with QRBridge integration.

© 2025 Claude's Improbably Reliable Software Solutions

## Overview

CamBridge is a Windows service that monitors folders for JPEG images from Ricoh G900 II cameras and automatically converts them to DICOM format. Patient and examination data embedded via QRBridge QR codes is extracted from EXIF metadata and mapped to appropriate DICOM tags.

## Features

- **Automatic JPEG to DICOM conversion** preserving original compression
- **QRBridge data extraction** from EXIF User Comment field
- **Flexible mapping configuration** via JSON files
- **Ricoh G900 II camera support** with specialized EXIF reading
- **Windows Service** for background operation
- **Dead Letter Queue** for failed conversions
- **Email & Event Log notifications** for critical errors
- **Web Dashboard** for real-time monitoring
- **REST API** for integration and monitoring
- **Comprehensive logging** with Serilog

## System Requirements

- Windows 10/11 or Windows Server 2016+
- .NET 8.0 Runtime
- Ricoh G900 II camera with QRBridge-encoded QR codes
- Administrator privileges for service installation

## Quick Start

1. Download the latest release
2. Extract to a temporary folder
3. Run PowerShell as Administrator:
   ```powershell
   .\Install-CamBridge.ps1
   ```
4. Access the dashboard at http://localhost:5050

## Installation

### Automated Installation

The PowerShell installation script handles:
- Service creation and configuration
- Directory structure setup
- Firewall rule configuration
- Event Log source creation

```powershell
# Install with custom path
.\Install-CamBridge.ps1 -InstallPath "D:\CamBridge"

# Uninstall
.\Install-CamBridge.ps1 -Uninstall
```

### Manual Installation

1. Extract files to installation directory
2. Create required directories:
   - `C:\CamBridge\Input`
   - `C:\CamBridge\Output`
   - `C:\CamBridge\Archive`
   - `C:\CamBridge\Errors`
   - `C:\CamBridge\Backup`
   - `C:\CamBridge\Logs`

3. Install as Windows Service:
   ```cmd
   sc create CamBridgeService binPath="C:\Program Files\CamBridge\CamBridge.Service.exe"
   ```

## Configuration

### Basic Settings (appsettings.json)

```json
{
  "CamBridge": {
    "WatchFolders": [
      {
        "Path": "C:\\Images\\Input",
        "OutputPath": "C:\\Images\\DICOM",
        "Enabled": true
      }
    ],
    "Processing": {
      "MaxRetryAttempts": 3,
      "RetryDelaySeconds": 5
    },
    "Notifications": {
      "EnableEmail": true,
      "EmailTo": "admin@hospital.com",
      "SmtpHost": "smtp.hospital.com"
    }
  }
}
```

### Mapping Configuration (mappings.json)

CamBridge uses a flexible JSON-based mapping system to convert EXIF and QRBridge data to DICOM tags:

```json
{
  "mappings": [
    {
      "name": "PatientName",
      "sourceType": "QRBridge",
      "sourceField": "name",
      "targetTag": "(0010,0010)",
      "transform": "None",
      "required": true
    }
  ]
}
```

## QRBridge Format

QRBridge encodes patient data as pipe-delimited strings:
```
EX002|Schmidt, Maria|1985-03-15|F|Röntgen Thorax
```

Fields:
1. Exam ID
2. Patient Name
3. Birth Date
4. Gender (M/F/O)
5. Comment/Study Description

## Monitoring

### Web Dashboard

Access the real-time monitoring dashboard at http://localhost:5050

Features:
- Service status and uptime
- Processing queue statistics
- Success/failure rates
- Dead letter queue management
- Active processing items

### REST API

API documentation available at http://localhost:5050/swagger

Key endpoints:
- `GET /api/status` - Service status
- `GET /api/status/statistics` - Processing statistics
- `GET /api/status/deadletters` - Dead letter items
- `POST /api/status/deadletters/{id}/reprocess` - Reprocess failed item
- `GET /api/status/health` - Health check

### Event Log

CamBridge logs to Windows Event Log under "Application" source "CamBridge Service".

## Dead Letter Queue

Files that fail processing after all retry attempts are moved to the dead letter queue:

- Located in `C:\CamBridge\Errors\dead-letters`
- Organized by date
- Metadata stored in `dead-letters.json`
- Can be reprocessed via dashboard or API

## Notifications

### Email Notifications

Configure SMTP settings for email alerts:
- Critical errors
- Dead letter threshold exceeded
- Daily processing summaries

### Event Log Notifications

All notifications are also logged to Windows Event Log.

## Troubleshooting

### Service Won't Start

1. Check Event Viewer for errors
2. Verify all directories exist and have proper permissions
3. Ensure .NET 8.0 runtime is installed
4. Check `C:\CamBridge\Logs` for detailed logs

### Files Not Processing

1. Verify watch folder configuration
2. Check file permissions
3. Ensure JPEG files contain valid EXIF data
4. Review dead letter queue for errors

### DICOM Validation Errors

1. Check mapping configuration
2. Verify required patient data is present
3. Review DICOM validation logs

## Development

### Building from Source

```bash
# Clone repository
git clone https://github.com/claude/cambridge.git

# Build
dotnet build --configuration Release

# Run tests
dotnet test

# Publish
dotnet publish -c Release -r win-x64
```

### Project Structure

```
CamBridge/
├── src/
│   ├── CamBridge.Core/          # Domain models and interfaces
│   ├── CamBridge.Infrastructure/ # Service implementations
│   └── CamBridge.Service/       # Windows Service & Web API
└── tests/
    └── CamBridge.Infrastructure.Tests/
```

### Running Tests

```powershell
# Run all tests with coverage
.\Run-Tests.ps1

# Run specific test category
dotnet test --filter Category=Integration
```

## Version History

- **0.3.2** - Dead letter queue, notifications, web dashboard
- **0.3.1** - Fixed dependency injection issues
- **0.3.0** - Windows Service implementation
- **0.2.0** - Dynamic mapping configuration
- **0.1.0** - Core EXIF/DICOM functionality
- **0.0.1** - Initial project structure

## Roadmap

### Phase 7: Dateiverarbeitung Pipeline (1 Chat)
- Ordnerüberwachung
- Datei-Queue System
- Fehlerbehandlung
- Backup-Funktionalität

- 
### Phase 8: WinUI 3 GUI Basis (2 Chats)
- CamBridge Config Projekt
- Moderne UI mit Animationen
- Navigation-Framework
- MVVM-Struktur


### Phase 9: Service-Steuerung GUI (1 Chat)
- Service Installation/Deinstallation
- Start/Stop/Status
- Uptime-Anzeige
- Admin-Rechte Handling


### Phase 10: Konfigurationsverwaltung (1 Chat)
- JSON-Konfiguration
- Settings-UI
- Ordner-Auswahl Dialoge
- Mapping-Editor


## License

Proprietary - © 2025 Claude's Improbably Reliable Software Solutions

## Support

For issues and feature requests, please contact support.

## Acknowledgments

- fo-dicom for DICOM processing
- MetadataExtractor for EXIF reading
- Serilog for structured logging
- QRBridge for patient data encoding
