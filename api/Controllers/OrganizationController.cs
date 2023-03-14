using api.Logic;
using shared.UpdateModels;
using shared.View;
using Microsoft.AspNetCore.Mvc;
using shared.Models;

namespace api.Controllers;

[ApiController]
[Route("[controller]")]
public class OrganizationController : ControllerBase
{
    private readonly OrganizationLogic _organizationLogic;

    public OrganizationController(OrganizationLogic organizationLogic)
    {
        _organizationLogic = organizationLogic;
    }
    
    // get orgs
    [HttpGet()]
    public async Task<List<Organization>> GetOrgs()
    {
        return await _organizationLogic.GetAll();
    }
    
    [HttpGet("{id}")]
    public async Task<Organization> GetOrg([FromRoute] Guid id)
    {
        return await _organizationLogic.Get(id);
    }

    [HttpPost]
    public async Task<Organization> Create([FromBody] UpdateOrganization body)
    {
        return await _organizationLogic.Create(body);
    }

    [HttpPut("{id}")]
    public async Task<Organization> Update([FromRoute] Guid id, [FromBody] UpdateOrganization body)
    {
        return await _organizationLogic.Update(id, body);
    }

    [HttpPut("{id}/metadata")]
    public async Task<Organization> UpdateMetadata([FromRoute] Guid id, [FromBody] Dictionary<string,string> body)
    {
        return await _organizationLogic.UpdateMetadata(id, body);
    }
    
    [HttpPut("{id}/private_metadata")]
    public async Task<Organization> UpdatePrivateMetadata([FromRoute] Guid id, [FromBody] Dictionary<string,string> body)
    {
        return await _organizationLogic.UpdatePrivateMetadata(id, body);
    }
    
    [HttpPut("{id}/policy")]
    public async Task<Organization> UpdatePolicy([FromRoute] Guid id, [FromBody] string body)
    {
        return await _organizationLogic.UpdatePolicy(id, body);
    }

    /// <summary>
    /// While this isn't technically correct from a REST perspective it makes it cleaner
    /// </summary>
    /// <param name="id"></param>
    /// <param name="variable"></param>
    /// <returns></returns>
    [HttpPut("{id}/variable")]
    public async Task<IActionResult> UpdateVariable([FromRoute] Guid id, [FromBody] UpdateOrganizationVariable variable)
    {
        await _organizationLogic.UpdateVariable(id, variable);
        
        return Ok();
    }
    
    [HttpGet("{id}/credential")]
    public async Task<List<Credential>> GetCredentials([FromRoute] Guid id)
    {
        return await _organizationLogic.GetCredentials(id);
    }

    [HttpPost("{id}/credential")]
    public async Task<Credential> CreateCredential([FromRoute] Guid id, [FromBody] Credential credential)
    {
        return await _organizationLogic.CreateCredential(id, credential);
    }

    [HttpGet("credential/{id}")]
    public async Task<Credential> GetCredential([FromRoute] Guid id)
    {
        return await _organizationLogic.GetCredential(id);
    }

    [HttpGet("credential/{id}/bundle")]
    public async Task<CredentialBundle> GetCredentialBundle([FromRoute] Guid id)
    {
        return await _organizationLogic.GetCredentialBundle(id);
    }

    [HttpPut("credential/{id}")]
    public async Task UpdateCredentialData([FromRoute] Guid id, [FromBody] Dictionary<string, string> data)
    {
        await _organizationLogic.UpdateCredentialData(id, data);
    }
}