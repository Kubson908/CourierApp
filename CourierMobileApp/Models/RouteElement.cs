namespace CourierMobileApp.Models;

public class RouteElement
{
    public int Id { get; set; }
    public string RouteDate { get; set; }
    public int Order { get; set; }
    public Shipment Shipment { get; set; }
}
