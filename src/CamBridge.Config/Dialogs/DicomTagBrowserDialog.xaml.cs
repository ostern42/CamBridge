// src/CamBridge.Config/Dialogs/DicomTagBrowserDialog.xaml.cs
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
using CamBridge.Core.ValueObjects;
using ModernWpf.Controls;

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

        public DicomTag? SelectedTag { get; private set; }

        private CollectionViewSource _tagsViewSource;
        public ICollectionView TagsView => _tagsViewSource.View;

        private List<DicomTagInfo> _allTags;

        #endregion

        public DicomTagBrowserDialog()
        {
            InitializeComponent();
            DataContext = this;
            LoadDicomTags();
            SearchBox.Focus();
        }

        #region Initialization

        private void LoadDicomTags()
        {
            // Load all DICOM tags - grouped by module
            _allTags = new List<DicomTagInfo>();

            // Patient Module
            AddTag("Patient", DicomTag.PatientModule.PatientName, "Patient's Name", "PN");
            AddTag("Patient", DicomTag.PatientModule.PatientID, "Patient ID", "LO");
            AddTag("Patient", DicomTag.PatientModule.PatientBirthDate, "Patient's Birth Date", "DA");
            AddTag("Patient", DicomTag.PatientModule.PatientSex, "Patient's Sex", "CS");
            AddTag("Patient", DicomTag.PatientModule.OtherPatientIDs, "Other Patient IDs", "LO");
            AddTag("Patient", DicomTag.PatientModule.PatientComments, "Patient Comments", "LT");

            // Study Module
            AddTag("Study", DicomTag.StudyModule.StudyInstanceUID, "Study Instance UID", "UI");
            AddTag("Study", DicomTag.StudyModule.StudyDate, "Study Date", "DA");
            AddTag("Study", DicomTag.StudyModule.StudyTime, "Study Time", "TM");
            AddTag("Study", DicomTag.StudyModule.StudyID, "Study ID", "SH");
            AddTag("Study", DicomTag.StudyModule.AccessionNumber, "Accession Number", "SH");
            AddTag("Study", DicomTag.StudyModule.StudyDescription, "Study Description", "LO");
            AddTag("Study", DicomTag.StudyModule.ReferringPhysicianName, "Referring Physician's Name", "PN");

            // Series Module
            AddTag("Series", DicomTag.SeriesModule.SeriesInstanceUID, "Series Instance UID", "UI");
            AddTag("Series", DicomTag.SeriesModule.SeriesNumber, "Series Number", "IS");
            AddTag("Series", DicomTag.SeriesModule.SeriesDate, "Series Date", "DA");
            AddTag("Series", DicomTag.SeriesModule.SeriesTime, "Series Time", "TM");
            AddTag("Series", DicomTag.SeriesModule.SeriesDescription, "Series Description", "LO");
            AddTag("Series", DicomTag.SeriesModule.Modality, "Modality", "CS");

            // Image Module
            AddTag("Image", DicomTag.ImageModule.SOPInstanceUID, "SOP Instance UID", "UI");
            AddTag("Image", DicomTag.ImageModule.InstanceNumber, "Instance Number", "IS");
            AddTag("Image", DicomTag.ImageModule.ContentDate, "Content Date", "DA");
            AddTag("Image", DicomTag.ImageModule.ContentTime, "Content Time", "TM");
            AddTag("Image", DicomTag.ImageModule.AcquisitionDateTime, "Acquisition DateTime", "DT");

            // Equipment Module
            AddTag("Equipment", DicomTag.EquipmentModule.Manufacturer, "Manufacturer", "LO");
            AddTag("Equipment", DicomTag.EquipmentModule.InstitutionName, "Institution Name", "LO");
            AddTag("Equipment", DicomTag.EquipmentModule.StationName, "Station Name", "SH");
            AddTag("Equipment", DicomTag.EquipmentModule.ManufacturerModelName, "Manufacturer's Model Name", "LO");
            AddTag("Equipment", DicomTag.EquipmentModule.SoftwareVersions, "Software Versions", "LO");

            // Setup CollectionViewSource with grouping
            _tagsViewSource = new CollectionViewSource { Source = _allTags };
            _tagsViewSource.GroupDescriptions.Add(new PropertyGroupDescription("Module"));
            _tagsViewSource.SortDescriptions.Add(new SortDescription("Module", ListSortDirection.Ascending));
            _tagsViewSource.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));

            // Notify UI
            OnPropertyChanged(nameof(TagsView));
        }

        private void AddTag(string module, DicomTag tag, string name, string vr)
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
            public DicomTag Tag { get; set; } = null!;
            public string Name { get; set; } = string.Empty;
            public string VR { get; set; } = string.Empty;
            public string TagString { get; set; } = string.Empty;
            public string DisplayText { get; set; } = string.Empty;
        }

        #endregion
    }
}
