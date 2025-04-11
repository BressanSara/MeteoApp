using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using AndroidX.Core.App;
using Plugin.Firebase.CloudMessaging;

namespace MeteoApp;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        HandleIntent(Intent);
        CreateNotificationChannelIfNeeded();
        CheckAndRequestNotificationPermission();

    }

    public void CheckAndRequestNotificationPermission()
    {
        // Verifica se le notifiche sono abilitate
        var notificationManager = NotificationManagerCompat.From(this);
        if (!notificationManager.AreNotificationsEnabled())
        {
            // Mostra un messaggio o reindirizza l'utente alle impostazioni
            Intent intent = new Intent(Android.Provider.Settings.ActionAppNotificationSettings);
            intent.PutExtra(Android.Provider.Settings.ExtraAppPackage, PackageName);
            StartActivity(intent);
        }

        if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Tiramisu)
        {
            if (CheckSelfPermission(Android.Manifest.Permission.PostNotifications) != Android.Content.PM.Permission.Granted)
            {
                RequestPermissions(new[] { Android.Manifest.Permission.PostNotifications }, 0);
            }
        }
    }

    protected override void OnNewIntent(Intent intent)
    {
        base.OnNewIntent(intent);
        HandleIntent(intent);
    }

    private static void HandleIntent(Intent intent)
    {
        FirebaseCloudMessagingImplementation.OnNewIntent(intent);
    }

    private void CreateNotificationChannelIfNeeded()
    {
        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
        {
            CreateNotificationChannel();
        }
    }

    private void CreateNotificationChannel()
    {
        var channelId = $"{PackageName}.general";
        var notificationManager = (NotificationManager)GetSystemService(NotificationService);
        var channel = new NotificationChannel(channelId, "General", NotificationImportance.Default);
        notificationManager.CreateNotificationChannel(channel);
        FirebaseCloudMessagingImplementation.ChannelId = channelId;
    }
}

