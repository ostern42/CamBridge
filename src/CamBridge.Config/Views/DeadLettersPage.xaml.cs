// src\CamBridge.Config\Views\DeadLettersPage.xaml.cs
// Version: 0.5.26
// Fixed: Using correct command names from updated ViewModel

using CamBridge.Config.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using System.Windows.Controls;

namespace CamBridge.Config.Views
{
    /// <summary>
    /// Dead letters page showing failed file processing attempts
    /// </summary>
    public partial class DeadLettersPage : Page
    {
        private DeadLettersViewModel? _viewModel;

        public DeadLettersPage()
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
                    _viewModel = app.Host.Services.GetRequiredService<DeadLettersViewModel>();
                    DataContext = _viewModel;

                    System.Diagnostics.Debug.WriteLine("DeadLettersViewModel loaded from DI container");
                }
                else
                {
                    // Fallback: Create with HttpClient
                    var httpClient = new System.Net.Http.HttpClient
                    {
                        BaseAddress = new Uri("http://localhost:5050/"),
                        Timeout = TimeSpan.FromSeconds(30)
                    };

                    var apiService = new Services.HttpApiService(httpClient, null!);
                    _viewModel = new DeadLettersViewModel(apiService);
                    DataContext = _viewModel;

                    System.Diagnostics.Debug.WriteLine("DeadLettersViewModel created manually");
                }

                // Load data when page loads
                Loaded += DeadLettersPage_Loaded;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating DeadLettersViewModel: {ex.Message}");
                ShowError("Failed to initialize Dead Letters", ex.Message);
            }
        }

        private async void DeadLettersPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (_viewModel == null) return;

            try
            {
                // The commands are named WITHOUT "Async" suffix!
                // Method: LoadDeadLettersAsync -> Command: LoadDeadLettersCommand
                if (_viewModel.LoadDeadLettersCommand?.CanExecute(null) == true)
                {
                    await _viewModel.LoadDeadLettersCommand.ExecuteAsync(null);
                }
                else if (_viewModel.RefreshCommand?.CanExecute(null) == true)
                {
                    // Alternative: Try RefreshCommand
                    await _viewModel.RefreshCommand.ExecuteAsync(null);
                }
                else
                {
                    // Fallback: Call the method directly
                    await _viewModel.LoadDeadLettersAsync();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading dead letters: {ex.Message}");
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            // Clean up - remove event handler
            Loaded -= DeadLettersPage_Loaded;

            // Cleanup ViewModel
            _viewModel?.Cleanup();

            // Clear ViewModel reference
            _viewModel = null;
        }

        private void ShowError(string title, string message)
        {
            MessageBox.Show(
                $"{message}\n\nPlease ensure the CamBridge Service is running.",
                title,
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }
    }
}
