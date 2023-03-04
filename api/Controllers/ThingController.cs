using api.Logic;
using Microsoft.AspNetCore.Mvc;
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
    
    [HttpGet]
    public async Task<List<ThingView>> GetThings([FromQuery] Guid org)
    {
        return await _thingLogic.GetThings(org);
    }
    
    [HttpGet("{id}")]
    public async Task<ThingView> GetThing([FromRoute] Guid id)
    {
        return await _thingLogic.GetThing(id);
    }

    [HttpPost]
    public async Task<ThingView> CreateThing([FromBody] ThingView thingView)
    {
        return await _thingLogic.CreateThing(thingView);
    }
    
    [HttpPut("{id}")]
    public async Task<ThingView> UpdateThing([FromRoute] Guid id, [FromBody] ThingView thingView)
    {
        return await _thingLogic.UpdateThing(id, thingView);
    }
}