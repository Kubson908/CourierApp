using CourierMobileApp.Models.Dto;
using Newtonsoft.Json;
namespace CourierMobileApp.Services;

public class LoginService
{
    readonly ConnectionService connectionService;
    public LoginService(ConnectionService connectionService)
    {
        this.connectionService = connectionService;
    }

    public async Task<ApiUserResponse> LoginAsync(LoginDto loginDto)
    {
        if (loginDto == null || loginDto.Login.Length <= 0 || loginDto.Password.Length <= 0)
        {
            return new ApiUserResponse
            {
                Message = "Login error",
                IsSuccess = false,
                Errors = new List<string>
                {
                    "Enter your login details"
                }
            };
        }

        var response = await connectionService.SendAsync(HttpMethod.Post, "api/auth/login-courier", body: loginDto);
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            return new ApiUserResponse
            {
                IsSuccess = false,
                Message = "Invalid credentials",
                Errors = new List<string>() { "InvalidCredentials" },
            };
        ApiUserResponse responseData = JsonConvert.DeserializeObject<ApiUserResponse>(await response.Content.ReadAsStringAsync());
        connectionService.Token = responseData.AccessToken;
        return responseData;
    }
}
