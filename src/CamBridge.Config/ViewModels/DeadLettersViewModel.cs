// src/CamBridge.Config/ViewModels/DeadLettersViewModel.cs
// Version: 0.7.8
// Description: SIMPLE error folder viewer - KISS approach!

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace CamBridge.Config.ViewModels
{
    /// <summary>
    /// Simple ViewModel for error folder viewing - KISS!
    /// </summary>
    public partial class DeadLettersViewModel : ViewModelBase
    {
        [ObservableProperty] private string _errorFolder;
        [ObservableProperty] private bool _errorFolderExists;
        [ObservableProperty] private int _errorFileCount;

        public DeadLettersViewModel()
        {
            // Default error folder from ProcessingOptions
            _errorFolder = @"C:\CamBridge\Errors";
            CheckErrorFolder();
        }

        /// <summary>
        /// Open error folder in Windows Explorer
        /// </summary>
        [RelayCommand]
        private void OpenErrorFolder()
        {
            try
            {
                if (!Directory.Exists(ErrorFolder))
                {
                    Directory.CreateDirectory(ErrorFolder);
                }

                Process.Start("explorer.exe", ErrorFolder);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(
                    $"Could not open error folder: {ex.Message}",
                    "Error",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// Refresh error folder status
        /// </summary>
        [RelayCommand]
        private async Task RefreshAsync()
        {
            await Task.Run(() => CheckErrorFolder());
        }

        /// <summary>
        /// Check if error folder exists and count files
        /// </summary>
        private void CheckErrorFolder()
        {
            try
            {
                ErrorFolderExists = Directory.Exists(ErrorFolder);

                if (ErrorFolderExists)
                {
                    var errorFiles = Directory.GetFiles(ErrorFolder, "*.jpg", SearchOption.AllDirectories);
                    ErrorFileCount = errorFiles.Length;
                }
                else
                {
                    ErrorFileCount = 0;
                }
            }
            catch
            {
                ErrorFileCount = 0;
            }
        }

        public void Cleanup()
        {
            // Nothing to cleanup in simple implementation
        }
    }
}
