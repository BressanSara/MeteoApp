using System.Collections.ObjectModel;
using MeteoApp.Models;
using Location = MeteoApp.Models.Location;

namespace MeteoApp.ViewModels
{
    class LocationListViewModel : BaseViewModel
    {
        ObservableCollection<Location> _locations;

        public ObservableCollection<Location> Locations
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
            Locations = new ObservableCollection<Location>();

            for (var i = 0; i < 10; i++)
            {
                var e = new Location
                {
                    Id = i,
                    Name = $"Location {i}"
                };

                Locations.Add(e);
            }
        }
    }
}
