using CourierAPI.Helpers;
using CourierAPI.Models.Dto;
using CourierAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourierAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private IUserService<AddCourierDto, LoginDto> _courierService;
    private IUserService<AddDispatcherDto, LoginDto> _dispatcherService;
    private IUserService<RegisterDto, LoginDto> _customerService;
    private AdminService _adminService;

    public AuthController(IUserService<AddCourierDto, LoginDto> courierService, 
        IUserService<AddDispatcherDto, LoginDto> dispatcherService, 
        IUserService<RegisterDto, LoginDto> customerService,
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
            result = await _adminService.LoginAsync(loginDto); // przetestować
        }
        else
        {
            result = await _dispatcherService.LoginAsync(loginDto);
            if (!result.IsSuccess)
            {
                result = await _customerService.LoginAsync(loginDto);
            }
        }
        if (result.IsSuccess) return Ok(result);
        return Unauthorized(result);
    }

    [HttpPost("add-courier"), Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddCourierAsync([FromBody] AddCourierDto model)
    {
        if (!ModelState.IsValid)
        {
            return UnprocessableEntity(ModelState);
        }
        var result = await _courierService.RegisterAsync(model);

        if (result.IsSuccess) return Ok(result);

        return BadRequest(result);
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

    [HttpPost("add-dispatcher"), Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddDispatcher([FromBody] AddDispatcherDto dto)
    {
        if (!ModelState.IsValid)
        {
            return UnprocessableEntity(ModelState);
        }
        var result = await _dispatcherService.RegisterAsync(dto);
        if (result.IsSuccess) return Ok(result);
        return BadRequest(result);
    }
}
