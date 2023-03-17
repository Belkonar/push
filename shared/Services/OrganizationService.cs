// <auto-generated/>

using System.Net.Http.Json;
using shared.Models;
using shared.Interfaces;
using shared.UpdateModels;
using shared.View;

namespace shared.services;

public class OrganizationService : IOrganizationController
{
    private readonly HttpClient _client;
    
    private const string Prefix = "/Organization";
    
    public OrganizationService(IHttpClientFactory factory)
    {
        _client = factory.CreateClient("api");
    }

    public async Task<List<Organization>> GetOrgs()
    {
        var route = $"{Prefix}/";

        var httpResponse = await _client.GetAsync(route);
        httpResponse.EnsureSuccessStatusCode();
        return await httpResponse.Content.ReadFromJsonAsync<List<Organization>>() ?? new List<Organization>();
    }

    public async Task<Organization> GetOrg(Guid id)
    {
        var route = $"{Prefix}/{id}";

        var httpResponse = await _client.GetAsync(route);
        httpResponse.EnsureSuccessStatusCode();
        return (await httpResponse.Content.ReadFromJsonAsync<Organization>())!;
    }

    public async Task<Organization> Create(UpdateOrganization body)
    {
        var route = $"{Prefix}/";


        var httpResponse = await _client.PostAsJsonAsync(route, body);

        httpResponse.EnsureSuccessStatusCode();
        return (await httpResponse.Content.ReadFromJsonAsync<Organization>())!;
    }

    public async Task<Organization> Update(Guid id, UpdateOrganization body)
    {
        var route = $"{Prefix}/{id}";


        var httpResponse = await _client.PutAsJsonAsync(route, body);

        httpResponse.EnsureSuccessStatusCode();
        return (await httpResponse.Content.ReadFromJsonAsync<Organization>())!;
    }

    public async Task<Organization> UpdateMetadata(Guid id, Dictionary<string,string> body)
    {
        var route = $"{Prefix}/{id}/metadata";


        var httpResponse = await _client.PutAsJsonAsync(route, body);

        httpResponse.EnsureSuccessStatusCode();
        return (await httpResponse.Content.ReadFromJsonAsync<Organization>())!;
    }

    public async Task<Organization> UpdatePrivateMetadata(Guid id, Dictionary<string,string> body)
    {
        var route = $"{Prefix}/{id}/private_metadata";


        var httpResponse = await _client.PutAsJsonAsync(route, body);

        httpResponse.EnsureSuccessStatusCode();
        return (await httpResponse.Content.ReadFromJsonAsync<Organization>())!;
    }

    public async Task<Organization> UpdatePolicy(Guid id, string body)
    {
        var route = $"{Prefix}/{id}/policy";


        var httpResponse = await _client.PutAsJsonAsync(route, body);

        httpResponse.EnsureSuccessStatusCode();
        return (await httpResponse.Content.ReadFromJsonAsync<Organization>())!;
    }

    public async Task<string> UpdateVariable(Guid id, UpdateOrganizationVariable variable)
    {
        var route = $"{Prefix}/{id}/variable";


        var httpResponse = await _client.PutAsJsonAsync(route, variable);

        httpResponse.EnsureSuccessStatusCode();
        return (await httpResponse.Content.ReadFromJsonAsync<string>())!;
    }

    public async Task<List<Credential>> GetCredentials(Guid id)
    {
        var route = $"{Prefix}/{id}/credential";

        var httpResponse = await _client.GetAsync(route);
        httpResponse.EnsureSuccessStatusCode();
        return await httpResponse.Content.ReadFromJsonAsync<List<Credential>>() ?? new List<Credential>();
    }

    public async Task<Credential> CreateCredential(Guid id, Credential credential)
    {
        var route = $"{Prefix}/{id}/credential";


        var httpResponse = await _client.PostAsJsonAsync(route, credential);

        httpResponse.EnsureSuccessStatusCode();
        return (await httpResponse.Content.ReadFromJsonAsync<Credential>())!;
    }

    public async Task<Credential> GetCredential(Guid id)
    {
        var route = $"{Prefix}/credential/{id}";

        var httpResponse = await _client.GetAsync(route);
        httpResponse.EnsureSuccessStatusCode();
        return (await httpResponse.Content.ReadFromJsonAsync<Credential>())!;
    }

    public async Task<CredentialBundle> GetCredentialBundle(Guid id)
    {
        var route = $"{Prefix}/credential/{id}/bundle";

        var httpResponse = await _client.GetAsync(route);
        httpResponse.EnsureSuccessStatusCode();
        return (await httpResponse.Content.ReadFromJsonAsync<CredentialBundle>())!;
    }

    public async Task UpdateCredentialData(Guid id, Dictionary<string,string> data)
    {
        var route = $"{Prefix}/credential/{id}";


        var httpResponse = await _client.PutAsJsonAsync(route, data);

        httpResponse.EnsureSuccessStatusCode();
    }

}