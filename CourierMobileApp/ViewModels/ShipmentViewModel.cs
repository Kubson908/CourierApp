using CourierMobileApp.Services;

namespace CourierMobileApp.ViewModels;

public partial class ShipmentViewModel : BaseViewModel
{
    private readonly ShipmentService shipmentService;
    [ObservableProperty]
    RouteElement routeElement;
    private int index;
    [ObservableProperty]
    string header;
    List<string> sizes = new()
    {
        "bardzo mała",
        "mała",
        "średnia",
        "duża",
    };
    [ObservableProperty]
    string address;
    [ObservableProperty]
    string city;
    [ObservableProperty]
    string size;

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
        StatusValue = RouteElement.Shipment.Status;
        if (RouteElement.Shipment.RecipientPhoneNumber.Length == 9)
        {
            RouteElement.Shipment.RecipientPhoneNumber = RouteElement.Shipment.RecipientPhoneNumber.Insert(3, " ").Insert(7, " ");
        }
        Address = StatusValue == Status.Accepted ? RouteElement.Shipment.PickupAddress : RouteElement.Shipment.RecipientAddress;
        City = StatusValue == Status.Accepted ? RouteElement.Shipment.PickupCity : RouteElement.Shipment.RecipientCity;
        /*RecipientName = routeElement.Shipment.RecipientName;
        RecipientPhoneNumber = routeElement.Shipment.RecipientPhoneNumber;
        Weight = routeElement.Shipment.Weight;*/
        Size = sizes[(int)RouteElement.Shipment.Size];
        FinishIconPath = RouteElement.Shipment.Status == Status.Accepted ? "pickup_button.svg" : "delivery_button.svg";
    }

    [RelayCommand]
    public void ChangeShipment(bool increment)
    {
        index = increment ? (index < shipmentService.route.Count - 1 ? index + 1 : index) : (index > 0 ? index - 1 : index);
        RouteElement = shipmentService.route[index];
        SetProperties();
    }
}
