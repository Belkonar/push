using api.Models;
using api.Services;
using AutoMapper;
using data;
using data.Models;
using data.ORM;
using data.UpdateModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Logic;

public class PolicyLogic
{
    private readonly MainContext _mainContext;
    private readonly IMapper _mapper;
    private readonly PermissionService _permissionService;

    public PolicyLogic(MainContext mainContext, IMapper mapper, PermissionService permissionService)
    {
        _mainContext = mainContext;
        _mapper = mapper;
        _permissionService = permissionService;
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
        var permissions =  await _permissionService.GetPermissions(
            new OpaInputDocument(),
            new PermissionQuery());

        if (!permissions.Contains("global_admin"))
        {
            throw new UnauthorizedAccessException("Fam what are you doing");
        }
        
        var dto = await _mainContext.Policies.FindAsync(key);

        _mapper.Map(policy, dto);

        await _mainContext.SaveChangesAsync();

        return dto!;
    }
}