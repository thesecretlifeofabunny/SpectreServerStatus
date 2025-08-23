using System.Globalization;
using System.Net.NetworkInformation;
using Spectre.Console;

namespace SpectreServerStatus;

//TODO: Allow for specifying a config of servers to ping.
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
        
        var listOfServersToPing = args.Select(server => new ServerPingService(server)).ToList();
        PingListOfServers(listOfServersToPing);
    }
        
        // TODO: Allow for event handling so that we have a do while until eventSignal
        // TODO: Async
        private static void PingListOfServers(List<ServerPingService> listOfServersToPing)
        {
            var table = ServerPingService.TableBuilder();

            while (true)
            {
                AnsiConsole.Live(table)
                    .AutoClear(true)
                    .Start(ctx =>
                    {
                        foreach (var serverToPing in listOfServersToPing)
                        {
                            serverToPing.PingServer();
                            table.AddRow(serverToPing.ArrayOfLatestPingInformation());
                        }

                        ctx.Refresh();
                        Thread.Sleep(FiveMilliseconds);
                    });
                table.Rows.Clear();
            }
        }
    }