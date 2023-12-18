 using CourierMobileApp.Models.Dto;
using CourierMobileApp.Services;
using CourierMobileApp.View;
using IntelliJ.Lang.Annotations;
using System.Net.Mail;

namespace CourierMobileApp.ViewModels;

public partial class LoginViewModel : BaseViewModel
{
    LoginService loginService;
    [ObservableProperty]
    public string login;
    [ObservableProperty]
    public string password;
    [ObservableProperty]
    public string loginValidator;

    private readonly ProfileService profileService;

    public LoginViewModel(LoginService loginService, ProfileService profileService)
    {
        Title = "Logowanie";
        this.loginService = loginService;
        Login = string.Empty;
        Password = string.Empty;
        this.profileService = profileService;
    }

    [RelayCommand]
    async Task LoginAsync()
    {
        await ValidateEmail();
        if (IsBusy || LoginValidator is not null) return;

        try
        {
            Platforms.KeyboardHelper.HideKeyboard();
            IsBusy = true;
            if (Password.Length < 8)
                throw new Exception("Niepoprawne dane logowania", null);
            ApiUserResponse response = await loginService.LoginAsync(new LoginDto
            {
                Login = Login,
                Password = Password
            });
            if (!response.IsSuccess)
            {
                throw new Exception(response.Errors.Contains("InvalidCredentials") ? "Niepoprawne dane logowania" : "Wyst¹pi³ b³¹d serwera", null);
            }
            await SecureStorage.Default.SetAsync("access_token", response.AccessToken);
            await SecureStorage.Default.SetAsync("user", response.User);
            await SecureStorage.Default.SetAsync("email", response.Email);
            if (!string.IsNullOrEmpty(response.Image))
            {
                await SecureStorage.Default.SetAsync("profile_image", response.Image);
                profileService.SetImage();
            }
                
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            await Shell.Current.DisplayAlert("B³¹d logowania", ex.Message, "OK");
        }
        finally
        {
            var authenticated = await SecureStorage.Default.GetAsync("access_token");
            if (authenticated is not null)
            {
                await Shell.Current.GoToAsync($"/{nameof(MainPage)}");
            }
            IsBusy = false;
        }
    }
    [RelayCommand]
    Task ValidateEmail()
    {
        if (Login == string.Empty)
        {
            LoginValidator = "Podaj e-mail";
            return Task.CompletedTask;
        }
        try
        {
            _ = new MailAddress(Login);
            LoginValidator = null;
            return Task.CompletedTask;
        }
        catch (FormatException)
        {
            LoginValidator = "Niepoprawny adres e-mail";
            return Task.CompletedTask;
        }
    }

    // TODO: Dodaæ funkcjê "Nie pamiêtam has³a"

    public async Task VerifyOnAppearing()
    {
        IsBusy = true;
        var authenticated = await SecureStorage.Default.GetAsync("access_token");
        if (authenticated is not null)
        {
            await Shell.Current.GoToAsync($"/{nameof(MainPage)}");
        }
        IsBusy = false;
    }
}