// src/CamBridge.Config/Views/ServiceControlPage.xaml.cs
// Version: 0.8.6
// Modified: Session 96 - Fixed DI parameters

using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using CamBridge.Config.ViewModels;

namespace CamBridge.Config.Views
{
    /// <summary>
    /// Service control page for managing the Windows Service
    /// </summary>
    public partial class ServiceControlPage : Page
    {
        private ServiceControlViewModel? _viewModel;

        public ServiceControlPage()
        {
            InitializeComponent();

            try
            {
                var app = Application.Current as App;
                if (app?.Host?.Services != null)
                {
                    _viewModel = app.Host.Services.GetRequiredService<ServiceControlViewModel>();
                    DataContext = _viewModel;
                    System.Diagnostics.Debug.WriteLine("ServiceControlViewModel loaded from DI container");
                }
                else
                {
                    // Fallback if DI not available
                    var serviceManager = new Services.ServiceManager();
                    var configurationService = new Services.ConfigurationService();

                    _viewModel = new ServiceControlViewModel(serviceManager, configurationService);
                    DataContext = _viewModel;
                    System.Diagnostics.Debug.WriteLine("ServiceControlViewModel created with fallback");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating ServiceControlViewModel: {ex.Message}");
                MessageBox.Show(
                    "Failed to initialize Service Control page. Some features may not be available.",
                    "Initialization Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            _viewModel?.Cleanup();
        }
    }
}
