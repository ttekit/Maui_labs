using Microsoft.Maui.Graphics;
using OOP.Shapes.Editors;

namespace OOP.Shapes;


public class ShapeObjectsEditor
{
    private readonly EditorSettings _settings;
    private readonly ShapeStorage _storage;
    private ShapeEditor? _activeEditor;
    private ShapeKind _currentKind = ShapeKind.Point;

    public ShapeObjectsEditor(EditorSettings settings)
    {
        _settings = settings;
        _storage = new ShapeStorage(settings);
        _storage.CapacityReached += () => StorageFull?.Invoke();
        StartPointEditor();
    }

    public event Action? Invalidated;

    public event Action<ShapeKind>? ModeChanged;

    public event Action? StorageFull;

    public ShapeStorage Storage => _storage;

    public EditorSettings Settings => _settings;

    public ShapeKind CurrentKind => _currentKind;

    public bool UseMenuModeIndicator => _settings.UseMenuModeIndicator;

    public string CurrentModeTitle => _currentKind switch
    {
        ShapeKind.Point => "Режим вводу крапок",
        ShapeKind.Freehand => "Режим малювання",
        ShapeKind.Line => "Режим вводу ліній",
        ShapeKind.Rectangle => "Режим вводу прямокутників",
        ShapeKind.Ellipse => "Режим вводу еліпсів",
        ShapeKind.LineWithCircles => "Режим вводу ліній з кружечками",
        ShapeKind.Cube => "Режим вводу каркасу куба",
        _ => "Редактор об'єктів",
    };

    public void StartPointEditor() => Start(ShapeKind.Point);

    public void StartFreehandEditor() => Start(ShapeKind.Freehand);

    public void StartLineEditor() => Start(ShapeKind.Line);

    public void StartRectEditor() => Start(ShapeKind.Rectangle);

    public void StartEllipseEditor() => Start(ShapeKind.Ellipse);

    public void StartLineOOEditor() => Start(ShapeKind.LineWithCircles);

    public void StartCubeEditor() => Start(ShapeKind.Cube);

    public void Start(ShapeKind kind)
    {
        _currentKind = kind;
        _activeEditor = CreateEditor(kind);
        ModeChanged?.Invoke(kind);
    }

    public void OnLeftButtonDown(float x, float y)
    {
        _activeEditor?.OnLeftButtonDown(x, y);
        RequestInvalidate();
    }

    public void OnLeftButtonUp(float x, float y)
    {
        _activeEditor?.OnLeftButtonUp(x, y);
        RequestInvalidate();
    }

    public void OnMouseMove(float x, float y)
    {
        _activeEditor?.OnMouseMove(x, y);
        RequestInvalidate();
    }

    public void OnPaint(ICanvas canvas, RectF dirtyRect)
    {
        canvas.FillColor = Colors.White;
        canvas.FillRectangle(dirtyRect);
        _activeEditor?.DrawShapes(canvas);
        _activeEditor?.DrawRubberBand(canvas);
    }

    public bool IsMenuItemChecked(ShapeKind kind) => _currentKind == kind;

    private ShapeEditor CreateEditor(ShapeKind kind) => kind switch
    {
        ShapeKind.Point => new PointEditor(_storage),
        ShapeKind.Freehand => new FreehandEditor(_storage),
        ShapeKind.Line => new LineEditor(_storage, ShapeKind.Line),
        ShapeKind.Rectangle => new RectEditor(_storage),
        ShapeKind.Ellipse => new EllipseEditor(_storage),
        ShapeKind.LineWithCircles => new LineOOEditor(_storage),
        ShapeKind.Cube => new CubeEditor(_storage),
        _ => new PointEditor(_storage),
    };

    private void RequestInvalidate() => Invalidated?.Invoke();
}
