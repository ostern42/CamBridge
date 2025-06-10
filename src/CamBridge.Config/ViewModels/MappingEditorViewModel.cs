// src/CamBridge.Config/ViewModels/MappingEditorViewModel.cs
// Version: 0.7.7
// Copyright: © 2025 Claude's Improbably Reliable Software Solutions
// Modified: 2025-06-08
// Status: Development/Local - Fixed Command Names

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
                StatusMessage = "Loading mapping sets...";

                // IMMER zuerst System Defaults laden - sie sind die Basis!
                LoadSystemDefaults();
                _logger.LogInformation("Loaded system default mapping sets");

                // Try to load v2 settings
                var settingsV2 = await _configurationService.LoadConfigurationAsync<CamBridgeSettingsV2>();

                bool hasUserSets = false;
                bool needsMigration = true;

                if (settingsV2 != null && settingsV2.MappingSets.Count > 0)
                {
                    // Add user sets from v2 settings
                    foreach (var set in settingsV2.MappingSets.Where(s => !s.IsSystemDefault))
                    {
                        MappingSets.Add(set);
                        hasUserSets = true;
                    }
                    needsMigration = false;
                    _logger.LogInformation($"Loaded {settingsV2.MappingSets.Count(s => !s.IsSystemDefault)} user mapping sets from settings");
                }

                // If no user sets exist, try migration from v1
                if (needsMigration && !hasUserSets)
                {
                    await MigrateFromV1Async();
                }

                // Debug: Log all loaded sets
                _logger.LogInformation($"Total mapping sets loaded: {MappingSets.Count}");
                foreach (var set in MappingSets)
                {
                    _logger.LogInformation($"  - {set.Name} (System: {set.IsSystemDefault}, Rules: {set.Rules?.Count ?? 0})");
                }

                // Select appropriate set - prefer Ricoh for initial experience
                SelectedMappingSet = MappingSets.FirstOrDefault(m => m.Name.Contains("Ricoh") && m.IsSystemDefault) ??
                                    MappingSets.FirstOrDefault(m => !m.IsSystemDefault) ??
                                    MappingSets.FirstOrDefault();

                IsModified = false;
                StatusMessage = $"Loaded {MappingSets.Count} mapping sets";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load mapping sets");
                StatusMessage = "Failed to load mapping sets";

                // Ensure we at least have system defaults
                if (!MappingSets.Any(m => m.IsSystemDefault))
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

        private async Task MigrateFromV1Async()
        {
            try
            {
                _logger.LogInformation("Attempting to migrate from v1 mappings...");

                // First try to load v1 settings
                var v1Settings = await _configurationService.LoadConfigurationAsync<CamBridgeSettings>();

                string mappingFile = "mappings.json";

                if (v1Settings != null && !string.IsNullOrEmpty(v1Settings.MappingConfigurationFile))
                {
                    mappingFile = v1Settings.MappingConfigurationFile;
                }
                else
                {
                    // No v1 settings - try common locations for mappings.json
                    var assemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
                    var assemblyDir = Path.GetDirectoryName(assemblyLocation) ?? "";

                    var possiblePaths = new[]
                    {
                        Path.Combine(assemblyDir, "mappings.json"),
                        Path.Combine(assemblyDir, "..", "..", "..", "..", "..", "CamBridge.Service", "mappings.json"),
                        Path.Combine(Environment.CurrentDirectory, "mappings.json"),
                        Path.Combine(Environment.CurrentDirectory, "src", "CamBridge.Service", "mappings.json"),
                        @"C:\Users\oliver.stern\source\repos\CamBridge\src\CamBridge.Service\mappings.json"
                    };

                    foreach (var path in possiblePaths)
                    {
                        var fullPath = Path.GetFullPath(path);
                        _logger.LogDebug($"Checking for mappings at: {fullPath}");
                        if (File.Exists(fullPath))
                        {
                            mappingFile = fullPath;
                            _logger.LogInformation($"Found mappings.json at: {fullPath}");
                            break;
                        }
                    }
                }

                if (_mappingConfiguration != null && File.Exists(mappingFile))
                {
                    // Try to load existing mapping rules
                    await _mappingConfiguration.LoadConfigurationAsync(mappingFile);
                    var rules = _mappingConfiguration.GetMappingRules().ToList();

                    if (rules.Count > 0)
                    {
                        // Fix transform names from old format
                        foreach (var rule in rules)
                        {
                            rule.Transform = rule.Transform switch
                            {
                                "GenderToDicom" => "MapGender",
                                "TruncateTo16" => "None", // Not available in current version
                                _ => rule.Transform
                            };
                        }

                        // Create migrated set from v1 rules
                        var migratedSet = new MappingSet
                        {
                            Id = Guid.NewGuid(),
                            Name = "[Migrated] Default Mapping Set",
                            Description = $"Migrated from {Path.GetFileName(mappingFile)}",
                            IsSystemDefault = false,
                            Rules = rules,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        };

                        MappingSets.Add(migratedSet);
                        _logger.LogInformation($"Migrated {rules.Count} rules from {mappingFile}");
                    }
                }
                else
                {
                    _logger.LogWarning($"Could not find mappings file at: {mappingFile}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not migrate v1 mappings");
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
                SourceField = "newField",
                DicomTag = "(0010,0010)",
                Description = $"New Rule {DateTime.Now:HHmmss}",
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
        }

        [RelayCommand(CanExecute = nameof(CanEditSelectedRule))]
        private void RemoveRule()
        {
            if (SelectedRule != null && SelectedMappingSet != null && !SelectedMappingSet.IsSystemDefault)
            {
                MappingRules.Remove(SelectedRule);

                // Update the set's rules
                SelectedMappingSet.Rules = MappingRules.Select(r => r.ToMappingRule()).ToList();
                SelectedMappingSet.UpdatedAt = DateTime.UtcNow;

                IsModified = true;
                SelectedRule = MappingRules.FirstOrDefault();
            }
        }

        private bool CanEditSelectedRule() => SelectedRule != null && CanEditCurrentSet;

        [RelayCommand(CanExecute = nameof(CanEditSelectedRule))]
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

        [RelayCommand(CanExecute = nameof(CanEditSelectedRule))]
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

                    // Load mappings from file
                    if (_mappingConfiguration != null)
                    {
                        await _mappingConfiguration.LoadConfigurationAsync(dialog.FileName);
                        var rules = _mappingConfiguration.GetMappingRules().ToList();

                        if (rules.Count > 0)
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
                        }
                        else
                        {
                            StatusMessage = "No mapping rules found in file";
                        }
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

                    if (_mappingConfiguration != null)
                    {
                        await _mappingConfiguration.SaveConfigurationAsync(SelectedMappingSet.Rules, dialog.FileName);
                        StatusMessage = $"Exported {SelectedMappingSet.Rules.Count} rules to {Path.GetFileName(dialog.FileName)}";
                    }
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
                            results.AppendLine($"✓ {rule.SourceField} → {rule.DicomTag}: '{transformedValue}'");
                            successCount++;
                        }
                        else
                        {
                            results.AppendLine($"⚠ {rule.SourceField}: No test data available");
                        }
                    }
                    catch (Exception ex)
                    {
                        results.AppendLine($"✗ {rule.SourceField}: Error - {ex.Message}");
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

        [RelayCommand(CanExecute = nameof(CanEditSelectedRule))]
        private void SelectDicomTag()
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
            RemoveRuleCommand.NotifyCanExecuteChanged();
            MoveRuleUpCommand.NotifyCanExecuteChanged();
            MoveRuleDownCommand.NotifyCanExecuteChanged();
            SelectDicomTagCommand.NotifyCanExecuteChanged();
            ExportMappingsCommand.NotifyCanExecuteChanged();
            ApplyRicohTemplateCommand.NotifyCanExecuteChanged();
            ApplyMinimalTemplateCommand.NotifyCanExecuteChanged();
            ApplyFullTemplateCommand.NotifyCanExecuteChanged();
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
