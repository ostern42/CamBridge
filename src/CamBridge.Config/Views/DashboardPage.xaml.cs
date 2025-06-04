// src\CamBridge.Config\Views\DashboardPage.xaml.cs
// Version: 0.5.26
// Complete dashboard implementation with proper DI and error handling

using CamBridge.Config.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using System.Windows.Controls;

namespace CamBridge.Config.Views
{
    /// <summary>
    /// Dashboard page showing real-time statistics and system status
    /// </summary>
    public partial class DashboardPage : Page
    {
        private DashboardViewModel? _viewModel;

        public DashboardPage()
        {
            InitializeComponent();
            InitializeViewModel();
        }

        private void InitializeViewModel()
        {
            try
            {
                var app = Application.Current as App;
                if (app?.Host?.Services != null)
                {
                    // Get ViewModel from DI container - all dependencies are properly injected
                    _viewModel = app.Host.Services.GetRequiredService<DashboardViewModel>();
                    DataContext = _viewModel;

                    System.Diagnostics.Debug.WriteLine("DashboardViewModel loaded from DI container");
                }
                else
                {
                    // Fallback: Create HttpClient with proper configuration
                    var httpClient = new System.Net.Http.HttpClient
                    {
                        BaseAddress = new Uri("http://localhost:5050/"),
                        Timeout = TimeSpan.FromSeconds(30)
                    };

                    // Create ApiService with HttpClient
                    var apiService = new Services.HttpApiService(httpClient, null);

                    // Create ViewModel
                    _viewModel = new DashboardViewModel(apiService);
                    DataContext = _viewModel;

                    System.Diagnostics.Debug.WriteLine("DashboardViewModel created manually with HttpClient");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating DashboardViewModel: {ex.Message}");
                ShowError("Failed to initialize Dashboard", ex.Message);
            }
        }

        private void ShowError(string title, string message)
        {
            MessageBox.Show(
                $"{message}\n\nPlease ensure the CamBridge Service is running on port 5050.",
                title,
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }
    }
}
