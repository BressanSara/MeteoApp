using MeteoApp.Models;
using MeteoApp.ViewModels;

namespace MeteoApp;

public partial class LocationListView : Shell
{
    public Dictionary<string, Type> Routes { get; private set; } = new Dictionary<string, Type>();

    public LocationListView()
	{
		InitializeComponent();
        RegisterRoutes();

        var viewModel = new LocationListViewModel();
        BindingContext = viewModel;
        _ = Initialize(viewModel);

    }

    private void RegisterRoutes()
    {
        Routes.Add("locationdetails", typeof(LocationDetailsView));

        foreach (var item in Routes)
            Routing.RegisterRoute(item.Key, item.Value);
    }

    private async Task Initialize(LocationListViewModel viewModel)
    {
        await viewModel.InitializeLocationsAsync();
    }

    private void OnCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
    {
        if (selectionChangedEventArgs.CurrentSelection.Count > 0)
        {
            var location = selectionChangedEventArgs.CurrentSelection.FirstOrDefault() as MeteoLocation;

            if (location == null)
            {
                throw new ArgumentNullException("Location is null");
                Console.WriteLine("Location is null");
            }

            
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
}