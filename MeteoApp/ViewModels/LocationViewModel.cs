using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using MeteoApp.Models;
using MeteoApp.Services;

namespace MeteoApp.ViewModels;

public class LocationsViewModel
{
    public async Task<ObservableCollection<MeteoLocation>> LoadLocationsAsync()
    {
        await AppWriteService.InitializeAsync();
        var appwriteResponse = await AppWriteService
            .Database
            .ListDocuments(
                databaseId: AppWriteService.DatabaseId, 
                collectionId: AppWriteService.CollectionId
                );

        if (appwriteResponse.Documents.Count != 0)
        {
            var locations = new ObservableCollection<MeteoLocation>();

            foreach (var doc in appwriteResponse.Documents)
            {
                var latitude = Convert.ToDouble(doc.Data["Latitude"]);
                var longitude = Convert.ToDouble(doc.Data["Longitude"]);
                
                locations.Add(new MeteoLocation
                {
                    Name = doc.Data["CityName"].ToString(),
                    Country = doc.Data["CountryName"].ToString(),
                    Latitude = latitude,
                    Longitude = longitude,
                    Coord = new Coord{lat = latitude, lon = longitude}
                });
            }
        
            return locations;
        }

        return new ObservableCollection<MeteoLocation>
        {
            new MeteoLocation
            {
                Name = "Roma",
                Country = "Italy",
                Latitude = 41.902782,
                Longitude = 12.496366
            }
        };

    }

    public async Task AddLocationAsync(MeteoLocation location)
    {
        
        
    }
}
