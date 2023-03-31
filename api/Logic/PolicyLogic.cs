using MongoDB.Bson;
using MongoDB.Driver;
using shared.Models;

namespace api.Logic;

// TODO: Rewire this to use policy-engine
public class PolicyLogic
{
    private readonly HttpClient _httpClient;
    
    public PolicyLogic(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("policy");
    }
    
    public async Task<List<Policy>> GetAll()
    {
        throw new NotImplementedException();
    }

    public async Task<Policy> Update(Policy policy)
    {
        throw new NotImplementedException();
    }

    public async Task<Policy> GetOne(string key)
    {
        throw new NotImplementedException();
    }

    public async Task Sync(List<Policy> policies)
    {
        Console.WriteLine(_httpClient.BaseAddress);
        using var response = await _httpClient.PutAsJsonAsync("/sync", policies);

        response.EnsureSuccessStatusCode();
    }
}
