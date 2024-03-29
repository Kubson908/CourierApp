﻿using CourierAPI.Data;
using CourierAPI.Helpers;
using CourierAPI.Models;
using CourierAPI.Models.Dto;
using CourierAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace CourierAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ShipmentController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly EmailService emailService;

    public ShipmentController(ApplicationDbContext context, EmailService emailService)
    {
        _context = context;
        this.emailService = emailService;
    }

    [HttpGet("get-registered-shipments")]
    public IActionResult GetRegisteredShipments()
    {
        IEnumerable<Shipment> shipments = _context.Shipments.Where(s => s.Status == Status.Registered 
                                        || s.Status == Status.Stored || s.Status == Status.StoredToReturn);
        return Ok(shipments);
    }

    [HttpPost("register-shipments")]
    public async Task<IActionResult> RegisterShipments([FromBody] RegisterShipmentsDto dto)
    {
        PriceListHelper.PriceList ??= await _context.PriceList.FirstAsync();
        if (!dto.Shipments.Any() || dto.Shipments is null)
        {
            return BadRequest();
        }
        string id = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        List<int> ids = new();
        try
        {
            Customer? user = await _context.Customers.FindAsync(id);
            if (user == null)
            {
                return BadRequest(new ApiUserResponse
                {
                    IsSuccess = false,
                    Message = "Invalid user",
                });
            }
            List<LabelShipmentDto> labels = new();
            float price = 0;
            Order order = new()
            {
                OrderDate = DateTime.Now,
                CustomerId = id,
            };
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
            foreach (Shipment shipment in dto.Shipments)
            {
                var size = PriceListHelper.PriceList?.GetType().GetProperty(Enum.GetName(shipment.Size)! + "Size")?.GetValue(PriceListHelper.PriceList, null);
                var weight = PriceListHelper.PriceList?.GetType().GetProperty(Enum.GetName(shipment.Weight)! + "Weight")?.GetValue(PriceListHelper.PriceList, null);
                shipment.Price = (float)size! + (float)weight!;
                shipment.CustomerId = id;
                shipment.OrderId = order.Id;
                shipment.DeliveryAttempts = 0;
                await _context.Shipments.AddAsync(shipment);
                price += (float)size! + (float)weight!;
                labels.Add(new LabelShipmentDto
                {
                    Id = shipment.Id,
                    Size = shipment.Size,
                    Weight = shipment.Weight,
                    CustomerEmail = user.Email,
                    CustomerPhone = user.PhoneNumber,
                    RecipientEmail = shipment.RecipientEmail,
                    RecipientPhone = shipment.RecipientPhoneNumber,
                });
            }
            await _context.SaveChangesAsync();
            var shipments = dto.Shipments.ToList();
            for (int i = 0; i < labels.Count; i++)
            {
                labels[i].Id = shipments[i].Id;
            }
            try
            {
                await emailService.SendLabels(user.Email!, labels);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
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

    [HttpPost("repeat-order")]
    public async Task<IActionResult> RepeatOrder([FromBody] RegisterShipmentsDto dto)
    {
        PriceListHelper.PriceList ??= await _context.PriceList.FirstAsync();
        if (!dto.Shipments.Any() || dto.Shipments is null)
        {
            return BadRequest();
        }
        string id = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        List<int> ids = new();
        try
        {
            Order order = new()
            {
                OrderDate = DateTime.Now,
                CustomerId = id,
            };
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
            foreach (Shipment shipment in dto.Shipments)
            {
                Shipment newShipment = new()
                {
                    PickupAddress = shipment.PickupAddress,
                    PickupApartmentNumber = shipment.PickupApartmentNumber,
                    PickupCity = shipment.PickupCity,
                    PickupPostalCode = shipment.PickupPostalCode,
                    Size = shipment.Size,
                    Weight = shipment.Weight,
                    RecipientName = shipment.RecipientName,
                    RecipientPhoneNumber = shipment.RecipientPhoneNumber,
                    RecipientAddress = shipment.RecipientAddress,
                    RecipientApartmentNumber = shipment.RecipientApartmentNumber,
                    RecipientCity = shipment.RecipientCity,
                    RecipientPostalCode = shipment.RecipientPostalCode,
                    RecipientEmail = shipment.RecipientEmail,
                    AdditionalDetails = shipment.AdditionalDetails,
                };

                var size = PriceListHelper.PriceList?.GetType().GetProperty(Enum.GetName(shipment.Size)! + "Size")?.GetValue(PriceListHelper.PriceList, null);
                var weight = PriceListHelper.PriceList?.GetType().GetProperty(Enum.GetName(shipment.Weight)! + "Weight")?.GetValue(PriceListHelper.PriceList, null);
                newShipment.Price = (float)size! + (float)weight!;
                newShipment.CustomerId = id;
                newShipment.OrderId = order.Id;
                newShipment.DeliveryAttempts = 0;
                await _context.Shipments.AddAsync(newShipment);
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
            DateOnly date = DateOnly.FromDateTime(DateTime.Parse(dto.Date));
            int order = 1;
            foreach (var shipment in dto.Shipments)
            {
                RouteElement element = new()
                {
                    RouteDate = date.ToString("dd.MM.yyyy"),
                    Order = order,
                    CourierId = dto.CourierId,
                    ShipmentId = shipment.Id,
                    Type = shipment.Status == Status.Accepted ? Models.Type.Pickup : 
                        (shipment.Status == Status.Stored ? Models.Type.Delivery : Models.Type.Return)
                };
                order++;
                await _context.RouteElements.AddAsync(element);
                var updateResult = await UpdateShipmentStatus(
                    shipment.Id, shipment.Status == Status.Registered ? Status.Accepted : 
                    (shipment.Status == Status.Stored ? Status.InDelivery : Status.InReturn));
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

    private async Task DeleteRouteElement(int id)
    {
        _context.RouteElements.Remove(_context.RouteElements.First(r => r.ShipmentId == id));
        await _context.SaveChangesAsync();
    }

    [HttpPatch("deliver-package/{id}")]
    public async Task<IActionResult> DeliverPackage([FromRoute] int id)
    {
        ApiUserResponse result = await UpdateShipmentStatus(id, Status.Delivered);
        if (!result.IsSuccess) return BadRequest(result);
        else if (result.Exception) return StatusCode(StatusCodes.Status500InternalServerError, result);
        await DeleteRouteElement(id);
        return Ok(result);
    }

    [HttpPatch("pickup-package/{id}")]
    public async Task<IActionResult> PickupPackage([FromRoute] int id)
    {
        ApiUserResponse result = await UpdateShipmentStatus(id, Status.PickedUp);
        if (!result.IsSuccess) return BadRequest(result);
        else if (result.Exception) return StatusCode(StatusCodes.Status500InternalServerError, result);
        await DeleteRouteElement(id);
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
        await DeleteRouteElement(id);
        return Ok(result);
    }

    [HttpPatch("package-not-delivered/{id}")]
    public async Task<IActionResult> PackageNotDelivered([FromRoute] int id)
    {
        ApiUserResponse result = await UpdateShipmentStatus(id, Status.NotDelivered);
        if (!result.IsSuccess) return BadRequest(result);
        else if (result.Exception) return StatusCode(StatusCodes.Status500InternalServerError, result);
        await DeleteRouteElement(id);
        return Ok(result);
    }

    [HttpGet("get-unavailable-dates"), Authorize(Roles = "Dispatcher")]
    public async Task<IActionResult> GetUnavailableDates()
    {
        List<string> courierIds = _context.Couriers.Select(c => c.Id).ToList();
        DateOnly today = DateOnly.FromDateTime(DateTime.Today);
        Dictionary<string, List<string>> dates = new();
        foreach (string id in courierIds)
        {
            dates.Add(id, new List<string>());
            await _context.RouteElements
            .ForEachAsync((element) =>
            {
                DateTime dateTime = DateTime.ParseExact(element.RouteDate, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                    if (element.CourierId == id && 
                        DateOnly.FromDateTime(dateTime).CompareTo(today) > 0 && 
                        !dates[id].Contains(element.RouteDate))
                            dates[id].Add(element.RouteDate);
                }, CancellationToken.None);
        }
        return Ok(dates);
    }

    [HttpGet("get-courier-route"), Authorize(Roles = "Courier")]
    public async Task<IActionResult> GetCourierRoute([FromQuery] string date)
    {
        string courierId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        /*string dateString = date.ToString("dd.MM.yyyy");*/

        var route = await _context.RouteElements.Where(r => r.RouteDate == date && r.CourierId == courierId).Select(r => new
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
        }).Where(r => r.Shipment.Status == Status.Accepted || r.Shipment.Status == Status.InDelivery 
            || r.Shipment.Status == Status.InReturn).OrderBy(s => s.Order).ToListAsync();
        return Ok(route);
    }

    [HttpGet("generate-label")]
    public IActionResult GeneratePDF([FromQuery] int[] idList)
    {
        string userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        List<LabelShipmentDto> shipments = _context.Shipments.Where(s => s.CustomerId == userId && idList.Contains(s.Id)).Select(s => new LabelShipmentDto
        {
            Id = s.Id,
            Size = s.Size,
            Weight = s.Weight,
            CustomerEmail = s.Customer!.Email,
            CustomerPhone = s.Customer!.PhoneNumber,
            CustomerAddress = s.PickupAddress + (s.PickupApartmentNumber != null ? "/" + s.PickupApartmentNumber : "") + ", " + s.PickupCity,
            RecipientEmail = s.RecipientEmail,
            RecipientPhone = s.RecipientPhoneNumber,
            RecipientAddress = s.RecipientAddress + (s.RecipientApartmentNumber != null ? "/" + s.RecipientApartmentNumber : "") + ", " + s.RecipientCity,
        }).ToList();
        if (shipments.Count == 0) return BadRequest();
        FileInfoDto result = PDFLabelHelper.GenerateLabels(shipments);
        return File(result.Bytes, result.Type, result.Name);
    }

    [HttpGet("get-price-list")]
    public async Task<IActionResult> GetPriceList()
    {
        PriceListHelper.PriceList ??= await _context.PriceList.FirstAsync();
        return Ok(PriceListHelper.PriceList);
    }

    [HttpGet("get-orders"), Authorize(Roles = "Customer")]
    public async Task<IActionResult> GetOrders()
    {
        string id = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        try
        {
            var orders = await _context.Orders.Where(o => o.CustomerId == id).OrderByDescending(o => o.OrderDate).Select(o => new
            {
                o.Id,
                o.OrderDate,
                ShipmentCount = _context.Shipments.Count(s => s.OrderId == o.Id),
            }).ToListAsync();
            return Ok(orders);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiUserResponse
            {
                IsSuccess = false,
                Message = ex.Message,
                Exception = true,
            });
        }
    }

    [HttpGet("get-order-details/{id}")]
    public async Task<IActionResult> GetOrderDetails([FromRoute] int id)
    {
        string customerId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        try
        {
            Order? order = await _context.Orders.FirstOrDefaultAsync(o => o.CustomerId == customerId && o.Id == id);
            if (order == null)
                return BadRequest(new ApiUserResponse
                {
                    IsSuccess = false,
                    Message = "Order not found",
                });
            List<Shipment> shipments = await _context.Shipments.Where(s => s.OrderId == order.Id).ToListAsync();
            OrderDetailsDto orderDetails = new()
            {
                OrderDate = order.OrderDate,
                Shipments = shipments,
            };
            return Ok(orderDetails);
        } catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiUserResponse
            {
                IsSuccess = false,
                Message = ex.Message,
                Exception = true,
            });
        }
    }
}
