using OOP.MyTable;
using OOP.Shapes;

namespace Lab5;

public partial class AppShell : Shell
{
    public AppShell(MyEditor editor, MyTable table)
    {
        InitializeComponent();
        Items.Add(new ShellContent
        {
            Title = "Lab5",
            Content = new MainPage(editor, table),
            Route = "MainPage",
        });
    }
}
