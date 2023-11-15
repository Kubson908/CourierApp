using CourierAPI.Data;
using CourierAPI.Models;
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

    public AdminController(ApplicationDbContext context,
        UserManager<Dispatcher> userManager,
        UserManager<Courier> courierManager)
    {
        _context = context;
        _dispatcherManager = userManager;
        _courierManager = courierManager;
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
        });
        return Ok(couriers);
    }


}
