namespace Lab1.Modules.Module2;


public static class Module2Facade
{

    public static async Task<int> Func_MOD2Async(Page hostPage, IList<string> output)
    {
        ArgumentNullException.ThrowIfNull(hostPage);
        ArgumentNullException.ThrowIfNull(output);

        Module2DialogResult result = await Module2DialogOpen.ShowAsync(hostPage).ConfigureAwait(true);

        if (result.IsConfirmed)
        {
            output.Clear();
            output.Add(result.ValueText);
            return 1;
        }

        return 0;
    }
}
