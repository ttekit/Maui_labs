namespace Lab1;


public partial class MainPage : ContentPage
{
    private readonly List<string> moduleOutputBuffer = [];

    public MainPage()
    {
        InitializeComponent();
    }

    private void OnWorksMenuClicked(object? sender, EventArgs e)
    {
        WorksSubmenu.IsVisible = !WorksSubmenu.IsVisible;
    }

    private void CloseWorksMenu()
    {
        WorksSubmenu.IsVisible = false;
    }
}
