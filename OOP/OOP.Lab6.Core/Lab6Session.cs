namespace OOP.Lab6.Core;

public sealed class Lab6Session
{
    public const int DefaultSeed = 42;

    public static string ParamsFilePath { get; } = Path.Combine(Path.GetTempPath(), "oop_lab6_params.txt");

    public static string PointsFilePath { get; } = Path.Combine(Path.GetTempPath(), "oop_lab6_points.txt");

    public static string ReadyFilePath { get; } = Path.Combine(Path.GetTempPath(), "oop_lab6_object2.ready");

    public int NPoints { get; init; }

    public int XMin { get; init; }

    public int XMax { get; init; }

    public int YMin { get; init; }

    public int YMax { get; init; }

    public static Lab6Session LoadFromParamsFile()
    {
        if (!File.Exists(ParamsFilePath))
            return Default();

        return ParseFile(File.ReadAllText(ParamsFilePath));
    }

    public static Lab6Session Default() => new()
    {
        NPoints = 10,
        XMin = 0,
        XMax = 100,
        YMin = 0,
        YMax = 100,
    };

    public static Lab6Session Parse(string[] args)
    {
        string? file = GetArg(args, "--params-file");
        if (file is not null && File.Exists(file))
            return ParseFile(File.ReadAllText(file));

        if (File.Exists(ParamsFilePath))
            return LoadFromParamsFile();

        return new Lab6Session
        {
            NPoints = int.Parse(GetArg(args, "--n") ?? "10"),
            XMin = int.Parse(GetArg(args, "--xmin") ?? "0"),
            XMax = int.Parse(GetArg(args, "--xmax") ?? "100"),
            YMin = int.Parse(GetArg(args, "--ymin") ?? "0"),
            YMax = int.Parse(GetArg(args, "--ymax") ?? "100"),
        };
    }

    public static Lab6Session ParseFile(string content)
    {
        string[] lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return new Lab6Session
        {
            NPoints = int.Parse(lines[0]),
            XMin = int.Parse(lines[1]),
            XMax = int.Parse(lines[2]),
            YMin = int.Parse(lines[3]),
            YMax = int.Parse(lines[4]),
        };
    }

    public async Task WriteParamsFileAsync()
    {
        string content = $"{NPoints}\n{XMin}\n{XMax}\n{YMin}\n{YMax}";
        await File.WriteAllTextAsync(ParamsFilePath, content).ConfigureAwait(false);
    }

    public async Task WritePointsFileAsync(IReadOnlyList<(int X, int Y)> points)
    {
        await File.WriteAllTextAsync(PointsFilePath, FormatClipboard(points)).ConfigureAwait(false);
    }

    public static IReadOnlyList<(int X, int Y)> ReadPointsFile()
    {
        if (!File.Exists(PointsFilePath))
            return Array.Empty<(int, int)>();

        return ParsePointsText(File.ReadAllText(PointsFilePath));
    }

    public static IReadOnlyList<(int X, int Y)> ReadPointsFile(string filePath)
    {
        if (!File.Exists(filePath))
            return Array.Empty<(int, int)>();

        return ParsePointsText(File.ReadAllText(filePath));
    }

    public static IReadOnlyList<(int X, int Y)> ParsePointsText(string text) => ParseClipboard(text);

    public IReadOnlyList<(int X, int Y)> GeneratePoints(Random random)
    {
        var points = new List<(int X, int Y)>(NPoints);
        for (int i = 0; i < NPoints; i++)
        {
            int x = random.Next(XMin, XMax + 1);
            int y = random.Next(YMin, YMax + 1);
            points.Add((x, y));
        }

        points.Sort((a, b) => a.X.CompareTo(b.X));
        return points;
    }

    public static string FormatClipboard(IReadOnlyList<(int X, int Y)> points)
    {
        var lines = points.Select(p => $"{p.X}\t{p.Y}");
        return string.Join(Environment.NewLine, lines);
    }

    public static IReadOnlyList<(int X, int Y)> ParseClipboard(string text)
    {
        var points = new List<(int X, int Y)>();
        foreach (string line in text.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            string[] parts = line.Split('\t', ' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2)
                continue;

            if (int.TryParse(parts[0], out int x) && int.TryParse(parts[1], out int y))
                points.Add((x, y));
        }

        points.Sort((a, b) => a.X.CompareTo(b.X));
        return points;
    }

    private static string? GetArg(string[] args, string name)
    {
        for (int i = 0; i < args.Length - 1; i++)
        {
            if (string.Equals(args[i], name, StringComparison.OrdinalIgnoreCase))
                return args[i + 1].Trim('"');
        }

        return null;
    }
}
