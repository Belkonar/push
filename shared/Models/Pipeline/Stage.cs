using System.Text.Json.Serialization;

namespace shared.Models.Pipeline;

public class Stage
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonPropertyName("steps")]
    public List<StageStep> Steps { get; set; } = new();
}