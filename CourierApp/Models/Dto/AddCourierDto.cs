using System.ComponentModel.DataAnnotations;

namespace CourierAPI.Models.Dto;

public class AddCourierDto
{
    [Required]
    [StringLength(50)]
    public required string FirstName { get; set; }

    [Required]
    [StringLength(50)]
    public required string LastName { get; set; }

    [Required]
    [StringLength(25)]
    public required string UserName { get; set; }

    [Required]
    [StringLength(50)]
    [EmailAddress]
    public required string Email { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 8)]
    public required string Password { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 8)]
    public required string ConfirmPassword { get; set; }

    [Required]
    [StringLength(12, MinimumLength = 9)]
    public required string PhoneNumber { get; set; }
}
