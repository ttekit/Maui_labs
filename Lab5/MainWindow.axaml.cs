using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;

namespace Lab5Graph;

public partial class MainWindow : Window
{
    private TraversalGraphCanvas? _graphView;
    private TextBlock? _infoText;
    private TextBlock? _protocolText;
    private TextBlock? _stepCounter;
    private ScrollViewer? _protocolScroll;
    private RadioButton? _modeBfs;
    private RadioButton? _modeDfs;

    private int[,]? _matrix;
    private Point[]? _positions;
    private List<TraversalStep> _steps = new();
    private int _stepIndex;

    public MainWindow()
    {
        InitializeComponent();
        _graphView = this.FindControl<TraversalGraphCanvas>("GraphView");
        _infoText = this.FindControl<TextBlock>("InfoText");
        _protocolText = this.FindControl<TextBlock>("ProtocolText");
        _stepCounter = this.FindControl<TextBlock>("StepCounter");
        _protocolScroll = this.FindControl<ScrollViewer>("ProtocolScroll");
        _modeBfs = this.FindControl<RadioButton>("ModeBfs");
        _modeDfs = this.FindControl<RadioButton>("ModeDfs");

        this.FindControl<Button>("BuildButton")!.Click += (_, _) => RebuildGraph();
        this.FindControl<Button>("StepBackButton")!.Click += (_, _) => StepDelta(-1);
        this.FindControl<Button>("StepFwdButton")!.Click += (_, _) => StepDelta(1);
        this.FindControl<Button>("ResetStepButton")!.Click += (_, _) => GoToStep(0);

        _modeBfs!.IsCheckedChanged += (_, _) => { if (_modeBfs.IsChecked == true) OnModeChanged(); };
        _modeDfs!.IsCheckedChanged += (_, _) => { if (_modeDfs.IsChecked == true) OnModeChanged(); };

        KeyDown += OnKeyDown;
        Loaded += (_, _) =>
        {
            Focus();
            RebuildGraph();
        };
    }

    private void OnModeChanged()
    {
        if (_matrix == null)
            return;
        RegenerateSteps();
        GoToStep(0);
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

    private void RebuildGraph()
    {
        var variantInput = this.FindControl<TextBox>("VariantInput");
        var raw = variantInput?.Text?.Trim() ?? "";
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

        _matrix = MatrixGenerator.BuildDirected(cfg);
        _positions = VertexLayout.Compute(cfg.Layout, cfg.VertexCount);
        _graphView!.Matrix = _matrix;
        _graphView.Positions = _positions;

        _infoText!.Text =
            $"n={cfg.VertexCount}, seed={cfg.Seed}, k={cfg.K:F6}, раскладка n4={cfg.N4}.";

        Console.WriteLine($"Лаб.5 Adir, n={cfg.VertexCount}, k={cfg.K:F6}");
        Console.WriteLine(MatrixGenerator.FormatMatrix(_matrix));

        RegenerateSteps();
        GoToStep(0);
    }

    private void RegenerateSteps()
    {
        if (_matrix == null)
        {
            _steps = new List<TraversalStep>();
            return;
        }

        _steps = _modeDfs!.IsChecked == true
            ? TraversalSimulator.BuildDfs(_matrix)
            : TraversalSimulator.BuildBfs(_matrix);
    }

    private void StepDelta(int delta)
    {
        GoToStep(_stepIndex + delta);
    }

    private void GoToStep(int index)
    {
        if (_steps.Count == 0)
        {
            _stepIndex = 0;
            _graphView!.VertexStates = null;
            _graphView.TreeEdges = null;
            _graphView.ActiveVertex = null;
            _protocolText!.Text = "Нет шагов.";
            _stepCounter!.Text = "";
            return;
        }

        index = Math.Clamp(index, 0, _steps.Count - 1);
        _stepIndex = index;
        var st = _steps[index];
        _graphView!.VertexStates = st.VertexStates;
        _graphView.TreeEdges = st.TreeEdges;
        _graphView.ActiveVertex = st.ActiveVertex;

        var lines = new List<string>(_stepIndex + 1);
        for (var i = 0; i <= _stepIndex; i++)
            lines.Add($"{i,4}. {_steps[i].Message}");
        _protocolText!.Text = string.Join(Environment.NewLine, lines);
        _stepCounter!.Text = $"Шаг {_stepIndex} / {_steps.Count - 1}";

        Dispatcher.UIThread.Post(() =>
        {
            _protocolScroll?.ScrollToEnd();
        }, DispatcherPriority.Background);
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
