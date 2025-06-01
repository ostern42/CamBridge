// src/CamBridge.Config/App.xaml.cs
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

        // Property for DI access
        public IHost Host => _host!;

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

                    // ViewModels - WICHTIG: Alle müssen registriert sein!
                    services.AddTransient<MainViewModel>();
                    services.AddTransient<DashboardViewModel>();
                    services.AddTransient<ServiceControlViewModel>();
                    services.AddTransient<SettingsViewModel>();
                    services.AddTransient<DeadLettersViewModel>();

                    // Main Window
                    services.AddSingleton<MainWindow>();
                })
                .ConfigureLogging(logging =>
                {
                    logging.AddDebug();
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
