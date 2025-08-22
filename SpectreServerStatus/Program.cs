using System.Globalization;
using Spectre.Console;

namespace SpectreServerStatus;

public static class Program
{
    private const int FiveMilliseconds = 5000;

    public static void Main(string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("you must provide an ip address to ping");
            return;
        }

        // TODO allow for multiple servers, or generated list from a config
        var serverToPing = args[0];

        var table = new Table();
        table.Caption($"Refreshing every five seconds");
        table.AddColumn("Server to Ping");
        table.AddColumn("IP Address");
        table.AddColumn("Ping Status");
        table.AddColumn("RoundTrip Time");
        table.AddColumn("Total Number of times pinged");
        table.AddColumn("Total Number of failed pings");
        table.AddColumn("Percentage of Success");

        double loopCount = 0;
        double failureCount = 0;
        double percentageOfSuccess = 0;
        while (true)
        {
            AnsiConsole.Live(table)
                .AutoClear(true)
                .Start(ctx =>
                {
                    var pingReplyFromPingService = PingService.PingServer(serverToPing);
                    
                    List<string> rowBuilder = [];
                    
                    if (pingReplyFromPingService is null)
                    {
                        Console.WriteLine("Failed To Ping server...");
                        AnsiConsole.MarkupLine("Failed To Ping server...");
                        failureCount++;
                    }
                    else
                    {
                        rowBuilder.Add(serverToPing);
                        rowBuilder.Add(pingReplyFromPingService.Address.ToString());
                        rowBuilder.Add((pingReplyFromPingService.Status.ToString()));
                        rowBuilder.Add((pingReplyFromPingService.RoundtripTime.ToString()));
                    }
                    
                    loopCount++;
                    percentageOfSuccess = (loopCount - failureCount) / loopCount * 100;
                    
                    rowBuilder.Add(loopCount.ToString(CultureInfo.CurrentCulture));
                    rowBuilder.Add(failureCount.ToString(CultureInfo.CurrentCulture));
                    rowBuilder.Add(percentageOfSuccess.ToString(CultureInfo.CurrentCulture));
                    
                    table.AddRow(rowBuilder.ToArray());
                    ctx.Refresh();

                    Thread.Sleep(FiveMilliseconds);
                });
            table.Rows.Clear();
        }
    }
}