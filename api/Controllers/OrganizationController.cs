using api.Logic;
using AutoMapper;
using data.ORM;
using data.UpdateModels;
using data.View;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[Route("[controller]")]
public class OrganizationController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly OrganizationLogic _organizationLogic;

    public OrganizationController(IMapper mapper, OrganizationLogic organizationLogic)
    {
        _mapper = mapper;
        _organizationLogic = organizationLogic;
    }
    
    // get orgs
    [HttpGet()]
    public async Task<List<OrganizationView>> GetOrgs()
    {
        return await _organizationLogic.GetAll();
    }

    [HttpPost]
    public async Task<OrganizationView> Create([FromBody] UpdateOrganization body)
    {
        return await _organizationLogic.Create(body);
    }

    [HttpPut("{id}")]
    public async Task<OrganizationView> Update([FromRoute] Guid id, [FromBody] UpdateOrganization body)
    {
        return await _organizationLogic.Update(id, body);
    }

    [HttpPut("{id}/metadata")]
    public async Task<OrganizationView> UpdateMetadata([FromRoute] Guid id, [FromBody] Dictionary<string,string> body)
    {
        return await _organizationLogic.UpdateMetadata(id, body);
    }
    
    [HttpPut("{id}/private_metadata")]
    public async Task<OrganizationView> UpdatePrivateMetadata([FromRoute] Guid id, [FromBody] Dictionary<string,string> body)
    {
        return await _organizationLogic.UpdatePrivateMetadata(id, body);
    }
    
    [HttpPut("{id}/policy")]
    public async Task<OrganizationView> UpdatePolicy([FromRoute] Guid id, [FromBody] string body)
    {
        return await _organizationLogic.UpdatePolicy(id, body);
    }
}