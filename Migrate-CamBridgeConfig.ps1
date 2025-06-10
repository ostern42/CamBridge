# Migrate-CamBridgeConfig.ps1
# Version: 0.7.1
# Description: Migrates CamBridge configuration to centralized location
# Copyright: © 2025 Claude's Improbably Reliable Software Solutions

<#
.SYNOPSIS
    Migrates CamBridge configuration files to the centralized ProgramData location.

.DESCRIPTION
    This script searches for existing CamBridge configuration files in various locations
    and migrates them to the new centralized location in %ProgramData%\CamBridge.
    It creates backups and handles conflicts intelligently.

.EXAMPLE
    .\Migrate-CamBridgeConfig.ps1
    
.EXAMPLE
    .\Migrate-CamBridgeConfig.ps1 -WhatIf
#>

[CmdletBinding(SupportsShouldProcess)]
param(
    [switch]$Force
)

# Configuration
$CompanyName = "CamBridge"
$ConfigFileName = "appsettings.json"
$MappingsFileName = "mappings.json"

# Paths
$ProgramDataPath = [Environment]::GetFolderPath([Environment+SpecialFolder]::CommonApplicationData)
$TargetConfigDir = Join-Path $ProgramDataPath $CompanyName
$TargetConfigPath = Join-Path $TargetConfigDir $ConfigFileName
$TargetMappingsPath = Join-Path $TargetConfigDir $MappingsFileName

# Ensure target directory exists
if (-not (Test-Path $TargetConfigDir)) {
    New-Item -ItemType Directory -Path $TargetConfigDir -Force | Out-Null
    Write-Host "Created directory: $TargetConfigDir" -ForegroundColor Green
}

Write-Host "`nCamBridge Configuration Migration Script v0.7.1" -ForegroundColor Cyan
Write-Host "=============================================" -ForegroundColor Cyan
Write-Host "Target location: $TargetConfigDir`n" -ForegroundColor Yellow

# Search locations for existing configs
$SearchLocations = @(
    # Service installations
    @{ Path = "C:\CamBridge\Service"; Type = "Service Installation" }
    @{ Path = "C:\Program Files\CamBridge"; Type = "Program Files" }
    @{ Path = "C:\Program Files (x86)\CamBridge"; Type = "Program Files (x86)" }
    
    # Development locations
    @{ Path = Join-Path $env:USERPROFILE "source\repos\CamBridge\src\CamBridge.Service"; Type = "Development" }
    @{ Path = Join-Path $env:USERPROFILE "source\repos\CamBridge\Deploy"; Type = "Deployment" }
    
    # User-specific locations
    @{ Path = Join-Path $env:APPDATA $CompanyName; Type = "User AppData" }
    @{ Path = Join-Path $env:LOCALAPPDATA $CompanyName; Type = "Local AppData" }
    
    # Current directory
    @{ Path = Get-Location; Type = "Current Directory" }
)

# Find all config files
$FoundConfigs = @()
$FoundMappings = @()

Write-Host "Searching for configuration files..." -ForegroundColor Cyan

foreach ($location in $SearchLocations) {
    if (Test-Path $location.Path) {
        # Search for appsettings.json
        $configs = Get-ChildItem -Path $location.Path -Filter $ConfigFileName -Recurse -ErrorAction SilentlyContinue
        foreach ($config in $configs) {
            $FoundConfigs += @{
                Path = $config.FullName
                Location = $location.Type
                LastModified = $config.LastWriteTime
                Size = $config.Length
            }
        }
        
        # Search for mappings.json
        $mappings = Get-ChildItem -Path $location.Path -Filter $MappingsFileName -Recurse -ErrorAction SilentlyContinue
        foreach ($mapping in $mappings) {
            $FoundMappings += @{
                Path = $mapping.FullName
                Location = $mapping.Type
                LastModified = $mapping.LastWriteTime
                Size = $mapping.Length
            }
        }
    }
}

# Display found files
if ($FoundConfigs.Count -eq 0) {
    Write-Host "`nNo configuration files found." -ForegroundColor Yellow
} else {
    Write-Host "`nFound $($FoundConfigs.Count) configuration file(s):" -ForegroundColor Green
    foreach ($config in $FoundConfigs) {
        Write-Host "  - $($config.Path)" -ForegroundColor White
        Write-Host "    Location: $($config.Location), Modified: $($config.LastModified), Size: $($config.Size) bytes" -ForegroundColor Gray
    }
}

