using CourierMobileApp.Services;
using CourierMobileApp.View;
using Maui.Plugins.PageResolver;

namespace CourierMobileApp.ViewModels;

public partial class MainPageViewModel : BaseViewModel
{
    public readonly ShipmentService shipmentService;
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
}
