using System.Text.Json.Serialization;

namespace shared.Models;

public class Deployable
{
    [JsonPropertyName("pipelineId")]
    public Guid? PipelineId { get; set; }

    [JsonPropertyName("pipelineConstraint")]
    public string PipelineConstraint { get; set; } = "";
    
    [JsonPropertyName("sourceControlUri")]
    public string SourceControlUri { get; set; } = "";
    
    [JsonPropertyName("variables")]
    public Dictionary<string, string> Variables { get; set; } = new();
}