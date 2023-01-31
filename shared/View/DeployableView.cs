using System.Text.Json.Serialization;
using shared.Models;

namespace shared.View;

public class DeployableView
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonPropertyName("contents")]
    public Deployable Contents { get; set; }
}