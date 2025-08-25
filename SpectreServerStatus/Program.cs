using Spectre.Console;

namespace SpectreServerStatus;

public abstract class Program
{
    private const int FiveSecondsInMilliseconds = 5000;
    private const int TenSecondsInMilliseconds = 10000;

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
                CancellationTokenSource cancellationTokenSource = new(TenSecondsInMilliseconds);
                await serverToPing.PingServer(cancellationTokenSource.Token);
                table.AddRow(serverToPing.ArrayOfLatestPingInformation());
            }

            AnsiConsole.Clear();
            AnsiConsole.Write(table);
            
            Thread.Sleep(FiveSecondsInMilliseconds);
            table.Rows.Clear();
        }
    }
}