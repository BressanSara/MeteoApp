using System;
using System.ComponentModel;
using System.Linq;
using MeteoApp.Models;
using MeteoApp.Services;
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
		var gps = new GPSOperations();
			
		var meteoLocation = await gps.GetLocationAsync(selectedLocation.Latitude, selectedLocation.Longitude);
		
		var lvm = new LocationsViewModel();
		await lvm.AddLocationAsync(meteoLocation);
	}
	
	private async void OnSearchConfirmed(object sender, EventArgs e)
	{
		var query = LocationSearchBar.Text;

		if (string.IsNullOrWhiteSpace(query))
			return;

		try
		{
			var locations = await Geocoding.GetLocationsAsync(query);
			var location = locations?.FirstOrDefault();

			if (location != null)
			{
				var mapSpan = MapSpan.FromCenterAndRadius(
					new Microsoft.Maui.Devices.Sensors.Location(location.Latitude, location.Longitude),
					Distance.FromKilometers(5));

				MyMap.MoveToRegion(mapSpan);

				MyMap.Pins.Clear();
				MyMap.Pins.Add(new Pin
				{
					Label = query,
					Location = new Microsoft.Maui.Devices.Sensors.Location(location.Latitude, location.Longitude)
				});
			}
		}
		catch (Exception ex)
		{
			await DisplayAlert("Error", "Unable to find location.", "OK");
			Console.WriteLine(ex);
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
}