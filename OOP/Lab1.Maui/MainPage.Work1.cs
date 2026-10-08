using Lab1.Modules.Module1;

namespace Lab1;


public partial class MainPage
{
    private async void OnWork1Clicked(object? sender, EventArgs e)
    {
        CloseWorksMenu();
        await RunWork1Async().ConfigureAwait(true);
    }

    private async Task RunWork1Async()
    {
        int result = await Module1Facade.Func_MOD1Async(this, moduleOutputBuffer).ConfigureAwait(true);
        if (result != 0 && moduleOutputBuffer.Count > 0)
        {
            DisplayLabel.Text = moduleOutputBuffer[0];
        }
    }
}
