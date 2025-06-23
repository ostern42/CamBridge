// src\CamBridge.Config\App.xaml.cs
// Version: 0.7.28
// Description: Application entry point with LogViewer registration
// Copyright: Â© 2025 Claude's Improbably Reliable Software Solutions

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;
using CamBridge.Config.Services;
using CamBridge.Config.ViewModels;
using CamBridge.Config.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CamBridge.Config
{
    /// <summary>
    /// Main application class
    /// </summary>
    public partial class App : Application
    {
        private IHost? _host;

        /// <summary>
        /// Gets the current host instance
        /// </summary>
        public IHost? Host => _host;

        /// <summary>
        /// Gets the current service provider
        /// </summary>
        public static IServiceProvider Services { get; private set; } = null!;

        /// <summary>
        /// Application startup
        /// </summary>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Setup global exception handlers
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            DispatcherUnhandledException += OnDispatcherUnhandledException;

            try
            {
                ConfigureHost();
            }
            catch (Exception ex)
            {
                LogException("Host configuration failed", ex);
                MessageBox.Show(
                    $"Failed to start application: {ex.Message}",
                    "Startup Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Shutdown(1);
            }
        }

        /// <summary>
        /// Configure the dependency injection host
        /// </summary>
        private void ConfigureHost()
        {
            // Verify config file exists - Added in v0.5.32
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var configPath = System.IO.Path.Combine(appDataPath, "CamBridge", "appsettings.json");

            if (!System.IO.File.Exists(configPath))
            {
                // Also check ProgramData (where Service saves config)
                var programDataPath = System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    "CamBridge",
                    "appsettings.json");

                if (System.IO.File.Exists(programDataPath))
                {
                    configPath = programDataPath;
                }
                else
                {
                    MessageBox.Show(
                        $"Configuration file not found.\nExpected at: {configPath}\n\nPlease run the service first to create initial configuration.",
                        "Configuration Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                }
            }

            _host = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    // Services
                    services.AddSingleton<INavigationService, NavigationService>();
                    services.AddSingleton<IServiceManager, ServiceManager>();
                    services.AddSingleton<IConfigurationService, ConfigurationService>();

                    // HttpClient for API calls - FIXED PORT!
                    services.AddHttpClient<IApiService, HttpApiService>(client =>
                    {
                        // CRITICAL: Use port 5111, not 5050!
                        client.BaseAddress = new Uri("http://localhost:5111/");
                        client.Timeout = TimeSpan.FromSeconds(5);
                    });

                    // ViewModels - Updated with LogViewerViewModel
                    services.AddTransient<MainViewModel>();
                    services.AddTransient<DashboardViewModel>();
                    services.AddTransient<ServiceControlViewModel>();
                    services.AddTransient<PipelineConfigViewModel>();
                    services.AddTransient<DeadLettersViewModel>();
                    services.AddTransient<MappingEditorViewModel>();
                    services.AddTransient<LogViewerViewModel>(); // NEW!

                    // Views - Registration for LogViewerPage
                    services.AddTransient<LogViewerPage>(); // NEW!

                    // Logging
                    services.AddLogging(configure =>
                    {
                        configure.AddDebug();
                        configure.SetMinimumLevel(LogLevel.Debug);
                    });
                })
                .Build();

            Services = _host.Services;

            // Create and show main window
            var mainWindow = new MainWindow();
            mainWindow.Show();
        }

        /// <summary>
        /// Application exit cleanup
        /// </summary>
        protected override void OnExit(ExitEventArgs e)
        {
            _host?.Dispose();
            base.OnExit(e);
        }

        /// <summary>
        /// Handle unhandled exceptions
        /// </summary>
        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            LogException("Unhandled exception", e.ExceptionObject as Exception);

            MessageBox.Show(
                "An unexpected error occurred. The application will now close.",
                "Fatal Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        /// <summary>
        /// Handle dispatcher unhandled exceptions
        /// </summary>
        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            LogException("Dispatcher exception", e.Exception);

            // Show error to user
            MessageBox.Show(
                $"An error occurred: {e.Exception.Message}",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            // Mark as handled to prevent crash
            e.Handled = true;
        }

        /// <summary>
        /// Logs an exception
        /// </summary>
        private void LogException(string context, Exception? exception)
        {
            if (exception == null) return;

            try
            {
                var logger = Services?.GetService<ILogger<App>>();
                logger?.LogError(exception, "{Context}", context);
            }
            catch
            {
                // Fallback to debug output if logging fails
                System.Diagnostics.Debug.WriteLine($"{context}: {exception}");
            }
        }
    }
}
