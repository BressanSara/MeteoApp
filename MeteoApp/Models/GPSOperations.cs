using System;
using System.Linq;
using System.Threading.Tasks;
using MeteoApp.Models;
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
}