using MeteoApp.Models;
using MeteoApp.ViewModels;
using Plugin.Firebase.CloudMessaging;

namespace MeteoApp;

public partial class LocationListView : Shell
{
    public Dictionary<string, Type> Routes { get; private set; } = new Dictionary<string, Type>();

    public LocationListView()
    {
        InitializeComponent();
        RegisterRoutes();

        BindingContext = new LocationListViewModel();
    }

    private void RegisterRoutes()
    {
        Routes.Add("locationdetails", typeof(LocationDetailsView));

        foreach (var item in Routes)
            Routing.RegisterRoute(item.Key, item.Value);
    }

    private async void OnCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
    {
        if (selectionChangedEventArgs.CurrentSelection.Count > 0)
        {
            var location = selectionChangedEventArgs.CurrentSelection.FirstOrDefault() as MeteoLocation;

            if (location == null)
                throw new ArgumentNullException("Location is null");

            var navigationParameter = new Dictionary<string, object>
        {
            { "MeteoLocation", location }
        };

            await Shell.Current.GoToAsync($"locationdetails", navigationParameter);
        }
    }

    private void OnItemAdded(object sender, EventArgs e)
    {
        _ = ShowPrompt();
    }

    private async Task ShowPrompt()
    {
        await DisplayAlert("Add City", "To Be Implemented", "OK");
    }

    private void SwipeItem_Invoked(object sender, EventArgs e)
    {
        var item = (sender as SwipeItem).BindingContext as MeteoLocation;
        (BindingContext as LocationListViewModel).Locations.Remove(item);
    }

    private async void GetToken(object sender, EventArgs e)
    {
#if ANDROID || IOS
        await CrossFirebaseCloudMessaging.Current.CheckIfValidAsync();
        var token = await CrossFirebaseCloudMessaging.Current.GetTokenAsync();
        await Share.RequestAsync(new ShareTextRequest
        {
            Text = token,
            Title = "Firebase Token"
        });
#endif
    }
}