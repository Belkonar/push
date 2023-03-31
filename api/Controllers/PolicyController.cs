using api.Logic;
using shared.UpdateModels;
using Microsoft.AspNetCore.Mvc;
using shared.Models;

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

    [HttpGet("{key}")]
    public async Task<Policy> GetOne([FromRoute] string key)
    {
        return await _policyLogic.GetOne(key);
    }
    
    [HttpPut]
    public async Task<Policy> Update([FromBody] Policy policy)
    {
        return await _policyLogic.Update(policy);
    }

    [HttpPut("sync")]
    public async Task Sync([FromBody] List<Policy> policies)
    {
        await _policyLogic.Sync(policies);
    }

    // TODO: Delete policy endpoint
}
