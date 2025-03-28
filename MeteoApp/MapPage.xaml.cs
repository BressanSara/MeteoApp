using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Maps;

namespace MeteoApp;

public partial class MapPage : ContentPage
{
    public MapPage()
    {
        InitializeComponent();
        initializeMapContext();
    }

    private async Task initializeMapContext()
    {
        var currentLocation = await new GPSOperations().GetCurrentLocationAsync();

        if (currentLocation != null)
        {
            var location = new Location(currentLocation.Longitude, currentLocation.Longitude);
            var mapSpan = new MapSpan(location, 0.01, 0.01);
        
            map.MoveToRegion(mapSpan);    
        }
    }
}