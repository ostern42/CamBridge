// src/CamBridge.Config/Dialogs/DicomTagBrowserDialog.xaml.cs
// Version: 0.7.25
// Copyright: Â© 2025 Claude's Improbably Reliable Software Solutions
// Enhanced with NEMA-compliant descriptions

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using ModernWpf.Controls;
// Use alias to avoid conflicts
using CoreDicomTag = CamBridge.Core.ValueObjects.DicomTag;

namespace CamBridge.Config.Dialogs
{
    /// <summary>
    /// Dialog for browsing and selecting DICOM tags with search functionality
    /// </summary>
    [SupportedOSPlatform("windows")]
    public partial class DicomTagBrowserDialog : Window, INotifyPropertyChanged
    {
        #region Properties

        private string? _searchText;
        public string? SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged();
                    UpdateFilter();
                }
            }
        }

        public CoreDicomTag? SelectedTag { get; private set; }
        public string? SelectedTagName { get; private set; }

        private CollectionViewSource _tagsViewSource = null!;
        public ICollectionView TagsView => _tagsViewSource.View;

        private List<DicomTagInfo> _allTags = null!;

        #endregion

        public DicomTagBrowserDialog()
        {
            InitializeComponent();
            DataContext = this;

            // Initialize collections before use
            _allTags = new List<DicomTagInfo>();
            _tagsViewSource = new CollectionViewSource();

            LoadDicomTags();
            SearchBox.Focus();
        }

        #region Initialization

        private void LoadDicomTags()
        {
            // Clear and reinitialize
            _allTags.Clear();

            // Patient Module - NEMA PS3.3 Table C.7-1
            AddTag("Patient", CoreDicomTag.PatientModule.PatientName, "Patient's Name", "PN",
                "Primary identifier - Format: Family^Given^Middle^Prefix^Suffix");
            AddTag("Patient", CoreDicomTag.PatientModule.PatientID, "Patient ID", "LO",
                "Primary hospital/institution identification number");
            AddTag("Patient", CoreDicomTag.PatientModule.PatientBirthDate, "Patient's Birth Date", "DA",
                "Format: YYYYMMDD (e.g., 19850315 for March 15, 1985)");
            AddTag("Patient", CoreDicomTag.PatientModule.PatientSex, "Patient's Sex", "CS",
                "M=Male, F=Female, O=Other (fixed enumeration)");
            AddTag("Patient", CoreDicomTag.PatientModule.OtherPatientIDs, "Other Patient IDs", "LO",
                "Additional identifiers from other systems");
            AddTag("Patient", CoreDicomTag.PatientModule.PatientComments, "Patient Comments", "LT",
                "Additional information about the patient");

            // Study Module - NEMA PS3.3 Table C.7-3
            AddTag("Study", CoreDicomTag.StudyModule.StudyInstanceUID, "Study Instance UID", "UI",
                "Unique identifier for the study - automatically generated");
            AddTag("Study", CoreDicomTag.StudyModule.StudyDate, "Study Date", "DA",
                "Date the study started - Format: YYYYMMDD");
            AddTag("Study", CoreDicomTag.StudyModule.StudyTime, "Study Time", "TM",
                "Time the study started - Format: HHMMSS.FFFFFF");
            AddTag("Study", CoreDicomTag.StudyModule.StudyID, "Study ID", "SH",
                "Institution-generated study identifier");
            AddTag("Study", CoreDicomTag.StudyModule.AccessionNumber, "Accession Number", "SH",
                "RIS/HIS generated number that identifies the order");
            AddTag("Study", CoreDicomTag.StudyModule.StudyDescription, "Study Description", "LO",
                "Institution-generated description or classification of the study");
            AddTag("Study", CoreDicomTag.StudyModule.ReferringPhysicianName, "Referring Physician's Name", "PN",
                "Name of the physician who requested the study");

            // Series Module - NEMA PS3.3 Table C.7-5a
            AddTag("Series", CoreDicomTag.SeriesModule.SeriesInstanceUID, "Series Instance UID", "UI",
                "Unique identifier for the series - automatically generated");
            AddTag("Series", CoreDicomTag.SeriesModule.SeriesNumber, "Series Number", "IS",
                "A number that identifies this series (e.g., 1, 2, 3...)");
            AddTag("Series", CoreDicomTag.SeriesModule.SeriesDate, "Series Date", "DA",
                "Date the series started");
            AddTag("Series", CoreDicomTag.SeriesModule.SeriesTime, "Series Time", "TM",
                "Time the series started");
            AddTag("Series", CoreDicomTag.SeriesModule.SeriesDescription, "Series Description", "LO",
                "Description of the series (e.g., 'Chest PA', 'Lateral View')");
            AddTag("Series", CoreDicomTag.SeriesModule.Modality, "Modality", "CS",
                "Type of equipment (e.g., XA=X-Ray Angiography, OT=Other)");

            // General Image Module - NEMA PS3.3 Table C.7-9
            AddTag("Image", CoreDicomTag.InstanceModule.SOPInstanceUID, "SOP Instance UID", "UI",
                "Unique identifier for this image - automatically generated");
            AddTag("Image", CoreDicomTag.InstanceModule.InstanceNumber, "Instance Number", "IS",
                "A number that identifies this image (1, 2, 3...)");
            AddTag("Image", CoreDicomTag.InstanceModule.ContentDate, "Content Date", "DA",
                "The date the image pixel data creation started");
            AddTag("Image", CoreDicomTag.InstanceModule.ContentTime, "Content Time", "TM",
                "The time the image pixel data creation started");
            AddTag("Image", CoreDicomTag.InstanceModule.AcquisitionDateTime, "Acquisition DateTime", "DT",
                "Date and time the acquisition started - Format: YYYYMMDDHHMMSS.FFFFFF");

            // General Equipment Module - NEMA PS3.3 Table C.7-8
            AddTag("Equipment", CoreDicomTag.EquipmentModule.Manufacturer, "Manufacturer", "LO",
                "Manufacturer of the equipment (e.g., 'RICOH')");
            AddTag("Equipment", CoreDicomTag.EquipmentModule.InstitutionName, "Institution Name", "LO",
                "Institution where the equipment is located");
            AddTag("Equipment", CoreDicomTag.EquipmentModule.StationName, "Station Name", "SH",
                "User-defined name identifying the machine");
            AddTag("Equipment", CoreDicomTag.EquipmentModule.ManufacturerModelName, "Manufacturer's Model Name", "LO",
                "Manufacturer's model name (e.g., 'G900 II')");
            AddTag("Equipment", CoreDicomTag.EquipmentModule.SoftwareVersions, "Software Versions", "LO",
                "Manufacturer's software version (e.g., 'CamBridge 0.7.25')");

            // Additional commonly used tags
            AddTag("General", new CoreDicomTag(0x0008, 0x0005), "Specific Character Set", "CS",
                "Character encoding (e.g., 'ISO_IR 100' for Latin-1)");
            AddTag("General", new CoreDicomTag(0x0008, 0x0016), "SOP Class UID", "UI",
                "Identifies the DICOM IOD (e.g., Secondary Capture)");
            AddTag("General", new CoreDicomTag(0x0008, 0x0064), "Conversion Type", "CS",
                "Describes the conversion (e.g., 'WSD' = Workstation)");
            AddTag("General", new CoreDicomTag(0x0020, 0x0013), "Instance Number", "IS",
                "Number that identifies this instance");
            AddTag("General", new CoreDicomTag(0x0028, 0x0002), "Samples per Pixel", "US",
                "Number of samples per pixel (1=grayscale, 3=color)");
            AddTag("General", new CoreDicomTag(0x0028, 0x0004), "Photometric Interpretation", "CS",
                "Color space (e.g., 'RGB', 'YBR_FULL_422')");

            // Setup CollectionViewSource with grouping
            _tagsViewSource = new CollectionViewSource { Source = _allTags };
            _tagsViewSource.GroupDescriptions.Add(new PropertyGroupDescription("Module"));
            _tagsViewSource.SortDescriptions.Add(new SortDescription("Module", ListSortDirection.Ascending));
            _tagsViewSource.SortDescriptions.Add(new SortDescription("TagString", ListSortDirection.Ascending));

            // Notify UI
            OnPropertyChanged(nameof(TagsView));
        }

        private void AddTag(string module, CoreDicomTag tag, string name, string vr, string description)
        {
            _allTags.Add(new DicomTagInfo
            {
                Module = module,
                Tag = tag,
                Name = name,
                VR = vr,
                VRDescription = GetVRDescription(vr),
                TagString = tag.ToString(),
                Description = description,
                DisplayText = $"{tag} - {name} ({vr})"
            });
        }

        private string GetVRDescription(string vr)
        {
            return vr switch
            {
                "CS" => "Code String - max 16 chars",
                "DA" => "Date - YYYYMMDD",
                "DT" => "DateTime - YYYYMMDDHHMMSS.FFFFFF",
                "IS" => "Integer String - max 12 chars",
                "LO" => "Long String - max 64 chars",
                "LT" => "Long Text - max 10240 chars",
                "PN" => "Person Name - 5 components with ^",
                "SH" => "Short String - max 16 chars",
                "TM" => "Time - HHMMSS.FFFFFF",
                "UI" => "Unique Identifier - max 64 chars",
                "US" => "Unsigned Short - 2 bytes",
                _ => vr
            };
        }

        #endregion

        #region Search and Filter

        private void UpdateFilter()
        {
            if (TagsView == null) return;

            if (string.IsNullOrWhiteSpace(SearchText))
            {
                TagsView.Filter = null;
            }
            else
            {
                TagsView.Filter = obj =>
                {
                    if (obj is DicomTagInfo tagInfo)
                    {
                        var searchLower = SearchText.ToLower();
                        return tagInfo.Name.ToLower().Contains(searchLower) ||
                               tagInfo.TagString.Contains(searchLower) ||
                               tagInfo.Module.ToLower().Contains(searchLower) ||
                               tagInfo.VR.ToLower().Contains(searchLower) ||
                               tagInfo.Description.ToLower().Contains(searchLower);
                    }
                    return false;
                };
            }

            // Select first item if any
            TagsView.MoveCurrentToFirst();
        }

        #endregion

        #region Event Handlers

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (TagsListView.SelectedItem is DicomTagInfo tagInfo)
            {
                SelectedTag = tagInfo.Tag;
                SelectedTagName = tagInfo.Name;  // NEW LINE!
                DialogResult = true;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void TagsListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (TagsListView.SelectedItem != null)
            {
                OkButton_Click(sender, e);
            }
        }

        private void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down && TagsListView.Items.Count > 0)
            {
                TagsListView.Focus();
                TagsListView.SelectedIndex = 0;
            }
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                DialogResult = false;
            }
            else if (e.Key == Key.Enter && TagsListView.SelectedItem != null)
            {
                OkButton_Click(sender, e);
            }
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Inner Classes

        /// <summary>
        /// DICOM tag information for display
        /// </summary>
        public class DicomTagInfo
        {
            public string Module { get; set; } = string.Empty;
            public CoreDicomTag Tag { get; set; } = null!;
            public string Name { get; set; } = string.Empty;
            public string VR { get; set; } = string.Empty;
            public string VRDescription { get; set; } = string.Empty;
            public string TagString { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public string DisplayText { get; set; } = string.Empty;
        }

        #endregion
    }
}
