namespace Lab6Object2;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = new Window(new HeadlessSyncPage())
        {
            Title = "Object2",
            Width = 1,
            Height = 1,
            X = -32000,
            Y = -32000,
        };

        return window;
    }
}
