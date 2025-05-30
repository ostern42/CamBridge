# CamBridge

JPEG to DICOM converter for medical imaging, specifically designed for Ricoh G900 II cameras with QRBridge barcode integration.

**Version:** 0.1.0  
**Copyright:** © 2025 Claude's Improbably Reliable Software Solutions

## Overview

CamBridge processes JPEG images from Ricoh G900 II cameras that contain patient and examination information encoded via QRBridge barcodes. The software extracts this information from EXIF metadata and converts the images to DICOM format for PACS integration.

## Features

- **EXIF Extraction** ✓ Extract QRBridge data from Ricoh camera barcode tags
- **DICOM Conversion** (Coming in v0.2.0)
- **Windows Service** (Coming in v0.3.0)
- **Configuration GUI** (Coming in v0.4.0)

## Architecture

```
CamBridge/
├── src/
│   ├── CamBridge.Core/          # Domain models and interfaces
│   ├── CamBridge.Infrastructure/# EXIF/DICOM processing
│   ├── CamBridge.Service/       # Windows Service (planned)
│   └── CamBridge.Config/        # WinUI 3 GUI (planned)
└── tests/
    └── CamBridge.Infrastructure.Tests/
```

## QRBridge Data Format

CamBridge supports two formats:

1. **Pipe-delimited** (Ricoh G900 II default):
   ```
   EX002|Schmidt, Maria|1985-03-15|F|Röntgen Thorax
   ```

2. **Command-line format**:
   ```
   -examid "EX002" -name "Schmidt, Maria" -birthdate "1985-03-15"
   ```

## Development Status

### Phase 3 Complete (v0.1.0) ✓
- EXIF reader implementation
- Ricoh-specific barcode tag extraction
- QRBridge format parsing
- Unit test framework

### Next: Phase 4 - DICOM Conversion
- fo-dicom library integration
- JPEG to DICOM converter
- DICOM tag mapping

## Requirements

- .NET 8.0 SDK
- Windows 10/11 (x64)
- Visual Studio 2022 or VS Code

## Building

```bash
dotnet build
dotnet test
```

## Usage (Development)

```csharp
// Register services
services.AddCamBridgeInfrastructureForRicoh();

// Extract QRBridge data
var exifReader = serviceProvider.GetRequiredService<IExifReader>();
var barcodeData = await exifReader.GetUserCommentAsync("image.jpg");
var patientInfo = exifReader.ParseQRBridgeData(barcodeData);
```

## License

Proprietary - Claude's Improbably Reliable Software Solutions

---

*For medical imaging professionals who need reliable JPEG to DICOM conversion.*