using CourierMobileApp.Services;

namespace CourierMobileApp.ViewModels;

public partial class ScheduleViewModel : BaseViewModel
{
    readonly ShipmentService shipmentService;
    [ObservableProperty]
    public DateTime date;
    [ObservableProperty]
    public DateTime minimumDate;

    public ObservableCollection<RouteElement> Route { get; set; }

    public ScheduleViewModel(ShipmentService shipmentService)
    {
        Title = "Harmonogram";
        Route = new ObservableCollection<RouteElement>();
        this.shipmentService = shipmentService;
        foreach (RouteElement routeElement in this.shipmentService.route)
        {
            Route.Add(routeElement);
        }
        MinimumDate = DateTime.Today;
        Date = DateTime.Today;
    }

    [RelayCommand]
    public async Task GetRouteAsync()
    {
        Route.Clear();
        var temp = await shipmentService.GetRouteAsync(Date);
        foreach (var route in temp)
        {
            Route.Add(route);
        }
    }
}
