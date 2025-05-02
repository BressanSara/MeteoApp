using MeteoApp.Models;
using MeteoApp.Services;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace MeteoApp.ViewModels
{
    public class ReminderListViewModel : BaseViewModel
    {
        private readonly HttpClient _httpClient;
        private ObservableCollection<Reminder> _reminders;

        public ObservableCollection<Reminder> Reminders
        {
            get { return _reminders; }
            set
            {
                _reminders = value;
                OnPropertyChanged();
                CurrentReminder = new Reminder();
            }
        }

        private ObservableCollection<MeteoLocation> _locations;

        public ObservableCollection<MeteoLocation> Locations 
        { 
            get { return _locations; } 
            set
            {
                _locations = value;
                OnPropertyChanged();
            }
        }

        private Reminder _currentReminder;
        public Reminder CurrentReminder
        {
            get => _currentReminder;
            set
            {
                _currentReminder = value;
                OnPropertyChanged();
            }
        }

        private MeteoLocation _currentLocation;

        public MeteoLocation CurrentLocation
        {
            get { return _currentLocation; }
            set
            {
                _currentLocation = value;
                OnPropertyChanged();
                if (_currentLocation != null)
                {
                    CurrentReminder.Lat = _currentLocation.Latitude;
                    CurrentReminder.Lon = _currentLocation.Longitude;
                    CurrentReminder.LocationName = _currentLocation.Name;
                }
            }
        }

        public bool IsEditing { get; internal set; }

        public ReminderListViewModel()
        {
            _httpClient = new HttpClient();
            Reminders = new ObservableCollection<Reminder>();
            _ = LoadRemindersAsync();
            _ = LoadLocationsAsync();
        }

        private async Task LoadLocationsAsync()
        {
            LocationListViewModel viewModel = new LocationListViewModel();
            Locations = viewModel.Locations;
        }

        private async Task LoadRemindersAsync()
        {
            try
            {
                var uri = NotificationService.getNotificationServerAddress() + "/reminders";

                var response = await _httpClient.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var reminders = JsonSerializer.Deserialize<List<Reminder>>(json);

                    if (reminders != null)
                    {
                        Reminders = new ObservableCollection<Reminder>(reminders);

                        foreach (var reminder in Reminders)
                        {
                            if (reminder.LocationName == null)
                            {
                                reminder.LocationName = await GetLocationNameAsync(reminder.Lat, reminder.Lon);
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Failed to load reminders. Status code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while loading reminders: {ex.Message}");
            }
        }

        public async Task AddReminderAsync(Reminder newReminder)
        {
            try
            {
                var uri = NotificationService.getNotificationServerAddress() + "/reminders";

                var json = JsonSerializer.Serialize(newReminder);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(uri, content);

                if (response.IsSuccessStatusCode)
                {
                    Reminders.Add(newReminder);
                }
                else
                {
                    Console.WriteLine($"Failed to add reminder. Status code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while adding the reminder: {ex.Message}");
            }
        }

        public async Task DeleteReminderAsync(string reminderId)
        {
            try
            {
                var uri = NotificationService.getNotificationServerAddress() + $"/reminders/{reminderId}";

                var response = await _httpClient.DeleteAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    var reminderToRemove = Reminders.FirstOrDefault(r => r.Id == reminderId);
                    if (reminderToRemove != null)
                    {
                        Reminders.Remove(reminderToRemove);
                    }
                }
                else
                {
                    Console.WriteLine($"Failed to delete reminder. {response.StatusCode} {response.Content} {response.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while deleting the reminder: {ex.Message}");
            }
        }

        public async Task UpdateReminderAsync(Reminder updatedReminder)
        {
            try
            {
                var uri = NotificationService.getNotificationServerAddress() + $"/reminders/{updatedReminder.Id}";

                var json = JsonSerializer.Serialize(updatedReminder);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync(uri, content);

                if (response.IsSuccessStatusCode)
                {
                    var existingReminder = Reminders.FirstOrDefault(r => r.Id == updatedReminder.Id);
                    if (existingReminder != null)
                    {
                        var index = Reminders.IndexOf(existingReminder);
                        Reminders[index] = updatedReminder;
                    }
                }
                else
                {
                    Console.WriteLine($"Failed to update reminder. Status code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while updating the reminder: {ex.Message}");
            }
        }

        public async Task<string> GetLocationNameAsync(double latitude, double longitude)
        {
            try
            {
                var placemarks = await Geocoding.GetPlacemarksAsync(latitude, longitude);
                var placemark = placemarks?.FirstOrDefault();

                if (placemark != null)
                {
                    return $"{placemark.Locality}, {placemark.CountryName}";
                }
                else
                {
                    return $"{latitude}, {longitude}";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching location name: {ex.Message}");
                return $"{latitude}, {longitude}";
            }
        }
    }

}
