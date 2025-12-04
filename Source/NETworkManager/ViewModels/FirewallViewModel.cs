namespace NETworkManager.ViewModels;

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using log4net;
using NETworkManager.Models.Network;
using NETworkManager.Settings;
using ConfigurationManager = System.Configuration.ConfigurationManager;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;
using NETworkManager.Localization.Resources;
using NETworkManager.Models.Firewall;
using NETworkManager.Utilities;

public class FirewallViewModel : ViewModelBase
{
    
        #region Variables
    private static readonly ILog Log = LogManager.GetLogger(typeof(FirewallViewModel));

    private readonly IDialogCoordinator _dialogCoordinator;

    private bool _firstLoad = true;
    private bool _closed;

    private NetworkConnectionProfile _networkProfileConfig;
    
    public NetworkConnectionProfile NetworkProfileConfig
    {
        get => _networkProfileConfig;
        set
        {
            if (_networkProfileConfig is NetworkConnectionProfile.DomainAuthenticated)
                return;
            if (value is NetworkConnectionProfile.DomainAuthenticated)
                return;
            _networkProfileConfig = value;
        }
    }
    
    private ObservableCollection<FirewallRule> _rules = [];

    public ObservableCollection<FirewallRule> Rules
    {
        get => _rules;
        set
        {
            if (Equals(value, _rules))
                return;

            _rules = value;
        }
    }

    public ICollectionView RulesView { get; }

    private FirewallRule _selectedRule;

    public FirewallRule SelectedRule
    {
        get => _selectedRule;
        set
        {
            if (value ==  _selectedRule)
                return;

             _selectedRule = value;
            OnPropertyChanged();
        }
    }

    private IList _selectedRules = new ArrayList();

    public IList SelectedResults
    {
        get => _selectedRules;
        set
        {
            if (Equals(value, _selectedRules))
                return;

            _selectedRules = value;
            OnPropertyChanged();
        }
    }

    private bool _isStatusMessageDisplayed;

    public bool IsStatusMessageDisplayed
    {
        get => _isStatusMessageDisplayed;
        set
        {
            if (value == _isStatusMessageDisplayed)
                return;

            _isStatusMessageDisplayed = value;
            OnPropertyChanged();
        }
    }

    private string _statusMessage;

    public string StatusMessage
    {
        get => _statusMessage;
        private set
        {
            if (value == _statusMessage)
                return;

            _statusMessage = value;
            OnPropertyChanged();
        }
    }
    
    #endregion

    
    public FirewallViewModel(IDialogCoordinator instance)
    {
        _dialogCoordinator = instance;
    }

    public void OnClose()
    {
        // Prevent multiple calls
        if (_closed)
            return;

        _closed = true;
    }

    public void OnViewHide()
    {
        
    }

    public void OnViewVisible()
    {
        
    }
}