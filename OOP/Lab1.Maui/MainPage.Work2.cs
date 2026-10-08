using Lab1.Modules.Module2;

namespace Lab1;


public partial class MainPage
{
    private async void OnWork2Clicked(object? sender, EventArgs e)
    {
        CloseWorksMenu();
        await RunWork2Async().ConfigureAwait(true);
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
