using CourierMobileApp.Services;
using Maui.Plugins.PageResolver;

namespace CourierMobileApp.View.Components;

public partial class NavBar : ContentView
{
    private ProfileService profileService;
    public EventHandler MenuClicked;
    public bool menuOpened = false;
	public NavBar()
	{
		InitializeComponent();
    }

    public void Initialize(ProfileService profileService)
    {
        this.profileService = profileService;
    }

    public async Task SetImage()
    {
        string imagePath = await SecureStorage.GetAsync("imagePath");
        profileImage.Source = ImageSource.FromFile(imagePath);
    }

    private async void ProfileClicked(object sender, EventArgs e)
    {
        /*await profileService.GetProfileData();*/
        await Shell.Current.Navigation.PushAsync<ProfilePage>();
    }

    private void OnMenuClicked(object sender, EventArgs e)
    {
        MenuClicked?.Invoke(this, EventArgs.Empty);
    }

    public async void RotateIcon()
    {
        if (!menuOpened)
        {
            await menuButton.RotateTo(90, 400u, Easing.Linear);
            menuOpened = true;
        }
        else
        {
            await menuButton.RotateTo(0, 400u, Easing.Linear);
            menuOpened = false;
        }
    }
}