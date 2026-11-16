namespace ChestCounter;

/// <summary>Version, GitHub repository and names shared by the tool's processes.</summary>
public static class AppInfo
{
    public static Version Version { get; } = typeof(AppInfo).Assembly.GetName().Version ?? new Version(1, 0, 0);

    public static string VersionText => $"{Version.Major}.{Version.Minor}.{Math.Max(0, Version.Build)}";

    /// <summary>This fork: help links, problem reports and updates (its releases carry <see cref="ExeName"/>).</summary>
    public const string Repo = "eierman324/ChestAddictionCounter";

    /// <summary>The original project: it keeps the treasure data (content.json, prize pictures) up to date every week.</summary>
    public const string UpstreamRepo = "LED-N/Fortnite-ChestCounter";
    public static string UpstreamUrl => "https://github.com/" + UpstreamRepo;
    public static string RepoUrl => "https://github.com/" + Repo;
    public static string IssuesUrl => RepoUrl + "/issues";

    /// <summary>Name shown everywhere (window, report, overlay page).</summary>
    public const string Name = "Storm Report";

    /// <summary>File name of the published exe (and of the release asset the updater looks for).</summary>
    public const string ExeName = "StormReport.exe";

    /// <summary>Earlier names of the exe: an update replaces the file in place, so an older install keeps its name.</summary>
    public static readonly string[] LegacyExeNames = { "ChestAddictionCounter.exe" };

    /// <summary>Path of the published exe (StormReport.exe, or an earlier name of it); null under "dotnet StormReport.dll".</summary>
    public static string? ExePath
    {
        get
        {
            var p = Environment.ProcessPath;
            return p != null && (Path.GetFileName(p).Equals(ExeName, StringComparison.OrdinalIgnoreCase)
                || LegacyExeNames.Contains(Path.GetFileName(p), StringComparer.OrdinalIgnoreCase)) ? p : null;
        }
    }

    // Names of the per-user kernel objects that let the processes find each other.
    /// <summary>One instance per data folder (the normal folder, or a test folder given with --data).</summary>
    public static string InstanceMutexFor(string dataDir)
    {
        var hash = System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(Path.GetFullPath(dataDir).TrimEnd('\\', '/').ToLowerInvariant()));
        return @"Local\ChestCounter.Instance." + Convert.ToHexString(hash, 0, 8);
    }
    /// <summary>Set by a second launch: the instance already open comes to the front.</summary>
    public static string ShowEventFor(string dataDir) => InstanceMutexFor(dataDir).Replace(".Instance.", ".Show.");

    public const string StandbyMutex = @"Local\ChestCounter.Standby";
    public const string StandbyStopEvent = @"Local\ChestCounter.StandbyStop";
}
