// src/CamBridge.Config/Services/IServiceManager.cs
using System;
using System.Threading.Tasks;

namespace CamBridge.Config.Services
{
    /// <summary>
    /// Interface for Windows Service management
    /// </summary>
    public interface IServiceManager
    {
        /// <summary>
        /// Checks if the CamBridge Service is installed
        /// </summary>
        Task<bool> IsServiceInstalledAsync();

        /// <summary>
        /// Gets the current service status
        /// </summary>
        Task<ServiceStatus> GetServiceStatusAsync();

        /// <summary>
        /// Starts the CamBridge Service
        /// </summary>
        Task<bool> StartServiceAsync();

        /// <summary>
        /// Stops the CamBridge Service
        /// </summary>
        Task<bool> StopServiceAsync();

        /// <summary>
        /// Restarts the CamBridge Service
        /// </summary>
        Task<bool> RestartServiceAsync();

        /// <summary>
        /// Gets the service start time if running
        /// </summary>
        Task<DateTime?> GetServiceStartTimeAsync();

        /// <summary>
        /// Checks if the application is running with administrator privileges
        /// </summary>
        bool IsRunningAsAdministrator();
    }

    /// <summary>
    /// Service status enumeration
    /// </summary>
    public enum ServiceStatus
    {
        Stopped,
        Starting,
        Running,
        Stopping,
        Unknown
    }
}
