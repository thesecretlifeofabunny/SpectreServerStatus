using System.Net.NetworkInformation;

namespace SpectreServerStatus;

public static class PingService
{
    private const int DefaultPingTimeout = 120;
    public static PingReply? PingServer(string serverToPing)
    {
        Ping pingSender = new();

        return pingSender.Send(serverToPing, DefaultPingTimeout);
    }
}