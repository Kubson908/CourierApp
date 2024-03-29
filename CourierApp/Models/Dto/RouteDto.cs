﻿namespace CourierAPI.Models.Dto;

public class RouteDto
{
    public required string CourierId { get; set; }
    public required List<Shipment> Shipments { get; set; }
    public required string Date { get; set; }
}
