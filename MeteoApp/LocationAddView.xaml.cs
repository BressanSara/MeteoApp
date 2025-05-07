using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeteoApp.Models;
using MeteoApp.Services;
using Microsoft.Maui.Controls;

namespace MeteoApp.ViewModels;

public partial class LocationAddView : ContentPage
{
    public LocationAddView()
    {
        InitializeComponent();
    }

    public async void OnAddLocation(object sender, EventArgs e)
    {
        var gps = new GPSOperations();
        var location = await gps.GetLocationAsync(RandomLatitude(), RandomLongitude());
        
        var model = new LocationsViewModel();
        await model.AddLocationAsync(location);
    }
    
    private double RandomLatitude()
    {
        Random rand = new Random();
        return rand.NextDouble() * 80 - 1;
    }

    private double RandomLongitude()
    {
        Random rand = new Random();
        return rand.NextDouble() * 90 - 9;
    }
}