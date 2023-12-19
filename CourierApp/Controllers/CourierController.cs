using CourierAPI.Data;
using CourierAPI.Models.Dto;
using CourierAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace CourierAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CourierController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;
    private readonly WorkService _workService;

    public CourierController(ApplicationDbContext context, 
        IWebHostEnvironment environment, WorkService workService)
    {
        _context = context;
        _environment = environment;
        _workService = workService;
    }

    [HttpGet("check-in"), Authorize(Roles = "Courier")]
    public IActionResult CheckIn()
    {
        string id = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        if (_workService.CheckWorkStatus(id))
        {
            _workService.CheckIn(id);
        }
        else
        {
            _workService.AddWorkTime(id);
        }
        return Ok(new { message = "Checked in", data = id});
    }

    [HttpDelete("end-route"), Authorize(Roles = "Courier")]
    public IActionResult EndRoute()
    {
        string id = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        try
        {
            if (_workService.CheckWorkStatus(id))
            {
                _workService.DeleteWorkTime(id);
            }
            
            return Ok(new { message = "Route ended", data = id });
        } catch
        {
            return BadRequest(new ApiUserResponse
            {
                Message = "Something went wrong",
                IsSuccess = false,
            });
        }
        
    }

    [HttpPost("upload-image"), Authorize(Roles = "Courier")]
    public async Task<IActionResult> UploadImage([FromForm] IFormFile postedFile)
    {
        if (postedFile == null) return BadRequest("File is null");
        string fileName = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)! + ".png";
        string path = Path.Combine(_environment.ContentRootPath, "StaticFiles");
        if (postedFile.Length > 0)
        {
            string upload = Path.Combine(path, fileName);
            using (Stream fileStream = new FileStream(upload, FileMode.Create))
            {
                await postedFile.CopyToAsync(fileStream);
            }
            var imageBytes = await System.IO.File.ReadAllBytesAsync(upload);
            ApiUserResponse response = new()
            {
                Message = "Image updated",
                IsSuccess = true,
                Image = Convert.ToBase64String(imageBytes)
            };
            return Ok(response);
        }
        return BadRequest("File is empty");
    }
}
