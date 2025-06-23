// src\CamBridge.Config\Views\PipelineConfigPage.xaml.cs
// Version: 0.7.27
// Description: Pipeline Configuration page - Code Behind

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

        private void BrowseOutputFolder_Click(object sender, RoutedEventArgs e)
        {
            BrowseFolder("Select Output Folder", path =>
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

        private void BrowseDeadLetterFolder_Click(object sender, RoutedEventArgs e)
        {
            BrowseFolder("Select Dead Letter Folder", path =>
            {
                if (DataContext is PipelineConfigViewModel vm && vm.SelectedPipeline != null)
                {
                    vm.SelectedPipeline.ProcessingOptions.DeadLetterFolder = path;
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
