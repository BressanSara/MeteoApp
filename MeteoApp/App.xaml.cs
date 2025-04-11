using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace MeteoApp;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState activationState)
    {
        //var window = new Window(new LocationListView());
        var window = new Window(new MapPage());
        window.Title = "MeteoApp";
        return window;
    }
}
