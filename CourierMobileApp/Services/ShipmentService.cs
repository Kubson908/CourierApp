using CourierMobileApp.Models.Dto;
using Newtonsoft.Json;

namespace CourierMobileApp.Services;

public class ShipmentService
{
    private ConnectionService connectionService;
    public List<RouteElement> route = new();

    public ShipmentService(ConnectionService connectionService)
    {
        this.connectionService = connectionService;
    }

    public async Task GetRouteAsync(DateTime date)
    {
        ApiParameters parameters = new()
        {
            Date = DateOnly.FromDateTime(date)
        };
        string url = $"api/shipment/get-courier-route";
        var routeData = await connectionService.SendAsync(HttpMethod.Get, url, queryParams: parameters);
        route = JsonConvert.DeserializeObject<List<RouteElement>>(await routeData.Content.ReadAsStringAsync());
        return;
    }

    public async Task<bool> UpdateShipmentStatus(RouteElement routeElement)
    {
        string url;
        switch (routeElement.Shipment.Status)
        {
            case Status.Accepted:
                url = @"api/shipment/pickup-package/" + routeElement.Shipment.Id;
                break;
            case Status.InDelivery:
                url = @"api/shipment/deliver-package/" + routeElement.Shipment.Id;
                break;
            case Status.PickedUp or Status.NotDelivered:
                url = @"api/shipment/store-package/" + routeElement.Shipment.Id;
                break;
            case Status.InReturn:
                url = @"api/shipment/return-package/" + routeElement.Shipment.Id;
                break;
            default:
                url = "";
                break;
        }

        try
        {
            var res = await connectionService.SendAsync(HttpMethod.Patch, url);
            ApiUserResponse response = JsonConvert.DeserializeObject<ApiUserResponse>(await res.Content.ReadAsStringAsync());
            if (response.IsSuccess)
            {
                route.Remove(routeElement);
                return true;
            }
            else return false;
        } catch (Exception)
        {
            throw new Exception("Błąd połączenia");
        }
    }
}
