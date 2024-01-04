using CourierAPI.Models;
using CourierAPI.Models.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace CourierAPI.Services;

public class CustomerService : IUserService<RegisterDto, LoginDto, Customer>
{
    private readonly UserManager<Customer> _userManager;
    private readonly IConfiguration _configuration;
    private readonly EmailService _emailService;

    public CustomerService(UserManager<Customer> userManager, IConfiguration configuration, EmailService emailService)
    {
        _userManager = userManager;
        _configuration = configuration;
        _emailService = emailService;
    }

    public async Task<ApiUserResponse> RegisterAsync(RegisterDto dto)
    {
        if (dto == null)
            throw new NullReferenceException("Object is null");

        if (dto.Password != dto.ConfirmPassword)
            return new ApiUserResponse
            {
                Message = "Passwords don't match",
                IsSuccess = false,
            };

        var customer = new Customer
        {
            Email = dto.Email,
            UserName = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            PhoneNumber = dto.PhoneNumber,
            EmailConfirmed = false
        };

        var result = await _userManager.CreateAsync(customer, dto.Password);
        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(customer, "Customer");

            try
            {
                string token = await _userManager.GenerateEmailConfirmationTokenAsync(customer);
                await _emailService.SendRegisterConfirmationLink(customer.Email, token, customer.Id);
            }
            catch (Exception) { }

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

    public async Task ResendConfirmationLinkAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) return;
        string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        await _emailService.SendRegisterConfirmationLink(email, token, user.Id);
    }

    public async Task<ApiUserResponse> SendResetPasswordLinkAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) return new ApiUserResponse
        {
            IsSuccess = false,
            Message = "User not found"
        };
        string token = await _userManager.GeneratePasswordResetTokenAsync(user);
        await _emailService.SendResetPasswordLink(email, token, user.Id);
        return new ApiUserResponse
        {
            IsSuccess = true,
            Message = "Link sent"
        };
    }

    public async Task<ApiUserResponse> ResetPassword(string token, string newPassword)
    {
        try
        {
            var tokenString = token.Split("|")[0];
            var id = token.Split("|")[1];
            if (id == null)
                return new ApiUserResponse
                {
                    IsSuccess = false,
                    Message = "Invalid token",
                };
            var customer = await _userManager.FindByIdAsync(id);
            if (customer == null)
                return new ApiUserResponse
                {
                    IsSuccess = false,
                    Message = "User not found",
                };
            var result = await _userManager.ResetPasswordAsync(customer, tokenString, newPassword);
            if (!result.Succeeded)
            {
                return new ApiUserResponse
                {
                    IsSuccess = false,
                    Message = "Cannot reset user's password",
                };
            }
            return new ApiUserResponse
            {
                IsSuccess = true,
                Message = "Pasword reset"
            };
        } catch (Exception ex)
        {
            return new ApiUserResponse
            {
                IsSuccess = false,
                Exception = true,
                Message = ex.Message,
            };
        }
    }

    public async Task<ApiUserResponse> ConfirmEmail(string token)
    {
        try
        {
            var tokenString = token.Split("|")[0];
            var id = token.Split("|")[1];
            if (id == null)
                return new ApiUserResponse
                {
                    IsSuccess = false,
                    Message = "Invalid token",
                };
            var customer = await _userManager.FindByIdAsync(id);
            if (customer == null)
                return new ApiUserResponse
                {
                    IsSuccess = false,
                    Message = "User not found",
                };
            var result = await _userManager.ConfirmEmailAsync(customer, tokenString);
            if (!result.Succeeded)
            {
                return new ApiUserResponse
                {
                    IsSuccess = false,
                    Message = "Cannot confirm user's email",
                };
            }
            return new ApiUserResponse
            {
                IsSuccess = true,
                Message = "Email confirmed"
            };

        } catch (Exception ex)
        {
            return new ApiUserResponse
            {
                IsSuccess = false,
                Exception = true,
                Message = ex.Message,
            };
        }
    }

    private static bool IsValidEmail(string email)
    {
        string regex = @"^[^@\s]+@[^@\s]+\.(com|net|org|gov|pl)$";
        return Regex.IsMatch(email, regex, RegexOptions.IgnoreCase);
    }

    public async Task<ApiUserResponse> LoginAsync(LoginDto dto)
    {
        if (dto == null)
            throw new NullReferenceException("Object is null");
        Customer? user;
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

        if (!user.EmailConfirmed)
            return new ApiUserResponse
            {
                IsSuccess = false,
                Message = "Email not confirmed"
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

    public ApiUserResponse GenerateToken(string login, string id)
    {
        var claims = new[]
        {
            new Claim("Login", login),
            new Claim(ClaimTypes.NameIdentifier, id),
            new Claim(ClaimTypes.Role, "Customer")
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

    public List<Customer> GetUsers()
    {
        throw new NotImplementedException();
    }

    public Task<ApiUserResponse> UpdateUserAsync(string id, UpdateUserDto dto)
    {
        throw new NotImplementedException();
    }

    public async Task<ApiUserResponse> RefreshToken(string id)
    {
        Customer? user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return new ApiUserResponse
            {
                IsSuccess = false,
                Message = "User not found",
            };
        ApiUserResponse response = GenerateToken(user!.UserName!, user.Id);
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
            Customer? user = await _userManager.FindByIdAsync(id);
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

    public async Task<ApiUserResponse> ChangePasswordAsync(string id, string oldPassword, string newPassword)
    {
        try
        {
            Customer? user = await _userManager.FindByIdAsync(id);
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
}
