using data;

namespace api.Data;

/// <summary>
/// Helpers for accessing data required by permission service
/// </summary>
public class PermissionData
{
    private readonly MainContext _mainContext;

    public PermissionData(MainContext mainContext)
    {
        _mainContext = mainContext;
    }

    public async Task<string> GetGlobalPolicy(string key)
    {
        return (await _mainContext.Policies.FindAsync(key)).Policy;
    }
}