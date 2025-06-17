// src\CamBridge.Config\Services\NavigationService.cs
// Version: 0.7.21
// Description: Navigation service with PROPER ViewModel injection!

using System;
using System.Collections.Generic;
using System.Windows.Controls;
using CamBridge.Config.Views;
using CamBridge.Config.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace CamBridge.Config.Services
{
    public class NavigationService : INavigationService
    {
        private Frame? _frame;
        private readonly Dictionary<string, Type> _pages = new();
        private readonly IServiceProvider _serviceProvider;

        public NavigationService()
        {
            // Get service provider from App
            var app = (App)App.Current;
            _serviceProvider = app.Host!.Services;

            // Register pages - New order, no Settings!
            _pages["Dashboard"] = typeof(DashboardPage);
            _pages["PipelineConfig"] = typeof(PipelineConfigPage);
            _pages["DeadLetters"] = typeof(DeadLettersPage);
            _pages["MappingEditor"] = typeof(MappingEditorPage);
            _pages["ServiceControl"] = typeof(ServiceControlPage);
            _pages["About"] = typeof(AboutPage);
        }

        public bool CanGoBack => _frame?.CanGoBack ?? false;

        public void SetFrame(object frame)
        {
            _frame = frame as Frame;
        }

        public void NavigateTo(string pageKey)
        {
            if (_frame != null && _pages.TryGetValue(pageKey, out var pageType))
            {
                var page = Activator.CreateInstance(pageType);

                // CRITICAL: Inject ViewModel based on page type!
                if (page is Page pageInstance)
                {
                    object? viewModel = pageKey switch
                    {
                        "Dashboard" => _serviceProvider.GetService<DashboardViewModel>(),
                        "PipelineConfig" => _serviceProvider.GetService<PipelineConfigViewModel>(),
                        "DeadLetters" => _serviceProvider.GetService<DeadLettersViewModel>(),
                        "MappingEditor" => _serviceProvider.GetService<MappingEditorViewModel>(),
                        "ServiceControl" => _serviceProvider.GetService<ServiceControlViewModel>(),
                        _ => null
                    };

                    if (viewModel != null)
                    {
                        pageInstance.DataContext = viewModel;
                        System.Diagnostics.Debug.WriteLine($"NavigationService: Injected {viewModel.GetType().Name} into {pageType.Name}");
                    }

                    _frame.Navigate(pageInstance);
                }
            }
        }

        public void GoBack()
        {
            if (_frame?.CanGoBack == true)
            {
                _frame.GoBack();
            }
        }
    }
}
