using CamBridge.Core;
using System;
using System.Threading.Tasks;

namespace CamBridge.Config.Services
{
    /// <summary>
    /// Minimal implementation for testing
    /// </summary>
    public class ConfigurationService : IConfigurationService
    {
        public async Task<T?> LoadConfigurationAsync<T>() where T : class
        {
            await Task.Delay(100); // Simulate async work

            if (typeof(T) == typeof(CamBridgeSettings))
            {
                // Return default settings for testing
                var settings = new CamBridgeSettings
                {
                    DefaultOutputFolder = @"C:\CamBridge\Output",
                    Processing = new ProcessingOptions
                    {
                        ArchiveFolder = @"C:\CamBridge\Archive",
                        ErrorFolder = @"C:\CamBridge\Errors",
                        BackupFolder = @"C:\CamBridge\Backup"
                    },
                    Dicom = new DicomSettings
                    {
                        ImplementationClassUid = "1.2.276.0.7230010.3.0.3.6.4",
                        ImplementationVersionName = "CAMBRIDGE_001"
                    },
                    Logging = new LoggingSettings
                    {
                        LogFolder = @"C:\CamBridge\Logs"
                    },
                    Notifications = new NotificationSettings()
                };

                return settings as T;
            }

            return null;
        }

        public async Task SaveConfigurationAsync<T>(T configuration) where T : class
        {
            await Task.Delay(100); // Simulate async work
            // For testing, just pretend we saved
        }
    }
}
