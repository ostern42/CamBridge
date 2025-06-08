// src/CamBridge.Config/Views/DashboardPage.xaml.cs
// Version: 0.6.8
// Copyright: © 2025 Claude's Improbably Reliable Software Solutions

using System;
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
            System.Diagnostics.Debug.WriteLine("=== MULTI-PIPELINE DASHBOARD CONSTRUCTOR ===");
            System.Diagnostics.Debug.WriteLine($"XAML loaded at: {DateTime.Now:HH:mm:ss.fff}");

            DataContextChanged += OnDataContextChanged;
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _viewModel = DataContext as DashboardViewModel;

            if (_viewModel != null)
            {
                System.Diagnostics.Debug.WriteLine($"Dashboard ViewModel set - Pipelines: {_viewModel.PipelineStatuses?.Count ?? 0}");

                // Force immediate refresh when ViewModel is set
                _ = _viewModel.RefreshDataCommand.ExecuteAsync(null);
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"Dashboard Page Loaded - ViewModel present: {_viewModel != null}");
            System.Diagnostics.Debug.WriteLine($"Pipeline count: {_viewModel?.PipelineStatuses?.Count ?? 0}");

            // Double-check we have the right dashboard
            if (_viewModel?.PipelineStatuses != null)
            {
                System.Diagnostics.Debug.WriteLine("✓ Multi-Pipeline Dashboard confirmed!");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("⚠ WARNING: No PipelineStatuses collection!");
            }
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            // Clean up timer when page is unloaded
            _viewModel?.Cleanup();
            System.Diagnostics.Debug.WriteLine("Dashboard Page Unloaded");
        }
    }
}
