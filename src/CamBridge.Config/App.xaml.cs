// src\CamBridge.Config\App.xaml.cs
// Version: 0.6.4
// Description: Updated DI registration - PipelineConfig instead of Settings

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

        // Properties for DI access
        public IHost Host => _host!;
        public IServiceProvider Services => _host!.Services;

        protected override void OnStartup(StartupEventArgs e)
        {
            _host = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    // Services
                    services.AddHttpClient<HttpApiService>();
                    services.AddSingleton<IApiService, HttpApiService>();
                    services.AddSingleton<INavigationService, NavigationService>();
                    services.AddSingleton<IServiceManager, ServiceManager>();
                    services.AddSingleton<IConfigurationService, ConfigurationService>();

                    // ViewModels - Updated for Pipeline Architecture!
                    services.AddTransient<MainViewModel>();
                    services.AddTransient<DashboardViewModel>();
                    services.AddTransient<ServiceControlViewModel>();
                    services.AddTransient<PipelineConfigViewModel>();  // NEW! Replaces SettingsViewModel
                    services.AddTransient<DeadLettersViewModel>();
                    services.AddTransient<MappingEditorViewModel>();
                    // SettingsViewModel REMOVED - Zero Global Settings!

                    // Main Window
                    services.AddSingleton<MainWindow>();

                    // Logging
                    services.AddLogging();
                })
                .ConfigureLogging(logging =>
                {
                    logging.AddDebug();
                    logging.SetMinimumLevel(LogLevel.Debug);
                })
                .Build();

            _host.Start();

            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _host?.StopAsync().Wait();
            _host?.Dispose();
            base.OnExit(e);
        }
    }
}
