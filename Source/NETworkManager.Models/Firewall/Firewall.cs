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

    public string RuleToName(FirewallRule rule)
    {
        if (rule is null)
            return string.Empty;
        StringBuilder resultBuilder = new();
        resultBuilder.Append("NwM");
        resultBuilder.Append($"_{Profile ?? "Unknown"}");
        var direction = rule.Direction switch
        {
            FirewallRuleDirection.Inbound => "in",
            FirewallRuleDirection.Outbound => "out",
            _ => null
        };
        if (direction is not null)
            resultBuilder.Append($"_{direction}");
        if (rule.LocalPorts?.Count > 0)
            resultBuilder.Append($"_{FirewallRule.PortsToString(rule.LocalPorts)}");
        if (rule.RemotePorts?.Count > 0)
            resultBuilder.Append($"_{FirewallRule.PortsToString((rule.RemotePorts))}");
        if (rule.Program is not null && !string.IsNullOrEmpty(rule.Program.Executable?.Name))
            resultBuilder.Append($"_{rule.Program.Executable.Name}");
        return resultBuilder.ToString();
    }

    private bool ExecuteCommand(string command)
    {
        return true;
    }

    #endregion
}