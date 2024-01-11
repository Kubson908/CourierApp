using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CourierAPI.Models;

public class Dispatcher : IdentityUser
{
    [Required]
    public required string FirstName { get; set; }

    [Required]
    public required string LastName { get; set; }
}
