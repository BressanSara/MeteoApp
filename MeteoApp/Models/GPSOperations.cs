using System;
using System.Linq;
using System.Threading.Tasks;
using MeteoApp.Models;
using MeteoApp.Services;
using Microsoft.Maui.Devices.Sensors;

public class GPSOperations
{
    public async Task<MeteoLocation> GetCurrentLocationAsync()
    {
        MeteoLocation meteoLocation = new MeteoLocation();
        
        try
        {
            var locationRequest = new GeolocationRequest(GeolocationAccuracy.Best);
            var location = await Geolocation.GetLocationAsync(locationRequest);

            if (location != null)
            {
                // Traduci in placemark
                var placemarks = await Geocoding.GetPlacemarksAsync(location);
                var placemark = placemarks?.FirstOrDefault();

                meteoLocation.Latitude = location.Latitude;
                meteoLocation.Longitude = location.Longitude;

                if (placemark != null)
                {
                    meteoLocation.Name = placemark.Locality;
                    meteoLocation.Country = placemark.CountryName;
                }
                else
                {
                    meteoLocation.Name = $"{location.Latitude}, {location.Longitude}";
                    meteoLocation.Country = "Unknown";
                }

                meteoLocation.Id = "CurrentPosition"; // Prima posizione come identificativo
            }
            
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }

        return meteoLocation;
    }
    
    public async Task<MeteoLocation> GetLocationAsync(double latitude, double longitude)
    {
        var meteoLocation = new MeteoLocation
        {
            Latitude = latitude,
            Longitude = longitude
        };

        try
        {
            if (latitude >= -90 && latitude <= 90 && longitude >= -180 && longitude <= 180)
            {
                var placemarks = await Geocoding.GetPlacemarksAsync(latitude, longitude);
                var placemark = placemarks?.FirstOrDefault();

                if (placemark != null)
                {
                    meteoLocation.Name = placemark.Locality;
                    meteoLocation.Country = placemark.CountryName;
                }
                else
                {
                    meteoLocation.Name = $"{Math.Round(latitude, 4)}, {Math.Round(longitude, 4)}";
                    meteoLocation.Country = "Unknown";
                }
            }
            await DialogService.Instance.ShowAlert("Location", meteoLocation.Name, "OK");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            await DialogService.Instance.ShowAlert("Geolocation Error", ex.Message, "OK");

            // fallback già assegnato sopra
            meteoLocation.Name = $"{latitude}, {longitude}";
            meteoLocation.Country = "Unknown";
        }

        return meteoLocation;
    }

}