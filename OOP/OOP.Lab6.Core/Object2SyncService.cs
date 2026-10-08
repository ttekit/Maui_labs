using System.Diagnostics;

namespace OOP.Lab6.Core;


public static class Object2SyncService
{
    public static async Task<IReadOnlyList<(int X, int Y)>> SyncAsync(
        Func<string, Task>? setClipboardAsync = null,
        CancellationToken cancellationToken = default)
    {
        var session = Lab6Session.LoadFromParamsFile();
        IReadOnlyList<(int X, int Y)> points = Lab6Session.ReadPointsFile();
        if (points.Count == 0)
            points = session.GeneratePoints(new Random(Lab6Session.DefaultSeed));

        string clipboardText = Lab6Session.FormatClipboard(points);
        if (setClipboardAsync is not null)
            await setClipboardAsync(clipboardText).ConfigureAwait(false);
        else
            await SetPlatformClipboardAsync(clipboardText, cancellationToken).ConfigureAwait(false);

        await File.WriteAllTextAsync(Lab6Session.ReadyFilePath, "ready", cancellationToken).ConfigureAwait(false);
        return points;
    }

    private static async Task SetPlatformClipboardAsync(string text, CancellationToken cancellationToken)
    {
        if (!OperatingSystem.IsMacOS() && !OperatingSystem.IsMacCatalyst())
            return;

        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "pbcopy",
                RedirectStandardInput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            },
        };

        process.Start();
        await process.StandardInput.WriteAsync(text.AsMemory(), cancellationToken).ConfigureAwait(false);
        process.StandardInput.Close();
        await process.WaitForExitAsync(cancellationToken).ConfigureAwait(false);
    }
}
