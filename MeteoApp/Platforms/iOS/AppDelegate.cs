using Foundation;
using UIKit;
using Firebase.Core;

namespace MeteoApp;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    public override bool FinishedLaunching(UIApplication app, NSDictionary options)
    {
        FirebaseApp.Configure();
        return base.FinishedLaunching(app, options);
    }
}

