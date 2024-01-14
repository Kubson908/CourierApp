using CourierMobileApp.Services;

namespace CourierMobileApp.View;

public partial class ProfilePage : ContentPage
{
	private readonly ProfileViewModel viewModel;
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
        if (imagePath != null && File.Exists(imagePath))
        {
            profileImage.Source = ImageSource.FromFile(imagePath);
        }
        else
        {
            if (Application.Current.UserAppTheme == AppTheme.Light)
            {
                profileImage.Source = "profile_circle_dark.svg";
            }
            else profileImage.Source = "profile_circle.svg";
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
		await viewModel.GetProfileData();
		LoadPhoto(null, EventArgs.Empty);
	}
}