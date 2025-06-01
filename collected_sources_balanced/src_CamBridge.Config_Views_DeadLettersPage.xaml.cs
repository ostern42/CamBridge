using System.Runtime.Versioning;
using System.Windows;
using System.Windows.Controls;
using CamBridge.Config.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace CamBridge.Config.Views
{
    [SupportedOSPlatform("windows")]
    public partial class DeadLettersPage : Page
    {
        private readonly DeadLettersViewModel _viewModel;

        public DeadLettersPage()
        {
            InitializeComponent();

            // Get ViewModel from DI
            _viewModel = ((App)Application.Current).Host.Services.GetRequiredService<DeadLettersViewModel>();
            DataContext = _viewModel;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            _viewModel?.Cleanup();
        }
    }
}
