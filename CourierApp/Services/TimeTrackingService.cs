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
            await Task.Delay(15 * 60 * 1000, stoppingToken); // czekanie 15 minut | TODO: USTAWIĆ 15 MINUT ZAMIAST 30 SEKUND
            workService.CheckTime();
        }
    }

}
