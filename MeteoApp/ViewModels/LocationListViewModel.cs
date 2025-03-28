using System.Collections.ObjectModel;
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
        }
    }
}
