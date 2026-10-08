namespace Lab1.Modules.Module1;

/// <summary>
/// Public interface of module1 (analog of module1.h — single entry point).
/// </summary>
public static class Module1Facade
{
    /// <summary>
    /// Shows text input dialog. Returns 0 if cancelled, non-zero if OK.
    /// </summary>
    /// <param name="hostPage">Main window page (HWND equivalent).</param>
    /// <param name="output">Entered text when result is non-zero.</param>
    public static async Task<int> Func_MOD1Async(Page hostPage, IList<string> output)
    {
        ArgumentNullException.ThrowIfNull(hostPage);
        ArgumentNullException.ThrowIfNull(output);

        var dialog = new Module1DialogPage();
        ModalPageConfiguration.ConfigureAsDialog(dialog);
        await hostPage.Navigation.PushModalAsync(dialog).ConfigureAwait(true);
        Module1DialogResult result = await dialog.WaitForCloseAsync().ConfigureAwait(true);

        if (result.IsConfirmed)
        {
            output.Clear();
            output.Add(result.Text);
            return 1;
        }

        return 0;
    }
}
