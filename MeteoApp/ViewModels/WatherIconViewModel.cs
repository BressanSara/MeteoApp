using MeteoApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeteoApp.ViewModels
{
    public class WeatherIconViewModel : BaseViewModel
    {
        private WeatherIcon _icon;
        public WeatherIcon Icon
        {
            get => _icon;
            set
            {
                _icon = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IconUrl));
            }
        }

        public string IconUrl => $"https://openweathermap.org/img/wn/{Icon?.Icon}@2x.png";
    }
}
