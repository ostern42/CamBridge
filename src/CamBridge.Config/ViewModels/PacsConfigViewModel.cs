// src\CamBridge.Config\ViewModels\PacsConfigViewModel.cs
// Version: 0.8.5
// Description: PACS Configuration ViewModel - Extracted from PipelineConfigViewModel
// Session: 95 - The Great Refactoring!

using CamBridge.Core;
using CamBridge.Infrastructure.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using System.Windows;

namespace CamBridge.Config.ViewModels
{
    /// <summary>
    /// Handles PACS upload configuration for a pipeline
    /// Extracted from PipelineConfigViewModel in Session 95
    /// </summary>
    [SupportedOSPlatform("windows")]
    public partial class PacsConfigViewModel : ViewModelBase
    {
        private readonly DicomStoreService? _dicomStoreService;
        private PacsConfiguration? _pacsConfiguration;

        // Test result properties (EXACT names from original!)
        [ObservableProperty]
        private string _pacsTestResult = string.Empty;

        [ObservableProperty]
        private string _pacsTestResultColor = "Black";

        [ObservableProperty]
        private bool _isTestingConnection;

        /// <summary>
        /// The PACS configuration being edited
        /// </summary>
        public PacsConfiguration? PacsConfiguration
        {
            get => _pacsConfiguration;
            set
            {
                if (_pacsConfiguration != null)
                {
                    _pacsConfiguration.PropertyChanged -= OnPacsConfigurationChanged;
                }

                if (SetProperty(ref _pacsConfiguration, value))
                {
                    if (_pacsConfiguration != null)
                    {
                        _pacsConfiguration.PropertyChanged += OnPacsConfigurationChanged;
                    }

                    // Notify all dependent properties
                    OnPropertyChanged(nameof(IsEnabled));
                    OnPropertyChanged(nameof(CanTestConnection));
                    TestPacsConnectionCommand.NotifyCanExecuteChanged();
                }
            }
        }

        /// <summary>
        /// Convenience property for binding
        /// </summary>
        public bool IsEnabled => PacsConfiguration?.Enabled ?? false;

        /// <summary>
        /// Can we test the connection?
        /// </summary>
        public bool CanTestConnection =>
            PacsConfiguration != null &&
            PacsConfiguration.Enabled &&
            !IsTestingConnection &&
            !string.IsNullOrWhiteSpace(PacsConfiguration.Host) &&
            !string.IsNullOrWhiteSpace(PacsConfiguration.CalledAeTitle);

        public PacsConfigViewModel()
        {
            // Design-time constructor
            Debug.WriteLine("PacsConfigViewModel created (design-time)");
        }

        public PacsConfigViewModel(DicomStoreService? dicomStoreService)
        {
            _dicomStoreService = dicomStoreService;
            Debug.WriteLine("PacsConfigViewModel created with DicomStoreService");
        }

        /// <summary>
        /// Test PACS connection using C-ECHO
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanTestConnection))]
        public async Task TestPacsConnectionAsync()
        {
            if (PacsConfiguration == null)
            {
                PacsTestResult = "No PACS configuration available";
                PacsTestResultColor = "Red";
                return;
            }

            try
            {
                IsTestingConnection = true;
                PacsTestResult = "Testing connection...";
                PacsTestResultColor = "Gray";

                // If no service injected, try to get from App
                var dicomStoreService = _dicomStoreService;
                if (dicomStoreService == null)
                {
                    var app = Application.Current as App;
                    if (app?.Host == null)
                    {
                        PacsTestResult = "[ERROR] Service not available";
                        PacsTestResultColor = "Red";
                        return;
                    }

                    dicomStoreService = app.Host.Services.GetService<DicomStoreService>();
                    if (dicomStoreService == null)
                    {
                        PacsTestResult = "[ERROR] DICOM Store service not configured";
                        PacsTestResultColor = "Red";
                        return;
                    }
                }

                var result = await dicomStoreService.TestConnectionAsync(PacsConfiguration);

                PacsTestResult = result.Success
                    ? $"[OK] Connection successful! {result.TransactionUid}"
                    : $"[ERROR] Failed: {result.ErrorMessage}";
                PacsTestResultColor = result.Success ? "Green" : "Red";

                Debug.WriteLine($"PACS test result: Success={result.Success}, Message={result.ErrorMessage}");
            }
            catch (Exception ex)
            {
                PacsTestResult = $"[ERROR] Error: {ex.Message}";
                PacsTestResultColor = "Red";
                Debug.WriteLine($"PACS connection test failed: {ex}");
            }
            finally
            {
                IsTestingConnection = false;
                TestPacsConnectionCommand.NotifyCanExecuteChanged();
            }
        }

        /// <summary>
        /// Initialize with a pipeline configuration
        /// </summary>
        public void Initialize(PipelineConfiguration? pipeline)
        {
            if (pipeline == null)
            {
                PacsConfiguration = null;
                return;
            }

            // Ensure PacsConfiguration is never null
            if (pipeline.PacsConfiguration == null)
            {
                Debug.WriteLine("Creating default PacsConfiguration for pipeline");
                pipeline.PacsConfiguration = new PacsConfiguration();
            }

            PacsConfiguration = pipeline.PacsConfiguration;
        }

        /// <summary>
        /// Handle property changes from PacsConfiguration
        /// </summary>
        private void OnPacsConfigurationChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Debug.WriteLine($"PacsConfiguration property changed: {e.PropertyName}");

            // Update can execute state when relevant properties change
            if (e.PropertyName is nameof(PacsConfiguration.Enabled) or
                nameof(PacsConfiguration.Host) or
                nameof(PacsConfiguration.CalledAeTitle))
            {
                OnPropertyChanged(nameof(IsEnabled));
                OnPropertyChanged(nameof(CanTestConnection));
                TestPacsConnectionCommand.NotifyCanExecuteChanged();
            }

            // Clear test result when configuration changes
            if (!string.IsNullOrEmpty(PacsTestResult))
            {
                PacsTestResult = string.Empty;
                PacsTestResultColor = "Black";
            }

            // Bubble up the change
            RaiseConfigurationChanged();
        }

        /// <summary>
        /// Event raised when configuration changes
        /// </summary>
        public event EventHandler? ConfigurationChanged;

        protected virtual void RaiseConfigurationChanged()
        {
            ConfigurationChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Cleanup
        /// </summary>
        public void Cleanup()
        {
            if (_pacsConfiguration != null)
            {
                _pacsConfiguration.PropertyChanged -= OnPacsConfigurationChanged;
            }
        }
    }
}
