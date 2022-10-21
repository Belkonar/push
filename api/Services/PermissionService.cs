using api.Data;
using api.Models;
using data.Models;

namespace api.Services;

public class PermissionService
{
    private readonly OpaService _opaService;
    private readonly PermissionData _permissionData;

    public PermissionService(OpaService opaService, PermissionData permissionData)
    {
        _opaService = opaService;
        _permissionData = permissionData;
    }
    
    // Go through each policy in the list and add all the permissions to the collection
    public async Task<List<string>> GetPermissions(OpaInputDocument inputDoc, PermissionQuery permissionQuery)
    {
        var globalPolicy = await _permissionData.GetGlobalPolicy("global");

        var globalPolicyResponse = await _opaService.Query(globalPolicy, inputDoc);
        inputDoc.Permissions.AddRange(globalPolicyResponse.Keys());
        
        // TODO: Add org and resource level permissions stuff here

        return inputDoc.Permissions;
    }
}