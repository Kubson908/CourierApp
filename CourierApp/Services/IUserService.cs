using CourierAPI.Models.Dto;
using Microsoft.AspNetCore.Identity;

namespace CourierAPI.Services;

public interface IUserService<R, L, U>
{
    Task<ApiUserResponse> RegisterAsync(R dto);
    Task<ApiUserResponse> LoginAsync(L dto);
    Task<ApiUserResponse> DeleteUserAsync(string id);
    Task<ApiUserResponse> ConfirmEmail(string token);
    Task ResendConfirmationLinkAsync(string email);
    Task<ApiUserResponse> SendResetPasswordLinkAsync(string email);
    Task<ApiUserResponse> ResetPassword(string tokenOrId, string newPassword);
    List<U> GetUsers();
    Task<ApiUserResponse> UpdateUserAsync(string id, UpdateUserDto dto);
    Task<ApiUserResponse> RefreshToken(string id);
    Task<IdentityUser?> GetUserData(string id);
    Task<ApiUserResponse> ChangePhoneNumberAsync(string id, string phoneNumber, string password);
    Task<ApiUserResponse> ChangePasswordAsync(string id, string oldPassword, string newPassword);
}
