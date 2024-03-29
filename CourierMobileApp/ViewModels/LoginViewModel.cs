using CourierMobileApp.Models.Dto;
using CourierMobileApp.Services;
using CourierMobileApp.View;
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

    public LoginViewModel(LoginService loginService)
    {
        Title = "Logowanie";
        this.loginService = loginService;
        Login = string.Empty;
        Password = string.Empty;
    }

    [RelayCommand]
    async Task LoginAsync()
    {
        await ValidateEmail();
        if (IsBusy || LoginValidator is not null || Password.Length <= 1) return;

        try
        {
            Platforms.KeyboardHelper.HideKeyboard();
            IsBusy = true;
            ApiUserResponse response = await loginService.LoginAsync(new LoginDto
            {
                Login = Login,
                Password = Password
            });
            if (!response.IsSuccess)
            {
                throw new Exception(response.Errors.Contains("InvalidCredentials") ? "Niepoprawne dane logowania" : "Wyst�pi� b��d serwera", null);
            }
            await SecureStorage.Default.SetAsync("access_token", response.AccessToken);
            await SecureStorage.Default.SetAsync("user", response.User);
            await SecureStorage.Default.SetAsync("email", response.Email);
            if (!string.IsNullOrEmpty(response.Image))
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
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            await Shell.Current.DisplayAlert("B��d logowania", ex.Message, "OK");
        }
        finally
        {
            Login = string.Empty;
            Password = string.Empty;
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