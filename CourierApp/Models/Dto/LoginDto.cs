using System.ComponentModel.DataAnnotations;

namespace CourierAPI.Models.Dto;

public class LoginDto
{
    [Required]
    [StringLength(50)]
    public string Login { get; set; } = string.Empty;

    [Required]
    [StringLength(50, MinimumLength = 8)]
    public string Password { get; set; } = string.Empty;
}
