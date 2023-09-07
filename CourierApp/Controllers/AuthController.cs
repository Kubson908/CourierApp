using CourierAPI.Models.Dto;
using CourierAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace CourierAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private IUserService<RegisterDto, LoginDto> _courierService;
    private IUserService<RegisterDto, LoginDto> _dispatcherService;

    public AuthController(CourierService courierService, DispatcherService dispatcherService)
    {
        _courierService = courierService;
        _dispatcherService = dispatcherService;
    }


    [HttpPost("add-courier")]
    public async Task<IActionResult> AddCourier([FromBody] RegisterDto model)
    {
        if (!ModelState.IsValid)
        {
            return UnprocessableEntity(ModelState);
        }
        var result = await _courierService.RegisterAsync(model);

        if (result.IsSuccess) return Ok("Succes"); // Status code: 200

        return BadRequest(result);
    }

    [HttpPost("login-courier")]
    public async Task<IActionResult> LoginCourier([FromBody] LoginDto model)
    {
        if (!ModelState.IsValid)
        {
            return UnprocessableEntity(ModelState);
        }
        var result = await _courierService.LoginAsync(model);

        if (result.IsSuccess)
            return Ok(result);

        return Unauthorized("Login error");
    }
    
    [HttpPost("login-dispatcher")]
    public async Task<IActionResult> LoginDispatcher([FromBody] LoginDto model)
    {
        if (!ModelState.IsValid)
        {
            return UnprocessableEntity(ModelState);
        }
        var result = await _dispatcherService.LoginAsync(model);

        if (result.IsSuccess)
            return Ok(result);

        return Unauthorized("Login error");
    }
}
