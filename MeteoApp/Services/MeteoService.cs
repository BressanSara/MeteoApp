using MeteoApp.Models;
using System.Net.Http.Json;

public class MeteoService
{
    private readonly HttpClient _httpClient;
    private readonly string API_KEY = "4668180ff7117459af24747cfb00db9a";

    public MeteoService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<CurrentWeatherData> GetWeatherAsync(MeteoLocation location)
    {
        try
        {
            var response = await _httpClient.GetAsync($"https://api.openweathermap.org/data/2.5/weather?lat={location.Coord.lat}&lon={location.Coord.lon}&appid={API_KEY}&units=metric");

            if (response.IsSuccessStatusCode)
            {
                var weatherData = await response.Content.ReadFromJsonAsync<CurrentWeatherData>();
                return weatherData;
            }
            else
            {
                // Handle non-success status codes
                Console.WriteLine($"Failed to retrieve weather data. Status code: {response.StatusCode}");
                return null;
            }
        }
        catch (Exception ex)
        {
            // Handle any exceptions that occur during the request
            Console.WriteLine($"An error occurred: {ex.Message}");
            return null;
        }
    }
}
