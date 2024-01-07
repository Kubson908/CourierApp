using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourierAPI.Models;

public class RouteElement
{
    [Key]
    public int Id { get; set; }
    [Required]
    public required string RouteDate { get; set; }
    [Required]
    public int Order { get; set; }
    [Required]
    public Type Type { get; set; }

    //Navigation properties
    [Required]
    [ForeignKey(nameof(Courier))]
    public required string CourierId { get; set; }
    public Courier? Courier { get; set; }
    [ForeignKey(nameof(Shipment))]
    public int? ShipmentId { get; set; }
    public Shipment? Shipment { get; set; }
}

public enum Type
{
    Pickup, Delivery, Return
}
