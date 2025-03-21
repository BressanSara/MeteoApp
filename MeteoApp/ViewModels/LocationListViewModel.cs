using System.Collections.ObjectModel;
using MeteoApp.Models;

namespace MeteoApp.ViewModels
{
    class LocationListViewModel : BaseViewModel
    {
        ObservableCollection<MeteoLocation> _locations;

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

            for (var i = 0; i < 10; i++)
            {
                var e = new MeteoLocation
                {
                    Id = i,
                    Name = $"Location {i}"
                };

                Locations.Add(e);
            }
        }
    }
}
