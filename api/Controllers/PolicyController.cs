using api.Logic;
using data.ORM;
using data.UpdateModels;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<List<PolicyDto>> GetAll()
    {
        return await _policyLogic.GetAll();
    }

    [HttpPut("{key}")]
    public async Task<PolicyDto> Update([FromRoute] string key, [FromBody] UpdatePolicy policy)
    {
        return await _policyLogic.Update(key, policy);
    }
}
