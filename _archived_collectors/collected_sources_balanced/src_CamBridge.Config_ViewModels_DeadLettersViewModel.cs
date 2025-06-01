// src/CamBridge.Config/ViewModels/DeadLettersViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CamBridge.Config.Models;
using CamBridge.Config.Services;

namespace CamBridge.Config.ViewModels
{
    /// <summary>
    /// ViewModel for Dead Letters management - simplified for initial compilation
    /// </summary>
    public partial class DeadLettersViewModel : ViewModelBase
    {
        private readonly IApiService _apiService;

        [ObservableProperty] private bool _isConnected;
        [ObservableProperty] private string _connectionStatus = "Connecting...";

        public DeadLettersViewModel(IApiService apiService)
        {
            _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
        }

        public void Cleanup()
        {
            // Cleanup resources
        }
    }
}
