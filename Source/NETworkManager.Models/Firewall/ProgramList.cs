using System.Collections.Generic;

namespace NETworkManager.Models.Firewall;

/// <summary>
/// Represents a list of programs relevant to firewall rules.
/// </summary>
/// <remarks>
/// This class is intended to hold and manage a collection of programs
/// associated with firewall rules. It facilitates the organization and
/// access of these programs for firewall management purposes.
/// </remarks>
public class ProgramList
{
    /// <summary>
    /// A list property that holds all the programs associated with firewall rules.
    /// </summary>
    /// <remarks>
    /// This property is used to manage and store instances of <see cref="FirewallRuleProgram"/>,
    /// which represent programs tied to specific firewall rules. Each program includes details
    /// about its executable file and directory.
    /// </remarks>
    public List<FirewallRuleProgram> Programs { get; private set; }

    /// <summary>
    /// Represents a collection of programs associated with firewall rules.
    /// </summary>
    /// <remarks>
    /// This class is designed to store and manage a list of programs that interact
    /// with firewall rules. It encapsulates a list of <see cref="FirewallRuleProgram"/>
    /// objects, providing functionality to manage and organize program details.
    /// </remarks>
    public ProgramList()
    {
        Programs = [];
    }
}