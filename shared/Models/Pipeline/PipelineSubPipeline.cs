namespace shared.Models.Pipeline;

public class PipelineSubPipeline
{
    public Guid PipelineId { get; set; }
    public string PipelineConstraint { get; set; }

    /// <summary>
    /// The root of the sub pipeline, added as an ENV
    /// </summary>
    public string Root { get; set; } = "";

    /// <summary>
    /// The sub directory to use if you need to isolate stuff.
    /// </summary>
    public string WorkingDirectory { get; set; } = "";
    
    public Dictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();
}