// src/CamBridge.Config/Dialogs/DicomTagBrowserDialog.xaml.cs
// Version: 0.5.26
// Fixed: Nullable warnings resolved

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

            // Patient Module
            AddTag("Patient", CoreDicomTag.PatientModule.PatientName, "Patient's Name", "PN");
            AddTag("Patient", CoreDicomTag.PatientModule.PatientID, "Patient ID", "LO");
            AddTag("Patient", CoreDicomTag.PatientModule.PatientBirthDate, "Patient's Birth Date", "DA");
            AddTag("Patient", CoreDicomTag.PatientModule.PatientSex, "Patient's Sex", "CS");
            AddTag("Patient", CoreDicomTag.PatientModule.OtherPatientIDs, "Other Patient IDs", "LO");
            AddTag("Patient", CoreDicomTag.PatientModule.PatientComments, "Patient Comments", "LT");

            // Study Module
            AddTag("Study", CoreDicomTag.StudyModule.StudyInstanceUID, "Study Instance UID", "UI");
            AddTag("Study", CoreDicomTag.StudyModule.StudyDate, "Study Date", "DA");
            AddTag("Study", CoreDicomTag.StudyModule.StudyTime, "Study Time", "TM");
            AddTag("Study", CoreDicomTag.StudyModule.StudyID, "Study ID", "SH");
            AddTag("Study", CoreDicomTag.StudyModule.AccessionNumber, "Accession Number", "SH");
            AddTag("Study", CoreDicomTag.StudyModule.StudyDescription, "Study Description", "LO");
            AddTag("Study", CoreDicomTag.StudyModule.ReferringPhysicianName, "Referring Physician's Name", "PN");

            // Series Module
            AddTag("Series", CoreDicomTag.SeriesModule.SeriesInstanceUID, "Series Instance UID", "UI");
            AddTag("Series", CoreDicomTag.SeriesModule.SeriesNumber, "Series Number", "IS");
            AddTag("Series", CoreDicomTag.SeriesModule.SeriesDate, "Series Date", "DA");
            AddTag("Series", CoreDicomTag.SeriesModule.SeriesTime, "Series Time", "TM");
            AddTag("Series", CoreDicomTag.SeriesModule.SeriesDescription, "Series Description", "LO");
            AddTag("Series", CoreDicomTag.SeriesModule.Modality, "Modality", "CS");

            // Instance Module (was ImageModule)
            AddTag("Instance", CoreDicomTag.InstanceModule.SOPInstanceUID, "SOP Instance UID", "UI");
            AddTag("Instance", CoreDicomTag.InstanceModule.InstanceNumber, "Instance Number", "IS");
            AddTag("Instance", CoreDicomTag.InstanceModule.ContentDate, "Content Date", "DA");
            AddTag("Instance", CoreDicomTag.InstanceModule.ContentTime, "Content Time", "TM");
            AddTag("Instance", CoreDicomTag.InstanceModule.AcquisitionDateTime, "Acquisition DateTime", "DT");

            // Equipment Module
            AddTag("Equipment", CoreDicomTag.EquipmentModule.Manufacturer, "Manufacturer", "LO");
            AddTag("Equipment", CoreDicomTag.EquipmentModule.InstitutionName, "Institution Name", "LO");
            AddTag("Equipment", CoreDicomTag.EquipmentModule.StationName, "Station Name", "SH");
            AddTag("Equipment", CoreDicomTag.EquipmentModule.ManufacturerModelName, "Manufacturer's Model Name", "LO");
            AddTag("Equipment", CoreDicomTag.EquipmentModule.SoftwareVersions, "Software Versions", "LO");

            // Setup CollectionViewSource with grouping
            _tagsViewSource = new CollectionViewSource { Source = _allTags };
            _tagsViewSource.GroupDescriptions.Add(new PropertyGroupDescription("Module"));
            _tagsViewSource.SortDescriptions.Add(new SortDescription("Module", ListSortDirection.Ascending));
            _tagsViewSource.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));

            // Notify UI
            OnPropertyChanged(nameof(TagsView));
        }

        private void AddTag(string module, CoreDicomTag tag, string name, string vr)
        {
            _allTags.Add(new DicomTagInfo
            {
                Module = module,
                Tag = tag,
                Name = name,
                VR = vr,
                TagString = tag.ToString(),
                DisplayText = $"{tag} - {name} ({vr})"
            });
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
                               tagInfo.VR.ToLower().Contains(searchLower);
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
            public string TagString { get; set; } = string.Empty;
            public string DisplayText { get; set; } = string.Empty;
        }

        #endregion
    }
}
