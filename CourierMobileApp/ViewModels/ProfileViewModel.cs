using CourierMobileApp.Models.Dto;
using CourierMobileApp.Services;

namespace CourierMobileApp.ViewModels;

public partial class ProfileViewModel : BaseViewModel
{
    private readonly ProfileService profileService;
    private readonly ConnectionService connectionService;
    [ObservableProperty]
    ImageSource imgSource;

    public ProfileViewModel(ProfileService profileService, ConnectionService connectionService)
    {
        this.profileService = profileService;
        this.connectionService = connectionService;
    }

    [RelayCommand]
    public async Task UploadPhoto()
    {
        var uploadFile = await MediaPicker.PickPhotoAsync();
        if (uploadFile == null) return;

        ApiUserResponse response = await connectionService.UploadPhotoAsync(uploadFile);
        if (!response.IsSuccess) await Shell.Current.DisplayAlert("Błąd przesyłania", response.Message, "OK");
        else
        {
            await SecureStorage.Default.SetAsync("profile_image", response.Image);
            profileService.SetImage();
            ImgSource = profileService.imageSource;
        }
    }
}
