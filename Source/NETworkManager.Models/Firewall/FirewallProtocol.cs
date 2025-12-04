using System.ServiceModel.Channels;

namespace NETworkManager.Models.Firewall;

public enum FirewallProtocol
{
    TCP = 6,
    UDP = 17,
    ICMP = 1,
    ICMPv6 = 58,
    HOPOPT = 0,
    GRE = 47,
    IPv6 = 41,
    IPv6_Route = 43,
    IPv6_Frag = 44,
    IPv6_NoNxt = 59,
    IPv6_Opts = 60,
    VRRP = 112,
    PGM = 113,
    L2TP = 115,
    Any = 255
}