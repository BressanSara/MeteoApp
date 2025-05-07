using MeteoApp.Models;
using MeteoApp.ViewModels;
using MeteoApp.Converters;
using Microsoft.Maui.Controls;

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

        // Register converters
        Resources.Add("DateTimeConverter", new DateTimeConverter());
        Resources.Add("WeatherIconConverter", new WeatherIconConverter());
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

                // Load forecast data
                await viewModel.LoadForecastDataAsync(meteoService);
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during the request
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}