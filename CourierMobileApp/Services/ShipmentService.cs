using CourierMobileApp.Models.Dto;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CourierMobileApp.Services;

public class ShipmentService
{
    private ConnectionService connectionService;
    public List<RouteElement> route = new List<RouteElement>();

    public ShipmentService(ConnectionService connectionService)
    {
        this.connectionService = connectionService;
    }

    public async Task<List<RouteElement>> GetRouteAsync(DateTime date)
    {
        ApiParameters parameters = new()
        {
            Date = DateOnly.FromDateTime(date)
        };
        string url = $"api/shipment/get-courier-route";
        var routeData = await connectionService.SendAsync(HttpMethod.Get, url, queryParams: parameters);
        route = JsonConvert.DeserializeObject<List<RouteElement>>(await routeData.Content.ReadAsStringAsync());
        return route;
    }
}
