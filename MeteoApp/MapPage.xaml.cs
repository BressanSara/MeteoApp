using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeteoApp.Models;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Maps;

namespace MeteoApp;

public partial class MapPage : ContentPage
{
    public MapPage()
    {
        InitializeComponent();
        var currentLocation = new GPSOperations().GetCurrentLocationAsync();
        var location = new Location(currentLocation.Result.Latitude, currentLocation.Result.Longitude);
        var mapSpan = new MapSpan(location, 0.01, 0.01);
        
        map.MoveToRegion(mapSpan);
    }
}