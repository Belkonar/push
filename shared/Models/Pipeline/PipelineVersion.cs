using System.Text.Json.Serialization;

// use this with `using Pipeline = shared.Models.Pipeline;` to handle collisions and make it cleaner
namespace shared.Models.Pipeline;

public class PipelineVersion
{
    [JsonPropertyName("stages")]
    public List<Stage> Stages { get; set; } = new ();

    [JsonPropertyName("steps")]
    public List<Step> Steps { get; set; } = new();
    
    [JsonPropertyName("files")]
    public List<PipelineFile> Files { get; set; } = new();
    
    [JsonPropertyName("parameters")]
    public List<StepParameter> Parameters { get; set; } = new();
}