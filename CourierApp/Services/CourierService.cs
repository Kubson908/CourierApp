using CourierAPI.Data;
using CourierAPI.Models;
using CourierAPI.Models.Dto;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace CourierAPI.Services;

public class CourierService : IUserService<AddCourierDto, LoginDto, Courier>
{
    private readonly UserManager<Courier> _userManager;
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;
    private readonly ApplicationDbContext _context;

    public CourierService(UserManager<Courier> userManager,
        IConfiguration configuration, IWebHostEnvironment environment,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _configuration = configuration;
        _environment = environment;
        _context = context;
    }

    public List<Courier> GetUsers()
    {
        return _userManager.Users.ToList();
    }

    public async Task<ApiUserResponse> RegisterAsync(AddCourierDto dto)
    {
        if (dto == null)
            throw new NullReferenceException("Object is null");

        if (dto.Password != dto.ConfirmPassword)
            return new ApiUserResponse
            {
                Message = "Passwords don't match",
                IsSuccess = false,
            };

        var courier = new Courier
        {
            Email = dto.Email,
            UserName = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            PhoneNumber = dto.PhoneNumber,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(courier, dto.Password);
        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(courier, "Courier");

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

    public async Task<ApiUserResponse> LoginAsync(LoginDto dto)
    {
        if (dto == null)
            throw new NullReferenceException("Object is null");

        var user = await _userManager.FindByEmailAsync(dto.Login);

        if (user == null)
            return new ApiUserResponse
            {
                Message = "Invalid login or password",
                IsSuccess = false,
                Errors = new List<string>()
                {
                    "InvalidCredentials"
                }
            };

        var result = await _userManager.CheckPasswordAsync(user, dto.Password);

        if (!result)
            return new ApiUserResponse
            {
                Message = "Invalid login or password",
                IsSuccess = false,
                Errors = new List<string>()
                {
                    "InvalidCredentials"
                }
            };

        ApiUserResponse response = await GenerateToken(user);

        var path = Path.Combine(_environment.ContentRootPath, "StaticFiles", user.Id + ".png");

        if (File.Exists(path))
        {
            var imageBytes = await File.ReadAllBytesAsync(path);
            response.Image = Convert.ToBase64String(imageBytes);
        }

        return response;
    }

    private async Task<ApiUserResponse> GenerateToken(Courier user)
    {
        var claims = new[]
        {
            new Claim("Login", user.Email!),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Role, "Courier")
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AuthSettings:Key"]!));

        var token = new JwtSecurityToken(
            issuer: _configuration["AuthSettings:Issuer"],
            audience: _configuration["AuthSettings:Audience"],
            claims: claims,
            expires: DateTime.Now.AddDays(7),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

        string tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        ApiUserResponse response = new()
        {
            Message = "Logged in",
            IsSuccess = true,
            AccessToken = tokenString,
            ExpireDate = token.ValidTo,
            User = user.FirstName + " " + user.LastName,
            Email = user.Email,
            Roles = await _userManager.GetRolesAsync(user)
        };
        return response;
    }
    
    public async Task<ApiUserResponse> DeleteUserAsync(string id)
    {
        try
        {
            Courier? user = await _userManager.FindByIdAsync(id);
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
        } catch (Exception ex)
        {
            return new ApiUserResponse
            {
                IsSuccess = false,
                Message = ex.Message,
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

    public Task<ApiUserResponse> SendResetPasswordLinkAsync(string email)
    {
        throw new NotImplementedException();
    }

    private static bool VerifyPassword(string password)
    {
        if (password.Length < 8) return false;
        if (!Regex.IsMatch(password, @"\d")) return false;
        if (!Regex.IsMatch(password, @"[A-Z]")) return false;
        return true;
    }

    public async Task<ApiUserResponse> ResetPassword(string id, string newPassword)
    {
        Courier? courier = await _userManager.FindByIdAsync(id);
        if (courier == null) return new ApiUserResponse
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
            courier.PasswordHash = new PasswordHasher<Courier>().HashPassword(courier, newPassword);
            await _context.SaveChangesAsync();
            return new ApiUserResponse
            {
                IsSuccess = true,
                Message = "Password has been changed"
            };
        }
        catch (Exception ex)
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
            foreach (var prop in typeof(Courier).GetProperties())
            {
                var fromProp = typeof(UpdateUserDto).GetProperty(prop.Name);
                var toValue = fromProp?.GetValue(dto, null);
                if (toValue != null)
                {
                    prop.SetValue(user, toValue, null);
                    _context.Entry(user).Property(prop.Name).IsModified = true;
                }
            }
            foreach (var prop in typeof(IdentityUser).GetProperties())
            {
                var fromProp = typeof(UpdateUserDto).GetProperty(prop.Name);
                var toValue = fromProp?.GetValue(dto, null);
                if (toValue != null)
                {
                    prop.SetValue(user, toValue, null);
                    _context.Entry(user).Property(prop.Name).IsModified = true;
                    if (prop.Name == "UserName")
                    {
                        user.NormalizedUserName = toValue?.ToString()?.ToUpper();
                    }
                    else if (prop.Name == "Email")
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

    public async Task<ApiUserResponse> RefreshToken(string id)
    {
        Courier? user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return new ApiUserResponse
            {
                IsSuccess = false,
                Message = "User not found",
            };
        ApiUserResponse response = await GenerateToken(user!);
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
            Courier? user = await _userManager.FindByIdAsync(id);
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
            Courier? user = await _userManager.FindByIdAsync(id);
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
}
