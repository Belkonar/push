using AutoMapper;
using data;
using data.ORM;
using Microsoft.EntityFrameworkCore;
using shared.Models;
using shared.UpdateModels;
using shared.View;

namespace api.Logic;

public class ThingLogic
{
    private readonly IMapper _mapper;
    private readonly MainContext _mainContext;

    public ThingLogic(IMapper mapper, MainContext mainContext)
    {
        _mapper = mapper;
        _mainContext = mainContext;
    }
    
    public async Task<List<ThingView>> GetThings()
    {
        return (await _mainContext.Things.ToListAsync())
            .Select(x => _mapper.Map<ThingDto, ThingView>(x))
            .ToList();
    }
    
    public async Task<ThingView> GetThing(Guid id)
    {
        var thing = await _mainContext.Things.FindAsync(id);
        
        if (thing == null)
        {
            throw new FileNotFoundException();
        }
        
        return _mapper.Map<ThingDto, ThingView>(thing);
    }

    public async Task<ThingView> CreateThing(UpdateThing thingView)
    {
        var request = _mapper.Map<UpdateThing, ThingDto>(thingView);
        request.Id = Guid.NewGuid();
        request.Contents = new Thing();

        await _mainContext.AddAsync(request);
        await _mainContext.SaveChangesAsync();

        return _mapper.Map<ThingDto, ThingView>(request);
    }

    public async Task<ThingView> UpdateThing(Guid id, ThingView thingView)
    {
        var thing = await _mainContext.Things.FindAsync(id);
        
        if (thing == null)
        {
            throw new FileNotFoundException();
        }

        _mapper.Map(thingView, thing);
        await _mainContext.SaveChangesAsync();
        
        return _mapper.Map<ThingDto, ThingView>(thing);
    }

    /// <summary>
    /// Does an upsert for the deployable resource
    /// </summary>
    /// <param name="id">id of the thing (not the deployable)</param>
    /// <param name="deployableView">deployable model</param>
    /// <returns></returns>
    public async Task<DeployableView> UpdateDeployable(Guid id, DeployableView deployableView)
    {
        var deployable = await _mainContext.Deployables.FirstOrDefaultAsync(x => x.ThingId == id);

        if (deployable == null)
        {
            deployable = _mapper.Map<DeployableView, DeployableDto>(deployableView);
            deployable.Id = Guid.NewGuid();
            deployable.Name = "delete me";
            await _mainContext.AddAsync(deployable);
        }
        else
        {
            _mapper.Map(deployableView, deployable);
            // TODO: This may not be needed
            _mainContext.Check(deployable);
            _mainContext.Mark(deployable);
        }

        await _mainContext.SaveChangesAsync();

        return _mapper.Map<DeployableDto, DeployableView>(deployable);
    }

    public async Task<DeployableView> GetDeployable(Guid id)
    {
        var deployable = await _mainContext.Deployables.FirstOrDefaultAsync(x => x.ThingId == id);

        if (deployable == null)
        {
            return new DeployableView()
            {
                Contents = new Deployable(),
                ThingId = id
            };
        }
        else
        {
            return _mapper.Map<DeployableDto, DeployableView>(deployable);
        }
    }

    /// <summary>
    /// Start a pipeline job with the given source reference
    /// </summary>
    /// <param name="id">id of the thing (not the deployable)</param>
    /// <param name="reference">the source reference</param>
    /// TODO: Add the job viewmodel and DTO stuff here
    public async Task StartJob(Guid id, string reference)
    {
        
    }
}