using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Maui;
using System.Reflection.Metadata;
using Plugin.Firebase.CloudMessaging;
using Microsoft.Maui.LifecycleEvents;


namespace MeteoApp;

#if IOS
using Plugin.Firebase.Core.Platforms.iOS;
#elif ANDROID
using Plugin.Firebase.Core.Platforms.Android;
#endif

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
            }).ConfigureLifecycleEvents(events => {
#if IOS
        events.AddiOS(iOS => iOS.WillFinishLaunching((_, __) => {
            CrossFirebase.Initialize();
            FirebaseCloudMessagingImplementation.Initialize();
            return false;
        }));
#elif ANDROID
        events.AddAndroid(android => android.OnCreate((activity, _) =>
    {
        CrossFirebase.Initialize(activity);
    }));
#endif
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        PrintToken();

        return builder.Build();
    }

    private static async void PrintToken()
    {
        try
        {
            await CrossFirebaseCloudMessaging.Current.CheckIfValidAsync();
            var token = await CrossFirebaseCloudMessaging.Current.GetTokenAsync();
            Console.WriteLine($"FCM token: {token}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Errore durante l'esecuzione delle task asincrone: {ex.Message}");
        }
    }
}