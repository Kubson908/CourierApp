using CourierMobileApp.Services;
using CourierMobileApp.View.Components;

namespace CourierMobileApp.View;

public partial class ShipmentPage : ContentPage
{
    readonly ShipmentViewModel viewModel;
    ProfileService profileService;
    private readonly MenuAnimation animation;
    public ShipmentPage(ShipmentViewModel shipmentViewModel, ProfileService profileService)
    {
        InitializeComponent();
        BindingContext = shipmentViewModel;
        viewModel = shipmentViewModel;
        this.profileService = profileService;
        animation = new()
        {
            layout = MainContent
        };
        navbar.MenuClicked += (object sender, EventArgs e) => { animation.OpenMenu(sender, e); navbar.RotateIcon(); };
        navbar.Initialize(this.profileService);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        navbar.SetImage();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        animation.CloseMenu(null, EventArgs.Empty);
        if (navbar.menuOpened) navbar.RotateIcon();
    }

    private void FinishButtonClicked(object sender, EventArgs e)
    {
        /*MainThread.InvokeOnMainThreadAsync(async () =>
        {
            await Shell.Current.Navigation.PushAsync(new Scanner(new ScannerViewModel(viewModel.RouteElement, true, shipmentService)));
        });*/
    }

    private async void PhoneDialer(object sender, EventArgs e)
    {
        await viewModel.DialNumber();
    }
}