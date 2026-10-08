using Microsoft.Maui.Graphics;

namespace OOP.Shapes;

public sealed class EditorDrawable : IDrawable
{
    private readonly Func<ShapeObjectsEditor> _editorProvider;

    public EditorDrawable(Func<ShapeObjectsEditor> editorProvider)
    {
        _editorProvider = editorProvider;
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        _editorProvider().OnPaint(canvas, dirtyRect);
    }
}
