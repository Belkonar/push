using System.Text.Json.Serialization;
using shared.Models.Pipeline;

namespace shared.View;

public class PipelineVersionView : BaseView
{
    // This is not generated but user supplied, semver for real versions
    // and anything else for dev versions
    [JsonPropertyName("version")]
    public string Version { get; set; }
    
    [JsonPropertyName("pipeline")]
    public Guid PipelineId { get; set; }
    
    [JsonPropertyName("contents")]
    public PipelineVersionContents Contents { get; set; }
}