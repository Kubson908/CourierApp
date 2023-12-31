using CourierAPI.Data;
using CourierAPI.Models;
using CourierAPI.Models.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CourierAPI.Services;

public class CourierService : IUserService<AddCourierDto, LoginDto, Courier>
{
    private readonly UserManager<Courier> _userManager;
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;
    private ApplicationDbContext _context;

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

        var claims = new[]
        {
            new Claim("Login", dto.Login),
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

        ApiUserResponse response = new ApiUserResponse
        {
            Message = "Logged in",
            IsSuccess = true,
            AccessToken = tokenString,
            ExpireDate = token.ValidTo,
            User = user.FirstName + " " + user.LastName,
            Email = user.Email,
            Roles = await _userManager.GetRolesAsync(user)
        };

        var path = Path.Combine(_environment.ContentRootPath, "StaticFiles", user.Id + ".png");

        if (File.Exists(path))
        {
            var imageBytes = await File.ReadAllBytesAsync(path);
            response.Image = Convert.ToBase64String(imageBytes);
        }

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

    public Task<ApiUserResponse> ResetPassword(string token, string newPassword)
    {
        throw new NotImplementedException();
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
}
