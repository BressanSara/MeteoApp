using MeteoApp.ViewModels;

namespace MeteoApp;

public partial class HomeMeteoPage : ContentPage
{
    
    public HomeMeteoPage()
    {
        InitializeComponent();
        BindingContext = new HomeMeteoViewModel();
    }

    public void OnListClicked(object sender, EventArgs e)
    {
        
    }

    public void OnAddClicked(object sender, EventArgs e)
    {
        
    }
}