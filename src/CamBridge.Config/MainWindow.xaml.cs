// src\CamBridge.Config\MainWindow.xaml.cs
// Version: 0.6.11
// Description: Main window with simplified navigation
// Copyright: © 2025 Claude's Improbably Reliable Software Solutions

using System;
using System.Diagnostics;
using System.Runtime.Versioning;
using System.Windows;
using CamBridge.Config.Services;
using CamBridge.Config.ViewModels;
using CamBridge.Config.Views;
using Microsoft.Extensions.DependencyInjection;

namespace CamBridge.Config
{
    [SupportedOSPlatform("windows")]
    public partial class MainWindow : Window
    {
        private readonly IServiceProvider _serviceProvider;
        private System.Windows.Controls.Page? _currentPage;

        public MainWindow(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            InitializeComponent();

            // Set MainViewModel
            DataContext = _serviceProvider.GetRequiredService<MainViewModel>();

            // Navigate to Dashboard on startup
            Loaded += (s, e) => NavigateToPage("Dashboard");
        }

        [SupportedOSPlatform("windows7.0")]
        private void NavigationView_SelectionChanged(ModernWpf.Controls.NavigationView sender,
            ModernWpf.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItemContainer?.Tag is string tag)
            {
                NavigateToPage(tag);
            }
        }

        private void NavigateToPage(string tag)
        {
            try
            {
                Debug.WriteLine($"\n=== NAVIGATION: {tag} ===");

                // Cleanup current page
                if (_currentPage is DashboardPage dashboardPage)
                {
                    (dashboardPage.DataContext as DashboardViewModel)?.Cleanup();
                }

                System.Windows.Controls.Page? page = tag switch
                {
                    "Dashboard" => CreatePageWithViewModel<DashboardPage, DashboardViewModel>(),
                    "PipelineConfig" => CreatePageWithViewModel<PipelineConfigPage, PipelineConfigViewModel>(),
                    "DeadLetters" => CreatePageWithViewModel<DeadLettersPage, DeadLettersViewModel>(),
                    "MappingEditor" => CreatePageWithViewModel<MappingEditorPage, MappingEditorViewModel>(),
                    "ServiceControl" => CreatePageWithViewModel<ServiceControlPage, ServiceControlViewModel>(),
                    "About" => new AboutPage(),
                    _ => null
                };

                if (page != null && ContentFrame != null)
                {
                    _currentPage = page;
                    ContentFrame.Navigate(page);
                    Debug.WriteLine($"✓ Navigated to {tag}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"✗ Navigation error: {ex.Message}");
                MessageBox.Show($"Error navigating to {tag}: {ex.Message}", "Navigation Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private TPage CreatePageWithViewModel<TPage, TViewModel>()
            where TPage : System.Windows.Controls.Page, new()
            where TViewModel : class
        {
            var page = new TPage();
            var viewModel = _serviceProvider.GetRequiredService<TViewModel>();
            page.DataContext = viewModel;

            Debug.WriteLine($"Created {typeof(TPage).Name} with {typeof(TViewModel).Name}");

            return page;
        }
    }
}
