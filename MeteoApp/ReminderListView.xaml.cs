using MeteoApp.Models;
using MeteoApp.ViewModels;

namespace MeteoApp;

public partial class ReminderListView : ContentPage
{
    private ReminderListViewModel ViewModel => BindingContext as ReminderListViewModel;

    public ReminderListView()
    {
        InitializeComponent();
    }

    private void OnEditReminder(object sender, EventArgs e)
    {
        var reminder = (sender as ImageButton)?.CommandParameter as Reminder;
        if (reminder != null)
        {
            ViewModel.CurrentReminder = new Reminder
            {
                Id = reminder.Id,
                Lat = reminder.Lat,
                Lon = reminder.Lon,
                LocationName = reminder.LocationName,
                Threshold = reminder.Threshold,
                IsMax = reminder.IsMax
            };
            ViewModel.IsEditing = true;
        }
    }

    private async void OnDeleteReminder(object sender, EventArgs e)
    {
        var reminderId = (sender as ImageButton)?.CommandParameter as string;
        if (!string.IsNullOrEmpty(reminderId))
        {
            await ViewModel.DeleteReminderAsync(reminderId);
            ViewModel.CurrentReminder = new Reminder();
            ViewModel.IsEditing = false;
        }
    }

    private async void OnSaveReminder(object sender, EventArgs e)
    {
        if (ViewModel.CurrentReminder == null)
        {
            await DisplayAlert("Error", "Reminder is not initialized.", "OK");
            return;
        }

        if (string.IsNullOrEmpty(ViewModel.CurrentReminder.LocationName))
        {
            try
            {
                ViewModel.CurrentReminder.LocationName = await ViewModel.GetLocationNameAsync(
                    ViewModel.CurrentReminder.Lat,
                    ViewModel.CurrentReminder.Lon
                );
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to get location name: {ex.Message}", "OK");
                return;
            }
        }

        try
        {
            if (string.IsNullOrEmpty(ViewModel.CurrentReminder.Id))
            {
                ViewModel.CurrentReminder.Id = Guid.NewGuid().ToString();
                await ViewModel.AddReminderAsync(ViewModel.CurrentReminder);
            }
            else
            {
                await ViewModel.UpdateReminderAsync(ViewModel.CurrentReminder);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to save reminder: {ex.Message}", "OK");
            return;
        }

        ViewModel.CurrentReminder = new Reminder();
        ViewModel.IsEditing = false;
    }

    private void OnCancelEdit(object sender, EventArgs e)
    {
        ViewModel.CurrentReminder = new Reminder(); 
        ViewModel.IsEditing = false;
    }
}

