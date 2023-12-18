using CourierMobileApp.Services;

namespace CourierMobileApp.View;

public partial class ShipmentPage : ContentPage
{
    ShipmentViewModel viewModel;
    ProfileService profileService;
    public ShipmentPage(ShipmentViewModel shipmentViewModel, ProfileService profileService)
    {
        InitializeComponent();
        BindingContext = shipmentViewModel;
        viewModel = shipmentViewModel;
        this.profileService = profileService;
        navbar.Initialize(this.profileService);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        navbar.SetImage();
    }

    private void FinishButtonClicked(object sender, EventArgs e)
    {
        /*MainThread.InvokeOnMainThreadAsync(async () =>
        {
            await Shell.Current.Navigation.PushAsync(new Scanner(new ScannerViewModel(viewModel.RouteElement, true, shipmentService)));
        });*/
    }
}