using System.ComponentModel.DataAnnotations;

namespace CourierAPI.Models.Dto;

public class LoginDto
{
    [Required]
    [EmailAddress]
    public string Login { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}
