using CourierMobileApp.Services;

namespace CourierMobileApp.View;

public partial class ProfilePage : ContentPage
{
	private ProfileViewModel viewModel;
	private ProfileService profileService;
	public ProfilePage(ProfileViewModel profileViewModel, ProfileService profileService)
	{
		InitializeComponent();
		BindingContext = profileViewModel;
		viewModel = profileViewModel;
		this.profileService = profileService;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
		viewModel.ImgSource = profileService.imageSource;
    }
}