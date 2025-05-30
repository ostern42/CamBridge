# Changelog
All notable changes to CamBridge will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [0.1.0] - 2025-05-30
### Added
- EXIF reader implementation with Ricoh G900 II support
- Barcode tag extraction for QRBridge data
- Pipe-delimited and command-line format parsers
- Raw EXIF data extraction fallback
- Comprehensive unit tests for EXIF processing
- Infrastructure test project

### Fixed
- QRBridge data location (Barcode tag instead of UserComment)

## [0.0.2] - 2025-05-30
### Added  
- Core domain models (PatientInfo, StudyInfo, ImageMetadata)
- Value objects (DicomTag, ExifTag, PatientId, StudyId)
- Domain interfaces (IExifReader, IDicomConverter)
- Configuration models (Settings, MappingRule)
- Infrastructure project structure

## [0.0.1] - 2025-01-30
### Added
- Initial project structure
- Solution with four projects: Core, Infrastructure, Service, Config
- Automatic versioning via Version.props
- Basic documentation (README, LICENSE)

---
© 2025 Claude's Improbably Reliable Software Solutions