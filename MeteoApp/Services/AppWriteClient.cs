using System;
using System.Threading.Tasks;
using Appwrite;

namespace MeteoApp.Services
{
    public static class AppWriteClient
    {
        private static readonly ApiKeyProvider _apiKeyProvider = new ApiKeyProvider();

        public static async Task<Client> InitializeAsync()
        {
            try
            {
                var endpoint = await _apiKeyProvider.GetApiKeyAsync("AppWriteEndpoint");
                var project = await _apiKeyProvider.GetApiKeyAsync("AppWriteProjectId");
                var apiKey = await _apiKeyProvider.GetApiKeyAsync("AppWriteApiKey");

                var client = new Client()
                    .SetEndpoint(endpoint) 
                    .SetProject(project)   
                    .SetKey(apiKey);

                return client;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing AppWrite client: {ex.Message}");
                throw;
            }
        }
    }
}
