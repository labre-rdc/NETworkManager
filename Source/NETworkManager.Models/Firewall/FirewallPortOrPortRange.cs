namespace NETworkManager.Models.Firewall;

public class FirewallPortOrPortRange
{
    public int Start { get; private set; }
    public int End { get; private set; } = -1;

    public FirewallPortOrPortRange(int start, int end = -1)
    {
        Start = start;
        End = end;
    }
}