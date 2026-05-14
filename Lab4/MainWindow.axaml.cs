using System.Text;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Lab4Graph;

public partial class MainWindow : Window
{
    private GraphCanvas? _directed1;
    private GraphCanvas? _undirected1;
    private GraphCanvas? _directed2;
    private GraphCanvas? _condensation;
    private TextBlock? _infoText;
    private TextBlock? _reportPhase1;
    private TextBlock? _reportPhase2;

    public MainWindow()
    {
        InitializeComponent();
        _directed1 = this.FindControl<GraphCanvas>("DirectedCanvas1");
        _undirected1 = this.FindControl<GraphCanvas>("UndirectedCanvas1");
        _directed2 = this.FindControl<GraphCanvas>("DirectedCanvas2");
        _condensation = this.FindControl<GraphCanvas>("CondensationCanvas");
        _infoText = this.FindControl<TextBlock>("InfoText");
        _reportPhase1 = this.FindControl<TextBlock>("ReportPhase1");
        _reportPhase2 = this.FindControl<TextBlock>("ReportPhase2");

        this.FindControl<Button>("BuildButton")!.Click += (_, _) => Rebuild();
        Loaded += (_, _) => Rebuild();
    }

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

        var adir1 = MatrixGenerator.BuildDirected(cfg, cfg.KPhase1);
        var aund1 = MatrixGenerator.BuildUndirectedFromDirected(adir1);
        var positions = VertexLayout.Compute(cfg.Layout, cfg.VertexCount);

        _directed1!.Directed = true;
        _directed1.Matrix = adir1;
        _directed1.Positions = positions;
        _directed1.VertexLabels = null;

        _undirected1!.Directed = false;
        _undirected1.Matrix = aund1;
        _undirected1.Positions = positions;
        _undirected1.VertexLabels = null;

        var degUnd = GraphAnalysis.UndirectedDegrees(aund1);
        var out1 = GraphAnalysis.OutDegrees(adir1);
        var in1 = GraphAnalysis.InDegrees(adir1);

        var report1 = BuildPhase1Text(cfg, adir1, aund1, degUnd, out1, in1);
        _reportPhase1!.Text = report1;

        var adir2 = MatrixGenerator.BuildDirected(cfg, cfg.KPhase2);
        _directed2!.Directed = true;
        _directed2.Matrix = adir2;
        _directed2.Positions = positions;
        _directed2.VertexLabels = null;

        var out2 = GraphAnalysis.OutDegrees(adir2);
        var in2 = GraphAnalysis.InDegrees(adir2);
        var a2 = GraphAnalysis.MatrixPowerCount(adir2, 2);
        var a3 = GraphAnalysis.MatrixPowerCount(adir2, 3);
        var paths2 = GraphAnalysis.PathsLength2(adir2);
        var paths3 = GraphAnalysis.PathsLength3(adir2);
        var reach = GraphAnalysis.TransitiveClosure(adir2);
        var strong = GraphAnalysis.StrongConnectivityMatrix(reach);
        var comps = GraphAnalysis.StronglyConnectedComponents(strong);
        var (condAdj, _) = GraphAnalysis.Condensation(adir2, comps);

        var c = comps.Count;
        var condPos = VertexLayout.Compute(VertexArrangement.Circle, c);
        var condLabels = new string[c];
        for (var i = 0; i < c; i++)
        {
            var verts = comps[i];
            var inner = verts.Count <= 10
                ? string.Join(",", verts.Select(v => v + 1))
                : string.Join(",", verts.Take(10).Select(v => v + 1)) + "…";
            condLabels[i] = $"K{i}:{{{inner}}}";
        }

        _condensation!.Directed = true;
        _condensation.Matrix = condAdj;
        _condensation.Positions = condPos;
        _condensation.VertexLabels = condLabels;

        var (report2Ui, report2Full) = BuildPhase2Text(
            cfg, adir2, out2, in2, a2, a3, paths2, paths3, reach, strong, comps, condAdj);

        _reportPhase2!.Text = report2Ui;
        _infoText!.Text =
            $"n1={cfg.N1} n2={cfg.N2} n3={cfg.N3} n4={cfg.N4}  |  n={cfg.VertexCount}  |  k₁={cfg.KPhase1:F4}  k₂={cfg.KPhase2:F4}";

