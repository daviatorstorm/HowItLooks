using CommunityToolkit.Maui;
using HowItLooks.Services;
using Microsoft.Extensions.Logging;

namespace HowItLooks;

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
                fonts.AddFont("Bravo_Stencil.otf", "BravoSCT");
                fonts.AddFont("FluentSystemIcons-Filled.ttf", "FluentSystemIcons-Filled");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        builder.Services.AddTransient<StartupService>();
        builder.Services.AddTransient<DatabaseService>();
        builder.Services.AddTransient<FileLoggerService>();
        builder.Services.AddTransient<MigrationsService>();

        return builder.Build();
    }
}
