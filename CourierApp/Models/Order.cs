using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourierAPI.Models;

public class Order
{
    [Key]
    public int Id { get; set; }
    [Required]
    public DateTime OrderDate { get; set; }

    //Navigation properties
    [ForeignKey(nameof(Customer))]
    public string? CustomerId { get; set; }
    public Customer? Customer { get; set; }
}