        Console.WriteLine("========== Лаб. 4 — полный отчёт (этап 1–2) ==========");
        Console.WriteLine(report1);
        Console.WriteLine();
        Console.WriteLine("========== Лаб. 4 — полный отчёт (этап 3–4) ==========");
        Console.WriteLine(report2Full);
    }

    private static string BuildPhase1Text(
        VariantConfig cfg,
        int[,] adir,
        int[,] aund,
        int[] degUnd,
        int[] outDeg,
        int[] inDeg)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"seed = {cfg.Seed},  n = {cfg.VertexCount}");
        sb.AppendLine($"k₁ = 1.0 − n3·0.01 − n4·0.01 − 0.3 = {cfg.KPhase1:F6}");
        sb.AppendLine($"раскладка: {DescribeLayout(cfg.Layout)}");
        sb.AppendLine();
        sb.AppendLine("Матрица Adir:");
        sb.AppendLine(MatrixGenerator.FormatMatrix(adir));
        sb.AppendLine();
        sb.AppendLine("Матрица Aundir:");
        sb.AppendLine(MatrixGenerator.FormatMatrix(aund));
        sb.AppendLine();

        sb.AppendLine("1) Степени вершин неориентированного графа (сумма по строке Aundir), вершины 1…n:");
        sb.AppendLine(string.Join(", ", degUnd.Select((d, i) => $"deg({i + 1})={d}")));
        sb.AppendLine();
        sb.AppendLine("2) Полустепени орграфа: исхода d⁺(i), захода d⁻(i):");
        sb.AppendLine(string.Join(", ", Enumerable.Range(0, degUnd.Length).Select(i => $"d⁺({i + 1})={outDeg[i]}")));
        sb.AppendLine(string.Join(", ", Enumerable.Range(0, degUnd.Length).Select(i => $"d⁻({i + 1})={inDeg[i]}")));
        sb.AppendLine();

        if (GraphAnalysis.TryRegularUndirected(degUnd, out var ku))
            sb.AppendLine($"3) Неориентированный граф однородный (регулярный): степень r = {ku}.");
        else
            sb.AppendLine("3) Неориентированный граф не является однородным (степени вершин различны).");

        if (GraphAnalysis.TryRegularDirected(inDeg, outDeg, out var kin, out var kout))
            sb.AppendLine(
                $"   Орграф регулярный: d⁺ = {kout}, d⁻ = {kin} для всех вершин.");
        else
            sb.AppendLine("   Орграф не является регулярным (полустепени не совпадают для всех вершин).");
        sb.AppendLine();

        var isoU = Enumerable.Range(0, degUnd.Length).Where(i => degUnd[i] == 0).ToList();
        var hangU = Enumerable.Range(0, degUnd.Length).Where(i => degUnd[i] == 1).ToList();
        sb.AppendLine("4) Изолированные вершины (неориент.): " + FormatVertexList(isoU));
        sb.AppendLine("   Висячие вершины (степень 1, неориент.): " + FormatVertexList(hangU));

        var isoD = Enumerable.Range(0, inDeg.Length).Where(i => inDeg[i] == 0 && outDeg[i] == 0).ToList();
        var hangD = Enumerable.Range(0, inDeg.Length).Where(i => inDeg[i] + outDeg[i] == 1).ToList();
        sb.AppendLine("   Изолированные вершины (орграф): " + FormatVertexList(isoD));
        sb.AppendLine("   «Висячие» по сумме полустепеней (=1 дуга инцидентна): " + FormatVertexList(hangD));

        return sb.ToString();
    }

    private static (string uiText, string fullText) BuildPhase2Text(
        VariantConfig cfg,
        int[,] adir,
        int[] outDeg,
        int[] inDeg,
        int[,] a2,
        int[,] a3,
        List<string> paths2,
        List<string> paths3,
        int[,] reach,
        int[,] strong,
        List<List<int>> comps,
        int[,] condAdj)
    {
        const int maxUiChars = 480_000;
        var sbFull = new StringBuilder();
        sbFull.AppendLine($"k₂ = 1.0 − n3·0.005 − n4·0.005 − 0.27 = {cfg.KPhase2:F6}");
        sbFull.AppendLine();
        sbFull.AppendLine("Матрица Adir (новая):");
        sbFull.AppendLine(MatrixGenerator.FormatMatrix(adir));
        sbFull.AppendLine();

        sbFull.AppendLine("1) Полустепени исхода и захода (вершины 1…n):");
        sbFull.AppendLine(string.Join(", ", Enumerable.Range(0, outDeg.Length).Select(i => $"d⁺({i + 1})={outDeg[i]}")));
        sbFull.AppendLine(string.Join(", ", Enumerable.Range(0, inDeg.Length).Select(i => $"d⁻({i + 1})={inDeg[i]}")));
        sbFull.AppendLine();

        sbFull.AppendLine("Матрица A² (число ориентированных маршрутов длины 2):");
        sbFull.AppendLine(MatrixGenerator.FormatMatrix(a2));
        sbFull.AppendLine();
        sbFull.AppendLine($"Все пути длины 2 (всего {paths2.Count}), с промежуточной вершиной:");
        foreach (var p in paths2)
            sbFull.AppendLine(p);
        sbFull.AppendLine();

        sbFull.AppendLine("Матрица A³ (число ориентированных маршрутов длины 3):");
        sbFull.AppendLine(MatrixGenerator.FormatMatrix(a3));
        sbFull.AppendLine();
        sbFull.AppendLine($"Все пути длины 3 (всего {paths3.Count}), с промежуточными вершинами:");
        foreach (var p in paths3)
            sbFull.AppendLine(p);
        sbFull.AppendLine();

        sbFull.AppendLine("Матрица достижимости R (транзитивное замыкание, рефлексивное):");
        sbFull.AppendLine(MatrixGenerator.FormatMatrix(reach));
        sbFull.AppendLine();

        sbFull.AppendLine("Матрица сильной связности S (R[i,j]∧R[j,i]):");
        sbFull.AppendLine(MatrixGenerator.FormatMatrix(strong));
        sbFull.AppendLine();

        sbFull.AppendLine("Компоненты сильной связности (вершины 1…n):");
        for (var i = 0; i < comps.Count; i++)
            sbFull.AppendLine($"  K{i} = {{ {string.Join(", ", comps[i].Select(v => v + 1))} }}");
        sbFull.AppendLine();

        sbFull.AppendLine("Матрица смежности графа конденсации (узлы — КСС K0, K1, …):");
        sbFull.AppendLine(MatrixGenerator.FormatMatrix(condAdj));

        var full = sbFull.ToString();
        string ui;
        if (full.Length <= maxUiChars)
            ui = full;
        else
        {
            const int preview = 400;
            var sbUi = new StringBuilder();
            var i2paths = full.IndexOf("Все пути длины 2", StringComparison.Ordinal);
            var a3Header = full.IndexOf("Матрица A³", StringComparison.Ordinal);
            var i3paths = full.IndexOf("Все пути длины 3", StringComparison.Ordinal);
            var iR = full.IndexOf("Матрица достижимости", StringComparison.Ordinal);
            if (i2paths >= 0)
                sbUi.AppendLine(full[..i2paths].TrimEnd());
            sbUi.AppendLine($"Все пути длины 2 (всего {paths2.Count}). Первые {preview} маршрутов:");
            foreach (var p in paths2.Take(preview))
                sbUi.AppendLine(p);
            if (paths2.Count > preview)
                sbUi.AppendLine($"… и ещё {paths2.Count - preview}.");
            sbUi.AppendLine();
            if (a3Header >= 0 && i3paths > a3Header)
                sbUi.AppendLine(full[a3Header..i3paths].TrimEnd());
            sbUi.AppendLine($"Все пути длины 3 (всего {paths3.Count}). Первые {preview} маршрутов:");
            foreach (var p in paths3.Take(preview))
                sbUi.AppendLine(p);
            if (paths3.Count > preview)
                sbUi.AppendLine($"… и ещё {paths3.Count - preview} (полный список — в консоли).");
            sbUi.AppendLine();
            if (iR >= 0)
                sbUi.AppendLine(full[iR..].TrimStart());
            ui = sbUi.ToString();
            if (ui.Length > maxUiChars)
                ui = ui[..maxUiChars] + "\n… (обрезано)";
        }

        return (ui, full);
    }

    private static string FormatVertexList(List<int> xs) =>
        xs.Count == 0 ? "нет" : string.Join(", ", xs.Select(x => x + 1));

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
