// src\CamBridge.Config\MainWindow.xaml.cs
// Version: 0.6.4
// Description: Main window with updated navigation order - No Settings!

using System;
using System.Runtime.Versioning;
using System.Windows;
using CamBridge.Config.Views;
using CamBridge.Config.ViewModels;
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

                switch (tag)
                {
                    case "Dashboard":
                        page = new DashboardPage();
                        break;

                    case "PipelineConfig":
                        page = new PipelineConfigPage();
                        break;

                    case "DeadLetters":
                        page = new DeadLettersPage();
                        break;

                    case "MappingEditor":
                        page = new MappingEditorPage();
                        break;

                    case "ServiceControl":
                        page = new ServiceControlPage();
                        break;

                    case "About":
                        page = new AboutPage();
                        break;

                        // Settings is GONE! No more Settings!
                }

                if (page != null && ContentFrame != null)
                {
                    ContentFrame.Navigate(page);
                    System.Diagnostics.Debug.WriteLine($"Navigated to {tag}");
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
