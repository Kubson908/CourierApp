﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourierAPI.Models;

public class Shipment
{
    [Key]
    public int Id { get; set; }
    [Required]
    public Status Status { get; set; } = Status.Registered;
    [Required]
    public required string PickupAddress { get; set; }
    public int? PickupApartmentNumber { get; set; }
    [Required]
    public required string PickupCity { get; set; }
    [Required]
    public required string PickupPostalCode { get; set; }
    [Required]
    public Size Size { get; set; }
    [Required]
    public Weight Weight { get; set; }
    [Required]
    public required string RecipientName { get; set; }
    [Required]
    public required string RecipientPhoneNumber { get; set; }
    [Required]
    public required string RecipientAddress { get; set; }
    public int? RecipientApartmentNumber { get; set; }
    [Required]
    public required string RecipientCity { get; set; }
    [Required]
    public required string RecipientPostalCode { get; set; }
    public string? RecipientEmail { get; set; }
    public DateTime? PickupDate { get; set; }
    public DateTime? StoreDate { get; set; }
    public DateTime? DeliveryDate { get; set; }
    public string? AdditionalDetails { get; set; }
    public float? Price { get; set; }
    public int DeliveryAttempts { get; set; } = 0;

    //Navigation properties
    [ForeignKey(nameof(Customer))]
    public string? CustomerId { get; set; }
    public Customer? Customer { get; set; }

    [ForeignKey(nameof(Order))]
    public int? OrderId { get; set; }
    public Order? Order { get; set; }
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
