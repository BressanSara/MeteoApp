using System.ComponentModel;
using Microsoft.Maui.Controls;
using Map = Microsoft.Maui.Controls.Maps.Map;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Maps;

namespace MeteoApp;

public partial class MapPage : ContentPage
{
	public MapPage()
	{
		InitializeComponent();

		var gps = new GPSOperations();

		var current = gps.GetCurrentLocationAsync();
		Map map = new Map();
		Content = map;
		
	}
}