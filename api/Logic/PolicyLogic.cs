using api.Services;
using AutoMapper;
using data;
using data.View;
using data.ORM;
using data.UpdateModels;
using Microsoft.EntityFrameworkCore;

namespace api.Logic;

public class PolicyLogic
{
    private readonly MainContext _mainContext;
    private readonly IMapper _mapper;
    private readonly PermissionService _permissionService;
    private readonly UserService _userService;

    public PolicyLogic(MainContext mainContext, IMapper mapper, PermissionService permissionService,
        UserService userService)
    {
        _mainContext = mainContext;
        _mapper = mapper;
        _permissionService = permissionService;
        _userService = userService;
    }
    
    public async Task<List<PolicyDto>> GetAll()
    {
        return await _mainContext.Policies.ToListAsync();
    }

    public async Task<string> GetByName(string name)
    {
        return (await _mainContext.Policies.FindAsync(name)).Policy;
    }

    public async Task<PolicyDto> Update(string key, UpdatePolicy policy)
    {
        var permissions =  await _permissionService.GetPermissions(new PermissionQuery());

        if (permissions.IsMissing("global_policy_manage"))
        {
            throw new UnauthorizedAccessException("Fam what are you doing");
        }
        
        var dto = await _mainContext.Policies.FindAsync(key);

        _mapper.Map(policy, dto);

        await _mainContext.SaveChangesAsync();

        return dto!;
    }
    
    public async Task<PolicyDto> Create(string key)
    {
        var permissions =  await _permissionService.GetPermissions(new PermissionQuery());

        if (permissions.IsMissing("global_policy_manage"))
        {
            throw new UnauthorizedAccessException("Fam what are you doing");
        }
        
        var dto = new PolicyDto()
        {
            Key = key,
            Policy = ""
        };

        await _mainContext.AddAsync(dto);
        
        await _mainContext.SaveChangesAsync();

        return dto;
    }
    
    // TODO: Add DELETE
}