using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using log4net;
using NETworkManager.Utilities;

namespace NETworkManager.Models.Firewall;

/// <summary>
/// Represents a firewall configuration and management class that provides functionalities
/// for applying and managing firewall rules based on a specified profile.
/// </summary>
public class Firewall(string profile)
{
    #region Variables

    /// <summary>
    /// Represents a logging utility used to capture and write log messages
    /// for operations within the Firewall class.
    /// </summary>
    /// <remarks>
    /// The logger is utilized to provide debugging, error reporting, and
    /// informational messages throughout the execution of firewall rule
    /// management operations. It helps in tracking system behavior and troubleshooting issues.
    /// </remarks>
    private readonly ILog Logger = LogManager.GetLogger(typeof(Firewall));

    /// <summary>
    /// Stores the command string used to configure firewall rules.
    /// This variable is utilized to construct PowerShell commands
    /// for creating, modifying, or deleting firewall rules during
    /// the rule application process.
    /// </summary>
    private string _command = "";

    /// <summary>
    /// Represents a user profile containing personal and account-related information.
    /// This property is typically used to store and retrieve details about a user in the system.
    /// </summary>
    public string Profile { get; set; } = profile;

    #endregion
    
    #region Events

    /// <summary>
    /// An event triggered when the user cancels an ongoing operation in the firewall configuration process.
    /// </summary>
    /// <remarks>
    /// This event is used to notify subscribers that the current firewall rule application or configuration has been
    /// canceled by the user. The event allows for cleanup operations and UI updates in response to a cancellation.
    /// </remarks>
    public event EventHandler UserHasCanceled;

    /// <summary>
    /// Raises the <c>UserHasCanceled</c> event to notify subscribers that the user has performed a cancellation action.
    /// This method is typically used internally within the <c>Firewall</c> class to signal the cancellation event.
    /// It invokes the event handlers associated with the <c>UserHasCanceled</c> event if there are any subscribers.
    /// </summary>
    private void OnUserHasCanceled()
    {
        UserHasCanceled?.Invoke(this, EventArgs.Empty);
    }

    #endregion

    
    #region Methods

    /// <summary>
    /// Applies the firewall rules specified in the list to the system's firewall settings.
    /// </summary>
    /// <param name="rules">A list of <see cref="FirewallRule"/> objects representing the firewall rules to be applied.</param>
    /// <returns>Returns <c>true</c> if the rules were successfully applied; otherwise, <c>false</c>.</returns>
    public bool ApplyRules(List<FirewallRule> rules)
    {
        // TODO: Implementation
        // Add and remove rule for now as PoC
        List<FirewallPortSpecification> localPorts = new();
        localPorts.Add(new FirewallPortSpecification(443));
        FirewallRule demo = new FirewallRule()
        {
            Name = "NwM_Demo Rule_Default",
            NetworkCategory = [true, false ,false],
            Action = FirewallRuleAction.Block,
            Description = "Dummy description",
            LocalPorts = localPorts,
            Direction = FirewallRuleDirection.Inbound
        };
        string command = $"New-NetFirewallRule -DisplayName '{demo.Name}'";
        command += $" -Direction {Enum.GetName(typeof(FirewallRuleDirection), demo.Direction)}";
        if (localPorts.Count > 0)
            command += $" -LocalPort {FirewallRule.PortsToString(localPorts)}";
        // TODO: remotePorts
        command += $" -Protocol {Enum.GetName(typeof(FirewallProtocol), demo.Protocol)}";
        command += $" -Action {Enum.GetName(typeof(FirewallRuleAction), demo.Action)}";
        try
        {
            PowerShellHelper.ExecuteCommand(command, true);
        }
        catch (Exception)
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// Removes all existing firewall rules associated with the specified profile.
    /// </summary>
    /// <remarks>
    /// This method clears all configured firewall rules for the current profile.
    /// It is useful for resetting the rules to a clean state. Errors or exceptions
    /// during the operation are logged using the configured logging mechanism.
    /// </remarks>
    private void ClearRulesForProfile()
    {
        Logger.Debug($"Clearing rules for profile {Profile}");
        // TODO: Implementation
        
    }

    /// <summary>
    /// Applies firewall rules asynchronously by invoking the ApplyRules method in a separate task.
    /// </summary>
    /// <param name="rules">A list of firewall rules to apply.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a boolean indicating whether the rules were successfully applied.</returns>
    public async Task<bool> ApplyRulesAsync(List<FirewallRule> rules)
    {
        return await Task.Run(() => ApplyRules(rules));
    }

    /// <summary>
    /// Executes a specified firewall command.
    /// </summary>
    /// <param name="command">The command string to be executed.</param>
    /// <returns>
    /// A boolean value indicating whether the command execution was successful.
    /// Returns <c>true</c> if the command operation succeeded; otherwise, <c>false</c>.
    /// </returns>
    private bool ExecuteCommand(string command)
    {
        if (profile is "Unknown")
            return false;
        return true;
    }

    #endregion
}