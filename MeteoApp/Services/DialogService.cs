using Microsoft.Maui.Controls;
using System.Threading.Tasks;

namespace MeteoApp.Services;
public class DialogService
{
    private static DialogService _instance;
    private Page _currentPage;

    public static DialogService Instance => _instance ??= new DialogService();

    private DialogService() { }

    public void Initialize(Page page)
    {
        _currentPage = page;
    }

    public async Task ShowAlert(string title, string message, string cancel = "OK")
    {
        if (_currentPage == null)
            throw new System.Exception("DialogService non è stato inizializzato con una Page.");

        await MainThread.InvokeOnMainThreadAsync(() =>
            _currentPage.DisplayAlert(title, message, cancel)
        );
    }
    
    public async Task<bool> ShowConfirmation(string title, string message, string accept = "Yes", string cancel = "No")
    {
        if (_currentPage == null) return false;

        return await MainThread.InvokeOnMainThreadAsync(() =>
            _currentPage.DisplayAlert(title, message, accept, cancel)
        );
    }
}