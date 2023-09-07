using CourierAPI.Models.Dto;

namespace CourierAPI.Services
{
    public interface IUserService<R, L>
    {
        Task<ApiUserResponse> RegisterAsync(R dto);
        Task<ApiUserResponse> LoginAsync(L dto);
        Task<ApiUserResponse> DeleteUserAsync(string id);
    }
}
