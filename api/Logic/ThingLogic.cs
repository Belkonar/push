using shared.View;

namespace api.Logic;

public class ThingLogic
{
    public async Task<List<ThingView>> GetThings(Guid org)
    {
        throw new NotImplementedException();
    }
    
    public async Task<ThingView> GetThing(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<ThingView> CreateThing(ThingView thingView)
    {
        throw new NotImplementedException();
    }

    public async Task<ThingView> UpdateThing(Guid id, ThingView thingView)
    {
        throw new NotImplementedException();
    }
}