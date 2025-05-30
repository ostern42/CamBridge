# Changelog
All notable changes to CamBridge will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.1.1] - 2025-01-15

### Added
- DICOM converter implementation using fo-dicom v5.1.2
- JPEG to DICOM conversion with preserved compression
- VL Photographic Image Storage SOP Class support
- Proper JPEG encapsulated pixel data handling
- Photometric interpretation YBR_FULL_422 for JPEG
- Character set ISO_IR 100 for German umlauts
- DICOM file validation with mandatory tag checks
- Comprehensive unit tests for DICOM conversion
- System.Drawing.Common for image dimension reading

### Technical
- Transfer Syntax: JPEG Baseline (1.2.840.10008.1.2.4.50)
- SOP Class: VL Photographic Image (1.2.840.10008.5.1.4.1.1.77.1.4)
- Implementation UID: 1.2.276.0.7230010.3.0.3.6.4

## [0.1.0] - 2025-01-15

### Added
- EXIF data extraction from JPEG files
- QRBridge data parsing (pipe-delimited format)
- Ricoh G900 II barcode tag support
- MetadataExtractor library integration
- Comprehensive unit test suite
- Service registration extensions
- Specialized RicohExifReader implementation

### Technical
- Clean Architecture structure
- .NET 8.0 target framework
- xUnit test framework

## [0.0.1] - 2025-01-14

### Added
- Initial project structure
- Core domain entities and value objects
- Windows Service infrastructure
- Serilog logging configuration
- Version management system

---
© 2025 Claude's Improbably Reliable Software Solutions