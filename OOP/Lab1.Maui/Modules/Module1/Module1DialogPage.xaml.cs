namespace Lab1.Modules.Module1;

internal sealed record Module1DialogResult(bool IsConfirmed, string Text);

/// <summary>
/// Internal dialog implementation (analog of static DlgProc in module1.cpp).
/// </summary>
public sealed partial class Module1DialogPage : ContentPage
{
    private readonly TaskCompletionSource<Module1DialogResult> closeTaskSource =
        new(TaskCreationOptions.RunContinuationsAsynchronously);

    public Module1DialogPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (Environment.GetEnvironmentVariable("OOP_LAB1_DEMO") == "module1")
        {
            TextInputEntry.Text = "Приклад тексту для Lab1";
        }
    }

    internal Task<Module1DialogResult> WaitForCloseAsync()
    {
        return closeTaskSource.Task;
    }

    private async void OnOkClicked(object? sender, EventArgs e)
    {
        string text = TextInputEntry.Text ?? string.Empty;
        await CloseAsync(new Module1DialogResult(true, text)).ConfigureAwait(false);
    }

    private async void OnCancelClicked(object? sender, EventArgs e)
    {
        await CloseAsync(new Module1DialogResult(false, string.Empty)).ConfigureAwait(false);
    }

    private async Task CloseAsync(Module1DialogResult result)
    {
        closeTaskSource.TrySetResult(result);
        await Navigation.PopModalAsync().ConfigureAwait(true);
    }
}
