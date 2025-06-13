/**************************************************************************
*  App.xaml.cs                                                            *
*  PATH: src\CamBridge.Config\App.xaml.cs                                *
*  VERSION: 0.7.11 | SIZE: ~6KB | MODIFIED: 2025-06-13                   *
*                                                                         *
*  DESCRIPTION: WPF Application entry point with PORT FIX                 *
*  Copyright (c) 2025 Claude's Improbably Reliable Software Solutions     *
**************************************************************************/

using System;
using System.IO;
using System.Windows;
using CamBridge.Config.Services;
using CamBridge.Config.ViewModels;
using CamBridge.Config.Views;
using CamBridge.Core.Services;
using CamBridge.Core.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CamBridge.Config
{
    /// <summary>
    /// WPF Application class - Entry point for CamBridge Configuration UI
    /// </summary>
    public partial class App : Application
    {
        private IHost? _host;

        /// <summary>
        /// Application startup
        /// </summary>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Set up unhandled exception handling
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            DispatcherUnhandledException += OnDispatcherUnhandledException;

            // Initialize configuration paths
            try
            {
                ConfigurationPaths.EnsureDirectoriesExist();
                ConfigurationPaths.InitializePrimaryConfig();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Failed to initialize configuration:\n{ex.Message}\n\nThe application will continue but some features may not work correctly.",
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

                    // HttpClient for API calls - FIXED PORT!
                    services.AddHttpClient<IApiService, HttpApiService>(client =>
                    {
                        // CRITICAL: Use port 5111, not 5050!
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

                    // Views - No registration needed, created directly

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
        /// Gets the current service provider
        /// </summary>
        public static IServiceProvider Services { get; private set; } = null!;

        /// <summary>
        /// Application exit
        /// </summary>
        protected override void OnExit(ExitEventArgs e)
        {
            _host?.Dispose();
            base.OnExit(e);
        }

        /// <summary>
        /// Handles unhandled exceptions from background threads
        /// </summary>
        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;
            LogException("Unhandled exception", exception);

            if (e.IsTerminating)
            {
                MessageBox.Show(
                    "A critical error has occurred and the application must close.\n\n" +
                    $"Error: {exception?.Message}\n\n" +
                    "Please check the logs for more information.",
                    "Critical Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Handles unhandled exceptions from the UI thread
        /// </summary>
        private void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            LogException("UI exception", e.Exception);

            // Show error to user
            MessageBox.Show(
                $"An error occurred:\n\n{e.Exception.Message}\n\n" +
                "The application will try to continue, but you may experience issues.",
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
