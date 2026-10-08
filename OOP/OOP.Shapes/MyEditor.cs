namespace OOP.Shapes;


public sealed class MyEditor : ShapeObjectsEditor
{
    private static MyEditor? _instance;
    private static readonly object SyncRoot = new();

    private MyEditor(EditorSettings settings)
        : base(settings)
    {
    }

    public static MyEditor CreateDynamic() => new(EditorSettings.Lab4);

    public static MyEditor GetClassicSingleton()
    {
        lock (SyncRoot)
        {
            _instance ??= new MyEditor(EditorSettings.Lab4);
            return _instance;
        }
    }

    public static void ResetSingleton()
    {
        lock (SyncRoot)
            _instance = null;
    }
}
