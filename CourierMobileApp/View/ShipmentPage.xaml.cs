using CourierMobileApp.View.Components;

namespace CourierMobileApp.View;

public partial class ShipmentPage : ContentPage
{
    readonly ShipmentViewModel viewModel;
    private readonly MenuAnimation animation;
    public ShipmentPage(ShipmentViewModel shipmentViewModel)
    {
        InitializeComponent();
        BindingContext = shipmentViewModel;
        viewModel = shipmentViewModel;
        animation = new()
        {
            layout = MainContent
        };
        navbar.MenuClicked += (object sender, EventArgs e) => { animation.OpenMenu(sender, e); navbar.RotateIcon(); };
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

    private async void Navigate(object sender, EventArgs e)
    {
        await viewModel.NavigateToAddress();
    }

    private async void Message(object sender, EventArgs e)
    {
        await viewModel.OpenSMS();
    }
}