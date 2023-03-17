// <auto-gene rated/>

using System.Net.Http.Json;
using shared.Models;
using shared.Interfaces;
using shared.UpdateModels;
using shared.Models.Job;

namespace shared.services;

public class ThingService : IThingController
{
    private readonly HttpClient _client;
    
    private const string Prefix = "/Thing";
    
    public ThingService(IHttpClientFactory factory)
    {
        _client = factory.CreateClient("api");
    }

    public async Task<List<Thing>> GetThings()
    {
        var route = $"{Prefix}/";

        var httpResponse = await _client.GetAsync(route);
        httpResponse.EnsureSuccessStatusCode();
        return await httpResponse.Content.ReadFromJsonAsync<List<Thing>>() ?? new List<Thing>();
    }

    public async Task<Thing> GetThing(Guid id)
    {
        var route = $"{Prefix}/{id}";

        var httpResponse = await _client.GetAsync(route);
        httpResponse.EnsureSuccessStatusCode();
        return (await httpResponse.Content.ReadFromJsonAsync<Thing>())!;
    }

    public async Task<Thing> CreateThing(UpdateThing thingView)
    {
        var route = $"{Prefix}/";


        var httpResponse = await _client.PostAsJsonAsync(route, thingView);

        httpResponse.EnsureSuccessStatusCode();
        return (await httpResponse.Content.ReadFromJsonAsync<Thing>())!;
    }

    public async Task<Thing> UpdateThing(Guid id, Thing thingView)
    {
        var route = $"{Prefix}/{id}";


        var httpResponse = await _client.PutAsJsonAsync(route, thingView);

        httpResponse.EnsureSuccessStatusCode();
        return (await httpResponse.Content.ReadFromJsonAsync<Thing>())!;
    }

    public async Task<Deployable> GetDeployable(Guid id)
    {
        var route = $"{Prefix}/{id}/deployable";

        var httpResponse = await _client.GetAsync(route);
        httpResponse.EnsureSuccessStatusCode();
        return (await httpResponse.Content.ReadFromJsonAsync<Deployable>())!;
    }

    public async Task<Deployable> UpdateDeployable(Guid id, Deployable deployableView)
    {
        var route = $"{Prefix}/{id}/deployable";


        var httpResponse = await _client.PutAsync(route, null);

        httpResponse.EnsureSuccessStatusCode();
        return (await httpResponse.Content.ReadFromJsonAsync<Deployable>())!;
    }

    public async Task<Job> StartDeployment(Guid id, string reference)
    {
        var route = $"{Prefix}/{id}/deployable/start/{reference}";


        var httpResponse = await _client.PostAsync(route, null);

        httpResponse.EnsureSuccessStatusCode();
        return (await httpResponse.Content.ReadFromJsonAsync<Job>())!;
    }

    public async Task UpdateInternalData(Guid id, string key, SimpleValue value)
    {
        var route = $"{Prefix}/{id}/data/{key}";


        var httpResponse = await _client.PutAsJsonAsync(route, value);

        httpResponse.EnsureSuccessStatusCode();
    }

}