using api.Logic;
using shared.UpdateModels;
using Microsoft.AspNetCore.Mvc;
using shared.View;

namespace api.Controllers;

[ApiController, Route("[controller]")]
public class PolicyController : ControllerBase
{
    private readonly PolicyLogic _policyLogic;

    public PolicyController(PolicyLogic policyLogic)
    {
        _policyLogic = policyLogic;
    }

    [HttpGet]
    public async Task<List<Policy>> GetAll()
    {
        return await _policyLogic.GetAll();
    }

    /// <summary>
    /// Update an existing policy
    /// </summary>
    /// <requires>(G).global_policy_manage</requires>
    /// <param name="key"></param>
    /// <param name="policy"></param>
    /// <returns></returns>
    [HttpPut("{key}")]
    public async Task<Policy> Update([FromRoute] string key, [FromBody] UpdatePolicy policy)
    {
        return await _policyLogic.Update(key, policy);
    }
    
    /// <summary>
    /// Create a new empty policy
    /// </summary>
    /// <requires>(G).global_policy_manage</requires>
    /// <param name="key"></param>
    /// <returns></returns>
    [HttpPost("{key}")]
    public async Task<Policy> Create([FromRoute] string key)
    {
        return await _policyLogic.Create(key);
    }
    
    // TODO: Delete policy endpoint
}
