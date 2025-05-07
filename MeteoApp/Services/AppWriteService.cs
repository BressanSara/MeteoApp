using System;
using System.Threading.Tasks;
using Appwrite;
using Appwrite.Models;
using Appwrite.Services;

namespace MeteoApp.Services
{
    public static class AppWriteService
    {
        private static readonly ApiKeyProvider ApiKeyProvider = new ApiKeyProvider();
        
        public static Client Client { get; private set; }
        public static Databases Database { get; private set; }

        public static string DatabaseId; // o ID creato nella console
        public static string CollectionId; // o ID della collezione
        
        public static async Task InitializeAsync()
        {
            try
            {
                DatabaseId = await ApiKeyProvider.GetApiKeyAsync("AppWriteDatabaseId");
                CollectionId = await ApiKeyProvider.GetApiKeyAsync("AppWriteCollectionId");
                
                var endpoint = await ApiKeyProvider.GetApiKeyAsync("AppWriteEndpoint");
                var project = await ApiKeyProvider.GetApiKeyAsync("AppWriteProjectId");
                var apiKey = await ApiKeyProvider.GetApiKeyAsync("AppWriteApiKey");

                Client = new Client()
                    .SetEndpoint(endpoint) 
                    .SetProject(project)   
                    .SetKey(apiKey);

                Database = new Databases(Client);
            }
            catch (Exception ex)
            {
                await DialogService.Instance.ShowAlert("Appwrite info", "Initialization failed");
                Console.WriteLine($"Error initializing AppWrite client: {ex.Message}");
                throw;
            }
        }
    }
}
