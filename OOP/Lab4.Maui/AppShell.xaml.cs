using OOP.Shapes;

namespace Lab4;

public partial class AppShell : Shell
{
    public AppShell(MyEditor editor)
    {
        InitializeComponent();
        Items.Add(new ShellContent
        {
            Title = "Lab4",
            Content = new MainPage(editor),
            Route = "MainPage",
        });
    }
}
