using System.Net.Http.Json;

namespace ui.Services;

public class WebClient
{
    private readonly HttpClient _client;

    public WebClient(HttpClient client)
    {
        _client = client;
    }

    public async Task<T> Get<T>(string uri)
    {
        return await _client.GetFromJsonAsync<T>(uri) ?? throw new Exception("404");
    }

    public async Task<TRet> Post<TPost, TRet>(string uri, TPost body)
    {
        var response = await _client.PostAsJsonAsync(uri, body);
        return (await response.Content.ReadFromJsonAsync<TRet>())!;
    }
    
    public async Task<TRet> Put<TPost, TRet>(string uri, TPost body)
    {
        var response = await _client.PutAsJsonAsync(uri, body);
        return (await response.Content.ReadFromJsonAsync<TRet>())!;
    }
}