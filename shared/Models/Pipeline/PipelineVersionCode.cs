using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

// use this with `using Pipeline = shared.Models.Pipeline;` to handle collisions and make it cleaner
namespace shared.Models.Pipeline;

public class PipelineVersionCode
{
    public bool CreateDeployment { get; set; } = true;
    
    public List<Stage> Stages { get; set; } = new();

    public List<Step> Steps { get; set; } = new();
    
    public List<PipelineFile> Files { get; set; } = new();
    
    public List<StepParameter> Parameters { get; set; } = new();

    public List<PipelineSubPipeline> SubPipelines { get; set; } = new();
}

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