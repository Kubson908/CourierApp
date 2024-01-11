using CourierAPI.Data;
using CourierAPI.Helpers;
using CourierAPI.Models;
using CourierAPI.Models.Dto;
using CourierAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourierAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AdminController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IUserService<AddDispatcherDto, LoginDto, Dispatcher> _dispatcherService;
    private readonly IUserService<AddCourierDto, LoginDto, Courier> _courierService;
    private readonly WorkService _workService;

    public AdminController(ApplicationDbContext context, IUserService<AddDispatcherDto, LoginDto, Dispatcher> dispatcherService,
        WorkService workService, IUserService<AddCourierDto, LoginDto, Courier> courierService)
    {
        _context = context;
        _dispatcherService = dispatcherService;
        _workService = workService;
        _courierService = courierService;
    }

    [HttpGet("get-dispatchers"), Authorize(Roles = "Admin")]
    public ActionResult GetDispatchers()
    {
        var dispatchers = _dispatcherService.GetUsers().Select(d => new
        {
            d.Id,
            d.FirstName,
            d.LastName,
            d.UserName,
            d.Email,
            d.PhoneNumber,
        });
        return Ok(dispatchers);
    }
    
    [HttpGet("get-couriers"),  Authorize(Roles = "Admin,Dispatcher")]
    public ActionResult GetCouriers()
    {
        var couriers = _courierService.GetUsers().Select(d => new
        {
            d.Id,
            d.FirstName,
            d.LastName,
            d.UserName,
            d.Email,
            d.PhoneNumber,
            Status = _workService.WorkTimes.Any(w => w.CourierId == d.Id) ? 
                    _workService.WorkTimes.First(w => w.CourierId == d.Id).Status : Models.Dto.WorkStatus.Inactive,
        });
        return Ok(couriers);
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

    [HttpDelete("delete-courier/{id}"), Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteCourier([FromRoute] string id)
    {
        ApiUserResponse result = await _courierService.DeleteUserAsync(id);
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpDelete("delete-dispatcher/{id}"), Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteDispatcher([FromRoute] string id)
    {
        ApiUserResponse result = await _dispatcherService.DeleteUserAsync(id);
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpPatch("update-courier/{id}"), Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateCourier([FromRoute] string id, [FromBody] UpdateUserDto dto)
    {
        ApiUserResponse result = await _courierService.UpdateUserAsync(id, dto);
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpPatch("update-dispatcher/{id}"), Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateDispatcher([FromRoute] string id, [FromBody] UpdateUserDto dto)
    {
        ApiUserResponse result = await _dispatcherService.UpdateUserAsync(id, dto);
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpPatch("reset-dispatcher-password/{id}")]
    public async Task<IActionResult> ResetDispatcherPassword([FromRoute] string id, [FromBody] ResetPasswordDto dto)
    {
        if (dto.Password == null || dto.ConfirmPassword == null || dto.Password != dto.ConfirmPassword)
            return BadRequest();
        ApiUserResponse response = await _dispatcherService.ResetPassword(id, dto.Password);
        if (response.Exception)
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        if (response.IsSuccess) return Ok(response);
        return BadRequest(response);
    }

    [HttpPatch("reset-courier-password/{id}")]
    public async Task<IActionResult> ResetCourierPassword([FromRoute] string id, [FromBody] ResetPasswordDto dto)
    {
        if (dto.Password == null || dto.ConfirmPassword == null || dto.Password != dto.ConfirmPassword)
            return BadRequest();
        ApiUserResponse response = await _courierService.ResetPassword(id, dto.Password);
        if (response.Exception)
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        if (response.IsSuccess) return Ok(response);
        return BadRequest(response);
    }

    [HttpPatch("update-price-list"), Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdatePriceList([FromBody] PriceList newPriceList)
    {
        var priceList = await _context.PriceList.FirstAsync();
        priceList.VerySmallSize = newPriceList.VerySmallSize;
        priceList.SmallSize = newPriceList.SmallSize;
        priceList.MediumSize = newPriceList.MediumSize;
        priceList.LargeSize = newPriceList.LargeSize;
        priceList.LightWeight = newPriceList.LightWeight;
        priceList.MediumWeight = priceList.MediumWeight;
        priceList.HeavyWeight = priceList.HeavyWeight;
        PriceListHelper.PriceList = newPriceList;
        await _context.SaveChangesAsync();
        return Ok(priceList);
    }
}
