using CourierMobileApp.Models.Dto;
using CourierMobileApp.Services;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace CourierMobileApp.ViewModels;

public partial class ChangePasswordViewModel : BaseViewModel
{
    public readonly ConnectionService connectionService;
    [ObservableProperty]
    string oldPassword;
    [ObservableProperty]
    string newPassword;
    [ObservableProperty]
    string confirmNewPassword;
    [ObservableProperty]
    string errorMessage;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(NotSuccess))]
    bool success;
    public bool NotSuccess => !Success;

    public ChangePasswordViewModel(ConnectionService connectionService)
    {
        this.connectionService = connectionService;
        ErrorMessage = null;
        Success = false;
    }

    private bool ValidatePassword()
    {
        if (OldPassword == null || OldPassword == string.Empty)
        {
            ErrorMessage = "Podaj dotychczasowe hasło";
            return false;
        }
        if (NewPassword == null || NewPassword == string.Empty)
        {
            ErrorMessage = "Podaj nowe hasło";
            return false;
        }
        if (ConfirmNewPassword == null || ConfirmNewPassword == string.Empty)
        {
            ErrorMessage = "Potwierdź nowe hasło";
            return false;
        }
        if (NewPassword.Length < 8 ||
            !Regex.IsMatch(NewPassword, @"\d") ||
            !Regex.IsMatch(NewPassword, @"[A-Z]"))
        {
            ErrorMessage = "Hasło powinno składać się z co najmniej 8 znaków i zawierać przynajmniej jedną wielkią literę i jedną cyfrę";
            return false;
        }
        if (NewPassword != ConfirmNewPassword)
        {
            ErrorMessage = "Hasła nie są identyczne";
            return false;
        }
        return true;
    }

    [RelayCommand]
    public async Task ChangePassword()
    {
        Platforms.KeyboardHelper.HideKeyboard();
        if (!ValidatePassword()) return;
        ErrorMessage = null;
        Success = false;
        IsBusy = true;
        try
        {
            ChangePasswordDto dto = new()
            {
                OldPassword = OldPassword,
                NewPassword = NewPassword,
            };
            var response = await connectionService.SendAsync(HttpMethod.Patch, "api/auth/change-password", body: dto);
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                IsBusy = false;
                ErrorMessage = "Podane dotychczasowe hasło jest nieprawidłowe";
                return;
            }
            ApiUserResponse responseData = JsonConvert.DeserializeObject<ApiUserResponse>(await response.Content.ReadAsStringAsync());
            if (responseData.IsSuccess)
            {
                Success = true;
                IsBusy = false;
                await Task.Delay(2000);
                await Shell.Current.Navigation.PopAsync();
                return;
            }
            IsBusy = false;
            ErrorMessage = "Wystąpił błąd serwera";
            return;
        } catch (Exception)
        {
            IsBusy = false;
            ErrorMessage = "Błąd z połączeniem";
            return;
        }
    }
}
