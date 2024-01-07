using CourierMobileApp.Services;
using CourierMobileApp.View;
using Maui.Plugins.PageResolver;

namespace CourierMobileApp.ViewModels;

public partial class MainPageViewModel : BaseViewModel
{
    private readonly ShipmentService shipmentService;
    public MainPageViewModel(ShipmentService shipmentService)
    {
        this.shipmentService = shipmentService;
    }

    [RelayCommand]
    private static void ScheduleClicked()
    {
        MainThread.InvokeOnMainThreadAsync(async () =>
        {
            await Shell.Current.Navigation.PushAsync<SchedulePage>();
        });
    }
    [RelayCommand]
    private static void WarehouseClicked()
    {
        MainThread.InvokeOnMainThreadAsync(async () =>
        {
            await Shell.Current.Navigation.PushAsync(new Scanner(new ScannerViewModel(null, true, MauiApplication.Current.Services.GetService<ShipmentService>()))) ;
        });
    }
}
