using Lab1;

namespace Lab1.Modules.Module2;


internal static class Module2DialogOpen
{
    internal static async Task<Module2DialogResult> ShowAsync(Page hostPage)
    {
        ArgumentNullException.ThrowIfNull(hostPage);

        var dialog = new Module2DialogPage();
        ModalPageConfiguration.ConfigureAsDialog(dialog);
        await hostPage.Navigation.PushModalAsync(dialog).ConfigureAwait(true);
        return await dialog.WaitForCloseAsync().ConfigureAwait(true);
    }
}
