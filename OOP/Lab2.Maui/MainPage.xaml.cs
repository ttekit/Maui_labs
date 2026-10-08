using OOP.Shapes;

namespace Lab2;

public partial class MainPage : ContentPage
{
    private readonly ShapeObjectsEditor _editor;
    private readonly EditorDrawable _drawable;
    private bool _isPointerDown;
    private float _lastPointerX;
    private float _lastPointerY;

    public MainPage()
    {
        _editor = new ShapeObjectsEditor(EditorSettings.Lab2);
        _drawable = new EditorDrawable(() => _editor);
        InitializeComponent();
        CanvasView.Drawable = _drawable;
        _editor.Invalidated += () => CanvasView.Invalidate();
        _editor.ModeChanged += OnModeChanged;
        _editor.StorageFull += OnStorageFull;
        UpdateModeIndicators();
    }

    private async void OnStorageFull()
    {
        ModeLabel.Text = $"Масив заповнено ({_editor.Storage.Capacity} об'єктів).";
        await DisplayAlertAsync(
            "Lab2",
            $"Досягнуто ліміт масиву ({_editor.Storage.Capacity} об'єктів).",
            "OK").ConfigureAwait(true);
    }

    private void OnModeChanged(ShapeKind kind)
    {
        _isPointerDown = false;
        UpdateModeIndicators();
    }

    private void UpdateModeIndicators()
    {
        UpdateMenuChecks();
        UpdateToolbarHighlight();
        ModeLabel.Text = $"Активний режим: {_editor.CurrentModeTitle}";
    }

    private void UpdateToolbarHighlight()
    {
        ResetButton(BtnPoint);
        ResetButton(BtnFreehand);
        ResetButton(BtnLine);
        ResetButton(BtnRect);
        ResetButton(BtnEllipse);
        Button active = _editor.CurrentKind switch
        {
            ShapeKind.Point => BtnPoint,
            ShapeKind.Freehand => BtnFreehand,
            ShapeKind.Line => BtnLine,
            ShapeKind.Rectangle => BtnRect,
            _ => BtnEllipse,
        };
        active.BackgroundColor = Colors.LightBlue;
        active.TextColor = Colors.Black;
    }

    private static void ResetButton(Button button)
    {
        button.BackgroundColor = Colors.White;
        button.TextColor = Colors.Black;
    }

    private void UpdateMenuChecks()
    {
        MenuPoint.Text = _editor.IsMenuItemChecked(ShapeKind.Point) ? "✓ Крапка" : "Крапка";
        MenuFreehand.Text = _editor.IsMenuItemChecked(ShapeKind.Freehand) ? "✓ Малювання" : "Малювання";
        MenuLine.Text = _editor.IsMenuItemChecked(ShapeKind.Line) ? "✓ Лінія" : "Лінія";
        MenuRect.Text = _editor.IsMenuItemChecked(ShapeKind.Rectangle) ? "✓ Прямокутник" : "Прямокутник";
        MenuEllipse.Text = _editor.IsMenuItemChecked(ShapeKind.Ellipse) ? "✓ Еліпс" : "Еліпс";
    }

    private void OnPointClicked(object? sender, EventArgs e) => _editor.StartPointEditor();
    private void OnFreehandClicked(object? sender, EventArgs e) => _editor.StartFreehandEditor();
    private void OnLineClicked(object? sender, EventArgs e) => _editor.StartLineEditor();
    private void OnRectClicked(object? sender, EventArgs e) => _editor.StartRectEditor();
    private void OnEllipseClicked(object? sender, EventArgs e) => _editor.StartEllipseEditor();

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

    private void OnEndInteraction(object? sender, TouchEventArgs e)
    {
        if (!_isPointerDown)
            return;

        if (e.Touches.Length > 0)
        {
            var point = e.Touches[0];
            _lastPointerX = (float)point.X;
            _lastPointerY = (float)point.Y;
        }

        _editor.OnLeftButtonUp(_lastPointerX, _lastPointerY);
        _isPointerDown = false;
    }

    private async void OnAboutClicked(object? sender, EventArgs e)
    {
        await DisplayAlertAsync("Lab2", "Графічний редактор об'єктів. Варіант Ж=4.", "OK").ConfigureAwait(true);
    }

    private void OnExitClicked(object? sender, EventArgs e) => Application.Current?.Quit();
}
