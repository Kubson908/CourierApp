using CourierAPI.Data;
using CourierAPI.Helpers;
using CourierAPI.Models;
using CourierAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PdfSharp.Pdf;
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
        IEnumerable<Shipment> shipments = _context.Shipments.Where(s => s.Status == Status.Registered || s.Status == Status.Stored);
        return Ok(shipments);
    }

    [HttpPost("register-shipments")]
    public async Task<IActionResult> RegisterShipments([FromBody] RegisterShipmentsDto dto)
    {
        
        if (dto.Shipments.Count() == 0 || dto.Shipments is null)
        {
            return BadRequest();
        }
        string id = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        List<int> ids = new();
        try
        {
            foreach (Shipment shipment in dto.Shipments)
            {
                shipment.CustomerId = id;
                await _context.Shipments.AddAsync(shipment);
            }
            await _context.SaveChangesAsync();
            ids = dto.Shipments.Select(s => s.Id).ToList();
            return StatusCode(StatusCodes.Status201Created, ids);
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

    

    [HttpPost("set-route")]
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
                    ShipmentId = shipment.Id,
                    Type = shipment.Status == 0 ? Models.Type.Pickup : Models.Type.Delivery
                };
                order++;
                await _context.RouteElements.AddAsync(element);
                var updateResult = await UpdateShipmentStatus(shipment.Id, shipment.Status == Status.Registered ? Status.Accepted : Status.InDelivery);
                if (!updateResult.IsSuccess) return StatusCode(StatusCodes.Status500InternalServerError, new ApiUserResponse
                {
                    Message = updateResult.Message,
                    IsSuccess = updateResult.IsSuccess,
                    Exception = updateResult.Exception,
                });
            }
            await _context.SaveChangesAsync();
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
            if (status == Status.NotDelivered)
                shipment.DeliveryAttempts++;
            if (shipment.DeliveryAttempts == 3 & status == Status.Stored)
                shipment.Status = Status.StoredToReturn;
            else shipment.Status = status;
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

    [HttpPatch("return-package/{id}")]
    public async Task<IActionResult> ReturnPackage([FromRoute] int id)
    {
        ApiUserResponse result = await UpdateShipmentStatus(id, Status.Returned);
        if (!result.IsSuccess) return BadRequest(result);
        else if (result.Exception) return StatusCode(StatusCodes.Status500InternalServerError, result);
        return Ok(result);
    }

    [HttpGet("get-unavailable-dates")]
    public async Task<IActionResult> GetUnavailableDates()
    {
        List<string> courierIds = _context.Couriers.Select(c => c.Id).ToList();
        DateOnly today = new();
        Dictionary<string, List<string>> dates = new();
        foreach (string id in courierIds)
        {
            dates.Add(id, new List<string>());
            await _context.RouteElements
                .ForEachAsync((element) =>
                {
                    Console.WriteLine(element.RouteDate);
                    if (DateOnly.Parse(element.RouteDate.ToString()).CompareTo(today) > 0 && !dates[id].Contains(element.RouteDate))
                        dates[id].Add(element.RouteDate);
                }, CancellationToken.None);
        }
        return Ok(dates);
    }

    [HttpGet("get-courier-route"), Authorize(Roles = "Courier")]
    public async Task<IActionResult> GetCourierRoute([FromQuery] DateOnly date)
    {
        string courierId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        string dateString = date.ToString("dd.MM.yyyy");

        var route = await _context.RouteElements.Where(r => r.RouteDate == dateString).Select(r => new
        {
            r.Id,
            r.RouteDate,
            r.Order,
            Shipment = new
            {
                r.Shipment!.Id,
                r.Shipment!.Status,
                r.Shipment!.PickupAddress,
                r.Shipment!.PickupApartmentNumber,
                r.Shipment!.PickupCity,
                r.Shipment!.PickupPostalCode,
                r.Shipment!.Size,
                r.Shipment!.Weight,
                r.Shipment!.RecipientName,
                r.Shipment!.RecipientPhoneNumber,
                r.Shipment!.RecipientAddress,
                r.Shipment!.RecipientApartmentNumber,
                r.Shipment!.RecipientCity,
                r.Shipment!.RecipientPostalCode,
                r.Shipment!.RecipientEmail,
                r.Shipment!.PickupDate,
                r.Shipment!.StoreDate,
                r.Shipment!.DeliveryDate,
                r.Shipment!.AdditionalDetails,
                Customer = new
                {
                    r.Shipment.Customer!.FirstName,
                    r.Shipment.Customer!.LastName,
                    r.Shipment.Customer!.PhoneNumber
                }
            }
        }).ToListAsync();
        return Ok(route);
    }

    [HttpGet("generate-label")]
    public IActionResult GeneratePDF([FromQuery] int[] idList)
    {
        List<LabelShipmentDto> shipments = _context.Shipments.Where(s => idList.Contains(s.Id)).Select(s => new LabelShipmentDto
        {
            Id = s.Id,
            Size = s.Size,
            Weight = s.Weight,
            CustomerEmail = s.Customer!.Email,
            CustomerPhone = s.Customer!.PhoneNumber,
            RecipientEmail = s.RecipientEmail,
            RecipientPhone = s.RecipientPhoneNumber,
        }).ToList();
        PdfDocument document = PDFLabelHelper.GeneratePDF(shipments);

        byte[]? response = null;
        using(MemoryStream ms = new())
        {
            document.Save(ms);
            response = ms.ToArray();
        }
        string Filename = "Label_" + idList[0] + ".pdf";
        return File(response, "application/pdf", Filename);
    }
}
