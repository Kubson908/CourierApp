using CourierMobileApp.Services;
using CourierMobileApp.View;

namespace CourierMobileApp.ViewModels;

public partial class ShipmentViewModel : BaseViewModel
{
    private readonly ShipmentService shipmentService;
    [ObservableProperty]
    RouteElement routeElement;
    private int index;
    [ObservableProperty]
    string header;
    readonly List<string> sizes = new()
    {
        "bardzo mała",
        "mała",
        "średnia",
        "duża",
    };
    readonly List<string> weights = new()
    {
        "lekka",
        "średnia",
        "ciężka",
    };
    [ObservableProperty]
    string address;
    [ObservableProperty]
    string city;
    [ObservableProperty]
    string customer;
    [ObservableProperty]
    string customerName;
    [ObservableProperty]
    string phoneNumber;
    [ObservableProperty]
    string size;
    [ObservableProperty]
    string weight;
    [ObservableProperty]
    string finishIconPath;
    [ObservableProperty]
    bool trueValue;
    [ObservableProperty]
    bool falseValue;
    [ObservableProperty]
    bool canIncrement;
    [ObservableProperty]
    bool canDecrement;
    [ObservableProperty]
    Status statusValue;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanNotFinish))]
    bool canFinish;

    public bool CanNotFinish => !CanFinish;

    private bool hasPhonePermission = false;

    public ShipmentViewModel(ShipmentService shipmentService, RouteElement element)
    {
        this.shipmentService = shipmentService;
        RouteElement = element;
        index = this.shipmentService.route.IndexOf(routeElement);
        TrueValue = true;
        FalseValue = false;
        SetProperties();
    }

    private void SetProperties()
    {
        CanIncrement = index < shipmentService.route.Count - 1;
        CanDecrement = index != 0;
        Header = RouteElement.Shipment.Status == Status.Accepted ? "Odbiór" : "Dostawa";
        Customer = RouteElement.Shipment.Status == Status.Accepted ? "Nadawca" : "Odbiorca";
        CustomerName =  RouteElement.Shipment.Status == Status.Accepted ?
            RouteElement.Shipment.Customer.FirstName + " " + RouteElement.Shipment.Customer.LastName :
            RouteElement.Shipment.RecipientName;
        StatusValue = RouteElement.Shipment.Status;
        PhoneNumber = RouteElement.Shipment.Status == Status.Accepted ? RouteElement.Shipment.Customer.PhoneNumber : RouteElement.Shipment.RecipientPhoneNumber;
        if (PhoneNumber.Length == 9)
        {
            PhoneNumber = PhoneNumber.Insert(3, " ").Insert(7, " ");
        }
        Address = StatusValue == Status.Accepted ? (RouteElement.Shipment.PickupAddress
            + (RouteElement.Shipment.PickupApartmentNumber.Length > 0 ? "/" + RouteElement.Shipment.PickupApartmentNumber : ""))
            : (RouteElement.Shipment.RecipientAddress + (RouteElement.Shipment.RecipientApartmentNumber.Length > 0 ? "/" + RouteElement.Shipment.RecipientApartmentNumber : ""));
        City = StatusValue == Status.Accepted ? RouteElement.Shipment.PickupCity : RouteElement.Shipment.RecipientCity;
        Size = sizes[(int)RouteElement.Shipment.Size];
        Weight = weights[(int)RouteElement.Shipment.Weight];
        FinishIconPath = RouteElement.Shipment.Status == Status.Accepted ? "pickup_button.svg" : "delivery_button.svg";
        CanFinish = MauiApplication.Current.Services.GetService<ScheduleViewModel>().IsWorking;
    }

    [RelayCommand]
    public void ChangeShipment(bool increment)
    {
        index = increment ? (index < shipmentService.route.Count - 1 ? index + 1 : index) : (index > 0 ? index - 1 : index);
        RouteElement = shipmentService.route[index];
        SetProperties();
    }

    private async Task RequestPhonePermission()
    {
        var status = await Permissions.CheckStatusAsync<Permissions.Phone>();

        if (status != PermissionStatus.Granted)
        {
            var results = await Permissions.RequestAsync<Permissions.Phone>();

            if (results != PermissionStatus.Granted)
                hasPhonePermission = false;
            else
                hasPhonePermission = true;
        }
        else
            hasPhonePermission = true;
    }

    public async Task DialNumber()
    {
        await RequestPhonePermission();
        if (!hasPhonePermission)
            return;

        if (PhoneDialer.Default.IsSupported)
            PhoneDialer.Default.Open(PhoneNumber);
    }

    [RelayCommand]
    public void FinishShipment()
    {
        MainThread.InvokeOnMainThreadAsync(async () =>
        {
            await Shell.Current.Navigation.PushAsync(new Scanner(new ScannerViewModel(RouteElement, true, shipmentService)));
        });
    }

    public async Task NavigateToAddress()
    {
        var address = City + " " + Address;
        var placemark = new Placemark
        {
            Thoroughfare = address,
        };
        try
        {
            await Map.Default.OpenAsync(placemark);
        } catch (Exception)
        {
            await Shell.Current.DisplayAlert("Brak aplikacji GPS", "Nie znaleziono dostępnej aplikacji GPS do uruchomienia", "OK");
        }
    }

    public async Task OpenSMS()
    {
        var text = Config.SMSMessage;
        var message = new SmsMessage(text ?? "", PhoneNumber);
        await Sms.Default.ComposeAsync(message);
    }
}
