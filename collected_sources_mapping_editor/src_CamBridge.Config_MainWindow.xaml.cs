// src/CamBridge.Config/MainWindow.xaml.cs
using System;
using System.Runtime.Versioning;
using System.Windows;
using System.Windows.Controls;
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

            // Navigate to Dashboard on startup
            if (ContentFrame != null)
            {
                ContentFrame.Navigate(new DashboardPage());
            }
        }

        [SupportedOSPlatform("windows7.0")]
        private void NavigationView_SelectionChanged(ModernWpf.Controls.NavigationView sender,
            ModernWpf.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItemContainer != null && ContentFrame != null)
            {
                var tag = args.SelectedItemContainer.Tag?.ToString();

                switch (tag)
                {
                    case "Dashboard":
                        ContentFrame.Navigate(new DashboardPage());
                        break;
                    case "ServiceControl":
                        ContentFrame.Navigate(new ServiceControlPage());
                        break;
                    case "DeadLetters":
                        ContentFrame.Navigate(new DeadLettersPage());
                        break;
                    case "Settings":
                        ContentFrame.Navigate(new SettingsPage());
                        break;
                    case "About":
                        ContentFrame.Navigate(new AboutPage());
                        break;
                }
            }
        }
    }
}
