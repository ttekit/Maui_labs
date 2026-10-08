namespace OOP.Shapes.Editors;

public abstract class Editor
{
    public abstract void OnLeftButtonDown(float x, float y);
    public abstract void OnLeftButtonUp(float x, float y);
    public abstract void OnMouseMove(float x, float y);
}
