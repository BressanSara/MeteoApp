using Appwrite;
using Appwrite.Services;
using MeteoApp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeteoApp.Services
{
    public class LocationService
    {
        private readonly Databases _databases;
        private readonly ApiKeyProvider _apiKeyProvider;
        private readonly string _databaseId;

        public LocationService()
        {
            _apiKeyProvider = new ApiKeyProvider();

            // Fetch required parameters
            var endpoint = _apiKeyProvider.GetApiKeyAsync("AppWriteEndpoint").Result;
            var projectId = _apiKeyProvider.GetApiKeyAsync("AppWriteProjectId").Result;
            var apiKey = _apiKeyProvider.GetApiKeyAsync("AppWriteApiKey").Result;
            _databaseId = _apiKeyProvider.GetApiKeyAsync("AppWriteDatabaseId").Result;

            // Initialize the Databases service
            var client = new Client()
                .SetEndpoint(endpoint)
                .SetProject(projectId)
                .SetKey(apiKey);

            _databases = new Databases(client);
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

            await _databases.CreateDocument(
                databaseId: _databaseId,
                collectionId: collectionId,
                documentId: "unique()",
                data: data
            );
        }

        public async Task<List<MeteoLocation>> LoadLocationsAsync()
        {
            var collectionId = await GetCollectionIdAsync();

            var response = await _databases.ListDocuments(_databaseId, collectionId);
            return response.Documents.Select(doc => new MeteoLocation
            {
                Id = int.Parse(doc.Id),
                Name = doc.Data["name"].ToString(),
                Longitude = double.Parse(doc.Data["longitude"].ToString()),
                Latitude = double.Parse(doc.Data["latitude"].ToString())
            }).ToList();
        }

        public async Task DeleteLocationAsync(string documentId)
        {
            var collectionId = await GetCollectionIdAsync();
            await _databases.DeleteDocument(_databaseId, collectionId, documentId);
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

            await _databases.UpdateDocument(_databaseId, collectionId, documentId, data);
        }
    }
}
