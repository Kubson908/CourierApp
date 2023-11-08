namespace CourierAPI.Models.Dto;

public class RegisterShipmentsDto
{
    public required IEnumerable<Shipment> Shipments { get; set; }
}
