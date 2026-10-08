using OOP.Lab6.Core;

namespace Lab6.Pages;

public partial class Variant0ResultPage : TabbedPage
{
    public Variant0ResultPage(Lab6Session session, IReadOnlyList<(int X, int Y)> points)
    {
        InitializeComponent();
        ModalPageConfiguration.ConfigureAsDialog(this);
        ToolbarItems.Add(new ToolbarItem("Закрити", null, async () => await Navigation.PopModalAsync()));

        SummaryLabel.Text =
            $"nPoint={session.NPoints}, x:[{session.XMin}..{session.XMax}], y:[{session.YMin}..{session.YMax}]";
        PointsView.ItemsSource = points.Select(point => $"({point.X}, {point.Y})").ToList();
        InfoLabel.Text = $"Графік y=f(x): {points.Count} точок";
        GraphView.Drawable = new GraphDrawable(points);
    }
}
