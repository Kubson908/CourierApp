﻿using CourierAPI.Data;
using CourierAPI.Models;
using CourierAPI.Models.Dto;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace CourierAPI.Services;

public partial class DispatcherService : IUserService<AddDispatcherDto, LoginDto, Dispatcher>
{
    private readonly UserManager<Dispatcher> _userManager;
    private readonly IConfiguration _configuration;
    private readonly ApplicationDbContext _context;

    public DispatcherService(UserManager<Dispatcher> userManager, 
        IConfiguration configuration, ApplicationDbContext context)
    {
        _userManager = userManager;
        _configuration = configuration;
        _context = context;
    }

    public List<Dispatcher> GetUsers()
    {
        return _userManager.Users.ToList();
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

    private static bool IsValidEmail(string email)
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

    public async Task<ApiUserResponse> DeleteUserAsync(string id)
    {
        try
        {
            Dispatcher? user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return new ApiUserResponse
                {
                    IsSuccess = false,
                    Message = "User not found",
                };
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
                return new ApiUserResponse
                {
                    IsSuccess = true,
                    Message = "User has been deleted",
                };
            return new ApiUserResponse
            {
                IsSuccess = false,
                Message = "Cannot delete user",
            };
        }
        catch (Exception ex)
        {
            return new ApiUserResponse
            {
                IsSuccess = false,
                Message = ex.Message,
                Exception = true,
            };
        }
    }

    public Task<ApiUserResponse> ConfirmEmail(string token)
    {
        throw new NotImplementedException();
    }

    public Task ResendConfirmationLinkAsync(string email)
    {
        throw new NotImplementedException();
    }

    public async Task<ApiUserResponse> SendResetPasswordLinkAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user != null)
            return new ApiUserResponse
            {
                IsSuccess = true,
                Message = "The user with this email is the dispatcher",
            };
        return new ApiUserResponse
        {
            IsSuccess = false,
            Message = "Dispatcher with this email does not exist",
        };
    }

    private static bool VerifyPassword(string password)
    {
        if (password.Length < 8) return false;
        if (!DigitRegex().IsMatch(password)) return false;
        if (!CapitalLetterRegex().IsMatch(password)) return false;
        return true;
    }

    public async Task<ApiUserResponse> ResetPassword(string id, string newPassword)
    {
        Dispatcher? dispatcher = await _userManager.FindByIdAsync(id);
        if (dispatcher == null) return new ApiUserResponse
        {
            IsSuccess = false,
            Message = "User not found"
        };
        try
        {
            if (!VerifyPassword(newPassword))
                return new ApiUserResponse
                {
                    IsSuccess = false,
                    Message = "Incorrect password"
                };
            dispatcher.PasswordHash = new PasswordHasher<Dispatcher>().HashPassword(dispatcher, newPassword);
            await _context.SaveChangesAsync();
            return new ApiUserResponse
            {
                IsSuccess = true,
                Message = "Password has been changed"
            };
        } catch (Exception ex)
        {
            return new ApiUserResponse
            {
                IsSuccess = false,
                Message = ex.Message,
                Exception = true
            };
        }
    }

    public async Task<ApiUserResponse> UpdateUserAsync(string id, UpdateUserDto dto)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) 
                return new ApiUserResponse
                {
                    IsSuccess = false,
                    Message = "User not found",
                };
            foreach (var prop in typeof(Dispatcher).GetProperties())
            {
                var fromProp = typeof(UpdateUserDto).GetProperty(prop.Name);
                var toValue = fromProp != null ? fromProp.GetValue(dto, null) : null;
                if (toValue != null)
                {
                    prop.SetValue(user, toValue, null);
                    _context.Entry(user).Property(prop.Name).IsModified = true;
                }
            }
            foreach (var prop in typeof(IdentityUser).GetProperties())
            {
                var fromProp = typeof(UpdateUserDto).GetProperty(prop.Name);
                var toValue = fromProp != null ? fromProp.GetValue(dto, null) : null;
                if (toValue != null)
                {
                    prop.SetValue(user, toValue, null);
                    _context.Entry(user).Property(prop.Name).IsModified = true;
                    if (prop.Name == "Email")
                    {
                        user.NormalizedEmail = toValue?.ToString()?.ToUpper();
                    }
                }
            }
            await _context.SaveChangesAsync();
            return new ApiUserResponse
            {
                IsSuccess = true,
                Message = "User updated",
            };
        } catch (Exception ex)
        {
            return new ApiUserResponse
            {
                IsSuccess = false,
                Message = ex.Message,
                Exception = true,
            };
        }
    }

    public async Task<ApiUserResponse> RefreshToken(string id)
    {
        Dispatcher? user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return new ApiUserResponse
            {
                IsSuccess = false,
                Message = "User not found",
            };
        ApiUserResponse response = GenerateToken(user.UserName!, user.Id);
        return response;
    }

    public async Task<IdentityUser?> GetUserData(string id)
    {
        IdentityUser? user = await _userManager.FindByIdAsync(id);
        return user;
    }

    public async Task<ApiUserResponse> ChangePhoneNumberAsync(string id, string phoneNumber, string password)
    {
        try
        {
            Dispatcher? user = await _userManager.FindByIdAsync(id);
            if (user == null) return new ApiUserResponse
            {
                IsSuccess = false,
                Message = "User not found",
            };
            if (!await _userManager.CheckPasswordAsync(user, password))
                return new ApiUserResponse
                {
                    IsSuccess = false,
                    Message = "Invalid password"
                };
            var result = await _userManager.SetPhoneNumberAsync(user, phoneNumber);
            if (!result.Succeeded) return new ApiUserResponse
            {
                IsSuccess = false,
                Message = "Could not change phone number",
            };
            return new ApiUserResponse
            {
                IsSuccess = true,
                Message = "Phone number has been changed",
            };
        }
        catch (Exception ex)
        {
            return new ApiUserResponse
            {
                IsSuccess = false,
                Message = ex.Message,
                Exception = true,
            };
        }
    }

    public async Task<ApiUserResponse> ChangePasswordAsync(string id, string oldPassword, string newPassword)
    {
        try
        {
            Dispatcher? user = await _userManager.FindByIdAsync(id);
            if (user == null) return new ApiUserResponse
            {
                IsSuccess = false,
                Message = "User not found",
            };
            var result = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
            if (result.Succeeded) return new ApiUserResponse
            {
                IsSuccess = true,
                Message = "Password has been changed",
            };
            return new ApiUserResponse
            {
                IsSuccess = false,
                Message = "Invalid password",
            };
        }
        catch (Exception ex)
        {
            return new ApiUserResponse
            {
                IsSuccess = false,
                Message = ex.Message,
                Exception = true,
            };
        }
    }

    [GeneratedRegex("\\d")]
    private static partial Regex DigitRegex();
    [GeneratedRegex("[A-Z]")]
    private static partial Regex CapitalLetterRegex();
}
