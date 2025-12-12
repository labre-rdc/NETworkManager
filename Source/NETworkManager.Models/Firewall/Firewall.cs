using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NETworkManager.Models.Firewall;

public class Firewall(string profile)
{
    #region Variables

    private string _command = "";
    public string Profile { get; set; } = profile;

    #endregion
    
    #region Events

    public event EventHandler UserHasCanceled;

    private void OnUserHasCanceled()
    {
        UserHasCanceled?.Invoke(this, EventArgs.Empty);
    }

    #endregion

    
    #region Methods

    private void CommandDeleteRule(FirewallRule rule)
    {
        
    }
    public bool DeleteRule(FirewallRule rule)
    {
        return true;
    }

    public bool DeleteRules(List<FirewallRule> rules)
    {
        return true;
    }

    private bool CommandAddRule(FirewallRule rule)
    {
        return true;
    }
    
    public bool AddRule(FirewallRule rule)
    {
        return true;
    }

    public bool AddRules(List<FirewallRule> rules)
    {
        return true;
    }

    private bool CommandUpdateRule(FirewallRule rule)
    {
        return true;
    }
    
    public bool UpdateRule(FirewallRule rule)
    {
        return true;
    }

    public bool UpdateRules(List<FirewallRule> rules)
    {
        return true;
    }

    public async Task<bool> UpdateRulesAsync(List<FirewallRule> rules)
    {
        return await Task.Run(() => UpdateRules(rules));
    }
    
    private bool ExecuteCommand(string command)
    {
        if (profile is "Unknown")
            return false;
        return true;
    }

    #endregion
}