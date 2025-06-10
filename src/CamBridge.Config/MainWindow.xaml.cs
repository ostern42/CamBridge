// src\CamBridge.Config\MainWindow.xaml.cs
// Version: 0.7.7
// Description: Main window code-behind with dynamic version display

using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using ModernWpf.Controls;
using CamBridge.Config.Services;
using CamBridge.Config.ViewModels;
using CamBridge.Config.Views;

namespace CamBridge.Config
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly INavigationService _navigationService;
        private readonly MainViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();

            // Set version dynamically from assembly
            var version = Assembly.GetExecutingAssembly()
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                ?.InformationalVersion ?? "Unknown";

            Title = $"CamBridge Configuration v{version}";

            // Get services from DI
            var app = (App)App.Current;
            _navigationService = app.Host!.Services.GetRequiredService<INavigationService>();
            _viewModel = app.Host!.Services.GetRequiredService<MainViewModel>();

            DataContext = _viewModel;

            // Initialize navigation
            if (_navigationService is NavigationService navService)
            {
                navService.SetFrame(ContentFrame);
            }

            // Navigate to dashboard on startup
            NavView.SelectedItem = NavView.MenuItems[0];
        }

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItem is NavigationViewItem item)
            {
                var tag = item.Tag?.ToString();
                if (!string.IsNullOrEmpty(tag))
                {
                    _navigationService.NavigateTo(tag);
                }
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // Clean shutdown
            Application.Current.Shutdown();
        }
    }
}
