using CourierAPI.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Net.WebSockets;
using System.Security.Claims;

namespace CourierAPI.Websocket;

public class WebsocketMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<WebsocketMiddleware> _logger;
#pragma warning disable IDE0052 // Remove unread private members
    private readonly WorkService _workService;
#pragma warning restore IDE0052 // Remove unread private members

    public List<WebsocketInfo> connections;
    readonly JwtSecurityTokenHandler handler;
    readonly List<string> endpoints = new()
    {
        "api/courier/check-in",
        "api/courier/end-route"
    };

    public WebsocketMiddleware(RequestDelegate next, ILogger<WebsocketMiddleware> logger, WorkService workService)
    {
        _next = next;
        _logger = logger;
        connections = new List<WebsocketInfo>();
        handler = new JwtSecurityTokenHandler();
        _workService = workService;
        workService.RecentlyActive += async (object? sender, WorkStatusEventArgs e) 
            => { await SendEventAsync(e.WorkTime.CourierId, "recentlyActive"); };
        workService.Inactive += async (object? sender, WorkStatusEventArgs e) 
            => { await SendEventAsync(e.WorkTime.CourierId, "routeEnded"); };
    }

    public async Task InvokeAsync(HttpContext context)
    {
        CancellationToken ct = context.RequestAborted;
        if (endpoints.Any(e => context.Request.Path.ToString().Contains(e)) && context.Response.Body != null)
        {
            var originalBody = context.Response.Body;
            try
            {
                var bodyMemoryStream = new MemoryStream();

                context.Response.Body = bodyMemoryStream;
                await _next.Invoke(context);
                bodyMemoryStream.Seek(0, SeekOrigin.Begin);
                string body = await new StreamReader(bodyMemoryStream).ReadToEndAsync();
                if (context.Response.StatusCode >= 300 || body == string.Empty || body == null) return;
                bodyMemoryStream.Seek(0, SeekOrigin.Begin);

                await bodyMemoryStream.CopyToAsync(originalBody);
                
                JObject json = JObject.Parse(body);
                switch (json["message"]?.ToString())
                {
                    case "Checked in":
                        await SendEventAsync(json["data"]?.ToString() ?? "", "courierActive", ct);
                        break;
                    
                    case "Route ended":
                        await SendEventAsync(json["data"]?.ToString() ?? "", "routeEnded", ct);
                        break;
                }

                return;
            }
            catch (WebSocketException) { }
            finally
            {
                context.Response.Body = originalBody;
            }
        }
        if (!context.WebSockets.IsWebSocketRequest)
        {
            await _next.Invoke(context);
            return;
        }

        string wsId = context.Connection.Id;

        WebSocket ws = await context.WebSockets.AcceptWebSocketAsync();
        connections.Add(new WebsocketInfo()
        {
            Id = wsId,
            Connection = ws,
        });
        _logger.Log(LogLevel.Information, wsId + ": WebSocket connection established" + "\nTotal connections: " + connections.Count);

        while (true)
        {
            if (ct.IsCancellationRequested)
            {
                return;
            }
            string data = string.Empty;
            try
            {
                data = await ReadStringAsync(ws, ct);
            } catch (OperationCanceledException)
            {
                break;
            }
            var con = connections.First(c => c.Id == wsId);
            if (string.IsNullOrEmpty(data))
            {
                if (ws.State != WebSocketState.Open)
                {
                    break;
                }

                continue;
            }
            if (!con.IsAuthenticated && handler.CanReadToken(data))
            {
                var token = handler.ReadJwtToken(data);
                con.IsAuthenticated = true;
                con.Roles = token.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
                con.UserId = token.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
                continue;
            }
            Console.WriteLine(wsId + ": " + data);
            foreach (var item in connections.Where(c => c.Roles != null && c.Roles.Contains("Staff")))
            {
                if (item.Connection != null && item.Connection.State != WebSocketState.Open)
                {
                    continue;
                }

                await SendStringAsync(item.Connection ?? ws, data, ct);
            }
        }
        connections.Remove(connections.First(c => c.Id == wsId));
        if (ws.State != WebSocketState.Aborted )
            try
            {
                await ws.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "UserDisconnected", ct);
            }  
            catch (TaskCanceledException) { } catch (OperationCanceledException) { } catch (WebSocketException) { }
        _logger.Log(LogLevel.Information, wsId + ": WebSocket connection closed"
                        + "\nTotal connections: " + connections.Count);
        ws.Dispose();
    }

    static async Task<string> ReadStringAsync(WebSocket ws, CancellationToken ct = default)
    {
        var buffer = new ArraySegment<byte>(new byte[1024 * 8]);

        using MemoryStream ms = new();
        WebSocketReceiveResult receiveResult;

        do
        {
            ct.ThrowIfCancellationRequested();

            receiveResult = await ws.ReceiveAsync(buffer, ct);

            if (buffer.Array != null) ms.Write(buffer.Array, buffer.Offset, receiveResult.Count);
        } while (!receiveResult.EndOfMessage);

        ms.Seek(0, SeekOrigin.Begin);

        if (receiveResult.MessageType != WebSocketMessageType.Text)
            return string.Empty;

        using StreamReader reader = new(ms, System.Text.Encoding.UTF8);
        return await reader.ReadToEndAsync();
    }

    static Task SendStringAsync(WebSocket ws, string data, CancellationToken ct = default)
    {
        var buffer = System.Text.Encoding.UTF8.GetBytes(data);
        var segment = new ArraySegment<byte>(buffer);
        return ws.SendAsync(segment, WebSocketMessageType.Text, true, ct);
    }

    public async Task SendEventAsync(string data, string eventName, CancellationToken ct = default)
    {
        var obj = new WebSocketEventDto
        {
            EventName = eventName,
            Id = data,
        };
        var buffer = System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj) ?? "");
        var segment = new ArraySegment<byte>(buffer);

        foreach (var item in connections)
        {
            if (item.Connection != null && item.Connection.State != WebSocketState.Open || item.Connection == null)
                continue;
            await item.Connection.SendAsync(segment, WebSocketMessageType.Text, true, ct);
        }
    }
}
