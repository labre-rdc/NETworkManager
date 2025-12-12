using System.Collections.Generic;
using System.Linq;

namespace NETworkManager.Models.Firewall;

/// <summary>
/// Represents a security rule used within a firewall to control network traffic based on
/// specific conditions such as IP addresses, ports, and protocols.
/// </summary>
public class FirewallRule
{
    #region Variables

    /// <summary>
    /// Represents the name associated with the object.
    /// </summary>
    /// <remarks>
    /// This property is used to identify the object with a descriptive, human-readable name.
    /// The value of this property is typically a string and can be used for display purposes
    /// or as an identifier in various contexts.
    /// </remarks>
    public string Name { get; set; }

    /// <summary>
    /// Represents a text-based explanation or information associated with an object.
    /// </summary>
    /// <remarks>
    /// This property is used to provide a descriptive message or additional context
    /// about the purpose, behavior, or usage of the associated object. It can be
    /// useful for display, debugging, or logging purposes.
    /// </remarks>
    public string Description { get; set; }

    /// <summary>
    /// Represents the communication protocol to be used in the network configuration.
    /// </summary>
    /// <remarks>
    /// This property identifies the specific protocol for managing network connections.
    /// Possible values may include common protocols such as TCP, UDP, or other supported
    /// protocol types as defined within the relevant enumeration or specification.
    /// </remarks>
    public FirewallProtocol Protocol { get; set; } = FirewallProtocol.TCP;

    /// <summary>
    /// Defines the direction of traffic impacted by the rule or configuration.
    /// </summary>
    /// <remarks>
    /// This property determines whether the traffic flow is inbound, outbound,
    /// or bidirectional based on the application's or system's requirements.
    /// The value is typically selected from a predefined set of directions.
    /// </remarks>
    public FirewallRuleDirection Direction { get; set; } = FirewallRuleDirection.Inbound;

    /// <summary>
    /// Represents the program associated with the firewall rule.
    /// </summary>
    /// <remarks>
    /// This variable defines the application or executable file to which the firewall rule is applied.
    /// It is used to specify program-specific rules to allow or block network traffic based on
    /// the executable file's path. Only valid and existing executable files can be assigned.
    /// </remarks>
    private FirewallRuleProgram _program;

    /// <summary>
    /// Represents the entry point and core execution logic for an application.
    /// </summary>
    /// <remarks>
    /// This property provides access to the main functionality and structural elements
    /// of the program. It typically defines and initializes the components required
    /// to execute the application's logic and serves as the coordinator for the
    /// application's flow.
    /// </remarks>
    public FirewallRuleProgram Program
    {
        get => _program;
        set
        {
            if (value is null)
                return;
            _program = value;
        }
    }

    /// <summary>
    /// Stores the list of local port specifications associated with the firewall rule.
    /// </summary>
    /// <remarks>
    /// This field holds details about the local ports affected by the firewall rule.
    /// It is configured using a collection of <c>FirewallPortSpecification</c> instances.
    /// Changes to this field should align with the protocol settings, as only TCP and UDP protocols allow port specifications.
    /// </remarks>
    private List<FirewallPortSpecification> _localPorts = null;

    /// <summary>
    /// Defines the local ports associated with the firewall rule.
    /// </summary>
    /// <remarks>
    /// This property specifies the local ports that are subject to the firewall rule's conditions.
    /// The value can include single ports, ranges, or a combination of both, depending on
    /// the firewall's configuration and supported formats.
    /// </remarks>
    public List<FirewallPortSpecification> LocalPorts
    {
        get => _localPorts;
        set
        {
            if (value is null)
                return;
            if (Protocol is not FirewallProtocol.TCP and not FirewallProtocol.UDP)
                return;
            _localPorts = value;
        }
    }

    /// <summary>
    /// Represents the remote ports associated with the firewall rule.
    /// </summary>
    /// <remarks>
    /// This field stores a list of port specifications that define the range of remote ports
    /// targeted by the firewall rule. It supports protocols such as TCP and UDP and remains
    /// null if other protocols are selected.
    /// </remarks>
    private List<FirewallPortSpecification> _remotePorts = null;

    /// <summary>
    /// Defines the range of remote ports associated with the firewall rule.
    /// </summary>
    /// <remarks>
    /// This property specifies the ports on the remote system that the firewall rule applies to.
    /// It supports specifying a single port, multiple ports, or a range of ports depending on the configuration.
    /// Proper configuration of remote ports ensures accurate filtering of network traffic.
    /// </remarks>
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

    /// <summary>
    /// Represents the network category settings associated with the firewall rule.
    /// </summary>
    /// <remarks>
    /// This field stores an array of boolean values indicating the applicability of the rule
    /// to specific network categories. Typically, the array is expected to have three elements,
    /// corresponding to the following categories: Domain, Private, and Public networks.
    /// Each index in the array signifies whether the rule applies to the respective category:
    /// - Index 0: Domain network
    /// - Index 1: Private network
    /// - Index 2: Public network
    /// Values outside the expected length of three are ignored during assignment.
    /// </remarks>
    private bool[] _networkCategory;

    /// <summary>
    /// Represents the category of the network associated with a specific configuration or context.
    /// </summary>
    /// <remarks>
    /// This property defines the classification of the network, such as Public, Private, or Domain.
    /// It helps specify the level of trust and access control policies that should be applied to the network.
    /// The value is typically derived from predefined categories aligned with operating system or application-specific conventions.
    /// </remarks>
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

    /// <summary>
    /// Represents the operation to be performed or executed.
    /// </summary>
    /// <remarks>
    /// This property defines the specific action that should occur when invoked.
    /// It is typically used to encapsulate a method call, delegate, or set of
    /// operations that can be executed at runtime.
    /// </remarks>
    public FirewallRuleAction Action { get; set; } = FirewallRuleAction.Block;
    
    #endregion
    
    #region Constructors

    /// <summary>
    /// Represents a rule within a firewall configuration.
    /// Used to control network traffic based on specified criteria, such as
    /// IP addresses, ports, and protocols.
    /// </summary>
    public FirewallRule()
    {
        
    }

    /// <summary>
    /// Represents a firewall rule that defines specific criteria for allowing
    /// or denying network traffic based on certain conditions such as IP address,
    /// port, or protocol.
    /// </summary>
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

    /// <summary>
    /// Converts a collection of port numbers to a single, comma-separated string representation.
    /// </summary>
    /// <param name="ports">A collection of integers representing port numbers.</param>
    /// <returns>A comma-separated string containing all the port numbers from the input collection.</returns>
    public static string PortsToString(List<FirewallPortSpecification> ports)
    {
        var result = ports.Aggregate(string.Empty, (current, port) => current + $"{port}; ");
        return result.Remove(result.Length - 2);
    }
    
    #endregion
}