using shared.Models;
using shared.UpdateModels;
using shared.Models.Job;

namespace shared.Interfaces;

public interface IThingController
{
    public Task<List<Thing>> GetThings();
    public Task<Thing> GetThing(Guid id);
    public Task<Thing> CreateThing(UpdateThing thingView);
    public Task<Thing> UpdateThing(Guid id, Thing thingView);
    public Task<Deployable> GetDeployable(Guid id);
    public Task<Deployable> UpdateDeployable(Guid id, Deployable deployableView);
    public Task<Job> StartDeployment(Guid id, string reference);
    public Task UpdateInternalData(Guid id, string key, SimpleValue value);
}
