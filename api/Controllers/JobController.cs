using api.Logic;
using Microsoft.AspNetCore.Mvc;
using shared;
using shared.Models;
using shared.Models.Job;
using shared.UpdateModels;

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

    [HttpGet("safe")]
    public async Task<List<Job>> GetSafeJobs([FromQuery] Guid? id)
    {
        return await _logic.GetSafeJobs(id);
    }

    [HttpGet("{id}")]
    public async Task<Job> GetJob([FromRoute] Guid id)
    {
        return await _logic.GetJob(id);
    }
    
    [HttpGet("{id}/safe")]
    public async Task<Job> GetSafeJob([FromRoute] Guid id)
    {
        return await _logic.GetSafeJob(id);
    }

    [HttpPost("{id}/status")]
    public async Task UpdateStatus([FromRoute] Guid id, [FromBody] UpdateStatus status)
    {
        await _logic.UpdateStatus(id, status);
    }

    [HttpPost("{id}/step/{ordinal}/status")]
    public async Task UpdateStepStatus([FromRoute] Guid id, [FromRoute] int ordinal, [FromBody] UpdateStatus status)
    {
        await _logic.UpdateStepStatus(id, ordinal, status);
    }
    
    [HttpGet("{id}/step/{ordinal}/output")]
    public async Task<SimpleValue> GetStepOutput([FromRoute] Guid id, [FromRoute] int ordinal)
    {
        return await _logic.GetStepOutput(id, ordinal);
    }

    [HttpPut("{id}/step/{ordinal}/output")]
    public async Task UpdateStepOutput([FromRoute] Guid id, [FromRoute] int ordinal, [FromBody] SimpleValue output)
    {
        await _logic.UpdateStepOutput(id, ordinal, output);
    }

    [HttpPost("{id}/approve/{ordinal}")]
    public async Task ApproveStep([FromRoute] Guid id, [FromRoute] int ordinal)
    {
        await _logic.ApproveStep(id, ordinal);
    }

    [HttpPost("{id}/step/{ordinal}/finalize")]
    public async Task FinalizeStep([FromRoute] Guid id, [FromRoute] int ordinal, [FromBody] ExecutorResponse response)
    {
        await _logic.FinalizeStep(id, ordinal, response);
    }

    [HttpPost("{id}/feature")]
    public async Task AddFeature(Guid id, JobFeature feature)
    {
        await _logic.AddFeature(id, feature);
    }
}