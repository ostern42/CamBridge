// src/CamBridge.Config/Dialogs/TransformEditorDialog.xaml.cs
// Version: 0.7.26
// Copyright: © 2025 Claude's Improbably Reliable Software Solutions

using CamBridge.Core;
using CamBridge.Core.ValueObjects;
using ModernWpf.Controls;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace CamBridge.Config.Dialogs
{
    /// <summary>
    /// Enhanced dialog for editing mapping rule transformations with encoding preview
    /// </summary>
    public partial class TransformEditorDialog : ContentDialog, INotifyPropertyChanged
    {
        private ValueTransform _selectedTransform;
        private string _sourceField = "";
        private string _dicomTag = "";
        private string _dicomTagName = "";
        private string _previewInput = "";
        private string _previewOutput = "";
        private string _previewOutputSpecialChars = "";
        private string _previewOutputHex = "";
        private bool _showPreview = true;
        private bool _showNormalView = true;
        private bool _showSpecialCharsView = false;
        private bool _showHexView = false;
        private string _inputEncoding = "UTF-8";
        private string _outputEncoding = "ISO_IR 100";
        private bool _showEncodingWarning = false;
        private string _encodingWarning = "";
        private bool _showDicomInfo = false;
        private string _dicomInfo = "";

        public TransformEditorDialog()
        {
            InitializeComponent();
            DataContext = this;
            InitializeTransforms();
        }

        #region Properties

        public ObservableCollection<ValueTransform> AvailableTransforms { get; } = new();

        public ValueTransform SelectedTransform
        {
            get => _selectedTransform;
            set
            {
                if (_selectedTransform != value)
                {
                    _selectedTransform = value;
                    OnPropertyChanged();

                    // Update preview input based on new transform
                    UpdatePreviewInputForTransform();

                    UpdatePreview();
                    UpdateShowPreview();
                    UpdateDicomInfo();
                }
            }
        }

        public string SourceField
        {
            get => _sourceField;
            set
            {
                _sourceField = value;
                OnPropertyChanged();
            }
        }

        public string DicomTag
        {
            get => _dicomTag;
            set
            {
                _dicomTag = value;
                OnPropertyChanged();
                UpdateDicomInfo();
            }
        }

        public string DicomTagName
        {
            get => _dicomTagName;
            set
            {
                _dicomTagName = value;
                OnPropertyChanged();
            }
        }

        public string PreviewInput
        {
            get => _previewInput;
            set
            {
                _previewInput = value;
                OnPropertyChanged();
                DetectInputEncoding();
                UpdatePreview();
            }
        }

        public string PreviewOutput
        {
            get => _previewOutput;
            set
            {
                _previewOutput = value;
                OnPropertyChanged();
            }
        }

        public string PreviewOutputSpecialChars
        {
            get => _previewOutputSpecialChars;
            set
            {
                _previewOutputSpecialChars = value;
                OnPropertyChanged();
            }
        }

        public string PreviewOutputHex
        {
            get => _previewOutputHex;
            set
            {
                _previewOutputHex = value;
                OnPropertyChanged();
            }
        }

        public bool ShowPreview
        {
            get => _showPreview;
            set
            {
                _showPreview = value;
                OnPropertyChanged();
            }
        }

        public bool ShowNormalView
        {
            get => _showNormalView;
            set
            {
                _showNormalView = value;
                OnPropertyChanged();
                if (value)
                {
                    ShowSpecialCharsView = false;
                    ShowHexView = false;
                }
            }
        }

        public bool ShowSpecialCharsView
        {
            get => _showSpecialCharsView;
            set
            {
                _showSpecialCharsView = value;
                OnPropertyChanged();
                if (value)
                {
                    ShowNormalView = false;
                    ShowHexView = false;
                }
            }
        }

        public bool ShowHexView
        {
            get => _showHexView;
            set
            {
                _showHexView = value;
                OnPropertyChanged();
                if (value)
                {
                    ShowNormalView = false;
                    ShowSpecialCharsView = false;
                }
            }
        }

        public string InputEncoding
        {
            get => _inputEncoding;
            set
            {
                _inputEncoding = value;
                OnPropertyChanged();
            }
        }

        public string OutputEncoding
        {
            get => _outputEncoding;
            set
            {
                _outputEncoding = value;
                OnPropertyChanged();
            }
        }

        public bool ShowEncodingWarning
        {
            get => _showEncodingWarning;
            set
            {
                _showEncodingWarning = value;
                OnPropertyChanged();
            }
        }

        public string EncodingWarning
        {
            get => _encodingWarning;
            set
            {
                _encodingWarning = value;
                OnPropertyChanged();
            }
        }

        public bool ShowDicomInfo
        {
            get => _showDicomInfo;
            set
            {
                _showDicomInfo = value;
                OnPropertyChanged();
            }
        }

        public string DicomInfo
        {
            get => _dicomInfo;
            set
            {
                _dicomInfo = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Methods

        private void UpdatePreviewInputForTransform()
        {
            // Update preview input when transform changes to provide appropriate test data
            PreviewInput = SelectedTransform switch
            {
                ValueTransform.DateToDicom => "1985-03-15",
                ValueTransform.ExtractDate => DateTime.Now.ToString("yyyy:MM:dd HH:mm:ss"),
                ValueTransform.ExtractTime => DateTime.Now.ToString("yyyy:MM:dd HH:mm:ss"),
                ValueTransform.DateTimeToDicom => DateTime.Now.ToString("yyyy:MM:dd HH:mm:ss"),
                ValueTransform.TimeToDicom => "14:30:45",
                ValueTransform.MapGender => "M",
                ValueTransform.ToUpperCase => "test text",
                ValueTransform.ToLowerCase => "TEST TEXT",
                ValueTransform.Trim => "  trimmed text  ",
                ValueTransform.RemovePrefix => "PREFIX_Value",
                ValueTransform.None => SourceField switch
                {
                    "birthdate" => "1985-03-15",
                    "gender" => "M",
                    "DateTimeOriginal" => DateTime.Now.ToString("yyyy:MM:dd HH:mm:ss"),
                    "name" => "Schmidt, Maria",
                    "examid" => "EX002",
                    "comment" => "Röntgen Thorax",
                    _ => "Sample Text"
                },
                _ => PreviewInput // Keep existing input
            };
        }

        private void InitializeTransforms()
        {
            // Add all available transforms
            foreach (ValueTransform transform in Enum.GetValues(typeof(ValueTransform)))
            {
                AvailableTransforms.Add(transform);
            }
        }

        private void DetectInputEncoding()
        {
            if (string.IsNullOrEmpty(PreviewInput))
            {
                InputEncoding = "UTF-8";
                return;
            }

            // Check for common encoding indicators
            bool hasUmlauts = PreviewInput.Any(c => "äöüÄÖÜß".Contains(c));
            bool hasExtendedAscii = PreviewInput.Any(c => c > 127);

            if (hasExtendedAscii && !hasUmlauts)
            {
                InputEncoding = "Windows-1252";
            }
            else if (hasUmlauts)
            {
                InputEncoding = "UTF-8";
            }
            else
            {
                InputEncoding = "ASCII";
            }
        }

        private void UpdatePreview()
        {
            if (string.IsNullOrEmpty(PreviewInput))
            {
                PreviewOutput = "";
                PreviewOutputSpecialChars = "";
                PreviewOutputHex = "";
                ShowEncodingWarning = false;
                return;
            }

            try
            {
                // Create a temporary rule to apply the transform
                var tempRule = new MappingRule
                {
                    Transform = SelectedTransform.ToString()
                };

                PreviewOutput = tempRule.ApplyTransform(PreviewInput) ?? "";

                // Create special chars view
                PreviewOutputSpecialChars = CreateSpecialCharsView(PreviewOutput);

                // Create hex view
                PreviewOutputHex = CreateHexView(PreviewOutput);

                // Check for encoding issues
                CheckEncodingIssues();
            }
            catch (Exception ex)
            {
                PreviewOutput = $"Error: {ex.Message}";
                PreviewOutputSpecialChars = PreviewOutput;
                PreviewOutputHex = "";
            }
        }

        private string CreateSpecialCharsView(string input)
        {
            if (string.IsNullOrEmpty(input)) return "";

            return input
                .Replace("\r", "[CR]")
                .Replace("\n", "[LF]")
                .Replace("\t", "[TAB]")
                .Replace("\0", "[NULL]")
                .Replace("\x1B", "[ESC]");
        }

        private string CreateHexView(string input)
        {
            if (string.IsNullOrEmpty(input)) return "";

            var sb = new StringBuilder();
            var bytes = Encoding.UTF8.GetBytes(input);

            for (int i = 0; i < bytes.Length; i++)
            {
                if (i > 0 && i % 16 == 0)
                {
                    sb.AppendLine();
                }
                else if (i > 0)
                {
                    sb.Append(" ");
                }

                sb.AppendFormat("{0:X2}", bytes[i]);
            }

            return sb.ToString();
        }

        private void CheckEncodingIssues()
        {
            ShowEncodingWarning = false;

            // Check for problematic characters
            if (PreviewOutput.Contains('?') && !PreviewInput.Contains('?'))
            {
                ShowEncodingWarning = true;
                EncodingWarning = "⚠ Character encoding issue detected - some characters may be lost in DICOM conversion";
            }
            else if (PreviewOutput.Any(c => c > 255))
            {
                ShowEncodingWarning = true;
                EncodingWarning = "⚠ Output contains Unicode characters that may not be supported in DICOM ISO_IR 100";
            }
        }

        private void UpdateShowPreview()
        {
            // Hide preview for certain transforms that don't need it
            ShowPreview = SelectedTransform != ValueTransform.None;
        }

        private void UpdateDicomInfo()
        {
            ShowDicomInfo = false;

            // Show DICOM-specific info for certain tags/transforms
            if (DicomTag == "(0010,0010)" && SelectedTransform == ValueTransform.None)
            {
                ShowDicomInfo = true;
                DicomInfo = "Patient Name: Max 64 chars, format: Last^First^Middle^Prefix^Suffix";
            }
            else if (DicomTag == "(0010,0030)" && SelectedTransform == ValueTransform.DateToDicom)
            {
                ShowDicomInfo = true;
                DicomInfo = "Birth Date: DICOM format YYYYMMDD, no separators";
            }
            else if (DicomTag == "(0010,0040)" && SelectedTransform == ValueTransform.MapGender)
            {
                ShowDicomInfo = true;
                DicomInfo = "Patient Sex: Valid values are M, F, O (Other)";
            }
        }

        public void SetMapping(string sourceField, string dicomTag, ValueTransform currentTransform)
        {
            SourceField = sourceField;
            DicomTag = dicomTag;
            SelectedTransform = currentTransform;

            // Look up DICOM tag name
            DicomTagName = GetDicomTagNameByString(dicomTag);

            // Set appropriate preview input based on transform type
            PreviewInput = currentTransform switch
            {
                ValueTransform.DateToDicom => "1985-03-15",
                ValueTransform.ExtractDate => DateTime.Now.ToString("yyyy:MM:dd HH:mm:ss"),
                ValueTransform.ExtractTime => DateTime.Now.ToString("yyyy:MM:dd HH:mm:ss"),
                ValueTransform.DateTimeToDicom => DateTime.Now.ToString("yyyy:MM:dd HH:mm:ss"),
                ValueTransform.TimeToDicom => "14:30:45",
                ValueTransform.MapGender => "M",
                ValueTransform.ToUpperCase => "test text",
                ValueTransform.ToLowerCase => "TEST TEXT",
                ValueTransform.Trim => "  trimmed text  ",
                ValueTransform.RemovePrefix => "PREFIX_Value",
                _ => sourceField switch
                {
                    "birthdate" => "1985-03-15",
                    "gender" => "M",
                    "DateTimeOriginal" => DateTime.Now.ToString("yyyy:MM:dd HH:mm:ss"),
                    "name" => "Schmidt, Maria",
                    "examid" => "EX002",
                    "comment" => "Röntgen Thorax\r\nAP-Aufnahme",
                    _ => "Sample Text"
                }
            };
        }

        private string GetDicomTagNameByString(string tagString)
        {
            // Direct tag name lookup by string
            return tagString switch
            {
                "(0010,0010)" => "Patient's Name",
                "(0010,0020)" => "Patient ID",
                "(0010,0030)" => "Patient's Birth Date",
                "(0010,0040)" => "Patient's Sex",
                "(0020,0010)" => "Study ID",
                "(0008,1030)" => "Study Description",
                "(0008,0020)" => "Study Date",
                "(0008,0030)" => "Study Time",
                "(0008,0070)" => "Manufacturer",
                "(0008,1090)" => "Manufacturer's Model Name",
                "(0018,1020)" => "Software Versions",
                "(0008,002A)" => "Acquisition DateTime",
                "(0010,4000)" => "Patient Comments",
                "(0008,0050)" => "Accession Number",
                _ => "Custom Tag"
            };
        }

        private string GetDicomTagName(Core.ValueObjects.DicomTag tag)
        {
            return GetDicomTagNameByString(tag.ToString());
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
