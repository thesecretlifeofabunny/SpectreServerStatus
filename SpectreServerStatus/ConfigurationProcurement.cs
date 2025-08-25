using System.Text.Json;

namespace SpectreServerStatus;

public static class ConfigurationProcurement
{
    private const string LinuxConfigurationEnvironmentVariable = "XDG_CONFIG_DIRS";
    private const string LinuxHomeEnvironmentVariable = "HOME";
    private const string ConfigFile = "SpectreServerStatus/config.json";

    /// <summary>
    /// Procures the list of servers to ping from arguments or config.json if arguments are 0
    /// </summary>
    /// <param name="listOfServersToPing"></param>
    /// <param name="args"></param>
    /// <returns>Returns true if successful, false if failed to procure the list</returns>
    // TODO: This could definitely use some more refactoring, error checking, and all around care.
    // *works* for now though, it is late as of writing this and I want sleep.
    public static bool ProcureListOfServersToPing(out List<ServerPingService> listOfServersToPing, string[] args)
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

        if (stringListOfServers.Length != 0)
            return stringListOfServers.Select(server => new ServerPingService(server)).ToList();

        Console.WriteLine("Failed to parse json file, please follow the format specified in README.md");
        return [];
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