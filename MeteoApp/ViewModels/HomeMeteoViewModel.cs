namespace MeteoApp.ViewModels;

public class HomeMeteoViewModel
{
    public string currentLocation { get; set; } = "Bellinzona";
    public string currentTemperature { get; set; } = "Temperature: 25 °C";
    public string currentWeather { get; set; } = "Sunny";
    public string currentWind { get; set; } = "Wind pressure: 10 Km/h";
}