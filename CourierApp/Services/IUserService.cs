using CourierAPI.Models.Dto;

namespace CourierAPI.Services
{
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
    }
}
