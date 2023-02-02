using System.Text.Json.Serialization;

namespace shared.View;

public class PipelineView
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonPropertyName("description")]
    public string Description { get; set; }
    
    [JsonPropertyName("organization")]
    public Guid? OrganizationId { get; set; }
}