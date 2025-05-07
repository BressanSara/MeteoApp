using MeteoApp.Models;
using MeteoApp.ViewModels;
using Plugin.Firebase.CloudMessaging;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using MeteoApp.Services;

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
            (BindingContext as HomePageViewModel)?.Locations.Remove(item);
        }
    }

    private async void OnAboutClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("about");
    }
}