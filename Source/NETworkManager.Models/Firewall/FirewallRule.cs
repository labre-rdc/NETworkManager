using System.Collections.Generic;
using System.Linq;
using System.Text;
using NETworkManager.Models.Network;

namespace NETworkManager.Models.Firewall;

public class FirewallRule
{
    #region Variables
    public string Name { get; set; }
    
    public string UserDefinedName { get; set; }
    
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

    private List<FirewallPortSpecification> _localPorts = null;
    
    public List<FirewallPortSpecification> LocalPorts
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

    private List<FirewallPortSpecification> _remotePorts = null;

    public List<FirewallPortSpecification> RemotePorts
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

    private bool[] _networkCategory;

    public bool[] NetworkCategory
    {
        get => _networkCategory;
        set
        {
            if (value?.Length is not 3)
                return;

            _networkCategory = value;
        }
    }
    
    #endregion
    
    #region Constructors

    public FirewallRule()
    {
        
    }
    
    public FirewallRule(FirewallProtocol protocol, FirewallRuleDirection direction, FirewallRuleProgram program,
        List<FirewallPortSpecification> localPorts, List<FirewallPortSpecification> remotePorts,
        bool[] networkCategory)
    {
        Protocol = protocol;
        Direction = direction;
        Program = program;
        LocalPorts = localPorts;
        RemotePorts = remotePorts;
        NetworkCategory = networkCategory;
    }
    
    #endregion
    
    #region Methods

    public static string PortsToString(List<FirewallPortSpecification> ports)
    {
        var result = ports.Aggregate(string.Empty, (current, port) => current + $"{port}; ");
        return result.Remove(result.Length - 2);
    }
    
    #endregion
}