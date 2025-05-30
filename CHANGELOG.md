# Changelog
All notable changes to CamBridge will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [0.0.2] - 2025-05-30
### Added
- Core domain models: PatientInfo, StudyInfo, ImageMetadata
- Value objects: DicomTag, ExifTag, PatientId, StudyId
- Core interfaces: IExifReader, IDicomConverter, IFileProcessor, IMappingConfiguration
- Configuration models: CamBridgeSettings, MappingRule, ProcessingOptions
- DICOM tag constants for common modules
- QRBridge data parsing support

## [0.0.1] - 2025-01-30
### Added
- Initial project structure
- Solution with four projects: Core, Infrastructure, Service, Config
- Automatic versioning via Version.props
- Basic documentation (README, LICENSE)

---
© 2025 Claude's Improbably Reliable Software Solutions