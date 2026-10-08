using OOP.Lab6.Core;

namespace Lab6Object2;

public sealed class HeadlessSyncPage : ContentPage
{
    private Lab6DataWatcher? _dataWatcher;

    public HeadlessSyncPage()
    {
        BackgroundColor = Colors.Transparent;
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void OnLoaded(object? sender, EventArgs e)
    {
#if MACCATALYST
        HeadlessWindowHelper.HideApplicationWindows();
#endif
        _ = Object2SyncService.SyncAsync();
        _dataWatcher = new Lab6DataWatcher(() =>
            MainThread.BeginInvokeOnMainThread(() => _ = Object2SyncService.SyncAsync()));
    }

    private void OnUnloaded(object? sender, EventArgs e)
    {
        _dataWatcher?.Dispose();
        _dataWatcher = null;
    }
}
