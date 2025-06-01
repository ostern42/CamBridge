// src/CamBridge.Config/Views/DashboardPage.xaml.cs
using System.Runtime.Versioning;
using System.Windows;
using System.Windows.Controls;
using CamBridge.Config.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace CamBridge.Config.Views
{
    [SupportedOSPlatform("windows")]
    public partial class DashboardPage : Page
    {
        private readonly DashboardViewModel _viewModel;

        public DashboardPage()
        {
            InitializeComponent();

            // Get ViewModel from DI
            _viewModel = ((App)Application.Current).Host.Services.GetRequiredService<DashboardViewModel>();
            DataContext = _viewModel;
        }

        // Clean up when page is unloaded
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            _viewModel?.Cleanup();
        }
    }
}
