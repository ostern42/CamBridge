// src\CamBridge.Config\App.xaml.cs
// Version: 0.7.7
// Description: WPF Application with dependency injection setup

using System;
using System.Runtime.Versioning;
using System.Windows;
using CamBridge.Config.Services;
using CamBridge.Config.ViewModels;
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
                    logging.SetMinimumLevel(LogLevel.Debug);
                })
                .Build();

            // Create and show main window
            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _host?.Dispose();
            base.OnExit(e);
        }
    }
}
