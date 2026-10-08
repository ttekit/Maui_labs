using Microsoft.Maui.Graphics;

namespace OOP.Shapes;

public enum RectInputMode
{
    OppositeCorners,
    CenterToCorner,
}

public enum EllipseInputMode
{
    OppositeCorners,
    CenterToCorner,
}

public enum RectDisplayStyle
{
    OutlineOnly,
    WhiteFill,
    ColoredFill,
}

public enum EllipseDisplayStyle
{
    OutlineOnly,
    WhiteFill,
    ColoredFill,
}


public sealed class EditorSettings
{
    public required int ArrayCapacity { get; init; }

    public required Color RubberBandColor { get; init; }

    public required bool RubberBandDashed { get; init; }

    public required RectInputMode RectInput { get; init; }

    public required RectDisplayStyle RectDisplay { get; init; }

    public required Color RectFillColor { get; init; }

    public required EllipseInputMode EllipseInput { get; init; }

    public required EllipseDisplayStyle EllipseDisplay { get; init; }

    public required Color EllipseFillColor { get; init; }

    public required bool UseMenuModeIndicator { get; init; }

    public bool UseTitleModeIndicator => !UseMenuModeIndicator;

    public required bool UseFreehandDrawing { get; init; }

    public static EditorSettings Lab2 { get; } = new()
    {
        ArrayCapacity = 104,
        RubberBandColor = Colors.Black,
        RubberBandDashed = false,
        RectInput = RectInputMode.OppositeCorners,
        RectDisplay = RectDisplayStyle.OutlineOnly,
        RectFillColor = Colors.Orange,
        EllipseInput = EllipseInputMode.CenterToCorner,
        EllipseDisplay = EllipseDisplayStyle.ColoredFill,
        EllipseFillColor = Colors.Pink,
        UseMenuModeIndicator = true,
        UseFreehandDrawing = false,
    };

    public static EditorSettings Lab3 { get; } = new()
    {
        ArrayCapacity = 105,
        RubberBandColor = Colors.Red,
        RubberBandDashed = false,
        RectInput = RectInputMode.CenterToCorner,
        RectDisplay = RectDisplayStyle.WhiteFill,
        RectFillColor = Colors.Gray,
        EllipseInput = EllipseInputMode.OppositeCorners,
        EllipseDisplay = EllipseDisplayStyle.OutlineOnly,
        EllipseFillColor = Colors.Gray,
        UseMenuModeIndicator = false,
        UseFreehandDrawing = false,
    };

    public static EditorSettings Lab4 { get; } = new()
    {
        ArrayCapacity = Lab3.ArrayCapacity,
        RubberBandColor = Lab3.RubberBandColor,
        RubberBandDashed = true,
        RectInput = Lab3.RectInput,
        RectDisplay = Lab3.RectDisplay,
        RectFillColor = Lab3.RectFillColor,
        EllipseInput = Lab3.EllipseInput,
        EllipseDisplay = Lab3.EllipseDisplay,
        EllipseFillColor = Lab3.EllipseFillColor,
        UseMenuModeIndicator = Lab3.UseMenuModeIndicator,
        UseFreehandDrawing = false,
    };
}
