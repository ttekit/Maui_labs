using OOP.Shapes;

namespace Lab4;

public partial class App : Application
{
    private readonly MyEditor _editor;

    public App(MyEditor editor)
    {
        _editor = editor;
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell(_editor));
    }
}
