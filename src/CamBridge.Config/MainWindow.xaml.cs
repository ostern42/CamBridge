// src\CamBridge.Config\MainWindow.xaml.cs
// Version: 0.6.8
// Description: Main window with proper DI-based navigation
// Copyright: © 2025 Claude's Improbably Reliable Software Solutions

using System;
using System.Runtime.Versioning;
using System.Windows;
using CamBridge.Config.Services;
using CamBridge.Config.ViewModels;
using CamBridge.Config.Views;
using Microsoft.Extensions.DependencyInjection;
using ModernWpf.Controls;

namespace CamBridge.Config
{
    [SupportedOSPlatform("windows")]
    public partial class MainWindow : Window
    {
        private readonly IServiceProvider _serviceProvider;

        public MainWindow(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            InitializeComponent();

            // Set MainViewModel
            DataContext = _serviceProvider.GetRequiredService<MainViewModel>();

            // Navigate to Dashboard on startup
            if (ContentFrame != null)
            {
                NavigateToPage("Dashboard");
            }
        }

        [SupportedOSPlatform("windows7.0")]
        private void NavigationView_SelectionChanged(ModernWpf.Controls.NavigationView sender,
            ModernWpf.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItemContainer != null && ContentFrame != null)
            {
                var tag = args.SelectedItemContainer.Tag?.ToString();
                if (!string.IsNullOrEmpty(tag))
                {
                    NavigateToPage(tag);
                }
            }
        }

        private void NavigateToPage(string tag)
        {
            try
            {
                System.Windows.Controls.Page? page = null;

                // Create pages WITH their ViewModels from DI!
                switch (tag)
                {
                    case "Dashboard":
                        // Create page manually but inject ViewModel from DI
                        page = new DashboardPage();
                        var dashboardVm = _serviceProvider.GetRequiredService<DashboardViewModel>();
                        page.DataContext = dashboardVm;
                        System.Diagnostics.Debug.WriteLine($"Created Dashboard with ViewModel - Pipelines: {dashboardVm.PipelineStatuses.Count}");
                        break;

                    case "PipelineConfig":
                        page = new PipelineConfigPage();
                        page.DataContext = _serviceProvider.GetRequiredService<PipelineConfigViewModel>();
                        break;

                    case "DeadLetters":
                        page = new DeadLettersPage();
                        page.DataContext = _serviceProvider.GetRequiredService<DeadLettersViewModel>();
                        break;

                    case "MappingEditor":
                        page = new MappingEditorPage();
                        page.DataContext = _serviceProvider.GetRequiredService<MappingEditorViewModel>();
                        break;

                    case "ServiceControl":
                        page = new ServiceControlPage();
                        page.DataContext = _serviceProvider.GetRequiredService<ServiceControlViewModel>();
                        break;

                    case "About":
                        page = new AboutPage();
                        // AboutPage doesn't need a ViewModel
                        break;
                }

                if (page != null && ContentFrame != null)
                {
                    // Force complete refresh - clear navigation history
                    ContentFrame.NavigationService.RemoveBackEntry();
                    while (ContentFrame.NavigationService.CanGoBack)
                    {
                        ContentFrame.NavigationService.RemoveBackEntry();
                    }

                    ContentFrame.Navigate(page);
                    System.Diagnostics.Debug.WriteLine($"Navigated to {tag} - Page type: {page.GetType().Name}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Navigation error: {ex.Message}");
                MessageBox.Show($"Error navigating to {tag}: {ex.Message}", "Navigation Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
