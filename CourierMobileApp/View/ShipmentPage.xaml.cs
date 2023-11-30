using CourierMobileApp.Services;

namespace CourierMobileApp.View;

public partial class ShipmentPage : ContentPage
{
    public ShipmentPage(ShipmentViewModel shipmentViewModel)
    {
        InitializeComponent();
        BindingContext = shipmentViewModel;
    }

    private void FinishButtonClicked(object sender, EventArgs e)
    {
        MainThread.InvokeOnMainThreadAsync(async () =>
        {
            await Shell.Current.Navigation.PushAsync(new Scanner(new ScannerViewModel()));
        });
    }
}