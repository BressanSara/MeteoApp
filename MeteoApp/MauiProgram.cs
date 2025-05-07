using MeteoApp.Services;
using MeteoApp.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.LifecycleEvents;
using Plugin.Firebase.CloudMessaging;

#if ANDROID
using Plugin.Firebase.Core.Platforms.Android;
#endif

namespace MeteoApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .RegisterFirebaseServices()
            .UseMauiMaps()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });


        builder.Services.AddSingleton<ApiKeyProvider>();
        builder.Services.AddSingleton<MeteoService>();
        builder.Services.AddSingleton<HomePageViewModel>();
        builder.Services.AddSingleton<GeoCodingService>();

        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
#endif


#if DEBUG
        builder.Logging.AddDebug();
#endif

#if ANDROID
#endif
        return builder.Build();
    }

    private static MauiAppBuilder RegisterFirebaseServices(this MauiAppBuilder builder)
    {
        builder.ConfigureLifecycleEvents(events => {

#if ANDROID
        events.AddAndroid(android => android.OnCreate((activity, _) =>
        CrossFirebase.Initialize(activity)));
#endif
        });

        var notificationService = new NotificationService();
        Task.Run(async () => await notificationService.RegisterTokenAsync());

        return builder;
    }

}

