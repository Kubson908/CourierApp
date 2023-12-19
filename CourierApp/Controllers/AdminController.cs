using CourierAPI.Data;
using CourierAPI.Models;
using CourierAPI.Services;
using CourierAPI.Websocket;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CourierAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
/*[Authorize(Roles = "Admin")]*/
public class AdminController : ControllerBase
{
    private ApplicationDbContext _context;
    private UserManager<Dispatcher> _dispatcherManager;
    private UserManager<Courier> _courierManager;
    private WorkService _workService;

    public AdminController(ApplicationDbContext context,
        UserManager<Dispatcher> userManager,
        UserManager<Courier> courierManager,
        WorkService workService)
    {
        _context = context;
        _dispatcherManager = userManager;
        _courierManager = courierManager;
        _workService = workService;
    }

    [HttpGet("get-dispatchers")]
    public ActionResult GetDispatchers()
    {
        var dispatchers = _dispatcherManager.Users.ToList().Select(d => new
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
        var couriers = _courierManager.Users.ToList().Select(d => new
        {
            d.Id,
            d.FirstName,
            d.LastName,
            d.UserName,
            d.Email,
            d.PhoneNumber,
            Status = _workService.workTimes.Any(w => w.CourierId == d.Id) ? 
                    _workService.workTimes.First(w => w.CourierId == d.Id).Status : Models.Dto.WorkStatus.Inactive
        });
        return Ok(couriers);
    }


}
