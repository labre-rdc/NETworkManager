using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MahApps.Metro.Controls.Dialogs;
using NETworkManager.ViewModels;

namespace NETworkManager.Views;

public partial class FirewallView
{
    private readonly FirewallViewModel _viewModel;
    
    public FirewallView()
    {
        InitializeComponent();

        _viewModel = new FirewallViewModel(DialogCoordinator.Instance);
        DataContext = _viewModel;
    }
    
    #region Events
    private void ContextMenu_Opened(object sender, RoutedEventArgs e)
    {
        if (sender is ContextMenu menu)
            menu.DataContext = _viewModel;
    }

    public void OnViewHide()
    {
        _viewModel.OnViewHide();
    }

    public void OnViewVisible()
    {
        _viewModel.OnViewVisible();
    }
    
    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        // Get the row from the sender
        for (var visual = sender as Visual; visual != null; visual = VisualTreeHelper.GetParent(visual) as Visual)
        {
            if (visual is not DataGridRow row)
                continue;

            row.DetailsVisibility =
                row.DetailsVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;

            break;
        }
    }
    
    #endregion
    
}