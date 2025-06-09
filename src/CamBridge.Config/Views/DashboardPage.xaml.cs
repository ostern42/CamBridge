// src/CamBridge.Config/Views/DashboardPage.xaml.cs
// Version: 0.6.8
// Copyright: © 2025 Claude's Improbably Reliable Software Solutions

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using CamBridge.Config.ViewModels;

namespace CamBridge.Config.Views
{
    /// <summary>
    /// Interaction logic for DashboardPage.xaml - Multi-Pipeline Version
    /// </summary>
    public partial class DashboardPage : Page
    {
        private DashboardViewModel? _viewModel;

        public DashboardPage()
        {
            InitializeComponent();

            // Debug output to confirm correct version
            Debug.WriteLine("=== MULTI-PIPELINE DASHBOARD CONSTRUCTOR ===");
            Debug.WriteLine($"XAML loaded at: {DateTime.Now:HH:mm:ss.fff}");

            DataContextChanged += OnDataContextChanged;
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _viewModel = DataContext as DashboardViewModel;

            if (_viewModel != null)
            {
                Debug.WriteLine($"Dashboard ViewModel set - Pipelines: {_viewModel.PipelineStatuses?.Count ?? 0}");

                // SAFE CALL with null checks!
                if (_viewModel.RefreshDataCommand != null && _viewModel.RefreshDataCommand.CanExecute(null))
                {
                    Debug.WriteLine("Executing initial refresh...");
                    _ = _viewModel.RefreshDataCommand.ExecuteAsync(null);
                }
                else
                {
                    Debug.WriteLine("WARNING: RefreshDataCommand is null or cannot execute!");
                }
            }
            else
            {
                Debug.WriteLine("WARNING: DataContext is not a DashboardViewModel!");
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine($"Dashboard Page Loaded - ViewModel present: {_viewModel != null}");
            Debug.WriteLine($"Pipeline count: {_viewModel?.PipelineStatuses?.Count ?? 0}");

            // Double-check we have the right dashboard
            if (_viewModel?.PipelineStatuses != null)
            {
                Debug.WriteLine("✓ Multi-Pipeline Dashboard confirmed!");
            }
            else
            {
                Debug.WriteLine("⚠ WARNING: No PipelineStatuses collection!");
            }
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            // Clean up timer when page is unloaded
            _viewModel?.Cleanup();
            Debug.WriteLine("Dashboard Page Unloaded");
        }
    }
}
