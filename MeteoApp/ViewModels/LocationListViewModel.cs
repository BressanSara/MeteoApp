using System.Collections.ObjectModel;
using MeteoApp.Models;

namespace MeteoApp.ViewModels
{
    class LocationListViewModel : BaseViewModel
    {
        private readonly LocationService _locationService;
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
            _locationService = new LocationService();
            Locations = new ObservableCollection<MeteoLocation>();
        }

        public async Task LoadLocationsAsync()
        {
            var locations = await _locationService.LoadLocationsAsync();
            Locations.Clear();
            foreach (var location in locations)
            {
                Locations.Add(location);
            }
        }

        public async Task AddLocationAsync(MeteoLocation location)
        {
            await _locationService.SaveLocationAsync(location);
            Locations.Add(location);
        }

        public async Task RemoveLocationAsync(MeteoLocation location)
        {
            await _locationService.DeleteLocationAsync(location.Id.ToString());
            Locations.Remove(location);
        }

        public async Task UpdateLocationAsync(MeteoLocation location)
        {
            await _locationService.UpdateLocationAsync(location.Id.ToString(), location);
            // Aggiorna la lista se necessario
        }
    }
}
