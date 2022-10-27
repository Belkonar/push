using System.Text.Json;

namespace api.Services;

/// <summary>
/// This class only exists to handle HTTP calls from a scoped context.
/// TODO: I hate it and will probably replace it with a more generic one later. 
/// </summary>
public class Auth0Service
{
    private readonly HttpClient _client;

    public Auth0Service(HttpClient client)
    {
        _client = client;
    }

    public async Task<JsonDocument> GetProfile(string authToken)
    {
        const string issuer = "https://dev-sl9gv5xa.us.auth0.com";
        const string profileEndpoint = $"{issuer}/userinfo";
        
        using var request = new HttpRequestMessage()
        {
            RequestUri = new Uri(profileEndpoint),
            Method = HttpMethod.Get
        };
            
        request.Headers.Add("Authorization", authToken);
        
        using var response = await _client.SendAsync(request);
        
        return (await response.Content.ReadFromJsonAsync<JsonDocument>())!;
    }
}