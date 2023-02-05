using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

// use this with `using Pipeline = shared.Models.Pipeline;` to handle collisions and make it cleaner
namespace shared.Models.Pipeline;

public class PipelineVersion
{
    [MinLength(1)]
    [JsonPropertyName("stages")]
    public List<Stage> Stages { get; set; } = new ();

    [MinLength(1)]
    [JsonPropertyName("steps")]
    public List<Step> Steps { get; set; } = new();
    
    [JsonPropertyName("files")]
    public List<PipelineFile> Files { get; set; } = new();
    
    [JsonPropertyName("parameters")]
    public List<StepParameter> Parameters { get; set; } = new();
}