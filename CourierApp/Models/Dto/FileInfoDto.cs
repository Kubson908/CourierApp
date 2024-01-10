namespace CourierAPI.Models.Dto;

public class FileInfoDto
{
    public required byte[] Bytes { get; set; }
    public required string Type { get; set; }
    public string? Name { get; set; }
}
