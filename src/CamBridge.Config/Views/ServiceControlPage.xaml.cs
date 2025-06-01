// src/CamBridge.Config/Views/ServiceControlPage.xaml.cs
using System.Runtime.Versioning;
using System.Windows;
using System.Windows.Controls;
using CamBridge.Config.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace CamBridge.Config.Views
{
    [SupportedOSPlatform("windows")]
    public partial class ServiceControlPage : Page
    {
        private readonly ServiceControlViewModel _viewModel;

        public ServiceControlPage()
        {
            InitializeComponent();

            // Get ViewModel from DI
            _viewModel = ((App)Application.Current).Host.Services.GetRequiredService<ServiceControlViewModel>();
            DataContext = _viewModel;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            _viewModel?.Cleanup();
        }
    }
}
