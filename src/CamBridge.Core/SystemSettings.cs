// src\CamBridge.Core\SystemSettings.cs
// Version: 0.7.3
// Description: System-wide settings (Service + Config Tool)
// © 2025 Claude's Improbably Reliable Software Solutions

using System;
using System.Collections.Generic;

namespace CamBridge.Core
{
    /// <summary>
    /// System-wide settings shared by Service and Config Tool
    /// Stored in: %ProgramData%\CamBridge\appsettings.json
    /// </summary>
    public class SystemSettings
    {
        public ServiceConfiguration Service { get; set; } = new();
        public CoreConfiguration Core { get; set; } = new();
        public LoggingConfiguration Logging { get; set; } = new();
        public DicomDefaultSettings DicomDefaults { get; set; } = new();
        public NotificationSettings Notifications { get; set; } = new();
    }

    /// <summary>
    /// Core system configuration
    /// </summary>
    public class CoreConfiguration
    {
        public string ExifToolPath { get; set; } = "Tools\\exiftool.exe";
        public int MaxGlobalConcurrency { get; set; } = 4;
        public string DefaultCulture { get; set; } = "de-DE";
        public string InstanceName { get; set; } = "CamBridge";
        public bool EnableDiagnostics { get; set; } = false;
        public string TempFolder { get; set; } = @"C:\CamBridge\Temp";
        public bool CleanupTempOnStartup { get; set; } = true;
    }

    /// <summary>
    /// Service-specific configuration (new structure)
    /// </summary>
    public class ServiceConfiguration
    {
        public string ServiceUrl { get; set; } = "http://localhost:5111";
        public bool EnableHealthChecks { get; set; } = true;
        public int HealthCheckIntervalSeconds { get; set; } = 30;
        public string ServiceDisplayName { get; set; } = "CamBridge Service";
        public string ServiceDescription { get; set; } = "JPEG to DICOM medical imaging converter";
        public int StartupDelaySeconds { get; set; } = 5;
        public int ShutdownTimeoutSeconds { get; set; } = 30;
        public bool EnableApi { get; set; } = true;
        public bool EnableSwagger { get; set; } = true;
    }

    /// <summary>
    /// Logging configuration (new structure)
    /// </summary>
    public class LoggingConfiguration
    {
        public string LogLevel { get; set; } = "Information";
        public string LogPath { get; set; } = @"C:\CamBridge\Logs";
        public int MaxLogFileSizeMB { get; set; } = 50;
        public int MaxLogFileCount { get; set; } = 10;
        public bool EnableConsoleLogging { get; set; } = true;
        public bool EnableFileLogging { get; set; } = true;
        public bool EnableEventLog { get; set; } = false;
        public string LogFileNamePattern { get; set; } = "cambridge_{Date}.log";
        public Dictionary<string, string> LogLevelOverrides { get; set; } = new()
        {
            { "Microsoft", "Warning" },
            { "System", "Warning" },
            { "Microsoft.Hosting.Lifetime", "Information" }
        };
    }

    /// <summary>
    /// DICOM default values for all conversions
    /// </summary>
    public class DicomDefaultSettings
    {
        public string ImplementationClassUID { get; set; } = "1.2.276.0.7230010.3.0.3.6.4";
        public string ImplementationVersionName { get; set; } = "CAMBRIDGE_001";
        public string DefaultModality { get; set; } = "OT";
        public string DefaultManufacturer { get; set; } = "Ricoh";
        public string DefaultManufacturerModelName { get; set; } = "G900 II";
        public string SourceApplicationEntityTitle { get; set; } = "CAMBRIDGE";
        public string DefaultInstitutionName { get; set; } = string.Empty;
        public string DefaultInstitutionAddress { get; set; } = string.Empty;

        // Character set configuration
        public string SpecificCharacterSet { get; set; } = "ISO_IR 100"; // Latin-1
        public bool UseUtf8 { get; set; } = false; // Future: UTF-8 support
    }
}
