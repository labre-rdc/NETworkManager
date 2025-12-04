using System.Windows;
using System;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using MahApps.Metro.Controls.Dialogs;
using NETworkManager.Controls;
using NETworkManager.Models.Network;
using NETworkManager.Utilities;
using NETworkManager.ViewModels;
using VisualTreeHelper = System.Windows.Media.VisualTreeHelper;

namespace NETworkManager.Views;

public partial class FirewallView
{
    private readonly FirewallViewModel _viewModel;
    
    public FirewallView()
    {
        InitializeComponent();

        _viewModel = new FirewallViewModel(DialogCoordinator.Instance);
        DataContext = _viewModel;

        // TODO: Required when searching for apps.
        //Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
    }
    
    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        return;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <remarks>Required when searching for apps.</remarks>
    private void Dispatcher_ShutdownStarted(object sender, EventArgs e)
    {
        _viewModel.OnClose();
    }

    public void OnViewHide()
    {
        _viewModel.OnViewHide();
    }

    public void OnViewVisible()
    {
        _viewModel.OnViewVisible();
    }

}