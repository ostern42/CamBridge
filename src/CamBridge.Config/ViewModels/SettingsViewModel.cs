using CommunityToolkit.Mvvm.ComponentModel;

namespace CamBridge.Config.ViewModels
{
    public partial class SettingsViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string watchFolder = @"C:\CamBridge\Input";

        [ObservableProperty]
        private string outputFolder = @"C:\CamBridge\Output";

        [ObservableProperty]
        private bool autoStart = true;
    }
}