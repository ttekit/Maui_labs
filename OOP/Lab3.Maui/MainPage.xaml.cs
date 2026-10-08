using OOP.Shapes;

namespace Lab3;

public partial class MainPage : ContentPage
{
    private readonly ShapeObjectsEditor _editor;
    private readonly EditorDrawable _drawable;
    private bool _isPointerDown;
    private float _lastPointerX;
    private float _lastPointerY;

    public MainPage()
    {
        _editor = new ShapeObjectsEditor(EditorSettings.Lab3);
        _drawable = new EditorDrawable(() => _editor);
        InitializeComponent();
        SetupTooltips();
        CanvasView.Drawable = _drawable;
        _editor.Invalidated += () => CanvasView.Invalidate();
        _editor.ModeChanged += OnModeChanged;
        Title = _editor.CurrentModeTitle;
        UpdateToolbarHighlight();
    }

    private void SetupTooltips()
    {
        SemanticProperties.SetDescription(BtnPoint, "Ввід крапки");
        SemanticProperties.SetDescription(BtnLine, "Ввід лінії");
        SemanticProperties.SetDescription(BtnRect, "Ввід прямокутника");
        SemanticProperties.SetDescription(BtnEllipse, "Ввід еліпсу");
    }

    private void OnModeChanged(ShapeKind kind)
    {
        Title = _editor.CurrentModeTitle;
        UpdateToolbarHighlight();
    }

    private void UpdateToolbarHighlight()
    {
        ResetButton(BtnPoint);
        ResetButton(BtnLine);
        ResetButton(BtnRect);
        ResetButton(BtnEllipse);
        Button active = _editor.CurrentKind switch
        {
            ShapeKind.Point => BtnPoint,
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

    private void OnPointClicked(object? sender, EventArgs e) => _editor.StartPointEditor();
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
        await DisplayAlertAsync("Lab3", "Редактор з Toolbar. Варіант Ж_lab3=5 (Ж+1).", "OK").ConfigureAwait(true);
    }

    private void OnExitClicked(object? sender, EventArgs e) => Application.Current?.Quit();
}
