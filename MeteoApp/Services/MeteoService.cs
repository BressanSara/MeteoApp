using MeteoApp.Models;
using System;
using Newtonsoft.Json;
using System.Net.Http;
using Microsoft.Maui.ApplicationModel;
using System.Diagnostics;
using System.Net.Http.Json;

public class MeteoService
{
    private readonly HttpClient _httpClient;

    public MeteoService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<CurrentWeatherData> GetWeatherAsync(MeteoLocation location)
    {
        try
        {
            var apiKeyProvider = new ApiKeyProvider();
            var API_KEY = await apiKeyProvider.GetOpenWeatherApiKeyAsync();

            var response = await _httpClient.GetAsync($"https://api.openweathermap.org/data/2.5/weather?lat={location.Coord.lat}&lon={location.Coord.lon}&appid={API_KEY}&units=metric");

            if (response.IsSuccessStatusCode)
            {
                var weatherData = await response.Content.ReadFromJsonAsync<CurrentWeatherData>();
                return weatherData;
            }
            else
            {
                Debug.WriteLine($"Failed to retrieve weather data. Status code: {response.StatusCode}");
                return null;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"An error occurred: {ex.Message}");
            return null;
        }
    }

    public async Task<ForecastData> GetForecastAsync(MeteoLocation location)
    {
        try
        {
            var apiKeyProvider = new ApiKeyProvider();
            var API_KEY = await apiKeyProvider.GetOpenWeatherApiKeyAsync();

            var response = await _httpClient.GetAsync($"https://api.openweathermap.org/data/2.5/forecast?lat={location.Latitude}&lon={location.Longitude}&appid={API_KEY}&units=metric");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"Forecast API Response: {content}");
                var forecastData = JsonConvert.DeserializeObject<ForecastData>(content);
                return forecastData;
            }
            else
            {
                Debug.WriteLine($"Failed to retrieve forecast data. Status code: {response.StatusCode}");
                return null;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"An error occurred while fetching forecast: {ex.Message}");
            return null;
        }
    }
}
