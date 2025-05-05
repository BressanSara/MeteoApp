using System;
using System.ComponentModel;
using MeteoApp.Models;
using System.Runtime.CompilerServices;

namespace MeteoApp.ViewModels
{
    public class LocationDetailsViewModel : INotifyPropertyChanged
    {
        private MeteoLocation _meteoLocation;
        private CurrentWeatherData _currentWeatherData;
        private string _iconString;

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
                    //IconString = _currentWeatherData.Weather[0].Icon;
                    IconString = _currentWeatherData.Weather[0].Main.ToLower() + ".png";
                }
            }
        }

        public string IconString
        {
            get => _iconString;
            set
            {
                _iconString = value;
                OnPropertyChanged();
                //OnPropertyChanged(nameof(IconUrl));
            }
        }

        //public string IconUrl => $"https://openweathermap.org/img/wn/{IconString}@2x.png";

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
