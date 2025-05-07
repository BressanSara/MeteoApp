using MeteoApp.Models;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace MeteoApp.Services
{
    public class GeoCodingService
    {
        private HttpClient _httpClient;

        public GeoCodingService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<MeteoLocation> GetLocationByNameAsync(string locationName)
        {
            try
            {
                ApiKeyProvider keyProvider = new ApiKeyProvider();
                string apiKey = await keyProvider.GetApiKeyAsync(locationName);

                string url = $"http://api.openweathermap.org/geo/1.0/direct?q={locationName}&limit=1&appid={apiKey}";

                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var locations = JsonSerializer.Deserialize<List<GeocodingResult>>(json);

                if (locations != null && locations.Count > 0)
                {
                    var result = locations[0];
                    return new MeteoLocation
                    {
                        Name = result.Name,
                        Latitude = result.Lat,
                        Longitude = result.Lon,
                        Coord = new Coord
                        {
                            lat = result.Lat,
                            lon = result.Lon
                        }
                    };
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        private class GeocodingResult
        {
            public string Name { get; set; }
            public double Lat { get; set; }
            public double Lon { get; set; }
        }
    }
}
