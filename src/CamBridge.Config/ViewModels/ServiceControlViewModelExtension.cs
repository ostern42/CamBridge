// src\CamBridge.Config\ViewModels\ServiceControlViewModelExtension.cs
// Version: 0.5.26
// Extension methods for ServiceControlViewModel

using System;

namespace CamBridge.Config.ViewModels
{
    /// <summary>
    /// Extension methods to add missing functionality to ViewModels
    /// </summary>
    public static class ViewModelExtensions
    {
        /// <summary>
        /// Stops monitoring if the ViewModel supports it
        /// </summary>
        public static void StopMonitoring(this ServiceControlViewModel viewModel)
        {
            // Stop any timers or monitoring tasks
            try
            {
                // If ViewModel has a timer, stop it
                var timerField = viewModel.GetType().GetField("_statusTimer",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                if (timerField?.GetValue(viewModel) is System.Threading.Timer timer)
                {
                    timer?.Dispose();
                }

                // If ViewModel has a cancellation token, cancel it
                var cancellationField = viewModel.GetType().GetField("_cancellationTokenSource",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                if (cancellationField?.GetValue(viewModel) is System.Threading.CancellationTokenSource cts)
                {
                    cts?.Cancel();
                    cts?.Dispose();
                }

                System.Diagnostics.Debug.WriteLine("Service monitoring stopped");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error stopping monitoring: {ex.Message}");
            }
        }
    }
}
