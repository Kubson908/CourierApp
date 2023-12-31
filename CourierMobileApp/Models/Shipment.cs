namespace CourierMobileApp.Models;

public class Shipment
{
    public int Id { get; set; }
    public Status Status { get; set; }
    public string PickupAddress { get; set; }
    public string PickupApartmentNumber { get; set; }
    public string PickupCity { get; set; }
    public string PickupPostalCode { get; set; }
    public Size Size { get; set; }
    public Weight Weight { get; set; }
    public string RecipientName { get; set; }
    public string RecipientPhoneNumber { get; set; }
    public string RecipientAddress { get; set; }
    public string RecipientApartmentNumber { get; set; }
    public string RecipientCity { get; set; }
    public string RecipientPostalCode { get; set; }
    public string RecipientEmail { get; set; }
    public DateTime? PickupDate { get; set; }
    public DateTime? StoreDate { get; set; }
    public DateTime? DeliveryDate { get; set; }
    public string AdditionalDetails { get; set; }
    public Customer Customer { get; set; }
}

public enum Status
{
    Registered,
    Accepted,
    PickedUp,
    Stored,
    InDelivery,
    Delivered,
    NotDelivered,
    StoredToReturn,
    InReturn,
    Returned
}

public enum Size
{
    VerySmall,
    Small,
    Medium,
    Large,
}

public enum Weight
{
    Light,
    Medium,
    Heavy
}
