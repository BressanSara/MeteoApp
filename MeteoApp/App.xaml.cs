﻿namespace MeteoApp;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState activationState)
    {
        var window = new Window(new LocationListView());
        window.Title = "MeteoApp";
        return window;
    }
}
