using System;
using System.ComponentModel;
using MeteoApp.Models;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Linq;

namespace MeteoApp.ViewModels
{
    public class LocationDetailsViewModel : INotifyPropertyChanged
    {
        private MeteoLocation _meteoLocation;
        private CurrentWeatherData _currentWeatherData;
        private string _iconString;
        private ObservableCollection<ForecastItem> _forecastItems;
        private bool _canAddLocation;
        private readonly LocationsViewModel _locationsViewModel;
        private readonly HomePageViewModel _homePageViewModel;

        public LocationDetailsViewModel(HomePageViewModel homePageViewModel = null)
        {
            _locationsViewModel = new LocationsViewModel();
            _homePageViewModel = homePageViewModel ?? new HomePageViewModel();
            _canAddLocation = false; 
        }

        public bool CanAddLocation
        {
            get => _canAddLocation;
            set
            {
                _canAddLocation = value;
                OnPropertyChanged();
            }
        }

        public MeteoLocation MeteoLocation
        {
            get => _meteoLocation;
            set
            {
                _meteoLocation = value;
                OnPropertyChanged();
                if (_meteoLocation != null)
                {
                    _ = CheckIfLocationExists();
                }
                else
                {
                    CanAddLocation = false;
                }
            }
        }

        public CurrentWeatherData CurrentWeatherData
        {
            get => _currentWeatherData;
            set
            {
                _currentWeatherData = value;
                OnPropertyChanged();
                if (_currentWeatherData?.Weather != null && _currentWeatherData.Weather.Count > 0)
                {
                    IconString = _currentWeatherData.Weather[0].Main.ToLower() + ".png";
                }
            }
        }

        public ObservableCollection<ForecastItem> ForecastItems
        {
            get => _forecastItems;
            set
            {
                _forecastItems = value;
                OnPropertyChanged();
            }
        }

        public string IconString
        {
            get => _iconString;
            set
            {
                _iconString = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async Task CheckIfLocationExists()
        {
            if (_meteoLocation == null) return;

            try
            {
                var locations = await _locationsViewModel.LoadLocationsAsync();
                var locationExists = locations.Any(l => 
                    Math.Abs(l.Latitude - _meteoLocation.Latitude) < 0.0001 && 
                    Math.Abs(l.Longitude - _meteoLocation.Longitude) < 0.0001);
                
                CanAddLocation = !locationExists;
                System.Diagnostics.Debug.WriteLine($"Location exists: {locationExists}, CanAddLocation: {CanAddLocation}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking if location exists: {ex.Message}");
                CanAddLocation = false; // In case of error, don't show the button
            }
        }

        public async Task AddLocationAsync()
        {
            if (_meteoLocation == null) return;

            try
            {
                await _locationsViewModel.AddLocationAsync(_meteoLocation);
                CanAddLocation = false;
                
                // Aggiorna la lista nella MainPageView
                await _homePageViewModel.ReloadWeatherDataAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error adding location: {ex.Message}");
                throw;
            }
        }

        public async Task LoadForecastDataAsync(MeteoService meteoService)
        {
            if (MeteoLocation != null)
            {
                var forecastData = await meteoService.GetForecastAsync(MeteoLocation);
                if (forecastData?.List != null)
                {
                    // Group forecast items by date and take the first item of each day
                    var dailyForecasts = forecastData.List
                        .GroupBy(x => DateTimeOffset.FromUnixTimeSeconds(x.Dt).Date)
                        .Select(g => g.First())
                        .ToList();

                    ForecastItems = new ObservableCollection<ForecastItem>(dailyForecasts);
                }
            }
        }
    }
}
