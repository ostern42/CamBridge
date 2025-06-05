# CamBridge Service Deployment Package v0.5.29

## Quick Start

1. **Extract** the deployment package to a temporary folder
2. **Right-click** `Install-CamBridge.ps1` and select "Run with PowerShell as Administrator"
3. Follow the installation wizard
4. Service will be installed and Configuration UI will launch

## Package Contents

```
CamBridge-Deploy-v0.5.29/
├── Install-CamBridge.ps1       # Installation script
├── Uninstall-CamBridge.ps1     # Uninstallation script
├── Service/                    # Service files
│   ├── CamBridge.Service.exe
│   ├── appsettings.json
│   ├── mappings.json
│   └── Tools/
│       └── exiftool.exe
└── Config/                     # Configuration UI
    └── CamBridge.Config.exe
```

## Requirements

- Windows 10/11 or Windows Server 2019/2022
- .NET 8.0 Runtime (installer will prompt if missing)
- Administrator privileges for installation

## Installation Options

### Default Installation
```powershell
.\Install-CamBridge.ps1
```

### Custom Installation Path
```powershell
.\Install-CamBridge.ps1 -InstallPath "D:\Apps\CamBridge" -DataPath "D:\CamBridgeData"
```

## Service Management

### Using Configuration UI
- Launch from Desktop shortcut or Start Menu
- Service Control page allows Start/Stop/Restart

### Using Windows Services
- Open `services.msc`
- Find "CamBridge Image Processing Service"
- Right-click for Start/Stop/Restart options

### Using PowerShell
```powershell
# Status
Get-Service CamBridgeService

# Start
Start-Service CamBridgeService

# Stop
Stop-Service CamBridgeService

# Restart
Restart-Service CamBridgeService
```

## Default Folders

After installation, these folders are created:
- `C:\CamBridge\Input` - Watch folder for JPEG files
- `C:\CamBridge\Output` - Converted DICOM files
- `C:\CamBridge\Archive` - Processed files (if archiving enabled)
- `C:\ProgramData\CamBridge\Logs` - Service logs

## Configuration

1. Launch Configuration UI
2. Go to Settings page
3. Configure:
   - Watch folders
   - Output paths
   - Processing options
   - Notification settings

## Monitoring

### Check Service Status
```powershell
Invoke-WebRequest http://localhost:5050/api/status | ConvertFrom-Json
```

### View Logs
- Via Configuration UI: Help → View Logs
- Direct: `C:\ProgramData\CamBridge\Logs`

## Uninstallation

### Keep Data
```powershell
.\Uninstall-CamBridge.ps1 -KeepData
```

### Complete Removal
```powershell
.\Uninstall-CamBridge.ps1
```

## Troubleshooting

### Service won't start
1. Check Event Viewer → Windows Logs → Application
2. Look for "CamBridgeService" entries
3. Verify `appsettings.json` exists in install directory
4. Ensure ExifTool is in Tools subfolder

### Config UI can't connect
1. Verify service is running
2. Check if port 5050 is blocked by firewall
3. Try: `http://localhost:5050/api/status` in browser

### Files not processing
1. Check watch folder permissions
2. Verify JPEG files have EXIF data
3. Review logs for errors

## Support

- Documentation: [Internal Wiki]
- Issues: Contact IT Support
- Version: 0.5.29
- © 2025 Claude's Improbably Reliable Software Solutions
