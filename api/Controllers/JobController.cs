using api.Logic;
using Microsoft.AspNetCore.Mvc;
using shared.Models.Job;
using shared.View;

namespace api.Controllers;

[ApiController]
[Route("[controller]")]
public class JobController : ControllerBase
{
    private readonly JobLogic _logic;

    public JobController(JobLogic logic)
    {
        _logic = logic;
    }
    
    [HttpGet()]
    public async Task<List<Job>> GetJobByStatus([FromQuery] string status)
    {
        return await _logic.GetJobByStatus(status);
    }
}