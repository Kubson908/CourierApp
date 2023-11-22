using CourierMobileApp.Services;
using Maui.Plugins.PageResolver;

namespace CourierMobileApp.View;

public partial class MainPage : ContentPage
{
    public MainPage(MainPageViewModel mainPageViewModel)
    {
        InitializeComponent();
        BindingContext = mainPageViewModel;
    }

    protected override bool OnBackButtonPressed()
    {
        return true;
    }
}