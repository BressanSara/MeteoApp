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
    private readonly LocationsViewModel locationsViewModel;
    private bool isNewLocation = true;

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
        locationsViewModel = new LocationsViewModel();

        // Register converters
        Resources.Add("DateTimeConverter", new DateTimeConverter());
        Resources.Add("WeatherIconConverter", new WeatherIconConverter());
    }

    private async void SaveLocationIfNew()
    {
        if (isNewLocation && MeteoLocation != null)
        {
            try
            {
                await locationsViewModel.AddLocationAsync(MeteoLocation);
                isNewLocation = false;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Failed to save location: " + ex.Message, "OK");
            }
        }
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

    private async void OnAddLocationClicked(object sender, EventArgs e)
    {
        try
        {
            await viewModel.AddLocationAsync();
            await DisplayAlert("Success", "Location added successfully!", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "Failed to add location: " + ex.Message, "OK");
        }
    }
}
