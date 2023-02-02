using System.Text.Json;
using System.Text.Json.Serialization;

namespace shared.Models.Pipeline;

public class StageStep
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonExtensionData]
    public IDictionary<string, JsonElement> Parameters { get; set; }
}