using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using OOP.MyTable;
using OOP.Shapes;

namespace Lab5;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddSingleton(_ => MyEditor.GetClassicSingleton());
        builder.Services.AddSingleton<MyTable>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
