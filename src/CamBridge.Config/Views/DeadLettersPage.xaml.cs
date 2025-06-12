// src\CamBridge.Config\Views\DeadLettersPage.xaml.cs
// Version: 0.7.8
// Description: Simple error folder page - KISS approach!
// Copyright: © 2025 Claude's Improbably Reliable Software Solutions

using CamBridge.Config.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using System.Windows.Controls;

namespace CamBridge.Config.Views
{
    /// <summary>
    /// Simple error folder page - no more dead letters!
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
                    // Fallback: Create directly - new ViewModel has parameterless constructor!
                    _viewModel = new DeadLettersViewModel();
                    DataContext = _viewModel;

                    System.Diagnostics.Debug.WriteLine("DeadLettersViewModel created manually");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating DeadLettersViewModel: {ex.Message}");
                ShowError("Failed to initialize Error Folder View", ex.Message);
            }
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (_viewModel == null) return;

            try
            {
                // Refresh the error folder status
                if (_viewModel.RefreshCommand?.CanExecute(null) == true)
                {
                    await _viewModel.RefreshCommand.ExecuteAsync(null);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error refreshing error folder: {ex.Message}");
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            // Clean up - remove event handler
            Loaded -= Page_Loaded;

            // Cleanup ViewModel
            _viewModel?.Cleanup();

            // Clear ViewModel reference
            _viewModel = null;
        }

        private void ShowError(string title, string message)
        {
            MessageBox.Show(
                $"{message}\n\nError files are now managed through Windows Explorer.",
                title,
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
    }
}
