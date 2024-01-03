using CourierAPI.Helpers;
using CourierAPI.Models.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CourierAPI.Services;

public class AdminService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IConfiguration _configuration;

    public AdminService(UserManager<IdentityUser> userManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    public async Task SetAdminLogin()
    {
        AdminHelper.AdminLogin = (await _userManager.FindByIdAsync("00000000-0000-0000-0000-000000000001"))?.UserName;
    }

    public async Task<ApiUserResponse> LoginAsync(LoginDto dto)
    {
        if (dto == null)
            throw new NullReferenceException("Object is null");

        var user = await _userManager.FindByNameAsync(dto.Login);
        if (user == null)
            return new ApiUserResponse
            {
                Message = "Invalid login or password",
                IsSuccess = false
            };

        var result = await _userManager.CheckPasswordAsync(user, dto.Password);

        if (!result)
            return new ApiUserResponse
            {
                Message = "Invalid login or password",
                IsSuccess = false
            };

        ApiUserResponse response = GenerateToken(dto.Login, user.Id);
        response.Roles = await _userManager.GetRolesAsync(user);
        response.User = user.UserName;
        return response;
    }

    private ApiUserResponse GenerateToken(string login, string id)
    {
        var claims = new[]
        {
            new Claim("Login", login),
            new Claim(ClaimTypes.NameIdentifier, id),
            new Claim(ClaimTypes.Role, "Admin")
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AuthSettings:Key"]!));

        var token = new JwtSecurityToken(
            issuer: _configuration["AuthSettings:Issuer"],
            audience: _configuration["AuthSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(4),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

        string tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        return new ApiUserResponse
        {
            Message = "Logged in",
            IsSuccess = true,
            AccessToken = tokenString,
            ExpireDate = token.ValidTo,
        };
    }

    public async Task<ApiUserResponse> RefreshToken(string id)
    {
        IdentityUser? user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return new ApiUserResponse
            {
                IsSuccess = false,
                Message = "User not found",
            };
        ApiUserResponse response = GenerateToken(user!.UserName!, user.Id);
        return response;
    }

}
