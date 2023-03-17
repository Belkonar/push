using shared.Models;
using shared.Models.Job;
using shared.UpdateModels;

namespace shared.Interfaces;

public interface IThingController
{
    Task<List<Thing>> GetThings();
    Task<Thing> GetThing(Guid id);
    Task<Thing> CreateThing(UpdateThing thingView);
    Task<Thing> UpdateThing(Guid id, Thing thingView);
    Task<Deployable> GetDeployable(Guid id);
    Task<Deployable> UpdateDeployable(Guid id, Deployable deployableView);
    Task<Job> StartDeployment(Guid id, string reference);
    Task UpdateInternalData(Guid id, string key, SimpleValue value);
}