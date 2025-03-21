using MeteoApp.ViewModels;
using Location = MeteoApp.Models.Location;

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
        Routes.Add("locationdetails", typeof(MeteoItemPage));

        foreach (var item in Routes)
            Routing.RegisterRoute(item.Key, item.Value);
    }

    private void OnCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
    {
        if (selectionChangedEventArgs.CurrentSelection.Count > 0)
        {
            var location = selectionChangedEventArgs.CurrentSelection.FirstOrDefault() as Location;
            
            if(location == null)
                throw new ArgumentNullException("Location is null");

            var navigationParameter = new Dictionary<string, object>
            {
                { "Location", location }
            };

            Shell.Current.GoToAsync($"locationdetails", navigationParameter);
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
        var item = (sender as SwipeItem).BindingContext as Location;
        (BindingContext as LocationListViewModel).Locations.Remove(item);
    }
}