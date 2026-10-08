namespace Lab6;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var mainPage = new MainPage();
        NavigationPage.SetHasNavigationBar(mainPage, true);

        return new Window(new NavigationPage(mainPage))
        {
            Title = "Lab6",
            Width = 720,
            Height = 480,
        };
    }
}
