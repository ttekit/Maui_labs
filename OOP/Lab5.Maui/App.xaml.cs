using OOP.MyTable;
using OOP.Shapes;

namespace Lab5;

public partial class App : Application
{
    private readonly MyEditor _editor;
    private readonly MyTable _table;

    public App(MyEditor editor, MyTable table)
    {
        _editor = editor;
        _table = table;
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var mainPage = new MainPage(_editor, _table);
        NavigationPage.SetHasNavigationBar(mainPage, false);

        return new Window(new NavigationPage(mainPage))
        {
            Title = "Lab5",
            Width = 900,
            Height = 650,
        };
    }
}
