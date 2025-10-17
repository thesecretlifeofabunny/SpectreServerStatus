using Spectre.Console;

namespace SpectreServerStatus;

public static class Program
{
    public static async Task Main(string[] args)
    {
        if (!ConfigurationProcurement.ProcureListOfServersToPing(out var listOfServersToPing, args)) return;

        await PingListOfServers(listOfServersToPing);
    }

    // TODO: Allow for event handling so that we have a do while until eventSignal
    private static async Task PingListOfServers(List<ServerPingService> listOfServersToPing)
    {
        var table = ServerPingService.TableBuilder();

        CancellationTokenSource loopCancellationTokenSource = new();

        while (!loopCancellationTokenSource.IsCancellationRequested)
        {
            foreach (var serverToPing in listOfServersToPing)
            {
                const int tenSecondsInMilliseconds = 10000;
                CancellationTokenSource cancellationTokenSource = new(tenSecondsInMilliseconds);
                await serverToPing.PingServer(cancellationTokenSource.Token);
                table.AddRow(serverToPing.ArrayOfLatestPingInformation());
            }

            AnsiConsole.Clear();
            AnsiConsole.Write(table);
            
            const int fiveSecondsInMilliseconds = 5000;
            Thread.Sleep(fiveSecondsInMilliseconds);
            table.Rows.Clear();
        }
    }
}
