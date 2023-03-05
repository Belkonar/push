using System.Net.Http.Json;
using shared.View;

namespace ui.Services;

// This exists to make stuff simpler on pages where you need to access the org list.
public class OrganizationService
{
    private readonly HttpClient _client;
    private List<OrganizationView>? _organizations = null;

    public OrganizationService(HttpClient client)
    {
        _client = client;
    }

    /// <summary>
    /// Clear the list (do this when a new org is added from the UI)
    /// </summary>
    public void Clear()
    {
        _organizations = null;
    }

    /// <summary>
    /// For mapping org ids to names
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public string GetName(Guid id)
    {
        return _organizations?.FirstOrDefault(x => x.Id == id)?.Name ?? "N/A";
    }
    
    public async Task<List<OrganizationView>> GetOrganizations()
    {
        if (_organizations != null)
        {
            return _organizations;
        }

        _organizations = await _client.GetFromJsonAsync<List<OrganizationView>>("/organization");

        return _organizations!;
    }
}