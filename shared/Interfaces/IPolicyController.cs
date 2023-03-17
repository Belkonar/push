using shared.Models;
using shared.UpdateModels;

namespace shared.Interfaces;

public interface IPolicyController
{
    public Task<List<Policy>> GetAll();
    public Task<Policy> Update(string key, UpdatePolicy policy);
    public Task<Policy> Create(string key);
}
