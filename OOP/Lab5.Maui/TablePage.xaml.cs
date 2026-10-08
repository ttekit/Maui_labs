using OOP.MyTable;

namespace Lab5;

public partial class TablePage : ContentPage
{
    public TablePage(MyTable table)
    {
        InitializeComponent();
        TableView.ItemsSource = table.Rows;
        table.RowsChanged += rows => TableView.ItemsSource = rows.ToList();
    }

    private async void OnCloseClicked(object? sender, EventArgs e)
    {
        await Navigation.PopModalAsync().ConfigureAwait(true);
    }

    private void OnRowSelected(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is TableRow row)
            Title = $"Вибрано: {row.Name} ({row.X1},{row.Y1})";
    }
}
