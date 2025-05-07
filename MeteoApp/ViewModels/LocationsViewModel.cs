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
            var latitude = Convert.ToDouble(doc.Data["latitude"]);
            var longitude = Convert.ToDouble(doc.Data["longitude"]);

            locations.Add(new MeteoLocation
            {
                Id = doc.Id,
                Name = doc.Data["name"].ToString(),
                Country = doc.Data["country"].ToString(),
                Latitude = latitude,
                Longitude = longitude,
                Coord = new Coord { lat = latitude, lon = longitude }
            });
        }

        return locations;

    }

    public async Task AddLocationAsync(MeteoLocation location)
    {
        var data = new Dictionary<string, object>
        {
            { "name", location.Name },
            { "country", location.Country },
            { "latitude", location.Latitude },
            { "longitude", location.Longitude }
        };

        try
        {
            var response = await AppWriteService.Database.CreateDocument(
                databaseId: AppWriteService.DatabaseId,
                collectionId: AppWriteService.CollectionId,
                documentId: "unique()",
                data: data
            );

            location.Id = response.Id;
        }
        catch (Exception e)
        {
            await DialogService.Instance.ShowAlert("Add error", "The location could not be added. Please try again.");
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task DeleteLocationAsync(MeteoLocation location)
    {
        await AppWriteService.InitializeAsync();
        if (string.IsNullOrEmpty(location.Id))
            throw new Exception("DocumentId non disponibile per la località selezionata.");

        try
        {
            await AppWriteService.Database.DeleteDocument(
                databaseId: AppWriteService.DatabaseId,
                collectionId: AppWriteService.CollectionId,
                documentId: location.Id
            );
        }
        catch (Exception e)
        {
            await DialogService.Instance.ShowAlert("Delete error", "The location could not be deleted, is either already deleted or you don't have permission to delete it.");
            Console.WriteLine(e);
        }

    }
}