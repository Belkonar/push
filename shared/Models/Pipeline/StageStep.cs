using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace shared.Models.Pipeline;

public class StageStep
{
    [Required]
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [Required]
    [JsonPropertyName("step")]
    public string Step { get; set; }
    
    [JsonExtensionData]
    public IDictionary<string, JsonElement> Parameters { get; set; }
}