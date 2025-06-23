using System.Threading.Tasks;

namespace CamBridge.Config.Services
{
    public interface IConfigurationService
    {
        Task<T?> LoadConfigurationAsync<T>() where T : class;
        Task SaveConfigurationAsync<T>(T configuration) where T : class;
    }
}
