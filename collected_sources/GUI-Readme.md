# CamBridge Configuration GUI

**Version 0.4.0**  
**© 2025 Claude's Improbably Reliable Software Solutions**

## Overview

CamBridge Configuration is a modern WPF application for managing and monitoring the CamBridge Service. It provides real-time monitoring, service control, and configuration management through an intuitive interface.

## Features

### 🏠 Dashboard
- Real-time processing statistics
- Active file processing monitor
- Success/failure rate visualization
- Recent dead letter items with reprocessing
- Auto-refresh every 5 seconds

### 🎮 Service Control
- Start/Stop/Restart Windows Service
- Service installation status
- Administrator privilege handling
- Direct access to Event Viewer & Services Manager

### 📄 Dead Letter Management
- View all failed conversions
- Reprocess failed items
- Remove items from queue
- Error categorization

### ⚙️ Settings
- Configure watch folders
- Manage output directories
- Edit mapping configurations
- Notification settings

## Requirements

- Windows 10/11 or Windows Server 2016+
- .NET 8.0 Desktop Runtime
- CamBridge Service installed and running
- Administrator privileges (for service control)

## Getting Started

### Running the GUI

1. **Start CamBridge Service** (if not already running)
   ```powershell
   sc start CamBridgeService
   ```

2. **Launch CamBridge Config**
   - Navigate to installation directory (default: `C:\Program Files\CamBridge`)
   - Run `CamBridge.Config.exe`

3. **Grant Administrator Access** (if prompted)
   - Service control requires elevated privileges
   - Click "Yes" when UAC prompt appears

### First Time Setup

1. **Check Service Status**
   - Dashboard should show "Running" status
   - If not, navigate to Service Control page

2. **Verify API Connection**
   - Green status indicator = Connected
   - Red indicator = Check if service is running

3. **Configure Watch Folders**
   - Go to Settings page
   - Add folders to monitor for JPEG files

## Usage Guide

### Dashboard

The dashboard provides an overview of the service status:

- **Queue Length**: Files waiting to be processed
- **Active Processing**: Files currently being converted
- **Success Count**: Successfully converted files
- **Error Count**: Failed conversions

**Active Processing** section shows:
- File names being processed
- Processing duration
- Retry attempt count

**Recent Failed Items** displays:
- Failed file names
- Error messages
- Reprocess button for each item

### Service Control

Control the Windows Service:

- **Start Service**: Starts the CamBridge Service
- **Stop Service**: Gracefully stops processing
- **Restart Service**: Stops and restarts the service

**Note**: These operations require administrator privileges.

### Troubleshooting

#### GUI won't connect to service

1. Verify service is running:
   ```powershell
   sc query CamBridgeService
   ```

2. Check API endpoint:
   - Open browser to http://localhost:5050/api/status
   - Should return JSON status

3. Check Windows Firewall:
   - Ensure port 5050 is not blocked

#### "Administrator Required" messages

The GUI needs elevated privileges for:
- Starting/stopping the service
- Installing/uninstalling service
- Modifying service configuration

To run as administrator:
1. Right-click `CamBridge.Config.exe`
2. Select "Run as administrator"

#### No data showing in dashboard

1. Check service logs:
   - Event Viewer → Application Log → CamBridge Service

2. Verify watch folders exist and are accessible

3. Check if test JPEG files have proper EXIF data

## API Endpoints

The GUI communicates with these service endpoints:

- `GET /api/status` - Service status
- `GET /api/status/statistics` - Processing statistics
- `GET /api/status/deadletters` - Failed items
- `POST /api/status/deadletters/{id}/reprocess` - Reprocess item
- `GET /api/status/health` - Health check

## Keyboard Shortcuts

- `F5` - Refresh dashboard
- `Ctrl+S` - Open Service Control
- `Ctrl+D` - Open Dead Letters
- `Alt+F4` - Exit application

## Configuration Files

The GUI reads configuration from:
- `appsettings.json` - Service settings
- `mappings.json` - EXIF to DICOM mappings

## Support

For issues or questions:
1. Check Windows Event Log for errors
2. Review service logs in `C:\CamBridge\Logs`
3. Contact support with error details

## Version History

- **0.4.0** - Initial GUI release with live monitoring
- **0.3.x** - Service-only versions

---

*Part of the CamBridge JPEG to DICOM conversion system*
