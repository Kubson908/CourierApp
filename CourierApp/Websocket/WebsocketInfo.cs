using System.Net.WebSockets;

namespace CourierAPI.Websocket;

public class WebsocketInfo
{
    public string? Id { get; set; }
    public WebSocket? Connection { get; set; }
    public string? UserId { get; set; }
    public bool IsAuthenticated { get; set; } = false;
    public List<string>? Roles { get; set; }
}
