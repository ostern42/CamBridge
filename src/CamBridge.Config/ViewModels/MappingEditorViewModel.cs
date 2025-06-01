using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CamBridge.Config.Models;
using CamBridge.Config.Services;
using CamBridge.Core;
using CamBridge.Core.ValueObjects;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CamBridge.Config.ViewModels
{
    public partial class MappingEditorViewModel : ViewModelBase
    {
        private readonly IConfigurationService _configurationService;
        private CamBridgeSettings? _settings;

        [ObservableProperty]
        private ObservableCollection<MappingRuleViewModel> _mappings = new();

        [ObservableProperty]
        private MappingRuleViewModel? _selectedMapping;

        [ObservableProperty]
        private ObservableCollection<FieldDefinition> _availableQRBridgeFields = new();

        [ObservableProperty]
        private ObservableCollection<FieldDefinition> _availableExifFields = new();

        [ObservableProperty]
        private ObservableCollection<DicomTagDefinition> _commonDicomTags = new();

        [ObservableProperty]
        private string _previewSource = "Schmidt, Maria";

        [ObservableProperty]
        private string _previewResult = "Schmidt^Maria";

        [ObservableProperty]
        private bool _hasChanges;

        [ObservableProperty]
        private string? _statusMessage;

        [ObservableProperty]
        private bool _isValid = true;

        [ObservableProperty]
        private string _validationMessage = "All required fields mapped";

        public MappingEditorViewModel(IConfigurationService configurationService)
        {
            _configurationService = configurationService ?? throw new ArgumentNullException(nameof(configurationService));

            InitializeAvailableFields();
            InitializeCommonDicomTags();
        }

        public async Task InitializeAsync()
        {
            try
            {
                IsLoading = true;
                StatusMessage = "Loading mappings...";

                _settings = await _configurationService.LoadConfigurationAsync<CamBridgeSettings>();

                if (_settings != null)
                {
                    LoadMappingsFromSettings();
                    ValidateMappings();
                }

                StatusMessage = "Mappings loaded successfully";
                HasChanges = false;
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading mappings: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void InitializeAvailableFields()
        {
            // QRBridge fields
            AvailableQRBridgeFields.Add(new FieldDefinition("examid", "Exam ID", "Examination identifier"));
            AvailableQRBridgeFields.Add(new FieldDefinition("name", "Patient Name", "Patient's full name"));
            AvailableQRBridgeFields.Add(new FieldDefinition("birthdate", "Birth Date", "Patient's date of birth"));
            AvailableQRBridgeFields.Add(new FieldDefinition("gender", "Gender", "Patient's gender (M/F/O)"));
            AvailableQRBridgeFields.Add(new FieldDefinition("comment", "Comment", "Study description or comment"));

            // Common EXIF fields
            AvailableExifFields.Add(new FieldDefinition("Make", "Camera Make", "Camera manufacturer"));
            AvailableExifFields.Add(new FieldDefinition("Model", "Camera Model", "Camera model name"));
            AvailableExifFields.Add(new FieldDefinition("Software", "Software", "Camera software version"));
            AvailableExifFields.Add(new FieldDefinition("DateTimeOriginal", "Capture Time", "Original capture date/time"));
            AvailableExifFields.Add(new FieldDefinition("ImageDescription", "Description", "Image description"));
        }

        private void InitializeCommonDicomTags()
        {
            // Patient Module
            CommonDicomTags.Add(new DicomTagDefinition("(0010,0010)", "PatientName", "Patient's name", true));
            CommonDicomTags.Add(new DicomTagDefinition("(0010,0020)", "PatientID", "Patient identifier", true));
            CommonDicomTags.Add(new DicomTagDefinition("(0010,0030)", "PatientBirthDate", "Patient's birth date"));
            CommonDicomTags.Add(new DicomTagDefinition("(0010,0040)", "PatientSex", "Patient's sex"));

            // Study Module
            CommonDicomTags.Add(new DicomTagDefinition("(0020,0010)", "StudyID", "Study identifier"));
            CommonDicomTags.Add(new DicomTagDefinition("(0008,1030)", "StudyDescription", "Study description"));
            CommonDicomTags.Add(new DicomTagDefinition("(0008,0050)", "AccessionNumber", "Accession number"));

            // Equipment Module
            CommonDicomTags.Add(new DicomTagDefinition("(0008,0070)", "Manufacturer", "Equipment manufacturer"));
            CommonDicomTags.Add(new DicomTagDefinition("(0008,1090)", "ManufacturerModelName", "Equipment model"));
            CommonDicomTags.Add(new DicomTagDefinition("(0018,1020)", "SoftwareVersions", "Software version"));
        }

        private void LoadMappingsFromSettings()
        {
            Mappings.Clear();

            // TODO: Load actual mappings from configuration file
            // For now, add some default mappings
            AddDefaultMappings();
        }

        private void AddDefaultMappings()
        {
            // Default patient mappings
            Mappings.Add(new MappingRuleViewModel
            {
                Name = "PatientName",
                SourceType = "QRBridge",
                SourceField = "name",
                TargetTag = "(0010,0010)",
                TargetTagName = "PatientName",
                Transform = ValueTransform.None,
                IsRequired = true
            });

            Mappings.Add(new MappingRuleViewModel
            {
                Name = "PatientID",
                SourceType = "QRBridge",
                SourceField = "patientid",
                TargetTag = "(0010,0020)",
                TargetTagName = "PatientID",
                Transform = ValueTransform.None,
                IsRequired = true
            });

            Mappings.Add(new MappingRuleViewModel
            {
                Name = "PatientBirthDate",
                SourceType = "QRBridge",
                SourceField = "birthdate",
                TargetTag = "(0010,0030)",
                TargetTagName = "PatientBirthDate",
                Transform = ValueTransform.DateToDicom
            });

            // Equipment mappings
            Mappings.Add(new MappingRuleViewModel
            {
                Name = "Manufacturer",
                SourceType = "EXIF",
                SourceField = "Make",
                TargetTag = "(0008,0070)",
                TargetTagName = "Manufacturer",
                Transform = ValueTransform.None
            });
        }

        private void ValidateMappings()
        {
            // Check if required DICOM tags are mapped
            var hasPatientName = Mappings.Any(m => m.TargetTag == "(0010,0010)");
            var hasPatientId = Mappings.Any(m => m.TargetTag == "(0010,0020)");

            if (!hasPatientName || !hasPatientId)
            {
                IsValid = false;
                ValidationMessage = "Missing required mappings: ";
                if (!hasPatientName) ValidationMessage += "PatientName ";
                if (!hasPatientId) ValidationMessage += "PatientID";
            }
            else
            {
                IsValid = true;
                ValidationMessage = "All required fields mapped";
            }
        }

        [RelayCommand]
        private void AddMapping()
        {
            var newMapping = new MappingRuleViewModel
            {
                Name = "NewMapping",
                SourceType = "QRBridge",
                SourceField = "",
                TargetTag = "",
                Transform = ValueTransform.None
            };

            Mappings.Add(newMapping);
            SelectedMapping = newMapping;
            HasChanges = true;
        }

        [RelayCommand]
        private void RemoveMapping(MappingRuleViewModel mapping)
        {
            if (mapping != null)
            {
                Mappings.Remove(mapping);
                HasChanges = true;
                ValidateMappings();
            }
        }

        [RelayCommand]
        private async Task SaveMappings()
        {
            try
            {
                IsLoading = true;
                StatusMessage = "Saving mappings...";

                // TODO: Save mappings to configuration
                await Task.Delay(500); // Simulate save

                HasChanges = false;
                StatusMessage = "Mappings saved successfully";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error saving mappings: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task ImportMappings()
        {
            // TODO: Implement import from JSON file
            StatusMessage = "Import functionality coming soon";
            await Task.CompletedTask;
        }

        [RelayCommand]
        private async Task ExportMappings()
        {
            // TODO: Implement export to JSON file
            StatusMessage = "Export functionality coming soon";
            await Task.CompletedTask;
        }

        [RelayCommand]
        private void AddBasicPatientTemplate()
        {
            // Add basic patient information mappings
            var templates = new[]
            {
                ("PatientName", "name", "(0010,0010)", ValueTransform.None),
                ("PatientID", "patientid", "(0010,0020)", ValueTransform.None),
                ("PatientBirthDate", "birthdate", "(0010,0030)", ValueTransform.DateToDicom),
                ("PatientSex", "gender", "(0010,0040)", ValueTransform.GenderToDicom)
            };

            foreach (var (name, field, tag, transform) in templates)
            {
                if (!Mappings.Any(m => m.TargetTag == tag))
                {
                    Mappings.Add(new MappingRuleViewModel
                    {
                        Name = name,
                        SourceType = "QRBridge",
                        SourceField = field,
                        TargetTag = tag,
                        TargetTagName = name,
                        Transform = transform,
                        IsRequired = tag == "(0010,0010)" || tag == "(0010,0020)"
                    });
                }
            }

            HasChanges = true;
            ValidateMappings();
            StatusMessage = "Basic patient template added";
        }

        partial void OnSelectedMappingChanged(MappingRuleViewModel? value)
        {
            if (value != null)
            {
                UpdatePreview(value);
            }
        }

        private void UpdatePreview(MappingRuleViewModel mapping)
        {
            // Update preview based on selected mapping
            PreviewSource = GetSampleValue(mapping.SourceType, mapping.SourceField);
            PreviewResult = ApplyTransform(PreviewSource, mapping.Transform);
        }

        private string GetSampleValue(string sourceType, string sourceField)
        {
            return (sourceType, sourceField) switch
            {
                ("QRBridge", "name") => "Schmidt, Maria",
                ("QRBridge", "birthdate") => "1985-03-15",
                ("QRBridge", "gender") => "F",
                ("QRBridge", "examid") => "EX002",
                ("QRBridge", "comment") => "Röntgen Thorax",
                ("EXIF", "Make") => "RICOH",
                ("EXIF", "Model") => "G900 II",
                _ => "Sample Value"
            };
        }

        private string ApplyTransform(string value, ValueTransform transform)
        {
            return transform switch
            {
                ValueTransform.DateToDicom => DateTime.TryParse(value, out var date) ? date.ToString("yyyyMMdd") : value,
                ValueTransform.GenderToDicom => value switch
                {
                    "F" or "Female" or "Weiblich" => "F",
                    "M" or "Male" or "Männlich" => "M",
                    _ => "O"
                },
                ValueTransform.ToUpper => value.ToUpper(),
                ValueTransform.ToLower => value.ToLower(),
                _ => value
            };
        }
    }

    // View Models for data binding
    public partial class MappingRuleViewModel : ObservableObject
    {
        [ObservableProperty] private string _name = string.Empty;
        [ObservableProperty] private string _sourceType = string.Empty;
        [ObservableProperty] private string _sourceField = string.Empty;
        [ObservableProperty] private string _targetTag = string.Empty;
        [ObservableProperty] private string _targetTagName = string.Empty;
        [ObservableProperty] private ValueTransform _transform;
        [ObservableProperty] private bool _isRequired;
        [ObservableProperty] private string? _defaultValue;
    }

    public class FieldDefinition
    {
        public string Name { get; }
        public string DisplayName { get; }
        public string Description { get; }

        public FieldDefinition(string name, string displayName, string description)
        {
            Name = name;
            DisplayName = displayName;
            Description = description;
        }
    }

    public class DicomTagDefinition
    {
        public string Tag { get; }
        public string Name { get; }
        public string Description { get; }
        public bool IsRequired { get; }

        public DicomTagDefinition(string tag, string name, string description, bool isRequired = false)
        {
            Tag = tag;
            Name = name;
            Description = description;
            IsRequired = isRequired;
        }
    }
}
