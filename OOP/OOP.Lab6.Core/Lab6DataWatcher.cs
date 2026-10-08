namespace OOP.Lab6.Core;


public sealed class Lab6DataWatcher : IDisposable
{
    private readonly FileSystemWatcher _paramsWatcher;
    private readonly FileSystemWatcher _pointsWatcher;
    private readonly Action _onChanged;

    public Lab6DataWatcher(Action onChanged)
    {
        _onChanged = onChanged;
        string directory = Path.GetTempPath();

        _paramsWatcher = CreateWatcher(directory, "oop_lab6_params.txt");
        _pointsWatcher = CreateWatcher(directory, "oop_lab6_points.txt");
    }

    public void Dispose()
    {
        _paramsWatcher.Dispose();
        _pointsWatcher.Dispose();
    }

    private FileSystemWatcher CreateWatcher(string directory, string fileName)
    {
        var watcher = new FileSystemWatcher(directory, fileName)
        {
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size,
            EnableRaisingEvents = true,
        };

        watcher.Changed += OnFileChanged;
        watcher.Created += OnFileChanged;
        return watcher;
    }

    private void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        Thread.Sleep(50);
        _onChanged();
    }
}
