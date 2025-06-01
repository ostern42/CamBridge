# CamBridge Changelog

All notable changes to CamBridge will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.3.3] - 2025-05-31 23:30
### Fixed
- Fixed Encoder namespace ambiguity in JpegTestFileGenerator
- Fixed undefined patientId variable in integration tests
- All integration tests now compile and run successfully
- Added missing Microsoft.Extensions.Logging.Console package to tests
- All integration tests now compile successfully
- Fixed namespace issues in test helpers
- 
### Changed
- Improved test stability and error handling
- Enhanced JPEG test file generation with proper EXIF data
- Improved test project dependencies
- Enhanced build configuration

## [0.3.2] - 2025-05-31 23:00
### Fixed
- Resolved System.Drawing.Imaging.Encoder namespace ambiguity in tests
- Corrected EncoderParameter quality value type conversion  
- Fixed FluentAssertions syntax for combined assertions
- Integration tests now execute successfully

### Added
- Dead Letter Queue for failed conversions with persistence
- Email and Event Log notification system
- Web Dashboard for real-time monitoring (http://localhost:5050)
- REST API with Swagger documentation
- Daily summary reports
- PowerShell installation script with automated setup
- Build and deployment automation

## [0.3.1] - 2025-05-31 16:00
### Fixed
- Fixed dependency injection issue where singleton ProcessingQueue tried to consume scoped IFileProcessor
- ProcessingQueue now uses IServiceScopeFactory to create scopes for file processing
- Removed duplicate IFileProcessor registration in Program.cs

### Added
- Batch and PowerShell scripts for collecting source files for easy deployment

## [0.3.0] - 2025-05-31 12:05
### Added
- FileProcessor service orchestrating complete conversion pipeline
- ProcessingQueue with thread-safe operation and retry logic
- FolderWatcherService monitoring multiple folders via FileSystemWatcher
- Comprehensive configuration system via appsettings.json
- Health check endpoint for service monitoring
- Statistics reporting and performance metrics
- Graceful shutdown with processing completion
- File debouncing to avoid processing during write operations

### Changed
- Worker service now coordinates all processing components
- Enhanced logging with structured output
- Program.cs with full dependency injection setup
- Target framework to net8.0-windows for Windows Service

### Fixed
- Robust error handling throughout pipeline
- File locking detection before processing
- Proper resource cleanup on service shutdown

## [0.2.0] - 2025-05-30 22:00
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

## [0.1.1] - 2025-05-30 20:00
### Added
- RicohExifReader for specialized Ricoh camera support
- Raw EXIF data extraction fallback
- Pipe-delimited QRBridge format parsing
### Fixed
- Improved barcode data detection in EXIF
- Better handling of custom Ricoh tags

## [0.1.0] - 2025-05-30 18:00
### Added
- EXIF extraction with MetadataExtractor
- QRBridge data parsing from User Comment
- DICOM conversion with fo-dicom
- JPEG compression preservation
- Core entity models (Patient, Study, Metadata)
- Value objects (DicomTag, ExifTag, PatientId, StudyId)
- Infrastructure services (ExifReader, DicomConverter)
- Unit tests for core functionality

## [0.0.1] - 2025-05-30 15:00
### Added
- Initial project structure
- Core interfaces and abstractions
- Windows Service skeleton
- Automatic versioning setup

[0.3.3]: https://github.com/claude/cambridge/compare/v0.3.2...v0.3.3
[0.3.2]: https://github.com/claude/cambridge/compare/v0.3.1...v0.3.2
[0.3.1]: https://github.com/claude/cambridge/compare/v0.3.0...v0.3.1
[0.3.0]: https://github.com/claude/cambridge/compare/v0.2.0...v0.3.0
[0.2.0]: https://github.com/claude/cambridge/compare/v0.1.1...v0.2.0
[0.1.1]: https://github.com/claude/cambridge/compare/v0.1.0...v0.1.1
[0.1.0]: https://github.com/claude/cambridge/compare/v0.0.1...v0.1.0
[0.0.1]: https://github.com/claude/cambridge/releases/tag/v0.0.1
