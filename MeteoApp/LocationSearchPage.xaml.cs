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
            // Normalizza il testo di ricerca
            searchText = searchText.Trim().ToLowerInvariant();
            
            // Usa il servizio di geocoding di .NET MAUI
            var locations = await Geocoding.GetLocationsAsync(searchText);
            
            if (locations == null || !locations.Any())
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    SearchResultsList.ItemsSource = null;
                    LoadingIndicator.IsVisible = false;
                    LoadingIndicator.IsRunning = false;
                });
                return;
            }

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

    private void OnClearSearchClicked(object sender, EventArgs e)
    {
        LocationSearchBar.Text = string.Empty;
        SearchResultsList.ItemsSource = null;
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

    private async void OnLocationSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is SearchResult selectedResult)
        {
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

                var navigationParameter = new Dictionary<string, object>
                {
                    { "MeteoLocation", meteoLocation }
                };

                await Shell.Current.GoToAsync($"locationdetails", navigationParameter);

                // Deseleziona l'elemento nella lista
                if (sender is CollectionView collectionView)
                {
                    collectionView.SelectedItem = null;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Navigation error: {ex.Message}");
                await DisplayAlert("Error", "Failed to navigate to location details: " + ex.Message, "OK");
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