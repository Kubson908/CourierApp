using CourierMobileApp.Services;

namespace CourierMobileApp.View;

public partial class MainPage : ContentPage
{
    private readonly ProfileService profileService;
    public MainPage(MainPageViewModel mainPageViewModel, ProfileService profileService, ProfileViewModel profileViewModel)
    {
        InitializeComponent();
        BindingContext = mainPageViewModel;
        this.profileService = profileService;
        navbar.Initialize(this.profileService);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        navbar.SetImage();
    }

    protected override bool OnBackButtonPressed()
    {
        return true;
    }
}