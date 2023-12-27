using CourierAPI.Models;
using CourierAPI.Models.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace CourierAPI.Services;

public class DispatcherService : IUserService<AddDispatcherDto, LoginDto>
{
    private readonly UserManager<Dispatcher> _userManager;
    private readonly IConfiguration _configuration;

    public DispatcherService(UserManager<Dispatcher> userManager, 
        IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    public async Task<ApiUserResponse> RegisterAsync(AddDispatcherDto dto)
    {
        if (dto == null)
            throw new NullReferenceException("Object is null");

        if (dto.Password != dto.ConfirmPassword)
            return new ApiUserResponse
            {
                Message = "Passwords don't match",
                IsSuccess = false,
            };

        var dispatcher = new Dispatcher
        {
            Email = dto.Email,
            UserName = dto.UserName,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            PhoneNumber = dto.PhoneNumber,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(dispatcher, dto.Password);
        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(dispatcher, "Dispatcher");

            return new ApiUserResponse
            {
                Message = "Success",
                IsSuccess = true,
            };

        }

        return new ApiUserResponse
        {
            Message = "Failed to create user",
            IsSuccess = false,
            Errors = result.Errors.Select(e => e.Description)
        };
    }

    private bool IsValidEmail(string email)
    {
        string regex = @"^[^@\s]+@[^@\s]+\.(com|net|org|gov)$";
        return Regex.IsMatch(email, regex, RegexOptions.IgnoreCase);
    }

    public async Task<ApiUserResponse> LoginAsync(LoginDto dto)
    {
        if (dto == null)
            throw new NullReferenceException("Object is null");
        Dispatcher? user;    
        if (IsValidEmail(dto.Login))
            user = await _userManager.FindByEmailAsync(dto.Login);
        else
            user = await _userManager.FindByNameAsync(dto.Login);

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
        response.User = user.FirstName + " " + user.LastName;
        return response;
    }

    private ApiUserResponse GenerateToken(string login, string id)
    {
        var claims = new[]
        {
            new Claim("Login", login),
            new Claim(ClaimTypes.NameIdentifier, id),
            new Claim(ClaimTypes.Role, "Dispatcher")
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AuthSettings:Key"]!));

        var token = new JwtSecurityToken(
            issuer: _configuration["AuthSettings:Issuer"],
            audience: _configuration["AuthSettings:Audience"],
            claims: claims,
            expires: DateTime.Now.AddDays(7),
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

    public Task<ApiUserResponse> DeleteUserAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task<ApiUserResponse> ConfirmEmail(string token)
    {
        throw new NotImplementedException();
    }

    public Task ResendConfirmationLinkAsync(string email)
    {
        throw new NotImplementedException();
    }

    public Task<ApiUserResponse> SendResetPasswordLinkAsync(string email)
    {
        throw new NotImplementedException();
    }

    public Task<ApiUserResponse> ResetPassword(string token, string newPassword)
    {
        throw new NotImplementedException();
    }
}
