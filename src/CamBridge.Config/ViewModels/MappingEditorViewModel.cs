// File: src/CamBridge.Config/ViewModels/MappingEditorViewModel.cs
// Version: 0.5.24
// Copyright: © 2025 Claude's Improbably Reliable Software Solutions
// Modified: 2025-06-04
// Status: Development/Local

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
    /// ViewModel for the mapping editor with drag & drop support and templates
    /// </summary>
    public partial class MappingEditorViewModel : ViewModelBase
    {
        private readonly ILogger<MappingEditorViewModel> _logger;
        private readonly IConfigurationService _configurationService;
        private readonly IMappingConfiguration _mappingConfiguration;

        #region Properties

        [ObservableProperty] private ObservableCollection<MappingRuleViewModel> _mappingRules = new();
        [ObservableProperty] private MappingRuleViewModel? _selectedRule;
        [ObservableProperty] private string? _previewInput;
        [ObservableProperty] private string? _previewOutput;
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

            // Create MappingConfigurationLoader with proper logger
            var nullLoggerFactory = new NullLoggerFactory();
            var mappingLoaderLogger = nullLoggerFactory.CreateLogger<MappingConfigurationLoader>();
            _mappingConfiguration = new MappingConfigurationLoader(mappingLoaderLogger);

            InitializeSourceFields();
            _ = LoadMappingsAsync();
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

        private async Task LoadMappingsAsync()
        {
            try
            {
                IsLoading = true;
                StatusMessage = "Loading mappings...";

                var settings = await _configurationService.LoadConfigurationAsync<CamBridgeSettings>();
                if (settings != null)
                {
                    await _mappingConfiguration.LoadConfigurationAsync(settings.MappingConfigurationFile);

                    MappingRules.Clear();
                    foreach (var rule in _mappingConfiguration.GetMappingRules())
                    {
                        var vm = new MappingRuleViewModel(rule);
                        vm.PropertyChanged += (s, e) =>
                        {
                            IsModified = true;
                            if (s == SelectedRule)
                                UpdatePreview();
                        };
                        MappingRules.Add(vm);
                    }

                    IsModified = false;
                    StatusMessage = $"Loaded {MappingRules.Count} mapping rules";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load mappings");
                StatusMessage = "Failed to load mappings";
            }
            finally
            {
                IsLoading = false;
            }
        }

        #endregion

        #region Commands

        [RelayCommand]
        private void AddRule()
        {
            var newRule = new MappingRule
            {
                SourceField = "newField",
                DicomTag = "(0010,0010)",
                Description = $"New Rule {DateTime.Now:HHmmss}",
                SourceType = "QRBridge",
                Transform = ValueTransform.None.ToString(),
                Required = false
            };

            var vm = new MappingRuleViewModel(newRule);
            vm.PropertyChanged += (s, e) =>
            {
                IsModified = true;
                if (s == SelectedRule)
                    UpdatePreview();
            };

            MappingRules.Add(vm);
            SelectedRule = vm;
            IsModified = true;
        }

        [RelayCommand]
        private void RemoveRule()
        {
            if (SelectedRule != null)
            {
                MappingRules.Remove(SelectedRule);
                IsModified = true;
                SelectedRule = MappingRules.FirstOrDefault();
            }
        }

        [RelayCommand]
        private void MoveRuleUp()
        {
            if (SelectedRule == null) return;

            var index = MappingRules.IndexOf(SelectedRule);
            if (index > 0)
            {
                MappingRules.Move(index, index - 1);
                IsModified = true;
            }
        }

        [RelayCommand]
        private void MoveRuleDown()
        {
            if (SelectedRule == null) return;

            var index = MappingRules.IndexOf(SelectedRule);
            if (index < MappingRules.Count - 1)
            {
                MappingRules.Move(index, index + 1);
                IsModified = true;
            }
        }

        [RelayCommand]
        private async Task SaveMappingsAsync()
        {
            try
            {
                IsLoading = true;
                StatusMessage = "Saving mappings...";

                var settings = await _configurationService.LoadConfigurationAsync<CamBridgeSettings>();
                if (settings != null)
                {
                    var rules = MappingRules.Select(vm => vm.ToMappingRule());
                    await _mappingConfiguration.SaveConfigurationAsync(rules, settings.MappingConfigurationFile);

                    IsModified = false;
                    StatusMessage = "Mappings saved successfully";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save mappings");
                StatusMessage = "Failed to save mappings";
                MessageBox.Show($"Error saving mappings: {ex.Message}", "Error",
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

                    await _mappingConfiguration.LoadConfigurationAsync(dialog.FileName);

                    MappingRules.Clear();
                    foreach (var rule in _mappingConfiguration.GetMappingRules())
                    {
                        var vm = new MappingRuleViewModel(rule);
                        vm.PropertyChanged += (s, e) =>
                        {
                            IsModified = true;
                            if (s == SelectedRule)
                                UpdatePreview();
                        };
                        MappingRules.Add(vm);
                    }

                    IsModified = true;
                    StatusMessage = $"Imported {MappingRules.Count} mapping rules";
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

        [RelayCommand]
        private async Task ExportMappingsAsync()
        {
            try
            {
                var dialog = new SaveFileDialog
                {
                    Title = "Export Mapping Configuration",
                    Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                    DefaultExt = ".json",
                    FileName = $"mappings_export_{DateTime.Now:yyyyMMdd_HHmmss}.json"
                };

                if (dialog.ShowDialog() == true)
                {
                    IsLoading = true;
                    StatusMessage = "Exporting mappings...";

                    var rules = MappingRules.Select(vm => vm.ToMappingRule());
                    await _mappingConfiguration.SaveConfigurationAsync(rules, dialog.FileName);

                    StatusMessage = "Mappings exported successfully";
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

        [RelayCommand]
        private void SelectDicomTag()
        {
            if (SelectedRule == null) return;

            var dialog = new DicomTagBrowserDialog
            {
                Owner = Application.Current.MainWindow
            };

            if (dialog.ShowDialog() == true && dialog.SelectedTag != null)
            {
                // Convert DicomTag object to string format
                SelectedRule.DicomTagString = dialog.SelectedTag.ToString();
                UpdatePreview();
            }
        }

        #endregion

        #region Template Commands

        [RelayCommand]
        private void ApplyRicohTemplate()
        {
            if (MessageBox.Show(
                "This will replace all current mappings with the Ricoh G900 II template. Continue?",
                "Apply Template",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question) != MessageBoxResult.Yes)
            {
                return;
            }

            MappingRules.Clear();

            // Patient identification
            AddTemplateRule("name", "Patient Name", "(0010,0010)", ValueTransform.None, "QRBridge", true);
            AddTemplateRule("patientid", "Patient ID", "(0010,0020)", ValueTransform.None, "QRBridge", true);
            AddTemplateRule("birthdate", "Patient Birth Date", "(0010,0030)", ValueTransform.DateToDicom, "QRBridge");
            AddTemplateRule("gender", "Patient Sex", "(0010,0040)", ValueTransform.MapGender, "QRBridge");

            // Study information
            AddTemplateRule("examid", "Study ID", "(0020,0010)", ValueTransform.None, "QRBridge");
            AddTemplateRule("comment", "Study Description", "(0008,1030)", ValueTransform.None, "QRBridge");

            // Equipment from EXIF
            AddTemplateRule("Make", "Manufacturer", "(0008,0070)", ValueTransform.None, "EXIF");
            AddTemplateRule("Model", "Manufacturer Model Name", "(0008,1090)", ValueTransform.None, "EXIF");

            IsModified = true;
            StatusMessage = "Ricoh template applied";
        }

        [RelayCommand]
        private void ApplyMinimalTemplate()
        {
            if (MessageBox.Show(
                "This will replace all current mappings with a minimal template. Continue?",
                "Apply Template",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question) != MessageBoxResult.Yes)
            {
                return;
            }

            MappingRules.Clear();

            // Only required DICOM fields
            AddTemplateRule("name", "Patient Name", "(0010,0010)", ValueTransform.None, "QRBridge", true);
            AddTemplateRule("examid", "Patient ID", "(0010,0020)", ValueTransform.None, "QRBridge", true);
            AddTemplateRule("examid", "Study ID", "(0020,0010)", ValueTransform.None, "QRBridge");

            IsModified = true;
            StatusMessage = "Minimal template applied";
        }

        [RelayCommand]
        private void ApplyFullTemplate()
        {
            if (MessageBox.Show(
                "This will replace all current mappings with a comprehensive template. Continue?",
                "Apply Template",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question) != MessageBoxResult.Yes)
            {
                return;
            }

            MappingRules.Clear();

            // Patient Module
            AddTemplateRule("name", "Patient Name", "(0010,0010)", ValueTransform.None, "QRBridge", true);
            AddTemplateRule("patientid", "Patient ID", "(0010,0020)", ValueTransform.None, "QRBridge", true);
            AddTemplateRule("birthdate", "Patient Birth Date", "(0010,0030)", ValueTransform.DateToDicom, "QRBridge");
            AddTemplateRule("gender", "Patient Sex", "(0010,0040)", ValueTransform.MapGender, "QRBridge");
            AddTemplateRule("comment", "Patient Comments", "(0010,4000)", ValueTransform.None, "QRBridge");

            // Study Module
            AddTemplateRule("examid", "Study ID", "(0020,0010)", ValueTransform.None, "QRBridge");
            AddTemplateRule("examid", "Accession Number", "(0008,0050)", ValueTransform.None, "QRBridge");
            AddTemplateRule("comment", "Study Description", "(0008,1030)", ValueTransform.None, "QRBridge");
            AddTemplateRule("DateTimeOriginal", "Study Date", "(0008,0020)", ValueTransform.ExtractDate, "EXIF");
            AddTemplateRule("DateTimeOriginal", "Study Time", "(0008,0030)", ValueTransform.ExtractTime, "EXIF");

            // Equipment Module
            AddTemplateRule("Make", "Manufacturer", "(0008,0070)", ValueTransform.None, "EXIF");
            AddTemplateRule("Model", "Manufacturer Model Name", "(0008,1090)", ValueTransform.None, "EXIF");
            AddTemplateRule("Software", "Software Versions", "(0018,1020)", ValueTransform.None, "EXIF");

            // Image Module
            AddTemplateRule("DateTimeOriginal", "Acquisition DateTime", "(0008,002A)", ValueTransform.DateTimeToDicom, "EXIF");

            IsModified = true;
            StatusMessage = "Full template applied";
        }

        private void AddTemplateRule(string sourceField, string description, string dicomTag,
            ValueTransform transform, string sourceType, bool isRequired = false)
        {
            var rule = new MappingRule
            {
                SourceField = sourceField,
                Description = description,
                DicomTag = dicomTag,
                Transform = transform.ToString(),
                SourceType = sourceType,
                Required = isRequired
            };

            var vm = new MappingRuleViewModel(rule);
            vm.PropertyChanged += (s, e) =>
            {
                IsModified = true;
                if (s == SelectedRule)
                    UpdatePreview();
            };
            MappingRules.Add(vm);
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
    /// ViewModel wrapper for MappingRule - adapted to work with Core v0.5.x
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
            _displayName = rule.Description ?? rule.SourceField;
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
