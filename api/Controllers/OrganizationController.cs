using AutoMapper;
using data.ORM;
using data.View;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[Route("[controller]")]
public class OrganizationController : ControllerBase
{
    private readonly IMapper _mapper;

    public OrganizationController(IMapper mapper)
    {
        _mapper = mapper;
    }
    
    // get orgs
    [HttpGet()]
    public List<OrganizationView> GetOrgs()
    {
        var dto = new OrganizationDto()
        {
            Id = Guid.NewGuid(),
            Name = "test",
            Created = DateTime.Now,
            Updated = DateTime.Today
        };

        return new List<OrganizationView>()
        {
            _mapper.Map<OrganizationView>(dto)
        };
    }
}