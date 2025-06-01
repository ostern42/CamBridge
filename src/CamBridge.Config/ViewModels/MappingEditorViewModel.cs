// src/CamBridge.Config/ViewModels/MappingEditorViewModel.cs
using CamBridge.Config.Dialogs;
using CamBridge.Config.Services;
using CamBridge.Core;
using CamBridge.Core.Interfaces;
using CamBridge.Core.ValueObjects;
using CamBridge.Infrastructure.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace CamBridge.Config.ViewModels
{
    /// <summary>
    /// ViewModel for the mapping editor with drag & drop support and templates
    /// </summary>
    public partial class MappingEditorViewModel : ViewModelBase
    {
        private readonly ILogger<MappingEditorViewModel> _logger;
        private readonly IConfigurationService _configurationService;
        private readonly MappingConfigurationLoader _mappingLoader;

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
            _mappingLoader = new MappingConfigurationLoader(_logger);

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
                    var mappingConfig = await _mappingLoader.LoadFromFileAsync(settings.MappingConfigurationFile);

                    MappingRules.Clear();
                    foreach (var rule in mappingConfig.GetMappingRules())
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
            var newRule = new MappingRule(
                $"NewRule_{DateTime.Now:HHmmss}",
                "QRBridge",
                "examid",
                DicomTag.StudyModule.StudyID,
                ValueTransform.None
            );

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
                    var config = new CustomMappingConfiguration();
                    foreach (var ruleVm in MappingRules)
                    {
                        config.AddRule(ruleVm.ToMappingRule());
                    }

                    await _mappingLoader.SaveToFileAsync(config, settings.MappingConfigurationFile);

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

                    var importedConfig = await _mappingLoader.LoadFromFileAsync(dialog.FileName);

                    MappingRules.Clear();
                    foreach (var rule in importedConfig.GetMappingRules())
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

                    var config = new CustomMappingConfiguration();
                    foreach (var ruleVm in MappingRules)
                    {
                        config.AddRule(ruleVm.ToMappingRule());
                    }

                    await _mappingLoader.SaveToFileAsync(config, dialog.FileName);

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
                SelectedRule.TargetTag = dialog.SelectedTag;
                SelectedRule.TargetTagString = dialog.SelectedTag.ToString();
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
            AddTemplateRule("PatientName", "QRBridge", "name",
                DicomTag.PatientModule.PatientName, ValueTransform.None, true);
            AddTemplateRule("PatientID", "QRBridge", "patientid",
                DicomTag.PatientModule.PatientID, ValueTransform.None, true);
            AddTemplateRule("PatientBirthDate", "QRBridge", "birthdate",
                DicomTag.PatientModule.PatientBirthDate, ValueTransform.DateToDicom);
            AddTemplateRule("PatientSex", "QRBridge", "gender",
                DicomTag.PatientModule.PatientSex, ValueTransform.GenderToDicom);

            // Study information
            AddTemplateRule("StudyID", "QRBridge", "examid",
                DicomTag.StudyModule.StudyID, ValueTransform.TruncateTo16);
            AddTemplateRule("StudyDescription", "QRBridge", "comment",
                DicomTag.StudyModule.StudyDescription, ValueTransform.None);

            // Equipment from EXIF
            AddTemplateRule("Manufacturer", "EXIF", "Make",
                DicomTag.EquipmentModule.Manufacturer, ValueTransform.None);
            AddTemplateRule("Model", "EXIF", "Model",
                DicomTag.EquipmentModule.ManufacturerModelName, ValueTransform.None);

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
            AddTemplateRule("PatientName", "QRBridge", "name",
                DicomTag.PatientModule.PatientName, ValueTransform.None, true);
            AddTemplateRule("PatientID", "QRBridge", "examid",
                DicomTag.PatientModule.PatientID, ValueTransform.None, true);
            AddTemplateRule("StudyID", "QRBridge", "examid",
                DicomTag.StudyModule.StudyID, ValueTransform.None);

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
            AddTemplateRule("PatientName", "QRBridge", "name",
                DicomTag.PatientModule.PatientName, ValueTransform.None, true);
            AddTemplateRule("PatientID", "QRBridge", "patientid",
                DicomTag.PatientModule.PatientID, ValueTransform.None, true);
            AddTemplateRule("PatientBirthDate", "QRBridge", "birthdate",
                DicomTag.PatientModule.PatientBirthDate, ValueTransform.DateToDicom);
            AddTemplateRule("PatientSex", "QRBridge", "gender",
                DicomTag.PatientModule.PatientSex, ValueTransform.GenderToDicom);
            AddTemplateRule("PatientComments", "QRBridge", "comment",
                DicomTag.PatientModule.PatientComments, ValueTransform.None);

            // Study Module
            AddTemplateRule("StudyID", "QRBridge", "examid",
                DicomTag.StudyModule.StudyID, ValueTransform.TruncateTo16);
            AddTemplateRule("AccessionNumber", "QRBridge", "examid",
                DicomTag.StudyModule.AccessionNumber, ValueTransform.None);
            AddTemplateRule("StudyDescription", "QRBridge", "comment",
                DicomTag.StudyModule.StudyDescription, ValueTransform.None);
            AddTemplateRule("StudyDate", "EXIF", "DateTimeOriginal",
                DicomTag.StudyModule.StudyDate, ValueTransform.DateToDicom);
            AddTemplateRule("StudyTime", "EXIF", "DateTimeOriginal",
                DicomTag.StudyModule.StudyTime, ValueTransform.TimeToDicom);

            // Equipment Module
            AddTemplateRule("Manufacturer", "EXIF", "Make",
                DicomTag.EquipmentModule.Manufacturer, ValueTransform.None);
            AddTemplateRule("Model", "EXIF", "Model",
                DicomTag.EquipmentModule.ManufacturerModelName, ValueTransform.None);
            AddTemplateRule("Software", "EXIF", "Software",
                DicomTag.EquipmentModule.SoftwareVersions, ValueTransform.None);

            // Image Module
            AddTemplateRule("AcquisitionDateTime", "EXIF", "DateTimeOriginal",
                DicomTag.ImageModule.AcquisitionDateTime, ValueTransform.DateToDicom);

            IsModified = true;
            StatusMessage = "Full template applied";
        }

        private void AddTemplateRule(string name, string sourceType, string sourceField,
            DicomTag targetTag, ValueTransform transform, bool isRequired = false)
        {
            var rule = new MappingRule(name, sourceType, sourceField, targetTag,
                transform, isRequired);
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

        private void UpdatePreview()
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
    /// ViewModel wrapper for MappingRule
    /// </summary>
    public partial class MappingRuleViewModel : ObservableObject
    {
        private readonly MappingRule _rule;

        [ObservableProperty] private string _name;
        [ObservableProperty] private string _sourceType;
        [ObservableProperty] private string _sourceField;
        [ObservableProperty] private DicomTag _targetTag;
        [ObservableProperty] private string _targetTagString;
        [ObservableProperty] private ValueTransform _transform;
        [ObservableProperty] private bool _isRequired;
        [ObservableProperty] private string? _defaultValue;

        public MappingRuleViewModel(MappingRule rule)
        {
            _rule = rule;
            _name = rule.Name;
            _sourceType = rule.SourceType;
            _sourceField = rule.SourceField;
            _targetTag = rule.TargetTag;
            _targetTagString = rule.TargetTag.ToString();
            _transform = rule.Transform;
            _isRequired = rule.IsRequired;
            _defaultValue = rule.DefaultValue;
        }

        public MappingRule ToMappingRule()
        {
            return new MappingRule(
                Name,
                SourceType,
                SourceField,
                TargetTag,
                Transform,
                IsRequired,
                DefaultValue
            );
        }

        partial void OnTargetTagChanged(DicomTag value)
        {
            TargetTagString = value?.ToString() ?? string.Empty;
        }
    }
}
