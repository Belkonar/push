using System.Text.Json.Serialization;
using data.Models;

namespace data.View;

public class DeployableView
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonPropertyName("contents")]
    public Deployable Contents { get; set; }
}