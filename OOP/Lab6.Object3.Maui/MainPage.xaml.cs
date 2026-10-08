using OOP.Lab6.Core;

namespace Lab6Object3;

public partial class MainPage : ContentPage
{
    private Lab6DataWatcher? _dataWatcher;

    public MainPage()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void OnLoaded(object? sender, EventArgs e)
    {
        _ = RefreshFromSharedFilesAsync();
        _dataWatcher = new Lab6DataWatcher(() => MainThread.BeginInvokeOnMainThread(() => _ = RefreshFromSharedFilesAsync()));
    }

    private void OnUnloaded(object? sender, EventArgs e)
    {
        _dataWatcher?.Dispose();
        _dataWatcher = null;
    }

    private async Task RefreshFromSharedFilesAsync()
    {
        IReadOnlyList<(int X, int Y)> points = Lab6Session.ReadPointsFile();
        if (points.Count == 0)
        {
            string? clipboardText = await Clipboard.Default.GetTextAsync().ConfigureAwait(true);
            if (!string.IsNullOrWhiteSpace(clipboardText))
                points = Lab6Session.ParseClipboard(clipboardText);
        }

        if (points.Count == 0)
        {
            InfoLabel.Text = "Очікування даних від Lab6 (файл /tmp/oop_lab6_points.txt)...";
            GraphView.Drawable = new GraphDrawable(Array.Empty<(int, int)>());
            GraphView.Invalidate();
            return;
        }

        InfoLabel.Text = $"Графік y=f(x): {points.Count} точок";
        GraphView.Drawable = new GraphDrawable(points);
        GraphView.Invalidate();
    }
}
