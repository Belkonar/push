// <auto-generated/>

using System.Net.Http.Json;
using shared.Models.Pipeline;
using shared.Interfaces;
using shared.Models;

namespace shared.services;

public class PipelineService : IPipelineController
{
    private readonly HttpClient _client;
    
    private const string Prefix = "/Pipeline";
    
    public PipelineService(IHttpClientFactory factory)
    {
        _client = factory.CreateClient("api");
    }

    public async Task<List<Pipeline>> GetPipelines(Nullable<Guid> org)
    {
        var route = $"{Prefix}/?org={org}";

        var httpResponse = await _client.GetAsync(route);
        httpResponse.EnsureSuccessStatusCode();
        return await httpResponse.Content.ReadFromJsonAsync<List<Pipeline>>() ?? new List<Pipeline>();
    }

    public async Task<Pipeline> GetPipeline(Guid id)
    {
        var route = $"{Prefix}/{id}";

        var httpResponse = await _client.GetAsync(route);
        httpResponse.EnsureSuccessStatusCode();
        return (await httpResponse.Content.ReadFromJsonAsync<Pipeline>())!;
    }

    public async Task<SimpleValue> GetLatestMajor(Guid id)
    {
        var route = $"{Prefix}/{id}/latest";

        var httpResponse = await _client.GetAsync(route);
        httpResponse.EnsureSuccessStatusCode();
        return (await httpResponse.Content.ReadFromJsonAsync<SimpleValue>())!;
    }

    public async Task<List<string>> GetVersions(Guid id)
    {
        var route = $"{Prefix}/{id}/versions";

        var httpResponse = await _client.GetAsync(route);
        httpResponse.EnsureSuccessStatusCode();
        return await httpResponse.Content.ReadFromJsonAsync<List<string>>() ?? new List<string>();
    }

    public async Task<PipelineVersion> GetVersionByConstraint(Guid id, string constraint)
    {
        var route = $"{Prefix}/{id}/version/{constraint}";

        var httpResponse = await _client.GetAsync(route);
        httpResponse.EnsureSuccessStatusCode();
        return (await httpResponse.Content.ReadFromJsonAsync<PipelineVersion>())!;
    }

    public async Task<Dictionary<string,string>> GetFiles(Guid id, string version)
    {
        var route = $"{Prefix}/{id}/files/{version}";

        var httpResponse = await _client.GetAsync(route);
        httpResponse.EnsureSuccessStatusCode();
        return (await httpResponse.Content.ReadFromJsonAsync<Dictionary<string,string>>())!;
    }

    public async Task<Pipeline> CreatePipeline(Pipeline data)
    {
        var route = $"{Prefix}/";


        var httpResponse = await _client.PostAsJsonAsync(route, data);

        httpResponse.EnsureSuccessStatusCode();
        return (await httpResponse.Content.ReadFromJsonAsync<Pipeline>())!;
    }

    public async Task<Pipeline> UpdatePipeline(Guid id, Pipeline data)
    {
        var route = $"{Prefix}/{id}";


        var httpResponse = await _client.PutAsJsonAsync(route, data);

        httpResponse.EnsureSuccessStatusCode();
        return (await httpResponse.Content.ReadFromJsonAsync<Pipeline>())!;
    }

    public async Task<PipelineVersion> UpdatePipelineVersion(Guid id, string key, PipelineVersion data)
    {
        var route = $"{Prefix}/{id}/version/{key}";


        var httpResponse = await _client.PostAsJsonAsync(route, data);

        httpResponse.EnsureSuccessStatusCode();
        return (await httpResponse.Content.ReadFromJsonAsync<PipelineVersion>())!;
    }

    public async Task ScheduledStep(Guid id, int ordinal)
    {
        var route = $"{Prefix}/{id}/step-scheduled/{ordinal}";


        var httpResponse = await _client.PostAsync(route, null);

        httpResponse.EnsureSuccessStatusCode();
    }

    public async Task FinishedStep(Guid id, int ordinal)
    {
        var route = $"{Prefix}/{id}/finished-step/{ordinal}";


        var httpResponse = await _client.PostAsync(route, null);

        httpResponse.EnsureSuccessStatusCode();
    }

}