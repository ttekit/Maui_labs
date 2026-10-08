using OOP.Shapes.Shapes;

namespace OOP.Shapes;


public static class ShapeFileService
{
    public const string FileExtension = "coolPaint";

    private const char FieldSeparator = '\t';

    public static Task ClearAsync(string filePath)
    {
        if (File.Exists(filePath))
            File.Delete(filePath);

        return Task.CompletedTask;
    }

    public static async Task AppendShapeAsync(string filePath, Shape shape)
    {
        await File.AppendAllTextAsync(filePath, FormatShapeLine(shape), CancellationToken.None).ConfigureAwait(false);
    }

    public static async Task SaveAllAsync(string filePath, IEnumerable<Shape> shapes)
    {
        await File.WriteAllTextAsync(filePath, BuildFileContent(shapes), CancellationToken.None).ConfigureAwait(false);
    }

    public static string BuildFileContent(IEnumerable<Shape> shapes) =>
        string.Concat(shapes.Select(FormatShapeLine));

    private static string FormatShapeLine(Shape shape)
    {
        var (x1, y1, x2, y2) = shape.GetCoordinates();
        return $"{shape.DisplayName}{FieldSeparator}{x1}{FieldSeparator}{y1}{FieldSeparator}{x2}{FieldSeparator}{y2}{Environment.NewLine}";
    }

    public static async Task<IReadOnlyList<ShapeRecord>> LoadAsync(string filePath)
    {
        if (!File.Exists(filePath))
            return Array.Empty<ShapeRecord>();

        string[] lines = await File.ReadAllLinesAsync(filePath).ConfigureAwait(false);
        var records = new List<ShapeRecord>();
        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            string[] parts = line.Split(FieldSeparator, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 5)
            {
                parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 5)
                    continue;
            }

            if (!long.TryParse(parts[^4], out long x1) ||
                !long.TryParse(parts[^3], out long y1) ||
                !long.TryParse(parts[^2], out long x2) ||
                !long.TryParse(parts[^1], out long y2))
                continue;

            string name = string.Join(FieldSeparator, parts[..^4]);
            records.Add(new ShapeRecord(name, x1, y1, x2, y2));
        }

        return records;
    }

    public static Shape CreateShapeFromRecord(ShapeRecord record) => record.Name switch
    {
        "Крапка" => Create<PointShape>(record),
        "Малювання" => Create<FreehandShape>(record),
        "Лінія" => Create<LineShape>(record),
        "Прямокутник" => Create<RectShape>(record),
        "Еліпс" => Create<EllipseShape>(record),
        "Лінія з кружечками" => Create<LineOOShape>(record),
        "Каркас куба" => Create<CubeShape>(record),
        _ => Create<LineShape>(record),
    };

    private static Shape Create<T>(ShapeRecord record) where T : Shape, new()
    {
        var shape = new T();
        shape.Set(record.X1, record.Y1, record.X2, record.Y2);
        return shape;
    }
}

public readonly record struct ShapeRecord(string Name, long X1, long Y1, long X2, long Y2);
