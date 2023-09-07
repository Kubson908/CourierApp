using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CourierAPI.Models;

public class Customer : IdentityUser
{
    [Required]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    public string LastName { get; set; } = string.Empty;
}
