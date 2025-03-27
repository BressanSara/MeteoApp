using System.Collections.ObjectModel;
using System.Threading.Tasks;
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
            Locations = new ObservableCollection<MeteoLocation>();
        }

        public async Task InitializeLocationsAsync()
        {
            GPSOperations gpsOperations = new GPSOperations();
            var currentLocation = await gpsOperations.GetCurrentLocationAsync();
            
            if (currentLocation != null)
                Locations.Insert(0, currentLocation);
            
            for (var i = 1; i < 10; i++)
            {
                var e = new MeteoLocation
                {
                    Id = i,
                    Name = $"Location {i}",
                    Latitude = 0.0,
                    Longitude = 0.0,
                };

                Locations.Add(e);
            }
        }
    }
}
