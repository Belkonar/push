using api.Services;
using shared.View;
using shared.UpdateModels;

namespace api.Logic;

// TODO: This later lol
public class PolicyLogic
{
    private readonly PermissionService _permissionService;
    private readonly UserService _userService;

    public PolicyLogic(PermissionService permissionService,
        UserService userService)
    {
        _permissionService = permissionService;
        _userService = userService;
    }
    
    public async Task<List<Policy>> GetAll()
    {
        // return await _mainContext.Policies.ToListAsync();
        throw new NotImplementedException();
    }

    public async Task<string> GetByName(string name)
    {
        // return (await _mainContext.Policies.FindAsync(name)).Policy;
        throw new NotImplementedException();
    }

    public async Task<Policy> Update(string key, UpdatePolicy policy)
    {
        // var permissions =  await _permissionService.GetPermissions(new PermissionQuery());
        //
        // if (permissions.IsMissing("global_policy_manage"))
        // {
        //     throw new UnauthorizedAccessException("Fam what are you doing");
        // }
        //
        // var dto = await _mainContext.Policies.FindAsync(key);
        //
        // _mapper.Map(policy, dto);
        //
        // await _mainContext.SaveChangesAsync();
        //
        // return dto!;
        throw new NotImplementedException();
    }
    
    public async Task<Policy> Create(string key)
    {
        // var permissions =  await _permissionService.GetPermissions(new PermissionQuery());
        //
        // if (permissions.IsMissing("global_policy_manage"))
        // {
        //     throw new UnauthorizedAccessException("Fam what are you doing");
        // }
        //
        // var dto = new PolicyDto()
        // {
        //     Key = key,
        //     Policy = ""
        // };
        //
        // await _mainContext.AddAsync(dto);
        //
        // await _mainContext.SaveChangesAsync();
        //
        // return dto;
        
        throw new NotImplementedException();
    }
    
    // TODO: Add DELETE
}