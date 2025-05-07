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

        public MeteoLocation MeteoLocation
        {
            get => _meteoLocation;
            set
            {
                _meteoLocation = value;
                OnPropertyChanged();
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
