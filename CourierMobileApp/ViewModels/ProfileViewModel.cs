using CourierMobileApp.Models.Dto;
using CourierMobileApp.Services;
using CourierMobileApp.View;
using Maui.Plugins.PageResolver;
using Newtonsoft.Json;

namespace CourierMobileApp.ViewModels;

public partial class ProfileViewModel : BaseViewModel
{
    public event EventHandler PhotoChanged;
    private readonly ConnectionService connectionService;
    [ObservableProperty]
    string user;
    [ObservableProperty]
    string email;
    [ObservableProperty]
    string phoneNumber;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(NoError))]
    bool error;

    public bool NoError => !Error;

    public ProfileViewModel(ConnectionService connectionService)
    {
        this.connectionService = connectionService;
    }

    [RelayCommand]
    public void UploadPhoto()
    {
        MainThread.InvokeOnMainThreadAsync(async () =>
        {
            var uploadFile = await MediaPicker.PickPhotoAsync();
            if (uploadFile == null) return;
            IsBusy = true;
            ApiUserResponse response = await connectionService.UploadPhotoAsync(uploadFile);
            if (!response.IsSuccess) await Shell.Current.DisplayAlert("Błąd przesyłania", response.Message, "OK");
            else
            {
                string filePath = await SecureStorage.Default.GetAsync("imagePath");
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                string fileName = $"courier_profile{DateTime.Now.Ticks}.png";
                filePath = Path.Combine(folderPath, fileName);
                File.WriteAllBytes(filePath, Convert.FromBase64String(response.Image));
                await SecureStorage.Default.SetAsync("imagePath", filePath);
                /*profileService.SetImage();*/
                /*ImgSource = profileService.imageSource;*/
            }
            PhotoChanged.Invoke(this, EventArgs.Empty);
            IsBusy = false;
        });
    }

    public async Task GetProfileData()
    {
        IsBusy = true;
        string url = $"api/auth/get-profile-info";
        try
        {
            var res = await connectionService.SendAsync(HttpMethod.Get, url);
            ProfileInfoDto profile = JsonConvert.DeserializeObject<ProfileInfoDto>(await res.Content.ReadAsStringAsync());
            Email = profile.Email;
            PhoneNumber = profile.PhoneNumber;
            User = await SecureStorage.Default.GetAsync("user");
            Error = false;
        }
        catch (Exception)
        {
            await Shell.Current.DisplayAlert("Błąd pobierania", "Nie udało się pobrać danych profilu", "OK");
            Error = true;
        } finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async static Task ChangePassword()
    {
        await Shell.Current.Navigation.PushAsync<ChangePasswordPage>();
    }

    [RelayCommand]
    public async Task LogOut()
    {
        string filePath = await SecureStorage.Default.GetAsync("imagePath");
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
        SecureStorage.RemoveAll();
        connectionService.Token = null;
        Email = null;
        PhoneNumber = null;
        Error = false;
        User = null;
        await Shell.Current.Navigation.PopToRootAsync();
    }


}
