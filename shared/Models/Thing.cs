using System.Text.Json.Serialization;

namespace shared.Models;

public class Thing
{
    [JsonPropertyName("id")]
    public Guid? Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("organization")]
    public Guid OrganizationId { get; set; }
    
    [JsonPropertyName("metadata")]
    public Dictionary<string, string> Metadata { get; set; } = new();

    [JsonPropertyName("privateMetadata")]
    public Dictionary<string, string> PrivateMetadata { get; set; } = new();
    
    [JsonPropertyName("deployable")]
    public Deployable? Deployable { get; set; }
}