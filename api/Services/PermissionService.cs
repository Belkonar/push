using api.Data;
using api.Models;
using data.View;

namespace api.Services;

public class PermissionService
{
    private readonly OpaService _opaService;
    private readonly PermissionData _permissionData;
    private readonly UserService _userService;

    public PermissionService(OpaService opaService, PermissionData permissionData, UserService userService)
    {
        _opaService = opaService;
        _permissionData = permissionData;
        _userService = userService;
    }
    
    // Go through each policy in the list and add all the permissions to the collection
    public async Task<Permissions> GetPermissions(PermissionQuery permissionQuery)
    {
        var inputDoc = new OpaInputDocument()
        {
            Permissions = new List<string>(),
            Profile = _userService.Profile
        };
        
        var globalPolicy = await _permissionData.GetGlobalPolicy("global");

        inputDoc.Permissions.AddRange(await RunPolicy(globalPolicy, inputDoc, false));

        if (permissionQuery.Organization.HasValue)
        {
            var policy = await _permissionData.GetOrgPolicy(permissionQuery.Organization.Value);
            
            inputDoc.Permissions.AddRange(await RunPolicy(policy, inputDoc));
        }

        if (permissionQuery.Resource.HasValue)
        {
            throw new NotImplementedException("resource level policies");
        }

        return new Permissions(inputDoc.Permissions);
    }

    // TODO: Think about just adding to the permissions list directly in here
    private async Task<List<string>> RunPolicy(string policy, OpaInputDocument inputDoc, bool followParent = true)
    {
        var response = await _opaService.Query(policy, inputDoc);
        
        if (followParent && response.ParentPolicy != null)
        {
            policy = await _permissionData.GetGlobalPolicy(response.ParentPolicy);
            response = await _opaService.Query(policy, inputDoc);
        }

        return response.Keys();
    }
}