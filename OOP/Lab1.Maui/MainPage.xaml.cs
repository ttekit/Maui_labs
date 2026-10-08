using Lab1.Modules.Module1;
using Lab1.Modules.Module2;

namespace Lab1;

/// <summary>
/// Main window (analog of Lab1.cpp — WndProc, menu, display area).
/// </summary>
public partial class MainPage : ContentPage
{
    private readonly List<string> moduleOutputBuffer = [];

    public MainPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        string? demoMode = Environment.GetEnvironmentVariable("OOP_LAB1_DEMO");
        if (demoMode == "module1")
        {
            await Task.Delay(800).ConfigureAwait(true);
            await RunWork1Async().ConfigureAwait(true);
        }
        else if (demoMode == "module2")
        {
            await Task.Delay(800).ConfigureAwait(true);
            await RunWork2Async().ConfigureAwait(true);
        }
    }

    private void OnWorksMenuClicked(object? sender, EventArgs e)
    {
        WorksSubmenu.IsVisible = !WorksSubmenu.IsVisible;
    }

    private void CloseWorksMenu()
    {
        WorksSubmenu.IsVisible = false;
    }

    private async void OnWork1Clicked(object? sender, EventArgs e)
    {
        CloseWorksMenu();
        await RunWork1Async().ConfigureAwait(true);
    }

    private async void OnWork2Clicked(object? sender, EventArgs e)
    {
        CloseWorksMenu();
        await RunWork2Async().ConfigureAwait(true);
    }

    private async Task RunWork1Async()
    {
        int result = await Module1Facade.Func_MOD1Async(this, moduleOutputBuffer).ConfigureAwait(true);
        if (result != 0 && moduleOutputBuffer.Count > 0)
        {
            DisplayLabel.Text = moduleOutputBuffer[0];
        }
    }

    private async Task RunWork2Async()
    {
        int result = await Module2Facade.Func_MOD2Async(this, moduleOutputBuffer).ConfigureAwait(true);
        if (result != 0 && moduleOutputBuffer.Count > 0)
        {
            DisplayLabel.Text = moduleOutputBuffer[0];
        }
    }
}