if ($FoundMappings.Count -gt 0) {
    Write-Host "`nFound $($FoundMappings.Count) mapping file(s):" -ForegroundColor Green
    foreach ($mapping in $FoundMappings) {
        Write-Host "  - $($mapping.Path)" -ForegroundColor White
        Write-Host "    Location: $($mapping.Location), Modified: $($mapping.LastModified)" -ForegroundColor Gray
    }
}

# Check if target already exists
$TargetExists = Test-Path $TargetConfigPath
if ($TargetExists) {
    $targetInfo = Get-Item $TargetConfigPath
    Write-Host "`nTarget configuration already exists:" -ForegroundColor Yellow
    Write-Host "  $TargetConfigPath" -ForegroundColor White
    Write-Host "  Modified: $($targetInfo.LastWriteTime), Size: $($targetInfo.Length) bytes" -ForegroundColor Gray
    
    if (-not $Force) {
        $response = Read-Host "`nDo you want to backup and replace it? (Y/N)"
        if ($response -ne 'Y' -and $response -ne 'y') {
            Write-Host "Migration cancelled." -ForegroundColor Red
            return
        }
    }
}

# Find the most recent config to migrate
if ($FoundConfigs.Count -gt 0) {
    $MostRecentConfig = $FoundConfigs | Sort-Object LastModified -Descending | Select-Object -First 1
    
    Write-Host "`nSelected for migration (most recent):" -ForegroundColor Cyan
    Write-Host "  $($MostRecentConfig.Path)" -ForegroundColor White
    Write-Host "  Modified: $($MostRecentConfig.LastModified)" -ForegroundColor Gray
    
    # Create backup if target exists
    if ($TargetExists) {
        $backupDir = Join-Path $TargetConfigDir "Backup"
        if (-not (Test-Path $backupDir)) {
            New-Item -ItemType Directory -Path $backupDir -Force | Out-Null
        }
        
        $timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
        $backupPath = Join-Path $backupDir "appsettings_$timestamp.json"
        
        if ($PSCmdlet.ShouldProcess($TargetConfigPath, "Backup to $backupPath")) {
            Copy-Item -Path $TargetConfigPath -Destination $backupPath -Force
            Write-Host "`nBacked up existing config to:" -ForegroundColor Green
            Write-Host "  $backupPath" -ForegroundColor White
        }
    }
    
    # Migrate the config
    if ($PSCmdlet.ShouldProcess($MostRecentConfig.Path, "Migrate to $TargetConfigPath")) {
        Copy-Item -Path $MostRecentConfig.Path -Destination $TargetConfigPath -Force
        Write-Host "`nMigrated configuration successfully!" -ForegroundColor Green
        
        # Also migrate mappings.json if found in same directory
        $sourceDir = Split-Path $MostRecentConfig.Path -Parent
        $sourceMappings = Join-Path $sourceDir $MappingsFileName
        if (Test-Path $sourceMappings) {
            Copy-Item -Path $sourceMappings -Destination $TargetMappingsPath -Force
            Write-Host "Also migrated mappings.json" -ForegroundColor Green
        }
    }
}

# Create default mappings if not exists
if (-not (Test-Path $TargetMappingsPath)) {
    # Find any mappings file to copy
    if ($FoundMappings.Count -gt 0) {
        $MostRecentMapping = $FoundMappings | Sort-Object LastModified -Descending | Select-Object -First 1
        if ($PSCmdlet.ShouldProcess($MostRecentMapping.Path, "Copy to $TargetMappingsPath")) {
            Copy-Item -Path $MostRecentMapping.Path -Destination $TargetMappingsPath -Force
            Write-Host "Copied mappings.json to target" -ForegroundColor Green
        }
    }
}

# Final summary
Write-Host "`n=============================================" -ForegroundColor Cyan
Write-Host "Migration Summary:" -ForegroundColor Cyan
Write-Host "  Configuration: $(if (Test-Path $TargetConfigPath) { 'OK' } else { 'MISSING' })" -ForegroundColor $(if (Test-Path $TargetConfigPath) { 'Green' } else { 'Red' })
Write-Host "  Mappings: $(if (Test-Path $TargetMappingsPath) { 'OK' } else { 'MISSING' })" -ForegroundColor $(if (Test-Path $TargetMappingsPath) { 'Green' } else { 'Yellow' })
Write-Host "`nTarget directory: $TargetConfigDir" -ForegroundColor Yellow

# Service restart reminder
Write-Host "`n⚠️  IMPORTANT: Restart CamBridge Service for changes to take effect!" -ForegroundColor Yellow
Write-Host "  Run: Restart-Service CamBridgeService" -ForegroundColor White

Write-Host "`nMigration completed!" -ForegroundColor Green