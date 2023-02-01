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
}