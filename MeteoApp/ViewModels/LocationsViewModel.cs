using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.Json;
using System.Threading.Tasks;
using Appwrite;
using MeteoApp.Models;
using MeteoApp.Services;
using Appwrite.Models;
using Appwrite.Services;

namespace MeteoApp.ViewModels;

public class LocationsViewModel
{
    public async Task<ObservableCollection<MeteoLocation>> LoadLocationsAsync()
    {
        var locations = new ObservableCollection<MeteoLocation>();
        
        try
        {
            await AppWriteService.InitializeAsync();

            var appwriteResponse = await AppWriteService
                .Database
                .ListDocuments(
                    databaseId: AppWriteService.DatabaseId, 
                    collectionId: AppWriteService.CollectionId
                );

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
                    Coord = new Coord{lat = latitude, lon = longitude}
                });
            }
            
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            await DialogService.Instance.ShowAlert("Error", "Could not load locations from Appwrite.\n" + e.Message);
        }

        return locations;
    }

    public async Task AddLocationAsync(MeteoLocation location)
    {
        await AppWriteService.InitializeAsync();
        
        // Prepara i dati da salvare
        var data = new Dictionary<string, object>
        {
            ["Name"] = location.Name,
            ["Country"] = location.Country,
            ["Latitude"] = location.Latitude,
            ["Longitude"] = location.Longitude
        };
        
        var permissions = new List<string>
        {
            "read(\"any\")",
            "update(\"any\")",
            "delete(\"any\")"
        };

        try
        {

            Document result = await AppWriteService.Database.CreateDocument(
                    databaseId: AppWriteService.DatabaseId,
                    collectionId: AppWriteService.CollectionId,
                    documentId: ID.Unique(),
                    data: data,
                    permissions: permissions
                );


            // Se vuoi salvare l’ID nel tuo oggetto:
            location.Id = result.Id;

            await DialogService.Instance.ShowAlert("Success", "Location added successfully.");
        }
        catch (Exception e)
        {
            await DialogService.Instance.ShowAlert("Error", "Could not add location to Appwrite.\n" + e.Message);
            Console.WriteLine(e);
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
