using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using MeteoApp.Models;
using MeteoApp.Services;
using Microsoft.Maui.Controls;


namespace MeteoApp.ViewModels
{
    class HomePageViewModel : BaseViewModel
    {
        private bool _appwriteDownload = false;

        public bool AppwriteDownload
        {
            get => _appwriteDownload;
            set
            {
                _appwriteDownload = value;
                OnPropertyChanged();
            }
        }
        
        private Timer _weatherUpdateTimer;

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

        public HomePageViewModel()
        {
            _ = LoadLocationsAsync();
            _ = LoadWeatherDataAsync();
            StartWeatherUpdateTimer();
        }

        private void StartWeatherUpdateTimer()
        {
            if (_weatherUpdateTimer != null)
            {
                _weatherUpdateTimer.Dispose();
            }
            _weatherUpdateTimer = new Timer(
                async _ => await ReloadWeatherDataAsync(), 
                null, 
                TimeSpan.Zero, 
                TimeSpan.FromMinutes(5)
             );
        }

        public void StopWeatherUpdateTimer()
        {
            if (_weatherUpdateTimer != null)
            {
                _weatherUpdateTimer.Dispose();
                _weatherUpdateTimer = null;
            }
        }

        public async Task ReloadWeatherDataAsync()
        {
            try
            {
                await AppWriteService.InitializeAsync();
                await LoadWeatherDataAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Errore durante il reload dei dati meteo: {ex.Message}");
            }
        }

        private async Task LoadLocationsAsync()
        {
            try
            {
                Locations = await new LocationsViewModel().LoadLocationsAsync();
                AppwriteDownload = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                AppwriteDownload = false;
                throw;
            }
        }

        private async Task LoadWeatherDataAsync()
        {
            try
            {
                var meteoService = new MeteoService(new HttpClient());
                var gpsOperations = new GPSOperations();

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