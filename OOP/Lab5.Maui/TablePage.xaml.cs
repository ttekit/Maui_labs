using OOP.MyTable;

namespace Lab5;

public partial class TablePage : ContentPage
{
    private readonly MyTable _table;
    private readonly Action<int>? _onDeleteFigure;
    private int _selectedIndex = -1;

    public TablePage(MyTable table, Action<int>? onDeleteFigure = null)
    {
        _table = table;
        _onDeleteFigure = onDeleteFigure;
        InitializeComponent();
        TableView.ItemsSource = table.Rows;
        table.RowsChanged += rows =>
        {
            TableView.ItemsSource = rows.ToList();
            if (_selectedIndex >= rows.Count)
            {
                _selectedIndex = -1;
                TableView.SelectedItem = null;
                BtnDelete.IsEnabled = false;
            }
        };
    }

    private async void OnCloseClicked(object? sender, EventArgs e)
    {
        await Navigation.PopModalAsync().ConfigureAwait(true);
    }

    private void OnRowSelected(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not TableRow row)
        {
            _selectedIndex = -1;
            BtnDelete.IsEnabled = false;
            return;
        }

        _selectedIndex = _table.Rows.ToList().IndexOf(row);
        BtnDelete.IsEnabled = _selectedIndex >= 0;
        if (_selectedIndex >= 0)
        {
            _table.SelectRow(_selectedIndex);
            Title = $"Вибрано: {row.Name} ({row.X1},{row.Y1})";
        }
    }

    private async void OnDeleteClicked(object? sender, EventArgs e)
    {
        if (_selectedIndex < 0)
            return;

        bool confirmed = await DisplayAlertAsync(
            "Lab5",
            "Видалити вибраний об'єкт?",
            "Так",
            "Відміна").ConfigureAwait(true);

        if (!confirmed)
            return;

        _onDeleteFigure?.Invoke(_selectedIndex);
        _selectedIndex = -1;
        TableView.SelectedItem = null;
        BtnDelete.IsEnabled = false;
        Title = "Таблиця об'єктів";
    }
}
