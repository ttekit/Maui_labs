using Lab1;

namespace Lab1.Modules.Module1;


internal static class Module1DialogOpen
{
    internal static async Task<Module1DialogResult> ShowAsync(Page hostPage)
    {
        ArgumentNullException.ThrowIfNull(hostPage);

        var dialog = new Module1DialogPage();
        ModalPageConfiguration.ConfigureAsDialog(dialog);
        await hostPage.Navigation.PushModalAsync(dialog).ConfigureAwait(true);
        return await dialog.WaitForCloseAsync().ConfigureAwait(true);
    }
}
