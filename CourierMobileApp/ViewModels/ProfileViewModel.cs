using CourierMobileApp.Models.Dto;
using CourierMobileApp.Services;
using CourierMobileApp.View;
using Maui.Plugins.PageResolver;
using Newtonsoft.Json;

namespace CourierMobileApp.ViewModels;

public partial class ProfileViewModel : BaseViewModel
{
    private readonly ProfileService profileService;
    private readonly ConnectionService connectionService;
    [ObservableProperty]
    ImageSource imgSource;
    [ObservableProperty]
    string user;
    [ObservableProperty]
    string email;
    [ObservableProperty]
    string phoneNumber;
    [ObservableProperty]
    bool error;
    [ObservableProperty]
    bool noError;

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

    public async Task GetProfileData()
    {
        IsBusy = true;
        string url = $"api/auth/get-profile-info";
        var res = await connectionService.SendAsync(HttpMethod.Get, url);
        try
        {
            ProfileInfoDto profile = JsonConvert.DeserializeObject<ProfileInfoDto>(await res.Content.ReadAsStringAsync());
            Email = profile.Email;
            PhoneNumber = profile.PhoneNumber;
            User = await SecureStorage.Default.GetAsync("user");
            Error = false;
            NoError = true;
            IsBusy = false;
        }
        catch (Exception)
        {
            await Shell.Current.DisplayAlert("Błąd pobierania", "Nie udało się pobrać danych profilu", "OK");
            Error = true;
            NoError = false;
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async static Task ChangePassword()
    {
        await Shell.Current.Navigation.PushAsync<ChangePasswordPage>();
    }


}
