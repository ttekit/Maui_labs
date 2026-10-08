using Microsoft.Maui.Graphics;

namespace OOP.Shapes.Shapes;


public abstract class Shape
{
    protected long Xs1 { get; private set; }
    protected long Ys1 { get; private set; }
    protected long Xs2 { get; private set; }
    protected long Ys2 { get; private set; }

    public void Set(long x1, long y1, long x2, long y2)
    {
        Xs1 = x1;
        Ys1 = y1;
        Xs2 = x2;
        Ys2 = y2;
    }

    public (long X1, long Y1, long X2, long Y2) GetCoordinates() => (Xs1, Ys1, Xs2, Ys2);

    public abstract ShapeKind Kind { get; }

    public abstract string DisplayName { get; }

    public abstract void Show(ICanvas canvas, EditorSettings settings, bool isSelected = false);

    protected static Color GetStrokeColor(bool isSelected) => isSelected ? Colors.Red : Colors.Black;

    protected static Color GetFillColor(bool isSelected) => isSelected ? Colors.Red : Colors.Black;
}
