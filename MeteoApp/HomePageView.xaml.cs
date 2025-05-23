using MeteoApp.Models;
using MeteoApp.ViewModels;
using Plugin.Firebase.CloudMessaging;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using MeteoApp.Services;
using System;
using System.Linq;

namespace MeteoApp;

public partial class HomePageView : Shell
{
    public Dictionary<string, Type> Routes { get; private set; } = new Dictionary<string, Type>();

    public HomePageView()
    {
        InitializeComponent();
        RegisterRoutes();

        DialogService.Instance.Initialize(this);

        BindingContext = new HomePageViewModel();
    }

    private void RegisterRoutes()
    {
        Routes.Add("locationdetails", typeof(LocationDetailsView));
        Routes.Add("ReminderList", typeof(ReminderListView));
        Routes.Add("about", typeof(BlazorHostPage));
        Routes.Add("locationsearch", typeof(LocationSearchPage));

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

            if (sender is CollectionView collectionView)
            {
                collectionView.SelectedItem = null;
            }
        }
    }

    private async void OnMapClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MapPage());
    }

    private async Task ShowPrompt(string message)
    {
        await DialogService.Instance.ShowAlert("To be implemented", message, "OK");
    }

    private async void OpenNotificationSetting(object sender, EventArgs e)
    {
#if IOS
        await DialogService.Instance.ShowAlert("Not implemented", "Functionality not enabled in IOS", "OK");
#elif ANDROID
        await Shell.Current.GoToAsync($"ReminderList");
#endif
    }

    private async void SwipeItem_Invoked(object sender, EventArgs e)
    {
        var item = (sender as SwipeItem).BindingContext as MeteoLocation;

        if (item == null)
            return;

        if (await DialogService.Instance.ShowConfirmation("Delete", "Are you sure you want to delete this location?"))
        {
            var vm = new LocationsViewModel();
            await vm.DeleteLocationAsync(item);

            if (BindingContext is HomePageViewModel viewModel)
            {
                viewModel.Locations.Remove(item);

                // Check if the deleted location was the current location
                if (viewModel.CurrentLocation != null &&
                    Math.Abs(viewModel.CurrentLocation.Latitude - item.Latitude) < 0.0001 &&
                    Math.Abs(viewModel.CurrentLocation.Longitude - item.Longitude) < 0.0001)
                {
                    viewModel.CanAddCurrentLocation = true;
                }
            }
        }
    }

    private async void OnAboutClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("about");
    }

    private async void OnAddLocationClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("locationsearch");
    }

    private async void OnUpdateClicked(object sender, EventArgs e)
    {
        if (BindingContext is HomePageViewModel viewModel)
        {
            await viewModel.ReloadWeatherDataAsync();
        }
    }

    private async void OnAddCurrentLocationClicked(object sender, EventArgs e)
    {
        try
        {
            if (BindingContext is HomePageViewModel viewModel)
            {
                await viewModel.AddCurrentLocationAsync();
                await DisplayAlert("Success", "Current location added successfully!", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "Failed to add current location: " + ex.Message, "OK");
        }
    }
}