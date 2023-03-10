using api.Logic;
using Microsoft.AspNetCore.Mvc;
using shared.Models.Job;
using shared.UpdateModels;
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

    [HttpGet("{id}")]
    public async Task<Job> GetJob([FromRoute] Guid id)
    {
        return await _logic.GetJob(id);
    }

    [HttpPost("{id}/status")]
    public async Task UpdateStatus([FromRoute] Guid id, [FromBody] UpdateStatus status)
    {
        await _logic.UpdateStatus(id, status);
    }

    [HttpPost("{id}/step/{ordinal}")]
    public async Task UpdateStepStatus([FromRoute] Guid id, [FromRoute] int ordinal, [FromBody] UpdateStatus status, [FromQuery] bool updateJob = false)
    {
        await _logic.UpdateStepStatus(id, ordinal, status, updateJob);
    }
}