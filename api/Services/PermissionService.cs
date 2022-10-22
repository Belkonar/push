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
    public async Task<Permissions> GetPermissions(OpaInputDocument inputDoc, PermissionQuery permissionQuery)
    {
        var globalPolicy = await _permissionData.GetGlobalPolicy("global");

        var globalPolicyResponse = await _opaService.Query(globalPolicy, inputDoc);
        inputDoc.Permissions.AddRange(globalPolicyResponse.Keys());
        
        // TODO: Add org and resource level permissions stuff here)

        if (permissionQuery.Organization.HasValue)
        {
            var policy = await _permissionData.GetOrgPolicy(permissionQuery.Organization.Value);
            var policyResponse = await _opaService.Query(policy, inputDoc);

            // If there's a parent policy, ignore the results and use the parent
            if (policyResponse.ParentPolicy != null)
            {
                policy = await _permissionData.GetGlobalPolicy(policyResponse.ParentPolicy);
                policyResponse = await _opaService.Query(policy, inputDoc);
            }
            
            inputDoc.Permissions.AddRange(policyResponse.Keys());
        }

        if (permissionQuery.Resource.HasValue)
        {
            
        }

        return (Permissions)inputDoc.Permissions;
    }
}