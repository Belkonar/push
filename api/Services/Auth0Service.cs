using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace api.Services;

/// <summary>
/// This class only exists to handle HTTP calls from a scoped context.
/// TODO: I hate it and will probably replace it with a more generic one later. 
/// </summary>
public class Auth0Service
{
    private readonly HttpClient _client;
    private readonly IMemoryCache _cache;

    public Auth0Service(HttpClient client, IMemoryCache cache)
    {
        _client = client;
        _cache = cache;
    }

    public async Task<JsonDocument?> GetProfile(string subject, string authToken)
    {
        return await _cache.GetOrCreateAsync($"profile.{subject}", async entry =>
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
        });
    }
}