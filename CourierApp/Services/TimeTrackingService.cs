namespace CourierAPI.Services;

public class TimeTrackingService : BackgroundService
{
    private WorkService workService;

    public TimeTrackingService(WorkService workService)
    {
        this.workService = workService;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(7 * 60 * 1000, stoppingToken);
            workService.CheckTime();
        }
    }

}
