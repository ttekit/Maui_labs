using OOP.MyTable;
using OOP.Shapes;
using OOP.Shapes.Shapes;

namespace Lab5;

public partial class MainPage : ContentPage
{
    private readonly MyEditor _editor;
    private readonly MyTable _table;
    private readonly EditorDrawable _drawable;
    private readonly string _shapeFilePath;
    private bool _isPointerDown;
    private float _lastPointerX;
    private float _lastPointerY;

    public MainPage(MyEditor editor, MyTable table)
    {
        _editor = editor;
        _table = table;
        _drawable = new EditorDrawable(() => _editor);
        _shapeFilePath = Path.Combine(FileSystem.AppDataDirectory, "shapes.txt");
        InitializeComponent();
        CanvasView.Drawable = _drawable;
        _editor.Invalidated += () => CanvasView.Invalidate();
        _editor.ModeChanged += kind => Title = _editor.CurrentModeTitle;
        Title = _editor.CurrentModeTitle;
    }

    private async Task RegisterLastShapeAsync()
    {
        int count = _editor.Storage.Count;
        if (count == 0)
            return;

        Shape? shape = _editor.Storage.Shapes[count - 1];
        if (shape is null)
            return;

        var (x1, y1, x2, y2) = shape.GetCoordinates();
        _table.Add(shape.DisplayName, x1, y1, x2, y2);
        await ShapeFileService.AppendShapeAsync(_shapeFilePath, shape).ConfigureAwait(true);
    }

    private void OnPointClicked(object? sender, EventArgs e) => _editor.Start(ShapeKind.Point);
    private void OnLineClicked(object? sender, EventArgs e) => _editor.Start(ShapeKind.Line);
    private void OnRectClicked(object? sender, EventArgs e) => _editor.Start(ShapeKind.Rectangle);
    private void OnEllipseClicked(object? sender, EventArgs e) => _editor.Start(ShapeKind.Ellipse);
    private void OnLineOClicked(object? sender, EventArgs e) => _editor.Start(ShapeKind.LineWithCircles);
    private void OnCubeClicked(object? sender, EventArgs e) => _editor.Start(ShapeKind.Cube);

    private void OnStartInteraction(object? sender, TouchEventArgs e)
    {
        if (e.Touches.Length == 0)
            return;

        var point = e.Touches[0];
        _lastPointerX = (float)point.X;
        _lastPointerY = (float)point.Y;
        _isPointerDown = true;
        _editor.OnLeftButtonDown(_lastPointerX, _lastPointerY);
    }

    private void OnDragInteraction(object? sender, TouchEventArgs e)
    {
        if (e.Touches.Length == 0)
            return;

        var point = e.Touches[0];
        _lastPointerX = (float)point.X;
        _lastPointerY = (float)point.Y;
        _editor.OnMouseMove(_lastPointerX, _lastPointerY);
    }

    private async void OnEndInteraction(object? sender, TouchEventArgs e)
    {
        if (!_isPointerDown)
            return;

        int before = _editor.Storage.Count;

        if (e.Touches.Length > 0)
        {
            var point = e.Touches[0];
            _lastPointerX = (float)point.X;
            _lastPointerY = (float)point.Y;
        }

        _editor.OnLeftButtonUp(_lastPointerX, _lastPointerY);
        _isPointerDown = false;

        if (_editor.Storage.Count > before)
            await RegisterLastShapeAsync().ConfigureAwait(true);
    }

    private async void OnShowTableClicked(object? sender, EventArgs e)
    {
        if (Navigation.ModalStack.Count > 0)
            return;

        var dialog = new TablePage(_table);
        ModalPageConfiguration.ConfigureAsDialog(dialog);
        await Navigation.PushModalAsync(dialog).ConfigureAwait(true);
    }

    private async void OnCloseTableClicked(object? sender, EventArgs e)
    {
        if (Navigation.ModalStack.Count == 0)
            return;

        await Navigation.PopModalAsync().ConfigureAwait(true);
    }

    private async void OnLoadClicked(object? sender, EventArgs e)
    {
        IReadOnlyList<ShapeRecord> records = await ShapeFileService.LoadAsync(_shapeFilePath).ConfigureAwait(true);
        _editor.Storage.Clear();
        _table.Clear();
        foreach (ShapeRecord record in records)
        {
            Shape shape = ShapeFileService.CreateShapeFromRecord(record);
            _editor.Storage.TryAdd(shape);
            _table.Add(record.Name, record.X1, record.Y1, record.X2, record.Y2);
        }

        CanvasView.Invalidate();
        await DisplayAlertAsync("Lab5", $"Завантажено {records.Count} об'єктів.", "OK").ConfigureAwait(true);
    }

    private async void OnClearClicked(object? sender, EventArgs e)
    {
        bool confirmed = await DisplayAlertAsync(
            "Lab5",
            "Очистити canvas, таблицю та файл shapes.txt?",
            "Так",
            "Відміна").ConfigureAwait(true);

        if (!confirmed)
            return;

        _editor.Storage.Clear();
        _table.Clear();
        await ShapeFileService.ClearAsync(_shapeFilePath).ConfigureAwait(true);
        CanvasView.Invalidate();
    }

    private void OnExitClicked(object? sender, EventArgs e)
    {
        OnCloseTableClicked(sender, e);
        Application.Current?.Quit();
    }
}
