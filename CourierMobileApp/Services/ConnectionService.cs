using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Web;

namespace CourierMobileApp.Services;

public class ConnectionService
{
    private HttpClient _client;
    private string token;

    public string Token
    {
        get => token;
        set
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", value);
            token = value;
        }
    }

    private async void SetTokenAsync()
    {
        Token = await SecureStorage.Default.GetAsync("access_token") ?? string.Empty;
    }

    public ConnectionService()
    {
        var handler = new HttpClientHandler();
        handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
        _client = new HttpClient(handler);
        _client.BaseAddress = new Uri(Config.ApiPath);
        SetTokenAsync();
    }

    private string setQuery(object parameters)
    {
        var properties = parameters.GetType().GetProperties();
        var notNull = properties.Where(prop => prop.GetValue(parameters, null) != null);
        var values = notNull.Select(prop => $"{HttpUtility.UrlEncode(prop.Name)}={HttpUtility.UrlEncode(prop.GetValue(parameters).ToString())}");
        var queryParams = $"?{string.Join("&", values)}";
        return queryParams;
    }

    public async Task<HttpResponseMessage> SendAsync(HttpMethod method, string url, object body = null, object queryParams = null) // dodać query params
    {
        var content = body != null ? new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json") : null;
        url = url.StartsWith("/") ? url : "/" + url;
        url = queryParams is not null ? url + setQuery(queryParams) : url;
        switch (method.Method)
        {
            case "POST":
                return await _client.PostAsync(url, content);
            case "GET":
                return await _client.GetAsync(url);
            case "PUT":
                return await _client.PutAsync(url, content);
            case "PATCH":
                return await _client.PatchAsync(url, content);
            case "DELETE":
                return await _client.DeleteAsync(url);
            default : throw new HttpRequestException($"Unsupported method: {method.Method}");
        }
    }
}
