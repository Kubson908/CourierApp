using CourierMobileApp.Models.Dto;
using CourierMobileApp.Services;
using CourierMobileApp.View;

namespace CourierMobileApp.ViewModels;

public partial class LoginViewModel : BaseViewModel
{
    LoginService loginService;
    [ObservableProperty]
    public string login;
    [ObservableProperty]
    public string password;

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
        if (Loading) return;

        try
        {
            Loading = true;
            ApiUserResponse response = await loginService.LoginAsync(new LoginDto
            {
                Login = Login,
                Password = Password
            });
            if (!response.IsSuccess)
            {
                throw new Exception(response.Message, new Exception(response.Errors.First()));
            }
            await SecureStorage.Default.SetAsync("access_token", response.AccessToken);
        } catch (Exception ex)
        {
            Debug.WriteLine(ex);
            await Shell.Current.DisplayAlert("B³¹d logowania", ex.Message, "OK");
        }
        finally
        {
            Loading = false;
            var authenticated = await SecureStorage.Default.GetAsync("access_token");
            if (authenticated is not null)
            {
                await Shell.Current.GoToAsync($"/{nameof(LoadingPage)}");
            }
        }
    }
}