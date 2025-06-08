// src\CamBridge.Config\Services\NavigationService.cs
// Version: 0.6.4
// Description: Navigation service with PipelineConfig instead of Settings

using System;
using System.Collections.Generic;
using System.Windows.Controls;
using CamBridge.Config.Views;

namespace CamBridge.Config.Services
{
    public class NavigationService : INavigationService
    {
        private Frame? _frame;
        private readonly Dictionary<string, Type> _pages = new();

        public NavigationService()
        {
            // Register pages - New order, no Settings!
            _pages["Dashboard"] = typeof(DashboardPage);
            _pages["PipelineConfig"] = typeof(PipelineConfigPage);      // NEW!
            _pages["DeadLetters"] = typeof(DeadLettersPage);
            _pages["MappingEditor"] = typeof(MappingEditorPage);
            _pages["ServiceControl"] = typeof(ServiceControlPage);
            _pages["About"] = typeof(AboutPage);
            // Settings REMOVED - Zero Global Settings!
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
                if (page != null)
                {
                    _frame.Navigate(page);
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
