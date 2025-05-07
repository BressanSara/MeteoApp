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
    private System.Timers.Timer searchDebounceTimer;
    private const int DEBOUNCE_INTERVAL = 500; // 500ms debounce interval

    public LocationSearchPage()
    {
        InitializeComponent();
        locationsViewModel = new LocationsViewModel();
        InitializeDebounceTimer();
    }

    private void InitializeDebounceTimer()
    {
        searchDebounceTimer = new System.Timers.Timer(DEBOUNCE_INTERVAL);
        searchDebounceTimer.Elapsed += async (sender, e) =>
        {
            searchDebounceTimer.Stop();
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await PerformSearch(LocationSearchBar.Text);
            });
        };
        searchDebounceTimer.AutoReset = false;
    }

    private async void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(e.NewTextValue))
        {
            SearchResultsList.ItemsSource = null;
            LoadingIndicator.IsVisible = false;
            LoadingIndicator.IsRunning = false;
            return;
        }

        // Reset the timer on each text change
        searchDebounceTimer.Stop();
        searchDebounceTimer.Start();
        
        // Show loading indicator
        LoadingIndicator.IsVisible = true;
        LoadingIndicator.IsRunning = true;
    }

    private async Task PerformSearch(string searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText))
        {
            SearchResultsList.ItemsSource = null;
            LoadingIndicator.IsVisible = false;
            LoadingIndicator.IsRunning = false;
            return;
        }

        try
        {
            System.Diagnostics.Debug.WriteLine($"Starting search for: {searchText}");
            // Normalizza il testo di ricerca
            searchText = searchText.Trim().ToLowerInvariant();
            
            // Usa il servizio di geocoding di .NET MAUI
            var locations = await Geocoding.GetLocationsAsync(searchText);
            
            if (locations == null || !locations.Any())
            {
                System.Diagnostics.Debug.WriteLine("No locations found");
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    SearchResultsList.ItemsSource = null;
                    LoadingIndicator.IsVisible = false;
                    LoadingIndicator.IsRunning = false;
                });
                return;
            }

            System.Diagnostics.Debug.WriteLine($"Found {locations.Count()} locations");
            searchResults = new List<SearchResult>();

            foreach (var location in locations.Take(10))
            {
                try
                {
                    var placemarks = await Geocoding.GetPlacemarksAsync(location);
                    if (placemarks?.FirstOrDefault() is Placemark placemark)
                    {
                        var formattedName = FormatLocationName(placemark);
                        var country = placemark.CountryName ?? "Unknown";
                        
                        // Aggiungi il risultato senza filtri
                        searchResults.Add(new SearchResult
                        {
                            Name = formattedName,
                            Country = country,
                            Location = location
                        });
                        System.Diagnostics.Debug.WriteLine($"Added result: {formattedName}, {country}");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error getting placemark: {ex.Message}");
                    continue;
                }
            }

            // Ordina i risultati per rilevanza
            searchResults = searchResults
                .OrderBy(r => !r.Name.ToLowerInvariant().StartsWith(searchText))
                .ThenBy(r => r.Name)
                .ToList();

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                SearchResultsList.ItemsSource = searchResults;
                LoadingIndicator.IsVisible = false;
                LoadingIndicator.IsRunning = false;
                System.Diagnostics.Debug.WriteLine($"Set ItemsSource with {searchResults.Count} results");
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Search error: {ex.Message}");
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await DisplayAlert("Error", "Failed to search locations: " + ex.Message, "OK");
                LoadingIndicator.IsVisible = false;
                LoadingIndicator.IsRunning = false;
            });
        }
    }

    private string FormatLocationName(Placemark placemark)
    {
        var parts = new List<string>();

        // Aggiungi prima la città/località
        if (!string.IsNullOrEmpty(placemark.Locality))
            parts.Add(placemark.Locality);
        else if (!string.IsNullOrEmpty(placemark.SubLocality))
            parts.Add(placemark.SubLocality);
        else if (!string.IsNullOrEmpty(placemark.Thoroughfare))
            parts.Add(placemark.Thoroughfare);

        // Aggiungi la regione/stato se disponibile
        if (!string.IsNullOrEmpty(placemark.AdminArea))
            parts.Add(placemark.AdminArea);

        // Se non abbiamo trovato nessuna località, usa il nome del paese
        if (parts.Count == 0 && !string.IsNullOrEmpty(placemark.CountryName))
            parts.Add(placemark.CountryName);

        return string.Join(", ", parts);
    }


    private async void OnItemTapped(object sender, TappedEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine("OnItemTapped called");
        if (e.Parameter is SearchResult selectedResult)
        {
            System.Diagnostics.Debug.WriteLine($"Tapped location: {selectedResult.Name}, {selectedResult.Country}");
            try
            {
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

                System.Diagnostics.Debug.WriteLine($"Created MeteoLocation: {meteoLocation}");
                var navigationParameter = new Dictionary<string, object>
                {
                    { "MeteoLocation", meteoLocation }
                };

                System.Diagnostics.Debug.WriteLine("Navigating to locationdetails");
                await Shell.Current.GoToAsync($"locationdetails", navigationParameter);
                System.Diagnostics.Debug.WriteLine("Navigation completed");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Navigation error: {ex.Message}");
                await DisplayAlert("Error", "Failed to navigate to location details: " + ex.Message, "OK");
            }
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("No item tapped or invalid selection");
        }
    }

    private class SearchResult
    {
        public string Name { get; set; }
        public string Country { get; set; }
        public Location Location { get; set; }
    }
} 