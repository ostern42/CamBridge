// src\CamBridge.Config\Views\PipelineConfigPage.xaml.cs
// Version: 0.8.10
// Description: Pipeline Configuration page - Code Behind with Output Path Handler

using System;
using System.Runtime.Versioning;
using System.Windows;
using System.Windows.Controls;
using CamBridge.Config.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace CamBridge.Config.Views
{
    /// <summary>
    /// Pipeline Configuration page - Zero Global Settings!
    /// NavigationService ALREADY injects the ViewModel, so we just need to initialize it
    /// </summary>
    [SupportedOSPlatform("windows")]
    public partial class PipelineConfigPage : Page
    {
        public PipelineConfigPage()
        {
            InitializeComponent();

            // Use Loaded event instead of OnInitialized to ensure NavigationService has done its job
            this.Loaded += Page_Loaded;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Only run once
            this.Loaded -= Page_Loaded;

            Debug.WriteLine("=== PipelineConfigPage Loaded ===");

            // NavigationService should have already set our DataContext
            if (DataContext is PipelineConfigViewModel vm)
            {
                Debug.WriteLine("ViewModel found! Initializing...");
                try
                {
                    await vm.InitializeAsync();
                    Debug.WriteLine($"Initialization complete. Pipelines: {vm.Pipelines.Count}");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Initialization failed: {ex.Message}");
                    MessageBox.Show(
                        $"Failed to load pipeline configuration:\n{ex.Message}",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                }
            }
            else
            {
                Debug.WriteLine($"ERROR: DataContext is {DataContext?.GetType().Name ?? "null"} - expected PipelineConfigViewModel");

                // Fallback - try to get it ourselves
                var app = Application.Current as App;
                if (app?.Host?.Services != null)
                {
                    var viewModel = app.Host.Services.GetRequiredService<PipelineConfigViewModel>();
                    DataContext = viewModel;
                    await viewModel.InitializeAsync();
                }
            }
        }

        // NEW: Test PACS Connection Click Handler (Session 95 - Quick Fix)
        private async void TestPacsConnection_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null) return;

            Debug.WriteLine("TestPacsConnection_Click called");

            try
            {
                button.IsEnabled = false;
                var originalContent = button.Content;
                button.Content = "Testing...";

                if (DataContext is PipelineConfigViewModel vm)
                {
                    Debug.WriteLine($"ViewModel found: {vm != null}");
                    Debug.WriteLine($"PacsConfigViewModel exists: {vm.PacsConfigViewModel != null}");
                    Debug.WriteLine($"SelectedPipeline: {vm.SelectedPipeline?.Name ?? "null"}");
                    Debug.WriteLine($"PacsConfiguration: {vm.SelectedPipeline?.PacsConfiguration != null}");

                    if (vm.PacsConfigViewModel != null &&
                        vm.SelectedPipeline?.PacsConfiguration != null)
                    {
                        // Initialize if needed
                        vm.PacsConfigViewModel.Initialize(vm.SelectedPipeline);

                        // Call test method directly
                        await vm.PacsConfigViewModel.TestPacsConnectionAsync();
                        Debug.WriteLine("Test completed");
                    }
                    else
                    {
                        MessageBox.Show(
                            "PACS configuration not available.\n\nPlease ensure:\n" +
                            "- A pipeline is selected\n" +
                            "- PACS upload is enabled\n" +
                            "- PACS settings are configured",
                            "Configuration Required",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);
                    }
                }
                else
                {
                    Debug.WriteLine("DataContext is not PipelineConfigViewModel!");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"TestPacsConnection_Click error: {ex}");
                MessageBox.Show(
                    $"Error testing connection:\n{ex.Message}",
                    "Test Failed",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            finally
            {
                // Restore button state
                if (button != null && DataContext is PipelineConfigViewModel viewModel)
                {
                    button.IsEnabled = viewModel.SelectedPipeline?.PacsConfiguration?.Enabled ?? false;
                    button.Content = "Test Connection (C-ECHO)";
                }
            }
        }

        // Browse button handlers
        private void BrowseWatchFolder_Click(object sender, RoutedEventArgs e)
        {
            BrowseFolder("Select Watch Folder", path =>
            {
                if (DataContext is PipelineConfigViewModel vm && vm.SelectedPipeline != null)
                {
                    vm.SelectedPipeline.WatchSettings.Path = path;
                }
            });
        }

        // NEW: Output Path handler (Session 107 Fix)
        private void BrowseOutputPath_Click(object sender, RoutedEventArgs e)
        {
            BrowseFolder("Select Output Path", path =>
            {
                if (DataContext is PipelineConfigViewModel vm && vm.SelectedPipeline != null)
                {
                    vm.SelectedPipeline.WatchSettings.OutputPath = path;
                }
            });
        }

        // RENAMED: Was BrowseOutputFolder_Click, now BrowseArchiveFolder_Click
        private void BrowseArchiveFolder_Click(object sender, RoutedEventArgs e)
        {
            BrowseFolder("Select Archive Folder", path =>
            {
                if (DataContext is PipelineConfigViewModel vm && vm.SelectedPipeline != null)
                {
                    vm.SelectedPipeline.ProcessingOptions.ArchiveFolder = path;
                }
            });
        }

        private void BrowseErrorFolder_Click(object sender, RoutedEventArgs e)
        {
            BrowseFolder("Select Error Folder", path =>
            {
                if (DataContext is PipelineConfigViewModel vm && vm.SelectedPipeline != null)
                {
                    vm.SelectedPipeline.ProcessingOptions.ErrorFolder = path;
                }
            });
        }

        private void BrowseBackupFolder_Click(object sender, RoutedEventArgs e)
        {
            BrowseFolder("Select Backup Folder for Original JPEGs", path =>
            {
                if (DataContext is PipelineConfigViewModel vm && vm.SelectedPipeline != null)
                {
                    vm.SelectedPipeline.ProcessingOptions.BackupFolder = path;
                }
            });
        }

        private void BrowseLogFolder_Click(object sender, RoutedEventArgs e)
        {
            BrowseFolder("Select Log Folder", path =>
            {
                if (DataContext is PipelineConfigViewModel vm)
                {
                    vm.PipelineLogFolder = path;
                }
            });
        }

        private void BrowseFolder(string description, Action<string> setPath)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog
            {
                Description = description,
                ShowNewFolderButton = true
            };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                setPath(dialog.SelectedPath);
            }
        }
    }
}
