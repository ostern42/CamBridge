// src/CamBridge.Config/ViewModels/MappingEditorViewModel.cs
// Version: 0.7.26
// Copyright: Â© 2025 Claude's Improbably Reliable Software Solutions
// Modified: 2025-06-18
// Status: Development/Local - Transform Editor Enhanced

using CamBridge.Config.Dialogs;
using CamBridge.Config.Extensions;
using CamBridge.Config.Services;
using CamBridge.Core;
using CamBridge.Core.Interfaces;
using CamBridge.Core.ValueObjects;
using CamBridge.Infrastructure.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Win32;
using ModernWpf.Controls;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

// Alias to avoid ambiguity with FellowOakDicom.DicomTag
using DicomTag = CamBridge.Core.ValueObjects.DicomTag;

namespace CamBridge.Config.ViewModels
{
    /// <summary>
    /// ViewModel for the mapping editor with multiple mapping sets support
    /// </summary>
    public partial class MappingEditorViewModel : ViewModelBase
    {
        private readonly ILogger<MappingEditorViewModel> _logger;
        private readonly IConfigurationService _configurationService;
        private IMappingConfiguration? _mappingConfiguration;
        private bool _isInitialized = false;

        #region Properties

        // Mapping Sets
        [ObservableProperty] private ObservableCollection<MappingSet> _mappingSets = new();
        [ObservableProperty] private MappingSet? _selectedMappingSet;
        [ObservableProperty] private bool _canEditCurrentSet = true;

        // Mapping Rules
        [ObservableProperty] private ObservableCollection<MappingRuleViewModel> _mappingRules = new();
        [ObservableProperty] private MappingRuleViewModel? _selectedRule;

        // Preview
        [ObservableProperty] private string? _previewInput;
        [ObservableProperty] private string? _previewOutput;

        // State
        [ObservableProperty] private bool _isModified;
        [ObservableProperty] private string? _statusMessage;
        [ObservableProperty] private bool _isLoading;
        [ObservableProperty] private bool _isError;

        // Save feedback
        [ObservableProperty] private bool _showSaveSuccess;
        private System.Windows.Threading.DispatcherTimer? _saveSuccessTimer;

        // Available source fields
        public ObservableCollection<SourceFieldInfo> QRBridgeFields { get; } = new();
        public ObservableCollection<SourceFieldInfo> ExifFields { get; } = new();

        #endregion

        public MappingEditorViewModel(
            ILogger<MappingEditorViewModel> logger,
            IConfigurationService configurationService)
        {
            _logger = logger;
            _configurationService = configurationService;

            InitializeSourceFields();
            StatusMessage = "Ready";
        }

