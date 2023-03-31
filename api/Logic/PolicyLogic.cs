using MongoDB.Bson;
using MongoDB.Driver;
using shared.Models;
using shared.UpdateModels;

namespace api.Logic;

// TODO: Rewire this to use policy-engine
public class PolicyLogic
{
    public async Task<List<Policy>> GetAll()
    {
        throw new NotImplementedException();
    }

    public async Task<string> GetByName(string name)
    {
        throw new NotImplementedException();
    }

    public async Task<Policy> Update(string key, Policy policy)
    {
        throw new NotImplementedException();
    }
    
    public async Task<Policy> Create(string key)
    {
        throw new NotImplementedException();
    }

    public async Task Delete(string key)
    {
        throw new NotImplementedException();
    }
}