using CourierMobileApp.Services;

namespace CourierMobileApp.View;

public partial class ProfilePage : ContentPage
{
	private ProfileViewModel viewModel;
	public ProfilePage(ProfileViewModel profileViewModel)
	{
		InitializeComponent();
		BindingContext = profileViewModel;
		viewModel = profileViewModel;
        viewModel.PhotoChanged += LoadPhoto;
	}

    private async void LoadPhoto(object sender, EventArgs e)
    {
        string imagePath = await SecureStorage.GetAsync("imagePath");
        profileImage.Source = ImageSource.FromFile(imagePath);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
		await viewModel.GetProfileData();
		LoadPhoto(null, EventArgs.Empty);
	}
}