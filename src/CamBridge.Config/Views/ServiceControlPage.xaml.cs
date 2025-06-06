// src\CamBridge.Config\Views\ServiceControlPage.xaml.cs
// Version: 0.5.26 - Fixed: Using Cleanup() instead of Dispose()

using CamBridge.Config.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using System.Windows.Controls;

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

            // Get ViewModel from DI container
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
                    _viewModel = new ServiceControlViewModel(serviceManager);
                    DataContext = _viewModel;

                    System.Diagnostics.Debug.WriteLine("ServiceControlViewModel created manually (fallback)");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating ServiceControlViewModel: {ex.Message}");
            }
        }

        // Fixed: Using Cleanup() instead of Dispose()
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            // Cleanup the ViewModel
            _viewModel?.Cleanup();
            _viewModel = null;

            System.Diagnostics.Debug.WriteLine("ServiceControlPage cleanup completed");
        }
    }
}
