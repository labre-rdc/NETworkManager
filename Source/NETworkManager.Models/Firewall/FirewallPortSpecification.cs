namespace NETworkManager.Models.Firewall;

public class FirewallPortSpecification
{
    public int Start { get; private set; }
    public int End { get; private set; } = -1;

    public FirewallPortSpecification(int start, int end = -1)
    {
        Start = start;
        End = end;
    }

    public override string ToString()
    {
        if (Start is 0)
            return string.Empty;
        return End is -1 or 0 ? $"{Start}" : $"{Start}-{End}";
    }
}