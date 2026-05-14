using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Lab3Graph;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        var variantInput = this.FindControl<TextBox>("VariantInput");
        var buildButton = this.FindControl<Button>("BuildButton");
        _directedCanvas = this.FindControl<GraphCanvas>("DirectedCanvas");
        _undirectedCanvas = this.FindControl<GraphCanvas>("UndirectedCanvas");
        _infoText = this.FindControl<TextBlock>("InfoText");
        _matrixText = this.FindControl<TextBlock>("MatrixText");

        buildButton!.Click += (_, _) => Rebuild();
        Loaded += (_, _) => Rebuild();
    }

    private GraphCanvas? _directedCanvas;
    private GraphCanvas? _undirectedCanvas;
    private TextBlock? _infoText;
    private TextBlock? _matrixText;

    private void Rebuild()
    {
        var variantInput = this.FindControl<TextBox>("VariantInput");
        var raw = variantInput?.Text?.Trim() ?? "";
        if (raw.Length != 4 || !raw.All(char.IsDigit))
        {
            _infoText!.Text = "Введите ровно 4 десятичные цифры.";
            return;
        }

        var d0 = raw[0] - '0';
        var d1 = raw[1] - '0';
        var d2 = raw[2] - '0';
        var d3 = raw[3] - '0';
        var cfg = new VariantConfig(d0, d1, d2, d3);
        if (cfg.VertexCount < 2)
        {
            _infoText!.Text = "По условию n = 10·n3; при n3 = 0 получается n = 0 — укажите n3 ≥ 1.";
            return;
        }

        var adir = MatrixGenerator.BuildDirected(cfg);
        var aund = MatrixGenerator.BuildUndirectedFromDirected(adir);
        var positions = VertexLayout.Compute(cfg.Layout, cfg.VertexCount);

        _directedCanvas!.Directed = true;
        _directedCanvas.Matrix = adir;
        _directedCanvas.Positions = positions;

        _undirectedCanvas!.Directed = false;
        _undirectedCanvas.Matrix = aund;
        _undirectedCanvas.Positions = positions;

        _matrixText!.Text =
            $"seed = {cfg.Seed},  n = {cfg.VertexCount},  k = {cfg.K:F6},  раскладка: {DescribeLayout(cfg.Layout)}\n\n" +
            "Adir:\n" + MatrixGenerator.FormatMatrix(adir) + "\n\nAundir:\n" + MatrixGenerator.FormatMatrix(aund);

        _infoText!.Text =
            $"n1={cfg.N1} n2={cfg.N2} n3={cfg.N3} n4={cfg.N4}  |  n=10·n3={cfg.VertexCount}  |  k={cfg.K:F4}";
    }

    private static string DescribeLayout(VertexArrangement a) => a switch
    {
        VertexArrangement.Circle => "окружность (n4=0,1)",
        VertexArrangement.Square => "прямоугольник по периметру (n4=2,3)",
        VertexArrangement.Triangle => "треугольник по периметру (n4=4,5)",
        VertexArrangement.CircleWithCenter => "окружность + центр (n4=6,7)",
        VertexArrangement.SquareWithCenter => "прямоугольник + центр (n4=8,9)",
        _ => a.ToString()
    };

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
