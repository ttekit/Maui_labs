namespace Lab1.Modules.Module1;


public static class Module1Facade
{

    public static async Task<int> Func_MOD1Async(Page hostPage, IList<string> output)
    {
        ArgumentNullException.ThrowIfNull(hostPage);
        ArgumentNullException.ThrowIfNull(output);

        Module1DialogResult result = await Module1DialogOpen.ShowAsync(hostPage).ConfigureAwait(true);

        if (result.IsConfirmed)
        {
            output.Clear();
            output.Add(result.Text);
            return 1;
        }

        return 0;
    }
}
