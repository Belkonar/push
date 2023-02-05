using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace shared.Models.Pipeline;

public class Stage
{
    [Required]
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [MinLength(1)]
    [JsonPropertyName("steps")]
    public List<StageStep> Steps { get; set; } = new();
}