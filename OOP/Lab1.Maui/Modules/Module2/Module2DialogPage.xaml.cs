namespace Lab1.Modules.Module2;

internal sealed record Module2DialogResult(bool IsConfirmed, string ValueText);


public sealed partial class Module2DialogPage : ContentPage
{
    private readonly TaskCompletionSource<Module2DialogResult> closeTaskSource =
        new(TaskCreationOptions.RunContinuationsAsynchronously);

    public Module2DialogPage()
    {
        InitializeComponent();
        UpdateValueLabel((int)ValueSlider.Value);
    }

    internal Task<Module2DialogResult> WaitForCloseAsync()
    {
        return closeTaskSource.Task;
    }

    private void OnSliderValueChanged(object? sender, ValueChangedEventArgs e)
    {
        UpdateValueLabel((int)Math.Round(e.NewValue));
    }

    private void UpdateValueLabel(int value)
    {
        ValueLabel.Text = value.ToString();
    }

    private async void OnOkClicked(object? sender, EventArgs e)
    {
        int value = (int)Math.Round(ValueSlider.Value);
        await CloseAsync(new Module2DialogResult(true, value.ToString())).ConfigureAwait(false);
    }

    private async void OnCancelClicked(object? sender, EventArgs e)
    {
        await CloseAsync(new Module2DialogResult(false, string.Empty)).ConfigureAwait(false);
    }

    private async Task CloseAsync(Module2DialogResult result)
    {
        closeTaskSource.TrySetResult(result);
        await Navigation.PopModalAsync().ConfigureAwait(true);
    }
}
