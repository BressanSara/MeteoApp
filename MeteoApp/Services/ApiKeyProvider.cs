using System.Text.Json;

public class ApiKeyProvider
{
    public async Task<string> GetOpenWeatherApiKeyAsync()
    {
        try
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync("keys.json");
            using var reader = new StreamReader(stream);
            var json = await reader.ReadToEndAsync();

            var keys = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            if (keys != null && keys.TryGetValue("OpenWeatherApiKey", out var apiKey))
            {
                return apiKey;
            }

            throw new Exception("API key not found in keys.json.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading API key: {ex.Message}");
            throw;
        }
    }

    public async Task<string> GetApiKeyAsync(string keyName)
    {
        try
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync("keys.json");
            using var reader = new StreamReader(stream);
            var json = await reader.ReadToEndAsync();

            var keys = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            if (keys != null && keys.TryGetValue(keyName, out var apiKey))
            {
                return apiKey;
            }

            throw new Exception($"API key '{keyName}' not found in keys.json.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading API key '{keyName}': {ex.Message}");
            throw;
        }
    }
}
