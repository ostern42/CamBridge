using System.Threading.Tasks;

namespace CamBridge.Config.Services
{
    public interface IServiceManager
    {
        Task<bool> IsServiceInstalledAsync();
        Task<ServiceStatus> GetServiceStatusAsync();
        Task<bool> StartServiceAsync();
        Task<bool> StopServiceAsync();
        Task<bool> RestartServiceAsync();
    }

    public enum ServiceStatus
    {
        Stopped,
        Starting,
        Running,
        Stopping,
        Unknown
    }
}
