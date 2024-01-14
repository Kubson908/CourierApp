using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using System.Web;
using CourierMobileApp.Models.Dto;

namespace CourierMobileApp.Services;

public class ConnectionService
{
    private readonly HttpClient _client;
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
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
        };
        _client = new HttpClient(handler)
        {
            BaseAddress = new Uri(Config.ApiPath),
            Timeout = TimeSpan.FromSeconds(10)
        };
        SetTokenAsync();
    }

    private static string SetQuery(object parameters)
    {
        var properties = parameters.GetType().GetProperties();
        var notNull = properties.Where(prop => prop.GetValue(parameters, null) != null);
        var values = notNull.Select(prop => $"{HttpUtility.UrlEncode(prop.Name)}={HttpUtility.UrlEncode(prop.GetValue(parameters).ToString())}");
        var queryParams = $"?{string.Join("&", values)}";
        return queryParams;
    }

    public async Task<HttpResponseMessage> SendAsync(HttpMethod method, string url, object body = null, object queryParams = null, string contentType = null)
    {
        var content = body != null ? new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, contentType ?? "application/json") : null;
        url = url.StartsWith("/") ? url : "/" + url;
        url = queryParams is not null ? url + SetQuery(queryParams) : url;
        return method.Method switch
        {
            "POST" => await _client.PostAsync(url, content),
            "GET" => await _client.GetAsync(url),
            "PUT" => await _client.PutAsync(url, content),
            "PATCH" => await _client.PatchAsync(url, content),
            "DELETE" => await _client.DeleteAsync(url),
            _ => throw new HttpRequestException($"Unsupported method: {method.Method}"),
        };
    }

    public async Task<ApiUserResponse> UploadPhotoAsync(FileResult file)
    {
        var httpContent = new MultipartFormDataContent
        {
            { new StreamContent(await file.OpenReadAsync()), "postedFile", file.FileName }
        };
        var response = await _client.PostAsync("/api/courier/upload-image", httpContent);
        if (!response.IsSuccessStatusCode)
            return new ApiUserResponse()
            {
                IsSuccess = false,
                Message = "Nie udało się przesłać zdjęcia"
            };
        ApiUserResponse responseData = JsonConvert.DeserializeObject<ApiUserResponse>(await response.Content.ReadAsStringAsync());
        return responseData;
    }
}
