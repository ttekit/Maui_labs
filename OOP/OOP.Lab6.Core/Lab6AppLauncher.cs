using System.Diagnostics;
using System.Runtime.InteropServices;

namespace OOP.Lab6.Core;


public static class Lab6AppLauncher
{
    public const string TargetFrameworkFolder = "net10.0-maccatalyst";

    private const string RuntimeIdentifier = "maccatalyst-arm64";

    public static string? ResolveExecutablePath(string appName)
    {
        string projectFolder = appName switch
        {
            "Lab6Object2" => "Lab6.Object2.Maui",
            "Lab6Object3" => "Lab6.Object3.Maui",
            _ => throw new ArgumentException($"Unknown app name: {appName}", nameof(appName)),
        };

        string? root = FindOopRootDirectory();
        if (root is null)
            return null;

        foreach (string configuration in new[] { "Debug", "Release" })
        {
            string path = Path.Combine(
                root,
                projectFolder,
                "bin",
                configuration,
                TargetFrameworkFolder,
                RuntimeIdentifier,
                $"{appName}.app",
                "Contents",
                "MacOS",
                appName);

            if (File.Exists(path))
                return path;
        }

        return null;
    }

    public static string? ResolveAppBundlePath(string appName)
    {
        string? executablePath = ResolveExecutablePath(appName);
        if (executablePath is null)
            return null;

        string? appBundle = Directory.GetParent(executablePath)?.Parent?.Parent?.FullName;
        return appBundle is not null && appBundle.EndsWith(".app", StringComparison.Ordinal)
            ? appBundle
            : null;
    }

    public static bool EnsureBuilt(string appName)
    {
        if (ResolveAppBundlePath(appName) is not null)
            return true;

        string? root = FindOopRootDirectory();
        if (root is null)
            return false;

        string projectFile = appName switch
        {
            "Lab6Object2" => Path.Combine(root, "Lab6.Object2.Maui", "Lab6Object2.csproj"),
            "Lab6Object3" => Path.Combine(root, "Lab6.Object3.Maui", "Lab6Object3.csproj"),
            _ => throw new ArgumentException($"Unknown app name: {appName}", nameof(appName)),
        };

        if (!File.Exists(projectFile))
            return false;

        using var process = Process.Start(new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"build \"{projectFile}\" -f {TargetFrameworkFolder} -v q",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
        });

        process?.WaitForExit();
        return process?.ExitCode == 0 && ResolveAppBundlePath(appName) is not null;
    }

    public static bool TryLaunch(string appName, bool background = false)
    {
        if (ResolveAppBundlePath(appName) is null && !EnsureBuilt(appName))
            return false;

        string? appBundle = ResolveAppBundlePath(appName);
        string? executablePath = ResolveExecutablePath(appName);
        if (executablePath is null)
            return false;

        if (IsMacPlatform() && appBundle is not null)
        {
            string backgroundFlag = background ? "-g " : string.Empty;
            Process.Start(new ProcessStartInfo
            {
                FileName = "open",
                Arguments = $"{backgroundFlag}\"{appBundle}\"",
                UseShellExecute = false,
            });
            return true;
        }

        Process.Start(new ProcessStartInfo
        {
            FileName = executablePath,
            UseShellExecute = false,
        });

        return true;
    }

    public static string? FindOopRootDirectory()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (Directory.Exists(Path.Combine(directory.FullName, "Lab6.Object2.Maui")) &&
                Directory.Exists(Path.Combine(directory.FullName, "Lab6.Object3.Maui")))
                return directory.FullName;

            directory = directory.Parent;
        }

        return null;
    }

    private static bool IsMacPlatform() =>
        RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ||
        OperatingSystem.IsMacCatalyst();
}
