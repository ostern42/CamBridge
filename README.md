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
- **Comprehensive logging** with Serilog

## System Requirements

- Windows 10/11 or Windows Server 2016+
- .NET 8.0 Runtime
- Ricoh G900 II camera with QRBridge-encoded QR codes

## Installation

1. Download the latest release
2. Extract to installation directory (e.g., `C:\Program Files\CamBridge`)
3. Configure settings in `appsettings.json`
4. Install as Windows Service:
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
    "DefaultOutputFolder": "C:\\CamBridge\\Output",
    "MappingConfigurationFile": "mappings.json"
  }
}
```

### Mapping Configuration (mappings.json)

CamBridge uses a flexible JSON-based mapping system to convert EXIF and QRBridge data to DICOM tags:

```json
{
  "version": "1.0",
  "description": "CamBridge EXIF to DICOM mapping configuration",
  "mappings": [
    {
      "name": "PatientName",
      "sourceType": "QRBridge",
      "sourceField": "name",
      "targetTag": "(0010,0010)",
      "transform": "None",
      "required": true
    },
    {
      "name": "PatientBirthDate",
      "sourceType": "QRBridge",
      "sourceField": "birthdate",
      "targetTag": "(0010,0030)",
      "transform": "DateToDicom"
    },
    {
      "name": "Manufacturer",
      "sourceType": "EXIF",
      "sourceField": "Make",
      "targetTag": "(0008,0070)"
    }
  ]
}
```

#### Mapping Configuration Options

- **sourceType**: "QRBridge" or "EXIF"
- **transform**: Value transformation
  - `None`: No transformation
  - `DateToDicom`: Convert dates to YYYYMMDD format
  - `GenderToDicom`: Convert to M/F/O
  - `ToUpper`/`ToLower`: Case conversion
  - `TruncateTo16`/`TruncateTo64`: Length limits
- **required**: If true, conversion fails when field is missing
- **defaultValue**: Used when source field is empty

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

## Development

### Building from Source

```bash
# Clone repository
git clone https://github.com/claude/cambridge.git

# Build
dotnet build

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
│   └── CamBridge.Service/       # Windows Service host
└── tests/
    └── CamBridge.Infrastructure.Tests/
```

## Roadmap

- [x] Phase 1-2: Project setup and core models
- [x] Phase 3-4: EXIF extraction and DICOM conversion
- [x] Phase 5: Mapping configuration system
- [ ] Phase 6-7: File monitoring and processing pipeline
- [ ] Phase 8: Enhanced error handling and retry logic
- [ ] Phase 9: Web management interface
- [ ] Phase 10: PACS integration

## License

Proprietary - © 2025 Claude's Improbably Reliable Software Solutions

## Support

For issues and feature requests, please contact support.
