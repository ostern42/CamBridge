// src/CamBridge.Config/Views/MappingEditorPage.xaml.cs
using System;
using System.Runtime.Versioning;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CamBridge.Config.ViewModels;
using CamBridge.Core;
using CamBridge.Core.ValueObjects;
using Microsoft.Extensions.DependencyInjection;

namespace CamBridge.Config.Views
{
    /// <summary>
    /// Interaction logic for MappingEditorPage.xaml with drag & drop support
    /// </summary>
    [SupportedOSPlatform("windows")]
    public partial class MappingEditorPage : Page
    {
        private MappingEditorViewModel? _viewModel;
        private Point _startPoint;
        private bool _isDragging;

        public MappingEditorPage()
        {
            InitializeComponent();
            Loaded += MappingEditorPage_Loaded;
        }

        private void MappingEditorPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (_viewModel != null) return;

            try
            {
                var app = Application.Current as App;
                if (app?.Host != null)
                {
                    _viewModel = app.Host.Services.GetRequiredService<MappingEditorViewModel>();
                    DataContext = _viewModel;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading MappingEditorViewModel: {ex.Message}");
                ShowErrorMessage($"Failed to load mapping editor: {ex.Message}");
            }
        }

        #region Drag & Drop

        private void SourceField_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                _isDragging = false;
                return;
            }

            var border = sender as Border;
            if (border?.Tag is MappingEditorViewModel.SourceFieldInfo fieldInfo)
            {
                Point currentPosition = e.GetPosition(null);
                Vector diff = _startPoint - currentPosition;

                if (!_isDragging &&
                    (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                     Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
                {
                    _isDragging = true;

                    // Create data for drag operation
                    var dragData = new DataObject();
                    dragData.SetData("SourceField", fieldInfo);

                    // Start drag operation
                    DragDrop.DoDragDrop(border, dragData, DragDropEffects.Copy);
                    _isDragging = false;
                }
            }
        }

        private void MappingArea_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("SourceField"))
            {
                e.Effects = DragDropEffects.Copy;
                e.Handled = true;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void MappingArea_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("SourceField") && _viewModel != null)
            {
                var fieldInfo = e.Data.GetData("SourceField") as MappingEditorViewModel.SourceFieldInfo;
                if (fieldInfo != null)
                {
                    CreateMappingFromDrop(fieldInfo);
                }
            }
        }

        private void CreateMappingFromDrop(MappingEditorViewModel.SourceFieldInfo fieldInfo)
        {
            if (_viewModel == null) return;

            // Determine source type
            string sourceType = _viewModel.QRBridgeFields.Contains(fieldInfo) ? "QRBridge" : "EXIF";

            // Suggest appropriate DICOM tag based on field
            var suggestedTag = SuggestDicomTag(fieldInfo.FieldName, sourceType);
            var suggestedTransform = SuggestTransform(fieldInfo.FieldName, fieldInfo.DataType);

            // Create new mapping rule
            var newRule = new MappingRule(
                $"{fieldInfo.DisplayName}_Mapping",
                sourceType,
                fieldInfo.FieldName,
                suggestedTag,
                suggestedTransform
            );

            var ruleVm = new MappingRuleViewModel(newRule);
            ruleVm.PropertyChanged += (s, e) =>
            {
                _viewModel.IsModified = true;
                if (s == _viewModel.SelectedRule)
                    _viewModel.UpdatePreview();
            };

            _viewModel.MappingRules.Add(ruleVm);
            _viewModel.SelectedRule = ruleVm;
            _viewModel.IsModified = true;
        }

        private DicomTag SuggestDicomTag(string fieldName, string sourceType)
        {
            // Smart suggestions based on field name
            return fieldName.ToLower() switch
            {
                "name" or "patientname" => DicomTag.PatientModule.PatientName,
                "patientid" or "id" => DicomTag.PatientModule.PatientID,
                "birthdate" or "dob" => DicomTag.PatientModule.PatientBirthDate,
                "gender" or "sex" => DicomTag.PatientModule.PatientSex,
                "examid" or "studyid" => DicomTag.StudyModule.StudyID,
                "comment" or "description" => DicomTag.StudyModule.StudyDescription,
                "make" or "manufacturer" => DicomTag.EquipmentModule.Manufacturer,
                "model" => DicomTag.EquipmentModule.ManufacturerModelName,
                "software" => DicomTag.EquipmentModule.SoftwareVersions,
                "datetimeoriginal" => DicomTag.ImageModule.AcquisitionDateTime,
                _ => DicomTag.StudyModule.StudyDescription // Default
            };
        }

        private ValueTransform SuggestTransform(string fieldName, string dataType)
        {
            // Smart transform suggestions
            if (fieldName.ToLower().Contains("date") && dataType == "date")
                return ValueTransform.DateToDicom;

            if (fieldName.ToLower().Contains("gender") || fieldName.ToLower().Contains("sex"))
                return ValueTransform.GenderToDicom;

            if (fieldName.ToLower() == "examid")
                return ValueTransform.TruncateTo16;

            return ValueTransform.None;
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            _startPoint = e.GetPosition(null);
            base.OnPreviewMouseLeftButtonDown(e);
        }

        #endregion

        private void ShowErrorMessage(string message)
        {
            var errorText = new TextBlock
            {
                Text = message,
                Margin = new Thickness(20),
                FontSize = 16,
                Foreground = System.Windows.Media.Brushes.Red
            };

            Content = new Grid { Children = { errorText } };
        }
    }
}
