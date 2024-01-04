using CourierMobileApp.Models.Dto;
using Newtonsoft.Json;

namespace CourierMobileApp.Services;

public class ProfileService
{
    private readonly ConnectionService connectionService;
    public ImageSource imageSource = null;
    public string user;
    public string email;
    public string phoneNumber;
    public bool error;
    public bool noError;
    public ProfileService(ConnectionService connectionService)
    {
        this.connectionService = connectionService;
    }

    public void SetImage()
    {
        var imageData = SecureStorage.Default.GetAsync("profile_image").Result;

        if (!string.IsNullOrEmpty(imageData))
        {
            var imageBytes = Convert.FromBase64String(imageData);
            imageSource = ImageSource.FromStream(() => new MemoryStream(imageBytes));
        }
    }

    public async Task GetProfileData()
    {
        string url = $"api/auth/get-profile-info";
        var res = await connectionService.SendAsync(HttpMethod.Get, url);
        try
        {
            ProfileInfoDto profile = JsonConvert.DeserializeObject<ProfileInfoDto>(await res.Content.ReadAsStringAsync());
            email = profile.Email;
            phoneNumber = profile.PhoneNumber;
            error = false;
            noError = true;
        }
        catch (Exception)
        {
            await Shell.Current.DisplayAlert("Błąd pobierania", "Nie udało się pobrać danych profilu", "OK");
            error = true;
            noError= false;
        }
    }
}
