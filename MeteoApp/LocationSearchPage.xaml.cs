using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices.Sensors;
using MeteoApp.Models;
using MeteoApp.ViewModels;

namespace MeteoApp;

public partial class LocationSearchPage : ContentPage
{
    private List<SearchResult> searchResults = new();
    private readonly LocationsViewModel locationsViewModel;

    public LocationSearchPage()
    {
        InitializeComponent();
        locationsViewModel = new LocationsViewModel();
    }

    private async void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(e.NewTextValue))
        {
            SearchResultsList.ItemsSource = null;
            return;
        }

        try
        {
            var searchText = e.NewTextValue.ToLowerInvariant();
            var locations = await Geocoding.GetLocationsAsync(searchText);
            searchResults = new List<SearchResult>();

            foreach (var location in locations.Take(5))
            {
                var placemarks = await Geocoding.GetPlacemarksAsync(location);
                if (placemarks?.FirstOrDefault() is Placemark placemark)
                {
                    if (IsLocationMatch(placemark, searchText))
                    {
                        searchResults.Add(new SearchResult
                        {
                            Name = FormatLocationName(placemark),
                            Country = placemark.CountryName ?? "Unknown",
                            Location = location
                        });
                    }
                }
            }

            SearchResultsList.ItemsSource = searchResults;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "Failed to search locations: " + ex.Message, "OK");
        }
    }

    private bool IsLocationMatch(Placemark placemark, string searchText)
    {
        return (!string.IsNullOrEmpty(placemark.Locality) && placemark.Locality.ToLowerInvariant().Contains(searchText)) ||
               (!string.IsNullOrEmpty(placemark.SubLocality) && placemark.SubLocality.ToLowerInvariant().Contains(searchText)) ||
               (!string.IsNullOrEmpty(placemark.AdminArea) && placemark.AdminArea.ToLowerInvariant().Contains(searchText)) ||
               (!string.IsNullOrEmpty(placemark.CountryName) && placemark.CountryName.ToLowerInvariant().Contains(searchText));
    }

    private string FormatLocationName(Placemark placemark)
    {
        var parts = new List<string>();

        if (!string.IsNullOrEmpty(placemark.Locality))
            parts.Add(placemark.Locality);
        else if (!string.IsNullOrEmpty(placemark.SubLocality))
            parts.Add(placemark.SubLocality);
        else if (!string.IsNullOrEmpty(placemark.Thoroughfare))
            parts.Add(placemark.Thoroughfare);

        if (!string.IsNullOrEmpty(placemark.AdminArea))
            parts.Add(placemark.AdminArea);

        return string.Join(", ", parts);
    }

    private async void OnLocationSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is SearchResult selectedResult)
        {
            try
            {
                // Crea la nuova MeteoLocation
                var meteoLocation = new MeteoLocation
                {
                    Latitude = selectedResult.Location.Latitude,
                    Longitude = selectedResult.Location.Longitude,
                    Name = selectedResult.Name,
                    Country = selectedResult.Country,
                    Coord = new Coord
                    {
                        lat = selectedResult.Location.Latitude,
                        lon = selectedResult.Location.Longitude
                    }
                };

                // Salva la location nel database
                await locationsViewModel.AddLocationAsync(meteoLocation);

                // Prepara i parametri per la navigazione
                var navigationParameter = new Dictionary<string, object>
                {
                    { "MeteoLocation", meteoLocation }
                };

                // Naviga alla pagina dei dettagli
                await Shell.Current.GoToAsync($"locationdetails", navigationParameter);

                // Deseleziona l'elemento nella lista
                if (sender is CollectionView collectionView)
                {
                    collectionView.SelectedItem = null;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Failed to add location: " + ex.Message, "OK");
            }
        }
    }

    private class SearchResult
    {
        public string Name { get; set; }
        public string Country { get; set; }
        public Location Location { get; set; }
    }
} 