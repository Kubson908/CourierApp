using CourierAPI.Helpers;
using CourierAPI.Models;
using CourierAPI.Models.Dto;
using CourierAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CourierAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IUserService<AddCourierDto, LoginDto, Courier> _courierService;
    private readonly IUserService<AddDispatcherDto, LoginDto, Dispatcher> _dispatcherService;
    private readonly IUserService<RegisterDto, LoginDto, Customer> _customerService;
    private readonly AdminService _adminService;

    public AuthController(IUserService<AddCourierDto, LoginDto, Courier> courierService,
        IUserService<AddDispatcherDto, LoginDto, Dispatcher> dispatcherService,
        IUserService<RegisterDto, LoginDto, Customer> customerService,
        AdminService adminService)
    {
        _courierService = courierService;
        _dispatcherService = dispatcherService;
        _customerService = customerService;
        _adminService = adminService;
    }

    [HttpPost("register-customer")]
    public async Task<IActionResult> RegisterCustomerAsync(RegisterDto registerDto)
    {
        if (!ModelState.IsValid)
        {
            return UnprocessableEntity(ModelState);
        }
        var result = await _customerService.RegisterAsync(registerDto);

        if (result.IsSuccess) return Ok(result);

        return BadRequest(result);
    }

    [HttpGet("resend-email")]
    public async Task<IActionResult> ResendEmail([FromQuery] string email)
    {
        await _customerService.ResendConfirmationLinkAsync(email);
        return Ok();
    }

    [HttpPatch("confirm-email")]
    public async Task<IActionResult> ConfirmEmail()
    {
        var token = HttpContext.Request.Headers.Authorization.ToString().Replace(" ", "+");
        ApiUserResponse result = await _customerService.ConfirmEmail(token!);
        if (result.IsSuccess) return Ok(result);
        return BadRequest(result);
    }

    [HttpGet("reset-password-email")]
    public async Task<IActionResult> SendResetPasswordLink([FromQuery] string email)
    {
        ApiUserResponse dispatcherCheck = await _dispatcherService.SendResetPasswordLinkAsync(email);
        if (dispatcherCheck.IsSuccess)
            return StatusCode(StatusCodes.Status423Locked, dispatcherCheck);
        ApiUserResponse res = await _customerService.SendResetPasswordLinkAsync(email);
        if (res.IsSuccess) return Ok(res);
        return BadRequest(res);
    }

    [HttpPatch("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        if (dto.Password == null || dto.ConfirmPassword == null || dto.Password != dto.ConfirmPassword)
            return BadRequest();
        var token = HttpContext.Request.Headers.Authorization.ToString().Replace(" ", "+");
        ApiUserResponse result = await _customerService.ResetPassword(token!, dto.Password!);
        if (result.IsSuccess) return Ok(result);
        return BadRequest(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginDto loginDto)
    {
        if (!ModelState.IsValid)
        {
            return UnprocessableEntity(ModelState);
        }
        if (AdminHelper.AdminLogin is null)
        {
            await _adminService.SetAdminLogin();
        }
        ApiUserResponse result;
        if (loginDto.Login == AdminHelper.AdminLogin)
        {
            result = await _adminService.LoginAsync(loginDto);
        }
        else
        {
            result = await _dispatcherService.LoginAsync(loginDto);
            if (!result.IsSuccess)
            {
                result = await _customerService.LoginAsync(loginDto);
                if (!result.IsSuccess && result.Message == "Email not confirmed")
                    return StatusCode(StatusCodes.Status403Forbidden, result.Message);
            }
        }
        if (result.IsSuccess) return Ok(result);
        return Unauthorized(result);
    }

    [HttpPost("login-courier")]
    public async Task<IActionResult> LoginCourierAsync([FromBody] LoginDto model)
    {
        if (!ModelState.IsValid)
        {
            return UnprocessableEntity(ModelState);
        }
        var result = await _courierService.LoginAsync(model);

        if (result.IsSuccess)
            return Ok(result);

        return Unauthorized(result);
    }

    [HttpGet("refresh-token"), Authorize]
    public async Task<IActionResult> RefreshToken()
    {
        string id = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        string role = HttpContext.User.FindFirstValue(ClaimTypes.Role)!;
        ApiUserResponse response = role switch
        {
            "Dispatcher" => await _dispatcherService.RefreshToken(id),
            "Customer" => await _customerService.RefreshToken(id),
            "Courier" => await _courierService.RefreshToken(id),
            "Admin" => await _adminService.RefreshToken(id),
            _ => new ApiUserResponse
            {
                IsSuccess = false,
                Message = "Role does not exist",
            },
        };
        if (response.IsSuccess) return Ok(response);
        return BadRequest(response);
    }

    [HttpGet("get-profile-info"), Authorize]
    public async Task<IActionResult> GetProfileInfo()
    {
        string role = HttpContext.User.FindFirstValue(ClaimTypes.Role)!;
        string id = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        IdentityUser? user = role switch
        {
            "Customer" => await _customerService.GetUserData(id),
            "Dispatcher" => await _dispatcherService.GetUserData(id),
            "Courier" => await _courierService.GetUserData(id),
            "Admin" => await _adminService.GetUserData(id),
            _ => null,
        };
        if (user != null)
            return Ok(new
            {
                email = user.Email,
                phoneNumber = user.PhoneNumber,
            });
        return BadRequest();
    }

    [HttpPatch("change-phone-number"), Authorize(Roles = "Customer,Dispatcher,Courier")]
    public async Task<IActionResult> ChangePhoneNumber([FromBody] ChangePhoneNumberDto dto)
    {
        string role = HttpContext.User.FindFirstValue(ClaimTypes.Role)!;
        string id = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        ApiUserResponse response = role switch
        {
            "Customer" => await _customerService.ChangePhoneNumberAsync(id, dto.PhoneNumber, dto.Password),
            "Dispatcher" => await _dispatcherService.ChangePhoneNumberAsync(id, dto.PhoneNumber, dto.Password),
            "Courier" => await _courierService.ChangePhoneNumberAsync(id, dto.PhoneNumber, dto.Password),
            _ => new ApiUserResponse
            {
                IsSuccess = false,
                Message = "Invalid role",
            },
        };
        if (response.IsSuccess) return Ok(response);
        if (response.Message == "Invalid password") return Unauthorized(response);
        if (response.Exception) return StatusCode(StatusCodes.Status500InternalServerError, response);
        return BadRequest(response);
    }

    [HttpPatch("change-password"), Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        if (dto == null || dto.OldPassword == null || dto.NewPassword == null)
            return BadRequest(new ApiUserResponse
            {
                IsSuccess = false,
                Message = "Password is required",
            });
        string role = HttpContext.User.FindFirstValue(ClaimTypes.Role)!;
        string id = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        ApiUserResponse response = role switch
        {
            "Dispatcher" => await _dispatcherService.ChangePasswordAsync(id, dto.OldPassword, dto.NewPassword),
            "Customer" => await _customerService.ChangePasswordAsync(id, dto.OldPassword, dto.NewPassword),
            "Courier" => await _courierService.ChangePasswordAsync(id, dto.OldPassword, dto.NewPassword),
            "Admin" => await _adminService.ChangePasswordAsync(id, dto.OldPassword, dto.NewPassword),
            _ => new ApiUserResponse
            {
                IsSuccess = false,
                Message = "Role does not exist",
            },
        };
        if (response.IsSuccess) return Ok(response);
        if (response.Message == "Invalid password") return Unauthorized(response);
        if (response.Exception) return StatusCode(StatusCodes.Status500InternalServerError, response);
        return BadRequest(response);
    }
}
