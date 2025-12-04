using System.Collections.Generic;

namespace NETworkManager.Models.Firewall;

/// <summary>
/// 
/// </summary>
/// <remarks>
/// TODO: Create list of installed programs (64 and 32 Bit)
/// https://stackoverflow.com/a/37313990
/// </remarks>
public class ProgramList
{
    public List<FirewallRuleProgram> Programs { get; private set; }

    public ProgramList()
    {
        Programs = [];
    }
}