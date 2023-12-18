namespace CourierAPI.Models.Dto;

public class WorkTime
{
    public required string CourierId { get; set; }
    public WorkStatus Status { get; set; }
    public DateTime RequestTime { get; set; }
}

public enum WorkStatus
{
    Active,
    RecentlyActive,
    Inactive
}
