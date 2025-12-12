namespace NETworkManager.Models.Firewall;

/// <summary>
/// Represents a firewall rule direction that allows or processes network traffic
/// incoming to the system or network from external sources.
/// </summary>
public enum FirewallRuleDirection
{
    /// <summary
    Inbound,

    /// <summary>
    /// Represents a firewall rule direction where network traffic is initiated
    /// from the local system to an external endpoint or remote destination.
    /// This
    Outbound
}