        /// <summary>
        /// Initialize the ViewModel - call this from the View's Loaded event
        /// </summary>
        public async Task InitializeAsync()
        {
            if (_isInitialized) return;

            try
            {
                IsLoading = true;
                StatusMessage = "Initializing mapping editor...";

                // Create MappingConfigurationLoader with proper logger
                var nullLoggerFactory = new NullLoggerFactory();
                var mappingLoaderLogger = nullLoggerFactory.CreateLogger<MappingConfigurationLoader>();
                _mappingConfiguration = new MappingConfigurationLoader(mappingLoaderLogger);

                // Load mapping sets
                await LoadMappingSetsAsync();

                _isInitialized = true;
                StatusMessage = "Mapping editor ready";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize mapping editor");
                StatusMessage = "Initialization failed";
                MessageBox.Show(
                    $"Failed to initialize mapping editor:\n{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        #region Initialization

        private void InitializeSourceFields()
        {
            // QRBridge fields (from QR code data)
            QRBridgeFields.Add(new SourceFieldInfo("examid", "Examination ID", "string"));
            QRBridgeFields.Add(new SourceFieldInfo("name", "Patient Name", "string"));
            QRBridgeFields.Add(new SourceFieldInfo("birthdate", "Birth Date", "date"));
            QRBridgeFields.Add(new SourceFieldInfo("gender", "Gender", "string"));
            QRBridgeFields.Add(new SourceFieldInfo("comment", "Comment", "string"));
            QRBridgeFields.Add(new SourceFieldInfo("patientid", "Patient ID", "string"));

            // EXIF fields (from image metadata)
            ExifFields.Add(new SourceFieldInfo("Make", "Camera Manufacturer", "string"));
            ExifFields.Add(new SourceFieldInfo("Model", "Camera Model", "string"));
            ExifFields.Add(new SourceFieldInfo("DateTimeOriginal", "Capture Date/Time", "datetime"));
            ExifFields.Add(new SourceFieldInfo("Software", "Software Version", "string"));
            ExifFields.Add(new SourceFieldInfo("ImageDescription", "Image Description", "string"));
        }

        private async Task LoadMappingSetsAsync()
        {
            try
            {
                IsLoading = true;
                IsError = false;
                StatusMessage = "Loading mapping sets...";

                // IMMER zuerst System Defaults laden - sie sind die Basis!
                LoadSystemDefaults();
                _logger.LogInformation("Loaded system default mapping sets");

                // Try to load v2 settings
                var settingsV2 = await _configurationService.LoadConfigurationAsync<CamBridgeSettingsV2>();

                if (settingsV2 != null && settingsV2.MappingSets.Count > 0)
                {
                    // Add user sets from v2 settings
                    foreach (var set in settingsV2.MappingSets.Where(s => !s.IsSystemDefault))
                    {
                        MappingSets.Add(set);
                    }
                    _logger.LogInformation($"Loaded {settingsV2.MappingSets.Count(s => !s.IsSystemDefault)} user mapping sets from settings");
                }

                // Debug: Log all loaded sets
                _logger.LogInformation($"Total mapping sets loaded: {MappingSets.Count}");
                foreach (var set in MappingSets)
                {
                    _logger.LogInformation($"  - {set.Name} (System: {set.IsSystemDefault}, Rules: {set.Rules?.Count ?? 0})");
                }

                // Select appropriate set - prefer Ricoh for initial experience
                var ricohSet = MappingSets.FirstOrDefault(s => s.Name.Contains("Ricoh", StringComparison.OrdinalIgnoreCase));
                SelectedMappingSet = ricohSet ?? MappingSets.FirstOrDefault();

                StatusMessage = $"Loaded {MappingSets.Count} mapping sets";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading mapping sets");
                StatusMessage = "Failed to load mapping sets";
                IsError = true;

                // Even on error, ensure system defaults are available
                if (MappingSets.Count == 0)
                {
                    LoadSystemDefaults();
                }
                SelectedMappingSet = MappingSets.FirstOrDefault();
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void LoadSystemDefaults()
        {
            // Ricoh G900 Standard Set
            var ricohSet = new MappingSet
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                Name = "[System] Ricoh G900 Standard",
                Description = "Standard mapping for Ricoh G900 II cameras",
                IsSystemDefault = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Rules = new List<MappingRule>
                {
                    // Patient identification
                    new() { SourceField = "name", DicomTag = "(0010,0010)", Description = "Patient Name",
                            SourceType = "QRBridge", Transform = "None", Required = true },
                    new() { SourceField = "patientid", DicomTag = "(0010,0020)", Description = "Patient ID",
                            SourceType = "QRBridge", Transform = "None", Required = true },
                    new() { SourceField = "birthdate", DicomTag = "(0010,0030)", Description = "Patient Birth Date",
                            SourceType = "QRBridge", Transform = "DateToDicom" },
                    new() { SourceField = "gender", DicomTag = "(0010,0040)", Description = "Patient Sex",
                            SourceType = "QRBridge", Transform = "MapGender" },
                    
                    // Study information
                    new() { SourceField = "examid", DicomTag = "(0020,0010)", Description = "Study ID",
                            SourceType = "QRBridge", Transform = "None" },
                    new() { SourceField = "comment", DicomTag = "(0008,1030)", Description = "Study Description",
                            SourceType = "QRBridge", Transform = "None" },
                    
                    // Image date/time from EXIF
                    new() { SourceField = "DateTimeOriginal", DicomTag = "(0008,0020)", Description = "Study Date",
                            SourceType = "EXIF", Transform = "ExtractDate" },
                    new() { SourceField = "DateTimeOriginal", DicomTag = "(0008,0030)", Description = "Study Time",
                            SourceType = "EXIF", Transform = "ExtractTime" },
                    
                    // Equipment info
                    new() { SourceField = "Make", DicomTag = "(0008,0070)", Description = "Manufacturer",
                            SourceType = "EXIF", Transform = "None" },
                    new() { SourceField = "Model", DicomTag = "(0008,1090)", Description = "Manufacturer Model Name",
                            SourceType = "EXIF", Transform = "None" }
                }
            };

            // Minimal Required Set
            var minimalSet = new MappingSet
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                Name = "[System] Minimal Required",
                Description = "Only the required DICOM fields",
                IsSystemDefault = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Rules = new List<MappingRule>
                {
                    new() { SourceField = "name", DicomTag = "(0010,0010)", Description = "Patient Name",
                            SourceType = "QRBridge", Transform = "None", Required = true },
                    new() { SourceField = "patientid", DicomTag = "(0010,0020)", Description = "Patient ID",
                            SourceType = "QRBridge", Transform = "None", Required = true },
                    new() { SourceField = "examid", DicomTag = "(0020,0010)", Description = "Study ID",
                            SourceType = "QRBridge", Transform = "None" }
                }
            };

            // Full Comprehensive Set
            var fullSet = new MappingSet
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000003"),
                Name = "[System] Full Comprehensive",
                Description = "Comprehensive mapping with all standard fields",
                IsSystemDefault = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Rules = new List<MappingRule>
                {
                    // Patient Module
                    new() { SourceField = "name", DicomTag = "(0010,0010)", Description = "Patient Name",
                            SourceType = "QRBridge", Transform = "None", Required = true },
                    new() { SourceField = "patientid", DicomTag = "(0010,0020)", Description = "Patient ID",
                            SourceType = "QRBridge", Transform = "None", Required = true },
                    new() { SourceField = "birthdate", DicomTag = "(0010,0030)", Description = "Patient Birth Date",
                            SourceType = "QRBridge", Transform = "DateToDicom" },
                    new() { SourceField = "gender", DicomTag = "(0010,0040)", Description = "Patient Sex",
                            SourceType = "QRBridge", Transform = "MapGender" },
                    new() { SourceField = "comment", DicomTag = "(0010,4000)", Description = "Patient Comments",
                            SourceType = "QRBridge", Transform = "None" },
                    
                    // Study Module
                    new() { SourceField = "examid", DicomTag = "(0020,0010)", Description = "Study ID",
                            SourceType = "QRBridge", Transform = "None" },
                    new() { SourceField = "examid", DicomTag = "(0008,0050)", Description = "Accession Number",
                            SourceType = "QRBridge", Transform = "None" },
                    new() { SourceField = "comment", DicomTag = "(0008,1030)", Description = "Study Description",
                            SourceType = "QRBridge", Transform = "None" },
                    new() { SourceField = "DateTimeOriginal", DicomTag = "(0008,0020)", Description = "Study Date",
                            SourceType = "EXIF", Transform = "ExtractDate" },
                    new() { SourceField = "DateTimeOriginal", DicomTag = "(0008,0030)", Description = "Study Time",
                            SourceType = "EXIF", Transform = "ExtractTime" },
                    
                    // Equipment Module
                    new() { SourceField = "Make", DicomTag = "(0008,0070)", Description = "Manufacturer",
                            SourceType = "EXIF", Transform = "None" },
                    new() { SourceField = "Model", DicomTag = "(0008,1090)", Description = "Manufacturer Model Name",
                            SourceType = "EXIF", Transform = "None" },
                    new() { SourceField = "Software", DicomTag = "(0018,1020)", Description = "Software Versions",
                            SourceType = "EXIF", Transform = "None" },
                    
                    // Image Module
                    new() { SourceField = "DateTimeOriginal", DicomTag = "(0008,002A)", Description = "Acquisition DateTime",
                            SourceType = "EXIF", Transform = "DateTimeToDicom" }
                }
            };

            MappingSets.Add(ricohSet);
            MappingSets.Add(minimalSet);
            MappingSets.Add(fullSet);
        }

        private MappingRuleViewModel CreateRuleViewModel(MappingRule rule)
        {
            var vm = new MappingRuleViewModel(rule);

            // Attach property changed handler
            vm.PropertyChanged += (s, e) =>
            {
                // Avoid recursive updates
                if (!IsLoading)
                {
                    IsModified = true;
                    if (s == SelectedRule && e.PropertyName != nameof(MappingRuleViewModel.DisplayName))
                    {
                        UpdatePreview();
                    }
                }
            };

            return vm;
        }

        #endregion

        #region Mapping Set Commands

        [RelayCommand]
        private async Task AddMappingSetAsync()
        {
            var newSet = new MappingSet
            {
                Name = $"New Mapping Set {DateTime.Now:HHmmss}",
                Description = "Custom mapping configuration",
                Rules = new List<MappingRule>(),
                IsSystemDefault = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            MappingSets.Add(newSet);
            SelectedMappingSet = newSet;
            IsModified = true;
            StatusMessage = $"Created new mapping set: {newSet.Name}";
        }

        [RelayCommand(CanExecute = nameof(CanDuplicateMappingSet))]
        private async Task DuplicateMappingSetAsync()
        {
            if (SelectedMappingSet == null) return;

            var duplicate = new MappingSet
            {
                Name = $"{SelectedMappingSet.Name} (Copy)",
                Description = SelectedMappingSet.Description,
                Rules = SelectedMappingSet.Rules.Select(r => new MappingRule
                {
                    SourceField = r.SourceField,
                    DicomTag = r.DicomTag,
                    Description = r.Description,
                    SourceType = r.SourceType,
                    Transform = r.Transform,
                    Required = r.Required,
                    DefaultValue = r.DefaultValue
                }).ToList(),
                IsSystemDefault = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            MappingSets.Add(duplicate);
            SelectedMappingSet = duplicate;
            IsModified = true;
            StatusMessage = $"Duplicated mapping set: {duplicate.Name}";
        }

        private bool CanDuplicateMappingSet() => SelectedMappingSet != null;

        [RelayCommand(CanExecute = nameof(CanDeleteMappingSet))]
        private async Task DeleteMappingSetAsync()
        {
            if (SelectedMappingSet == null || SelectedMappingSet.IsSystemDefault) return;

            var result = MessageBox.Show(
                $"Delete mapping set '{SelectedMappingSet.Name}'?\n\nThis action cannot be undone.",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var setName = SelectedMappingSet.Name;
                MappingSets.Remove(SelectedMappingSet);
                SelectedMappingSet = MappingSets.FirstOrDefault();
                IsModified = true;
                StatusMessage = $"Deleted mapping set: {setName}";
            }
        }

        private bool CanDeleteMappingSet() => SelectedMappingSet != null && !SelectedMappingSet.IsSystemDefault;

        #endregion

        #region Rule Commands

        [RelayCommand(CanExecute = nameof(CanEditCurrentSet))]
        private void AddRule()
        {
            if (SelectedMappingSet == null || SelectedMappingSet.IsSystemDefault) return;

            var newRule = new MappingRule
            {
                SourceField = "Select a field...",
                DicomTag = "(0010,0010)",
                Description = "New Mapping Rule",
                SourceType = "QRBridge",
                Transform = ValueTransform.None.ToString(),
                Required = false
            };

            var vm = CreateRuleViewModel(newRule);
            MappingRules.Add(vm);
            SelectedRule = vm;

            // Update the set's rules
            SelectedMappingSet.Rules = MappingRules.Select(r => r.ToMappingRule()).ToList();
            SelectedMappingSet.UpdatedAt = DateTime.UtcNow;

            IsModified = true;
            StatusMessage = "Added new mapping rule - drag a source field to configure";
        }

        [RelayCommand]
        private void RemoveRule(MappingRuleViewModel? rule)
        {
            if (rule != null && SelectedMappingSet != null && !SelectedMappingSet.IsSystemDefault)
            {
                MappingRules.Remove(rule);

                // Update the set's rules
                SelectedMappingSet.Rules = MappingRules.Select(r => r.ToMappingRule()).ToList();
                SelectedMappingSet.UpdatedAt = DateTime.UtcNow;

                IsModified = true;
                SelectedRule = MappingRules.FirstOrDefault();
                StatusMessage = "Removed mapping rule";
            }
        }

        [RelayCommand(CanExecute = nameof(CanMoveRuleUp))]
        private void MoveRuleUp()
        {
            if (SelectedRule == null || SelectedMappingSet?.IsSystemDefault == true) return;

            var index = MappingRules.IndexOf(SelectedRule);
            if (index > 0)
            {
                MappingRules.Move(index, index - 1);

                // Update the set's rules
                SelectedMappingSet.Rules = MappingRules.Select(r => r.ToMappingRule()).ToList();
                SelectedMappingSet.UpdatedAt = DateTime.UtcNow;

                IsModified = true;
            }
        }

        private bool CanMoveRuleUp() => SelectedRule != null && CanEditCurrentSet && MappingRules.IndexOf(SelectedRule) > 0;

        [RelayCommand(CanExecute = nameof(CanMoveRuleDown))]
        private void MoveRuleDown()
        {
            if (SelectedRule == null || SelectedMappingSet?.IsSystemDefault == true) return;

            var index = MappingRules.IndexOf(SelectedRule);
            if (index < MappingRules.Count - 1)
            {
                MappingRules.Move(index, index + 1);

                // Update the set's rules
                SelectedMappingSet.Rules = MappingRules.Select(r => r.ToMappingRule()).ToList();
                SelectedMappingSet.UpdatedAt = DateTime.UtcNow;

                IsModified = true;
            }
        }

        private bool CanMoveRuleDown() => SelectedRule != null && CanEditCurrentSet && MappingRules.IndexOf(SelectedRule) < MappingRules.Count - 1;

        #endregion

        #region Transform Edit Command

        [RelayCommand(CanExecute = nameof(CanEditTransform))]
        private async void EditTransform(MappingRuleViewModel? rule)
        {
            if (rule == null || SelectedMappingSet?.IsSystemDefault == true) return;

            var dialog = new TransformEditorDialog();

            dialog.SetMapping(rule.SourceField, rule.DicomTagString, rule.Transform);

            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                // Update the transform
                rule.Transform = dialog.SelectedTransform;

                // Update the set
                if (SelectedMappingSet != null)
                {
                    SelectedMappingSet.UpdatedAt = DateTime.UtcNow;
                    IsModified = true;
                }

                StatusMessage = $"Transform updated for {rule.SourceField}";
            }
        }

        private bool CanEditTransform(MappingRuleViewModel? rule) => rule != null && CanEditCurrentSet;

        #endregion

        #region Save/Load Commands

        [RelayCommand]
        private async Task SaveMappingsAsync()
        {
            try
            {
                IsLoading = true;
                StatusMessage = "Saving mapping sets...";

                // Update rules in selected set if it's not read-only
                if (SelectedMappingSet != null && !SelectedMappingSet.IsSystemDefault)
                {
                    SelectedMappingSet.Rules = MappingRules.Select(vm => vm.ToMappingRule()).ToList();
                    SelectedMappingSet.UpdatedAt = DateTime.UtcNow;
                }

                // Load current v2 settings or create new
                var settingsV2 = await _configurationService.LoadConfigurationAsync<CamBridgeSettingsV2>()
                                 ?? new CamBridgeSettingsV2();

                // Update mapping sets - nur User Sets speichern!
                settingsV2.MappingSets = MappingSets.Where(s => !s.IsSystemDefault).ToList();

                // Save
                await _configurationService.SaveConfigurationAsync(settingsV2);

                IsModified = false;
                StatusMessage = "Mapping sets saved successfully";

                // Show save success feedback
                ShowSaveSuccess = true;

                // Hide after 3 seconds
                if (_saveSuccessTimer == null)
                {
                    _saveSuccessTimer = new System.Windows.Threading.DispatcherTimer
                    {
                        Interval = TimeSpan.FromSeconds(3)
                    };
                    _saveSuccessTimer.Tick += (s, e) =>
                    {
                        ShowSaveSuccess = false;
                        _saveSuccessTimer.Stop();
                    };
                }
                _saveSuccessTimer.Start();

                _logger.LogInformation($"Saved {settingsV2.MappingSets.Count} user mapping sets");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save mapping sets");
                StatusMessage = "Failed to save mapping sets";
                MessageBox.Show($"Error saving mapping sets: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task ImportMappingsAsync()
        {
            try
            {
                var dialog = new OpenFileDialog
                {
                    Title = "Import Mapping Configuration",
                    Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                    DefaultExt = ".json"
                };

                if (dialog.ShowDialog() == true)
                {
                    IsLoading = true;
                    StatusMessage = "Importing mappings...";

                    // Read file content
                    var jsonContent = await File.ReadAllTextAsync(dialog.FileName);

                    // Try to deserialize as array of MappingRule
                    var rules = System.Text.Json.JsonSerializer.Deserialize<List<MappingRule>>(jsonContent);

                    if (rules != null && rules.Count > 0)
                    {
                        // Create new set from imported rules
                        var importedSet = new MappingSet
                        {
                            Name = $"Imported from {Path.GetFileName(dialog.FileName)}",
                            Description = $"Imported on {DateTime.Now:yyyy-MM-dd HH:mm}",
                            Rules = rules,
                            IsSystemDefault = false,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        };

                        MappingSets.Add(importedSet);
                        SelectedMappingSet = importedSet;
                        IsModified = true;
                        StatusMessage = $"Imported {rules.Count} mapping rules";

                        MessageBox.Show($"Successfully imported {rules.Count} mapping rules.", "Import Successful",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        StatusMessage = "No mapping rules found in file";
                        MessageBox.Show("No valid mapping rules found in the selected file.", "Import Failed",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to import mappings");
                MessageBox.Show($"Error importing mappings: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand(CanExecute = nameof(CanExportMappings))]
        private async Task ExportMappingsAsync()
        {
            if (SelectedMappingSet == null) return;

            try
            {
                var dialog = new SaveFileDialog
                {
                    Title = "Export Mapping Configuration",
                    Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                    DefaultExt = ".json",
                    FileName = $"{SelectedMappingSet.Name.Replace("[System]", "").Trim()}_{DateTime.Now:yyyyMMdd_HHmmss}.json"
                };

                if (dialog.ShowDialog() == true)
                {
                    IsLoading = true;
                    StatusMessage = "Exporting mappings...";

                    // Serialize rules to JSON
                    var jsonOptions = new System.Text.Json.JsonSerializerOptions
                    {
                        WriteIndented = true,
                        PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
                    };

                    var json = System.Text.Json.JsonSerializer.Serialize(SelectedMappingSet.Rules, jsonOptions);
                    await File.WriteAllTextAsync(dialog.FileName, json);

                    StatusMessage = $"Exported {SelectedMappingSet.Rules.Count} rules to {Path.GetFileName(dialog.FileName)}";

                    MessageBox.Show($"Successfully exported {SelectedMappingSet.Rules.Count} mapping rules.", "Export Successful",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to export mappings");
                MessageBox.Show($"Error exporting mappings: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private bool CanExportMappings() => SelectedMappingSet != null;

        [RelayCommand]
        private async Task TestMappingAsync()
        {
            if (SelectedMappingSet == null)
            {
                MessageBox.Show("Please select a mapping set to test", "No Mapping Set",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                IsLoading = true;
                StatusMessage = "Testing mapping configuration...";

                // Create test data
                var testData = new System.Collections.Generic.Dictionary<string, string>
                {
                    { "examid", "TEST001" },
                    { "name", "Test, Patient" },
                    { "birthdate", "1990-01-15" },
                    { "gender", "M" },
                    { "comment", "Test mapping" },
                    { "patientid", "PAT001" },
                    { "Make", "RICOH" },
                    { "Model", "G900 II" },
                    { "DateTimeOriginal", DateTime.Now.ToString("yyyy:MM:dd HH:mm:ss") },
                    { "Software", "CamBridge Test" }
                };

                var results = new System.Text.StringBuilder();
                results.AppendLine($"Testing Mapping Set: {SelectedMappingSet.Name}");
                results.AppendLine($"Rules: {SelectedMappingSet.Rules.Count}");
                results.AppendLine();

                int successCount = 0;
                int errorCount = 0;

                foreach (var rule in SelectedMappingSet.Rules)
                {
                    try
                    {
                        if (testData.TryGetValue(rule.SourceField, out var sourceValue))
                        {
                            var transformedValue = rule.ApplyTransform(sourceValue);
                            results.AppendLine($"âœ” {rule.SourceField} â†’ {rule.DicomTag}: '{transformedValue}'");
                            successCount++;
                        }
                        else
                        {
                            results.AppendLine($"âš  {rule.SourceField}: No test data available");
                        }
                    }
                    catch (Exception ex)
                    {
                        results.AppendLine($"âœ— {rule.SourceField}: Error - {ex.Message}");
                        errorCount++;
                    }
                }

                results.AppendLine();
                results.AppendLine($"Summary: {successCount} successful, {errorCount} errors");

                // Show results
                MessageBox.Show(results.ToString(), "Mapping Test Results",
                    MessageBoxButton.OK,
                    errorCount > 0 ? MessageBoxImage.Warning : MessageBoxImage.Information);

                StatusMessage = $"Test completed: {successCount} successful, {errorCount} errors";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to test mappings");
                MessageBox.Show($"Error testing mappings: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand(CanExecute = nameof(CanBrowseDicomTags))]
        private void BrowseDicomTags()
        {
            if (SelectedRule == null || SelectedMappingSet?.IsSystemDefault == true) return;

            var dialog = new DicomTagBrowserDialog
            {
                Owner = Application.Current.MainWindow
            };

            if (dialog.ShowDialog() == true && dialog.SelectedTag != null)
            {
                // Convert DicomTag object to string format
                SelectedRule.DicomTagString = dialog.SelectedTag.ToString();
                UpdatePreview();

                // Update the set
                if (SelectedMappingSet != null)
                {
                    SelectedMappingSet.UpdatedAt = DateTime.UtcNow;
                    IsModified = true;
                }
            }
        }

        private bool CanBrowseDicomTags() => SelectedRule != null && CanEditCurrentSet;

        #endregion

        #region Template Commands

        [RelayCommand(CanExecute = nameof(CanEditCurrentSet))]
        private void ApplyRicohTemplate()
        {
            if (SelectedMappingSet == null || SelectedMappingSet.IsSystemDefault) return;

            if (MessageBox.Show(
                "This will replace all current mappings with the Ricoh G900 II template. Continue?",
                "Apply Template",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question) != MessageBoxResult.Yes)
            {
                return;
            }

            // Find the Ricoh system template
            var ricohTemplate = MappingSets.FirstOrDefault(s => s.Name.Contains("Ricoh") && s.IsSystemDefault);
            if (ricohTemplate != null)
            {
                MappingRules.Clear();
                foreach (var rule in ricohTemplate.Rules)
                {
                    // Create copies of the rules
                    var newRule = new MappingRule
                    {
                        SourceField = rule.SourceField,
                        DicomTag = rule.DicomTag,
                        Description = rule.Description,
                        SourceType = rule.SourceType,
                        Transform = rule.Transform,
                        Required = rule.Required,
                        DefaultValue = rule.DefaultValue
                    };
                    var vm = CreateRuleViewModel(newRule);
                    MappingRules.Add(vm);
                }

                // Update the set
                SelectedMappingSet.Rules = MappingRules.Select(r => r.ToMappingRule()).ToList();
                SelectedMappingSet.UpdatedAt = DateTime.UtcNow;

                IsModified = true;
                StatusMessage = "Ricoh template applied";
            }
        }

        [RelayCommand(CanExecute = nameof(CanEditCurrentSet))]
        private void ApplyMinimalTemplate()
        {
            if (SelectedMappingSet == null || SelectedMappingSet.IsSystemDefault) return;

            if (MessageBox.Show(
                "This will replace all current mappings with a minimal template. Continue?",
                "Apply Template",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question) != MessageBoxResult.Yes)
            {
                return;
            }

            // Find the minimal system template
            var minimalTemplate = MappingSets.FirstOrDefault(s => s.Name.Contains("Minimal") && s.IsSystemDefault);
            if (minimalTemplate != null)
            {
                MappingRules.Clear();
                foreach (var rule in minimalTemplate.Rules)
                {
                    // Create copies of the rules
                    var newRule = new MappingRule
                    {
                        SourceField = rule.SourceField,
                        DicomTag = rule.DicomTag,
                        Description = rule.Description,
                        SourceType = rule.SourceType,
                        Transform = rule.Transform,
                        Required = rule.Required,
                        DefaultValue = rule.DefaultValue
                    };
                    var vm = CreateRuleViewModel(newRule);
                    MappingRules.Add(vm);
                }

                // Update the set
                SelectedMappingSet.Rules = MappingRules.Select(r => r.ToMappingRule()).ToList();
                SelectedMappingSet.UpdatedAt = DateTime.UtcNow;

                IsModified = true;
                StatusMessage = "Minimal template applied";
            }
        }

        [RelayCommand(CanExecute = nameof(CanEditCurrentSet))]
        private void ApplyFullTemplate()
        {
            if (SelectedMappingSet == null || SelectedMappingSet.IsSystemDefault) return;

            if (MessageBox.Show(
                "This will replace all current mappings with a comprehensive template. Continue?",
                "Apply Template",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question) != MessageBoxResult.Yes)
            {
                return;
            }

            // Find the full system template
            var fullTemplate = MappingSets.FirstOrDefault(s => s.Name.Contains("Full") && s.IsSystemDefault);
            if (fullTemplate != null)
            {
                MappingRules.Clear();
                foreach (var rule in fullTemplate.Rules)
                {
                    // Create copies of the rules
                    var newRule = new MappingRule
                    {
                        SourceField = rule.SourceField,
                        DicomTag = rule.DicomTag,
                        Description = rule.Description,
                        SourceType = rule.SourceType,
                        Transform = rule.Transform,
                        Required = rule.Required,
                        DefaultValue = rule.DefaultValue
                    };
                    var vm = CreateRuleViewModel(newRule);
                    MappingRules.Add(vm);
                }

                // Update the set
                SelectedMappingSet.Rules = MappingRules.Select(r => r.ToMappingRule()).ToList();
                SelectedMappingSet.UpdatedAt = DateTime.UtcNow;

                IsModified = true;
                StatusMessage = "Full template applied";
            }
        }

        #endregion

        #region Drag & Drop Support

        /// <summary>
        /// Add a new rule from a dropped field
        /// </summary>
        public void AddRuleFromField(SourceFieldInfo fieldInfo)
        {
            if (SelectedMappingSet == null || SelectedMappingSet.IsSystemDefault) return;

            // Determine appropriate DICOM tag based on field
            string dicomTag = fieldInfo.FieldName switch
            {
                "name" => "(0010,0010)",          // Patient Name
                "patientid" => "(0010,0020)",     // Patient ID
                "birthdate" => "(0010,0030)",     // Patient Birth Date
                "gender" => "(0010,0040)",        // Patient Sex
                "examid" => "(0020,0010)",        // Study ID
                "comment" => "(0008,1030)",       // Study Description
                "Make" => "(0008,0070)",          // Manufacturer
                "Model" => "(0008,1090)",         // Model Name
                "DateTimeOriginal" => "(0008,0020)", // Study Date
                "Software" => "(0018,1020)",      // Software Version
                _ => "(0010,0010)"                // Default to Patient Name
            };

            // Determine transform based on field type
            ValueTransform transform = fieldInfo.FieldName switch
            {
                "birthdate" => ValueTransform.DateToDicom,
                "gender" => ValueTransform.MapGender,
                "DateTimeOriginal" when dicomTag == "(0008,0020)" => ValueTransform.ExtractDate,
                "DateTimeOriginal" when dicomTag == "(0008,0030)" => ValueTransform.ExtractTime,
                _ => ValueTransform.None
            };

            var newRule = new MappingRule
            {
                SourceField = fieldInfo.FieldName,
                DicomTag = dicomTag,
                Description = fieldInfo.DisplayName,
                SourceType = QRBridgeFields.Contains(fieldInfo) ? "QRBridge" : "EXIF",
                Transform = transform.ToString(),
                Required = fieldInfo.FieldName == "name" || fieldInfo.FieldName == "patientid"
            };

            var vm = CreateRuleViewModel(newRule);
            MappingRules.Add(vm);
            SelectedRule = vm;

            // Update the set's rules
            SelectedMappingSet.Rules = MappingRules.Select(r => r.ToMappingRule()).ToList();
            SelectedMappingSet.UpdatedAt = DateTime.UtcNow;

            IsModified = true;
            StatusMessage = $"Added mapping for {fieldInfo.DisplayName}";
        }

        #endregion

        #region Preview

        public void UpdatePreview()
        {
            if (SelectedRule == null || string.IsNullOrWhiteSpace(PreviewInput))
            {
                PreviewOutput = string.Empty;
                return;
            }

            try
            {
                var rule = SelectedRule.ToMappingRule();
                PreviewOutput = rule.ApplyTransform(PreviewInput);
            }
            catch (Exception ex)
            {
                PreviewOutput = $"Error: {ex.Message}";
            }
        }

        partial void OnPreviewInputChanged(string? value)
        {
            UpdatePreview();
        }

        partial void OnSelectedRuleChanged(MappingRuleViewModel? value)
        {
            UpdatePreview();

            // Update command states
            BrowseDicomTagsCommand.NotifyCanExecuteChanged();
            MoveRuleUpCommand.NotifyCanExecuteChanged();
            MoveRuleDownCommand.NotifyCanExecuteChanged();
            EditTransformCommand.NotifyCanExecuteChanged();
        }

        partial void OnSelectedMappingSetChanged(MappingSet? value)
        {
            if (value == null) return;

            CanEditCurrentSet = !value.IsSystemDefault;
            LoadRulesFromSet(value);

            // Update command states
            DuplicateMappingSetCommand.NotifyCanExecuteChanged();
            DeleteMappingSetCommand.NotifyCanExecuteChanged();
            AddRuleCommand.NotifyCanExecuteChanged();
            MoveRuleUpCommand.NotifyCanExecuteChanged();
            MoveRuleDownCommand.NotifyCanExecuteChanged();
            BrowseDicomTagsCommand.NotifyCanExecuteChanged();
            ExportMappingsCommand.NotifyCanExecuteChanged();
            ApplyRicohTemplateCommand.NotifyCanExecuteChanged();
            ApplyMinimalTemplateCommand.NotifyCanExecuteChanged();
            ApplyFullTemplateCommand.NotifyCanExecuteChanged();
            EditTransformCommand.NotifyCanExecuteChanged();
        }

        private void LoadRulesFromSet(MappingSet set)
        {
            IsLoading = true;
            try
            {
                MappingRules.Clear();

                if (set.Rules != null)
                {
                    foreach (var rule in set.Rules)
                    {
                        var vm = CreateRuleViewModel(rule);
                        MappingRules.Add(vm);
                    }

                    StatusMessage = $"Loaded {set.Rules.Count} rules from '{set.Name}'";
                }
                else
                {
                    StatusMessage = $"Selected '{set.Name}' (no rules defined)";
                }
            }
            finally
            {
                IsLoading = false;
            }
        }

        #endregion

        #region Inner Classes

        public class SourceFieldInfo
        {
            public string FieldName { get; }
            public string DisplayName { get; }
            public string DataType { get; }

            public SourceFieldInfo(string fieldName, string displayName, string dataType)
            {
                FieldName = fieldName;
                DisplayName = displayName;
                DataType = dataType;
            }
        }

        #endregion
    }

    /// <summary>
    /// ViewModel wrapper for MappingRule
    /// </summary>
    public partial class MappingRuleViewModel : ObservableObject
    {
        private readonly MappingRule _rule;

        // UI-friendly properties that map to Core properties
        [ObservableProperty] private string _displayName;
        [ObservableProperty] private string _sourceType;
        [ObservableProperty] private string _sourceField;
        [ObservableProperty] private string _dicomTagString;
        [ObservableProperty] private ValueTransform _transform;
        [ObservableProperty] private bool _isRequired;
        [ObservableProperty] private string? _defaultValue;

        public MappingRuleViewModel(MappingRule rule)
        {
            _rule = rule;

            // Map Core properties to UI properties
            _displayName = rule.Description ?? $"{rule.SourceType}.{rule.SourceField}";
            _sourceType = rule.SourceType ?? "QRBridge";
            _sourceField = rule.SourceField;
            _dicomTagString = rule.DicomTag;
            _transform = Enum.TryParse<ValueTransform>(rule.Transform, out var t) ? t : ValueTransform.None;
            _isRequired = rule.Required;
            _defaultValue = rule.DefaultValue;
        }

        public MappingRule ToMappingRule()
        {
            // Update Core rule with UI values
            _rule.Description = DisplayName;
            _rule.SourceType = SourceType;
            _rule.SourceField = SourceField;
            _rule.DicomTag = DicomTagString;
            _rule.Transform = Transform.ToString();
            _rule.Required = IsRequired;
            _rule.DefaultValue = DefaultValue;

            return _rule;
        }

        partial void OnDicomTagStringChanged(string value)
        {
            // Validate DICOM tag format
            try
            {
                var tag = DicomTag.Parse(value);
                // Valid tag
            }
            catch
            {
                // Invalid tag format - could show error in UI
            }
        }
    }
}
