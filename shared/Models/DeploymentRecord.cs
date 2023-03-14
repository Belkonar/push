using MongoDB.Bson;
using shared.Models.Job;
using shared.Models.Pipeline;

namespace shared.Models;

public class DeploymentRecord
{
    // may change this later
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public DateTime Created { get; set; } = DateTime.Now;
    
    public string SourceReference { get; set; }
    
    public Guid ThingId { get; set; }
    public Guid JobId { get; set; }
    
    public PipelineVersionKey PipelineVersion { get; set; }

    public List<JobFeature> Features { get; set; } = new List<JobFeature>();
}