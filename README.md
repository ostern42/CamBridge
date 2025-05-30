# CamBridge - JPEG to DICOM Converter

[![Version](https://img.shields.io/badge/version-0.1.1-blue.svg)](CHANGELOG.md)
[![.NET](https://img.shields.io/badge/.NET-8.0-purple.svg)](https://dotnet.microsoft.com)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)

CamBridge is a Windows service that monitors folders for JPEG images from Ricoh cameras and automatically converts them to DICOM format, preserving patient and examination data embedded in EXIF fields.

## 🎯 Features

- **EXIF Data Extraction**: Reads patient/exam data from JPEG EXIF fields
- **QRBridge Compatibility**: Parses QRBridge-encoded data from Ricoh cameras
- **DICOM Conversion**: Creates standard-compliant DICOM files with preserved JPEG compression
- **Automatic Processing**: Windows service monitors folders for new images
- **Metadata Preservation**: Maps EXIF data to appropriate DICOM tags
- **German Umlaut Support**: Proper character encoding (ISO_IR 100)

## 🏗️ Architecture

```
CamBridge/
├── src/
│   ├── CamBridge.Core/          # Domain entities, interfaces
│   ├── CamBridge.Infrastructure/# EXIF reader, DICOM converter
│   └── CamBridge.Service/       # Windows service host
└── tests/
    └── CamBridge.Infrastructure.Tests/
```

## 🔧 Technical Details

### DICOM Implementation
- **SOP Class**: VL Photographic Image Storage (1.2.840.10008.5.1.4.1.1.77.1.4)
- **Transfer Syntax**: JPEG Baseline Process 1 (1.2.840.10008.1.2.4.50)
- **Photometric Interpretation**: YBR_FULL_422 (for JPEG compressed data)
- **Character Set**: ISO_IR 100 (Latin-1 with German extensions)

### Supported Data Format
QRBridge data in pipe-delimited format:
```
EX002|Schmidt, Maria|1985-03-15|F|Röntgen Thorax
```

## 📦 Dependencies

- [fo-dicom](https://github.com/fo-dicom/fo-dicom) v5.1.2 - DICOM toolkit
- [MetadataExtractor](https://github.com/drewnoakes/metadata-extractor-dotnet) v2.8.1 - EXIF reading
- [Serilog](https://serilog.net/) - Structured logging
- System.Drawing.Common - Image processing

## 🚀 Getting Started

### Prerequisites
- .NET 8.0 SDK
- Windows 10/11 or Windows Server 2019+
- Visual Studio 2022 (optional)

### Building
```bash
dotnet build
dotnet test
```

### Installation
```bash
# Install as Windows Service
sc create CamBridgeService binPath="C:\Path\To\CamBridge.Service.exe"
```

## 📸 Ricoh Camera Configuration

1. Enable QRBridge barcode scanning
2. Set camera to save barcodes in EXIF User Comment field
3. Configure image output folder

## 🔍 EXIF to DICOM Mapping

| EXIF/QRBridge Field | DICOM Tag | Description |
|---------------------|-----------|-------------|
| name | (0010,0010) | Patient Name |
| patientid | (0010,0020) | Patient ID |
| birthdate | (0010,0030) | Patient Birth Date |
| gender | (0010,0040) | Patient Sex |
| examid | (0020,0010) | Study ID |
| comment | (0008,1030) | Study Description |
| Make | (0008,0070) | Manufacturer |
| Model | (0008,1090) | Model Name |

## 📊 Current Status

- ✅ Phase 1: Project setup (v0.0.1)
- ✅ Phase 2: Core domain model
- ✅ Phase 3: EXIF extraction (v0.1.0)
- ✅ Phase 4: DICOM conversion (v0.1.1)
- ⏳ Phase 5: Mapping configuration
- ⏳ Phase 6: File processing pipeline
- ⏳ Phase 7: Folder monitoring
- ⏳ Phase 8: Configuration & UI

## 📝 License

MIT License - see [LICENSE](LICENSE) file

## 👥 Credits

© 2025 Claude's Improbably Reliable Software Solutions

---

*CamBridge - Bridging the gap between camera and PACS*