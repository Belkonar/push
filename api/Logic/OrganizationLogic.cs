using AutoMapper;
using data;
using data.ORM;
using data.UpdateModels;
using data.View;
using Microsoft.EntityFrameworkCore;

namespace api.Logic;

public class OrganizationLogic
{
    private readonly MainContext _mainContext;
    private readonly IMapper _mapper;

    public OrganizationLogic(MainContext mainContext, IMapper mapper)
    {
        _mainContext = mainContext;
        _mapper = mapper;
    }
    
    public async Task<List<OrganizationView>> GetAll()
    {
        return (await _mainContext.Organizations.ToListAsync())
            .Select(x => _mapper.Map<OrganizationView>(x))
            .ToList();
    }

    public async Task<OrganizationView> Update(Guid id, UpdateOrganization body)
    {
        var dto = await _mainContext.Organizations.FindAsync(id);

        _mapper.Map(body, dto);

        await _mainContext.SaveChangesAsync();

        return _mapper.Map<OrganizationView>(dto);
    }

    public async Task<OrganizationView> UpdateMetadata(Guid id, Dictionary<string, string> body)
    {
        var dto = await _mainContext.Organizations.FindAsync(id);

        dto!.Contents.Metadata = body;
        
        _mainContext.Mark(dto);

        await _mainContext.SaveChangesAsync();

        return _mapper.Map<OrganizationView>(dto);
    }

    public async Task<OrganizationView> UpdatePrivateMetadata(Guid id, Dictionary<string, string> body)
    {
        var dto = await _mainContext.Organizations.FindAsync(id);

        dto!.Contents.PrivateMetadata = body;
        
        _mainContext.Mark(dto);

        await _mainContext.SaveChangesAsync();

        return _mapper.Map<OrganizationView>(dto);
    }

    public async Task<OrganizationView> UpdatePolicy(Guid id, string body)
    {
        var dto = await _mainContext.Organizations.FindAsync(id);

        dto!.Contents.Policy = body;
        
        _mainContext.Mark(dto);

        await _mainContext.SaveChangesAsync();

        return _mapper.Map<OrganizationView>(dto);
    }

    public async Task<OrganizationView> Create(UpdateOrganization body)
    {
        var dto = _mapper.Map<OrganizationDto>(body);
        
        dto.Id = Guid.NewGuid();

        await _mainContext.AddAsync(dto);
        await _mainContext.SaveChangesAsync();

        return _mapper.Map<OrganizationView>(dto);
    }
}