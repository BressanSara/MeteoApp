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

        
        var locations = new ObservableCollection<MeteoLocation>();

        foreach (var doc in appwriteResponse.Documents)
        {
            var latitude = Convert.ToDouble(doc.Data["Latitude"]);
            var longitude = Convert.ToDouble(doc.Data["Longitude"]);
                
            locations.Add(new MeteoLocation
            {
                Id = doc.Id,
                Name = doc.Data["CityName"].ToString(),
                Country = doc.Data["CountryName"].ToString(),
                Latitude = latitude,
                Longitude = longitude,
                Coord = new Coord{lat = latitude, lon = longitude}
            });
        }
        
        return locations;

    }

    public async Task AddLocationAsync(MeteoLocation location)
    {
        
        
    }

    public async Task DeleteLocationAsync(MeteoLocation location)
    {
        if (string.IsNullOrEmpty(location.Id))
            throw new Exception("DocumentId non disponibile per la località selezionata.");

        await AppWriteService.Database.DeleteDocument(
            databaseId: AppWriteService.DatabaseId,
            collectionId: AppWriteService.CollectionId,
            documentId: location.Id
        );
    }
}
