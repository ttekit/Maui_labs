using OOP.Lab6.Core;

namespace Lab6Object2;

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
        var session = Lab6Session.LoadFromParamsFile();
        IReadOnlyList<(int X, int Y)> points = Lab6Session.ReadPointsFile();
        if (points.Count == 0)
            points = session.GeneratePoints(new Random(Lab6Session.DefaultSeed));

        SummaryLabel.Text =
            $"nPoint={session.NPoints}, x:[{session.XMin}..{session.XMax}], y:[{session.YMin}..{session.YMax}]";
        PointsView.ItemsSource = points.Select(point => $"({point.X}, {point.Y})").ToList();

        await Clipboard.Default.SetTextAsync(Lab6Session.FormatClipboard(points)).ConfigureAwait(true);
        await File.WriteAllTextAsync(Lab6Session.ReadyFilePath, "ready").ConfigureAwait(true);
    }
}
