using System.Collections.ObjectModel;
using System.Diagnostics;
using MeteoApp.Models;

namespace MeteoApp.ViewModels
{
    class LocationListViewModel : BaseViewModel
    {
        private ObservableCollection<MeteoLocation> _locations;

        public ObservableCollection<MeteoLocation> Locations
        {
            get { return _locations; }
            set
            {
                _locations = value;
                OnPropertyChanged();
            }
        }

        private MeteoLocation _currentLocation;

        public MeteoLocation CurrentLocation
        {
            get { return _currentLocation; }
            set
            {
                _currentLocation = value;
                _currentLocation.Coord = new Coord { 
                    lat = _currentLocation.Latitude,
                    lon = _currentLocation.Longitude
                };
                OnPropertyChanged();
            }
        }

        private CurrentWeatherData _currentLocationWeatherData;

        public CurrentWeatherData CurrentLocationWeatherData
        {
            get { return _currentLocationWeatherData; }
            set
            {
                _currentLocationWeatherData = value;
                OnPropertyChanged();
                if (_currentLocationWeatherData?.Weather != null && _currentLocationWeatherData.Weather.Count > 0)
                {
                    IconWeather = _currentLocationWeatherData.Weather[0].Icon;
                }
            }
        }

        private string _iconWeather;

        public string IconWeather
        {
            get => _iconWeather;
            set
            {
                _iconWeather = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IconWeatherUrl));
            }
        }

        public string IconWeatherUrl => $"https://openweathermap.org/img/wn/{IconWeather}@2x.png";

        public LocationListViewModel()
        {
            Locations = new ObservableCollection<MeteoLocation>
            {
                new MeteoLocation
                {
                    Id = 1,
                    Name = "New York, USA",
                    Coord = new Coord
                    {
                        lat = 40.7128,
                        lon = -74.0060
                    }
                },
                new MeteoLocation
                {
                    Id = 2,
                    Name = "Tokyo, Japan",
                    Coord = new Coord
                    {
                        lat = 35.6895,
                        lon = 139.6917
                    }
                },
                new MeteoLocation
                {
                    Id = 3,
                    Name = "Sydney, Australia",
                    Coord = new Coord
                    {
                        lat = -33.8688,
                        lon = 151.2093
                    }
                },
                new MeteoLocation
                {
                    Id = 4,
                    Name = "Cape Town, South Africa",
                    Coord = new Coord
                    {
                        lat = -33.9249,
                        lon = 18.4241
                    }
                },
                new MeteoLocation
                {
                    Id = 5,
                    Name = "Paris, France",
                    Coord = new Coord
                    {
                        lat = 48.8566,
                        lon = 2.3522
                    }
                }
            };

            _ = LoadWeatherDataAsync();
        }

        private async Task LoadWeatherDataAsync()
        {
            try
            {
                var meteoService = new MeteoService(new HttpClient());
                GPSOperations gpsOperations = new GPSOperations();

                CurrentLocation = await gpsOperations.GetCurrentLocationAsync();

                if (CurrentLocation != null)
                {
                    CurrentLocationWeatherData = await meteoService.GetWeatherAsync(CurrentLocation);

                    if (CurrentLocationWeatherData == null)
                    {
                        Debug.WriteLine("Impossibile ottenere i dati meteo per la posizione corrente.");
                    }
                }
                else
                {
                    Debug.WriteLine("Impossibile ottenere la posizione corrente.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Errore durante il caricamento dei dati meteo: {ex.Message}");
            }
            finally
            {
                OnPropertyChanged(nameof(CurrentLocation));
                OnPropertyChanged(nameof(CurrentLocationWeatherData));
            }
        }
    }

}