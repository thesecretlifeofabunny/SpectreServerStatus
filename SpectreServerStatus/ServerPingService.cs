using System.Globalization;
using System.Net.NetworkInformation;
using Spectre.Console;

namespace SpectreServerStatus;

public class ServerPingService(string serverToPing)
{
    private string ServerToPing { get; init; } = serverToPing;
    private PingReply? LatestPingReply { get; set; }
    private string PingReplyIpAddress { get; set; } = string.Empty;
    private string PingReplyStatus { get; set; } = string.Empty;
    private string PingReplyRoundTripTime { get; set; } = string.Empty;
    private double PingCount { get; set; }
    private double PingFailureCount { get; set; }

    private double PingSuccessPercentage => (PingCount - PingFailureCount) / PingCount * 100;
    private const int DefaultPingTimeout = 120;

    /// <summary>
    /// Takes the relevant information of the PingReply and returns the array of information
    /// </summary>
    /// <returns>In order of [ServerToPing, PingReplyIpAddress, PingReplyStatus PingReplyRoundTripTime,
    /// PingCount, PingFailureCount, PingSuccessPercentage]</returns>
    public string[] ArrayOfLatestPingInformation()
    {
        return
        [
            ServerToPing,
            PingReplyIpAddress,
            PingReplyStatus,
            PingReplyRoundTripTime,
            PingCount.ToString(CultureInfo.InvariantCulture),
            PingFailureCount.ToString(CultureInfo.InvariantCulture),
            PingSuccessPercentage.ToString(CultureInfo.InvariantCulture)
        ];
    }
    /// <summary>
    /// Pings the ServerToPing attribute and updates the other attributes
    /// </summary>
    public void PingServer()
    {
        Ping pingSender = new();

        LatestPingReply = pingSender.Send(ServerToPing, DefaultPingTimeout);

        UpdatePingReplyInformation();
        UpdatePingCounts();
    }

    /// <summary>
    /// Builds an ordered table that PingService expects for proper user of ArrayOfLatestPingInformation()
    /// </summary>
    /// <returns> A Spectre.Console.Table object with columns pre allocated, and no rows initialized</returns>
    public static Table TableBuilder()
    {
        var table = new Table();
        table.Caption($"Refreshing every five seconds");
        table.AddColumn("Server to Ping");
        table.AddColumn("IP Address");
        table.AddColumn("Ping Status");
        table.AddColumn("RoundTrip Time");
        table.AddColumn("Total Number of times pinged");
        table.AddColumn("Total Number of failed pings");
        table.AddColumn("Percentage of Success");

        return table;
    }

    private void UpdatePingCounts()
    {
        PingCount++;

        if (LatestPingReply is null || LatestPingReply.Status != IPStatus.Success)
        {
            PingFailureCount++;
        }
    }

    private void UpdatePingReplyInformation()
    {
        PingReplyIpAddress = LatestPingReply is not null ? LatestPingReply.Address.ToString() : string.Empty;
        PingReplyStatus = LatestPingReply is not null ? LatestPingReply.Status.ToString() : string.Empty;
        PingReplyRoundTripTime = LatestPingReply is not null ? LatestPingReply.RoundtripTime.ToString() : string.Empty;
    }
}