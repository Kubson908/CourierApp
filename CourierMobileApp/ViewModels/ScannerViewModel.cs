using CourierMobileApp.Services;

namespace CourierMobileApp.ViewModels;

public partial class ScannerViewModel : BaseViewModel
{
    [ObservableProperty]
    int? label;
    private RouteElement routeElement;
    private bool finish;
    private readonly ShipmentService shipmentService;

    public ScannerViewModel(RouteElement routeElement, bool finish, ShipmentService shipmentService)
    {
        Label = null;
        this.routeElement = routeElement;
        this.finish = finish;
        this.shipmentService = shipmentService;
    }

    private bool VerifyShipmentId(int shipmentId)
    {
        return shipmentId == routeElement.Shipment.Id;
    }

    public async Task AfterScanAction(string label)
    {
        int id = LabelResolver.GetShipmentId(label);
        Label = id;
        if (!VerifyShipmentId(id))
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                Vibration.Default.Vibrate(TimeSpan.FromSeconds(0.5));
                await Shell.Current.DisplayAlert("Błąd weryfikacji", "Błędny numer przesyłki", "OK");
                await Shell.Current.Navigation.PopAsync();
            });
            
            return;
        }

        try
        {
            if (finish)
            {
                bool success = await shipmentService.UpdateShipmentStatus(routeElement);
                if (success)
                {
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                        Vibration.Default.Vibrate(TimeSpan.FromSeconds(0.5));
                        // TODO: Dodać wyświetlanie snackbara z informacją o zakończeniu
                        await Shell.Current.Navigation.PopAsync();
                    });
                }
            }
        } catch (Exception)
        {
            await MainThread.InvokeOnMainThreadAsync(async () => 
            {
               await Shell.Current.DisplayAlert("Błąd połączenia", "Wystąpił błąd podczas łączenia z serwerem", "OK");
            });
    }
    }
}
