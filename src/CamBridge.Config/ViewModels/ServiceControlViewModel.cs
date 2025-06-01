using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;

namespace CamBridge.Config.ViewModels
{
    public partial class ServiceControlViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string serviceStatus = "Stopped";

        [ObservableProperty]
        private bool canStart = true;

        [ObservableProperty]
        private bool canStop = false;

        [RelayCommand]
        private async Task StartService()
        {
            IsLoading = true;
            await Task.Delay(1000); // Simulate service start
            ServiceStatus = "Running";
            CanStart = false;
            CanStop = true;
            IsLoading = false;
        }

        [RelayCommand]
        private async Task StopService()
        {
            IsLoading = true;
            await Task.Delay(1000); // Simulate service stop
            ServiceStatus = "Stopped";
            CanStart = true;
            CanStop = false;
            IsLoading = false;
        }
    }
}