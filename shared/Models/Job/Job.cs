using shared.Models.Pipeline;

namespace shared.Models.Job;

/*
 * Job statuses are
 * pending, ready, active, done, error
 */

public class Job
{
    public Guid Id { get; set; }
    
    public DateTime Created { get; set; }
    
    public Guid ThingId { get; set; }
    
    // To prevent needing a join (this however means it can get out of date)
    public string ThingName { get; set; }

    public string Status { get; set; } = "pending";
    
    public string StatusReason { get; set; } = "";
    
    public List<JobStage> Stages { get; set; } = new();

    public List<JobStep> Steps { get; set; } = new();
    
    /// <summary>
    /// This field is solely used right now for file replacements
    /// </summary>
    public List<JobStepParameter> Parameters { get; set; } = new();
    
    public string SourceControlUri { get; set; }
    
    public string SourceReference { get; set; }
    
    public List<PipelineFile> Files { get; set; } = new();

    public PipelineVersionKey PipelineVersion { get; set; }

    public List<JobFeature> Features { get; set; } = new();
}