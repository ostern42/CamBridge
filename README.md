# CamBridge

JPEG to DICOM converter for medical imaging

## Overview

CamBridge processes JPEG images from Ricoh G900 II cameras, extracting patient and examination data from EXIF metadata and converting them to DICOM format for PACS integration.

## Features

- Automatic EXIF extraction from JPEG files
- DICOM-compliant conversion (Secondary Capture)
- Windows Service for folder monitoring
- Configuration GUI (WinUI 3)
- Configurable tag mapping

## Requirements

- Windows 10/11 (64-bit)
- .NET 8.0 Runtime
- Administrator privileges (for service installation)

## Architecture

- **CamBridge.Core**: Domain logic and entities
- **CamBridge.Infrastructure**: EXIF/DICOM processing
- **CamBridge.Service**: Windows Service for automation
- **CamBridge.Config**: Configuration interface

## Installation

1. Extract all files to desired location
2. Run `CamBridge.Config.exe` as Administrator
3. Configure input/output folders and mappings
4. Install and start the service

## Configuration

Service configuration is stored in `appsettings.json`:
- Input/output folder paths
- EXIF to DICOM tag mappings
- Processing options

## DICOM Compliance

- SOP Class: VL Photographic Image (1.2.840.10008.5.1.4.1.1.77.1.4)
- Transfer Syntax: JPEG Baseline (1.2.840.10008.1.2.4.50)
- Modality: XC (External Camera)

## Version

Current: 0.0.1 (Initial structure)

---
© 2025 Claude's Improbably Reliable Software Solutions