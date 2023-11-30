using CourierMobileApp.Services;
using CourierMobileApp.View;

namespace CourierMobileApp.ViewModels;

public partial class ScheduleViewModel : BaseViewModel
{
    readonly ShipmentService shipmentService;
    [ObservableProperty]
    DateTime date;
    [ObservableProperty]
    DateTime minimumDate;
    [ObservableProperty]
    bool listEmpty;
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
        ListEmpty = Route.Count == 0;
        MinimumDate = DateTime.Today;
        Date = DateTime.Today;
    }

    [RelayCommand]
    public async Task GetRouteAsync()
    {
        ListEmpty = false;
        IsBusy = true;
        Route.Clear();
        var temp = await shipmentService.GetRouteAsync(Date);
        foreach (var route in temp)
        {
            Route.Add(route);
        }
        IsBusy = false;
        ListEmpty = Route.Count == 0;
    }
    [RelayCommand]
    public void GoToDetailsAsync(RouteElement element)
    {
        MainThread.InvokeOnMainThreadAsync(async () =>
        {
            await Shell.Current.Navigation.PushAsync(new ShipmentPage(new ShipmentViewModel(shipmentService, element)));
        });
    }
}
