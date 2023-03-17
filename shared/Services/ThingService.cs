using System.Net.Http.Json;
using Amazon.Runtime;
using shared.Interfaces;
using shared.Models;
using shared.Models.Job;
using shared.UpdateModels;

namespace shared.Services;

public class ThingService : IThingController
{
    private readonly HttpClient _client;

    private const string Prefix = "/thing"; 

    public ThingService(IHttpClientFactory factory)
    {
        _client = factory.CreateClient("api");
    }
    
    public async Task<List<Thing>> GetThings()
    {
        var route = $"{Prefix}/";

        using var response = await _client.GetAsync(route);

        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync<List<Thing>>())!;
        
        return await _client.GetFromJsonAsync<List<Thing>>(Prefix)
               ?? new List<Thing>();
    }

    public Task<Thing> GetThing(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Thing> CreateThing(UpdateThing thingView)
    {
        throw new NotImplementedException();
    }

    public Task<Thing> UpdateThing(Guid id, Thing thingView)
    {
        throw new NotImplementedException();
    }

    public Task<Deployable> GetDeployable(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Deployable> UpdateDeployable(Guid id, Deployable deployableView)
    {
        throw new NotImplementedException();
    }

    public Task<Job> StartDeployment(Guid id, string reference)
    {
        throw new NotImplementedException();
    }

    public Task UpdateInternalData(Guid id, string key, SimpleValue value)
    {
        throw new NotImplementedException();
    }
}