using api.Services;
using AutoMapper;
using data;
using shared.Models;
using data.ORM;
using Microsoft.AspNetCore.Mvc;
using shared.UpdateModels;
using shared.View;
using Microsoft.EntityFrameworkCore;

namespace api.Logic;

public class OrganizationLogic
{
    private readonly MainContext _mainContext;
    private readonly IMapper _mapper;
    private readonly PermissionService _permissionService;

    public OrganizationLogic(MainContext mainContext, IMapper mapper, PermissionService permissionService)
    {
        _mainContext = mainContext;
        _mapper = mapper;
        _permissionService = permissionService;
    }
    
    public async Task<List<OrganizationView>> GetAll()
    {
        return (await _mainContext.Organizations.ToListAsync())
            .Select(x => _mapper.Map<OrganizationView>(x))
            .ToList();
    }

    public async Task<OrganizationView> Update(Guid id, UpdateOrganization body)
    {
        var permissions =  await _permissionService.GetPermissions(new PermissionQuery());

        if (permissions.IsMissing("global.org.manage"))
        {
            throw new UnauthorizedAccessException("Fam what are you doing");
        }
        
        var dto = await _mainContext.Organizations.FindAsync(id);

        _mapper.Map(body, dto);

        await _mainContext.SaveChangesAsync();

        return _mapper.Map<OrganizationView>(dto);
    }

    public async Task<OrganizationView> UpdateMetadata(Guid id, Dictionary<string, string> body)
    {
        var permissions =  await _permissionService.GetPermissions(new PermissionQuery()
        {
            Organization = id
        });

        if (permissions.IsMissing("org.manage"))
        {
            throw new UnauthorizedAccessException("Fam what are you doing");
        }
        
        var dto = await _mainContext.Organizations.FindAsync(id);

        dto!.Contents.Metadata = body;
        
        _mainContext.Mark(dto);

        await _mainContext.SaveChangesAsync();

        return _mapper.Map<OrganizationView>(dto);
    }

    public async Task<OrganizationView> UpdatePrivateMetadata(Guid id, Dictionary<string, string> body)
    {
        var permissions =  await _permissionService.GetPermissions(new PermissionQuery());
        
        permissions.Check("global.org.update.private_metadata");
        
        var dto = await _mainContext.Organizations.FindAsync(id);

        dto!.Contents.PrivateMetadata = body;
        
        _mainContext.Mark(dto);

        await _mainContext.SaveChangesAsync();

        return _mapper.Map<OrganizationView>(dto);
    }

    public async Task<OrganizationView> UpdatePolicy(Guid id, string body)
    {
        var permissions =  await _permissionService.GetPermissions(new PermissionQuery()
        {
            Organization = id
        });

        if (permissions.IsMissing("org.update.policy"))
        {
            throw new UnauthorizedAccessException("Fam what are you doing");
        }
        
        var dto = await _mainContext.Organizations.FindAsync(id);

        dto!.Contents.Policy = body;
        
        _mainContext.Mark(dto);

        await _mainContext.SaveChangesAsync();

        return _mapper.Map<OrganizationView>(dto);
    }

    public async Task<OrganizationView> Create(UpdateOrganization body)
    {
        var permissions =  await _permissionService.GetPermissions(new PermissionQuery());

        permissions.Check("global.org.manage");
        
        var dto = _mapper.Map<OrganizationDto>(body);
        
        dto.Id = Guid.NewGuid();
        dto.Contents = new Organization();

        await _mainContext.AddAsync(dto);
        await _mainContext.SaveChangesAsync();

        return _mapper.Map<OrganizationView>(dto);
    }

    public async Task<OrganizationView> Get(Guid id)
    {
        return _mapper.Map<OrganizationView>(await _mainContext.Organizations.FindAsync(id));
    }

    public async Task UpdateVariable(Guid id, UpdateOrganizationVariable variable)
    {
        var org = await _mainContext.Organizations.FindAsync(id);
        if (org == null)
        {
            throw new FileNotFoundException();
        }

        org.Contents.Variables[variable.Key] = variable.Value;
        _mainContext.Mark(org);

        await _mainContext.SaveChangesAsync();
    }
}