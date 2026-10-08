using Lab6.Pages;
using OOP.Lab6.Core;

namespace Lab6;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private async void OnRunVariant0Clicked(object? sender, EventArgs e)
    {
        string? nText = await DisplayPromptAsync("Параметри", "nPoint:", initialValue: "10", keyboard: Keyboard.Numeric).ConfigureAwait(true);
        if (nText is null) return;
        string? xMinText = await DisplayPromptAsync("Параметри", "xMin:", initialValue: "0", keyboard: Keyboard.Numeric).ConfigureAwait(true);
        if (xMinText is null) return;
        string? xMaxText = await DisplayPromptAsync("Параметри", "xMax:", initialValue: "100", keyboard: Keyboard.Numeric).ConfigureAwait(true);
        if (xMaxText is null) return;
        string? yMinText = await DisplayPromptAsync("Параметри", "yMin:", initialValue: "0", keyboard: Keyboard.Numeric).ConfigureAwait(true);
        if (yMinText is null) return;
        string? yMaxText = await DisplayPromptAsync("Параметри", "yMax:", initialValue: "100", keyboard: Keyboard.Numeric).ConfigureAwait(true);
        if (yMaxText is null) return;

        var session = new Lab6Session
        {
            NPoints = int.Parse(nText),
            XMin = int.Parse(xMinText),
            XMax = int.Parse(xMaxText),
            YMin = int.Parse(yMinText),
            YMax = int.Parse(yMaxText),
        };

        StatusLabel.Text = "Генерація точок...";
        IReadOnlyList<(int X, int Y)> points = session.GeneratePoints(new Random(Lab6Session.DefaultSeed));
        await session.WriteParamsFileAsync().ConfigureAwait(true);
        await session.WritePointsFileAsync(points).ConfigureAwait(true);
        await Object2SyncService.SyncAsync(text => Clipboard.Default.SetTextAsync(text)).ConfigureAwait(true);

        var resultPage = new Variant0ResultPage(session, points);
        await Navigation.PushModalAsync(resultPage).ConfigureAwait(true);

        StatusLabel.Text = "Результат у Lab6. Object3 — кнопкою «Відкрити Object3».";
    }

    private async void OnOpenObject2Clicked(object? sender, EventArgs e)
    {
        StatusLabel.Text = "Object2: оновлення Clipboard...";
        IReadOnlyList<(int X, int Y)> points = await Object2SyncService
            .SyncAsync(text => Clipboard.Default.SetTextAsync(text))
            .ConfigureAwait(true);
        StatusLabel.Text = $"Object2 (без вікна): Clipboard оновлено, {points.Count} точок.";
    }

    private async void OnOpenObject3Clicked(object? sender, EventArgs e)
    {
        StatusLabel.Text = "Запуск Object3...";
        if (!await Task.Run(() => Lab6AppLauncher.TryLaunch("Lab6Object3", background: false)).ConfigureAwait(true))
            await ShowMissingAppAlertAsync("Lab6Object3").ConfigureAwait(true);
    }

    private async Task ShowMissingAppAlertAsync(string appName)
    {
        string buildHint = $"dotnet build Lab6.Object3.Maui -f {Lab6AppLauncher.TargetFrameworkFolder}";

        StatusLabel.Text = $"Не вдалося запустити {appName}.";
        await DisplayAlertAsync(
            "Lab6",
            $"Не вдалося зібрати або запустити {appName}.\n\nСпробуйте:\n{buildHint}",
            "OK").ConfigureAwait(true);
    }

    private void OnExitClicked(object? sender, EventArgs e) => Application.Current?.Quit();
}
