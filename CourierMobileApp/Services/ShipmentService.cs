using CourierMobileApp.Models.Dto;
using Java.Net;
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
            Date = DateOnly.FromDateTime(date).ToString("dd.MM.yyyy")
        };
        string url = $"api/shipment/get-courier-route";
        var routeData = await connectionService.SendAsync(HttpMethod.Get, url, queryParams: parameters);
        route = JsonConvert.DeserializeObject<List<RouteElement>>(await routeData.Content.ReadAsStringAsync());
        return;
    }

    public async Task<bool> UpdateShipmentStatus(RouteElement routeElement)
    {
        string url = routeElement.Shipment.Status switch
        {
            Status.Accepted => @"api/shipment/pickup-package/" + routeElement.Shipment.Id,
            Status.InDelivery => @"api/shipment/deliver-package/" + routeElement.Shipment.Id,
            Status.InReturn => @"api/shipment/return-package/" + routeElement.Shipment.Id,
            _ => "",
        };
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

    public async Task<bool> RecipientAbsent(RouteElement routeElement)
    {
        try
        {
            var res = await connectionService.SendAsync(HttpMethod.Patch, @"api/shipment/package-not-delivered/" + routeElement.Shipment.Id);
            ApiUserResponse response = JsonConvert.DeserializeObject<ApiUserResponse>(await res.Content.ReadAsStringAsync());
            if (response.IsSuccess)
            {
                route.Remove(routeElement);
                return true;
            }
            else return false;
        }
        catch (Exception)
        {
            throw new Exception("Błąd połączenia");
        }
    }

    public async Task<bool> StorePackage(int id)
    {
        try
        {
            var res = await connectionService.SendAsync(HttpMethod.Patch, @"api/shipment/store-package/" + id);
            ApiUserResponse response = JsonConvert.DeserializeObject<ApiUserResponse>(await res.Content.ReadAsStringAsync());
            return response.IsSuccess;
        }
        catch (Exception)
        {
            throw new Exception("Błąd połączenia");
        }
    }
}
