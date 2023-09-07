using CourierMobileApp.Models.Dto;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourierMobileApp.Services;

public class LoginService
{
    ConnectionService connectionService;

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

        var response = await connectionService.SendAsync(HttpMethod.Post, "api/auth/login-courier", loginDto);
        if (!response.IsSuccessStatusCode)
            throw new Exception(await response.Content.ReadAsStringAsync());
        ApiUserResponse responseData = JsonConvert.DeserializeObject<ApiUserResponse>(await response.Content.ReadAsStringAsync());
        connectionService.Token = responseData.AccessToken;
        return responseData;
    }
}
