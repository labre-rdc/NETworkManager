namespace NETworkManager.Models.Firewall;

/// <summary>
/// Represents the actions available for a firewall rule.
/// </summary>
/// <remarks>
/// FirewallRuleAction determines whether a specific network traffic is permitted or blocked.
/// This enum is used in defining firewall rules, impacting how traffic is managed based on those rules.
/// </remarks>
public enum FirewallRuleAction
{
    /// <summary>
    /// Represents the action to block network traffic in a firewall rule.
    /// This action denies the specified traffic explicitly, ensuring it is not allowed to pass through the network.
    /// </summary>
    Block,

    /// <summary>
    /// Represents the action to allow network traffic.
    /// When this value is selected, the firewall rule permits
    /// the specified network traffic to pass through without restriction.
    /// </summary>
    Allow,
    // Unsupported for now
    //AllowIPsec
}