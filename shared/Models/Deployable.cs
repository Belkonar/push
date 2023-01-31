using System.Text.Json.Serialization;

namespace shared.Models;

public class Deployable
{
    [JsonPropertyName("pipelineId")]
    public Guid? PipelineId { get; set; }
    
    [JsonPropertyName("pipelineVersionId")]
    public Guid? PipelineVersionId { get; set; }
    
    [JsonPropertyName("sourceControlUri")]
    public string SourceControlUri { get; set; }
    
    [JsonPropertyName("variables")]
    public Dictionary<string, string> Variables { get; set; } = new();
}