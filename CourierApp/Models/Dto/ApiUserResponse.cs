namespace CourierAPI.Models.Dto;

public class ApiUserResponse
{
    public string Message { get; set; } = string.Empty;
    public bool IsSuccess { get; set; } = false;
    public IEnumerable<string> Errors { get; set; } = Enumerable.Empty<string>();
    public bool Exception { get; set; } = false;
    public string? AccessToken { get; set; } = string.Empty;
    public DateTime? ExpireDate { get; set; }
    public string? User { get; set; } = string.Empty;
    public IList<string>? Roles { get; set; }
}
