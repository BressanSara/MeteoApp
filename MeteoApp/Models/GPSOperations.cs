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
                
                var coord = new Coord();
                coord.lat = location.Latitude;
                coord.lon = location.Longitude;

                meteoLocation.Coord = coord;

                if (placemark != null)
                {
                    meteoLocation.Name = $"{placemark.Locality}, {placemark.CountryName}";
                }
                else
                {
                    meteoLocation.Name = $"{location.Latitude}, {location.Longitude}";
                }

                meteoLocation.Id = 0; // Prima posizione come identificativo
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }

        return meteoLocation;
    }
}