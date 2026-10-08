namespace Lab1.Modules.Module2;

/// <summary>
/// Public interface of module2 (analog of module2.h — single entry point).
/// </summary>
public static class Module2Facade
{
    /// <summary>
    /// Shows horizontal scroll dialog (1–100). Returns 0 if cancelled, non-zero if OK.
    /// </summary>
    /// <param name="hostPage">Main window page (HWND equivalent).</param>
    /// <param name="output">Selected number as string when result is non-zero.</param>
    public static async Task<int> Func_MOD2Async(Page hostPage, IList<string> output)
    {
        ArgumentNullException.ThrowIfNull(hostPage);
        ArgumentNullException.ThrowIfNull(output);

        var dialog = new Module2DialogPage();
        ModalPageConfiguration.ConfigureAsDialog(dialog);
        await hostPage.Navigation.PushModalAsync(dialog).ConfigureAwait(true);
        Module2DialogResult result = await dialog.WaitForCloseAsync().ConfigureAwait(true);

        if (result.IsConfirmed)
        {
            output.Clear();
            output.Add(result.ValueText);
            return 1;
        }

        return 0;
    }
}
