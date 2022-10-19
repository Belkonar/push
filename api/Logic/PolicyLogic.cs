using AutoMapper;
using data;
using data.ORM;
using data.UpdateModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Logic;

public class PolicyLogic
{
    private readonly MainContext _mainContext;
    private readonly IMapper _mapper;

    public PolicyLogic(MainContext mainContext, IMapper mapper)
    {
        _mainContext = mainContext;
        _mapper = mapper;
    }
    
    public async Task<List<PolicyDto>> GetAll()
    {
        return await _mainContext.Policies.ToListAsync();
    }

    public async Task<PolicyDto> Update(string key, UpdatePolicy policy)
    {
        var dto = await _mainContext.Policies.FindAsync(key);

        _mapper.Map(policy, dto);

        await _mainContext.SaveChangesAsync();

        return dto!;
    }
}