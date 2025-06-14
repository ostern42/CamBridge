# Emergency-Fix-CamBridge.ps1
# © 2025 Claude's Improbably Reliable Software Solutions
# CRITICAL: Fix broken JSON configuration NOW!

Write-Host "=== CamBridge Emergency JSON Fix ===" -ForegroundColor Red
Write-Host "This script will fix the OutputOrganization error" -ForegroundColor Yellow

$configPath = "$env:ProgramData\CamBridge\appsettings.json"

# Stop service first
Write-Host "`nStopping CamBridge Service..." -ForegroundColor Yellow
try {
    Stop-Service CamBridgeService -ErrorAction SilentlyContinue
    Write-Host "✓ Service stopped" -ForegroundColor Green
} catch {
    Write-Host "⚠ Service was not running" -ForegroundColor Yellow
}

# Backup current config
$backupPath = "$configPath.broken_$(Get-Date -Format 'yyyyMMdd_HHmmss')"
if (Test-Path $configPath) {
    Write-Host "`nBacking up broken config to: $backupPath" -ForegroundColor Yellow
    Copy-Item $configPath $backupPath
    Write-Host "✓ Backup created" -ForegroundColor Green
}

# Option 1: Try to fix the existing JSON
Write-Host "`n=== Attempting to repair existing JSON ===" -ForegroundColor Cyan
$fixed = $false

try {
    $content = Get-Content $configPath -Raw
    
    # Common fixes:
    # 1. Replace invalid OutputOrganization values
    $content = $content -replace '"OutputOrganization"\s*:\s*"Flat"', '"OutputOrganization": "None"'
    $content = $content -replace '"OutputOrganization"\s*:\s*"YearMonth"', '"OutputOrganization": "ByDate"'
    $content = $content -replace '"OutputOrganization"\s*:\s*"PatientStudy"', '"OutputOrganization": "ByPatient"'
    $content = $content -replace '"OutputOrganization"\s*:\s*"DatePatient"', '"OutputOrganization": "ByPatientAndDate"'
    $content = $content -replace '"OutputOrganization"\s*:\s*null', '"OutputOrganization": "None"'
    $content = $content -replace '"OutputOrganization"\s*:\s*0', '"OutputOrganization": "None"'
    $content = $content -replace '"OutputOrganization"\s*:\s*1', '"OutputOrganization": "ByPatient"'
    $content = $content -replace '"OutputOrganization"\s*:\s*2', '"OutputOrganization": "ByDate"'
    $content = $content -replace '"OutputOrganization"\s*:\s*3', '"OutputOrganization": "ByPatientAndDate"'
    
    # Save fixed content
    $content | Set-Content $configPath -Encoding UTF8
    
    # Test if it's valid JSON now
    $null = $content | ConvertFrom-Json
    Write-Host "✓ JSON repaired successfully!" -ForegroundColor Green
    $fixed = $true
} catch {
    Write-Host "✗ Could not repair JSON: $_" -ForegroundColor Red
}

# Option 2: If repair failed, create minimal working config
if (-not $fixed) {
    Write-Host "`n=== Creating new minimal configuration ===" -ForegroundColor Cyan
    
    $minimalConfig = @{
        CamBridge = @{
            Version = "2.0"
            Service = @{
                ListenPort = 5111
                EnableApi = $true
            }
            Core = @{
                ExifToolPath = "Tools\exiftool.exe"
            }
            Pipelines = @(
                @{
                    Id = [Guid]::NewGuid().ToString()
                    Name = "Default Pipeline"
                    Enabled = $true
                    WatchSettings = @{
                        Path = "C:\CamBridge\Watch"
                        FilePattern = "*.jpg;*.jpeg"
                        OutputPath = "C:\CamBridge\Output"
                    }
                    ProcessingOptions = @{
                        SuccessAction = "Archive"
                        FailureAction = "MoveToError"
                        ArchiveFolder = "C:\CamBridge\Archive"
                        ErrorFolder = "C:\CamBridge\Errors"
                        MaxConcurrentProcessing = 2
                        RetryOnFailure = $true
                        MaxRetryAttempts = 3
                        OutputOrganization = "None"
                        ProcessExistingOnStartup = $true
                    }
                }
            )
        }
    }
    
    try {
        $minimalConfig | ConvertTo-Json -Depth 10 | Set-Content $configPath -Encoding UTF8
        Write-Host "✓ Created new minimal configuration" -ForegroundColor Green
        $fixed = $true
    } catch {
        Write-Host "✗ Failed to create new config: $_" -ForegroundColor Red
    }
}

# Start service again
if ($fixed) {
    Write-Host "`n=== Starting CamBridge Service ===" -ForegroundColor Cyan
    try {
        Start-Service CamBridgeService
        Start-Sleep -Seconds 2
        
        # Test if service is responding
        $response = Invoke-RestMethod -Uri "http://localhost:5111/api/status/version" -ErrorAction SilentlyContinue
        if ($response) {
            Write-Host "✓ Service started successfully!" -ForegroundColor Green
            Write-Host "✓ API responding on port 5111" -ForegroundColor Green
            Write-Host "`nCamBridge is now operational!" -ForegroundColor Green
        }
    } catch {
        Write-Host "⚠ Service started but API not responding yet" -ForegroundColor Yellow
        Write-Host "Check logs at: $env:ProgramData\CamBridge\logs\" -ForegroundColor Yellow
    }
} else {
    Write-Host "`n✗ Could not fix configuration!" -ForegroundColor Red
    Write-Host "Manual intervention required." -ForegroundColor Red
}

Write-Host "`n=== Summary ===" -ForegroundColor Cyan
Write-Host "Config path: $configPath" -ForegroundColor White
Write-Host "Backup path: $backupPath" -ForegroundColor White
Write-Host "Service URL: http://localhost:5111" -ForegroundColor White

# Show valid OutputOrganization values
Write-Host "`nValid OutputOrganization values:" -ForegroundColor Yellow
Write-Host "  - None              (no subfolder organization)" -ForegroundColor Green
Write-Host "  - ByPatient         (organize by patient ID)" -ForegroundColor Green
Write-Host "  - ByDate            (organize by study date)" -ForegroundColor Green
Write-Host "  - ByPatientAndDate  (organize by patient ID and date)" -ForegroundColor Green