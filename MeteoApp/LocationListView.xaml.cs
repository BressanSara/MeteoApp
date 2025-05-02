using MeteoApp.Models;
using MeteoApp.ViewModels;
using Plugin.Firebase.CloudMessaging;
using System.ComponentModel;
using System.Runtime.CompilerServices;

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
        Routes.Add("ReminderList", typeof(ReminderListView));

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
        _ = ShowPrompt("Add new city");
        //await Navigation.PushAsync(new ListPage());
    }

    private async void OnMapClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MapPage());
    }

    private async Task ShowPrompt(string message)
    {
        await DisplayAlert("To be implemented", message, "OK");
    }

    private async void OpenNotificationSetting(object sender, EventArgs e)
    {
#if IOS
        await DisplayAlert("Not implemented", "Functionality not enabled in IOS", "OK");
#elif ANDROID
        await Shell.Current.GoToAsync($"ReminderList");
#endif
    }

    private async void SwipeItem_Invoked(object sender, EventArgs e)
    {
        var item = (sender as SwipeItem).BindingContext as MeteoLocation;

        if (item == null)
            return;

        bool confirm = await DisplayAlert("Delete Location", $"Are you sure you want to delete {item.Name}?", "Yes", "No");

        if (confirm)
        {
            (BindingContext as LocationListViewModel)?.Locations.Remove(item);
        }
    }
}