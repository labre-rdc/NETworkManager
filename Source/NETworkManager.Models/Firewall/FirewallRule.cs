using System.Collections.Generic;

namespace NETworkManager.Models.Firewall;

public class FirewallRule
{
    #region Variables

    /// <summary>
    /// Specifies the network protocol associated with the firewall rule.
    /// </summary>
    /// <remarks>
    /// This property defines the protocol type for traffic affected by the firewall rule.
    /// The protocol can be set to specific values such as TCP, UDP, ICMP, or Any, among others,
    /// based on the predefined options in the <c>FirewallProtocol</c> enumeration.
    /// </remarks>
    public FirewallProtocol Protocol { get; private set; } = FirewallProtocol.Any;
    
    /// <summary>
    /// Represents the direction of network traffic in a firewall rule.
    /// </summary>
    /// <remarks>
    /// This property determines whether the rule applies to inbound or outbound traffic,
    /// or if it is currently set to none.
    /// </remarks>
    public FirewallRuleDirection Direction { get; private set; } = FirewallRuleDirection.Inbound;

    private FirewallRuleProgram _program;

    public FirewallRuleProgram Program
    {
        get => _program;
        private set
        {
            if (value is null)
                return;
            _program = value;
        }
    }

    private List<FirewallPortOrPortRange> _localPorts = null;
    
    public List<FirewallPortOrPortRange> LocalPorts
    {
        get => _localPorts;
        private set
        {
            if (value is null)
                return;
            if (Protocol is not FirewallProtocol.TCP and not FirewallProtocol.UDP)
                return;
            _localPorts = value;
        }
    }

    private List<FirewallPortOrPortRange> _remotePorts = null;

    public List<FirewallPortOrPortRange> RemotePorts
    {
        get => _remotePorts;
        set
        {
            if (value is null)
                return;
            if (Protocol is not FirewallProtocol.TCP and not FirewallProtocol.UDP)
                return;
            _remotePorts = value;
        }
    }
    
    #endregion
    
    #region Constructors

    public FirewallRule(FirewallProtocol protocol, FirewallRuleDirection direction, FirewallRuleProgram program,
        List<FirewallPortOrPortRange> localPorts, List<FirewallPortOrPortRange> remotePorts)
    {
        Protocol = protocol;
        Direction = direction;
        Program = program;
        LocalPorts = localPorts;
        RemotePorts = remotePorts;
    }
    
    #endregion
}