using CommunityToolkit.Maui.Storage;
using OOP.MyTable;
using OOP.Shapes;
using OOP.Shapes.Shapes;

namespace Lab5;

public partial class MainPage : ContentPage
{
    private readonly MyEditor _editor;
    private readonly MyTable _table;
    private readonly EditorDrawable _drawable;
    private string? _currentFilePath;
    private bool _isPointerDown;
    private float _lastPointerX;
    private float _lastPointerY;

    public MainPage(MyEditor editor, MyTable table)
    {
        _editor = editor;
        _table = table;
        _drawable = new EditorDrawable(() => _editor);
        InitializeComponent();
        CanvasView.Drawable = _drawable;
        _editor.Invalidated += () => CanvasView.Invalidate();
        _editor.ModeChanged += kind => Title = _editor.CurrentModeTitle;
        _table.RowSelected += OnTableRowSelected;
        Title = _editor.CurrentModeTitle;
    }

    private void OnTableRowSelected(int index)
    {
        _editor.SelectedShapeIndex = index;
        CanvasView.Invalidate();
    }

    private Task RegisterLastShapeAsync()
    {
        int count = _editor.Storage.Count;
        if (count == 0)
            return Task.CompletedTask;

        Shape? shape = _editor.Storage.Shapes[count - 1];
        if (shape is null)
            return Task.CompletedTask;

        var (x1, y1, x2, y2) = shape.GetCoordinates();
        _table.Add(shape.DisplayName, x1, y1, x2, y2);
        return Task.CompletedTask;
    }

    private void DeleteFigureAt(int index)
    {
        _editor.Storage.RemoveAt(index);
        _table.RemoveAt(index);

        if (_editor.SelectedShapeIndex == index)
            _editor.SelectedShapeIndex = null;
        else if (_editor.SelectedShapeIndex > index)
            _editor.SelectedShapeIndex--;

        CanvasView.Invalidate();
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

        var dialog = new TablePage(_table, DeleteFigureAt);
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
        try
        {
            IEnumerable<FileResult?>? results = await MainThread.InvokeOnMainThreadAsync(() =>
                FilePicker.Default.PickMultipleAsync(CoolPaintFileTypes.CreatePickOptions())).ConfigureAwait(true);

            if (results is null)
                return;

            List<FileResult> files = results.Where(file => file is not null).Select(file => file!).ToList();
            if (files.Count == 0)
                return;

            _editor.Storage.Clear();
            _table.Clear();
            _editor.SelectedShapeIndex = null;

            int total = 0;
            foreach (FileResult file in files)
            {
                IReadOnlyList<ShapeRecord> records = await ShapeFileService.LoadAsync(file.FullPath).ConfigureAwait(true);
                foreach (ShapeRecord record in records)
                {
                    Shape shape = ShapeFileService.CreateShapeFromRecord(record);
                    if (!_editor.Storage.TryAdd(shape))
                        break;

                    _table.Add(record.Name, record.X1, record.Y1, record.X2, record.Y2);
                    total++;
                }
            }

            _currentFilePath = files.Count == 1 ? files[0].FullPath : null;
            CanvasView.Invalidate();
            await DisplayAlertAsync("Lab5", $"Завантажено {total} об'єктів.", "OK").ConfigureAwait(true);
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Lab5", $"Помилка завантаження: {ex.Message}", "OK").ConfigureAwait(true);
        }
    }

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        try
        {
            string content = ShapeFileService.BuildFileContent(_editor.Storage.EnumerateShapes());
            using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content));

            string defaultName = _currentFilePath is not null
                ? Path.GetFileName(_currentFilePath)
                : $"painting.{ShapeFileService.FileExtension}";

            FileSaverResult result = await MainThread.InvokeOnMainThreadAsync(() =>
                FileSaver.Default.SaveAsync(defaultName, stream, CancellationToken.None)).ConfigureAwait(true);

            if (result.IsSuccessful)
            {
                _currentFilePath = result.FilePath;
                await DisplayAlertAsync("Lab5", "Збережено.", "OK").ConfigureAwait(true);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Lab5", $"Помилка збереження: {ex.Message}", "OK").ConfigureAwait(true);
        }
    }

    private async void OnClearClicked(object? sender, EventArgs e)
    {
        bool confirmed = await DisplayAlertAsync(
            "Lab5",
            "Очистити canvas та таблицю?",
            "Так",
            "Відміна").ConfigureAwait(true);

        if (!confirmed)
            return;

        _editor.Storage.Clear();
        _table.Clear();
        _editor.SelectedShapeIndex = null;
        _currentFilePath = null;
        CanvasView.Invalidate();
    }

    private void OnExitClicked(object? sender, EventArgs e)
    {
        OnCloseTableClicked(sender, e);
        Application.Current?.Quit();
    }
}
