// src\CamBridge.Config\ViewModels\MainViewModel.cs
// Version: Dynamic from Version.props
// Description: Main view model with dynamic version
// Copyright: © 2025 Claude's Improbably Reliable Software Solutions

using System.Reflection;

namespace CamBridge.Config.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public string Title
        {
            get
            {
                // Get version from assembly (populated from Version.props)
                var version = Assembly.GetExecutingAssembly()
                    .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                    ?.InformationalVersion
                    ?? Assembly.GetExecutingAssembly().GetName().Version?.ToString()
                    ?? "Unknown";

                return $"CamBridge Configuration v{version}";
            }
        }
    }
}
