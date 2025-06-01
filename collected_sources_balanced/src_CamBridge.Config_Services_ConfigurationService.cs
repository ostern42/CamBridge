using CamBridge.Core;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CamBridge.Config.Services
{
    /// <summary>
    /// Service for loading and saving CamBridge configuration from/to appsettings.json
    /// </summary>
    public class ConfigurationService : IConfigurationService
    {
        private readonly string _configPath;
        private readonly JsonSerializerOptions _jsonOptions;

        public ConfigurationService()
        {
            // Find the appsettings.json file
            var serviceDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                "CamBridge", "Service"
            );

            _configPath = Path.Combine(serviceDir, "appsettings.json");

            // If not found, try current directory (for development)
            if (!File.Exists(_configPath))
            {
                _configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
            }

            // Configure JSON serialization options
            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter() },
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
        }

        public async Task<T?> LoadConfigurationAsync<T>() where T : class
        {
            try
            {
                if (!File.Exists(_configPath))
                {
                    if (typeof(T) == typeof(CamBridgeSettings))
                    {
                        return new CamBridgeSettings() as T;
                    }
                    return null;
                }

                var json = await File.ReadAllTextAsync(_configPath);
                var config = JsonSerializer.Deserialize<AppSettings>(json, _jsonOptions);

                if (typeof(T) == typeof(CamBridgeSettings))
                {
                    return config?.CamBridge as T;
                }

                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading configuration: {ex.Message}");

                if (typeof(T) == typeof(CamBridgeSettings))
                {
                    return new CamBridgeSettings() as T;
                }
                return null;
            }
        }

        public async Task SaveConfigurationAsync<T>(T configuration) where T : class
        {
            try
            {
                AppSettings appSettings;

                if (File.Exists(_configPath))
                {
                    var json = await File.ReadAllTextAsync(_configPath);
                    appSettings = JsonSerializer.Deserialize<AppSettings>(json, _jsonOptions) ?? new AppSettings();
                }
                else
                {
                    appSettings = new AppSettings();
                }

                if (configuration is CamBridgeSettings camBridgeSettings)
                {
                    appSettings.CamBridge = camBridgeSettings;
                }

                // Create backup of existing file
                if (File.Exists(_configPath))
                {
                    var backupPath = _configPath + ".backup";
                    File.Copy(_configPath, backupPath, true);
                }

                var updatedJson = JsonSerializer.Serialize(appSettings, _jsonOptions);
                await File.WriteAllTextAsync(_configPath, updatedJson);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving configuration: {ex.Message}");
                throw new InvalidOperationException("Failed to save configuration", ex);
            }
        }

        private class AppSettings
        {
            public LoggingSection? Logging { get; set; }
            public CamBridgeSettings CamBridge { get; set; } = new();
        }

        private class LoggingSection
        {
            public Dictionary<string, string>? LogLevel { get; set; }
        }
    }
}
