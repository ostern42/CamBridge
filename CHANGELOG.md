# CamBridge Changelog

All notable changes to CamBridge will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.2.0] - 2025-01-24
### Added
- JSON-based mapping configuration system
- MappingConfigurationLoader for loading/saving mappings
- DicomTagMapper service for dynamic tag mapping
- CustomMappingConfiguration with validation
- Support for value transformations (date, gender, truncation)
- Default value support for optional fields
- Comprehensive mapping configuration tests

### Changed
- DicomConverter now supports dynamic mappings
- ServiceCollectionExtensions enhanced with configuration options
- Improved dependency injection setup

## [0.1.1] - 2025-01-18
### Added
- RicohExifReader for specialized Ricoh camera support
- Raw EXIF data extraction fallback
- Pipe-delimited QRBridge format parsing
### Fixed
- Improved barcode data detection in EXIF
- Better handling of custom Ricoh tags

## [0.1.0] - 2025-01-17
### Added
- EXIF extraction with MetadataExtractor
- QRBridge data parsing from User Comment
- DICOM conversion with fo-dicom
- JPEG compression preservation
- Core entity models (Patient, Study, Metadata)
- Value objects (DicomTag, ExifTag, PatientId, StudyId)
- Infrastructure services (ExifReader, DicomConverter)
- Unit tests for core functionality

## [0.0.1] - 2025-01-10
### Added
- Initial project structure
- Core interfaces and abstractions
- Windows Service skeleton
- Automatic versioning setup

[0.2.0]: https://github.com/claude/cambridge/compare/v0.1.1...v0.2.0
[0.1.1]: https://github.com/claude/cambridge/compare/v0.1.0...v0.1.1
[0.1.0]: https://github.com/claude/cambridge/compare/v0.0.1...v0.1.0
[0.0.1]: https://github.com/claude/cambridge/releases/tag/v0.0.1
