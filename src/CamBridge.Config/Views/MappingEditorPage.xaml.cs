// src/CamBridge.Config/Views/MappingEditorPage.xaml.cs
using CamBridge.Config.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CamBridge.Config.Views
{
    /// <summary>
    /// Interaction logic for MappingEditorPage.xaml
    /// </summary>
    public partial class MappingEditorPage : Page
    {
        public MappingEditorPage()
        {
            InitializeComponent();

            // TODO: This should come from DI container in production
            // For now, create ViewModel manually for testing
            var logger = Microsoft.Extensions.Logging.Abstractions.NullLogger<MappingEditorViewModel>.Instance;

            // Mock IConfigurationService for testing
            var configService = new MockConfigurationService();

            DataContext = new MappingEditorViewModel(logger, configService);
        }

        /// <summary>
        /// Handles the start of a drag operation for source fields
        /// </summary>
        private void SourceField_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && sender is Border border)
            {
                var sourceField = border.Tag as MappingEditorViewModel.SourceFieldInfo;
                if (sourceField != null)
                {
                    // Create drag data
                    var dragData = new DataObject();
                    dragData.SetData("SourceFieldInfo", sourceField);

                    // Start the drag operation
                    DragDrop.DoDragDrop(border, dragData, DragDropEffects.Copy);
                }
            }
        }

        /// <summary>
        /// Handles drag over for the mapping area
        /// </summary>
        private void MappingArea_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("SourceFieldInfo"))
            {
                e.Effects = DragDropEffects.Copy;
                e.Handled = true;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        /// <summary>
        /// Handles drop in the mapping area
        /// </summary>
        private void MappingArea_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("SourceFieldInfo"))
            {
                var sourceField = e.Data.GetData("SourceFieldInfo") as MappingEditorViewModel.SourceFieldInfo;
                if (sourceField != null && DataContext is MappingEditorViewModel viewModel)
                {
                    // Create a new rule based on the dropped field
                    viewModel.AddRuleCommand.Execute(null);

                    // Configure the newly created rule with the dropped field data
                    if (viewModel.SelectedRule != null)
                    {
                        // Determine source type based on which collection the field belongs to
                        var sourceType = viewModel.QRBridgeFields.Contains(sourceField) ? "QRBridge" : "EXIF";

                        viewModel.SelectedRule.SourceType = sourceType;
                        viewModel.SelectedRule.SourceField = sourceField.FieldName;
                        viewModel.SelectedRule.Name = $"{sourceField.DisplayName} Mapping";
                    }
                }

                e.Handled = true;
            }
        }
    }
}
