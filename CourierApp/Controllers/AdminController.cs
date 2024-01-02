using CourierAPI.Data;
using CourierAPI.Helpers;
using CourierAPI.Models;
using CourierAPI.Models.Dto;
using CourierAPI.Services;
using CourierAPI.Websocket;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourierAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
/*[Authorize(Roles = "Admin")]*/
public class AdminController : ControllerBase
{
    private ApplicationDbContext _context;
    private readonly IUserService<AddDispatcherDto, LoginDto, Dispatcher> _dispatcherService;
    private readonly IUserService<AddCourierDto, LoginDto, Courier> _courierService;
    private WorkService _workService;

    public AdminController(ApplicationDbContext context, IUserService<AddDispatcherDto, LoginDto, Dispatcher> dispatcherService,
        WorkService workService, IUserService<AddCourierDto, LoginDto, Courier> courierService)
    {
        _context = context;
        _dispatcherService = dispatcherService;
        _workService = workService;
        _courierService = courierService;
    }

    [HttpGet("get-dispatchers")]
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
    
    [HttpGet("get-couriers")]
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
            Status = _workService.workTimes.Any(w => w.CourierId == d.Id) ? 
                    _workService.workTimes.First(w => w.CourierId == d.Id).Status : Models.Dto.WorkStatus.Inactive,
        });
        return Ok(couriers);
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

    [HttpPatch("update-price-list")]
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
