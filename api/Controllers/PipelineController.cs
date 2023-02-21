using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[Route("[controller]")]
public class PipelineController
{
    // Get pipelines, optionally by org
    // GET /?org=
    
    // Get a single pipeline
    // GET /{id}
    
    // Get the latest major version constraint
    // GET /{id}/latest
    
    // Get the versions of a pipeline (this is a string array)
    // GET /{id}/versions
    
    // Get the most recent version that satisfies the constraint
    // GET /{id}/version/{constraint}
    
    // Create a new pipeline
    // POST /
    
    // Update a pipeline
    // PUT /{id}
    
    // Create/Update a version
    // This is technically an upsert but only if the version is a dev version
    // TODO: If it's a non-major change, check the params don't change
    // POST /{id}/version/{key}
}