using CourierMobileApp.Services;
using Maui.Plugins.PageResolver;

namespace CourierMobileApp.View.Components;

public partial class NavBar : ContentView
{
    private ProfileService profileService;
	public NavBar()
	{
		InitializeComponent();
        
    }

    public void Initialize(ProfileService profileService)
    {
        this.profileService = profileService;
        if (profileService.imageSource == null) profileService.SetImage();
        ProfileImage.Source = profileService.imageSource;
    }

    public void SetImage()
    {
        ProfileImage.Source = profileService.imageSource;
    }

    private async void ProfileClicked(object sender, EventArgs e)
    {
        await Shell.Current.Navigation.PushAsync<ProfilePage>();
    }
}