// src/CamBridge.Config/Views/SettingsPage.xaml.cs
using System.Runtime.Versioning;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CamBridge.Config.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;

namespace CamBridge.Config.Views
{
    [SupportedOSPlatform("windows")]
    public partial class SettingsPage : Page
    {
        private readonly SettingsViewModel _viewModel;

        public SettingsPage()
        {
            InitializeComponent();

            // Get ViewModel from DI
            _viewModel = ((App)Application.Current).Host.Services.GetRequiredService<SettingsViewModel>();
            DataContext = _viewModel;

            // Initialize the view model
            _ = _viewModel.InitializeAsync();
        }

        // Number validation for TextBox inputs
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        // Browse folder dialogs
        private void BrowseWatchFolder_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Select Watch Folder",
                CheckFileExists = false,
                CheckPathExists = true,
                FileName = "Select Folder",
                Filter = "Folder|*.none",
                ValidateNames = false
            };

            if (dialog.ShowDialog() == true)
            {
                string? folderPath = System.IO.Path.GetDirectoryName(dialog.FileName);
                if (_viewModel.SelectedWatchFolder != null && !string.IsNullOrEmpty(folderPath))
                {
                    _viewModel.SelectedWatchFolder.Path = folderPath;
                }
            }
        }

        private void BrowseOutputFolder_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Select Output Folder",
                CheckFileExists = false,
                CheckPathExists = true,
                FileName = "Select Folder",
                Filter = "Folder|*.none",
                ValidateNames = false
            };

            if (dialog.ShowDialog() == true)
            {
                string? folderPath = System.IO.Path.GetDirectoryName(dialog.FileName);
                if (!string.IsNullOrEmpty(folderPath))
                {
                    _viewModel.DefaultOutputFolder = folderPath;
                }
            }
        }

        private void BrowseLogFolder_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Select Log Folder",
                CheckFileExists = false,
                CheckPathExists = true,
                FileName = "Select Folder",
                Filter = "Folder|*.none",
                ValidateNames = false
            };

            if (dialog.ShowDialog() == true)
            {
                string? folderPath = System.IO.Path.GetDirectoryName(dialog.FileName);
                if (!string.IsNullOrEmpty(folderPath))
                {
                    _viewModel.LogFolder = folderPath;
                }
            }
        }
    }
}
