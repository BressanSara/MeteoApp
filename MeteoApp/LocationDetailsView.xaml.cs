using MeteoApp.Models;
using MeteoApp.ViewModels;

namespace MeteoApp;

[QueryProperty(nameof(MeteoLocation), "MeteoLocation")]
public partial class LocationDetailsView : ContentPage
{
    private readonly LocationDetailsViewModel viewModel;
    private readonly MeteoService meteoService;

    public MeteoLocation MeteoLocation
    {
        get => viewModel.MeteoLocation;
        set
        {
            viewModel.MeteoLocation = value;
            OnPropertyChanged();
            LoadWeatherData();
        }
    }

    public LocationDetailsView()
    {
        InitializeComponent();
        viewModel = new LocationDetailsViewModel();
        BindingContext = viewModel;
        meteoService = new MeteoService(new HttpClient());
    }

    private async void LoadWeatherData()
    {
        if (MeteoLocation != null)
        {
            try
            {
                var weatherInfo = await meteoService.GetWeatherAsync(MeteoLocation);
                if (weatherInfo != null)
                {
                    viewModel.CurrentWeatherData = weatherInfo;
                }
                else
                {
                    // Handle the case where weatherInfo is null
                    Console.WriteLine("Failed to retrieve weather information.");
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during the request
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}
