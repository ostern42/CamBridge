// src/CamBridge.Config/Views/DashboardPage.xaml.cs
// Version: 0.6.7
// Copyright: © 2025 Claude's Improbably Reliable Software Solutions

using System;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using CamBridge.Config.Services;
using CamBridge.Config.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

namespace CamBridge.Config.Views
{
    /// <summary>
    /// Interaction logic for DashboardPage.xaml
    /// </summary>
    public partial class DashboardPage : Page
    {
        private DashboardViewModel? _viewModel;
        private HttpClient? _httpClient;

        public DashboardPage()
        {
            InitializeComponent();

            // Force a complete refresh - alte Version könnte im Cache sein
            System.Diagnostics.Debug.WriteLine("=== NEW MULTI-PIPELINE DASHBOARD LOADING ===");

            DataContextChanged += OnDataContextChanged;
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _viewModel = DataContext as DashboardViewModel;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Get the ViewModel from DI container if not already set
            if (_viewModel == null)
            {
                var app = Application.Current as App;
                if (app?.Host?.Services != null)
                {
                    try
                    {
                        _viewModel = app.Host.Services.GetRequiredService<DashboardViewModel>();
                        DataContext = _viewModel;
                    }
                    catch
                    {
                        // Fallback if DI fails
                        CreateViewModelManually();
                    }
                }
                else
                {
                    // No DI container available
                    CreateViewModelManually();
                }
            }

            // Debug output to verify correct version loaded
            System.Diagnostics.Debug.WriteLine($"Dashboard loaded - Multi-Pipeline: {_viewModel?.PipelineStatuses != null}");
            System.Diagnostics.Debug.WriteLine($"Pipeline count: {_viewModel?.PipelineStatuses?.Count ?? 0}");

            // Force immediate refresh
            if (_viewModel != null)
            {
                _ = _viewModel.RefreshDataCommand.ExecuteAsync(null);
            }
        }

        private void CreateViewModelManually()
        {
            try
            {
                // Create services manually
                _httpClient = new HttpClient();
                _httpClient.BaseAddress = new Uri("http://localhost:5111/");

                var nullLogger = new NullLogger<HttpApiService>();
                var apiService = new HttpApiService(_httpClient, nullLogger);

                _viewModel = new DashboardViewModel(apiService);
                DataContext = _viewModel;

                System.Diagnostics.Debug.WriteLine("Dashboard ViewModel created manually");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to create Dashboard ViewModel: {ex.Message}");
            }
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            // Clean up timer when page is unloaded
            _viewModel?.Cleanup();

            // Dispose HttpClient if we created it manually
            _httpClient?.Dispose();
            _httpClient = null;
        }
    }
}
