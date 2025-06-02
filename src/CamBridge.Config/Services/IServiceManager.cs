// src/CamBridge.Config/Services/IServiceManager.cs
using System;
using System.Threading.Tasks;

namespace CamBridge.Config.Services
{
    /// <summary>
    /// Interface for managing the CamBridge Windows Service
    /// </summary>
    public interface IServiceManager
    {
        /// <summary>
        /// Checks if the application is running with administrator privileges
        /// </summary>
        bool IsRunningAsAdministrator();

        /// <summary>
        /// Checks if the CamBridge service is installed
        /// </summary>
        Task<bool> IsServiceInstalledAsync();

        /// <summary>
        /// Gets the current status of the CamBridge service
        /// </summary>
        Task<ServiceStatus> GetServiceStatusAsync();

        /// <summary>
        /// Gets the start time of the service if it's running
        /// </summary>
        Task<DateTime?> GetServiceStartTimeAsync();

        /// <summary>
        /// Installs the CamBridge service
        /// </summary>
        Task<bool> InstallServiceAsync();

        /// <summary>
        /// Uninstalls the CamBridge service
        /// </summary>
        Task<bool> UninstallServiceAsync();

        /// <summary>
        /// Starts the CamBridge service
        /// </summary>
        Task<bool> StartServiceAsync();

        /// <summary>
        /// Stops the CamBridge service
        /// </summary>
        Task<bool> StopServiceAsync();

        /// <summary>
        /// Restarts the CamBridge service
        /// </summary>
        Task<bool> RestartServiceAsync();
    }

    /// <summary>
    /// Service status enumeration
    /// </summary>
    public enum ServiceStatus
    {
        Unknown,
        Running,
        Stopped,
        Starting,
        Stopping
    }
}
