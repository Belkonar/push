using System.Text.Json.Serialization;

namespace shared.Models.Pipeline;

public class PipelineVersionKey
{
    [JsonPropertyName("version")]
    public string Version { get; set; }
    
    [JsonPropertyName("pipeline")]
    public Guid PipelineId { get; set; }
}