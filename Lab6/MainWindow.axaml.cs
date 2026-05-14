using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;

namespace Lab6Graph;

public partial class MainWindow : Window
{
    private MstCanvas? _graphView;
    private TextBlock? _infoText;
    private TextBlock? _legendText;
    private TextBlock? _graphTitle;
    private TextBlock? _protocolText;
    private TextBlock? _stepCounter;
    private ScrollViewer? _protocolScroll;

    private int[,]? _w;
    private Point[]? _positions;
    private AdjacencyListGraph? _graph;
    private List<MstStep> _steps = new();
    private int _stepIndex;

    public MainWindow()
    {
        InitializeComponent();
        _graphView = this.FindControl<MstCanvas>("GraphView");
        _infoText = this.FindControl<TextBlock>("InfoText");
        _legendText = this.FindControl<TextBlock>("LegendText");
        _graphTitle = this.FindControl<TextBlock>("GraphTitle");
        _protocolText = this.FindControl<TextBlock>("ProtocolText");
        _stepCounter = this.FindControl<TextBlock>("StepCounter");
        _protocolScroll = this.FindControl<ScrollViewer>("ProtocolScroll");

        this.FindControl<Button>("BuildButton")!.Click += (_, _) => Rebuild();
        this.FindControl<Button>("StepBackButton")!.Click += (_, _) => StepDelta(-1);
        this.FindControl<Button>("StepFwdButton")!.Click += (_, _) => StepDelta(1);
        this.FindControl<Button>("ResetStepButton")!.Click += (_, _) => GoToStep(0);

        KeyDown += OnKeyDown;
        Loaded += (_, _) =>
        {
            Focus();
            Rebuild();
        };
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key is Key.Right or Key.Space)
        {
            StepDelta(1);
            e.Handled = true;
        }
        else if (e.Key == Key.Left)
        {
            StepDelta(-1);
            e.Handled = true;
        }
    }

    private void Rebuild()
    {
        var raw = this.FindControl<TextBox>("VariantInput")?.Text?.Trim() ?? "";
        if (raw.Length != 4 || !raw.All(char.IsDigit))
        {
            _infoText!.Text = "Введите ровно 4 десятичные цифры.";
            return;
        }

        var cfg = new VariantConfig(raw[0] - '0', raw[1] - '0', raw[2] - '0', raw[3] - '0');
        if (cfg.VertexCount < 2)
        {
            _infoText!.Text = "n = 10·n3; укажите n3 ≥ 1.";
            return;
        }

        var built = WeightGraphBuilder.Build(cfg);
        _w = built.W;
        _positions = VertexLayout.Compute(cfg.Layout, cfg.VertexCount);
        _graph = AdjacencyListGraph.FromWeightMatrix(_w);

        _graphView!.WeightMatrix = _w;
        _graphView.Positions = _positions;

        var algo = cfg.UseKruskal ? "Краскал" : "Прим";
        _graphTitle!.Text = $"Взвешенный граф и минимальный остов — {algo} (n4={cfg.N4}, {(cfg.UseKruskal ? "чётное" : "нечётное")})";
        _legendText!.Text =
            "Серые рёбра — исходный граф, зелёные — текущий остов, оранжевая пунктир — рассматриваемое ребро. У дуг указан вес wij. Граф в памяти — списки смежности (AdjacencyListGraph).";
        _infoText!.Text =
            $"n={cfg.VertexCount}, seed={cfg.Seed}, k={cfg.K:F6}. Алгоритм: {algo}.";

        _steps = cfg.UseKruskal
            ? KruskalMst.BuildSteps(_w, _graph)
            : PrimMst.BuildSteps(_w, _graph);

        DumpConsole(cfg, built);

        GoToStep(0);
    }

    private static void DumpConsole(VariantConfig cfg, WeightGraphBuilder.Result built)
    {
        Console.WriteLine("=== Лаб. 6 ===");
        Console.WriteLine($"n={cfg.VertexCount}, k={cfg.K:F6}");
        Console.WriteLine("Adir:");
        Console.WriteLine(WeightGraphBuilder.FormatIntMatrix(built.Adir));
        Console.WriteLine("Aundir:");
        Console.WriteLine(WeightGraphBuilder.FormatIntMatrix(built.Aundir));
        Console.WriteLine("W:");
        Console.WriteLine(WeightGraphBuilder.FormatIntMatrix(built.W));
    }

    private void StepDelta(int d) => GoToStep(_stepIndex + d);

    private void GoToStep(int index)
    {
        if (_steps.Count == 0 || _w == null)
        {
            _protocolText!.Text = "Нет шагов.";
            _stepCounter!.Text = "";
            _graphView!.MstEdges = new HashSet<(int, int)>();
            _graphView.CandidateEdge = null;
            return;
        }

        index = Math.Clamp(index, 0, _steps.Count - 1);
        _stepIndex = index;
        var st = _steps[index];
        _graphView!.MstEdges = st.MstEdges;
        _graphView.CandidateEdge = st.CandidateEdge;

        var lines = new List<string>(_stepIndex + 1);
        for (var i = 0; i <= _stepIndex; i++)
            lines.Add($"{i,4}. {_steps[i].Message}");
        _protocolText!.Text = string.Join(Environment.NewLine, lines);
        _stepCounter!.Text = $"Шаг {_stepIndex} / {_steps.Count - 1}";

        Dispatcher.UIThread.Post(() => _protocolScroll?.ScrollToEnd(), DispatcherPriority.Background);
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
