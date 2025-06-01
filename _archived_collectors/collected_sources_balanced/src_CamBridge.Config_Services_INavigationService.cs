using System;

namespace CamBridge.Config.Services
{
    /// <summary>
    /// Navigation service interface
    /// </summary>
    public interface INavigationService
    {
        bool CanGoBack { get; }
        void NavigateTo(string pageKey);
        void GoBack();
    }
}
