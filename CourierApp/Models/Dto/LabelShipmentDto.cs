namespace CourierAPI.Models.Dto;

public class LabelShipmentDto
{
    public int Id { get; set; }
    public Size Size { get; set; }
    public Weight Weight { get; set; }
    public string? CustomerEmail { get; set; }
    public string? CustomerPhone { get; set; }
    public string? RecipientEmail { get; set;}
    public string? RecipientPhone { get;set; }
}
