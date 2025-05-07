using System;
using System.ComponentModel;
using MeteoApp.Models;
using MeteoApp.ViewModels;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Maps;
using Map = Microsoft.Maui.Controls.Maps.Map;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Maps;

namespace MeteoApp;

public partial class MapPage : ContentPage
{
	private Location? selectedLocation;
	
	public MapPage()
	{
		InitializeComponent();
		LoadMap();
	}
	
	private async void LoadMap()
	{
		var gps = new GPSOperations();
		MeteoLocation location = await gps.GetCurrentLocationAsync();

		if (location != null)
		{
			var position = new Location(location.Latitude, location.Longitude);
			var mapSpan = MapSpan.FromCenterAndRadius(position, Distance.FromKilometers(1));
			MyMap.MoveToRegion(mapSpan);
		}
		else
		{
			var position = new Location(RandomLatitude(), RandomLongitude());
			var mapSpan = MapSpan.FromCenterAndRadius(position, Distance.FromKilometers(1));
			MyMap.MoveToRegion(mapSpan);	
		}
	}
	
	private async void OnMapClicked(object? sender, MapClickedEventArgs e)
	{
		selectedLocation = e.Location;
		
		var placemarks = await Geocoding.GetPlacemarksAsync(selectedLocation);
		var placemark = placemarks?.FirstOrDefault();

		var meteoLocation = new MeteoLocation
		{
			Latitude = selectedLocation.Latitude,
			Longitude = selectedLocation.Longitude,
			Name = placemark?.Locality ?? $"{selectedLocation.Latitude}, {selectedLocation.Longitude}",
			Country = placemark?.CountryName ?? "Unknown",
			Coord = new Coord
			{
				lat = selectedLocation.Latitude,
				lon = selectedLocation.Longitude
			}
		};

		var navigationParameter = new Dictionary<string, object>
		{
			{ "MeteoLocation", meteoLocation }
		};

		await Shell.Current.GoToAsync($"locationdetails", navigationParameter);
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
}