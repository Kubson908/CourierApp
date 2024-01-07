using Android.Content.Res;
using CourierMobileApp.Interfaces;
using CourierMobileApp.Services;
using CourierMobileApp.View;

namespace CourierMobileApp.ViewModels;

public partial class ScheduleViewModel : BaseViewModel
{
    IBackgroundService foregroundServiceHandler;
    public readonly ShipmentService shipmentService;
    [ObservableProperty]
    DateTime date;
    [ObservableProperty]
    DateTime minimumDate;
    [ObservableProperty]
    bool listEmpty;

    [ObservableProperty]
    bool canStartRoute;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(RouteButtonString))]
    [NotifyPropertyChangedFor(nameof(ButtonColor))]
    bool isWorking;

    public string RouteButtonString => IsWorking ? "Zakończ trasę" : "Rozpocznij trasę";
    public Color ButtonColor => IsWorking ? Color.FromArgb("#848C8E") : Color.FromArgb("#15AB54");

    public ObservableCollection<RouteElement> Route { get; set; }
    private readonly ProfileService profileService;

    public ScheduleViewModel(ShipmentService shipmentService, ProfileService profileService, IBackgroundService backgroundService)
    {
        Title = "Harmonogram";
        Route = new ObservableCollection<RouteElement>();
        this.shipmentService = shipmentService;
        foregroundServiceHandler = backgroundService;
        foreach (RouteElement routeElement in this.shipmentService.route)
        {
            Route.Add(routeElement);
        }
        ListEmpty = Route.Count == 0;
        MinimumDate = DateTime.Today;
        Date = DateTime.Today;
        this.profileService = profileService;
        foregroundServiceHandler.ServiceStopped += (object sender, EventArgs e) => { IsWorking = false; };
        foregroundServiceHandler.ServiceStarted += (object sender, EventArgs e) => { IsWorking = true; };
    }

    public void SetRoute()
    {
        Route.Clear();
        foreach (var route in shipmentService.route)
        {
            Route.Add(route);
        }
        CanStartRoute = Date.CompareTo(DateTime.Today) == 0 && Route.Count > 0;
    }

    [RelayCommand]
    public void GetRouteAsync()
    {
        MainThread.InvokeOnMainThreadAsync(async () =>
        {
            ListEmpty = false;
            IsBusy = true;
            await shipmentService.GetRouteAsync(Date);
            SetRoute();
            IsBusy = false;
            ListEmpty = Route.Count == 0;
        });
    }
    [RelayCommand]
    public void GoToDetailsAsync(RouteElement element)
    {
        MainThread.InvokeOnMainThreadAsync(async () =>
        {
            await Shell.Current.Navigation.PushAsync(new ShipmentPage(new ShipmentViewModel(shipmentService, element), profileService));
        });
    }

    public void ToggleRoute()
    {
        if (IsWorking)
            foregroundServiceHandler.Stop();
        else
            IsWorking = StartRoute();
    }

    public bool StartRoute()
    {
        Vibration.Vibrate(TimeSpan.FromMilliseconds(200));
        try
        {
            foregroundServiceHandler.Start();
        } catch (Exception)
        {
            return false;
        }
        return true;
    }
}
