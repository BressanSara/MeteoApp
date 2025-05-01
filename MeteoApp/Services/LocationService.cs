using Appwrite.Models;
using Appwrite.Services;
using MeteoApp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeteoApp.Services
{
    public class LocationService
    {
        private readonly Database _database;
        private readonly ApiKeyProvider _apiKeyProvider;

        public LocationService()
        {
            var client = AppWriteClient.Initialize();
            _database = new Database(client);
            _apiKeyProvider = new ApiKeyProvider();
        }

        private async Task<string> GetCollectionIdAsync()
        {
            return await _apiKeyProvider.GetApiKeyAsync("AppWriteCollectionId");
        }

        public async Task SaveLocationAsync(MeteoLocation location)
        {
            var collectionId = await GetCollectionIdAsync();

            var data = new Dictionary<string, object>
            {
                { "name", location.Name },
                { "longitude", location.Longitude },
                { "latitude", location.Latitude }
            };

            await _database.CreateDocument(
                collectionId: collectionId,
                documentId: "unique()",
                data: data
            );
        }

        public async Task<List<MeteoLocation>> LoadLocationsAsync()
        {
            var collectionId = await GetCollectionIdAsync();

            var response = await _database.ListDocuments(collectionId);
            return response.Documents.Select(doc => new MeteoLocation
            {
                Id = int.Parse(doc["$id"].ToString()),
                Name = doc["name"].ToString(),
                Longitude = double.Parse(doc["longitude"].ToString()),
                Latitude = double.Parse(doc["latitude"].ToString())
            }).ToList();
        }

        public async Task DeleteLocationAsync(string documentId)
        {
            var collectionId = await GetCollectionIdAsync();
            await _database.DeleteDocument(collectionId, documentId);
        }

        public async Task UpdateLocationAsync(string documentId, MeteoLocation updatedLocation)
        {
            var collectionId = await GetCollectionIdAsync();

            var data = new Dictionary<string, object>
            {
                { "name", updatedLocation.Name },
                { "longitude", updatedLocation.Longitude },
                { "latitude", updatedLocation.Latitude }
            };

            await _database.UpdateDocument(collectionId, documentId, data);
        }
    }
}
