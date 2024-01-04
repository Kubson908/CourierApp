namespace CourierAPI.Models.Dto;

public class OrderDetailsDto
{
    public DateTime OrderDate { get; set; }
    public List<Shipment>? Shipments { get; set; }
}
