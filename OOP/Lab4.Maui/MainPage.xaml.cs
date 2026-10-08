using OOP.Shapes;

namespace Lab4;

public partial class MainPage : ContentPage
{
    private readonly MyEditor _editor;
    private readonly EditorDrawable _drawable;
    private bool _isPointerDown;
    private float _lastPointerX;
    private float _lastPointerY;

    public MainPage(MyEditor editor)
    {
        _editor = editor;
        _drawable = new EditorDrawable(() => _editor);
        InitializeComponent();
        CanvasView.Drawable = _drawable;
        _editor.Invalidated += () => CanvasView.Invalidate();
        _editor.ModeChanged += kind => Title = _editor.CurrentModeTitle;
        Title = _editor.CurrentModeTitle;
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
        await DisplayAlertAsync("Lab4", "MyEditor (dynamic). 6 типів фігур. Ж=4.", "OK").ConfigureAwait(true);
    }

    private void OnExitClicked(object? sender, EventArgs e) => Application.Current?.Quit();
}
