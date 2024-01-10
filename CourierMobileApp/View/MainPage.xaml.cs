using CourierMobileApp.Services;
using CourierMobileApp.View.Components;

namespace CourierMobileApp.View;

public partial class MainPage : ContentPage
{
    private readonly ProfileService profileService;
    private readonly MenuAnimation animation;
    public MainPage(MainPageViewModel mainPageViewModel, ProfileService profileService)
    {
        InitializeComponent();
        BindingContext = mainPageViewModel;
        this.profileService = profileService;
        animation = new()
        {
            layout = MainContent
        };
        navbar.MenuClicked += (object sender, EventArgs e) => { animation.OpenMenu(sender, e); navbar.RotateIcon(); };
        navbar.Initialize(this.profileService);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        navbar.SetImage();
        profileService.user = await SecureStorage.Default.GetAsync("user");
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        animation.CloseMenu(null, EventArgs.Empty);
        if (navbar.menuOpened) navbar.RotateIcon();
    }

    protected override bool OnBackButtonPressed()
    {
        animation.OpenMenu(null, EventArgs.Empty);
        navbar.RotateIcon();
        return true;
    }
}