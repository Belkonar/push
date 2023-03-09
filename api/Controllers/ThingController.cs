using api.Logic;
using Microsoft.AspNetCore.Mvc;
using shared.UpdateModels;
using shared.View;

namespace api.Controllers;

// TODO: Split this out into multiple methods like orgs so that private metadata can be properly handled
[ApiController]
[Route("[controller]")]
public class ThingController : ControllerBase
{
    private readonly ThingLogic _thingLogic;

    public ThingController(ThingLogic thingLogic)
    {
        _thingLogic = thingLogic;
    }
    
    // TODO: Maybe add a method to get stuff by org, but for now it's not needed
    // TODO: Add query string to filter list, maybe even paging stuff
    [HttpGet]
    public async Task<List<ThingView>> GetThings()
    {
        return await _thingLogic.GetThings();
    }
    
    [HttpGet("{id}")]
    public async Task<ThingView> GetThing([FromRoute] Guid id)
    {
        return await _thingLogic.GetThing(id);
    }

    [HttpPost]
    public async Task<ThingView> CreateThing([FromBody] UpdateThing thingView)
    {
        return await _thingLogic.CreateThing(thingView);
    }
    
    [HttpPut("{id}")]
    public async Task<ThingView> UpdateThing([FromRoute] Guid id, [FromBody] ThingView thingView)
    {
        return await _thingLogic.UpdateThing(id, thingView);
    }

    [HttpGet("{id}/deployable")]
    public async Task<DeployableView> GetDeployable([FromRoute] Guid id)
    {
        return await _thingLogic.GetDeployable(id);
    }

    [HttpPut("{id}/deployable")]
    public async Task<DeployableView> UpdateDeployable([FromRoute] Guid id, DeployableView deployableView)
    {
        return await _thingLogic.UpdateDeployable(id, deployableView);
    }
    
    [HttpPost("{id}/deployable/start/{reference}")]
    public async Task<JobView> StartDeployment([FromRoute] Guid id, [FromRoute] string reference)
    {
        return await _thingLogic.StartDeployment(id, reference);
    }
}