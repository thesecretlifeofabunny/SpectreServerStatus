using Spectre.Console;
using System.IO;
using System.Text.Json;

namespace SpectreServerStatus;

public abstract class Program
{
    private const int FiveSecondsInMilliseconds = 5000;
    private const int TenSecondsInMilliseconds = 10000;
    private const string LinuxConfigurationEnvironmentVariable = "XDG_CONFIG_DIRS";
    private const string LinuxHomeEnvironmentVariable = "HOME";
    private const string ConfigFile = "SpectreServerStatus/config.json";

    public static async Task Main(string[] args)
    {
        if (!ProcureListOfServersToPing(out var listOfServersToPing, args)) return;

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

    /// <summary>
    /// Procures the list of servers to ping from arguments or config.json if arguments are 0
    /// </summary>
    /// <param name="listOfServersToPing"></param>
    /// <param name="args"></param>
    /// <returns>Returns true if successful, false if failed to procure the list</returns>
    // TODO: This could definitely use some more refactoring, error checking, and all around care.
    // *works* for now though, it is late as of writing this and I want sleep.
    private static bool ProcureListOfServersToPing(out List<ServerPingService> listOfServersToPing, string[] args)
    {
        listOfServersToPing = [];

        if (args.Length > 0)
        {
            listOfServersToPing = args.Select(server => new ServerPingService(server)).ToList();
            return true;
        }

        GetListOfPathsToCheck(out var listOfPathsToCheck);

        if (listOfPathsToCheck.Count == 0)
        {
            Console.WriteLine("Failed to get lists of potential configuration file directories from your system");
            Console.WriteLine("We only support Linux at this moment, apologies");
            Console.Write("If you are on Linux please add a config.json to your ~/.config/SpectreServerStatus");
            Console.WriteLine("folder following the README.md instructions");
            return false;
        }

        listOfServersToPing = GetJsonFromListOfPaths(listOfPathsToCheck);

        return true;
    }

    private static List<ServerPingService> GetJsonFromListOfPaths(List<string> listOfPathsToCheck)
    {
        string[] stringListOfServers = [];
        foreach (var jsonString in from pathToCheck
                     in listOfPathsToCheck
                                   where File.Exists(pathToCheck)
                                   select File.ReadAllText(pathToCheck)
                )
        {
            stringListOfServers = JsonSerializer.Deserialize<string[]>(jsonString)!;
        }

        if (stringListOfServers.Length == 0)
        {
            Console.WriteLine("Failed to parse json file, please follow the format specified in README.md");
            return [];
        };

        return stringListOfServers.Select(server => new ServerPingService(server)).ToList();
    }

    private static void GetListOfPathsToCheck(out List<string> listOfPathsToCheck)
    {
        listOfPathsToCheck = [];
        var configurationDirectories = Environment.GetEnvironmentVariable(LinuxConfigurationEnvironmentVariable);
        if (!string.IsNullOrEmpty(configurationDirectories))
        {
            listOfPathsToCheck.AddRange(
                configurationDirectories.Split(":").Select(directory => directory + "/" + ConfigFile)
            );
        }

        var homeDirectory = Environment.GetEnvironmentVariable(LinuxHomeEnvironmentVariable);
        if (!string.IsNullOrEmpty(homeDirectory))
        {
            listOfPathsToCheck.Add(homeDirectory + "/.config/" + ConfigFile);
        }
    }
}