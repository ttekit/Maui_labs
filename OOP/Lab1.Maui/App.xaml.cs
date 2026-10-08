using Microsoft.Extensions.DependencyInjection;

namespace Lab1;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		var mainPage = new MainPage();
		NavigationPage.SetHasNavigationBar(mainPage, false);

		return new Window(new NavigationPage(mainPage))
		{
			Title = "Lab1",
			Width = 800,
			Height = 600,
		};
	}
}