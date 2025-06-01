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
            // Register pages
            _pages["Dashboard"] = typeof(DashboardPage);
            _pages["ServiceControl"] = typeof(ServiceControlPage);
            _pages["DeadLetters"] = typeof(DeadLetterPage);
            _pages["Settings"] = typeof(SettingsPage);
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
                _frame.Navigate(page);
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
