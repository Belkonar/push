namespace api.Data;

/// <summary>
/// Helpers for accessing data required by permission service
/// </summary>
public class PermissionData
{
    public async Task<string> GetGlobalPolicy(string key)
    {
        // return (await _mainContext.Policies.FindAsync(key)).Policy;
        throw new NotImplementedException();
    }

    public async Task<string> GetOrgPolicy(Guid id)
    {
        //return (await _mainContext.Organizations.FindAsync(id)).Contents.Policy;
        throw new NotImplementedException();
    }
}