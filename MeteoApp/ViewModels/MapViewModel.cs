using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using MeteoApp.Models;


namespace MeteoApp.ViewModels;

public class MapViewModel : INotifyPropertyChanged
{
    private double latitude;
    public double Latitude
    {
        get => latitude;
        set
        {
            if (latitude != value)
            {
                latitude = value;
                OnPropertyChanged();
            }
        }
    }

    private double longitude;
    public double Longitude
    {
        get => longitude;
        set
        {
            if (longitude != value)
            {
                longitude = value;
                OnPropertyChanged();
            }
        }
    }

    public MapViewModel()
    {
        LoadLocationAsync();
    }

    private async Task LoadLocationAsync()
    {
        var gps = new GPSOperations();
        MeteoLocation location = await gps.GetCurrentLocationAsync();
        if (location != null)
        {
            Latitude = RandomLatitude();
            Longitude = RandomLongitude();
            /*
            Latitude = location.Latitude;
            Longitude = location.Longitude;        
            */
        }
    }

    private double RandomLatitude()
    {
        Random rand = new Random();
        return rand.NextDouble() * 180 - 90;
    }

    private double RandomLongitude()
    {
        Random rand = new Random();
        return rand.NextDouble() * 360 - 180;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
