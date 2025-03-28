using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MeteoApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

   
        var configuration = new ConfigurationBuilder()
            .SetBasePath(FileSystem.AppDataDirectory) 
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();

        builder.Configuration.AddConfiguration(configuration);

        builder.Services.AddSingleton<MeteoService>();


#if DEBUG
        builder.Logging.AddDebug();
#endif
        return builder.Build();
    }
}

