using CourierAPI.Data;
using CourierAPI.Models;
using CourierAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CourierAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ShipmentController : ControllerBase
{
    private ApplicationDbContext _context;

    public ShipmentController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("get-registered-shipments")]
    public IActionResult GetRegisteredShipments()
    {
        IEnumerable<Shipment> shipments = _context.Shipments.Where(s => s.Status == Status.Registered);
        return Ok(shipments);
    }

    /*[HttpPost("register-shipment")]
    public async Task<IActionResult> RegisterShipment([FromBody] Shipment shipment)
    {
        if (shipment == null)
        {
            return BadRequest();
        }
        await _context.Shipments.AddAsync(shipment);
        await _context.SaveChangesAsync();
        return StatusCode(StatusCodes.Status201Created, "Shipment registered");
    }*/

    [HttpPost("register-shipments")]
    public async Task<IActionResult> RegisterShipments([FromBody] IEnumerable<Shipment> shipments)
    {
        
        if (shipments.Count() == 0 || shipments is null)
        {
            return BadRequest();
        }
        string id = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        try
        {
            foreach (Shipment shipment in shipments)
            {
                shipment.CustomerId = id;
                await _context.Shipments.AddAsync(shipment);
            }
            await _context.SaveChangesAsync();
            return StatusCode(StatusCodes.Status201Created, new ApiUserResponse
            {
                Message = "Shipments registered",
                IsSuccess = true,
            });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiUserResponse
            {
                Message = ex.Message,
                IsSuccess = false,
                Exception = true
            });
        }
    }

    [HttpGet("get-shipments-by-user")]
    [Authorize(Roles = "Customer")]
    public IActionResult GetShipmentsByUser()
    {
        string id = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        IEnumerable<Shipment> shipments = _context.Shipments.Where(s => s.CustomerId == id);
        return Ok(shipments);
    }

    private async Task<ApiUserResponse> UpdateShipmentStatus(int id, Status status)
    {
        try
        {
            var shipment = await _context.Shipments.FindAsync(id);
            if (shipment == null) return new ApiUserResponse
            {
                Message = "Shipment not found",
                IsSuccess = false
            };
            shipment.Status = status;
            await _context.SaveChangesAsync();
            return new ApiUserResponse
            {
                Message = "Shipment status updated",
                IsSuccess = true,
            };
        }
        catch (Exception ex)
        {
            return new ApiUserResponse
            {
                Message = ex.Message,
                IsSuccess = false,
                Exception = true,
            };
        }
        
    }
    // TODO: dodać endpoint do przypisywania paczek do kuriera i zmiany statusu na Accepted dla nowych paczek <-- chyba zrobione ale przetestować
    [HttpPatch("set-route")]
    public async Task<IActionResult> SetRoute([FromBody] RouteDto dto)
    {
        try
        {
            int order = 1;
            foreach (var shipment in dto.Shipments)
            {
                RouteElement element = new RouteElement
                {
                    RouteDate = dto.Date.ToString(),
                    Order = order,
                    CourierId = dto.CourierId,
                    ShipmentId = shipment.Id
                };
                order++;
                await _context.RouteElements.AddAsync(element);
            }
            return Ok("Route created");
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiUserResponse
            {
                Message = ex.Message,
                IsSuccess = false,
                Exception = true
            });
        }
    }

    [HttpPatch("deliver-package/{id}")]
    public async Task<IActionResult> DeliverPackage([FromRoute] int id)
    {
        ApiUserResponse result = await UpdateShipmentStatus(id, Status.Delivered);
        if (!result.IsSuccess) return BadRequest(result);
        else if (result.Exception) return StatusCode(StatusCodes.Status500InternalServerError, result);
        return Ok(result);
    }

    [HttpPatch("pickup-package/{id}")]
    public async Task<IActionResult> PickupPackage([FromRoute] int id)
    {
        ApiUserResponse result = await UpdateShipmentStatus(id, Status.PickedUp);
        if (!result.IsSuccess) return BadRequest(result);
        else if (result.Exception) return StatusCode(StatusCodes.Status500InternalServerError, result);
        return Ok(result);
    }

    [HttpPatch("store-package/{id}")]
    public async Task<IActionResult> StorePackage([FromRoute] int id)
    {
        ApiUserResponse result = await UpdateShipmentStatus(id, Status.Stored);
        if (!result.IsSuccess) return BadRequest(result);
        else if (result.Exception) return StatusCode(StatusCodes.Status500InternalServerError, result);
        return Ok(result);
    }
}
