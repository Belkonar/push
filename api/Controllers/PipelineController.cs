using api.Logic;
using Microsoft.AspNetCore.Mvc;
using shared.View;

namespace api.Controllers;

[ApiController]
[Route("[controller]")]
public class PipelineController
{
    private readonly PipelineLogic _pipelineLogic;

    public PipelineController(PipelineLogic pipelineLogic)
    {
        _pipelineLogic = pipelineLogic;
    }
    
    // Get pipelines, optionally by org
    [HttpGet]
    public async Task<List<PipelineView>> GetPipelines([FromQuery] Guid? org)
    {
        return await _pipelineLogic.GetPipelines(org);
    }
    
    // Get a single pipeline
    [HttpGet("{id}")]
    public async Task<PipelineView> GetPipeline([FromRoute]Guid id)
    {
        return await _pipelineLogic.GetPipeline(id);
    }
    
    // Get the latest major version constraint
    [HttpGet("{id}/latest")]
    public async Task<string> GetLatestMajor([FromRoute] Guid id)
    {
        return await _pipelineLogic.GetLatestMajor(id);
    }
    
    // Get the versions of a pipeline (this is a string array)
    [HttpGet("{id}/versions")]
    public async Task<List<string>> GetVersions([FromRoute] Guid id)
    {
        return await _pipelineLogic.GetVersions(id);
    }
    
    // Get the most recent version that satisfies the constraint
    [HttpGet("{id}/version/{constraint}")]
    public async Task<PipelineVersionView> GetVersionByConstraint([FromRoute] Guid id, [FromRoute] string constraint)
    {
        return await _pipelineLogic.GetVersionByConstraint(id, constraint);
    }
    
    // Create a new pipeline
    [HttpPost]
    public async Task<PipelineView> CreatePipeline([FromBody] PipelineView data)
    {
        return await _pipelineLogic.CreatePipeline(data);
    }
    
    // Update a pipeline
    [HttpPut("{id}")]
    public async Task<PipelineView> UpdatePipeline([FromRoute] Guid id, [FromBody] PipelineView data)
    {
        return await _pipelineLogic.UpdatePipeline(id, data);
    }

    // Create/Update a version
    // This is technically an upsert but only if the version is a dev version
    // TODO: If it's a non-major change, check the params don't change, since the last version
    // Also check that the 
    [HttpPost("{id}/version/{key}")]
    public async Task<PipelineVersionView> UpdatePipelineVersion([FromRoute] Guid id, [FromRoute] string key, [FromBody] PipelineVersionView data)
    {
        return await _pipelineLogic.UpdatePipelineVersion(id, key, data);
    }
}