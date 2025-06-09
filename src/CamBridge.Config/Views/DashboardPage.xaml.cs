// src\CamBridge.Config\Views\DashboardPage.xaml.cs
// Version: 0.6.11
// Description: Dashboard page code-behind with simplified initialization
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

            Debug.WriteLine("=== DASHBOARD PAGE CONSTRUCTOR ===");

            DataContextChanged += OnDataContextChanged;
            Unloaded += OnUnloaded;
        }

        private async void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _viewModel = DataContext as DashboardViewModel;

            if (_viewModel != null)
            {
                Debug.WriteLine($"Dashboard ViewModel set - Pipelines: {_viewModel.PipelineStatuses?.Count ?? 0}");

                // Initialize the ViewModel ONCE
                await _viewModel.InitializeAsync();
            }
            else if (DataContext != null)
            {
                Debug.WriteLine($"WARNING: DataContext is {DataContext.GetType().Name}, not DashboardViewModel!");
            }
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            // Clean up timer when page is unloaded
            _viewModel?.Cleanup();
            Debug.WriteLine("Dashboard Page Unloaded - Cleanup done");
        }
    }
}
