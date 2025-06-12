// src\CamBridge.Config\App.xaml.cs
// Version: 0.7.10
// Description: WPF Application with dependency injection setup and ConfigurationPaths
// Copyright: © 2025 Claude's Improbably Reliable Software Solutions

using System;
using System.IO;
using System.Runtime.Versioning;
using System.Windows;
using CamBridge.Config.Services;
using CamBridge.Config.ViewModels;
using CamBridge.Core; // For ConfigurationPaths!
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CamBridge.Config
{
    [SupportedOSPlatform("windows")]
    public partial class App : Application
    {
        private IHost? _host;

        public IHost? Host => _host;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // CRITICAL FIX: Initialize configuration path like Service does!
            // This ensures Config UI and Service use the SAME config file
            try
            {
                if (ConfigurationPaths.InitializePrimaryConfig())
                {
                    // Config was created from defaults
                    MessageBox.Show(
                        "Configuration was initialized with default settings.\n" +
                        $"Location: {ConfigurationPaths.GetPrimaryConfigPath()}",
                        "CamBridge Configuration",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Failed to initialize configuration:\n{ex.Message}\n\n" +
                    "The application will continue but may not function correctly.",
                    "Configuration Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }

            _host = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    // Services
                    services.AddSingleton<INavigationService, NavigationService>();
                    services.AddSingleton<IServiceManager, ServiceManager>();
                    services.AddSingleton<IConfigurationService, ConfigurationService>();

                    // HttpClient for API calls
                    services.AddHttpClient<IApiService, HttpApiService>(client =>
                    {
                        // For now, use default port 5111
                        // The ConfigurationService will handle loading the actual config
                        client.BaseAddress = new Uri("http://localhost:5111/");
                        client.Timeout = TimeSpan.FromSeconds(5);
                    });

                    // ViewModels - Updated for Pipeline Architecture!
                    services.AddTransient<MainViewModel>();
                    services.AddTransient<DashboardViewModel>();
                    services.AddTransient<ServiceControlViewModel>();
                    services.AddTransient<PipelineConfigViewModel>();  // NEW! Replaces SettingsViewModel
                    services.AddTransient<DeadLettersViewModel>();
                    services.AddTransient<MappingEditorViewModel>();
                    // SettingsViewModel REMOVED - Zero Global Settings!

                    // MainWindow
                    services.AddSingleton<MainWindow>();
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddDebug();

                    // In Debug mode, also log to console
#if DEBUG
                    logging.AddConsole();
#endif

                    logging.SetMinimumLevel(LogLevel.Debug);
                })
                .Build();

            // Log configuration info
            var logger = _host.Services.GetRequiredService<ILogger<App>>();
            logger.LogInformation("CamBridge Config UI starting...");
            logger.LogInformation("Configuration path: {ConfigPath}", ConfigurationPaths.GetPrimaryConfigPath());
            logger.LogInformation("Configuration exists: {Exists}", ConfigurationPaths.PrimaryConfigExists());

            // Create and show main window
            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            var logger = _host?.Services.GetService<ILogger<App>>();
            logger?.LogInformation("CamBridge Config UI shutting down");

            _host?.Dispose();
            base.OnExit(e);
        }
    }
}
