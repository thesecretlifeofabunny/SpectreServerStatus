using Spectre.Console;

namespace SpectreServerStatus;

//TODO: Allow for specifying a config of servers to ping.
public static class Program
{
    private const int FiveSecondsInMilliseconds = 5000;
    private const int TenSecondsInMilliseconds = 10000;
    public static async Task Main(string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("you must provide an ip address to ping");
            return;
        }

        var listOfServersToPing = args.Select(server => new ServerPingService(server)).ToList();
        await PingListOfServers(listOfServersToPing);
    }

    // TODO: Allow for event handling so that we have a do while until eventSignal
    private static async Task PingListOfServers(List<ServerPingService> listOfServersToPing)
    {
        var table = ServerPingService.TableBuilder();

        while (true)
        {
            await AnsiConsole.Live(table)
                .AutoClear(true)
                .StartAsync(async ctx =>
                {
                    foreach (var serverToPing in listOfServersToPing)
                    {
                        CancellationTokenSource cancellationTokenSource = new(TenSecondsInMilliseconds);
                        await serverToPing.PingServer(cancellationTokenSource.Token);
                        table.AddRow(serverToPing.ArrayOfLatestPingInformation());
                    }

                    ctx.Refresh();
                    Thread.Sleep(FiveSecondsInMilliseconds);
                });
            table.Rows.Clear();
        }
    }
}