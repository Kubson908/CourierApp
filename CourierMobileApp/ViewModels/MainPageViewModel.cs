using CourierMobileApp.Services;
using CourierMobileApp.View;
using Maui.Plugins.PageResolver;

namespace CourierMobileApp.ViewModels;

public partial  class MainPageViewModel : BaseViewModel
{
    readonly ShipmentService shipmentService;
    public MainPageViewModel(ShipmentService shipmentService)
    {
        this.shipmentService = shipmentService;
    }

    [RelayCommand]
    private void ScheduleClicked()
    {
        MainThread.InvokeOnMainThreadAsync(async () =>
        {
            IsBusy = true;
            await shipmentService.GetRouteAsync(DateTime.Today);
            await Shell.Current.Navigation.PushAsync<SchedulePage>();
            IsBusy = false;
        });
    }
}
